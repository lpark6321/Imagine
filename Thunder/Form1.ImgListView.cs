using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Thunder
{
    public partial class mainWindow
    {
        // ImgList 搜尋、分組、縮圖-------------------------------------------------------------------------------------
        private TextBox tabilSearch_txt;
        private Label tabilSearchCount_lbl;
        private CheckBox tabilThumbnail_chk;
        private void InitImgListView()
        {
            // 清單上方一列：搜尋框、清除、找到幾筆、縮圖切換
            var bar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 28, WrapContents = false, Padding = new Padding(0, 2, 0, 0) };
            tabilSearch_txt = new TextBox { Width = 200, ImeMode = ImeMode.NoControl };  // 可打中文名稱
            var clear = new Button { Text = "✕", Width = 26, Height = 23, Margin = new Padding(0, 0, 6, 0) };
            tabilSearchCount_lbl = new Label { AutoSize = true, Margin = new Padding(0, 6, 12, 0), ForeColor = Color.DimGray };
            tabilThumbnail_chk = new CheckBox { Text = "縮圖", AutoSize = true, Margin = new Padding(0, 4, 0, 0) };
            bar.Controls.AddRange(new Control[] { new Label { Text = "搜尋", AutoSize = true, Margin = new Padding(3, 6, 3, 0) }, tabilSearch_txt, clear, tabilSearchCount_lbl, tabilThumbnail_chk });
            tabImgList.Controls.Add(bar);  // 加在最後 = 最先停靠，佔用上方；清單（Fill）填滿其餘
            tabilSearch_txt.TextChanged += (s, e) => { if (!_suppressFilter) ApplyFilter(null); };
            tabilSearch_txt.KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) { tabilSearch_txt.Clear(); e.SuppressKeyPress = true; } };
            clear.Click += (s, e) => tabilSearch_txt.Clear();
            tabilThumbnail_chk.CheckedChanged += (s, e) => SetThumbnailView(tabilThumbnail_chk.Checked);
            toolTip.SetToolTip(tabilSearch_txt, "依名稱、類型、參數、選項或群組名稱篩選（空格分隔多個關鍵字，需全部符合；Esc 清除）\r\n篩選中幻燈片只播放找到的項目");
            toolTip.SetToolTip(clear, "清除搜尋");
            toolTip.SetToolTip(tabilThumbnail_chk, "以縮圖顯示每一行（只產生畫面上看得到的項目；Mask 類圖樣畫成設定時的格子示意圖）");
            // 右鍵：群組展開 / 收合（只有 config 有分組時顯示）
            var expandAll = new ToolStripMenuItem("展開全部群組", null, (s, e) => SetAllGroupsCollapsed(false));
            var collapseAll = new ToolStripMenuItem("收合全部群組", null, (s, e) => SetAllGroupsCollapsed(true));
            var onlyThis = new ToolStripMenuItem("只展開這一組", null, (s, e) => ExpandOnlySelectedGroup()) { ToolTipText = "其他群組收合；幻燈片只會播放展開的群組" };
            var html = new ToolStripMenuItem("匯出說明文件（HTML）...", null, (s, e) => ExportConfigHtml()) { ToolTipText = "整份 config 匯出成 HTML，每行附縮圖、名稱、類型、參數與說明" };
            var groupSeparator = new ToolStripSeparator();
            tabilPatternList_cms.Items.AddRange(new ToolStripItem[] { html, groupSeparator, expandAll, collapseAll, onlyThis });
            tabilPatternList_cms.Opening += (s, e) =>
            {
                bool grouped = tabilPatternList_lst.ShowGroups && tabilPatternList_lst.Groups.Count > 0;
                foreach (ToolStripItem item in new ToolStripItem[] { groupSeparator, expandAll, collapseAll, onlyThis }) item.Visible = grouped;
            };
            InitThumbnails();
            // ListView 重建 handle 時（例如第一次開啟 ShowItemToolTips）項目的群組會遺失：先開好，並在重建前後保存 / 還原
            tabilPatternList_lst.ShowItemToolTips = true;
            Dictionary<ListViewItem, ListViewGroup> savedGroups = null;
            HashSet<string> savedCollapsed = null;
            tabilPatternList_lst.HandleDestroyed += (s, e) =>
            {
                if (!tabilPatternList_lst.RecreatingHandle) return;
                savedGroups = tabilPatternList_lst.Items.Cast<ListViewItem>().Where(i => i.Group != null).ToDictionary(i => i, i => i.Group);
                savedCollapsed = CollapsedGroupNames();
            };
            tabilPatternList_lst.HandleCreated += (s, e) =>
            {
                if (savedGroups == null) return;
                BeginInvoke((Action)(() =>
                {
                    if (savedGroups == null) return;
                    foreach (var pair in savedGroups) if (pair.Key.ListView == tabilPatternList_lst && pair.Key.Group != pair.Value) pair.Key.Group = pair.Value;
                    ApplyGroupStates(savedCollapsed);
                    savedGroups = null;
                }));
            };
        }  // 建立搜尋列與群組選單
        // 完整清單（搜尋時被篩掉的項目也算在內）-------------------------------------------------------------------------
        private List<(ListViewItem item, ListViewGroup group)> _filterAll;  // 搜尋中：完整清單與各自的群組；沒有搜尋時為 null
        private bool IsFiltering => _filterAll != null;
        private IEnumerable<(ListViewItem item, ListViewGroup group)> AllConfigEntries() =>
            _filterAll ?? tabilPatternList_lst.Items.Cast<ListViewItem>().Select(i => (i, i.Group));
        private IEnumerable<ListViewItem> AllConfigItems() => AllConfigEntries().Select(e => e.item);
        private static readonly Regex GroupHeaderRegex = new Regex(@"^\s*#\s*==\s*(.*?)\s*=*\s*$");
        private static string GroupName(ListViewGroup group) => group?.Name ?? "";
        private List<string> ConfigLinesWithGroups()
        {
            // 依清單順序輸出 config；群組改變時先寫一行 #== 群組 ==（未分組的不寫）
            var lines = new List<string>();
            string current = "";
            foreach (var (item, group) in AllConfigEntries())
            {
                string name = GroupName(group);
                if (name != current && name != "") lines.Add($"#== {name} ==");
                current = name;
                lines.Add(item.Tag.ToString());
            }
            return lines;
        }  // 含群組標題的 config 行
        private void FillPatternList(IEnumerable<string> lines)
        {
            // 載入 config 行：#== 名稱 == 開始一個群組，其他 # 開頭為註解；沒有任何群組時不顯示群組
            var collapsed = CollapsedGroupNames();
            var entries = new List<(ListViewItem, string)>();
            string groupName = "";
            foreach (string raw in lines)
            {
                Match header = GroupHeaderRegex.Match(raw);
                if (header.Success && header.Groups[1].Value != "")
                {
                    groupName = header.Groups[1].Value;
                    continue;
                }
                ListViewItem item = CreateConfigItem(raw);
                if (item != null) entries.Add((item, groupName));
            }
            bool grouped = entries.Any(e => e.Item2 != "");
            tabilPatternList_lst.BeginUpdate();
            try
            {
                tabilPatternList_lst.Items.Clear();
                tabilPatternList_lst.Groups.Clear();
                // 切換 ShowGroups 會重建 handle，項目的群組會遺失：清單清空時先切好，再加入項目
                if (tabilPatternList_lst.ShowGroups != grouped) tabilPatternList_lst.ShowGroups = grouped;
                ListViewGroup current = null;
                foreach (var (item, name) in entries)
                {
                    tabilPatternList_lst.Items.Add(item);
                    if (!grouped) continue;
                    if (current == null || current.Name != name)
                    {
                        current = tabilPatternList_lst.Groups.Cast<ListViewGroup>().FirstOrDefault(g => g.Name == name)
                                  ?? tabilPatternList_lst.Groups.Add(name, name);
                    }
                    item.Group = current;
                }
                RenumberPatternList();
            }
            finally
            {
                tabilPatternList_lst.EndUpdate();
            }
            ApplyGroupStates(collapsed);
        }  // 填入 ImgList
        private void AdoptNeighborGroup(ListViewItem item)
        {
            // 分組時新加入或移動的項目跟著上一列的群組（在最上面時跟下一列）
            if (!tabilPatternList_lst.ShowGroups || item.ListView == null) return;
            int index = item.Index;
            ListViewItem neighbor = index > 0 ? tabilPatternList_lst.Items[index - 1] : tabilPatternList_lst.Items.Count > 1 ? tabilPatternList_lst.Items[1] : null;
            ListViewGroup group = neighbor?.Group ?? tabilPatternList_lst.Groups.Cast<ListViewGroup>().FirstOrDefault();
            if (item.Group != group) item.Group = group;
        }  // 跟著鄰近項目的群組
        private void UpdateGroupHeaders()
        {
            // 群組標題顯示項目數；未分組的顯示「（未分組）」
            foreach (ListViewGroup group in tabilPatternList_lst.Groups)
            {
                string header = $"{(group.Name == "" ? "（未分組）" : group.Name)}（{group.Items.Count}）";
                if (group.Header != header) group.Header = header;
            }
        }  // 更新群組標題
        // 群組收合（.NET Framework 沒有內建，用 LVM_SETGROUPINFO 設定 LVGS_COLLAPSIBLE）
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct LVGROUP
        {
            public int cbSize, mask; public IntPtr pszHeader; public int cchHeader; public IntPtr pszFooter; public int cchFooter; public int iGroupId, stateMask, state, uAlign;
            public IntPtr pszSubtitle; public int cchSubtitle; public IntPtr pszTask; public int cchTask; public IntPtr pszDescriptionTop; public int cchDescriptionTop;
            public IntPtr pszDescriptionBottom; public int cchDescriptionBottom; public int iTitleImage, iExtendedImage, iFirstItem, cItems; public IntPtr pszSubsetTitle; public int cchSubsetTitle;
        }
        [DllImport("user32.dll")] private static extern IntPtr SendMessage(IntPtr hwnd, int msg, IntPtr wParam, ref LVGROUP group);
        [DllImport("user32.dll", EntryPoint = "SendMessage")] private static extern IntPtr SendMessageInt(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);
        private const int LVM_SETGROUPINFO = 0x1000 + 147, LVM_GETGROUPSTATE = 0x1000 + 92, LVGF_STATE = 0x4, LVGS_COLLAPSED = 0x1, LVGS_COLLAPSIBLE = 0x8;
        private static readonly PropertyInfo GroupIdProperty = typeof(ListViewGroup).GetProperty("ID", BindingFlags.Instance | BindingFlags.NonPublic);
        private static int GroupId(ListViewGroup group) => GroupIdProperty != null ? (int)GroupIdProperty.GetValue(group) : -1;
        private bool IsGroupCollapsed(ListViewGroup group)
        {
            if (group == null || !tabilPatternList_lst.ShowGroups || !tabilPatternList_lst.IsHandleCreated) return false;
            int id = GroupId(group);
            return id >= 0 && ((int)SendMessageInt(tabilPatternList_lst.Handle, LVM_GETGROUPSTATE, (IntPtr)id, (IntPtr)LVGS_COLLAPSED) & LVGS_COLLAPSED) != 0;
        }  // 群組是否收合
        private void SetGroupState(ListViewGroup group, bool collapsed)
        {
            int id = GroupId(group);
            if (id < 0 || !tabilPatternList_lst.IsHandleCreated) return;
            var info = new LVGROUP { cbSize = Marshal.SizeOf(typeof(LVGROUP)), mask = LVGF_STATE, iGroupId = id, stateMask = LVGS_COLLAPSIBLE | LVGS_COLLAPSED, state = LVGS_COLLAPSIBLE | (collapsed ? LVGS_COLLAPSED : 0) };
            SendMessage(tabilPatternList_lst.Handle, LVM_SETGROUPINFO, (IntPtr)id, ref info);
        }  // 設定群組可收合 / 收合
        private HashSet<string> CollapsedGroupNames() =>
            new HashSet<string>(tabilPatternList_lst.Groups.Cast<ListViewGroup>().Where(IsGroupCollapsed).Select(GroupName));  // 目前收合的群組
        private void ApplyGroupStates(HashSet<string> collapsed)
        {
            if (!tabilPatternList_lst.ShowGroups) return;
            foreach (ListViewGroup group in tabilPatternList_lst.Groups) SetGroupState(group, collapsed?.Contains(GroupName(group)) == true);
        }  // 套用群組狀態（重建清單後）
        private void SetAllGroupsCollapsed(bool collapsed)
        {
            foreach (ListViewGroup group in tabilPatternList_lst.Groups) SetGroupState(group, collapsed);
        }  // 全部展開 / 收合
        private void ExpandOnlySelectedGroup()
        {
            ListViewGroup selected = tabilPatternList_lst.SelectedItems.Count > 0 ? tabilPatternList_lst.SelectedItems[0].Group : null;
            if (selected == null) return;
            foreach (ListViewGroup group in tabilPatternList_lst.Groups) SetGroupState(group, group != selected);
            ssrStatus_lbl.Text = $"只展開「{GroupName(selected)}」，幻燈片只播放這一組";
        }  // 只展開選取項目的群組
        private bool IsSlideVisible(ListViewItem item) => !IsGroupCollapsed(item.Group);  // 收合的群組不播放
        // 搜尋---------------------------------------------------------------------------------------------------------
        private void ApplyFilter(ListViewItem alwaysShow)
        {
            // 依搜尋框篩選；alwaysShow（例如剛編輯的項目）即使不符合也保留
            string[] terms = tabilSearch_txt.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(t => t.ToLower()).ToArray();
            ListViewItem selected = alwaysShow ?? (tabilPatternList_lst.SelectedItems.Count > 0 ? tabilPatternList_lst.SelectedItems[0] : null);
            var collapsed = CollapsedGroupNames();
            tabilPatternList_lst.BeginUpdate();
            _movingConfigLine = true;  // 重建清單時不重新產生圖片
            try
            {
                RestoreFilter();
                if (terms.Length == 0)
                {
                    tabilSearchCount_lbl.Text = "";
                }
                else
                {
                    _filterAll = tabilPatternList_lst.Items.Cast<ListViewItem>().Select(i => (i, i.Group)).ToList();
                    tabilPatternList_lst.Items.Clear();
                    foreach (var (item, group) in _filterAll)
                    {
                        string text = $"{item.Tag} {GroupName(group)}".ToLower();
                        if (item != selected || alwaysShow == null)
                        {
                            if (!terms.All(text.Contains)) continue;
                        }
                        tabilPatternList_lst.Items.Add(item);
                        if (group != null) item.Group = group;
                    }
                    tabilSearchCount_lbl.Text = $"找到 {tabilPatternList_lst.Items.Count} / {_filterAll.Count}";
                }
                UpdateGroupHeaders();
                if (selected != null && selected.ListView == tabilPatternList_lst)
                {
                    selected.Selected = true;
                    selected.Focused = true;
                    selected.EnsureVisible();
                }
            }
            finally
            {
                _movingConfigLine = false;
                tabilPatternList_lst.EndUpdate();
            }
            ApplyGroupStates(collapsed);
        }  // 套用搜尋
        private void RestoreFilter()
        {
            // 把篩掉的項目依原本順序放回清單
            if (_filterAll == null) return;
            var all = _filterAll;
            _filterAll = null;
            tabilPatternList_lst.Items.Clear();
            foreach (var (item, group) in all)
            {
                tabilPatternList_lst.Items.Add(item);
                if (group != null) item.Group = group;
            }
        }  // 取消篩選
        private sealed class FilterSuspension : IDisposable
        {
            private readonly mainWindow _owner;
            public FilterSuspension(mainWindow owner) { _owner = owner; }
            public void Dispose()
            {
                // 編輯完重新篩選，並保留目前選取的項目
                var list = _owner.tabilPatternList_lst;
                _owner.ApplyFilter(list.SelectedItems.Count > 0 ? list.SelectedItems[0] : null);
            }
        }  // 暫停篩選
        private IDisposable SuspendFilter()
        {
            // 搜尋中要新增、修改、刪除時：先放回完整清單，結束後再篩選（沒有搜尋時什麼都不做）
            if (!IsFiltering) return null;
            ListViewItem selected = tabilPatternList_lst.SelectedItems.Count > 0 ? tabilPatternList_lst.SelectedItems[0] : null;
            var collapsed = CollapsedGroupNames();
            _movingConfigLine = true;
            try
            {
                tabilPatternList_lst.BeginUpdate();
                RestoreFilter();
                tabilPatternList_lst.EndUpdate();
                ApplyGroupStates(collapsed);
                if (selected != null) { selected.Selected = true; selected.Focused = true; }
            }
            finally
            {
                _movingConfigLine = false;
            }
            return new FilterSuspension(this);
        }  // 暫停篩選
        private bool BlockWhileFiltering(string action)
        {
            if (!IsFiltering) return false;
            ssrStatus_lbl.Text = $"搜尋中無法{action}，請先清除搜尋";
            return true;
        }  // 搜尋中不能調整順序
        private void ClearSearchSilently()
        {
            // 換 config 或復原時清除搜尋（不重建清單，呼叫端會重新填入）
            _filterAll = null;
            if (tabilSearch_txt == null || tabilSearch_txt.Text == "") return;
            _suppressFilter = true;
            try
            {
                tabilSearch_txt.Text = "";
            }
            finally
            {
                _suppressFilter = false;
            }
            tabilSearchCount_lbl.Text = "";
        }  // 清除搜尋
        private bool _suppressFilter;  // 清除搜尋框時不觸發篩選
        // 拖入圖片：每張加一行 Image------------------------------------------------------------------------------------
        private static readonly string[] ImageExtensions = { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tif", ".tiff" };
        private static bool IsImageFile(string path) => ImageExtensions.Contains(Path.GetExtension(path).ToLower());
        private void AddImageFilesToList(IEnumerable<string> files)
        {
            // 加在選取項目的下一列（沒有選取時加在最後），名稱用檔名；在 config 資料夾內的圖寫成相對路徑
            var images = files.Where(IsImageFile).OrderBy(f => f, StringComparer.OrdinalIgnoreCase).ToList();
            var skipped = images.Where(f => f.Contains(",")).ToList();  // 逗號是 config 的欄位分隔，不能出現在路徑
            images = images.Except(skipped).ToList();
            if (images.Count == 0)
            {
                if (skipped.Count > 0) MessageBox.Show("路徑含有逗號的圖檔無法加入：\r\n" + string.Join("\r\n", skipped), "加入圖片", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string source = tabilPatternList_lst.Tag?.ToString();
            string folder = (!string.IsNullOrEmpty(source) && File.Exists(source) ? Path.GetDirectoryName(source) : Application.StartupPath).TrimEnd('\\') + "\\";
            ListViewItem last = null;
            using (SuspendFilter())
            {
                PushConfigUndo();  // 記錄復原點
                int index = tabilPatternList_lst.SelectedIndices.Count > 0 ? tabilPatternList_lst.SelectedIndices[0] + 1 : tabilPatternList_lst.Items.Count;
                foreach (string file in images)
                {
                    string path = file.StartsWith(folder, StringComparison.OrdinalIgnoreCase) ? file.Substring(folder.Length) : file;
                    string name = Path.GetFileNameWithoutExtension(file);
                    last = CreateConfigItem($"{name},Image,{path},fit");
                    tabilPatternList_lst.Items.Insert(index++, last);
                    AdoptNeighborGroup(last);
                    MarkConfigItem(last, null);
                }
                RenumberPatternList();
                last.Selected = true;  // 預覽最後一張
                last.Focused = true;
                last.EnsureVisible();
            }
            ssrStatus_lbl.Text = $"已加入 {images.Count} 張圖片（Image 類型）{(skipped.Count > 0 ? $"，{skipped.Count} 張路徑含逗號未加入" : "")}，要保留請存 config 檔";
        }  // 拖入圖片
        // 隨機播放-----------------------------------------------------------------------------------------------------
        private readonly Random _slideRandom = new Random();
        private readonly List<ListViewItem> _shuffle = new List<ListViewItem>();
        public bool ShowRandomSlide(pictureWindow window)
        {
            // 幻燈片隨機順序：每輪把可播放的項目打亂，播完再重新打亂（新一輪第一張避開剛播過的）
            ListView list = tabControl.SelectedTab == tabImgList ? tabilPatternList_lst : tabControl.SelectedTab == tabDirList ? tabdlPatternList_lvw : null;
            if (list == null) return false;
            var playable = list.Items.Cast<ListViewItem>().Where(i => list != tabilPatternList_lst || IsSlideVisible(i)).ToList();
            if (playable.Count == 0) return false;
            _shuffle.RemoveAll(i => !playable.Contains(i));
            if (_shuffle.Count == 0)
            {
                ListViewItem current = list.SelectedItems.Count > 0 ? list.SelectedItems[0] : null;
                _shuffle.AddRange(playable.OrderBy(_ => _slideRandom.Next()));
                if (_shuffle.Count > 1 && _shuffle[0] == current) { _shuffle.RemoveAt(0); _shuffle.Add(current); }
            }
            ListViewItem next = _shuffle[0];
            _shuffle.RemoveAt(0);
            list.SelectedItems.Clear();
            next.Selected = true;
            next.Focused = true;
            next.EnsureVisible();
            FullScreen(window, EventArgs.Empty);
            return true;
        }  // 隨機下一張
        // 縮圖---------------------------------------------------------------------------------------------------------
        private readonly ImageList _thumbnails = new ImageList { ImageSize = new Size(128, 72), ColorDepth = ColorDepth.Depth32Bit };
        private readonly Timer _thumbnailTimer = new Timer { Interval = 40 };
        private static readonly Size ThumbnailSize = new Size(128, 72);
        private bool _renderingThumbnail;  // 產生縮圖中（蒙太奇縮小邊框與標籤）
        private string _lastThumbnailError;  // 上一張縮圖的錯誤（沒有錯誤為 null）
        private void InitThumbnails()
        {
            tabilPatternList_lst.LargeImageList = _thumbnails;
            _thumbnailTimer.Tick += (s, e) => RenderNextThumbnail();
            FormClosed += (s, e) => { _thumbnailTimer.Dispose(); _thumbnails.Dispose(); };
        }  // 縮圖初始化
        private void SetThumbnailView(bool on)
        {
            var collapsed = CollapsedGroupNames();
            tabilPatternList_lst.View = on ? View.Tile : View.Details;
            if (on) tabilPatternList_lst.TileSize = new Size(Math.Max(300, tabilPatternList_lst.ClientSize.Width / 2 - 8), ThumbnailSize.Height + 8);
            ApplyGroupStates(collapsed);
            _thumbnailTimer.Enabled = on;
            ssrStatus_lbl.Text = on ? "縮圖模式：只產生畫面上看得到的項目，捲動時陸續補上" : "清單模式";
        }  // 切換縮圖
        private string ThumbnailKey(ListViewItem item) => $"{mnsW_txt.Text}x{mnsH_txt.Text}|{item.Tag}";  // 內容或解析度改變就重新產生
        private void RenderNextThumbnail()
        {
            // 每次只產生一張畫面上看得到、還沒有縮圖的項目，避免卡住介面；全螢幕顯示中不產生（會用到同一份圖片）
            if (!tabilThumbnail_chk.Checked || tabControl.SelectedTab != tabImgList || !Visible || WindowState == FormWindowState.Minimized) return;
            if (Application.OpenForms.OfType<pictureWindow>().Any(w => w.Visible)) return;
            if (!int.TryParse(mnsW_txt.Text, out int width) || !int.TryParse(mnsH_txt.Text, out int height)) return;
            Rectangle client = tabilPatternList_lst.ClientRectangle;
            foreach (ListViewItem item in tabilPatternList_lst.Items)
            {
                string key = ThumbnailKey(item);
                if (item.ImageKey != key) item.ImageKey = key;
                if (_thumbnails.Images.ContainsKey(key) || !item.Bounds.IntersectsWith(client)) continue;
                if (_thumbnails.Images.Count > 500) _thumbnails.Images.Clear();  // 換解析度多次後清掉舊的
                using (Bitmap thumbnail = RenderThumbnail(item.Tag.ToString(), ThumbnailSize, width, height))
                {
                    _thumbnails.Images.Add(key, thumbnail);
                }
                tabilPatternList_lst.Invalidate(item.Bounds);
                return;
            }
        }  // 產生下一張縮圖
        private Bitmap RenderThumbnail(string line, Size size, int width, int height)
        {
            // 產生一行 config 的縮圖，ImgEditor 設定與預覽在結束後還原
            //   Mask / UserMask：畫成設定時的格子示意圖
            //   Window 用到 mask 名稱或蒙太奇：以縮圖一半的解析度產生再放大，1px 圖樣看得出結構
            //   其他：依目前解析度產生再縮小
            string[] fields = StripDwell(line, out _).Split(',');
            string type = fields.Length > 1 ? fields[1].Trim().ToLower() : "";
            var thumbnail = new Bitmap(size.Width, size.Height);
            Action restoreEditor = SnapshotEditor();
            int function = tabimgeFuncList.SelectedIndex;
            Bitmap savedPicture = mypicture.Getpicture() is Bitmap current ? new Bitmap(current) : null;
            string savedTag = mypicture.tag, savedW = mnsW_txt.Text, savedH = mnsH_txt.Text, savedSize = showimgSize_lbl.Text, savedNote = showimgConfig_lbl.Text;
            bool savedNoteVisible = showimgConfig_lbl.Visible;
            Image savedPreview = showimgPicture_pic.Image;
            _renderingThumbnail = true;
            _lastThumbnailError = null;
            try
            {
                using (Graphics g = Graphics.FromImage(thumbnail))
                {
                    g.Clear(Color.FromArgb(40, 40, 40));
                    var errors = new List<string>();
                    if (type == "mask" || type == "usermask")
                    {
                        if (ApplyConfigLine(line, errors, parseOnly: true)) DrawMaskSchematic(g, size);
                        else { DrawThumbnailError(g, size); _lastThumbnailError = errors.FirstOrDefault() ?? "無法產生圖片"; }
                    }
                    else
                    {
                        bool small = type == "window" && fields.Length > 2 && (line.Contains("|") || fields[2].Trim().ToLower().StartsWith("custom"));
                        int renderW = small ? size.Width / 2 : width, renderH = small ? size.Height / 2 : height;
                        mnsW_txt.Text = renderW.ToString();
                        mnsH_txt.Text = renderH.ToString();
                        Bitmap picture = ApplyConfigLine(line, errors) ? showimgPicture_pic.Image as Bitmap ?? mypicture.Getpicture() : null;
                        if (picture == null) { DrawThumbnailError(g, size); _lastThumbnailError = errors.FirstOrDefault() ?? "無法產生圖片"; }
                        else
                        {
                            g.InterpolationMode = small ? InterpolationMode.NearestNeighbor : InterpolationMode.HighQualityBicubic;
                            g.PixelOffsetMode = PixelOffsetMode.Half;
                            // 等比例置中
                            float scale = Math.Min(size.Width / (float)picture.Width, size.Height / (float)picture.Height);
                            float w = picture.Width * scale, h = picture.Height * scale;
                            g.DrawImage(picture, (size.Width - w) / 2, (size.Height - h) / 2, w, h);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (Graphics g = Graphics.FromImage(thumbnail)) DrawThumbnailError(g, size);
                _lastThumbnailError = ex.Message;
            }
            finally
            {
                _renderingThumbnail = false;
                mnsW_txt.Text = savedW;
                mnsH_txt.Text = savedH;
                restoreEditor();
                if (function >= 0 && tabimgeFuncList.SelectedIndex != function) tabimgeFuncList.SelectedIndex = function;
                if (savedPicture != null)
                {
                    mypicture.Setpicture(savedPicture, savedTag);
                    savedPicture.Dispose();
                    showimgPicture_pic.Image = savedPreview == null ? null : mypicture.Getpicture();
                }
                else
                {
                    showimgPicture_pic.Image = null;  // 原本沒有圖片：不留下縮圖的那張
                }
                showimgSize_lbl.Text = savedSize;
                showimgConfig_lbl.Text = savedNote;
                showimgConfig_lbl.Visible = savedNoteVisible;
            }
            return thumbnail;
        }  // 產生縮圖
        private void DrawMaskSchematic(Graphics g, Size size)
        {
            // 依 Mask 頁的格子畫示意圖（與設定時看到的一樣：每格一個色塊、灰色格線）
            var blocks = tabiemPixelPanel_pnl.Controls.OfType<Panel>().Select(p => (cell: BlockCell(p), color: p.BackColor.A == 0 ? Color.Black : p.BackColor)).ToList();
            if (blocks.Count == 0) return;
            int columns = blocks.Max(b => b.cell.x) + 1, rows = blocks.Max(b => b.cell.y) + 1;
            int cell = Math.Max(1, Math.Min((size.Width - 4) / columns, (size.Height - 4) / rows));
            int left = (size.Width - cell * columns) / 2, top = (size.Height - cell * rows) / 2;
            using (var pen = new Pen(Color.FromArgb(110, 110, 110)))
            {
                foreach (var (c, color) in blocks)
                {
                    var rect = new Rectangle(left + c.x * cell, top + c.y * cell, cell, cell);
                    using (Brush brush = new SolidBrush(color)) g.FillRectangle(brush, rect);
                    if (cell >= 4) g.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
                }
            }
        }  // Mask 格子示意圖
        private static void DrawThumbnailError(Graphics g, Size size)
        {
            g.Clear(Color.FromArgb(90, 30, 30));
            using (Font font = new Font("Arial", 10f, GraphicsUnit.Pixel))
            {
                g.DrawString("設定有誤", font, Brushes.White, 4, 4);
            }
        }  // 無法產生縮圖
        // 說明文件匯出--------------------------------------------------------------------------------------------------
        private static readonly Dictionary<string, string> ConfigTypeNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Solid"] = "純色（可加框線）", ["Gradient"] = "漸層 / ColorBar", ["Chess"] = "棋盤", ["Window"] = "窗口", ["Mask"] = "Mask（Flicker）",
            ["Text"] = "文字", ["Dynamic"] = "動態", ["Test"] = "測試圖", ["Image"] = "圖檔", ["UserMask"] = "自訂 mask",
        };
        private void ExportConfigHtml()
        {
            if (!AllConfigItems().Any() || !int.TryParse(mnsW_txt.Text, out int width) || !int.TryParse(mnsH_txt.Text, out int height)) return;
            string source = tabilPatternList_lst.Tag?.ToString() ?? "";
            string path;
            using (var dialog = new SaveFileDialog { Filter = "HTML|*.html", FileName = $"{Path.GetFileNameWithoutExtension(source == "" || source == BuiltinConfigName ? "config" : source)}_說明.html" })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                path = dialog.FileName;
            }
            ExportConfigHtmlTo(path, width, height, source);
        }  // 選擇檔案後匯出 HTML
        private void ExportConfigHtmlTo(string path, int width, int height, string source)
        {
            var entries = AllConfigEntries().ToList();
            var html = new StringBuilder();
            string E(string s) => WebUtility.HtmlEncode(s ?? "");
            html.AppendLine("<!DOCTYPE html><html lang=\"zh-Hant\"><head><meta charset=\"utf-8\"><title>" + E(Path.GetFileName(source)) + " 圖樣說明</title><style>");
            html.AppendLine("body{font-family:'Microsoft JhengHei',sans-serif;margin:24px;color:#222}table{border-collapse:collapse;width:100%;margin-bottom:24px}th,td{border:1px solid #ccc;padding:6px 8px;text-align:left;vertical-align:middle;font-size:14px}th{background:#f0f0f0}td.n{text-align:right;width:40px}img{display:block;image-rendering:pixelated;border:1px solid #888}tr.bad td{background:#ffe5e5;color:#900}h2{border-bottom:2px solid #444;padding-bottom:4px}");
            html.AppendLine("</style></head><body>");
            html.AppendLine($"<h1>{E(source == "" ? "ImgList" : Path.GetFileName(source))} 圖樣說明</h1>");
            html.AppendLine($"<p>解析度 {width} x {height}　共 {entries.Count} 項　匯出時間 {DateTime.Now:yyyy-MM-dd HH:mm}　PatternMagic {Application.ProductVersion}</p>");
            ssrProgressbar_prg.Maximum = entries.Count;
            Cursor = Cursors.WaitCursor;
            string currentGroup = null;
            try
            {
                for (int i = 0; i < entries.Count; i++)
                {
                    var (item, group) = entries[i];
                    string groupName = GroupName(group);
                    if (groupName != currentGroup)
                    {
                        if (currentGroup != null) html.AppendLine("</table>");
                        if (groupName != "") html.AppendLine($"<h2>{E(groupName)}</h2>");
                        html.AppendLine("<table><tr><th>編號</th><th>縮圖</th><th>名稱</th><th>類型</th><th>參數</th><th>選項</th><th>說明</th></tr>");
                        currentGroup = groupName;
                    }
                    string line = item.Tag.ToString();
                    string[] fields = StripDwell(line, out int? dwell).Split(',');
                    string type = fields.Length > 1 ? fields[1].Trim() : "";
                    string image;
                    using (Bitmap thumbnail = RenderThumbnail(line, new Size(192, 108), width, height))
                    using (var stream = new MemoryStream())
                    {
                        thumbnail.Save(stream, ImageFormat.Png);
                        image = Convert.ToBase64String(stream.ToArray());
                    }
                    // 錯誤取自產生縮圖時的檢查（載入時不再逐行檢查）
                    bool ok = _lastThumbnailError == null;
                    MarkConfigItem(item, _lastThumbnailError);
                    string note = (ConfigTypeNames.TryGetValue(type, out string typeName) ? typeName : type) + (dwell != null ? $"，幻燈片停留 {dwell / 1000.0:0.#} 秒" : "") + (ok ? "" : $"<br>設定有誤：{E(_lastThumbnailError)}");
                    html.AppendLine($"<tr{(ok ? "" : " class=\"bad\"")}><td class=\"n\">{i + 1}</td><td><img src=\"data:image/png;base64,{image}\" width=\"192\" height=\"108\" alt=\"\"></td>" +
                        $"<td>{E(fields[0].Trim())}</td><td>{E(type)}</td><td>{E(fields.Length > 2 ? fields[2].Trim() : "")}</td><td>{E(string.Join(",", fields.Skip(3)).Trim())}{(dwell != null ? E($" @{dwell / 1000.0:0.#}s") : "")}</td><td>{(ok ? E(note) : note)}</td></tr>");
                    ssrProgressbar_prg.Value = i + 1;
                    ssrStatus_lbl.Text = $"匯出說明文件 {i + 1}/{entries.Count}";
                    Application.DoEvents();
                }
                if (currentGroup != null) html.AppendLine("</table>");
                html.AppendLine("</body></html>");
                File.WriteAllText(path, html.ToString(), new UTF8Encoding(true));
                ssrStatus_lbl.Text = $"已匯出說明文件：{path}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"匯出失敗：{ex.Message}", "匯出說明文件", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
                ssrProgressbar_prg.Value = 0;
            }
        }  // 匯出 HTML 說明文件
    }
}
