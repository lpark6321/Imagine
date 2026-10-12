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
                if (e.Button == MouseButtons.Left && e.Item is ListViewItem item) tabilPatternList_lst.DoDragDrop(item, DragDropEffects.Move);
            };
            tabilPatternList_lst.DragOver += (s, e) =>
            {
                if (!e.Data.GetDataPresent(typeof(ListViewItem))) return;
                e.Effect = DragDropEffects.Move;
                // 插入線：滑鼠在項目上半部插在前面、下半部插在後面
                Point point = tabilPatternList_lst.PointToClient(new Point(e.X, e.Y));
                int index = tabilPatternList_lst.InsertionMark.NearestIndex(point);
                if (index >= 0)
                {
                    Rectangle bounds = tabilPatternList_lst.GetItemRect(index);
                    tabilPatternList_lst.InsertionMark.AppearsAfterItem = point.Y > bounds.Top + bounds.Height / 2;
                }
                tabilPatternList_lst.InsertionMark.Index = index;
            };
            tabilPatternList_lst.DragLeave += (s, e) => tabilPatternList_lst.InsertionMark.Index = -1;
            cmsConfigUndo = new ToolStripMenuItem("復原", null, (s, e) => UndoConfigEdit()) { ShortcutKeyDisplayString = "Ctrl+Z" };
            tabilPatternList_cms.Items.Insert(0, cmsConfigUndo);
            tabilPatternList_cms.Items.Insert(1, new ToolStripSeparator());
            tabilPatternList_cms.Opening += (s, e) =>
            {
                cmsConfigUndo.Enabled = _configUndo.Count > 0;
                cmsConfigUndo.Text = _configUndo.Count > 0 ? $"復原（可復原 {_configUndo.Count} 步）" : "復原";
            };
        }  // 建立拖拉排序與復原
        private bool DropConfigItem(DragEventArgs e)
        {
            // 清單內拖拉移動項目；回傳 false 表示不是清單項目（交給原本的檔案拖放處理）
            if (!e.Data.GetDataPresent(typeof(ListViewItem))) return false;
            var item = (ListViewItem)e.Data.GetData(typeof(ListViewItem));
            int target = tabilPatternList_lst.InsertionMark.Index;
            bool after = tabilPatternList_lst.InsertionMark.AppearsAfterItem;
            tabilPatternList_lst.InsertionMark.Index = -1;
            if (target < 0 || item.ListView != tabilPatternList_lst) return true;
            int from = item.Index;
            int to = target + (after ? 1 : 0);
            if (to > from) to--;  // 先移除原本的位置，後面的索引往前一格
            if (to == from) return true;
            PushConfigUndo();
            _movingConfigLine = true;  // 重新選取時不重新產生圖片
            try
            {
                tabilPatternList_lst.Items.RemoveAt(from);
                tabilPatternList_lst.Items.Insert(to, item);
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
            // 變更前記下整份清單與選取位置
            var lines = tabilPatternList_lst.Items.Cast<ListViewItem>().Select(i => i.Tag.ToString()).ToList();
            int selected = tabilPatternList_lst.SelectedIndices.Count > 0 ? tabilPatternList_lst.SelectedIndices[0] : -1;
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
                tabilPatternList_lst.BeginUpdate();
                tabilPatternList_lst.Items.Clear();
                foreach (string line in lines)
                {
                    ListViewItem item = CreateConfigItem(line);
                    if (item != null) tabilPatternList_lst.Items.Add(item);
                }
                RenumberPatternList();
                tabilPatternList_lst.EndUpdate();
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
            ValidateAllConfigLines();  // 重新標示錯誤列，並預覽選取的項目
            ssrStatus_lbl.Text = $"已復原（還可復原 {_configUndo.Count} 步）";
        }  // 復原上一步
    }
}
