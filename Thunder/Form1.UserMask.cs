using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Thunder
{
    public partial class mainWindow
    {
        // 子像素排列 RGB / BGR-------------------------------------------------------------------------------------------
        private static bool _subPixelBgr;  // BGR 面板：子像素由左到右為 B G R
        private readonly List<CheckBox> _bgrChecks = new List<CheckBox>();
        private void InitSubPixelOrder()
        {
            // Mask 頁與 Window 自製圖各放一個勾選框，兩邊同步
            foreach (var (box, x) in new[] { ((Control)tabimgeMask, 200), (tabiwCustomMask_grp, 190) })
            {
                var check = new CheckBox { Text = "BGR", AutoSize = true, Location = new Point(x, 15) };
                check.CheckedChanged += (s, e) => SetSubPixelBgr(check.Checked);
                toolTip.SetToolTip(check, "BGR 排列的面板：子像素由左到右為 B G R\r\n影響 Sub Pixel Base 的格子顏色與 dotDell；sub1、sub2（R+B / G 交錯）左右對稱，不受影響");
                box.Controls.Add(check);
                _bgrChecks.Add(check);
            }
        }  // 建立 BGR 勾選框
        private void SetSubPixelBgr(bool bgr)
        {
            if (_subPixelBgr == bgr) return;
            _subPixelBgr = bgr;
            _bgrChecks.ForEach(c => c.Checked = bgr);  // 另一個勾選框跟著改（值相同時直接返回）
            // 已畫好的子像素格：亮的格子換成新排列下該位置的顏色，暗的保持黑色
            if (tabiemSubPixel_rdo.Checked) RecolorSubPixelPanels(tabiemPixelPanel_pnl, tabiemPixelGray_vsc);
            if (tabiwCMaskSubPixel_rdo.Checked) RecolorSubPixelPanels(tabiwCMaskPixelPanel_pnl, tabiwCMaskPixelGray_vsc);
            ssrStatus_lbl.Text = $"子像素排列：{(bgr ? "BGR" : "RGB")}";
        }  // 切換 RGB / BGR
        private static void RecolorSubPixelPanels(Panel panel, VScrollBar grayScroll)
        {
            foreach (Panel block in panel.Controls.OfType<Panel>())
            {
                if (block.BackColor.ToArgb() == Color.Black.ToArgb()) continue;
                block.BackColor = SubPixelColumnColor(BlockCell(block).x, 255 - grayScroll.Value);
            }
        }  // 子像素格重新上色
        private static Color SubPixelColumnColor(int x, int level)
        {
            // 第 x 個子像素格的顏色：RGB 面板依序 R G B，BGR 面板依序 B G R
            int channel = _subPixelBgr ? 2 - x % 3 : x % 3;
            return channel == 0 ? Color.FromArgb(level, 0, 0) : channel == 1 ? Color.FromArgb(0, level, 0) : Color.FromArgb(0, 0, level);
        }  // 子像素格顏色
        private static Color CombineSubPixels(Panel first, Panel second, Panel third)
        {
            // 每格只有自己的色版（或黑色），三格取聯集就是這個像素的顏色，不必管排列順序
            Color a = first.BackColor, b = second.BackColor, c = third.BackColor;
            return Color.FromArgb(a.R | b.R | c.R, a.G | b.G | c.G, a.B | b.B | c.B);
        }  // 三個子像素合成一個像素
        // 自訂 mask 名稱------------------------------------------------------------------------------------------------
        // config 寫法：名稱,UserMask,p-寬*高,格子（與 Mask 的 p 寫法相同），之後 Window Custom、蒙太奇、Mask 範本可直接用名稱
        private const string UserMaskType = "UserMask";
        private const int MaxUserMaskSize = 64;
        private static readonly Dictionary<string, (int width, int height, Color[] cells)> _userMasks = new Dictionary<string, (int, int, Color[])>();
        private static readonly Regex UserMaskNameRegex = new Regex(@"^[a-z][a-z0-9_]*$");
        private ComboBox tabiemUserMask_cmb;
        private void InitUserMaskControls()
        {
            // Mask 頁範本按鈕下方：自訂 mask 下拉選單（選了就套用）與「存成自訂」按鈕
            var title = new Label { Text = "自訂", AutoSize = true, Location = new Point(330, 318) };
            tabiemUserMask_cmb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Bounds = new Rectangle(376, 314, 74, 24), ImeMode = ImeMode.Disable };
            tabiemUserMask_cmb.DropDown += (s, e) => RefreshUserMaskCombo();  // 展開時依 ImgList 重新整理
            tabiemUserMask_cmb.SelectionChangeCommitted += (s, e) => { if (tabiemUserMask_cmb.SelectedItem is string name) ApplyMaskTemplate(name); };
            var save = new Button { Text = "存成自訂…", Bounds = new Rectangle(330, 344, 120, 24) };
            save.Click += (s, e) => PromptSaveUserMask();
            toolTip.SetToolTip(tabiemUserMask_cmb, "ImgList 中的自訂 mask（UserMask），選擇後套用到格子");
            toolTip.SetToolTip(save, "目前的格子存成自訂 mask（加到 ImgList，存 config 檔後保留）\r\n之後可在 Window Custom、蒙太奇、這裡的下拉選單用名稱叫出");
            tabimgeMask.Controls.AddRange(new Control[] { title, tabiemUserMask_cmb, save });
        }  // 建立自訂 mask 控制項
        private void RefreshUserMaskCombo()
        {
            RefreshUserMasks();
            string selected = tabiemUserMask_cmb.SelectedItem as string;
            tabiemUserMask_cmb.Items.Clear();
            tabiemUserMask_cmb.Items.AddRange(_userMasks.Keys.ToArray<object>());
            if (selected != null && _userMasks.ContainsKey(selected)) tabiemUserMask_cmb.SelectedItem = selected;
        }  // 更新自訂 mask 下拉選單
        private static bool IsUserMaskLine(string[] fields) => fields.Length >= 3 && fields[1].Trim().Equals(UserMaskType, StringComparison.OrdinalIgnoreCase);
        private void RefreshUserMasks()
        {
            // 依 ImgList 目前內容重建自訂 mask 表；名稱重複時以第一個為準（重複的那行檢查時會標紅）
            _userMasks.Clear();
            foreach (ListViewItem item in AllConfigItems())  // 搜尋中被篩掉的也算
            {
                string[] fields = StripDwell(item.Tag.ToString(), out _).Split(',');
                if (!IsUserMaskLine(fields)) continue;
                string name = fields[0].Trim().ToLower();
                if (_userMasks.ContainsKey(name) || !IsValidUserMaskName(name, out _)) continue;
                try
                {
                    Color[] cells = ParseUserMask(fields[2], string.Join(",", fields.Skip(3)), out int width, out int height);
                    _userMasks[name] = (width, height, cells);
                }
                catch (FormatException)
                {
                    // 格式錯誤的那行在檢查時會標紅
                }
            }
        }  // 重建自訂 mask 表
        private static bool IsValidUserMaskName(string name, out string error)
        {
            error = null;
            if (!UserMaskNameRegex.IsMatch(name)) error = "名稱需以英文字母開頭，後面只能接英文、數字或 _（例：my1）";
            else if (name == "custom" || name == "dotdell" || name.StartsWith("excel") || LegacyCustomMasks.ContainsKey(name) || IsBuiltinMaskName(name)) error = "與內建名稱相同";
            else if (Color.FromName(name).IsKnownColor) error = "與顏色名稱相同";
            return error == null;
        }  // 檢查自訂 mask 名稱
        private static bool IsBuiltinMaskName(string name)
        {
            try
            {
                return CustomMaskPatterns.ContainsKey(name) || TryDiagonalMask(name, out _) || TryHvMask(name, out _);
            }
            catch (FormatException)
            {
                return true;  // 例如 9h1v：雖然超出範圍，仍屬於內建寫法
            }
        }  // 是否為內建 mask 名稱
        private Color[] ParseUserMask(string param, string option, out int width, out int height)
        {
            // 參數 p-寬*高（只寫一個數字時為正方形），選項為精簡寫法（10.01）或顏色寫法（red black/black red）
            Match match = Regex.Match(param.Trim().ToLower(), @"^p-(\d+)(?:\*(\d+))?$");
            if (!match.Success) throw new FormatException("參數格式應為 p-寬*高，例：p-2*2");
            width = int.Parse(match.Groups[1].Value);
            height = match.Groups[2].Value == "" ? width : int.Parse(match.Groups[2].Value);
            if (width < 1 || height < 1 || width > MaxUserMaskSize || height > MaxUserMaskSize)
            {
                throw new FormatException($"寬、高需為 1~{MaxUserMaskSize}");
            }
            option = option.Trim().ToLower();
            if (IsColorMaskOption(option)) return ParseMaskColorRows(option, width, height);
            string compact = option.Replace(".", "");
            if (compact.Length != width * height || compact.Any(c => c < '0' || c > '2'))
            {
                throw new FormatException($"精簡寫法需要 {width * height} 個 0 / 1 / 2（列之間用 .），例：10.01");
            }
            return compact.Select(c => c == '0' ? Color.Black : c == '1' ? Color.White : Color.Gray).ToArray();
        }  // 解析自訂 mask
        private void CheckUserMaskLine(string name, string param, string option)
        {
            // ApplyConfigLine 用：名稱、重複與格式都檢查，有錯丟出 FormatException
            name = name.Trim().ToLower();
            if (!IsValidUserMaskName(name, out string error)) throw new FormatException($"名稱「{name}」{error}");
            int count = UserMaskItems(name).Count();
            if (count > 1) throw new FormatException($"自訂 mask 名稱「{name}」重複了 {count} 次");
            ParseUserMask(param, option, out _, out _);
        }  // 檢查自訂 mask 那一行
        private IEnumerable<ListViewItem> UserMaskItems(string name) => AllConfigItems().Where(item =>
        {
            string[] fields = StripDwell(item.Tag.ToString(), out _).Split(',');
            return IsUserMaskLine(fields) && fields[0].Trim().ToLower() == name;
        });  // ImgList 中名稱為 name 的 UserMask 行
        private void PromptSaveUserMask()
        {
            RefreshUserMasks();
            string suggestion = Enumerable.Range(1, 999).Select(i => $"my{i}").First(n => !_userMasks.ContainsKey(n));
            string name = InputText("存成自訂 mask", "名稱（英文字母開頭，可用英文、數字、_）：", suggestion);
            if (name != null) SaveUserMask(name, true);
        }  // 詢問名稱後存成自訂 mask
        private bool SaveUserMask(string name, bool confirmOverwrite)
        {
            // 目前格子轉成一行 UserMask 加到 ImgList；同名時取代原本那行
            name = name.Trim().ToLower();
            if (!IsValidUserMaskName(name, out string error))
            {
                MessageBox.Show($"「{name}」{error}", "存成自訂 mask", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            Color[] cells = CurrentMaskCells(out int width, out int height);
            if (width > MaxUserMaskSize || height > MaxUserMaskSize)
            {
                MessageBox.Show($"自訂 mask 的寬、高最多 {MaxUserMaskSize}", "存成自訂 mask", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            ListViewItem existing = UserMaskItems(name).FirstOrDefault();
            if (existing != null && confirmOverwrite &&
                MessageBox.Show($"自訂 mask「{name}」已存在（ImgList 第 {existing.Index + 1} 列），要覆蓋嗎？", "存成自訂 mask",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return false;
            }
            using (SuspendFilter())  // 搜尋中被篩掉的同名項目也要找得到
            {
                PushConfigUndo();  // 記錄復原點
                ListViewItem item = CreateConfigItem($"{name},{UserMaskType},p-{width}*{height},{MaskCellsToOption(cells, width, height)}");
                if (existing != null)
                {
                    ListViewGroup group = existing.Group;
                    tabilPatternList_lst.Items[existing.Index] = item;
                    if (tabilPatternList_lst.ShowGroups) item.Group = group;  // 維持原本的群組
                }
                else
                {
                    tabilPatternList_lst.Items.Add(item);
                    AdoptNeighborGroup(item);
                }
                RenumberPatternList();
                item.EnsureVisible();
                RefreshUserMasks();
                ssrStatus_lbl.Text = $"已存成自訂 mask「{name}」（ImgList 第 {item.Index + 1} 列），要保留請存 config 檔";
            }
            return true;
        }  // 存成自訂 mask
        private Color[] CurrentMaskCells(out int width, out int height)
        {
            // Mask 頁目前的格子轉成每個像素的顏色；子像素模式把三格合成一個像素
            width = (int)tabiemWNum_nud.Value;
            height = (int)tabiemHNum_nud.Value;
            bool subPixel = tabiemSubPixel_rdo.Checked;
            Color[] cells = Enumerable.Repeat(Color.Black, width * height).ToArray();
            foreach (Panel block in tabiemPixelPanel_pnl.Controls.OfType<Panel>())
            {
                var (x, y) = BlockCell(block);
                int px = subPixel ? x / 3 : x;
                if (px >= width || y >= height) continue;
                Color color = block.BackColor.A == 0 ? Color.Black : block.BackColor;  // 未設定（透明）視為黑
                Color old = cells[y * width + px];
                cells[y * width + px] = subPixel ? Color.FromArgb(old.R | color.R, old.G | color.G, old.B | color.B) : Color.FromArgb(color.R, color.G, color.B);
            }
            return cells;
        }  // 目前 Mask 格子的顏色
        private string InputText(string title, string prompt, string text)
        {
            // 簡單的輸入框，取消回傳 null
            using (var form = new Form
            {
                Text = title, FormBorderStyle = FormBorderStyle.FixedDialog, StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false, MaximizeBox = false, ShowInTaskbar = false, ClientSize = new Size(320, 100), Font = Font,
            })
            {
                var label = new Label { Text = prompt, AutoSize = true, Location = new Point(10, 12) };
                var box = new TextBox { Text = text, Bounds = new Rectangle(10, 34, 300, 23), ImeMode = ImeMode.Disable };
                var ok = new Button { Text = "確定", DialogResult = DialogResult.OK, Bounds = new Rectangle(154, 66, 75, 25) };
                var cancel = new Button { Text = "取消", DialogResult = DialogResult.Cancel, Bounds = new Rectangle(235, 66, 75, 25) };
                form.Controls.AddRange(new Control[] { label, box, ok, cancel });
                form.AcceptButton = ok;
                form.CancelButton = cancel;
                Localize(form);
                return form.ShowDialog(this) == DialogResult.OK ? box.Text : null;
            }
        }  // 輸入文字
        // 幻燈片停留時間-------------------------------------------------------------------------------------------------
        // config 結尾加 @秒數s（例：紅,Solid,Red,@5s），幻燈片播到這張時停留指定秒數，其餘照全域間隔
        private static readonly Regex DwellRegex = new Regex(@"\s*@\s*(\d+(?:\.\d+)?)\s*s\s*$", RegexOptions.IgnoreCase);
        private const int MinDwell = 100, MaxDwell = 3600 * 1000;  // 毫秒
        private static string StripDwell(string line, out int? milliseconds)
        {
            // 回傳去掉停留時間後的 config；沒有寫時 milliseconds 為 null
            milliseconds = null;
            Match match = DwellRegex.Match(line);
            if (!match.Success) return line;
            milliseconds = (int)Math.Min(int.MaxValue, Math.Round(double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture) * 1000));
            return line.Substring(0, match.Index);
        }  // 拆出停留時間
        public int? SelectedSlideDwell()
        {
            // 全螢幕幻燈片用：ImgList 目前選取那行的停留時間（毫秒），沒有寫或不在 ImgList 時為 null
            if (tabControl.SelectedTab != tabImgList || tabilPatternList_lst.SelectedItems.Count == 0) return null;
            StripDwell(tabilPatternList_lst.SelectedItems[0].Tag.ToString(), out int? milliseconds);
            return milliseconds >= MinDwell && milliseconds <= MaxDwell ? milliseconds : null;
        }  // 目前這張的停留時間
    }
}
