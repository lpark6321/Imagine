using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Deployment.Application;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Thunder.Properties;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using Object = System.Object;
using String = System.String;

namespace Thunder
{
    public partial class mainWindow
    {
        // ImgEditor 設定加入 ImgList------------------------------------------------------------------------------
        private Button tabimgeAddToList_btn;
        private void InitEditorToConfig()
        {
            // 放在 ImgEditor 右上角：版面加一欄，功能下拉選單同一列的右側；ImageAdjust 時隱藏
            tabimgeAddToList_btn = new Button { Text = "新增至 ImgList", AutoSize = true, Anchor = AnchorStyles.Top | AnchorStyles.Right, Margin = new Padding(3, 0, 3, 0) };
            tabimgeAddToList_btn.Click += (s, e) => AddEditorSettingToList();
            toolTip.SetToolTip(tabimgeAddToList_btn, "把目前頁面的設定轉成一行 config，加到 ImgList 最後一列");
            tabimgeTableLayoutPanel.ColumnCount = 2;
            tabimgeTableLayoutPanel.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 100);  // 第一欄填滿，按鈕靠右
            tabimgeTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tabimgeTableLayoutPanel.Controls.Add(tabimgeAddToList_btn, 1, 0);
            tabimgeTableLayoutPanel.SetColumnSpan(tabimgePanel, 2);
            tabimgeFuncList.SelectedIndexChanged += (s, e) => tabimgeAddToList_btn.Visible = tabimgeFuncList.Text != "ImageAdjust";

            // Window 頁：背景與方塊一鍵設成同一純色
            var presets = new (string text, Color color, Color fore)[]
            {
                ("黑", Color.Black, Color.White), ("白", Color.White, Color.Black), ("灰", Color.FromArgb(127, 127, 127), Color.White),
            };
            for (int i = 0; i < presets.Length; i++)
            {
                var (text, color, fore) = presets[i];
                var button = new Button { Text = text, BackColor = color, ForeColor = fore, FlatStyle = FlatStyle.Flat, Location = new Point(167, 90 + i * 30), Size = new Size(46, 26) };
                button.Click += (s, e) => SetWindowSolidColor(color);
                toolTip.SetToolTip(button, $"背景與方塊都設成{text}色（純色）");
                tabimgeWindow.Controls.Add(button);
            }
        }  // 建立「新增至 ImgList」與 Window 顏色按鈕
        private void InitMaskQuickButtons()
        {
            // ImgEditor Mask：Pixel 選色在格子下方一排，Sub Pixel 亮度在捲軸右側
            AddMaskQuickButtons(tabimgeMask, tabiemPixelColor_btn, tabiemPixelGray_vsc, tabiemPixel_rdo, tabiemSubPixel_rdo,
                i => new Rectangle(10 + i * 35, 322, 34, 24), i => new Rectangle(266, 70 + i * 49, 40, 24));
            // Window 自製圖 Mask：空間較小，兩種按鈕都放在捲軸右側一欄（同時只顯示一種）
            AddMaskQuickButtons(tabiwCustomMask_grp, tabiwCMaskPixelColor_btn, tabiwCMaskPixelGray_vsc, tabiwCMaskPixel_rdo, tabiwCMaskSubPixel_rdo,
                i => new Rectangle(246, 58 + i * 28, 48, 24), i => new Rectangle(246, 58 + i * 44, 48, 24));
        }  // Mask 快速選色與亮度按鈕
        private void AddMaskQuickButtons(Control box, Button colorButton, VScrollBar grayScroll, RadioButton pixelRadio, RadioButton subPixelRadio,
            Func<int, Rectangle> pixelBounds, Func<int, Rectangle> subPixelBounds)
        {
            // Pixel：快速選色（設定「選擇顏色」，之後點格子即填入）
            var colors = new (string text, Color color)[]
            {
                ("黑", Color.Black), ("白", Color.White), ("紅", Color.Red), ("綠", Color.Lime), ("藍", Color.Blue),
                ("64", Color.FromArgb(64, 64, 64)), ("127", Color.FromArgb(127, 127, 127)),
            };
            var pixelButtons = new List<Button>();
            for (int i = 0; i < colors.Length; i++)
            {
                var (text, color) = colors[i];
                bool dark = color.R * 0.299 + color.G * 0.587 + color.B * 0.114 < 128;  // 依亮度決定字色
                var button = new Button
                {
                    Text = text, BackColor = color, ForeColor = dark ? Color.White : Color.Black, FlatStyle = FlatStyle.Flat,
                    Bounds = pixelBounds(i), Margin = Padding.Empty,
                };
                button.Click += (s, e) => colorButton.BackColor = color;
                toolTip.SetToolTip(button, $"選擇顏色設為{(text.All(char.IsDigit) ? text + " 灰" : text)}（{color.R},{color.G},{color.B}）");
                pixelButtons.Add(button);
            }
            // Sub Pixel：快速設定亮度（捲軸值 = 255 - 亮度）
            int[] levels = { 255, 192, 127, 64, 0 };
            var subPixelButtons = new List<Button>();
            for (int i = 0; i < levels.Length; i++)
            {
                int level = levels[i];
                var button = new Button { Text = level.ToString(), Bounds = subPixelBounds(i) };
                button.Click += (s, e) => grayScroll.Value = 255 - level;
                toolTip.SetToolTip(button, $"子像素亮度設為 {level}");
                subPixelButtons.Add(button);
            }
            box.Controls.AddRange(pixelButtons.Concat(subPixelButtons).ToArray<Control>());
            void UpdateVisible()
            {
                bool subPixel = subPixelRadio.Checked;
                pixelButtons.ForEach(b => b.Visible = !subPixel);
                subPixelButtons.ForEach(b => b.Visible = subPixel);
            }
            pixelRadio.CheckedChanged += (s, e) => UpdateVisible();
            subPixelRadio.CheckedChanged += (s, e) => UpdateVisible();
            UpdateVisible();
        }  // 加入 Mask 快速按鈕
        private CheckBox tabigAll_chk;
        private ComboBox tabigAll_cmb;
        private RadioButton tabigAllGrid_rdo;  // 全畫面格狀（每階一格）
        private RadioButton tabigAllGrad_rdo;  // 依左側漸層條件，階數改為全階並標示
        private static readonly int[] AllLevelCounts = { 16, 32, 64, 128, 256, 1024 };
        private bool AllLevelGrid => tabigAll_chk.Checked && tabigAllGrid_rdo.Checked;
        private bool AllLevelGradient => tabigAll_chk.Checked && tabigAllGrad_rdo.Checked;
        // 漸層實際階數：依漸層條件的全階取「全階」與「漸層分幾階」較小的那個
        private int GradientStepCount => AllLevelGradient ? Math.Min(int.Parse(tabigAll_cmb.Text), int.Parse(tabigStep_cmb.Text)) : int.Parse(tabigStep_cmb.Text);
        private void InitGradientExtras()
        {
            // Gradient 右側：全階顯示（全畫面格狀 / 依漸層條件）
            var group = new GroupBox { Text = "全階 / 階數", Location = new Point(445, 8), Size = new Size(175, 100) };
            tabigAll_chk = new CheckBox { Text = "顯示", AutoSize = true, Location = new Point(10, 22) };
            tabigAll_cmb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(70, 20), Width = 70, ImeMode = ImeMode.Disable };
            tabigAll_cmb.Items.AddRange(AllLevelCounts.Select(n => (object)n.ToString()).ToArray());
            tabigAll_cmb.SelectedItem = "256";
            var unit = new Label { Text = "階", AutoSize = true, Location = new Point(144, 24) };
            tabigAllGrid_rdo = new RadioButton { Text = "全畫面格狀", AutoSize = true, Location = new Point(26, 48), Checked = true };
            tabigAllGrad_rdo = new RadioButton { Text = "依漸層條件", AutoSize = true, Location = new Point(26, 70) };
            group.Controls.AddRange(new Control[] { tabigAll_chk, tabigAll_cmb, unit, tabigAllGrid_rdo, tabigAllGrad_rdo });
            tabimgeGradient.Controls.Add(group);
            group.BringToFront();
            toolTip.SetToolTip(tabigAll_chk, "勾選後顯示全部階數（16~256 階標示 8 bit 階數；1024 階標示 10 bit 代碼 0~1023）");
            toolTip.SetToolTip(tabigAll_cmb, "全階的階數");
            toolTip.SetToolTip(tabigAllGrid_rdo, "每一階一格，依畫面比例排成多行多列並標示階數\r\nV 漸層由上往下排、H 漸層由左往右排；顏色依基底色彩或其他顏色（每個顏色一區）");
            toolTip.SetToolTip(tabigAllGrad_rdo, "依左側漸層設定（方向、分割、起始結束、顏色或其他顏色）產生，並在每一階標示階數\r\n階數取「全階」與「漸層分幾階」較小的那個");
            void Update()
            {
                // ColorBar 模式沒有漸層，全階不能用
                if (tabigColorBar_rdo.Checked && tabigAll_chk.Checked) tabigAll_chk.Checked = false;
                tabigAll_chk.Enabled = !tabigColorBar_rdo.Checked;
                tabigAll_cmb.Enabled = tabigAllGrid_rdo.Enabled = tabigAllGrad_rdo.Enabled = tabigAll_chk.Checked;
                tabigStep_cmb.Enabled = tabigDivid_cmb.Enabled = !AllLevelGrid;  // 全畫面格狀用全階的階數並鋪滿整個畫面，漸層分幾階、畫面幾等分都不使用
            }
            tabigAll_chk.CheckedChanged += (s, e) => Update();
            tabigAllGrid_rdo.CheckedChanged += (s, e) => Update();
            tabigColorBar_rdo.CheckedChanged += (s, e) => Update();
            Update();
        }  // 建立全階與顯示階數
        private static void PickAllLevelGrid(int count, int width, int height, out int columns, out int rows)
        {
            // 在 2 的次方的組合中挑「欄 / 列」最接近畫面寬高比的（例：1920x1080 的 256 階 → 16 x 16）
            columns = count;
            rows = 1;
            double best = double.MaxValue, target = Math.Log((double)width / height);
            for (int c = 1; c <= count; c *= 2)
            {
                if (count % c != 0) continue;
                int r = count / c;
                double diff = Math.Abs(Math.Log((double)c / r) - target);
                if (diff < best - 1e-9)
                {
                    best = diff;
                    columns = c;
                    rows = r;
                }
            }
        }  // 決定全階的欄列數
        private Bitmap DrawAllLevels(int width, int height, int count)
        {
            // 全畫面格狀：每個顏色一區（單色時整個畫面），區內每一階一格並標示階數
            var colors = new List<Color>();
            bool splitVertically = true;  // 多個顏色時上下排列
            if (tabigOther_chk.Checked)
            {
                var panels = tabigOtherSplit_pnl.Controls.OfType<Panel>()
                    .Where(p => p.BackColor != Color.Transparent && p.Tag != null)
                    .OrderBy(p => BlockCell(p).x + BlockCell(p).y).ToList();
                colors.AddRange(panels.Select(p => p.BackColor));
                splitVertically = panels.Count == 0 || panels[0].Tag.ToString().StartsWith("H");
            }
            if (colors.Count == 0) colors.Add(tabigBaseColor_lbl.BackColor);
            bool rowMajor = tabigHWay_rdo.Checked;  // H 漸層由左往右排，其他由上往下排
            var bitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                for (int i = 0; i < colors.Count; i++)
                {
                    Rectangle area = splitVertically
                        ? Rectangle.FromLTRB(0, height * i / colors.Count, width, height * (i + 1) / colors.Count)
                        : Rectangle.FromLTRB(width * i / colors.Count, 0, width * (i + 1) / colors.Count, height);
                    DrawAllLevelArea(g, area, count, colors[i], rowMajor);
                }
            }
            return bitmap;
        }  // 產生全階圖
        private static void DrawAllLevelArea(Graphics g, Rectangle area, int count, Color baseColor, bool rowMajor)
        {
            // 每一階一格；格子左上標示階數，深色格用白字、淺色格用黑字
            PickAllLevelGrid(count, area.Width, area.Height, out int columns, out int rows);
            float cellWidth = (float)area.Width / columns, cellHeight = (float)area.Height / rows;
            float fontSize = Math.Max(7f, Math.Min(20f, Math.Min(cellHeight / 4f, cellWidth / 6f)));
            using (Font font = new Font("Times New Roman", fontSize, GraphicsUnit.Pixel))
            using (var format = new StringFormat(StringFormat.GenericTypographic))
            {
                for (int i = 0; i < count; i++)
                {
                    int column = rowMajor ? i % columns : i / rows, row = rowMajor ? i / columns : i % rows;
                    int x0 = area.X + (int)Math.Round(column * cellWidth), x1 = area.X + (int)Math.Round((column + 1) * cellWidth);
                    int y0 = area.Y + (int)Math.Round(row * cellHeight), y1 = area.Y + (int)Math.Round((row + 1) * cellHeight);
                    int level = (int)Math.Round(i * 255.0 / (count - 1));  // 實際顯示的 8 bit 灰階
                    Color color = Color.FromArgb(baseColor.R * level / 255, baseColor.G * level / 255, baseColor.B * level / 255);
                    using (Brush brush = new SolidBrush(color))
                    {
                        g.FillRectangle(brush, x0, y0, x1 - x0, y1 - y0);
                    }
                    string label = (count > 256 ? i : level).ToString();
                    double luminance = color.R * 0.299 + color.G * 0.587 + color.B * 0.114;
                    g.DrawString(label, font, luminance < 128 ? Brushes.White : Brushes.Black, x0 + fontSize * 0.5f, y0 + fontSize * 0.5f, format);
                }
            }
        }  // 畫一區全階
        private static void DrawGradientLabels(Bitmap bitmap, bool horizontalBands)
        {
            // 依實際像素找出每一階（顏色相同的連續區段），在放得下的階上標示階數
            // horizontalBands：H 漸層（往下變化，每階是一條橫帶）
            // 另一方向先找出「色條」（ColorBar 的每個顏色、反向的每一段），每條各自標示
            int along = horizontalBands ? bitmap.Height : bitmap.Width;
            int across = horizontalBands ? bitmap.Width : bitmap.Height;
            Color Pixel(int a, int c) => horizontalBands ? bitmap.GetPixel(c, a) : bitmap.GetPixel(a, c);
            // 色條：在漸層方向的 1/4 與 3/4 處沿另一方向掃描，兩處顏色都相同的連續區段為一條
            // （只看中間的話，反向的兩段在中間亮度相同會被誤判成同一條）
            var strips = new List<(int start, int end)>();
            int quarter = along / 4, threeQuarter = along * 3 / 4;
            (int, int) Key(int c) => (Pixel(quarter, c).ToArgb(), Pixel(threeQuarter, c).ToArgb());
            for (int c = 0; c < across; c++)
            {
                if (strips.Count > 0 && Key(c) == Key(strips[strips.Count - 1].start)) strips[strips.Count - 1] = (strips[strips.Count - 1].start, c);
                else strips.Add((c, c));
            }
            // 每條色條沿漸層方向找出每一階
            var labels = new List<(float x, float y, string text, Brush brush)>();
            using (Graphics g = Graphics.FromImage(bitmap))
            using (Font font = new Font("Times New Roman", 18f, GraphicsUnit.Pixel))
            {
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                foreach (var (stripStart, stripEnd) in strips)
                {
                    int sample = (stripStart + stripEnd) / 2;
                    int stripWidth = stripEnd - stripStart + 1;
                    var runs = new List<(int start, int end, Color color)>();
                    for (int p = 0; p < along; p++)
                    {
                        Color color = Pixel(p, sample);
                        if (runs.Count > 0 && runs[runs.Count - 1].color.ToArgb() == color.ToArgb()) runs[runs.Count - 1] = (runs[runs.Count - 1].start, p, color);
                        else runs.Add((p, p, color));
                    }
                    int lastEnd = int.MinValue;
                    foreach (var (start, end, color) in runs)
                    {
                        string text = Math.Max(color.R, Math.Max(color.G, color.B)).ToString();
                        SizeF size = g.MeasureString(text, font);
                        if ((horizontalBands ? size.Width : size.Height) > stripWidth) continue;  // 色條太窄放不下
                        float need = horizontalBands ? size.Height : size.Width;
                        float begin = (start + end + 1) / 2f - need / 2f;
                        if (begin < lastEnd + 2) continue;  // 跟前一個標示重疊就跳過
                        float x = horizontalBands ? sample - size.Width / 2f : begin;
                        float y = horizontalBands ? begin : sample - size.Height / 2f;
                        double luminance = color.R * 0.299 + color.G * 0.587 + color.B * 0.114;
                        labels.Add((x, y, text, luminance < 128 ? Brushes.White : Brushes.Black));
                        lastEnd = (int)Math.Ceiling(begin + need);
                    }
                }
                // 全部量完再畫，標示不會影響其他色條的判斷
                foreach (var (x, y, text, brush) in labels) g.DrawString(text, font, brush, x, y);
            }
        }  // 漸層標示階數
        private void InitGradientReverse()
        {
            // 反向是讓奇數等分反轉漸層方向，畫面等分 < 2 時沒有作用，停用並取消勾選
            void Update()
            {
                bool usable = int.TryParse(tabigDivid_cmb.Text, out int divisions) && divisions >= 2;
                tabigReverse_chk.Enabled = usable;
                if (!usable) tabigReverse_chk.Checked = false;
            }
            tabigDivid_cmb.TextChanged += (s, e) => Update();
            toolTip.SetToolTip(tabigReverse_chk, "奇數等分反轉漸層方向（畫面幾等分 ≥ 2 才能使用）");
            Update();
        }  // 漸層反向的啟用條件
        private void InitWindowTabs()
        {
            // 自製圖與窗口 / 線條清單合併成分頁，放在原自製圖的位置
            var tabs = new TabControl { Location = new Point(tabiwCustom_grp.Left, 8), Size = new Size(tabiwCustom_grp.Width + 5, tabiwCustom_grp.Bottom - 8 + 14) };
            var customPage = new TabPage("自製圖") { UseVisualStyleBackColor = true };
            var overlayPage = new TabPage("窗口 / 線條") { UseVisualStyleBackColor = true };
            tabs.TabPages.AddRange(new[] { customPage, overlayPage });
            tabimgeWindow.Controls.Add(tabs);
            tabimgeWindow.Controls.Remove(tabiwCustom_grp);
            tabiwCustom_grp.Dock = DockStyle.Fill;
            customPage.Controls.Add(tabiwCustom_grp);
            // 窗口 / 線條清單與按鈕移入第二頁
            string[] overlayButtonTexts = { "新增", "修改", "複製", "上移", "下移", "移除", "全清" };
            var overlayButtons = tabimgeWindow.Controls.OfType<Button>().Where(b => overlayButtonTexts.Contains(b.Text)).OrderBy(b => b.Left).ToList();
            // 用 Dock 排版：清單填滿，按鈕列在底部（不依賴分頁當下的尺寸）
            overlayPage.Padding = new Padding(6);
            tabimgeWindow.Controls.Remove(tabiwOverlay_lvw);
            tabiwOverlay_lvw.Dock = DockStyle.Fill;
            tabiwOverlay_lvw.Columns[0].Width = 70;
            tabiwOverlay_lvw.Columns[1].Width = 280;
            var buttonBar = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 34, Padding = new Padding(0, 4, 0, 0) };
            foreach (Button button in overlayButtons)
            {
                tabimgeWindow.Controls.Remove(button);
                button.Size = new Size(48, 26);
                button.Margin = new Padding(2, 3, 2, 3);  // 7 個按鈕排成一列
                buttonBar.Controls.Add(button);
            }
            overlayPage.Controls.Add(tabiwOverlay_lvw);
            overlayPage.Controls.Add(buttonBar);  // 後加入的先 Dock，按鈕列先佔底部，清單填滿其餘
            tabs.SelectedTab = overlayPage;  // 預設顯示「窗口 / 線條」（自製圖頁有大量格子，切到時才排版，載入較快）
        }  // Window 自製圖與窗口 / 線條分頁
        private ComboBox tabiwDiagonal_cmb;
        private void InitWindowDiagonal()
        {
            // 斜角改為下拉選單；原勾選框隱藏保留，作為「有斜角」的狀態（沿用其停用視窗位置等邏輯）
            tabiwDiagonal_btn.Visible = false;
            var label = new Label { Text = "斜角", AutoSize = true, Location = new Point(10, 312) };
            tabiwDiagonal_cmb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(10, 327), Width = 58, ImeMode = ImeMode.Disable };
            tabiwDiagonal_cmb.Items.AddRange(new object[] { "無", "左上", "右上", "右下", "左下" });
            tabiwDiagonal_cmb.SelectedIndexChanged += (s, e) => tabiwDiagonal_btn.Checked = tabiwDiagonal_cmb.SelectedIndex > 0;
            tabiwDiagonal_cmb.SelectedIndex = tabiwDiagonal_btn.Checked ? 2 : 0;
            toolTip.SetToolTip(tabiwDiagonal_cmb, "以對角線切開畫面，在選擇的角落畫出方塊顏色的三角形");
            tabimgeWindow.Controls.AddRange(new Control[] { label, tabiwDiagonal_cmb });
            // 騰出空間：線顏色按鈕移到標籤右側（窗口 / 線條清單已移到右側分頁）
            tabiwLineColor_lbl.Location = new Point(10, 357);
            tabiwLineColor_btn.Location = new Point(54, 352);
        }  // 建立斜角下拉選單
        private CheckBox tabiefRedBlue_chk;
        private void InitFrameRedBlue()
        {
            // 「其他(H&V)」右側
            tabiefRedBlue_chk = new CheckBox { Text = "左紅右藍", AutoSize = true, Location = new Point(tabiefOther_chk.Right + 8, tabiefOther_chk.Top) };
            toolTip.SetToolTip(tabiefRedBlue_chk, "畫面四邊：上下白線、左紅線、右藍線（固定 1 pixel）");
            tabimgeFrame.Controls.Add(tabiefRedBlue_chk);
            tabiefRedBlue_chk.BringToFront();
        }  // 建立左紅右藍
        private CheckBox tabiefText_chk;
        private CheckBox tabiefTextBoom_chk;  // 爆炸模式：字級由小到大逐行排列
        private ComboBox tabiefTextFont_cmb;  // 文字模式字型（預設：鋪滿用微軟正黑體粗體、爆炸用 Arial）
        private CheckBox tabiefTextBold_chk;
        private const string DefaultTextFont = "預設";
        private static readonly string[] TextFontChoices = { "Microsoft JhengHei", "PMingLiU", "DFKai-SB", "Arial", "Times New Roman", "Verdana", "Tahoma", "Segoe UI", "Consolas", "Courier New" };
        private TextBox tabiefText_txt;
        private NumericUpDown tabiefTextSize_nud, tabiefTextCharGap_nud, tabiefTextLineGap_nud, tabiefTextStringGap_nud;
        private Button tabiefTextColor_btn;
        private Control[] _frameTextSettings;  // 未勾選文字模式時停用的設定
        private void InitFrameTextMode()
        {
            // Frame 頁：框線顏色 / 粗細右側
            const int x = 340;
            Label Caption(string text) => new Label { Text = text, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 3, 3, 3) };
            NumericUpDown Nud(decimal min, decimal max, int decimals) => new NumericUpDown
            {
                Width = 60, Minimum = min, Maximum = max, DecimalPlaces = decimals, Increment = decimals > 0 ? 0.5m : 1m,
                ImeMode = ImeMode.Disable, Anchor = AnchorStyles.Left, Margin = new Padding(0, 2, 6, 2),
            };
            tabiefText_chk = new CheckBox { Text = "文字模式", AutoSize = true, Location = new Point(x, 57) };
            tabiefText_txt = new TextBox { MaxLength = 30, Width = 150, Location = new Point(x, 80), ImeMode = ImeMode.NoControl };  // 鋪滿模式最多 10 個字、爆炸模式最多 30 個字，可打中文
            tabiefText_txt.Text = "H";  // 預設文字
            tabiefTextSize_nud = Nud(1, 500, 0);            // 大小 1~500 px
            tabiefTextCharGap_nud = Nud(0, 500, 1);         // 間距 0~500 px（以筆畫邊緣計算，0 = 貼齊）
            tabiefTextLineGap_nud = Nud(0, 500, 1);
            tabiefTextStringGap_nud = Nud(0, 500, 1);
            tabiefTextColor_btn = new Button { Size = new Size(24, 24), BackColor = Color.White, FlatStyle = FlatStyle.Flat, Anchor = AnchorStyles.Left, Margin = new Padding(0, 2, 0, 2) };
            var labels = new[] { Caption("大小"), Caption("顏色"), Caption("字距"), Caption("行距"), Caption("串距") };
            // 用表格自動排版，不同縮放比例下標籤也不會被擋住
            var grid = new TableLayoutPanel { Location = new Point(x, 105), AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, ColumnCount = 4, RowCount = 4, Padding = Padding.Empty, Margin = Padding.Empty };
            grid.Controls.Add(labels[0], 0, 0); grid.Controls.Add(tabiefTextSize_nud, 1, 0); grid.Controls.Add(labels[1], 2, 0); grid.Controls.Add(tabiefTextColor_btn, 3, 0);
            grid.Controls.Add(labels[2], 0, 1); grid.Controls.Add(tabiefTextCharGap_nud, 1, 1);
            grid.Controls.Add(labels[3], 0, 2); grid.Controls.Add(tabiefTextLineGap_nud, 1, 2);
            grid.Controls.Add(labels[4], 0, 3); grid.Controls.Add(tabiefTextStringGap_nud, 1, 3);
            // 字型：只列出這台電腦有安裝的字型
            var installed = new System.Drawing.Text.InstalledFontCollection().Families.Select(f => f.GetName(1033)).ToList();
            tabiefTextFont_cmb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 130, ImeMode = ImeMode.Disable, Anchor = AnchorStyles.Left, Margin = new Padding(0, 2, 0, 2) };
            tabiefTextFont_cmb.Items.Add(DefaultTextFont);
            tabiefTextFont_cmb.Items.AddRange(TextFontChoices.Where(f => installed.Contains(f, StringComparer.OrdinalIgnoreCase)).ToArray<object>());
            tabiefTextFont_cmb.SelectedIndex = 0;
            tabiefTextBold_chk = new CheckBox { Text = "粗體", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 4, 0, 2) };
            var fontLabel = Caption("字型");
            grid.Controls.Add(fontLabel, 2, 1); grid.Controls.Add(tabiefTextFont_cmb, 3, 1);
            grid.Controls.Add(tabiefTextBold_chk, 3, 2);
            tabiefTextFont_cmb.SelectedIndexChanged += (s, e) => UpdateFrameTextEnabled();
            toolTip.SetToolTip(tabiefTextFont_cmb, "文字字型；「預設」為鋪滿模式用微軟正黑體粗體、爆炸模式用 Arial");
            toolTip.SetToolTip(tabiefTextBold_chk, "選了字型後可設定粗體");
            tabiefTextSize_nud.Value = 12;
            ResetFrameTextGaps();
            // 改大小時間距依比例重設為預設值（之後仍可個別調整）
            tabiefTextSize_nud.ValueChanged += (s, e) => ResetFrameTextGaps();
            tabiefTextColor_btn.Click += (s, e) =>
            {
                using (ColorDialog dialog = new PatternColorDialog())
                {
                    if (dialog.ShowDialog(this) == DialogResult.OK) tabiefTextColor_btn.BackColor = dialog.Color;
                }
            };
            _frameTextSettings = new Control[] {tabiefText_txt, tabiefTextSize_nud, tabiefTextColor_btn, tabiefTextCharGap_nud, tabiefTextLineGap_nud, tabiefTextStringGap_nud, tabiefTextFont_cmb, fontLabel }.Concat(labels).ToArray();
            tabiefText_chk.CheckedChanged += (s, e) => UpdateFrameTextEnabled();
            UpdateFrameTextEnabled();
            toolTip.SetToolTip(tabiefText_chk, "勾選後以文字依座標重複鋪滿整個畫面");
            toolTip.SetToolTip(tabiefText_txt, "鋪滿模式最多 10 個字、爆炸模式最多 30 個字（不能有逗號 , 與斜線 /）");
            toolTip.SetToolTip(tabiefTextSize_nud, "字的大小（像素），改變時間距會依比例重設");
            toolTip.SetToolTip(tabiefTextCharGap_nud, "字與字筆畫之間的空白，0 = 貼齊（預設 大小/5）");
            toolTip.SetToolTip(tabiefTextLineGap_nud, "上下兩行筆畫之間的空白，0 = 貼齊（預設 大小/3）");
            toolTip.SetToolTip(tabiefTextStringGap_nud, "重複字串之間的空白，0 = 貼齊（預設 大小/2）");
            tabiefTextBoom_chk = new CheckBox { Text = "爆炸模式", AutoSize = true, Location = new Point(x + 80, 57) };
            tabiefTextBoom_chk.CheckedChanged += (s, e) => UpdateFrameTextEnabled();
            toolTip.SetToolTip(tabiefTextBoom_chk, "每行字級由 4pt 起，20pt 前每次 +2、40pt 前 +4、80pt 前 +8、之後 +16；左邊標示字級，排到畫面底為止（文字最多 30 個字）");
            UpdateFrameTextEnabled();
            tabimgeFrame.Controls.AddRange(new Control[] { tabiefText_chk, tabiefTextBoom_chk, tabiefText_txt, grid });
            foreach (Control control in new Control[] { tabiefText_chk, tabiefTextBoom_chk, tabiefText_txt, grid }) control.BringToFront();  // 避免被其他控制項蓋住
            // 逗號是 config 欄位分隔、/ 是 Text 選項分隔，文字裡不能有
            tabiefText_txt.KeyPress += (s, e) => { if (e.KeyChar == ',' || e.KeyChar == '/') e.Handled = true; };
            tabiefText_txt.TextChanged += (s, e) =>
            {
                string clean = tabiefText_txt.Text.Replace(",", "").Replace("/", "");
                if (clean != tabiefText_txt.Text) tabiefText_txt.Text = clean;  // 貼上時移除
            };
        }  // 建立 Frame 文字模式
        private void ResetFrameTextGaps()
        {
            decimal size = tabiefTextSize_nud.Value;
            tabiefTextCharGap_nud.Value = Math.Round(size / 5, 1);
            tabiefTextLineGap_nud.Value = Math.Round(size / 3, 1);
            tabiefTextStringGap_nud.Value = Math.Round(size / 2, 1);
        }  // 間距預設值
        private void UpdateFrameTextEnabled()
        {
            foreach (Control control in _frameTextSettings) control.Enabled = tabiefText_chk.Checked;
            if (tabiefTextBoom_chk == null) return;
            tabiefTextBold_chk.Enabled = tabiefText_chk.Checked && tabiefTextFont_cmb.SelectedIndex > 0;  // 預設字型的粗細固定
            tabiefTextBoom_chk.Enabled = tabiefText_chk.Checked;
            // 爆炸模式的字級固定，大小與間距不使用
            foreach (Control control in new Control[] { tabiefTextSize_nud, tabiefTextCharGap_nud, tabiefTextLineGap_nud, tabiefTextStringGap_nud })
            {
                control.Enabled = tabiefText_chk.Checked && !tabiefTextBoom_chk.Checked;
            }
        }  // 未勾選時停用設定
        private static void DrawTiledText(Graphics g, string text, Font font, Color color, float charGap, float lineGap, float stringGap, int width, int height)
        {
            // 間距以「筆畫實際範圍」計算：字型本身左右留白與行高預留都不算，間距 0 即筆畫貼齊
            // 不做反鋸齒，畫面只有背景色與文字色
            var textHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            using (Brush brush = new SolidBrush(color))
            using (StringFormat format = (StringFormat)StringFormat.GenericTypographic.Clone())
            {
                format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;  // 空白字元也要算寬度
                g.TextRenderingHint = textHint;
                // 量每個字的筆畫範圍（相對於繪製原點）；空白字元沒有筆畫，用字寬
                int pad = (int)Math.Ceiling(font.GetHeight(g));
                var ink = new Rectangle[text.Length];
                var advance = new float[text.Length];
                for (int i = 0; i < text.Length; i++)
                {
                    string ch = text[i].ToString();
                    advance[i] = g.MeasureString(ch, font, PointF.Empty, format).Width;
                    using (Bitmap probe = new Bitmap((int)Math.Ceiling(advance[i]) + pad * 2, pad * 3))
                    using (Graphics pg = Graphics.FromImage(probe))
                    {
                        pg.Clear(Color.White);
                        pg.TextRenderingHint = textHint;
                        pg.DrawString(ch, font, Brushes.Black, pad, pad, format);
                        ink[i] = InkBounds(probe, pad);
                    }
                }
                var drawn = ink.Where(r => !r.IsEmpty).ToList();
                if (drawn.Count == 0) return;  // 只有空白
                int inkTop = drawn.Min(r => r.Top), inkBottom = drawn.Max(r => r.Bottom);
                float stepY = Math.Max(1f, inkBottom - inkTop + lineGap);  // 行與行：筆畫高度 + 行距
                // 一個字串內各字的 X 位置（以筆畫左緣對齊游標）
                var charX = new float[text.Length];
                float cursor = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    if (ink[i].IsEmpty) { charX[i] = cursor; cursor += advance[i] + charGap; continue; }
                    charX[i] = cursor - ink[i].Left;
                    cursor += ink[i].Width + charGap;
                }
                float stepX = Math.Max(1f, cursor - charGap + stringGap);  // 字串與字串：最後一字筆畫右緣 + 串距
                for (float y = 0; y < height; y += stepY)
                {
                    for (float x = 0; x < width; x += stepX)
                    {
                        for (int i = 0; i < text.Length; i++)
                        {
                            if (ink[i].IsEmpty) continue;
                            // 取整數座標，筆畫點陣與量測時一致
                            g.DrawString(text[i].ToString(), font, brush, (float)Math.Round(x + charX[i]), (float)Math.Round(y - inkTop), format);
                        }
                    }
                }
            }
        }  // 文字鋪滿畫面
        private static void DrawBoomText(Graphics g, string text, Color color, int width, int height, Func<float, Font> makeFont)
        {
            // 爆炸模式：每行字級由 4pt 起，20pt 前每次 +2、40pt 前 +4、80pt 前 +8、之後 +16；左邊標示字級，排到畫面底為止（最後一行可被切掉）
            GraphicsState state = g.Save();
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            using (Brush brush = new SolidBrush(color))
            using (var format = new StringFormat(StringFormat.GenericTypographic))
            {
                float y = 0;
                for (int size = 4; y < height && size <= 400; size += size < 20 ? 2 : size < 40 ? 4 : size < 80 ? 8 : 16)
                {
                    using (Font font = makeFont(size))
                    {
                        float lineHeight = font.GetHeight(g);
                        float pixelSize = size * g.DpiY / 72f;
                        string label = $"{size}pt";
                        float x = pixelSize * 0.15f + 2;
                        g.DrawString(label, font, brush, x, y, format);
                        g.DrawString(text, font, brush, x + pixelSize * 4.2f, y, format);  // 字級標示寬度約 4 個字寬
                        y += lineHeight;
                    }
                }
            }
            g.Restore(state);
        }  // 文字爆炸模式
        private Font FrameTextFont(float size, GraphicsUnit unit, string defaultFamily, FontStyle defaultStyle)
        {
            // 「預設」時依模式的預設字型，否則用選的字型與粗體設定
            if (tabiefTextFont_cmb.SelectedIndex <= 0) return new Font(defaultFamily, size, defaultStyle, unit);
            return new Font(tabiefTextFont_cmb.Text, size, tabiefTextBold_chk.Checked ? FontStyle.Bold : FontStyle.Regular, unit);
        }  // 文字模式字型
        private string TextFontTokens() =>
            tabiefTextFont_cmb.SelectedIndex <= 0 ? "" : $"/f:{tabiefTextFont_cmb.Text}" + (tabiefTextBold_chk.Checked ? "/b" : "");  // 寫進 config 的字型段
        private void ApplyTextFontTokens(IEnumerable<string> tokens)
        {
            // f:字型名稱、b 粗體；沒寫時為預設字型
            tabiefTextFont_cmb.SelectedIndex = 0;
            tabiefTextBold_chk.Checked = false;
            foreach (string token in tokens)
            {
                if (token.StartsWith("f:", StringComparison.OrdinalIgnoreCase))
                {
                    string name = token.Substring(2).Trim();
                    int index = tabiefTextFont_cmb.Items.Cast<object>().ToList().FindIndex(i => string.Equals(i.ToString(), name, StringComparison.OrdinalIgnoreCase));
                    if (index < 0) throw new FormatException($"字型「{name}」不在清單或這台電腦沒有安裝，可用：{string.Join("、", tabiefTextFont_cmb.Items.Cast<object>().Skip(1))}");
                    tabiefTextFont_cmb.SelectedIndex = index;
                }
                else if (token.Equals("b", StringComparison.OrdinalIgnoreCase))
                {
                    tabiefTextBold_chk.Checked = true;
                }
            }
        }  // 套用 config 的字型段
        private static bool IsTextFontToken(string token) => token.StartsWith("f:", StringComparison.OrdinalIgnoreCase) || token.Equals("b", StringComparison.OrdinalIgnoreCase);
        private static Rectangle InkBounds(Bitmap probe, int origin)
        {
            // 找出非白色像素的範圍，回傳相對於繪製原點 (origin, origin) 的矩形；沒有筆畫回傳 Empty
            int left = int.MaxValue, top = int.MaxValue, right = -1, bottom = -1;
            for (int y = 0; y < probe.Height; y++)
            {
                for (int x = 0; x < probe.Width; x++)
                {
                    if (probe.GetPixel(x, y).R < 128)
                    {
                        if (x < left) left = x;
                        if (x > right) right = x;
                        if (y < top) top = y;
                        if (y > bottom) bottom = y;
                    }
                }
            }
            return right < 0 ? Rectangle.Empty : Rectangle.FromLTRB(left - origin, top - origin, right - origin + 1, bottom - origin + 1);
        }  // 量測筆畫範圍
        private void SetWindowSolidColor(Color color)
        {
            tabiwBackColor_rdo.Checked = true;
            tabiwBack_pic.Image = null;
            tabiwBack_pic.BackColor = color;
            tabiwWinColor_rdo.Checked = true;
            tabiwWin_pic.Image = null;
            tabiwWin_pic.BackColor = color;
        }  // Window 背景與方塊設成同一純色
        private static string ConfigColorText(Color c)
        {
            // 一般顏色名稱直接寫名稱，其餘寫 R.G.B（系統色如 Control 也寫成 R.G.B）
            return c.IsKnownColor && !c.IsSystemColor ? c.Name : $"{c.R}.{c.G}.{c.B}";
        }  // 顏色轉 config 寫法
        private void AddEditorSettingToList()
        {
            var ignored = new List<string>();
            string line = EditorToConfigLine(ignored, out string error);
            if (line == null && error == null) return;  // ImageAdjust 等不支援的功能
            if (error != null)
            {
                MessageBox.Show($"無法加入 ImgList：{error}", "新增至 ImgList", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (ignored.Count > 0 &&
                MessageBox.Show("以下設定 config 無法描述，加入後會被忽略：\r\n\r\n・" + string.Join("\r\n・", ignored) + "\r\n\r\n仍要加入嗎？",
                    "新增至 ImgList", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }
            using (SuspendFilter())  // 搜尋中也加在完整清單的最後
            {
                PushConfigUndo();  // 記錄復原點
                ListViewItem item = CreateConfigItem(line);
                tabilPatternList_lst.Items.Add(item);
                AdoptNeighborGroup(item);  // 分組時加到最後一組
                RenumberPatternList();
                item.EnsureVisible();
                ssrStatus_lbl.Text = $"已加入 ImgList 第 {item.Index + 1} 列：{item.SubItems[1].Text}";
            }
        }  // 目前 ImgEditor 設定加入 ImgList
        private string EditorToConfigLine(List<string> ignored, out string error)
        {
            // 目前 ImgEditor 設定轉成一行 config（名稱為預設名稱）；不支援的功能回傳 null 且 error 為 null
            string type, param, option;
            switch (tabimgeFuncList.Text)
            {
                case "Frame&CrossLine&Border":
                    // 勾選文字模式時存成 Text 類型
                    if (tabiefText_chk.Checked) { type = "Text"; error = TextToConfig(ignored, out param, out option); }
                    else { type = "Solid"; error = FrameToConfig(ignored, out param, out option); }
                    break;
                case "Gradient&ColorBar": type = "Gradient"; error = GradientToConfig(ignored, out param, out option); break;
                case "Chess": type = "Chess"; error = ChessToConfig(out param, out option); break;
                case "Window": type = "Window"; error = WindowToConfig(ignored, out param, out option); break;
                case "Mask(FlickerPattern)": type = "Mask"; error = MaskToConfig(out param, out option); break;
                case "Dynamic": type = "Dynamic"; error = DynamicToConfig(out param, out option); break;
                case "Testpattern": type = "Test"; error = TestToConfig(out param, out option); break;
                default: error = null; return null;
            }
            if (error != null) return null;
            string name = $"{type} {param}{(option != "" ? " " + option : "")}";  // 預設名稱，可在 ImgList 編輯區修改
            return string.Join(",", name, type, param, option);
        }  // ImgEditor 設定轉 config
        private string FrameToConfig(List<string> ignored, out string param, out string option)
        {
            param = option = "";
            if (mypicture.tag == "image") return "背景使用匯入的截圖，config 只能描述純色背景";
            if (tabiefOther_chk.Checked) ignored.Add("其他(H&V) 線條");
            if (tabiefLineSize_nud.Value != 1) ignored.Add($"框線粗細 {tabiefLineSize_nud.Value}（config 固定為 1）");
            param = ConfigColorText(tabiefBack_btn.BackColor);
            string lines = (tabiefCenter_chk.Checked ? "c" : "") + (tabiefOutside_chk.Checked ? "o" : "") + (tabiefNine_chk.Checked ? "n" : "");
            if (lines != "") option = $"{lines}{(tabiefRedBlue_chk.Checked ? "r" : "")}-{ConfigColorText(tabiefLineColor_btn.BackColor)}";
            else if (tabiefRedBlue_chk.Checked) option = "r";  // 左紅右藍不需要線色
            return null;
        }  // Frame 轉 config
        private string TextToConfig(List<string> ignored, out string param, out string option)
        {
            param = option = "";
            if (mypicture.tag == "image") return "背景使用匯入的截圖，config 只能描述純色背景";
            string text = tabiefText_txt.Text.Trim();
            if (text == "") return "文字模式沒有輸入文字";
            if (tabiefCenter_chk.Checked || tabiefOutside_chk.Checked || tabiefNine_chk.Checked || tabiefRedBlue_chk.Checked) ignored.Add("中心對位 / 外框線 / 九點對位 / 左紅右藍");
            if (tabiefOther_chk.Checked) ignored.Add("其他(H&V) 線條");
            string Num(NumericUpDown nud) => nud.Value.ToString("0.#", System.Globalization.CultureInfo.InvariantCulture);
            param = $"{ConfigColorText(tabiefBack_btn.BackColor)}-{ConfigColorText(tabiefTextColor_btn.BackColor)}";
            if (tabiefTextBoom_chk.Checked)
            {
                option = $"{text}/boom{TextFontTokens()}";  // 爆炸模式
                return null;
            }
            if (text.Length > 10) return "鋪滿模式的文字最多 10 個字";
            option = $"{text}/{Num(tabiefTextSize_nud)}/{Num(tabiefTextCharGap_nud)}/{Num(tabiefTextLineGap_nud)}/{Num(tabiefTextStringGap_nud)}{TextFontTokens()}";
            return null;
        }  // Frame 文字模式轉 config
        private string GradientToConfig(List<string> ignored, out string param, out string option)
        {
            param = option = "";
            if (tabigOther_chk.Checked)
            {
                // 自訂 ColorBar：colorbar-方向-數量.色1.色2…（依 Tag 的序號排列）
                var panels = tabigOtherSplit_pnl.Controls.OfType<Panel>()
                    .Select(p => new { Panel = p, Tag = p.Tag?.ToString().Split(',') })
                    .Where(p => p.Tag != null && p.Tag.Length == 2)
                    .OrderBy(p => int.Parse(p.Tag[1]))
                    .ToList();
                if (panels.Count == 0) return "ColorBar 沒有任何色塊";
                if (panels.Any(p => p.Panel.BackColor == Color.Transparent)) return "ColorBar 有透明（未設定）的色塊";
                string direction = panels[0].Tag[0] == "H" ? "h" : "v";
                param = $"colorbar-{direction}-{panels.Count}." + string.Join(".", panels.Select(p => ConfigColorText(p.Panel.BackColor)));
            }
            else
            {
                param = tabigSaturation_chk.Checked ? "saturation" : ConfigColorText(tabigBaseColor_lbl.BackColor);
            }
            if (AllLevelGrid)
            {
                option = $"all{(tabigHWay_rdo.Checked ? "-h" : "")}-{tabigAll_cmb.Text}";  // 全畫面格狀全階（H 漸層為橫排）
                return null;
            }
            if (!tabigColorBar_rdo.Checked)
            {
                option = $"{(tabigReverse_chk.Checked && tabigReverse_chk.Enabled ? "r-" : "")}{(GradientRadial ? "o" : tabigHWay_rdo.Checked ? "h" : "v")}-{GradientStepCount}-{tabigDivid_cmb.Text}";
                // 起始.結束灰階不是預設 0.255 時才寫（例：255.0 由亮到暗）
                if (tabigFirstLevel_cmb.Text.Trim() != "0" || tabigLastLevel_cmb.Text.Trim() != "255")
                {
                    option += $"-{tabigFirstLevel_cmb.Text.Trim()}.{tabigLastLevel_cmb.Text.Trim()}";
                }
                if (AllLevelGradient) option += "-num";  // 依漸層條件標示階數
                option += GradientPlusTokens();  // 曲線、起點色
            }
            return null;
        }  // Gradient 轉 config
        private string ChessToConfig(out string param, out string option)
        {
            // 實際格子顏色 = 顏色與灰階捲軸取較小值
            Color baseColor = tabiecGray_lbl.BackColor;
            int intensity = tabiecGray_hsc.Value;
            Color color = Color.FromArgb(Math.Min(baseColor.R, intensity), Math.Min(baseColor.G, intensity), Math.Min(baseColor.B, intensity));
            param = $"{tabiecHNum_nud.Value}*{tabiecVNum_nud.Value}";
            string flip = (tabiecHFlip_chk.Checked ? "h" : "") + (tabiecVFlip_chk.Checked ? "v" : "");
            option = color.ToArgb() == Color.White.ToArgb() ? flip : ConfigColorText(color) + (flip != "" ? "-" + flip : "");
            return null;
        }  // Chess 轉 config
        private string WindowToConfig(List<string> ignored, out string param, out string option)
        {
            param = option = "";
            if (!tabiwBackColor_rdo.Checked) return "背景使用截圖或自製圖，config 只能描述純色背景與方塊（自製圖請改用 Window 的 Custom / excel 寫法手動輸入）";
            if (!tabiwWinColor_rdo.Checked) return "方塊使用截圖或自製圖，config 只能描述純色背景與方塊";
            if (!tabiwWinSizePercent_rdo.Checked) return "視窗大小使用指定 Pixel，config 只支援百分比";
            if (tabiwLineOutside_btn.Checked) ignored.Add("外框");
            if (tabiwLineCross_btn.Checked) ignored.Add("十字");
            if (tabiwDiagonal_btn.Checked) ignored.Add($"斜角（{tabiwDiagonal_cmb.Text}）");
            if (tabiwOverlay_lvw.Items.Count > 0) ignored.Add($"右側疊加清單的 {tabiwOverlay_lvw.Items.Count} 個窗口 / 線條");
            param = $"{ConfigColorText(tabiwBack_pic.BackColor)}-{ConfigColorText(tabiwWin_pic.BackColor)}";
            string location = tabiwWinLocPixcel_rdo.Checked ? $"xy_{tabiwWinLocPixcelX_nud.Value}.{tabiwWinLocPixcelY_nud.Value}"
                : tabiwWinLocTwo_rdo.Checked ? "two" : "c";
            option = $"{tabiwWinSizePercent_nud.Value}%{location}{MontageConfigSuffix()}";  // 蒙太奇接在後面：|名稱+名稱|亮度
            return null;
        }  // Window 轉 config
        private string MaskToConfig(out string param, out string option)
        {
            param = option = "";
            int w = (int)tabiemWNum_nud.Value, h = (int)tabiemHNum_nud.Value;
            bool subPixel = tabiemSubPixel_rdo.Checked;
            int columns = subPixel ? w * 3 : w;
            var colors = new Color[columns * h];
            for (int i = 0; i < colors.Length; i++) colors[i] = Color.Black;
            foreach (Panel panel in tabiemPixelPanel_pnl.Controls.OfType<Panel>())
            {
                string[] xy = panel.Tag?.ToString().Split(',');
                if (xy == null || xy.Length != 2 || !int.TryParse(xy[0], out int x) || !int.TryParse(xy[1], out int y) || x >= columns || y >= h) continue;
                colors[y * columns + x] = panel.BackColor;
            }
            param = subPixel ? $"s-{w}*{h}-{255 - tabiemPixelGray_vsc.Value}" : $"p-{w}*{h}";
            if (subPixel)
            {
                // 子像素：非黑即亮
                option = string.Join(".", Enumerable.Range(0, h).Select(row =>
                    new string(colors.Skip(row * columns).Take(columns).Select(c => c.ToArgb() == Color.Black.ToArgb() ? '0' : '1').ToArray())));
                return null;
            }
            option = MaskCellsToOption(colors, w, h);
            return null;
        }  // Mask 轉 config
        private static string MaskCellsToOption(Color[] colors, int w, int h)
        {
            char Code(Color c) => c.ToArgb() == Color.Black.ToArgb() ? '0' : c.ToArgb() == Color.White.ToArgb() ? '1' : c.ToArgb() == Color.Gray.ToArgb() ? '2' : '?';
            if (colors.All(c => Code(c) != '?'))
            {
                // 只有黑、白、灰時用精簡寫法，例：10.01
                return string.Join(".", Enumerable.Range(0, h).Select(row => new string(colors.Skip(row * w).Take(w).Select(Code).ToArray())));
            }
            // 其他顏色用顏色寫法，例：red lime/blue 127.127.127
            string option = string.Join("/", Enumerable.Range(0, h).Select(row => string.Join(" ", colors.Skip(row * w).Take(w).Select(ConfigColorText))));
            if (!IsColorMaskOption(option)) option += "/";  // 只有 1 格且為 R.G.B 時補 /，避免被當成精簡寫法
            return option;
        }  // Mask 格子顏色轉 config 選項
        private string DynamicToConfig(out string param, out string option)
        {
            param = option = "";
            if (mypicture.tag == "image") return "背景使用匯入的截圖，config 只能描述純色背景";
            param = $"{ConfigColorText(tabiedBackColor_btn.BackColor)}-{ConfigColorText(tabiedObjectColor_btn.BackColor)}";
            string shape = tabiedObjectCube_rdo.Checked ? "cube" : tabiedObjectStrip_rdo.Checked ? "strip" : tabiedObjectBlur_rdo.Checked ? "blur"
                : tabiedObjectOverdrive_rdo.Checked ? "overdrive" : tabiedObjectMarquee_rdo.Checked ? "marquee" : "ball";
            string direction = tabiedMoveH_rdo.Checked ? "h" : tabiedMoveV_rdo.Checked ? "v" : tabiedMoveRandom_rdo.Checked ? "r" : "b";
            option = $"{shape}-{direction}-size{tabiedObjectSize_nud.Value}-speed{tabiedMoveSpeed_nud.Value}{DynamicPlusTokens()}";
            return null;
        }  // Dynamic 轉 config
        // Window 疊加清單--------------------------------------------------------------------------------------------
        private ListView tabiwOverlay_lvw;
        private void InitWindowOverlayList()
        {
            // 放在線條選項（外框、十字、斜角、線顏色）右側
            tabiwOverlay_lvw = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                HideSelection = false,
                MultiSelect = false,
                HeaderStyle = ColumnHeaderStyle.Nonclickable,
                Location = new Point(68, 268),
                Size = new Size(147, 86),
                ImeMode = ImeMode.Disable,
                ShowItemToolTips = true,
            };
            tabiwOverlay_lvw.Columns.Add("類型", 44);
            tabiwOverlay_lvw.Columns.Add("設定", 99);  // 剛好填滿清單寬度，完整內容用 tooltip 顯示
            var add = new Button { Text = "新增", Location = new Point(68, 357), Size = new Size(47, 23) };
            var edit = new Button { Text = "修改", Location = new Point(118, 357), Size = new Size(47, 23) };
            var copy = new Button { Text = "複製", Location = new Point(168, 357), Size = new Size(47, 23) };
            var moveUp = new Button { Text = "上移", Location = new Point(218, 357), Size = new Size(47, 23) };
            var moveDown = new Button { Text = "下移", Location = new Point(268, 357), Size = new Size(47, 23) };
            var remove = new Button { Text = "移除", Location = new Point(318, 357), Size = new Size(47, 23) };
            var clear = new Button { Text = "全清", Location = new Point(368, 357), Size = new Size(47, 23) };
            add.Click += (s, e) => EditWindowOverlay(null);
            edit.Click += (s, e) => { if (tabiwOverlay_lvw.SelectedItems.Count > 0) EditWindowOverlay(tabiwOverlay_lvw.SelectedItems[0]); };
            copy.Click += (s, e) => CopyWindowOverlay();
            moveUp.Click += (s, e) => MoveWindowOverlay(-1);
            moveDown.Click += (s, e) => MoveWindowOverlay(+1);
            remove.Click += (s, e) => RemoveWindowOverlay();
            clear.Click += (s, e) => tabiwOverlay_lvw.Items.Clear();
            toolTip.SetToolTip(copy, "複製選取的項目，加在它的下一列");
            toolTip.SetToolTip(moveUp, "選取項目往上移（越下面的越晚畫，會蓋住上面的）");
            toolTip.SetToolTip(moveDown, "選取項目往下移（越下面的越晚畫，會蓋住上面的）");
            tabiwOverlay_lvw.DoubleClick += (s, e) => { if (tabiwOverlay_lvw.SelectedItems.Count > 0) EditWindowOverlay(tabiwOverlay_lvw.SelectedItems[0]); };
            tabiwOverlay_lvw.KeyDown += (s, e) => { if (e.KeyCode == Keys.Delete) { RemoveWindowOverlay(); e.Handled = true; } };
            toolTip.SetToolTip(add, "新增額外疊加的窗口或線條，按 Generate 時依清單順序畫上（雙擊項目修改、Delete 移除）");
            toolTip.SetToolTip(clear, "清除清單上所有窗口 / 線條");
            tabimgeWindow.Controls.AddRange(new Control[] { tabiwOverlay_lvw, add, edit, copy, moveUp, moveDown, remove, clear });
        }  // 建立 Window 疊加清單
        private void EditWindowOverlay(ListViewItem item)
        {
            int imageW = int.TryParse(mnsW_txt.Text, out int w) ? w : 1920;
            int imageH = int.TryParse(mnsH_txt.Text, out int h) ? h : 1080;
            // 新增時預設：窗口在畫面中央、四分之一大小，白色
            WindowOverlay initial = item?.Tag as WindowOverlay ?? new WindowOverlay
            {
                Kind = OverlayKind.Window,
                X = imageW / 4, Y = imageH / 4, Width = imageW / 2, Height = imageH / 2,
                Color = Color.White,
            };
            using (WindowOverlayDialog dialog = new WindowOverlayDialog(initial))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK || dialog.Result == null) return;
                WindowOverlay result = dialog.Result;
                if (item == null)
                {
                    item = new ListViewItem(new[] { "", "" });
                    tabiwOverlay_lvw.Items.Add(item);
                }
                SetWindowOverlayItem(item, result);
                item.Selected = true;
                item.EnsureVisible();
            }
        }  // 新增 / 修改疊加項目
        private static void SetWindowOverlayItem(ListViewItem item, WindowOverlay overlay)
        {
            item.Tag = overlay;
            item.SubItems[0].Text = WindowOverlay.KindText(overlay.Kind);
            item.SubItems[1].Text = overlay.Description();
            item.ToolTipText = $"{item.SubItems[0].Text} {item.SubItems[1].Text}";
        }  // 疊加項目顯示內容
        private void CopyWindowOverlay()
        {
            if (tabiwOverlay_lvw.SelectedItems.Count == 0) return;
            int index = tabiwOverlay_lvw.SelectedIndices[0];
            var source = (WindowOverlay)tabiwOverlay_lvw.Items[index].Tag;
            var copy = new WindowOverlay { Kind = source.Kind, X = source.X, Y = source.Y, Width = source.Width, Height = source.Height, Color = source.Color };
            var item = new ListViewItem(new[] { "", "" });
            SetWindowOverlayItem(item, copy);
            tabiwOverlay_lvw.Items.Insert(index + 1, item);
            item.Selected = true;
            item.EnsureVisible();
        }  // 複製疊加項目（加在下一列）
        private void MoveWindowOverlay(int step)
        {
            // 清單順序就是繪製順序，往下移代表較晚畫、蓋在上面
            if (tabiwOverlay_lvw.SelectedItems.Count == 0) return;
            int index = tabiwOverlay_lvw.SelectedIndices[0];
            int target = index + step;
            if (target < 0 || target >= tabiwOverlay_lvw.Items.Count) return;
            ListViewItem item = tabiwOverlay_lvw.Items[index];
            tabiwOverlay_lvw.Items.RemoveAt(index);
            tabiwOverlay_lvw.Items.Insert(target, item);
            item.Selected = true;
            item.Focused = true;
            item.EnsureVisible();
        }  // 上移 / 下移疊加項目
        private void RemoveWindowOverlay()
        {
            if (tabiwOverlay_lvw.SelectedItems.Count == 0) return;
            int index = tabiwOverlay_lvw.SelectedIndices[0];
            tabiwOverlay_lvw.Items.RemoveAt(index);
            if (tabiwOverlay_lvw.Items.Count > 0)
            {
                tabiwOverlay_lvw.Items[Math.Min(index, tabiwOverlay_lvw.Items.Count - 1)].Selected = true;
            }
        }  // 移除疊加項目
    }
}
