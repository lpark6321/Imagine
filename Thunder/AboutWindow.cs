using System.Drawing;
using System.Windows.Forms;

namespace Thunder
{
    public class AboutWindow : Form
    {
        //快捷鍵清單（新增快捷鍵時記得同步更新這裡）-------------------------------------------------------------------
        private static readonly (string group, string key, string action)[] Shortcuts =
        {
            ("主視窗", "Alt+Space", "依 ImgEditor 設定生成圖片"),
            ("主視窗", "空白鍵", "ImgEditor / ImgList：用目前寬高重新產生並全螢幕；DirList：全螢幕"),
            ("主視窗", "Esc / Alt+Z", "全螢幕顯示目前圖片"),
            ("主視窗", "雙擊預覽圖", "全螢幕顯示目前圖片"),
            ("主視窗", "Ctrl+S", "AutoSave（自動編號存檔，勾選批量時批量輸出）"),
            ("主視窗", "Alt+S", "另存新檔（選擇檔名）"),
            ("主視窗", "F1 ~ F8", "切換 ImgEditor 功能（F8：Testpattern）"),
            ("主視窗", "F11", "切換到 DirList 分頁"),
            ("主視窗", "F12", "切換到 ImgList 分頁"),
            ("主視窗", "= / -（主鍵盤）", "切換輸出螢幕（寬高跟著改，並依分頁重新產生圖片）"),
            ("主視窗", "Enter / Generate", "ImgList 編輯區撰寫 config 時：預覽草稿（不加入清單）"),
            ("主視窗", "Ctrl+Z", "ImgList：復原新增 / 修改 / 刪除 / 移動 / 拖拉排序（最多 20 步）"),
            ("主視窗", "雙擊最近使用", "開啟最近使用的圖片（單擊只選取）"),
            ("全螢幕視窗", "Esc", "關閉全螢幕"),
            ("全螢幕視窗", "空白鍵", "Dynamic：開始 / 暫停小球；其他：關閉全螢幕"),
            ("全螢幕視窗", "F1 / 雙擊", "切換全螢幕 / 視窗"),
            ("全螢幕視窗", "← / →", "DirList / ImgList 上一張 / 下一張"),
            ("全螢幕視窗", "滑鼠滾輪", "往上滾上一張、往下滾下一張（同 ← / →）"),
            ("全螢幕視窗", "W / A / S / D", "滑鼠游標移動 1 像素"),
            ("全螢幕視窗", "= / -、數字鍵盤 + / -", "切換輸出螢幕，全螢幕直接移過去"),
            ("全螢幕視窗", "Ctrl+Z", "還原畫圖（最多 10 步）"),
            ("全螢幕視窗", "R", "開始 / 停止每 1 秒幻燈片"),
            ("全螢幕視窗", "Alt+R", "自動掃灰階：從目前亮度掃到 255（右鍵選單可設步進與間隔），再按一次停在目前階數"),
            ("全螢幕視窗", "數字鍵盤 + Enter", "純色圖直接跳到輸入的階數（Backspace 刪一碼、Esc 取消輸入）"),
            ("全螢幕視窗", "1 / 2 / 3 / 4", "純色圖切換色版：R / G / B / 灰階（之後的 ↑↓、輸入階數、Alt+R 都作用在該色版）"),
            ("全螢幕視窗", "B", "目前這張與上一張（換圖或調整階數前）互換比對"),
            ("全螢幕視窗", "G", "格線：每 100px → 每 10px → 關，上緣與左緣標示座標"),
            ("全螢幕視窗", "N", "反相（負片），再按一次恢復"),
            ("全螢幕視窗", "Q", "拖曳窗口（Window 圖樣）：滑鼠拖曳窗口，方向鍵移動 1px（Shift 10px），Ctrl+滾輪調整大小"),
            ("全螢幕視窗", "K", "燒機 / 殘影計時：開始 / 停止（右鍵選單可設定時間與時間到的動作）"),
            ("全螢幕視窗", "Backspace", "顯示座標時清除量尺起點"),
            ("全螢幕視窗", "I", "資訊列：位置、圖片 / 螢幕尺寸、更新率、是否 1:1 顯示"),
            ("全螢幕視窗", "T / E", "幻燈片播放中：間隔加 / 減 1 秒"),
            ("全螢幕視窗", "↑ / ↓、PgUp / PgDn", "純色圖：階數 ±1 / ±16（灰階三色一起，單色只調該色）"),
            ("全螢幕視窗", "M","放大鏡開 / 關（右鍵選單可選 4 / 8 / 16 / 32 倍）"),
            ("全螢幕視窗", "C", "顏色吸管開 / 關，左鍵複製 R.G.B"),
            ("全螢幕視窗", "右鍵", "選單：座標、幻燈片、旋轉 / 翻轉、縮放、畫圖、螢幕"),
            ("EDID 視窗", "Esc", "關閉"),
        };
        //AboutWindow類別----------------------------------------------------------------------------------------------
        public AboutWindow()
        {
            Text = "About";
            Size = new Size(620, 640);
            StartPosition = FormStartPosition.CenterParent;
            ShowIcon = false;
            MinimizeBox = false;
            MaximizeBox = false;
            KeyPreview = true;
            KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) Close(); };
            Load += (s, e) => mainWindow.Localize(this);  // 英文介面

            var title = new Label
            {
                Text = $"PatternMagic  {mainWindow.T("版本")} {typeof(AboutWindow).Assembly.GetName().Version}",  // 版本來自 AssemblyInfo.cs
                Dock = DockStyle.Top,
                Height = 40,
                Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 0, 0),
            };
            var author = new Label
            {
                Text = "作者：Ethan Huang",
                Dock = DockStyle.Bottom,
                Height = 32,
                Font = new Font("Microsoft JhengHei UI", 9F),
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 8, 0),
            };
            var list = new ListView
            {
                View = View.Details,
                Dock = DockStyle.Fill,
                FullRowSelect = true,
                HeaderStyle = ColumnHeaderStyle.Nonclickable,
                Font = new Font("Microsoft JhengHei UI", 9F),
            };
            list.Columns.Add("按鍵", 150);
            list.Columns.Add("功能", 420);
            foreach (var (group, key, action) in Shortcuts)
            {
                ListViewGroup listGroup = list.Groups[group] ?? list.Groups.Add(group, mainWindow.T(group));
                list.Items.Add(new ListViewItem(new[] { mainWindow.T(key), mainWindow.T(action) }, listGroup));
            }
            Controls.Add(list);
            Controls.Add(title);
            Controls.Add(author);
        }
    }
}
