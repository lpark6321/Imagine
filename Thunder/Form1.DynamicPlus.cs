using System;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;

namespace Thunder
{
    public partial class mainWindow
    {
        // Dynamic 加強：隨機方向、Overdrive 多灰階移動條、跑馬燈、長條條數與交錯顏色-----------------------------------------
        private RadioButton tabiedObjectOverdrive_rdo, tabiedObjectMarquee_rdo, tabiedMoveRandom_rdo;
        private NumericUpDown tabiedStripCount_nud;
        private CheckBox tabiedStripAlt_chk;
        private TextBox tabiedMarquee_txt;
        private const string DefaultMarqueeText = "PatternMagic 0123456789 ABCDEFG 跑馬燈";
        private static readonly int[] OverdriveLevels = { 0, 64, 128, 192 };  // Overdrive 背景橫帶的灰階
        private float _marqueeX, _marqueeWidth;
        private int _stripOffset;
        private void InitDynamicPlus()
        {
            // 物件種類多兩種、移動方向多「隨機」；右側一組放長條與跑馬燈的設定
            tabiedObjectOverdrive_rdo = new RadioButton { Text = "Overdrive 條", AutoSize = true, Location = new Point(15, 120) };
            tabiedObjectMarquee_rdo = new RadioButton { Text = "跑馬燈", AutoSize = true, Location = new Point(15, 145) };
            tabiedObject_grp.Controls.AddRange(new Control[] { tabiedObjectOverdrive_rdo, tabiedObjectMarquee_rdo });
            tabiedObject_grp.Height = 175;
            tabiedMoveRandom_rdo = new RadioButton { Text = "隨機", AutoSize = true, Location = new Point(15, 95) };
            tabiedMove_grp.Controls.Add(tabiedMoveRandom_rdo);
            tabiedMoveSpeed_nud.Top = 122;
            tabiedMove_grp.Height = 155;
            tabiedMoveSpeed_nud.Maximum = 512;  // 看撕裂需要更快的速度
            var group = new GroupBox { Text = "長條 / 跑馬燈", Location = new Point(260, 115), Size = new Size(300, 90) };
            tabiedStripCount_nud = new NumericUpDown { Minimum = 1, Maximum = 16, Value = 1, Bounds = new Rectangle(55, 20, 50, 23) };
            tabiedStripAlt_chk = new CheckBox { Text = "交錯顏色", AutoSize = true, Location = new Point(125, 22) };
            tabiedMarquee_txt = new TextBox { Text = DefaultMarqueeText, Bounds = new Rectangle(55, 54, 235, 23), ImeMode = ImeMode.NoControl };
            tabiedMarquee_txt.TextChanged += (s, e) =>
            {
                // config 用 - 與 , 分隔，文字不能有這兩個字元
                string clean = tabiedMarquee_txt.Text.Replace("-", " ").Replace(",", " ");
                if (clean != tabiedMarquee_txt.Text) tabiedMarquee_txt.Text = clean;
            };
            group.Controls.AddRange(new Control[]
            {
                new Label { Text = "條數", AutoSize = true, Location = new Point(10, 24) }, tabiedStripCount_nud, tabiedStripAlt_chk,
                new Label { Text = "文字", AutoSize = true, Location = new Point(10, 57) }, tabiedMarquee_txt,
            });
            tabimgeDynamic.Controls.Add(group);
            toolTip.SetToolTip(tabiedObjectOverdrive_rdo, "背景分成 0 / 64 / 128 / 192 四條灰階橫帶，同一個方塊在各條上水平移動，比較不同灰階轉換的拖影與反白鬼影");
            toolTip.SetToolTip(tabiedObjectMarquee_rdo, "一行文字由右往左捲動（字級 = 物件大小 px），看移動中的字能不能看清楚");
            toolTip.SetToolTip(tabiedMoveRandom_rdo, "碰到邊界時換一個隨機方向（速度不變）");
            toolTip.SetToolTip(tabiedStripCount_nud, "長條的條數：2 條以上時等距排列並一直往同方向捲動（不反彈），適合看撕裂");
            toolTip.SetToolTip(tabiedStripAlt_chk, "長條交錯使用物件色與它的反相色");
            toolTip.SetToolTip(tabiedMarquee_txt, "跑馬燈文字（不能有 - 與 ,）");
            void Update(object sender, EventArgs e)
            {
                // 長條、模糊條：不能斜角 / 隨機；Overdrive、跑馬燈：只能水平
                bool lineOnly = tabiedObjectStrip_rdo.Checked || tabiedObjectBlur_rdo.Checked;
                bool horizontalOnly = tabiedObjectOverdrive_rdo.Checked || tabiedObjectMarquee_rdo.Checked;
                tabiedMoveV_rdo.Enabled = !horizontalOnly;
                tabiedMoveBevel_rdo.Enabled = tabiedMoveRandom_rdo.Enabled = !lineOnly && !horizontalOnly;
                if ((lineOnly && (tabiedMoveBevel_rdo.Checked || tabiedMoveRandom_rdo.Checked)) || (horizontalOnly && !tabiedMoveH_rdo.Checked)) tabiedMoveH_rdo.Checked = true;
                tabiedStripCount_nud.Enabled = tabiedStripAlt_chk.Enabled = tabiedObjectStrip_rdo.Checked;
                tabiedMarquee_txt.Enabled = tabiedObjectMarquee_rdo.Checked;
            }
            // 在原本的處理之後才判斷（最後訂閱的最後執行）
            foreach (var radio in new[] { tabiedObjectBall_rdo, tabiedObjectCube_rdo, tabiedObjectStrip_rdo, tabiedObjectBlur_rdo, tabiedObjectOverdrive_rdo, tabiedObjectMarquee_rdo })
            {
                radio.CheckedChanged += Update;
            }
            Update(null, EventArgs.Empty);
        }  // 建立 Dynamic 加強
        private void ResetDynamicPlusOptions()
        {
            tabiedStripCount_nud.Value = 1;
            tabiedStripAlt_chk.Checked = false;
            tabiedMarquee_txt.Text = DefaultMarqueeText;
        }  // config 沒寫的選項恢復預設
        private bool TakeDynamicPlusToken(string raw, ref RadioButton shape)
        {
            // overdrive、marquee、n條數、alt、t:跑馬燈文字
            string token = raw.ToLower();
            if (token == "overdrive") shape = tabiedObjectOverdrive_rdo;
            else if (token == "marquee") shape = tabiedObjectMarquee_rdo;
            else if (token == "alt") tabiedStripAlt_chk.Checked = true;
            else if (token.StartsWith("n") && int.TryParse(token.Substring(1), out int count))
            {
                if (count < 1 || count > 16) throw new FormatException("長條條數 n 需為 1~16");
                tabiedStripCount_nud.Value = count;
            }
            else if (token.StartsWith("t:")) tabiedMarquee_txt.Text = raw.Substring(2);
            else return false;
            return true;
        }  // 解析加強選項
        private string DynamicPlusTokens()
        {
            string tokens = "";
            if (tabiedObjectStrip_rdo.Checked && tabiedStripCount_nud.Value > 1) tokens += $"-n{tabiedStripCount_nud.Value}";
            if (tabiedObjectStrip_rdo.Checked && tabiedStripAlt_chk.Checked) tokens += "-alt";
            if (tabiedObjectMarquee_rdo.Checked && tabiedMarquee_txt.Text.Trim() != "" && tabiedMarquee_txt.Text != DefaultMarqueeText) tokens += "-t:" + tabiedMarquee_txt.Text.Trim();
            return tokens;
        }  // 加強選項轉 config
        private void DrawDynamicBackgroundPlus(Graphics g, int width, int height)
        {
            // Overdrive：背景改成四條灰階橫帶，左側標示灰階
            if (!tabiedObjectOverdrive_rdo.Checked) return;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            using (Font font = new Font("Arial", Math.Max(12f, height / 50f), GraphicsUnit.Pixel))
            {
                for (int i = 0; i < OverdriveLevels.Length; i++)
                {
                    int level = OverdriveLevels[i], y0 = height * i / OverdriveLevels.Length, y1 = height * (i + 1) / OverdriveLevels.Length;
                    using (Brush brush = new SolidBrush(Color.FromArgb(level, level, level))) g.FillRectangle(brush, 0, y0, width, y1 - y0);
                    g.DrawString(level.ToString(), font, level < 128 ? Brushes.Gray : Brushes.Black, 6, y0 + 4);
                }
            }
        }  // Overdrive 背景
        private void ResetDynamicPlus(int width)
        {
            _marqueeX = width * 0.1f;  // 預覽時文字從畫面左側開始
            _stripOffset = 0;
        }  // 產生 Dynamic 時重設位置
        private bool AdvanceDynamicPlus()
        {
            // 跑馬燈與多條長條自己移動（一直往同方向捲動）；回傳 false 時照原本的方式移動
            int speed = (int)tabiedMoveSpeed_nud.Value, width = mypicture._width, height = mypicture._height;
            if (tabiedObjectMarquee_rdo.Checked)
            {
                _marqueeX -= speed;
                if (_marqueeX + _marqueeWidth < 0) _marqueeX = width;  // 整行離開左邊後從右邊重新進來
                return true;
            }
            if (tabiedObjectStrip_rdo.Checked && tabiedStripCount_nud.Value > 1)
            {
                int span = tabiedMoveV_rdo.Checked ? height : width;
                _stripOffset = span > 0 ? (_stripOffset + speed) % span : 0;
                return true;
            }
            return false;
        }  // 移動加強物件
        private bool DrawDynamicPlus(Graphics g, Brush brush, int width, int height)
        {
            // Overdrive 方塊、跑馬燈、多條長條；其他物件回傳 false
            int size = mypicture.objectSize;
            if (tabiedObjectOverdrive_rdo.Checked)
            {
                for (int i = 0; i < OverdriveLevels.Length; i++)
                {
                    int center = height * (2 * i + 1) / (2 * OverdriveLevels.Length);
                    g.FillRectangle(brush, mypicture.startX, center - size, size * 2, size * 2);
                }
                return true;
            }
            if (tabiedObjectMarquee_rdo.Checked)
            {
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                using (Font font = new Font("Microsoft JhengHei", Math.Max(8, size), FontStyle.Bold, GraphicsUnit.Pixel))
                {
                    string text = tabiedMarquee_txt.Text;
                    SizeF measured = g.MeasureString(text, font);
                    _marqueeWidth = measured.Width;
                    g.DrawString(text, font, brush, _marqueeX, (height - measured.Height) / 2);
                }
                return true;
            }
            if (tabiedObjectStrip_rdo.Checked && tabiedStripCount_nud.Value > 1)
            {
                // 等距 N 條，超出邊界的部分從另一邊接回來
                int count = (int)tabiedStripCount_nud.Value;
                bool vertical = tabiedMoveV_rdo.Checked;
                int span = vertical ? height : width;
                Color baseColor = tabiedObjectColor_btn.BackColor, inverse = Color.FromArgb(255 - baseColor.R, 255 - baseColor.G, 255 - baseColor.B);
                using (Brush alternate = new SolidBrush(inverse))
                {
                    for (int k = 0; k < count; k++)
                    {
                        Brush stripBrush = tabiedStripAlt_chk.Checked && k % 2 == 1 ? alternate : brush;
                        int position = (_stripOffset + span * k / count) % span;
                        foreach (int p in new[] { position, position - span })
                        {
                            if (vertical) g.FillRectangle(stripBrush, 0, p, width, size);
                            else g.FillRectangle(stripBrush, p, 0, size, height);
                        }
                    }
                }
                return true;
            }
            return false;
        }  // 畫加強物件
    }
}
