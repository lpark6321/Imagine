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
        // 預覽區：config 文字、最近使用、螢幕更新率-----------------------------------------------------------------
        private Label showimgConfig_lbl;
        private GroupBox recent_grp;
        private ListBox recent_lst;
        private ToolStripLabel mnsRefresh_lbl;
        private bool _configDraftFocused;  // 焦點在 ImgList 編輯區後，Enter / Generate 預覽草稿
        private RecentItem _shownItem;     // 目前預覽來自最近使用 / 草稿（null 表示來自目前分頁）
        private bool _dirListLoaded;       // DirList 只在第一次切換時自動讀取
        private const int RecentMax = 10;
        private class RecentItem
        {
            public string Name { get; private set; }
            public string Line { get; private set; }  // config 一行；圖檔為 null
            public string Path { get; private set; }  // 圖檔路徑；config 為 null
            // 相同內容只留一筆：config 比對名稱以外的欄位，圖檔比對路徑
            public string Key => Line != null
                ? "cfg:" + string.Join(",", Line.Split(',').Skip(1).Select(v => v.Trim().ToLower()))
                : "file:" + Path.ToLower();
            public string Source { get; private set; }  // 來源分頁：ImgList / ImgEditor / DirList
            public static RecentItem FromConfig(string line) => new RecentItem { Source = "ImgList", Name = line.Split(',')[0].Trim(), Line = line };
            public static RecentItem FromEditor(string line)
            {
                // ImgEditor 沒有名稱：以「類型_參數_選項」顯示（選項空白時省略）
                string[] v = line.Split(',').Select(s => s.Trim()).ToArray();
                string name = string.Join("_", v.Skip(1).Where(s => s != ""));
                return new RecentItem { Source = "ImgEditor", Name = name, Line = line };
            }
            public static RecentItem FromFile(string path) => new RecentItem { Source = "DirList", Name = System.IO.Path.GetFileName(path), Path = path };
            public override string ToString() => $"{Source} / {Name}";  // 清單顯示「分頁 / 名稱」；全螢幕左上角只用 Name
        }  // 最近使用項目
        private void InitPreviewPanel()
        {
            // Showimage_flp 由下往上排列（index 越大越上面）：縮圖上方插入 config 文字，最上方為最近使用
            int width = showimgPicture_pic.Width;
            showimgConfig_lbl = new Label
            {
                AutoSize = true, MaximumSize = new Size(width, 0), UseMnemonic = false, Visible = false,
                ForeColor = Color.DimGray, Margin = new Padding(3, 0, 3, 2)
            };
            float fontSize = showimgConfig_lbl.Font.Size == 12f ? 14f : showimgConfig_lbl.Font.Size + 2f;  // 12 調到 14，其他大 2 號
            showimgConfig_lbl.Font = new Font(showimgConfig_lbl.Font.FontFamily, fontSize, showimgConfig_lbl.Font.Style);
            Showimage_flp.Controls.Add(showimgConfig_lbl);
            Showimage_flp.Controls.SetChildIndex(showimgConfig_lbl, Showimage_flp.Controls.GetChildIndex(showimgSize_lbl));
            recent_lst = new ListBox { Dock = DockStyle.Fill, IntegralHeight = false, HorizontalScrollbar = true, DrawMode = DrawMode.OwnerDrawFixed };
            recent_lst.Font = new Font(recent_lst.Font.FontFamily, 11f);
            recent_lst.ItemHeight = recent_lst.Font.Height + 2;
            recent_lst.DrawItem += (s, e) =>
            {
                // 第 2、4、6… 項用 225 灰底區分；選取時用系統選取色
                if (e.Index < 0) return;
                bool selected = (e.State & DrawItemState.Selected) != 0;
                Color back = selected ? SystemColors.Highlight : e.Index % 2 == 1 ? Color.FromArgb(225, 225, 225) : recent_lst.BackColor;
                using (Brush brush = new SolidBrush(back))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }
                TextRenderer.DrawText(e.Graphics, recent_lst.Items[e.Index].ToString(), recent_lst.Font, e.Bounds,
                    selected ? SystemColors.HighlightText : recent_lst.ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            };
            recent_lst.DoubleClick += (s, e) => OpenRecent();
            recent_lst.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; OpenRecent(); }
            };
            toolTip.SetToolTip(recent_lst, "最近開啟 / 編輯的 10 張圖片：單擊選取、雙擊或 Enter 開啟");
            recent_grp = new GroupBox { Text = "最近使用", Width = width, Height = 100, Margin = new Padding(3, 2, 3, 2) };
            recent_grp.Controls.Add(recent_lst);
            Showimage_flp.Controls.Add(recent_grp);  // 最後加入 = 最上方
            Showimage_flp.Resize += (s, e) => LayoutRecent();
            // 工具列最右邊顯示目前螢幕的更新率（右對齊項目中第一個在最右邊）
            mnsRefresh_lbl = new ToolStripLabel { Alignment = ToolStripItemAlignment.Right, ToolTipText = "目前螢幕的更新率" };
            menuStrip.Items.Insert(0, mnsRefresh_lbl);
            // ImgList 編輯區：取得焦點後 Enter 或 Generate 預覽草稿
            foreach (Control field in new Control[] { cfgName_txt, cfgType_cmb, cfgParam_txt, cfgOption_txt })
            {
                field.Enter += (s, e) => _configDraftFocused = true;
                field.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; PreviewConfigDraft(); }
                };
            }
            tabilPatternList_lst.Enter += (s, e) => _configDraftFocused = false;  // 回到清單後 Generate 依清單選取
        }  // 建立預覽區的 config 文字、最近使用與更新率
        private void LayoutRecent()
        {
            // 最近使用填滿上方剩餘的高度
            if (recent_grp == null) return;
            int used = Showimage_flp.Controls.Cast<Control>()
                .Where(c => c != recent_grp && (c != showimgConfig_lbl || showimgConfig_lbl.Text != ""))
                .Sum(c => c.Height + c.Margin.Vertical);
            recent_grp.Height = Math.Max(60, Showimage_flp.ClientSize.Height - Showimage_flp.Padding.Vertical - used - recent_grp.Margin.Vertical);
        }  // 調整最近使用高度
        private void ShowPreviewInfo(int width, int height, string line)
        {
            // 圖片大小與 config（line 為 null 時隱藏 config 文字）
            showimgSize_lbl.Text = $"當前圖片大小：{width} x {height}";
            if (line != null)
            {
                line = string.Join(",", line.Split(',').Select(v => v.Trim())).TrimEnd(',');  // 去掉對齊用的 Tab 與空白
            }
            ShowConfigNote(line);
        }  // 更新預覽資訊
        private void ShowConfigNote(string text)
        {
            // 縮圖上方的文字（null 或空字串時隱藏）
            showimgConfig_lbl.Text = text ?? "";
            showimgConfig_lbl.Visible = !string.IsNullOrEmpty(text);
            toolTip.SetToolTip(showimgConfig_lbl, text);
            LayoutRecent();
        }  // 顯示 config 文字
        private void AddRecent(RecentItem item)
        {
            var existing = recent_lst.Items.Cast<RecentItem>().FirstOrDefault(i => i.Key == item.Key);
            if (existing != null) recent_lst.Items.Remove(existing);
            recent_lst.Items.Insert(0, item);
            while (recent_lst.Items.Count > RecentMax)
            {
                recent_lst.Items.RemoveAt(recent_lst.Items.Count - 1);
            }
            // 自繪清單不會自動計算水平捲軸寬度
            recent_lst.HorizontalExtent = recent_lst.Items.Cast<object>().Max(i => TextRenderer.MeasureText(i.ToString(), recent_lst.Font).Width) + 4;
        }  // 加入最近使用（最新的在最上面）
        private void OpenRecent()
        {
            if (!(recent_lst.SelectedItem is RecentItem item)) return;
            bool ok = item.Line != null ? ApplyConfigLine(item.Line) : LoadImageFile(item.Path);
            if (!ok) return;
            _shownItem = item;
            AddRecent(item);
            recent_lst.SelectedItem = item;
            ssrStatus_lbl.Text = $"最近使用：{item.Path ?? item.Line}";
            if (_pictureWindow != null && !_pictureWindow.IsDisposed)
            {
                FullScreen(recent_lst, EventArgs.Empty);  // 全螢幕開著時一起更新
            }
        }  // 開啟最近使用
        private void PreviewConfigDraft()
        {
            // 依 ImgList 編輯區目前內容產生預覽（不寫入清單）
            string name = cfgName_txt.Text.Trim();
            string[] values = { name == "" ? "預覽" : name, cfgType_cmb.SelectedItem?.ToString() ?? "", cfgParam_txt.Text.Trim(), cfgOption_txt.Text.Trim() };
            if (values[2] == "")
            {
                ssrStatus_lbl.Text = "預覽：請先填寫參數";
                return;
            }
            if (values.Any(v => v.Contains(",")))
            {
                MessageBox.Show("欄位內容不能包含逗號（逗號是欄位分隔符號）！", "Config", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string line = string.Join(",", values);
            if (!ApplyConfigLine(line)) return;
            var item = RecentItem.FromConfig(line);
            _shownItem = item;
            AddRecent(item);
            ssrStatus_lbl.Text = $"預覽草稿：{line}";
        }  // 預覽編輯區草稿
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string dmDeviceName;
            public short dmSpecVersion, dmDriverVersion, dmSize, dmDriverExtra;
            public int dmFields, dmPositionX, dmPositionY, dmDisplayOrientation, dmDisplayFixedOutput;
            public short dmColor, dmDuplex, dmYResolution, dmTTOption, dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel, dmPelsWidth, dmPelsHeight, dmDisplayFlags, dmDisplayFrequency;
            public int dmICMMethod, dmICMIntent, dmMediaType, dmDitherType, dmReserved1, dmReserved2, dmPanningWidth, dmPanningHeight;
        }
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);
        private void UpdateRefreshRate()
        {
            // 顯示下拉選單目前螢幕的更新率（沒有選擇時保留原本顯示）
            string device = mnsScreenlist_cmb.SelectedItem?.ToString();
            if (device == null || mnsRefresh_lbl == null) return;
            int hz = GetRefreshRate(device);
            mnsRefresh_lbl.Text = hz > 1 ? $"{hz} Hz" : "? Hz";
        }  // 更新螢幕更新率
        public static int GetRefreshRate(string device)
        {
            var mode = new DEVMODE { dmSize = (short)Marshal.SizeOf(typeof(DEVMODE)) };
            return EnumDisplaySettings(device, -1, ref mode) ? mode.dmDisplayFrequency : 0;  // -1：ENUM_CURRENT_SETTINGS
        }  // 螢幕目前的更新率（取不到時為 0）
        public string FullScreenPositionText(pictureWindow window)
        {
            // 全螢幕資訊列第一行：來源分頁、第幾張與名稱（多螢幕的非作用中視窗只顯示名稱）
            if (!ReferenceEquals(window, _pictureWindow)) return window.Caption ?? "";
            if (_shownItem != null) return $"{_shownItem.Source} / {_shownItem.Name}";
            if (tabControl.SelectedTab == tabImgList && tabilPatternList_lst.SelectedIndices.Count > 0)
            {
                int index = tabilPatternList_lst.SelectedIndices[0];
                return $"ImgList {index + 1}/{tabilPatternList_lst.Items.Count}　{tabilPatternList_lst.Items[index].SubItems[1].Text}";
            }
            if (tabControl.SelectedTab == tabDirList && tabdlPatternList_lvw.SelectedIndices.Count > 0)
            {
                int index = tabdlPatternList_lvw.SelectedIndices[0];
                return $"DirList {index + 1}/{tabdlPatternList_lvw.Items.Count}　{tabdlPatternList_lvw.Items[index].Text}";
            }
            return $"ImgEditor　{tabimgeFuncList.Text}";
        }  // 全螢幕資訊列的位置文字
        private void InitDirListReload()
        {
            // 上方一列：重新讀取、選擇資料夾、目前資料夾路徑
            var bar = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = false };
            var reload = new Button { Text = "重新讀取", AutoSize = true };
            reload.Click += (s, e) => { PopulateListView(); ssrStatus_lbl.Text = $"已重新讀取：{_dirListFolder}"; };
            toolTip.SetToolTip(reload, "重新掃描目前資料夾的圖檔（新增 / 刪除檔案後使用）");
            var choose = new Button { Text = "選擇資料夾...", AutoSize = true };
            choose.Click += (s, e) => ChooseDirListFolder();
            toolTip.SetToolTip(choose, "讀取其他資料夾的圖片（不必把程式放到圖片所在的資料夾）");
            dirListFolder_lbl = new Label { AutoSize = true, Margin = new Padding(6, 8, 3, 0), UseMnemonic = false };
            bar.Controls.AddRange(new Control[] { reload, choose, dirListFolder_lbl });
            tabDirList.Controls.Add(bar);
            tabdlPatternList_lvw.BringToFront();  // 清單最後 Dock，填滿按鈕以外的空間
            ShowDirListFolder();
        }  // DirList 重新讀取與選擇資料夾
        private string _dirListFolder = Application.StartupPath;  // DirList 讀取的資料夾（預設為程式資料夾）
        private Label dirListFolder_lbl;
        private void ChooseDirListFolder()
        {
            using (var dialog = new FolderBrowserDialog { Description = "選擇要讀取圖片的資料夾", SelectedPath = _dirListFolder })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                _dirListFolder = dialog.SelectedPath;
            }
            _dirListLoaded = true;
            ShowDirListFolder();
            PopulateListView();
            ssrStatus_lbl.Text = $"已讀取：{_dirListFolder}（{tabdlPatternList_lvw.Items.Count} 張）";
        }  // 選擇 DirList 資料夾
        private void ShowDirListFolder()
        {
            dirListFolder_lbl.Text = string.Equals(_dirListFolder.TrimEnd('\\'), Application.StartupPath.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase)
                ? $"程式資料夾：{_dirListFolder}" : _dirListFolder;
            toolTip.SetToolTip(dirListFolder_lbl, _dirListFolder);
        }  // 顯示目前資料夾
        private void InitImgListExport()
        {
            var export = new ToolStripMenuItem("全部匯出成 PNG...", null, (s, e) => ExportImgList());
            export.ToolTipText = "依目前寬高把 ImgList 每一項存成「編號_名稱.png」";
            var check = new ToolStripMenuItem("檢查全部設定", null, (s, e) => ValidateAllConfigLines());
            check.ToolTipText = "逐行檢查格式，設定有錯的列標紅（載入 config 時會自動檢查）";
            tabilPatternList_cms.Items.Add(new ToolStripSeparator());
            tabilPatternList_cms.Items.Add(check);
            tabilPatternList_cms.Items.Add(export);
        }  // ImgList 右鍵檢查與匯出
        private void ExportImgList()
        {
            if (tabilPatternList_lst.Items.Count == 0) return;
            string folder;
            using (var dialog = new FolderBrowserDialog { Description = $"選擇匯出資料夾（{mnsW_txt.Text} x {mnsH_txt.Text}，共 {tabilPatternList_lst.Items.Count} 張）", SelectedPath = Application.StartupPath })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                folder = dialog.SelectedPath;
            }
            ExportImgListTo(folder);
        }  // 選擇資料夾後匯出
        private int ExportImgListTo(string folder)
        {
            // 逐項產生並存檔；有問題的項目略過，最後一次列出
            var errors = new List<string>();
            int saved = 0, total = tabilPatternList_lst.Items.Count;
            char[] invalid = Path.GetInvalidFileNameChars();
            ssrProgressbar_prg.Maximum = total;
            Cursor = Cursors.WaitCursor;
            try
            {
                foreach (ListViewItem item in tabilPatternList_lst.Items)
                {
                    string name = new string(item.SubItems[1].Text.Trim().Select(ch => invalid.Contains(ch) ? '_' : ch).ToArray());
                    string fileName = $"{item.Index + 1:000}_{name}.png";
                    int before = errors.Count;
                    if (ApplyConfigLine(item.Tag.ToString(), errors) && showimgPicture_pic.Image != null)
                    {
                        showimgPicture_pic.Image.Save(Path.Combine(folder, fileName), ImageFormat.Png);
                        saved++;
                    }
                    else if (errors.Count == before)
                    {
                        errors.Add($"「{name}」無法產生圖片");
                    }
                    ssrProgressbar_prg.Value = item.Index + 1;
                    ssrStatus_lbl.Text = $"匯出中 {item.Index + 1}/{total}：{fileName}";
                    Application.DoEvents();  // 更新進度列
                }
            }
            finally
            {
                Cursor = Cursors.Default;
                ssrProgressbar_prg.Value = 0;
            }
            ssrStatus_lbl.Text = $"已匯出 {saved}/{total} 張到 {folder}";
            if (errors.Count > 0)
            {
                MessageBox.Show($"以下 {errors.Count} 項沒有匯出：\r\n\r\n・" + string.Join("\r\n・", errors), "全部匯出", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            // 預覽恢復成目前選取的項目
            if (tabilPatternList_lst.SelectedItems.Count > 0)
            {
                ApplyConfigLine(tabilPatternList_lst.SelectedItems[0].Tag.ToString());
            }
            return saved;
        }  // 匯出 ImgList 全部圖片
        private static void DrawScreenshot(Graphics g, Image image, int x, int y, int width, int height)
        {
            // Window 匯入的截圖：比目標大時 1:1 從左上裁切，比目標小時拉伸（寬、高分開判斷）
            if (image == null || width <= 0 || height <= 0) return;
            var source = new Rectangle(0, 0, Math.Min(image.Width, width), Math.Min(image.Height, height));
            GraphicsState state = g.Save();
            g.InterpolationMode = InterpolationMode.NearestNeighbor;  // 拉伸時保持像素銳利
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.DrawImage(image, new Rectangle(x, y, width, height), source, GraphicsUnit.Pixel);
            g.Restore(state);
        }  // 畫截圖（大裁切、小拉伸）
        private void InitMassTooltip()
        {
            string tip = "勾選後按 AutoSave（Ctrl+S）批量輸出純色圖：\r\n" +
                         "・從 0 開始，每隔設定的階數輸出一張，最後一張為背景色\r\n" +
                         "　例：背景白 (255)、每 32 階 → 0、32、64 … 224、255\r\n" +
                         "・各色版不超過背景色（背景 255.0.0 → 只輸出紅色階）\r\n" +
                         "・檔名為「檔名_灰階.png」，存在程式資料夾，同名時加 (2)、(3)…\r\n" +
                         "・只輸出背景純色，框線、文字等設定不會畫上";
            toolTip.SetToolTip(tabiefMass_chk, tip);
            toolTip.SetToolTip(tabiefMassNum_nud, "每幾階輸出一張（1 = 每一階都輸出）");
        }  // 大量建圖說明
        private void InitMaskZoom()
        {
            // Mask 格子與 Gradient 色條面板旁加「放大」：移到可拖拉大小的視窗編輯，關閉後放回原位
            foreach (var (panel, box, clear) in new[] {
                (tabiemPixelPanel_pnl, (Control)tabimgeMask, tabiemPixelClear_btn),
                (tabiwCMaskPixelPanel_pnl, (Control)tabiwCustomMask_grp, tabiwCMaskPixelClear_btn) })
            {
                AddZoomButton(panel, box, new Rectangle(clear.Right + 6, clear.Top, 44, 24), "Mask");
            }
            AddZoomButton(tabigOtherSplit_pnl, tabigOther_grp, new Rectangle(285, 160, 62, 24), "ColorBar");
            // Window 自製圖的 ColorBar 空間較小：四色 / 八色按鈕稍微上移、群組加高放入放大按鈕
            tabiwCGradOther_grp.Height += 5;
            foreach (Button button in new[] { tabiwCGradFourColor_btn, tabiwCGradEightColor_btn })
            {
                button.SetBounds(button.Left, 128, button.Width, 22);
            }
            AddZoomButton(tabiwCGradOtherSplit_pnl, tabiwCGradOther_grp, new Rectangle(280, 152, 62, 20), "ColorBar");
        }  // Mask / ColorBar 放大按鈕
        private void AddZoomButton(Panel panel, Control box, Rectangle bounds, string title)
        {
            var zoom = new Button { Text = "放大", Bounds = bounds };
            zoom.Click += (s, e) => ZoomBlockPanel(panel, title);
            toolTip.SetToolTip(zoom, $"開啟可拖拉大小的視窗編輯 {title}（格子多時比較好點選）");
            box.Controls.Add(zoom);
            panel.Resize += (s, e) => LayoutBlocks(panel);
        }  // 加入放大按鈕
        private static (int x, int y) BlockCell(Control block)
        {
            // Mask 格子 Tag 為「x,y」；ColorBar 色條為「V,序號」（直排）或「H,序號」（橫排）
            string[] xy = block.Tag.ToString().Split(',');
            if (xy[0] == "V") return (int.Parse(xy[1]), 0);
            if (xy[0] == "H") return (0, int.Parse(xy[1]));
            return (int.Parse(xy[0]), int.Parse(xy[1]));
        }  // 格子座標
        private static void LayoutBlocks(Panel panel)
        {
            // 依面板大小重新排列格子（保留顏色）
            var blocks = panel.Controls.OfType<Panel>().ToList();
            if (blocks.Count == 0) return;
            int columns = blocks.Max(b => BlockCell(b).x) + 1, rows = blocks.Max(b => BlockCell(b).y) + 1;
            int blockWidth = Math.Max(1, panel.Width / columns), blockHeight = Math.Max(1, panel.Height / rows);
            panel.SuspendLayout();
            foreach (Panel block in blocks)
            {
                var (x, y) = BlockCell(block);
                block.SetBounds(x * blockWidth, y * blockHeight, blockWidth, blockHeight);
            }
            panel.ResumeLayout();
        }  // 重新排列格子
        private void ZoomBlockPanel(Panel panel, string title)
        {
            // 每次開啟建立新視窗、關閉即釋放；格子本身直接搬過去用，不另外複製
            var blocks = panel.Controls.OfType<Panel>().ToList();
            if (blocks.Count == 0) return;
            int columns = blocks.Max(b => BlockCell(b).x) + 1, rows = blocks.Max(b => BlockCell(b).y) + 1;
            Control parent = panel.Parent;
            Rectangle bounds = panel.Bounds;
            int index = parent.Controls.GetChildIndex(panel);
            var coord = new Label { Dock = DockStyle.Bottom, Height = 22, TextAlign = ContentAlignment.MiddleLeft, Text = "左鍵填色、右鍵清除；拖拉視窗邊緣可調整大小，Esc 關閉" };
            EventHandler showCoord = (s, e) => coord.Text = $"座標 {((Control)s).Tag}";
            Rectangle area = Screen.FromControl(this).WorkingArea;
            Size client;
            if (columns > 1 && rows > 1)
            {
                // Mask：格子盡量為正方形
                int cell = Math.Max(6, Math.Min((area.Width - 80) / columns, (area.Height - 120) / rows));
                cell = Math.Min(cell, Math.Max(24, 640 / Math.Max(columns, rows)));
                client = new Size(columns * cell, rows * cell);
            }
            else
            {
                // ColorBar：直排色條用橫長視窗、橫排用直長視窗
                client = columns > 1 ? new Size(720, 400) : new Size(400, Math.Min(640, area.Height - 120));
            }
            using (var form = new Form
            {
                Text = $"{title} 放大編輯（{columns} x {rows}）", ShowIcon = false, ShowInTaskbar = false, MinimizeBox = false,
                StartPosition = FormStartPosition.CenterParent, KeyPreview = true,
                ClientSize = new Size(client.Width, client.Height + coord.Height), MinimumSize = new Size(200, 200),
            })
            {
                form.KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) form.Close(); };
                blocks.ForEach(b => b.MouseEnter += showCoord);
                parent.Controls.Remove(panel);
                panel.Dock = DockStyle.Fill;
                form.Controls.Add(panel);
                form.Controls.Add(coord);  // 後加入的先 Dock，座標列在下方
                try
                {
                    form.ShowDialog(this);
                }
                finally
                {
                    form.Controls.Remove(panel);
                    blocks.ForEach(b => b.MouseEnter -= showCoord);
                    panel.Dock = DockStyle.None;
                    panel.Bounds = bounds;
                    parent.Controls.Add(panel);
                    parent.Controls.SetChildIndex(panel, index);
                    LayoutBlocks(panel);
                }
            }
        }  // 放大編輯視窗
    }
}
