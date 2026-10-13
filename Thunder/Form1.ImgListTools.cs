using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Thunder
{
    public partial class mainWindow
    {
        // ImgList 圖檔類型--------------------------------------------------------------------------------------------
        private static readonly string[] ConfigImageModes = { "stretch", "fit", "fill", "1:1" };  // 對應 ResizeRectangle 的 0~3
        private string ResolveConfigImagePath(string path)
        {
            // 相對路徑以目前 config 檔所在資料夾為準（內建設定則為程式資料夾）
            if (Path.IsPathRooted(path)) return path;
            string source = tabilPatternList_lst.Tag?.ToString();
            string folder = !string.IsNullOrEmpty(source) && File.Exists(source) ? Path.GetDirectoryName(source) : Application.StartupPath;
            return Path.GetFullPath(Path.Combine(folder, path));
        }  // 圖檔路徑
        private bool LoadConfigImage(string path, string option, int width, int height, bool parseOnly)
        {
            // 參數：圖檔路徑；選項：fit（預設）、stretch、fill、1:1；parseOnly 只檢查，回傳 false
            if (path == "") throw new FormatException("沒有圖檔路徑（參數為圖檔路徑）");
            string modeName = option == "" ? "fit" : option;
            int mode = Array.IndexOf(ConfigImageModes, modeName);
            if (mode < 0) throw new FormatException($"縮放方式「{option}」只能是 fit、stretch、fill、1:1");
            string fullPath = ResolveConfigImagePath(path);
            if (!File.Exists(fullPath)) throw new FileNotFoundException($"找不到圖檔：{fullPath}");
            if (parseOnly) return false;
            timerDynamic.Stop();
            using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
            using (var source = Image.FromStream(stream))
            using (var output = new Bitmap(width, height))
            {
                using (Graphics g = Graphics.FromImage(output))
                {
                    g.Clear(Color.Black);
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;  // 與全螢幕相同，像素不糊
                    g.PixelOffsetMode = PixelOffsetMode.Half;
                    g.DrawImage(source, ResizeRectangle(source.Size, width, height, mode));
                }
                mypicture.Setpicture(output, "configimage");  // 不用 "image"，避免 Frame 把它當成匯入的背景
            }
            return true;
        }  // 讀取 config 的圖檔
        // ImgList 拖拉排序與復原---------------------------------------------------------------------------------------
        private const int MaxConfigUndo = 20;
        private readonly List<(List<string> lines, int selected)> _configUndo = new List<(List<string>, int)>();
        private ToolStripMenuItem cmsConfigUndo;
        private void InitImgListTools()
        {
            tabilPatternList_lst.ItemDrag += (s, e) =>
            {
                if (BlockWhileFiltering("拖拉排序")) return;
                if (e.Button == MouseButtons.Left && e.Item is ListViewItem item) tabilPatternList_lst.DoDragDrop(item, DragDropEffects.Move);
                ShowDropLine(null, false);  // 放下或取消後收起插入線
            };
            tabilPatternList_lst.DragOver += (s, e) =>
            {
                if (!e.Data.GetDataPresent(typeof(ListViewItem))) return;
                e.Effect = DragDropEffects.Move;
                // 插入線：滑鼠在項目上半部插在前面、下半部插在後面（縮圖模式看左右）
                Point point = tabilPatternList_lst.PointToClient(new Point(e.X, e.Y));
                (_dropTarget, _dropAfter) = DropPosition(point);
                ShowDropLine(_dropTarget, _dropAfter);
            };
            tabilPatternList_lst.DragLeave += (s, e) => { ShowDropLine(null, false); _dropTarget = null; };
            cmsConfigUndo = new ToolStripMenuItem("復原", null, (s, e) => UndoConfigEdit()) { ShortcutKeyDisplayString = "Ctrl+Z" };
            tabilPatternList_cms.Items.Insert(0, cmsConfigUndo);
            tabilPatternList_cms.Items.Insert(1, new ToolStripSeparator());
            tabilPatternList_cms.Opening += (s, e) =>
            {
                cmsConfigUndo.Enabled = _configUndo.Count > 0;
                cmsConfigUndo.Text = _configUndo.Count > 0 ? $"復原（可復原 {_configUndo.Count} 步）" : "復原";
            };
        }  // 建立拖拉排序與復原
        private ListViewItem _dropTarget;  // 拖拉時滑鼠下方的項目
        private bool _dropAfter;  // 放在該項目之後
        private Panel _dropLine;
        private void ShowDropLine(ListViewItem target, bool after)
        {
            // 清單（詳細資料）檢視 Windows 不畫 InsertionMark，自己用一條細線標示放下的位置
            if (target == null || target.ListView != tabilPatternList_lst)
            {
                if (_dropLine != null) _dropLine.Visible = false;
                return;
            }
            if (_dropLine == null)
            {
                _dropLine = new Panel { BackColor = SystemColors.Highlight, Visible = false, Enabled = false };
                tabilPatternList_lst.Controls.Add(_dropLine);
            }
            Rectangle bounds = ItemBounds(target);
            Rectangle line = tabilPatternList_lst.View == View.Tile
                ? new Rectangle((after ? bounds.Right : bounds.Left) - 1, bounds.Top, 3, bounds.Height)
                : new Rectangle(0, (after ? bounds.Bottom : bounds.Top) - 1, tabilPatternList_lst.ClientSize.Width, 3);
            if (_dropLine.Bounds != line) _dropLine.Bounds = line;
            if (!_dropLine.Visible) { _dropLine.Visible = true; _dropLine.BringToFront(); }
        }  // 顯示 / 收起插入線
        private (ListViewItem item, bool after) DropPosition(Point point)
        {
            // 自己找滑鼠下方（或最近）的項目：分組顯示時 InsertionMark.NearestIndex 會偏一列（群組標題也佔位置）
            ListViewItem nearest = null;
            Rectangle nearestBounds = Rectangle.Empty;
            int best = int.MaxValue;
            bool tiles = tabilPatternList_lst.View == View.Tile;
            foreach (ListViewItem item in tabilPatternList_lst.Items)
            {
                Rectangle bounds = ItemBounds(item);
                if (bounds.Height == 0) continue;  // 收合群組內的項目
                if (!tiles) bounds = new Rectangle(0, bounds.Top, Math.Max(bounds.Right, tabilPatternList_lst.ClientSize.Width), bounds.Height);  // 整列都算
                if (bounds.Contains(point)) { nearest = item; nearestBounds = bounds; break; }
                int dx = Math.Max(0, Math.Max(bounds.Left - point.X, point.X - bounds.Right));
                int dy = Math.Max(0, Math.Max(bounds.Top - point.Y, point.Y - bounds.Bottom));
                int distance = dx * dx + dy * dy;
                if (distance < best) { best = distance; nearest = item; nearestBounds = bounds; }
            }
            if (nearest == null) return (null, false);
            bool after = tiles ? point.X > nearestBounds.Left + nearestBounds.Width / 2 : point.Y > nearestBounds.Top + nearestBounds.Height / 2;
            return (nearest, after);
        }  // 拖拉放下的位置
        private bool DropConfigItem(DragEventArgs e)
        {
            // 清單內拖拉移動項目；回傳 false 表示不是清單項目（交給原本的檔案拖放處理）
            if (!e.Data.GetDataPresent(typeof(ListViewItem))) return false;
            var item = (ListViewItem)e.Data.GetData(typeof(ListViewItem));
            ListViewItem targetItem = _dropTarget;
            bool after = _dropAfter;
            _dropTarget = null;
            ShowDropLine(null, false);
            if (targetItem == null || targetItem.ListView != tabilPatternList_lst || item.ListView != tabilPatternList_lst) return true;
            ListViewGroup targetGroup = targetItem.Group;
            int target = targetItem.Index;
            int from = item.Index;
            int to = target + (after ? 1 : 0);
            if (to > from) to--;  // 先移除原本的位置，後面的索引往前一格
            if (to == from && (!tabilPatternList_lst.ShowGroups || item.Group == targetGroup)) return true;
            PushConfigUndo();
            _movingConfigLine = true;  // 重新選取時不重新產生圖片
            try
            {
                tabilPatternList_lst.Items.RemoveAt(from);
                tabilPatternList_lst.Items.Insert(to, item);
                if (tabilPatternList_lst.ShowGroups && targetGroup != null) item.Group = targetGroup;  // 分組時加入放下位置那一項的群組
                RenumberPatternList();
                item.Selected = true;
                item.Focused = true;
                item.EnsureVisible();
            }
            finally
            {
                _movingConfigLine = false;
            }
            ssrStatus_lbl.Text = $"已移動「{item.SubItems[1].Text}」到第 {to + 1} 列（Ctrl+Z 復原）";
            return true;
        }  // 拖拉移動項目
        private void PushConfigUndo()
        {
            // 變更前記下整份清單（含群組標題、搜尋中被篩掉的項目）與選取項目在完整清單中的位置
            var lines = ConfigLinesWithGroups();
            ListViewItem current = tabilPatternList_lst.SelectedItems.Count > 0 ? tabilPatternList_lst.SelectedItems[0] : null;
            int selected = current == null ? -1 : AllConfigItems().ToList().IndexOf(current);
            _configUndo.Add((lines, selected));
            if (_configUndo.Count > MaxConfigUndo) _configUndo.RemoveAt(0);
        }  // 記錄復原點
        private void UndoConfigEdit()
        {
            if (_configUndo.Count == 0)
            {
                ssrStatus_lbl.Text = "沒有可以復原的 ImgList 變更";
                return;
            }
            var (lines, selected) = _configUndo[_configUndo.Count - 1];
            _configUndo.RemoveAt(_configUndo.Count - 1);
            _movingConfigLine = true;
            try
            {
                ClearSearchSilently();  // 復原後顯示完整清單
                FillPatternList(lines);
                if (selected >= 0 && selected < tabilPatternList_lst.Items.Count)
                {
                    tabilPatternList_lst.Items[selected].Selected = true;
                    tabilPatternList_lst.Items[selected].Focused = true;
                    tabilPatternList_lst.Items[selected].EnsureVisible();
                }
            }
            finally
            {
                _movingConfigLine = false;
            }
            if (tabilPatternList_lst.SelectedItems.Count > 0)
            {
                // 預覽並檢查選取的項目（不逐行檢查整份清單）
                ListViewItem item = tabilPatternList_lst.SelectedItems[0];
                var errors = new List<string>();
                bool ok = ApplyConfigLine(item.Tag.ToString(), errors);
                MarkConfigItem(item, ok ? null : errors.FirstOrDefault() ?? "無法產生圖片");
            }
            ssrStatus_lbl.Text = $"已復原（還可復原 {_configUndo.Count} 步）";
        }  // 復原上一步
    }
}
