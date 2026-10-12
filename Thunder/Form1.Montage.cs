using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;

namespace Thunder
{
    public partial class mainWindow
    {
        // 常用 mask 名稱（Mask 範本、Window 蒙太奇共用）；斜線顯示中文名稱
        private static readonly (string name, string text)[] MaskShortcuts =
        {
            ("dot1", "dot1"), ("dot2", "dot2"), ("sub1", "sub1"), ("sub2", "sub2"), ("row1", "row1"), ("row2", "row2"),
            ("col1", "col1"), ("col2", "col2"), ("2h1v", "2h1v"), ("1h2v", "1h2v"),
            ("rdiag1", "右斜1"), ("rdiag2", "右斜2"), ("rdiag3", "右斜3"), ("ldiag1", "左斜1"), ("ldiag2", "左斜2"), ("ldiag3", "左斜3"),
        };
        // Mask 頁範本按鈕-----------------------------------------------------------------------------------------------
        private void InitMaskTemplates()
        {
            // 子像素亮度按鈕右側兩欄：點下去依名稱填好格子（亮點用「選擇顏色」）
            var title = new Label { Text = "範本", AutoSize = true, Location = new Point(330, 50) };
            tabimgeMask.Controls.Add(title);
            for (int i = 0; i < MaskShortcuts.Length; i++)
            {
                var (name, text) = MaskShortcuts[i];
                int slot = i < 10 ? i : 10 + (i - 10) % 3 * 2 + (i - 10) / 3;  // 斜線同線寬的右斜 / 左斜排同一列
                var button = new Button { Text = text, Bounds = new Rectangle(330 + (slot % 2) * 62, 70 + (slot / 2) * 30, 58, 24) };
                button.Click += (s, e) => ApplyMaskTemplate(name);
                toolTip.SetToolTip(button, $"Mask 範本 {name}：亮點用「選擇顏色」（子像素圖樣用其亮度），暗點為黑");
                tabimgeMask.Controls.Add(button);
            }
        }  // 建立 Mask 範本按鈕
        private void ApplyMaskTemplate(string name)
        {
            // 依目前「選擇顏色」決定亮點；子像素圖樣（R+B / G）用它的亮度
            Color selected = tabiemPixelColor_btn.BackColor;
            int level = Math.Max(selected.R, Math.Max(selected.G, selected.B));
            if (level == 0) { selected = Color.White; level = 255; }  // 選黑色時圖樣會全黑，改用白色
            bool user = _userMasks.ContainsKey(name);  // 自訂 mask 保留原本的顏色
            if (!TryCustomMask(user ? name : $"{name}.{level}", out int width, out int height, out Color[] cells)) return;
            Color levelGray = Color.FromArgb(level, level, level);
            if (!tabiemPixel_rdo.Checked) tabiemPixel_rdo.Checked = true;  // 範本以像素為單位
            tabiemWNum_nud.Value = width;
            tabiemHNum_nud.Value = height;
            MaskPanelCreat(tabiemPixelClear_btn, EventArgs.Empty);
            foreach (Panel block in tabiemPixelPanel_pnl.Controls.OfType<Panel>())
            {
                var (x, y) = BlockCell(block);
                Color color = cells[y * width + x];
                block.BackColor = !user && color.ToArgb() == levelGray.ToArgb() ? selected : color;
            }
            ssrStatus_lbl.Text = $"Mask 範本：{name}（{width} x {height}）";
        }  // 套用 Mask 範本
        // Window 蒙太奇-------------------------------------------------------------------------------------------------
        private readonly List<CheckBox> _montageButtons = new List<CheckBox>();
        private VScrollBar tabiwMontageGray_vsc;
        private Label tabiwMontageGray_lbl;
        private const int MontageMargin = 50;  // 邊框 50px 內不放
        private int MontageLevel => 255 - tabiwMontageGray_vsc.Value;
        private void InitMontage()
        {
            // Window 右側分頁多一頁：點名稱按鈕把該圖樣排進畫面（鋪在 50px 邊框以內），可調亮度
            var tabs = tabimgeWindow.Controls.OfType<TabControl>().First();
            var page = new TabPage("蒙太奇") { UseVisualStyleBackColor = true, Padding = new Padding(6) };
            tabs.TabPages.Add(page);
            for (int i = 0; i < MaskShortcuts.Length; i++)
            {
                var (name, text) = MaskShortcuts[i];
                page.Controls.Add(CreateMontageToggle(name, text, i < 10 ? i : i + 2));  // 斜線另起一列：右斜一排、左斜一排
            }
            tabs.SelectedIndexChanged += (s, e) => { if (tabs.SelectedTab == page) SyncMontageUserMasks(); };  // 切到蒙太奇時加上自訂 mask
            tabiwMontageGray_lbl = new Label { Text = "Gray: 255", AutoSize = true, Location = new Point(250, 10) };
            tabiwMontageGray_vsc = new VScrollBar { Minimum = 0, Maximum = 255 + 9, LargeChange = 10, Bounds = new Rectangle(255, 30, 20, 220) };  // 值 = 255 - 亮度
            tabiwMontageGray_vsc.ValueChanged += (s, e) => tabiwMontageGray_lbl.Text = $"Gray: {MontageLevel}";
            page.Controls.AddRange(new Control[] { tabiwMontageGray_lbl, tabiwMontageGray_vsc });
            int[] levels = { 255, 192, 127, 64, 0 };
            for (int i = 0; i < levels.Length; i++)
            {
                int level = levels[i];
                var button = new Button { Text = level.ToString(), Bounds = new Rectangle(285, 30 + i * 49, 48, 24) };
                button.Click += (s, e) => tabiwMontageGray_vsc.Value = 255 - level;
                toolTip.SetToolTip(button, $"蒙太奇亮度設為 {level}");
                page.Controls.Add(button);
            }
            var bar = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 34, Padding = new Padding(0, 4, 0, 0) };
            var clear = new Button { Text = "清空", Size = new Size(70, 26) };
            var all = new Button { Text = "全選", Size = new Size(70, 26) };
            clear.Click += (s, e) => _montageButtons.ForEach(b => b.Checked = false);
            all.Click += (s, e) => _montageButtons.ForEach(b => b.Checked = true);
            bar.Controls.AddRange(new Control[] { clear, all });
            page.Controls.Add(bar);
            toolTip.SetToolTip(tabiwMontageGray_vsc, "蒙太奇圖樣亮點的亮度");
        }  // 建立蒙太奇分頁
        private CheckBox CreateMontageToggle(string name, string text, int slot)
        {
            var toggle = new CheckBox
            {
                Text = text, Tag = name, Appearance = Appearance.Button, TextAlign = ContentAlignment.MiddleCenter,
                Bounds = new Rectangle(10 + (slot % 3) * 76, 10 + (slot / 3) * 34, 70, 28),
            };
            toolTip.SetToolTip(toggle, $"{name}：按下後排進畫面，再按一次取消");
            _montageButtons.Add(toggle);
            return toggle;
        }  // 建立蒙太奇切換按鈕
        private const int MaxMontageUserMasks = 9;  // 內建按鈕下方剩 3 列
        private void SyncMontageUserMasks()
        {
            // 自訂 mask 排在內建按鈕下方（第 7 列起），依 ImgList 增減；已選取的保持選取
            RefreshUserMasks();
            List<string> names = _userMasks.Keys.Take(MaxMontageUserMasks).ToList();
            Control page = _montageButtons[0].Parent;
            foreach (CheckBox toggle in _montageButtons.Where(b => !MaskShortcuts.Any(m => m.name == (string)b.Tag)).ToList())
            {
                if (names.Contains((string)toggle.Tag)) continue;
                _montageButtons.Remove(toggle);
                page.Controls.Remove(toggle);
                toggle.Dispose();
            }
            for (int i = 0; i < names.Count; i++)
            {
                int slot = 18 + i;
                CheckBox toggle = _montageButtons.FirstOrDefault(b => (string)b.Tag == names[i]) ?? CreateMontageToggle(names[i], names[i], slot);
                toggle.Location = new Point(10 + (slot % 3) * 76, 10 + (slot / 3) * 34);
                if (toggle.Parent == null) page.Controls.Add(toggle);
            }
        }  // 同步自訂 mask 按鈕
        private List<string> MontageSelection => _montageButtons.Where(b => b.Checked).Select(b => (string)b.Tag).ToList();
        private void DrawMontage(Graphics g, int width, int height)
        {
            // 依選擇順序排成格狀，避開 50px 邊框；每格上方標示名稱
            List<string> names = MontageSelection;
            if (names.Count == 0) return;
            int margin = _renderingThumbnail ? Math.Max(1, Math.Min(width, height) / 20) : MontageMargin;  // 縮圖時邊框跟著縮小
            var area = Rectangle.FromLTRB(margin, margin, width - margin, height - margin);
            if (area.Width <= 0 || area.Height <= 0) return;
            int columns = Math.Max(1, (int)Math.Ceiling(Math.Sqrt(names.Count * (double)area.Width / area.Height)));
            int rows = (int)Math.Ceiling(names.Count / (double)columns);
            int gap = _renderingThumbnail ? 1 : 20, labelHeight = _renderingThumbnail ? 0 : 18;  // 縮圖不畫名稱
            float cellWidth = (area.Width + gap) / (float)columns, cellHeight = (area.Height + gap) / (float)rows;
            GraphicsState state = g.Save();
            g.SmoothingMode = SmoothingMode.None;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            using (Font font = new Font("Arial", 12f, GraphicsUnit.Pixel))
            using (Brush label = new SolidBrush(Color.FromArgb(160, 160, 160)))
            {
                for (int i = 0; i < names.Count; i++)
                {
                    int column = i % columns, row = i / columns;
                    int x0 = area.X + (int)(column * cellWidth), y0 = area.Y + (int)(row * cellHeight);
                    var tile = new Rectangle(x0, y0 + labelHeight, (int)cellWidth - gap, (int)cellHeight - gap - labelHeight);
                    if (tile.Width <= 0 || tile.Height <= 0) continue;
                    if (!TryCustomMask($"{names[i]}.{MontageLevel}", out int w, out int h, out Color[] cells)) continue;
                    using (var pattern = new Bitmap(w, h))
                    {
                        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++) pattern.SetPixel(x, y, cells[y * w + x]);
                        using (var brush = new TextureBrush(pattern, WrapMode.Tile))  // 以畫面原點對齊，像素相位一致
                        {
                            g.FillRectangle(brush, tile);
                        }
                    }
                    if (!_renderingThumbnail) g.DrawString(_montageButtons.FirstOrDefault(b => (string)b.Tag == names[i])?.Text ?? names[i], font, label, x0, y0 + 2);
                }
            }
            g.Restore(state);
        }  // 畫蒙太奇
        private void ApplyMontageConfig(string montage, string level)
        {
            // config：名稱以 + 連接，亮度省略為 255
            SyncMontageUserMasks();  // config 可以用自訂 mask 名稱
            _montageButtons.ForEach(b => b.Checked = false);
            tabiwMontageGray_vsc.Value = 0;
            if (montage == "") return;
            foreach (string raw in montage.Split('+').Select(n => n.Trim()).Where(n => n != ""))
            {
                string name = raw == "rdiag" || raw == "ldiag" ? raw + "2" : raw;  // 不寫線寬的舊寫法視為 2 點寬
                CheckBox toggle = _montageButtons.FirstOrDefault(b => (string)b.Tag == name);
                if (toggle == null) throw new FormatException($"蒙太奇名稱「{name}」只能是 {string.Join("、", MaskShortcuts.Select(m => m.name))} 或 ImgList 中的自訂 mask（UserMask，最多 {MaxMontageUserMasks} 個）");
                toggle.Checked = true;
            }
            if (level != "")
            {
                if (!int.TryParse(level, out int value) || value < 0 || value > 255) throw new FormatException($"蒙太奇亮度「{level}」需為 0~255");
                tabiwMontageGray_vsc.Value = 255 - value;
            }
        }  // 套用 config 的蒙太奇
        private string MontageConfigSuffix() =>
            MontageSelection.Count == 0 ? "" : $"|{string.Join("+", MontageSelection)}" + (MontageLevel == 255 ? "" : $"|{MontageLevel}");  // 寫進 config 的蒙太奇段
        // 全螢幕拖曳窗口（給 pictureWindow 呼叫）--------------------------------------------------------------------------
        public bool CanDragWindowPattern => tabimgeFuncList.Text == "Window" && !tabiwDiagonal_btn.Checked && !tabiwWinLocTwo_rdo.Checked;
        public Rectangle WindowPatternBounds(int width, int height)
        {
            // 目前 Window 設定的窗口位置與大小（圖片座標）
            int w = tabiwWinSizePercent_rdo.Checked ? width * (int)tabiwWinSizePercent_nud.Value / 100 : (int)tabiwWinSizePixelW_nud.Value;
            int h = tabiwWinSizePercent_rdo.Checked ? height * (int)tabiwWinSizePercent_nud.Value / 100 : (int)tabiwWinSizePixelH_nud.Value;
            if (tabiwWinLocPixcel_rdo.Checked) return new Rectangle((int)tabiwWinLocPixcelX_nud.Value, (int)tabiwWinLocPixcelY_nud.Value, w, h);
            return new Rectangle((width - w) / 2, (height - h) / 2, w, h);
        }  // 窗口範圍
        public Bitmap MoveWindowPattern(int x, int y, int width, int height)
        {
            // 窗口改成「指定 Pixel」並移到 (x, y)，依目前寬高重新產生；回傳新圖（呼叫端負責釋放）
            tabiwWinLocPixcelX_nud.Maximum = Math.Max(tabiwWinLocPixcelX_nud.Maximum, width);
            tabiwWinLocPixcelY_nud.Maximum = Math.Max(tabiwWinLocPixcelY_nud.Maximum, height);
            if (!tabiwWinLocPixcel_rdo.Checked) tabiwWinLocPixcel_rdo.Checked = true;
            tabiwWinLocPixcelX_nud.Value = Math.Max(0, Math.Min(tabiwWinLocPixcelX_nud.Maximum, x));
            tabiwWinLocPixcelY_nud.Value = Math.Max(0, Math.Min(tabiwWinLocPixcelY_nud.Maximum, y));
            GenerateImage("window", width, height);
            showimgPicture_pic.Image = mypicture.Getpicture();
            return new Bitmap(mypicture.Getpicture());
        }  // 移動窗口並重新產生
        public int WindowPatternPercent => (int)tabiwWinSizePercent_nud.Value;
        public Bitmap ResizeWindowPattern(int percent, int width, int height)
        {
            // 窗口大小加減 percent%（config 只支援百分比，指定 Pixel 大小時先換算成百分比），以窗口中心為基準
            Rectangle before = WindowPatternBounds(width, height);
            if (!tabiwWinSizePercent_rdo.Checked)
            {
                tabiwWinSizePercent_nud.Value = Math.Max(1, Math.Min(100, (int)Math.Round(before.Width * 100.0 / width)));
                tabiwWinSizePercent_rdo.Checked = true;
            }
            decimal next = Math.Max(Math.Max(1, tabiwWinSizePercent_nud.Minimum), Math.Min(tabiwWinSizePercent_nud.Maximum, tabiwWinSizePercent_nud.Value + percent));
            if (next == tabiwWinSizePercent_nud.Value && tabiwWinSizePercent_rdo.Checked && WindowPatternBounds(width, height) == before) return null;
            tabiwWinSizePercent_nud.Value = next;
            if (tabiwWinLocPixcel_rdo.Checked)
            {
                // 指定位置時維持窗口中心不動
                Rectangle after = WindowPatternBounds(width, height);
                tabiwWinLocPixcelX_nud.Maximum = Math.Max(tabiwWinLocPixcelX_nud.Maximum, width);
                tabiwWinLocPixcelY_nud.Maximum = Math.Max(tabiwWinLocPixcelY_nud.Maximum, height);
                tabiwWinLocPixcelX_nud.Value = Math.Max(0, before.X + before.Width / 2 - after.Width / 2);
                tabiwWinLocPixcelY_nud.Value = Math.Max(0, before.Y + before.Height / 2 - after.Height / 2);
            }
            GenerateImage("window", width, height);
            showimgPicture_pic.Image = mypicture.Getpicture();
            return new Bitmap(mypicture.Getpicture());
        }  // 調整窗口大小並重新產生
    }
}
