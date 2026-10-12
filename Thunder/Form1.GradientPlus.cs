using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;

namespace Thunder
{
    public partial class mainWindow
    {
        // Gradient 加強：曲線、起點色（雙色漸層）、放射狀、飽和度階梯----------------------------------------------------------
        private ComboBox tabigCurve_cmb;
        private CheckBox tabigFrom_chk, tabigSaturation_chk;
        private Button tabigFrom_btn;
        private RadioButton tabigRadial_rdo;
        private static readonly string[] GradientCurves = { "線性", "Gamma 2.2", "反 Gamma" };
        private static readonly string[] GradientCurveTokens = { "", "g22", "ig22" };  // config 選項中的寫法
        private int GradientCurve => tabigCurve_cmb?.SelectedIndex ?? 0;
        private Color? GradientFromColor => tabigFrom_chk != null && tabigFrom_chk.Checked && tabigFrom_chk.Enabled ? tabigFrom_btn.BackColor : (Color?)null;
        private bool GradientRadial => tabigRadial_rdo != null && tabigRadial_rdo.Checked;
        private void InitGradientPlus()
        {
            // 方向列多一個「放射」；右側「全階 / 階數」下方放曲線、起點色、飽和度階梯
            tabigRadial_rdo = new RadioButton { Text = "放射", AutoSize = true, Location = new Point(205, 15) };
            tabimgeGradient.Controls.Add(tabigRadial_rdo);
            tabigRadial_rdo.BringToFront();
            var group = new GroupBox { Text = "曲線 / 起點 / 飽和度", Location = new Point(445, 115), Size = new Size(175, 112) };
            tabigCurve_cmb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(50, 20), Width = 115, ImeMode = ImeMode.Disable };
            tabigCurve_cmb.Items.AddRange(GradientCurves);
            tabigCurve_cmb.SelectedIndex = 0;
            tabigFrom_chk = new CheckBox { Text = "起點色", AutoSize = true, Location = new Point(10, 53) };
            tabigFrom_btn = new Button { BackColor = Color.Black, Bounds = new Rectangle(95, 50, 40, 24), FlatStyle = FlatStyle.Flat };
            tabigFrom_btn.Click += (s, e) =>
            {
                using (var dialog = new ColorDialog { Color = tabigFrom_btn.BackColor, FullOpen = true })
                {
                    if (dialog.ShowDialog(this) == DialogResult.OK) { tabigFrom_btn.BackColor = dialog.Color; tabigFrom_chk.Checked = true; }
                }
            };
            tabigSaturation_chk = new CheckBox { Text = "飽和度階梯", AutoSize = true, Location = new Point(10, 83) };
            group.Controls.AddRange(new Control[] { new Label { Text = "曲線", AutoSize = true, Location = new Point(10, 24) }, tabigCurve_cmb, tabigFrom_chk, tabigFrom_btn, tabigSaturation_chk });
            tabimgeGradient.Controls.Add(group);
            group.BringToFront();
            toolTip.SetToolTip(tabigRadial_rdo, "由畫面中心往外的圓形漸層（起始灰階在中心）；色階斷層會變成同心環");
            toolTip.SetToolTip(tabigCurve_cmb, "每一階的分布：\r\n線性：code 等距\r\nGamma 2.2：code = t^(1/2.2)，螢幕亮度（線性光）等距增加，亮部階數較密\r\n反 Gamma：code = t^2.2，暗部階數較密");
            toolTip.SetToolTip(tabigFrom_chk, "雙色漸層：起點從黑色改成這個顏色（終點為基底色彩），例如紅 → 綠；使用其他顏色（ColorBar）時不能用");
            toolTip.SetToolTip(tabigFrom_btn, "選擇起點色");
            toolTip.SetToolTip(tabigSaturation_chk, "飽和度階梯：R / G / B / C / M / Y 各一排，從灰（0%）到純色（100%）\r\n亮度為結束灰階，階數為漸層分幾階，方向依 V / H；勾選「顯示 > 依漸層條件」時標示百分比");
            void Update()
            {
                // 放射狀不使用畫面等分與反向；飽和度階梯不使用起點色與其他顏色
                tabigFrom_chk.Enabled = tabigFrom_btn.Enabled = !tabigOther_chk.Checked && !tabigSaturation_chk.Checked && !tabigColorBar_rdo.Checked;
                bool reverseUsable = int.TryParse(tabigDivid_cmb.Text, out int divisions) && divisions >= 2 && !GradientRadial;  // 與 InitGradientReverse 相同，另外排除放射狀
                tabigReverse_chk.Enabled = reverseUsable;
                if (!reverseUsable) tabigReverse_chk.Checked = false;
                tabigDivid_cmb.Enabled = !AllLevelGrid && !GradientRadial;
            }
            foreach (var check in new CheckBox[] { tabigOther_chk, tabigSaturation_chk }) check.CheckedChanged += (s, e) => Update();
            foreach (var radio in new[] { tabigRadial_rdo, tabigColorBar_rdo, tabigHWay_rdo, tabigVWay_rdo }) radio.CheckedChanged += (s, e) => Update();
            tabigDivid_cmb.TextChanged += (s, e) => Update();
            Update();
        }  // 建立 Gradient 加強
        private static byte CurveLevel(int from, int to, int stepIndex, int steps, int curve)
        {
            // 第 stepIndex 階（0 ~ steps-1）的數值，依曲線分布
            double t = steps > 1 ? Math.Max(0, Math.Min(1, stepIndex / (double)(steps - 1))) : 0;
            double c = CurveValue(t, curve);
            return (byte)Math.Round(from + (to - from) * c);
        }  // 曲線取值
        private static double CurveValue(double t, int curve) => curve == 1 ? Math.Pow(t, 1 / 2.2) : curve == 2 ? Math.Pow(t, 2.2) : t;  // 0~1 依曲線分布
        private Bitmap ApplyRadialTransparency(Bitmap bitmap, int startAlpha, int endAlpha, int steps, int curve)
        {
            // 放射狀：以中心到角落的距離分階，中心為起始灰階
            int width = bitmap.Width, height = bitmap.Height;
            double cx = width / 2.0, cy = height / 2.0, max = Math.Sqrt(cx * cx + cy * cy);
            steps = Math.Max(1, Math.Min(steps, (int)max));
            var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            try
            {
                var row = new byte[width * 4];
                for (int y = 0; y < height; y++)
                {
                    System.Runtime.InteropServices.Marshal.Copy(data.Scan0 + y * data.Stride, row, 0, row.Length);
                    for (int x = 0; x < width; x++)
                    {
                        double dx = x + 0.5 - cx, dy = y + 0.5 - cy;
                        int step = Math.Min(steps - 1, (int)(Math.Sqrt(dx * dx + dy * dy) / max * steps));
                        row[x * 4 + 3] = curve == 0 && steps > 1 ? (byte)(startAlpha + (float)(endAlpha - startAlpha) / (steps - 1) * step) : CurveLevel(startAlpha, endAlpha, step, steps, curve);
                    }
                    System.Runtime.InteropServices.Marshal.Copy(row, 0, data.Scan0 + y * data.Stride, row.Length);
                }
            }
            finally
            {
                bitmap.UnlockBits(data);
            }
            return bitmap;
        }  // 放射狀透明度
        private static readonly (string name, int r, int g, int b)[] SaturationColors =
        {
            ("R", 255, 0, 0), ("G", 0, 255, 0), ("B", 0, 0, 255), ("C", 0, 255, 255), ("M", 255, 0, 255), ("Y", 255, 255, 0),
        };
        private Bitmap DrawSaturationLadder(int width, int height)
        {
            // 六色各一排：每一階從灰（飽和度 0%）往純色（100%），灰的亮度 = 結束灰階
            bool stepsDown = tabigHWay_rdo.Checked;  // H：階數往下變化、顏色左右排列
            int along = stepsDown ? height : width;
            int steps = Math.Max(2, Math.Min(GradientStepCount, along));
            int level = int.TryParse(tabigLastLevel_cmb.Text, out int l) ? Math.Max(0, Math.Min(255, l)) : 255;
            var bitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bitmap))
            using (Font font = new Font("Arial", Math.Max(10f, Math.Min(18f, along / (float)steps / 3)), GraphicsUnit.Pixel))
            {
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                for (int i = 0; i < SaturationColors.Length; i++)
                {
                    var (name, pr, pg, pb) = SaturationColors[i];
                    int across0 = (stepsDown ? width : height) * i / SaturationColors.Length, across1 = (stepsDown ? width : height) * (i + 1) / SaturationColors.Length;
                    for (int k = 0; k < steps; k++)
                    {
                        int a0 = along * k / steps, a1 = along * (k + 1) / steps;
                        double s = CurveValue(k / (double)(steps - 1), GradientCurve);  // 飽和度 0 ~ 1
                        int Mix(int pure) => (int)Math.Round(level * (1 - s) + pure * level / 255.0 * s);
                        Color color = Color.FromArgb(Mix(pr), Mix(pg), Mix(pb));
                        Rectangle cell = stepsDown ? Rectangle.FromLTRB(across0, a0, across1, a1) : Rectangle.FromLTRB(a0, across0, a1, across1);
                        using (Brush brush = new SolidBrush(color)) g.FillRectangle(brush, cell);
                        if (AllLevelGradient && cell.Width > font.Size * 3 && cell.Height > font.Size * 1.5f)
                        {
                            Brush text = color.R * 0.299 + color.G * 0.587 + color.B * 0.114 > 128 ? Brushes.Black : Brushes.White;
                            g.DrawString($"{(k == 0 ? name + " " : "")}{s * 100:0}%", font, text, cell.X + 3, cell.Y + 2);
                        }
                    }
                }
            }
            return bitmap;
        }  // 飽和度階梯
        private string GradientPlusTokens()
        {
            // 曲線與起點色寫在漸層選項後面，例：v-16-1-g22-fromred
            string tokens = GradientCurve > 0 ? "-" + GradientCurveTokens[GradientCurve] : "";
            if (GradientFromColor is Color from) tokens += "-from" + ConfigColorText(from);
            return tokens;
        }  // 加強選項轉 config
        private string TakeGradientPlusTokens(string option)
        {
            // 從漸層選項取出曲線（g22 / ig22）與起點色（from顏色），回傳其餘部分
            tabigCurve_cmb.SelectedIndex = 0;
            tabigFrom_chk.Checked = false;
            var kept = new System.Collections.Generic.List<string>();
            foreach (string token in option.Split('-'))
            {
                int curve = Array.IndexOf(GradientCurveTokens, token);
                if (token != "" && curve > 0) tabigCurve_cmb.SelectedIndex = curve;
                else if (token.StartsWith("from") && token.Length > 4)
                {
                    tabigFrom_btn.BackColor = switchcolor(token.Substring(4));
                    tabigFrom_chk.Checked = true;
                }
                else kept.Add(token);
            }
            return string.Join("-", kept);
        }  // 解析加強選項
    }
}
