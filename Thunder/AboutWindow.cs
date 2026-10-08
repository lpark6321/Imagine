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
            ("主視窗", "F1 ~ F7", "切換 ImgEditor 功能"),
            ("主視窗", "F11", "切換到 DirList 分頁"),
            ("主視窗", "F12", "切換到 ImgList 分頁"),
            ("主視窗", "= / -（主鍵盤）", "切換輸出螢幕（寬高跟著改，並依分頁重新產生圖片）"),
            ("主視窗", "Enter / Generate", "ImgList 編輯區撰寫 config 時：預覽草稿（不加入清單）"),
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

            var title = new Label
            {
                Text = $"PatternMagic  版本 {typeof(AboutWindow).Assembly.GetName().Version}",  // 版本來自 AssemblyInfo.cs
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
                ListViewGroup listGroup = list.Groups[group] ?? list.Groups.Add(group, group);
                list.Items.Add(new ListViewItem(new[] { key, action }, listGroup));
            }
            Controls.Add(list);
            Controls.Add(title);
            Controls.Add(author);
        }
    }
}
