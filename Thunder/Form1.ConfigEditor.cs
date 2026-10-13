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
        // ImgList 編輯區--------------------------------------------------------------------------------------------
        private const string ConfigColorHelp = "顏色寫法：名稱（White、Red、Lime、Blue、Yellow、Aqua、Magenta…）、R.G.B（58.110.165）、灰階（50%Gray）、HEX（#FF8000、#F80）";
        private static readonly Dictionary<string, (string param, string option)> ConfigHelp = new Dictionary<string, (string, string)>
        {
            ["Solid"] = (
                "參數：底色\r\n" + ConfigColorHelp + "\r\n例：White、58.110.165、50%Gray",
                "選項：框線（可省略）\r\n格式：線種-線色\r\n線種可組合：c 中心十字、o 外框、n 九點、r 左紅右藍（上下白線）\r\nr 不使用線色，只有 r 時可省略線色\r\n例：con-white、c-red、r、cr-yellow"),
            ["Gradient"] = (
                "參數：漸層顏色或 ColorBar\r\n單色：White（" + ConfigColorHelp + "）\r\n四色 ColorBar：colorbar-four-h、colorbar-four-v\r\n八色 ColorBar：colorbar-eight-h、colorbar-eight-v\r\n自訂 ColorBar：colorbar-h-數量.色1.色2…\r\n　例：colorbar-h-4.White.Red.Lime.Blue",
                "選項：漸層（省略則不漸層，只畫 ColorBar）\r\n格式：[r-]方向-階數-分割[-起始.結束]\r\nr-：反向，奇數等分反轉漸層方向（分割需 ≥ 2）\r\n方向：v（Vertical 漸層）、h（Horizontal 漸層）\r\n起始.結束：灰階 0~255，用 . 連接，省略為 0.255\r\n結尾加 -num：在每一階上標示階數\r\n例：v-256-1、h-8-1、h-256-1-255.0（由亮到暗）、r-h-256-2、v-16-1-num\r\n全階（全畫面格狀）：all-階數 直排、all-h-階數 橫排（16、32、64、128、256、1024），顏色可用單色或 colorbar，例：all-256"),
            ["Chess"] = (
                "參數：格數  寬*高\r\n例：8*8、5*5、1*2",
                "選項：可省略，多項用 - 連接\r\n顏色：格子顏色，另一色固定黑色，預設白色\r\n" + ConfigColorHelp + "\r\n翻轉：h 水平、v 垂直、hv 兩者\r\n例：Red、255.255.0-hv、v"),
            ["Window"] = (
                "參數：背景與窗口\r\n純色：背景色-窗口色，例：Gray-Black、Black-White\r\n自製圖：Custom-背景-窗口，背景與窗口都可以是 mask 名稱或顏色\r\n　dot1 點棋盤、dot2 2×2 點棋盤、sub1 子像素棋盤、sub2 2×2 子像素棋盤\r\n　row1 / row2 1 列 / 2 列橫條、col1 / col2 1 行 / 2 行直條、rdiag1~3 / ldiag1~3 右斜 / 左斜線（數字為線寬）、dotDell\r\n　自訂 mask：ImgList 中 UserMask 那行的名稱，例：Custom-my1-Black\r\n　幾h幾v：每格幾點寬、幾點高的棋盤（1~8），例：2h1v、1h2v\r\n　名稱後加 .亮度（0~255）調整亮度，不寫為 255，例：Custom-dot1.127-Black、Custom-Gray-2h1v.64\r\nCrossTalk：excel1 ~ excel7",
                "選項：窗口大小%＋位置\r\nc：置中，例：50%c\r\nxy_X.Y：窗口左上角像素，例：50%xy_0.0\r\nxyq_象限：++ 右上、-+ 左上、-- 左下、+- 右下，例：50%xyq_++\r\ntwo：兩組窗口，例：50%two\r\n蒙太奇：結尾加 |名稱+名稱|亮度（亮度省略為 255），例：50%c|dot1+row2+ldiag2|127（也可用自訂 mask 名稱）"),
            ["Mask"] = (
                "參數：單位與大小\r\np-寬*高：以像素為單位，例：p-2*2\r\ns-寬*高-亮度：以子像素 (RGB) 為單位，亮度 0~255，例：s-2*2-127",
                "選項：每一格的顏色\r\np 顏色寫法：每格寫一個顏色，格之間空格、列之間 /\r\n　" + ConfigColorHelp + "\r\n　也可寫 0 黑、1 白、2 灰，例：red lime/blue 127.127.127\r\n　（只有 1 格且用 R.G.B 時，結尾加 /，例：255.0.0/）\r\np 精簡寫法：每格 1 個字元 0 黑、1 白、2 灰，列之間 .，例：10.01\r\ns：每像素 3 個字元（子像素由左到右，BGR 面板為 B G R），1 亮、0 暗，列之間 .，例：101010.010101"),
            [UserMaskType] = (
                "自訂 mask：名稱就是之後使用的名稱（英文字母開頭，可用英文、數字、_）\r\n參數：p-寬*高（1~64），例：p-2*2\r\n可在 Mask 頁畫好後按「存成自訂…」自動產生這一行",
                "選項：每一格的顏色，與 Mask 的 p 寫法相同\r\n精簡寫法：0 黑、1 白、2 灰，列之間 .，例：10.01\r\n顏色寫法：格之間空格、列之間 /，例：red black/black red\r\n使用：Window Custom-my1-Black、蒙太奇 |my1、Mask 頁「自訂」下拉選單\r\n名稱後加 .亮度（0~255）依比例調暗，例：Custom-my1.127-Black"),
            ["Text"] = (
                "參數：背景色-文字色\r\n" + ConfigColorHelp + "\r\n例：Black-White、Navy-Yellow",
                "選項：文字/大小/字距/行距/串距（大小之後可省略）\r\n爆炸模式：文字/boom（字級由 4pt 遞增，最多 30 個字）\r\n字型：再加 /f:字型名稱（例 /f:Arial），粗體再加 /b；不寫為預設字型\r\n文字：最多 10 個字，不能有逗號 , 與斜線 /，保留大小寫\r\n大小：像素 1~500（預設 12）\r\n間距：筆畫之間的空白像素 0~500，0 = 貼齊\r\n　預設 字距 大小/5、行距 大小/3、串距 大小/2\r\n例：測試ABC、ABC/24、ABC/24/0/0/10"),
            ["Dynamic"] = (
                "參數：背景色-物件色（只寫一色時物件為白色）\r\n" + ConfigColorHelp + "\r\n例：Black-White、Gray-Red",
                "選項：可省略，多項用 - 連接（選取時只預覽，按空白鍵或 Esc 全螢幕播放）\r\n形狀：ball 球、cube 方塊、strip 長條、blur 模糊測試條（預設 ball）\r\n方向：h 水平、v 垂直、b 斜角（預設 b，長條不能斜角）\r\n大小：size 加數字 1~200（預設 50）\r\n速度：speed 加數字 1~256（預設 50）\r\n例：ball-b-size50-speed50、strip-h-size20-speed10"),
            ["Test"] = (
                "參數：測試圖\r\ngamma：Gamma 檢查（gamma-r / gamma-g / gamma-b 為單一色版）\r\npluge：黑位 1~20 / 白位 235~254\r\noverscan：邊緣刻度（overscan-10 / overscan-20 / overscan-50，預設 20）\r\nresolution：解析度（西門子星與線對）\r\nhue：色相漸層（hue-h 水平、hue-v 垂直、hue-ring 環狀）\r\nshading：高頻 Shading、banding：高對比 Banding\r\nzoneplate：Zone plate 同心圓、colorchecker：24 色色票\r\nnoise：隨機雜訊，可加 c（彩色）、100pt / 1% / 10% / 50%（數量）、g2 / g4 / g8（顆粒），例：noise-c-10%-g4\r\nclipping-black / clipping-white：截斷條（結尾 -f 閃爍）、geometry：幾何、sharpness：銳利度、darkgrad-32 / darkgrad-64：暗部彩色漸層",
                "選項：不需要"),
            ["Image"] = (
                "參數：圖檔路徑（jpg / png / bmp，不能有逗號）\r\n可寫絕對路徑，或相對於 config 檔所在資料夾的路徑（內建設定則相對於程式資料夾）\r\n例：D:\\Pattern\\logo.png、pic\\gray.bmp",
                "選項：縮放方式（省略為 fit）\r\nfit 等比例（留黑邊）、stretch 拉伸填滿、fill 填滿裁切、1:1 原尺寸置中"),
        };
        private ToolTip configHelp_tip;
        private TextBox cfgName_txt, cfgParam_txt, cfgOption_txt;
        private ComboBox cfgType_cmb;
        private void InitConfigEditor()
        {
            configHelp_tip = new ToolTip { AutoPopDelay = 30000, InitialDelay = 200, ReshowDelay = 100, ShowAlways = true };
            var panel = new FlowLayoutPanel { Dock = DockStyle.Bottom, AutoSize = true, WrapContents = true, Padding = new Padding(2) };
            Label Caption(string text) => new Label { Text = text, AutoSize = true, Margin = new Padding(6, 7, 0, 0) };
            cfgName_txt = new TextBox { Width = 120, ImeMode = ImeMode.NoControl };  // 名稱可打中文
            cfgType_cmb = new ComboBox { Width = 85, DropDownStyle = ComboBoxStyle.DropDownList, ImeMode = ImeMode.Disable };
            cfgType_cmb.Items.AddRange(ConfigHelp.Keys.ToArray());
            cfgParam_txt = new TextBox { Width = 190, ImeMode = ImeMode.Disable };
            cfgOption_txt = new TextBox { Width = 190, ImeMode = ImeMode.Disable };
            var add = new Button { Text = "新增", Width = 50 };
            var update = new Button { Text = "修改", Width = 50 };
            var delete = new Button { Text = "刪除", Width = 50 };
            var moveUp = new Button { Text = "上移", Width = 50 };
            var moveDown = new Button { Text = "下移", Width = 50 };
            add.Click += (s, e) => SaveConfigLine(false);
            update.Click += (s, e) => SaveConfigLine(true);
            delete.Click += (s, e) => DeleteConfigLine();
            moveUp.Click += (s, e) => MoveConfigLine(-1);
            moveDown.Click += (s, e) => MoveConfigLine(+1);
            cfgType_cmb.SelectedIndexChanged += (s, e) => UpdateConfigHelp();
            // 第一行：名稱、類型、按鈕；第二行：參數、選項、config 檔案
            cfgFile_cmb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 150, ImeMode = ImeMode.Disable };
            cfgFile_cmb.DropDown += (s, e) => RefreshConfigFiles();  // 展開時重新掃描資料夾
            cfgFile_cmb.SelectionChangeCommitted += (s, e) => LoadSelectedConfigFile();
            configHelp_tip.SetToolTip(cfgFile_cmb, "程式資料夾中 config 開頭的 .txt；選擇後載入該檔案");
            // 「檔案」標籤與選單包成一組，換行時一起移動
            var fileGroup = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Margin = Padding.Empty };
            fileGroup.Controls.AddRange(new Control[] { Caption("檔案"), cfgFile_cmb });
            panel.Controls.AddRange(new Control[] {
                Caption("名稱"), cfgName_txt, Caption("類型"), cfgType_cmb, add, update, delete, moveUp, moveDown,
                Caption("參數"), cfgParam_txt, Caption("選項"), cfgOption_txt, fileGroup });
            panel.SetFlowBreak(moveDown, true);
            configHelp_tip.SetToolTip(moveUp, "選取項目往上移一列");
            configHelp_tip.SetToolTip(moveDown, "選取項目往下移一列");
            configHelp_tip.SetToolTip(cfgName_txt, "顯示在清單上的名稱，不能包含逗號");
            configHelp_tip.SetToolTip(add, "加到目前選取項目的下一列並預覽");
            configHelp_tip.SetToolTip(update, "以編輯區內容取代目前選取的項目");
            configHelp_tip.SetToolTip(delete, "刪除目前選取的項目（右鍵 SaveConfig 存檔後才會寫入檔案）");
            tabImgList.Controls.Add(panel);
            cfgType_cmb.SelectedIndex = 0;
        }  // 建立 config 編輯區
        private ComboBox cfgFile_cmb;
        private const string BuiltinConfigName = "內建設定";
        private void RefreshConfigFiles()
        {
            // 列出程式資料夾中 config 開頭的 .txt，加上內建設定與目前載入的來源
            string current = tabilPatternList_lst.Tag?.ToString();
            var items = new List<string> { BuiltinConfigName };
            items.AddRange(Directory.GetFiles(Application.StartupPath, "config*.txt")
                .Where(path => Path.GetFileName(path).StartsWith("config", StringComparison.OrdinalIgnoreCase))
                .OrderBy(path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(current) && !items.Contains(current, StringComparer.OrdinalIgnoreCase))
            {
                items.Add(current);  // 從其他位置開啟或拖入的檔案
            }
            cfgFile_cmb.Items.Clear();
            foreach (string item in items)
            {
                cfgFile_cmb.Items.Add(new ConfigFileItem(item));
            }
            SyncConfigFileCombo();
        }  // 掃描 config 檔案
        private void SyncConfigFileCombo()
        {
            // 下拉選單顯示目前載入的來源（不重新載入）
            string current = tabilPatternList_lst.Tag?.ToString();
            cfgFile_cmb.SelectedItem = cfgFile_cmb.Items.Cast<ConfigFileItem>().FirstOrDefault(i => string.Equals(i.Path, current, StringComparison.OrdinalIgnoreCase));
        }  // 同步目前來源
        private void LoadSelectedConfigFile()
        {
            if (!(cfgFile_cmb.SelectedItem is ConfigFileItem item)) return;
            if (!ConfirmDiscardConfig())
            {
                BeginInvoke((Action)SyncConfigFileCombo);  // 取消：下拉選單改回目前的來源（等選單收起後再改，否則會被蓋回去）
                return;
            }
            try
            {
                if (item.Path == BuiltinConfigName)
                {
                    LoadConfig(Resources.config, BuiltinConfigName);
                }
                else
                {
                    LoadConfig(ReadConfigText(item.Path), item.Path);
                }
                ssrStatus_lbl.Text = $"Config：{item.Path}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"讀取檔案失敗：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SyncConfigFileCombo();
            }
        }  // 載入選擇的 config
        private class ConfigFileItem
        {
            public string Path { get; }
            public ConfigFileItem(string path) { Path = path; }
            public override string ToString() => Path == BuiltinConfigName ? Path : System.IO.Path.GetFileName(Path);
        }  // 下拉選單項目：顯示檔名，保留完整路徑
        private void UpdateConfigHelp()
        {
            // 參數與選項的 tooltip 依類型顯示對應寫法
            string type = cfgType_cmb.SelectedItem?.ToString();
            if (type == null || !ConfigHelp.TryGetValue(type, out var help)) return;
            configHelp_tip.SetToolTip(cfgType_cmb, T($"{type}：滑鼠移到參數、選項欄位可看寫法"));  // 英文介面時翻譯
            configHelp_tip.SetToolTip(cfgParam_txt, T(help.param));
            configHelp_tip.SetToolTip(cfgOption_txt, T(help.option + "\r\n\r\n幻燈片停留時間（各類型通用）：結尾加 @秒數s，例：@5s、50%c @2.5s（0.1~3600 秒）"));
        }  // 更新寫法提示
        private void FillConfigEditor(ListViewItem item)
        {
            // SubItems：0 編號、1 名稱、2 類型、3 參數、4 選項
            cfgName_txt.Text = item.SubItems[1].Text;
            string type = ConfigHelp.Keys.FirstOrDefault(k => k.Equals(item.SubItems[2].Text, StringComparison.OrdinalIgnoreCase));
            if (type != null) cfgType_cmb.SelectedItem = type;
            cfgParam_txt.Text = item.SubItems[3].Text;
            cfgOption_txt.Text = item.SubItems[4].Text;
        }  // 選取項目帶入編輯區
        private void SaveConfigLine(bool replaceSelected)
        {
            string[] values = { cfgName_txt.Text.Trim(), cfgType_cmb.SelectedItem?.ToString() ?? "", cfgParam_txt.Text.Trim(), cfgOption_txt.Text.Trim() };
            if (values[0] == "" || values[2] == "")
            {
                MessageBox.Show("請填寫名稱與參數！", "Config", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (values.Any(v => v.Contains(",")))
            {
                MessageBox.Show("欄位內容不能包含逗號（逗號是欄位分隔符號）！", "Config", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (SuspendFilter())  // 搜尋中：先放回完整清單，改完再篩選
            {
                ListViewItem item = CreateConfigItem(string.Join(",", values));
                int selected = tabilPatternList_lst.SelectedIndices.Count > 0 ? tabilPatternList_lst.SelectedIndices[0] : -1;
                if (replaceSelected)
                {
                    if (selected < 0)
                    {
                        MessageBox.Show("請先在清單選取要修改的項目！", "Config", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    PushConfigUndo();  // 記錄復原點
                    ListViewGroup group = tabilPatternList_lst.Items[selected].Group;
                    tabilPatternList_lst.Items[selected] = item;
                    if (tabilPatternList_lst.ShowGroups) item.Group = group;  // 維持原本的群組
                }
                else
                {
                    // 加在選取項目的下一列；沒有選取時加在最後
                    PushConfigUndo();  // 記錄復原點
                    tabilPatternList_lst.Items.Insert(selected >= 0 ? selected + 1 : tabilPatternList_lst.Items.Count, item);
                    AdoptNeighborGroup(item);
                }
                RenumberPatternList();
                item.Selected = true;  // 觸發預覽，格式錯誤會提示
                item.Focused = true;
                item.EnsureVisible();
            }
        }  // 新增 / 修改 config 項目
        private void DeleteConfigLine()
        {
            if (tabilPatternList_lst.SelectedItems.Count == 0) return;
            using (SuspendFilter())
            {
                int index = tabilPatternList_lst.SelectedIndices[0];
                PushConfigUndo();  // 記錄復原點
                tabilPatternList_lst.Items.RemoveAt(index);
                RenumberPatternList();
                if (tabilPatternList_lst.Items.Count > 0)
                {
                    tabilPatternList_lst.Items[Math.Min(index, tabilPatternList_lst.Items.Count - 1)].Selected = true;
                }
            }
        }  // 刪除 config 項目
        private bool _movingConfigLine;  // 移動時重新選取不重新產生圖片
        private void MoveConfigLine(int step)
        {
            if (tabilPatternList_lst.SelectedItems.Count == 0 || BlockWhileFiltering("調整順序")) return;
            int index = tabilPatternList_lst.SelectedIndices[0];
            int target = index + step;
            if (target < 0 || target >= tabilPatternList_lst.Items.Count) return;
            ListViewItem item = tabilPatternList_lst.Items[index];
            ListViewGroup neighborGroup = tabilPatternList_lst.Items[target].Group;
            if (tabilPatternList_lst.ShowGroups && neighborGroup != item.Group)
            {
                // 在群組邊界：先移到相鄰群組（變成上一組的最後一項 / 下一組的第一項），位置不變
                PushConfigUndo();
                item.Group = neighborGroup;
                RenumberPatternList();
                item.EnsureVisible();
                return;
            }
            PushConfigUndo();  // 記錄復原點
            _movingConfigLine = true;
            try
            {
                tabilPatternList_lst.Items.RemoveAt(index);
                tabilPatternList_lst.Items.Insert(target, item);
                if (neighborGroup != null) item.Group = neighborGroup;  // 移除再插入會清掉群組（同一組內移動）
                RenumberPatternList();
                item.Selected = true;
                item.Focused = true;
                item.EnsureVisible();
            }
            finally
            {
                _movingConfigLine = false;
            }
        }  // 上移 / 下移 config 項目
        private void ConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem targetbutton)
            {
                if (targetbutton.Name == "openConfigToolStripMenuItem")
                {
                    // 使用 OpenFileDialog 選擇配置檔案
                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.Filter = "Text Files|*.txt";
                        if (openFileDialog.ShowDialog() == DialogResult.OK && ConfirmDiscardConfig())
                        {
                            try
                            {
                                LoadConfig(ReadConfigText(openFileDialog.FileName), openFileDialog.FileName);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"讀取檔案失敗：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                else if (targetbutton.Name == "saveConfigToolStripMenuItem")
                {
                    SaveConfigAs();
                }
            }
        }  // 設定檔案載入與儲存
        private bool SaveConfigAs()
        {
            // 儲存配置檔案；回傳是否真的存檔（取消或失敗為 false）
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text Files|*.txt";
                saveFileDialog.FileName = "config.txt";
                string source = tabilPatternList_lst.Tag?.ToString();
                if (!string.IsNullOrEmpty(source) && File.Exists(source))
                {
                    // 預設存回目前的 config 檔
                    saveFileDialog.InitialDirectory = Path.GetDirectoryName(source);
                    saveFileDialog.FileName = Path.GetFileName(source);
                }
                if (saveFileDialog.ShowDialog() != DialogResult.OK) return false;
                // 第一行寫欄名，其餘照原始內容寫回；UTF-8（含 BOM）方便記事本與 Excel 開啟
                List<string> configLines = ConfigLinesWithGroups();  // 含群組標題、搜尋中被篩掉的項目
                var lines = new[] { string.Join(",", ConfigColumns) }.Concat(configLines);
                try
                {
                    File.WriteAllText(saveFileDialog.FileName, string.Join(Environment.NewLine, lines), Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"儲存失敗：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                _savedConfigLines = configLines;
                ssrStatus_lbl.Text = $"已儲存：{saveFileDialog.FileName}";
                return true;
            }
        }  // 另存 config
        private List<string> _savedConfigLines;  // 載入或儲存時的內容，用來判斷有沒有變更
        private bool ConfigModified => _savedConfigLines != null && !_savedConfigLines.SequenceEqual(ConfigLinesWithGroups());  // 復原回原樣也算沒變更
        private bool ConfirmDiscardConfig()
        {
            // config 有變更時詢問是否儲存：是（開啟儲存，存檔成功才繼續）、否（放棄變更繼續）、取消（不繼續）；回傳是否繼續
            if (!ConfigModified) return true;
            DialogResult answer = MessageBox.Show(this, T("ImgList 的 config 已變更，要儲存嗎？"), "PatternMagic", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            return answer == DialogResult.No || (answer == DialogResult.Yes && SaveConfigAs());  // 儲存對話框按取消也不繼續
        }  // 換掉或關閉 config 前確認
        private void InitConfigSavePrompt()
        {
            // 關閉前提醒儲存（換 config 時在開啟、拖入、下拉選單各自呼叫 ConfirmDiscardConfig）
            FormClosing += (s, e) =>
            {
                if (e.Cancel || e.CloseReason == CloseReason.TaskManagerClosing) return;
                if (!ConfirmDiscardConfig()) e.Cancel = true;
            };
        }  // 關閉前提醒儲存 config
        private static readonly Color ConfigErrorBackColor = Color.FromArgb(255, 215, 215);
        private void MarkConfigItem(ListViewItem item, string error)
        {
            // 有錯的列標成紅底紅字，滑鼠移上去顯示原因
            tabilPatternList_lst.ShowItemToolTips = true;
            item.UseItemStyleForSubItems = true;
            item.BackColor = error == null ? tabilPatternList_lst.BackColor : ConfigErrorBackColor;
            item.ForeColor = error == null ? tabilPatternList_lst.ForeColor : Color.DarkRed;
            item.ToolTipText = error == null ? "" : $"設定有誤：{error}";
        }  // 標示 config 錯誤
        private int ValidateAllConfigLines()
        {
            // 逐行解析檢查格式（不產生圖片、不跳訊息），結束後恢復原本的功能頁與預覽
            if (tabilPatternList_lst.Items.Count == 0 || mypicture == null) return 0;
            int function = tabimgeFuncList.SelectedIndex;
            int bad = 0;
            // 解析時會改到 ImgEditor 的設定，先記下來，檢查完全部還原
            Action restoreEditor = SnapshotEditor();
            // 啟動時先載入 config、之後才依螢幕填入寬高：寬高還沒設定時暫時用主螢幕解析度檢查
            string width = mnsW_txt.Text, height = mnsH_txt.Text;
            bool sizeMissing = !int.TryParse(width, out _) || !int.TryParse(height, out _);
            if (sizeMissing)
            {
                mnsW_txt.Text = ScreenPixelSize(Screen.PrimaryScreen).Width.ToString();
                mnsH_txt.Text = ScreenPixelSize(Screen.PrimaryScreen).Height.ToString();
            }
            Cursor = Cursors.WaitCursor;
            tabilPatternList_lst.BeginUpdate();
            try
            {
                foreach (ListViewItem item in tabilPatternList_lst.Items)
                {
                    var errors = new List<string>();
                    bool ok = ApplyConfigLine(item.Tag.ToString(), errors, parseOnly: true);
                    MarkConfigItem(item, ok ? null : errors.FirstOrDefault() ?? "無法產生圖片");
                    if (!ok) bad++;
                }
            }
            finally
            {
                tabilPatternList_lst.EndUpdate();
                Cursor = Cursors.Default;
                if (sizeMissing)
                {
                    mnsW_txt.Text = width;
                    mnsH_txt.Text = height;
                }
                restoreEditor();
            }
            if (tabilPatternList_lst.SelectedItems.Count > 0)
            {
                ApplyConfigLine(tabilPatternList_lst.SelectedItems[0].Tag.ToString(), new List<string>());
            }
            else
            {
                if (function >= 0) tabimgeFuncList.SelectedIndex = function;
                showimgPicture_pic.Image = null;  // 沒有選取時不留下最後檢查的那張
                showimgSize_lbl.Text = "";
                ShowConfigNote(null);
            }
            ssrStatus_lbl.Text = bad == 0
                ? $"Config：{tabilPatternList_lst.Tag}（{tabilPatternList_lst.Items.Count} 行皆正確）"
                : $"Config：{tabilPatternList_lst.Tag}（有 {bad} 行設定錯誤，已標紅，滑鼠移上去看原因）";
            return bad;
        }  // 檢查 ImgList 全部設定
        private Action SnapshotEditor()
        {
            // 記下 ImgEditor 分頁所有輸入欄位的值，回傳還原用的動作
            var setters = new List<Action>();
            var blockColors = new List<(Panel panel, Color[] colors)>();
            void Walk(Control parent)
            {
                foreach (Control c in parent.Controls)
                {
                    switch (c)
                    {
                        case NumericUpDown nud:
                            decimal value = nud.Value;
                            setters.Add(() => { if (nud.Value != value) nud.Value = Math.Max(nud.Minimum, Math.Min(nud.Maximum, value)); });
                            continue;  // 不往內找（內含的文字框不用記）
                        case ComboBox combo when combo.DropDownStyle == ComboBoxStyle.DropDownList:
                            int index = combo.SelectedIndex;
                            setters.Add(() => { if (combo.SelectedIndex != index && index < combo.Items.Count) combo.SelectedIndex = index; });
                            continue;
                        case ComboBox combo:
                            string comboText = combo.Text;
                            setters.Add(() => { if (combo.Text != comboText) combo.Text = comboText; });
                            continue;
                        case RadioButton radio:
                            if (radio.Checked) setters.Add(() => { if (!radio.Checked) radio.Checked = true; });
                            continue;
                        case CheckBox check:
                            bool isChecked = check.Checked;
                            setters.Add(() => { if (check.Checked != isChecked) check.Checked = isChecked; });
                            continue;
                        case ScrollBar scroll:
                            int scrollValue = scroll.Value;
                            setters.Add(() => { if (scroll.Value != scrollValue) scroll.Value = scrollValue; });
                            continue;
                        case TextBox box:
                            string text = box.Text;
                            setters.Add(() => { if (box.Text != text) box.Text = text; });
                            continue;
                        case PictureBox picture:
                            Color pictureBack = picture.BackColor;
                            Image image = picture.Image;
                            setters.Add(() => { picture.BackColor = pictureBack; picture.Image = image; });
                            continue;
                        case Label _:
                        case Button _:
                            Color back = c.BackColor;
                            setters.Add(() => { if (c.BackColor != back) c.BackColor = back; });
                            continue;
                        case Panel panel when panel.Controls.Count > 0 && panel.Controls.Cast<Control>().All(b => b is Panel && b.Tag != null):
                            // Mask 格子、ColorBar 色條：面板會依格數重建，最後再依序套回顏色
                            blockColors.Add((panel, panel.Controls.Cast<Control>().Select(b => b.BackColor).ToArray()));
                            continue;
                    }
                    Walk(c);
                }
            }
            Walk(tabImgEditor);
            string pictureTag = mypicture.tag;
            return () =>
            {
                // 跑兩次：前一次設定可能觸發連動（例如選長條會改方向），第二次蓋回原值
                for (int pass = 0; pass < 2; pass++) setters.ForEach(set => set());
                foreach (var (panel, colors) in blockColors)
                {
                    var blocks = panel.Controls.Cast<Control>().ToList();
                    if (blocks.Count != colors.Length) continue;  // 格數不同就不套
                    for (int i = 0; i < blocks.Count; i++) blocks[i].BackColor = colors[i];
                }
                mypicture.tag = pictureTag;
            };
        }  // 記錄 / 還原 ImgEditor 設定
    }
}
