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
        // 多螢幕-------------------------------------------------------------------------------------------------------
        private enum MultiScreenMode { Single, SyncOthers, SeparateOthers, SequenceOthers }
        private MultiScreenMode _multiMode = MultiScreenMode.Single;
        private ToolStripComboBox mnsMultiScreen_cmb;
        private readonly Dictionary<string, pictureWindow> _screenWindows = new Dictionary<string, pictureWindow>();
        private bool _updatingScreens;      // 產生 / 顯示多螢幕時，忽略視窗啟用事件
        private bool _closingAllScreens;
        private bool _restoringSelection;   // 還原分離螢幕的選取時，DirList 不要再觸發全螢幕
        private bool _skipScreenRegenerate; // 切換作用中螢幕時先不重新產生（之後還原選取會產生）
        private class ScreenSelection
        {
            public TabPage Tab;
            public int Index = -1;
            public RecentItem Shown;
        }  // 分離模式每個螢幕記住的分頁與項目
        private void InitMultiScreen()
        {
            // 螢幕清單左邊的多螢幕模式；預設單一螢幕，與原本相同
            mnsMultiScreen_cmb = new ToolStripComboBox
            {
                Alignment = ToolStripItemAlignment.Right, DropDownStyle = ComboBoxStyle.DropDownList, AutoSize = false, Size = new Size(130, 23),
                ToolTipText = "單一螢幕：全螢幕顯示在右邊選的螢幕\r\n" +
                              "主螢幕之外同步：主螢幕以外的螢幕全部顯示同一張圖、一起換圖\r\n" +
                              "分離第二視窗：主螢幕以外每個螢幕各自顯示，點哪個螢幕就控制哪個（主視窗會跳到那個螢幕的圖）\r\n" +
                              "主螢幕之外依序：第二螢幕顯示目前的圖、第三螢幕顯示下一張…，← → 一起往下換\r\n" +
                              "多螢幕時各螢幕依自己的解析度產生圖片；任一個全螢幕關閉時全部關閉",
            };
            mnsMultiScreen_cmb.ComboBox.ImeMode = ImeMode.Disable;
            mnsMultiScreen_cmb.Items.AddRange(new object[] { "單一螢幕", "主螢幕之外同步", "分離第二視窗", "主螢幕之外依序" });
            mnsMultiScreen_cmb.SelectedIndex = 0;
            mnsMultiScreen_cmb.SelectedIndexChanged += (s, e) =>
            {
                CloseAllFullScreen();  // 換模式時關掉目前的全螢幕，下次全螢幕依新模式開啟
                _multiMode = (MultiScreenMode)mnsMultiScreen_cmb.SelectedIndex;
                int count = OtherScreens().Count;
                ssrStatus_lbl.Text = _multiMode == MultiScreenMode.Single || count > 0
                    ? $"全螢幕模式：{mnsMultiScreen_cmb.Text}{(_multiMode == MultiScreenMode.Single ? "" : $"（主螢幕以外 {count} 個螢幕）")}"
                    : "只有主螢幕，多螢幕模式會改用單一螢幕";
            };
            // 右對齊項目依集合順序由右往左排，插在螢幕清單後面 = 顯示在它左邊
            menuStrip.Items.Insert(menuStrip.Items.IndexOf(mnsScreenlist_cmb) + 1, mnsMultiScreen_cmb);
        }  // 建立多螢幕選單
        private static List<Screen> OtherScreens() =>
            Screen.AllScreens.Where(s => !s.Primary).OrderBy(s => s.Bounds.X).ThenBy(s => s.Bounds.Y).ToList();  // 主螢幕以外，由左到右
        private static string ScreenLabel(string device) => device?.TrimStart('\\', '.') ?? "";
        private void FullScreenMulti(List<Screen> others)
        {
            string selected = mnsScreenlist_cmb.SelectedItem?.ToString();
            Screen active = others.FirstOrDefault(s => s.DeviceName == selected) ?? others[0];
            // 要產生的螢幕與偏移（依序模式第 n 個螢幕顯示後面第 n 張）
            var renders = new List<(Screen screen, int offset)>();
            switch (_multiMode)
            {
                case MultiScreenMode.SyncOthers:
                    renders.AddRange(others.Select(s => (s, 0)));
                    break;
                case MultiScreenMode.SequenceOthers:
                    renders.AddRange(others.Select((s, i) => (s, i)));
                    break;
                default:  // 分離：只更新作用中螢幕；還沒開的螢幕先顯示同一張
                    renders.AddRange(others.Where(s => s == active || !HasScreenWindow(s.DeviceName)).Select(s => (s, 0)));
                    break;
            }
            // 主視窗預覽要停在「偏移 0、作用中螢幕」的結果，所以它排最後產生
            renders = renders.OrderBy(r => r.offset == 0 ? 1 : 0).ThenBy(r => r.screen == active ? 1 : 0).ToList();
            string width = mnsW_txt.Text, height = mnsH_txt.Text;
            _updatingScreens = true;
            try
            {
                Screen lastScreen = null;
                foreach (var (screen, offset) in renders)
                {
                    using (Bitmap bitmap = RenderForScreen(screen, offset, out string caption))
                    {
                        pictureWindow window = ShowOnScreen(screen, bitmap, caption);
                        if (_multiMode == MultiScreenMode.SeparateOthers) window.Selection = CaptureSelection();
                    }
                    lastScreen = screen;
                }
                mnsW_txt.Text = width;
                mnsH_txt.Text = height;
                if (lastScreen != null && (ScreenPixelSize(lastScreen).Width.ToString() != width || ScreenPixelSize(lastScreen).Height.ToString() != height))
                {
                    RegenerateCurrent();  // 選的螢幕解析度不同（例如選主螢幕）：主視窗預覽恢復成原本寬高
                }
                _pictureWindow = _screenWindows.TryGetValue(active.DeviceName, out var activeWindow) ? activeWindow : _pictureWindow;
                _pictureWindow?.Activate();
            }
            finally
            {
                _updatingScreens = false;
            }
            if (_dynamicBackground != null && ReferenceEquals(mypicture.Getpicture(), _dynamicBackground) && !timerDynamic.Enabled)
            {
                timerDynamic.Start();  // 動畫只在作用中螢幕播放
            }
        }  // 多螢幕全螢幕
        private bool HasScreenWindow(string device) => _screenWindows.TryGetValue(device, out var w) && !w.IsDisposed;
        private Bitmap RenderForScreen(Screen screen, int offset, out string caption)
        {
            // 依螢幕解析度產生目前的圖；offset > 0 時改用清單後面第 offset 項（ImgList / DirList）
            mnsW_txt.Text = ScreenPixelSize(screen).Width.ToString();
            mnsH_txt.Text = ScreenPixelSize(screen).Height.ToString();
            if (offset > 0 && _shownItem == null)
            {
                if (tabControl.SelectedTab == tabImgList && tabilPatternList_lst.SelectedIndices.Count > 0)
                {
                    var item = tabilPatternList_lst.Items[(tabilPatternList_lst.SelectedIndices[0] + offset) % tabilPatternList_lst.Items.Count];
                    caption = item.SubItems[1].Text;
                    if (ApplyConfigLine(item.Tag.ToString(), new List<string>()) && mypicture.Getpicture() != null)
                    {
                        return new Bitmap(showimgPicture_pic.Image);  // Dynamic 時是含物件的預覽
                    }
                    return ErrorBitmap(screen, $"{caption}：設定有誤");
                }
                if (tabControl.SelectedTab == tabDirList && tabdlPatternList_lvw.SelectedIndices.Count > 0)
                {
                    var item = tabdlPatternList_lvw.Items[(tabdlPatternList_lvw.SelectedIndices[0] + offset) % tabdlPatternList_lvw.Items.Count];
                    caption = item.Text;
                    try
                    {
                        using (var stream = new FileStream(item.Tag.ToString(), FileMode.Open, FileAccess.Read))
                        using (var image = Image.FromStream(stream))
                        {
                            return new Bitmap(image);
                        }
                    }
                    catch (Exception ex)
                    {
                        return ErrorBitmap(screen, $"{caption}：{ex.Message}");
                    }
                }
            }
            RegenerateCurrent();
            caption = CurrentImageName();
            Bitmap current = mypicture.Getpicture();
            return current != null ? new Bitmap(current) : ErrorBitmap(screen, "沒有圖片");
        }  // 產生某個螢幕要顯示的圖
        private static Bitmap ErrorBitmap(Screen screen, string message)
        {
            var bitmap = new Bitmap(ScreenPixelSize(screen).Width, ScreenPixelSize(screen).Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            using (Font font = new Font("新細明體", 24f))
            {
                g.Clear(Color.Black);
                g.DrawString(message, font, Brushes.White, 20, 20);
            }
            return bitmap;
        }  // 無法產生時顯示的圖
        private pictureWindow ShowOnScreen(Screen screen, Bitmap bitmap, string caption)
        {
            if (!_screenWindows.TryGetValue(screen.DeviceName, out pictureWindow window) || window.IsDisposed)
            {
                window = new pictureWindow { ScreenDevice = screen.DeviceName, StartPosition = FormStartPosition.Manual };
                window.FormClosed += (s, e) => OnScreenWindowClosed((pictureWindow)s);
                window.Activated += (s, e) => OnScreenWindowActivated((pictureWindow)s);
                _screenWindows[screen.DeviceName] = window;
            }
            window.setBitmap(bitmap);
            window.SetBlink(BlinkAlternateFor(mypicture.Getpicture()));  // 這個螢幕的圖是 Clipping 閃爍時
            window.Caption = caption;
            if (!window.Visible)
            {
                PlaceFullScreenWindow(window, screen);  // 自動調整縮放時為實際像素座標
                window.Show();
                window.WindowState = FormWindowState.Maximized;
            }
            return window;
        }  // 顯示到指定螢幕
        private ScreenSelection CaptureSelection()
        {
            var selection = new ScreenSelection { Tab = tabControl.SelectedTab, Shown = _shownItem };
            if (selection.Tab == tabImgList && tabilPatternList_lst.SelectedIndices.Count > 0) selection.Index = tabilPatternList_lst.SelectedIndices[0];
            if (selection.Tab == tabDirList && tabdlPatternList_lvw.SelectedIndices.Count > 0) selection.Index = tabdlPatternList_lvw.SelectedIndices[0];
            return selection;
        }  // 記錄目前分頁與項目
        private void OnScreenWindowActivated(pictureWindow window)
        {
            // 分離模式：點哪個螢幕就控制哪個，主視窗跳回那個螢幕顯示的圖
            if (_updatingScreens || _closingAllScreens || _multiMode != MultiScreenMode.SeparateOthers) return;
            if (ReferenceEquals(window, _pictureWindow)) return;
            _pictureWindow = window;
            _skipScreenRegenerate = true;
            try
            {
                mnsScreenlist_cmb.SelectedItem = window.ScreenDevice;  // 寬高改成這個螢幕的解析度
            }
            finally
            {
                _skipScreenRegenerate = false;
            }
            if (window.Selection is ScreenSelection selection)
            {
                _restoringSelection = true;
                try
                {
                    tabControl.SelectedTab = selection.Tab;
                    ListView list = selection.Tab == tabImgList ? tabilPatternList_lst : selection.Tab == tabDirList ? tabdlPatternList_lvw : null;
                    if (list != null && selection.Index >= 0 && selection.Index < list.Items.Count)
                    {
                        list.Items[selection.Index].Selected = true;
                        list.Items[selection.Index].Focused = true;
                        list.Items[selection.Index].EnsureVisible();
                    }
                    _shownItem = selection.Shown;
                    RegenerateCurrent();
                }
                finally
                {
                    _restoringSelection = false;
                }
            }
            window.FlashMessage($"{ScreenLabel(window.ScreenDevice)} 作用中");
        }  // 分離模式切換作用中螢幕
        private void OnScreenWindowClosed(pictureWindow window)
        {
            if (window.ScreenDevice != null && _screenWindows.TryGetValue(window.ScreenDevice, out var stored) && ReferenceEquals(stored, window))
            {
                _screenWindows.Remove(window.ScreenDevice);
            }
            if (!_closingAllScreens) CloseAllFullScreen();  // 關掉其中一個（Esc）時全部關閉
        }  // 全螢幕視窗關閉
        private void CloseAllFullScreen()
        {
            _closingAllScreens = true;
            try
            {
                foreach (pictureWindow window in _screenWindows.Values.ToList())
                {
                    if (!window.IsDisposed) window.Close();
                }
                if (_pictureWindow != null && !_pictureWindow.IsDisposed) _pictureWindow.Close();
            }
            finally
            {
                _closingAllScreens = false;
            }
        }  // 關閉所有全螢幕
    }
}
