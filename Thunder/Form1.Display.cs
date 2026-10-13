using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Thunder
{
    public partial class mainWindow
    {
        // 顯示器：更新頻率、縮放比例、投影模式--------------------------------------------------------------------------
        private ToolStripMenuItem mnsDisplay_btn;
        private readonly ContextMenuStrip _refreshMenu = new ContextMenuStrip();
        private void InitDisplayMenu()
        {
            // EDID 右邊的「顯示器」選單；工具列右邊的更新率點一下也能直接換頻率
            mnsDisplay_btn = new ToolStripMenuItem("顯示器") { ToolTipText = "目前選擇螢幕的更新頻率、縮放比例與投影模式" };
            var refresh = new ToolStripMenuItem("更新頻率");
            var resolution = new ToolStripMenuItem("解析度");
            var rotation = new ToolStripMenuItem("旋轉");
            var projection = new ToolStripMenuItem("投影模式");
            foreach (var (menu, fill) in new (ToolStripMenuItem, Action<ToolStripItemCollection>)[]
                { (refresh, FillRefreshItems), (resolution, FillResolutionItems), (rotation, FillRotationItems), (projection, FillProjectionItems) })
            {
                menu.DropDownItems.Add("");  // 先放一個項目才會顯示子選單箭頭，展開時再填入
                menu.DropDownOpening += (s, e) => fill(menu.DropDownItems);
            }
            var keepAwake = new ToolStripMenuItem("全螢幕時防止螢幕關閉") { CheckOnClick = true, Checked = KeepAwakeEnabled,
                ToolTipText = "全螢幕顯示時不讓 Windows 關閉螢幕或進入睡眠（長時間跑圖、殘影測試用）" };
            keepAwake.CheckedChanged += (s, e) => SetKeepAwake(keepAwake.Checked);
            var autoScale = new ToolStripMenuItem("自動調整縮放") { CheckOnClick = true, Checked = AutoDpiScaling,
                ToolTipText = "Windows 縮放不是 100% 時，全螢幕改用實際解析度 1:1 輸出（例：150% 的 1280 x 800 改成 1920 x 1200）" };
            autoScale.CheckedChanged += (s, e) => SetAutoDpiScaling(autoScale.Checked);
            mnsDisplay_btn.DropDownItems.AddRange(new ToolStripItem[]
            {
                refresh, resolution, rotation, projection, new ToolStripSeparator(),
                autoScale, keepAwake, new ToolStripSeparator(),
                new ToolStripMenuItem("量測實際更新率…", null, (s, e) => MeasureSelectedRefresh()) { ToolTipText = "以垂直同步訊號量測目前螢幕實際的更新率、抖動與掉幀（約 2 秒）" },
                new ToolStripMenuItem("識別螢幕", null, (s, e) => IdentifyScreens()) { ToolTipText = "每個螢幕顯示編號與解析度、頻率、縮放、HDR" },
                new ToolStripMenuItem("螢幕資訊…", null, (s, e) => ShowScaleReport()) { ToolTipText = "所有螢幕的解析度、頻率、縮放、HDR、色深、夜燈與色彩描述檔" },
                new ToolStripMenuItem("開啟 Windows 顯示設定（縮放）", null, (s, e) => OpenWindowsSettings("ms-settings:display")),
                new ToolStripMenuItem("開啟 Windows 文字大小設定", null, (s, e) => OpenWindowsSettings("ms-settings:easeofaccess-display")),
            });
            menuStrip.Items.Insert(menuStrip.Items.IndexOf(mnsEdid_btn) + 1, mnsDisplay_btn);
            mnsRefresh_lbl.Click += (s, e) =>
            {
                FillRefreshItems(_refreshMenu.Items);
                _refreshMenu.Show(Cursor.Position);
            };
            // 換頻率、投影模式或插拔螢幕後更新螢幕清單；從 Windows 設定改完縮放回來時更新提示
            SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;
            FormClosed += (s, e) => SystemEvents.DisplaySettingsChanged -= OnDisplaySettingsChanged;  // 靜態事件，關閉時要取消
            Activated += (s, e) => UpdateRefreshRate();
        }  // 建立顯示器選單
        private string SelectedScreenDevice => mnsScreenlist_cmb.SelectedItem?.ToString();
        // 最外層佈局--------------------------------------------------------------------------------------------------
        private SplitContainer _mainSplit;
        private void InitMainLayout()
        {
            // 左欄分頁、右欄預覽改放進 SplitContainer（原本是兩個 Dock = Left）：
            // 兩欄各有寬度下限（原本設計的寬度），視窗大小改變時兩欄依比例自動調整，中間的分隔線也可以直接拖曳；
            // 程式沒有 DPI 感知，關掉主視窗的自動縮放，避免切換螢幕或全螢幕時被重新縮放
            int left = tabControl.Width, right = Math.Max(Showimage_flp.Width, Showimage_flp.PreferredSize.Width);
            const int splitterWidth = 5;
            // 主視窗最小寬度要放得下兩欄的下限加分隔線
            int border = Width - ClientSize.Width;
            MinimumSize = new Size(Math.Max(MinimumSize.Width, left + right + splitterWidth + border), MinimumSize.Height);
            if (Width < MinimumSize.Width) Width = MinimumSize.Width;
            _mainSplit = new SplitContainer
            {
                Dock = DockStyle.Fill, Orientation = Orientation.Vertical, FixedPanel = FixedPanel.None, SplitterWidth = splitterWidth,
                Margin = Padding.Empty, TabStop = false,
            };
            SuspendLayout();
            Controls.Remove(tabControl);
            Controls.Remove(Showimage_flp);
            tabControl.Dock = DockStyle.Fill;
            tabControl.Margin = Padding.Empty;
            Showimage_flp.AutoSize = false;
            Showimage_flp.Dock = DockStyle.Fill;
            _mainSplit.Panel1.Controls.Add(tabControl);
            _mainSplit.Panel2.Controls.Add(Showimage_flp);
            Controls.Add(_mainSplit);
            _mainSplit.BringToFront();  // Fill 要最後停靠（選單、狀態列先佔上下）
            AutoScaleMode = AutoScaleMode.None;
            AutoSize = false;
            ResumeLayout(true);
            // 停靠完成、容器已有寬度後才能設定下限與分隔線位置
            _mainSplit.Panel1MinSize = left;
            _mainSplit.Panel2MinSize = right;
            _mainSplit.SplitterDistance = Math.Max(left, Math.Min(_mainSplit.Width - right - splitterWidth, left));
            toolTip.SetToolTip(_mainSplit, "拖曳中間的分隔線調整左右寬度");
        }  // 建立最外層佈局
        private void OnDisplaySettingsChanged(object sender, EventArgs e)
        {
            if (IsDisposed) return;
            BeginInvoke((Action)(() =>
            {
                // 螢幕可能增減、解析度可能改變：重建清單並盡量保留原本的選擇
                string selected = SelectedScreenDevice;
                _skipScreenRegenerate = true;
                try
                {
                    Screenlist(this, EventArgs.Empty);
                    if (selected != null && mnsScreenlist_cmb.Items.Contains(selected)) mnsScreenlist_cmb.SelectedItem = selected;
                    else if (mnsScreenlist_cmb.Items.Count > 0) mnsScreenlist_cmb.SelectedIndex = 0;
                }
                finally
                {
                    _skipScreenRegenerate = false;
                }
                UpdateRefreshRate();
            }));
        }  // 顯示設定變更
        private static void OpenWindowsSettings(string uri)
        {
            try
            {
                Process.Start(uri);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"無法開啟 Windows 設定：{ex.Message}", "顯示器", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }  // 開啟 Windows 設定頁
        // 更新頻率------------------------------------------------------------------------------------------------------
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int ChangeDisplaySettingsEx(string deviceName, ref DEVMODE devMode, IntPtr hwnd, int flags, IntPtr lParam);
        private const int CDS_UPDATEREGISTRY = 0x1, CDS_TEST = 0x2, DM_DISPLAYFREQUENCY = 0x400000, DM_INTERLACED = 0x2;
        private const int DisplayChangeSeconds = 15;  // 變更後沒有按「保留」就自動還原
        private static DEVMODE NewDevMode() => new DEVMODE { dmSize = (short)Marshal.SizeOf(typeof(DEVMODE)) };
        private static bool TryGetCurrentMode(string device, out DEVMODE mode)
        {
            mode = NewDevMode();
            return EnumDisplaySettings(device, -1, ref mode);  // -1：ENUM_CURRENT_SETTINGS
        }  // 目前的顯示模式
        private static List<int> SupportedRefreshRates(string device)
        {
            // 與目前解析度、色彩深度、交錯方式相同的模式中，所有可用的更新頻率
            var rates = new SortedSet<int>();
            if (!TryGetCurrentMode(device, out DEVMODE current)) return rates.ToList();
            DEVMODE mode = NewDevMode();
            for (int i = 0; EnumDisplaySettings(device, i, ref mode); i++)
            {
                if (mode.dmPelsWidth == current.dmPelsWidth && mode.dmPelsHeight == current.dmPelsHeight && mode.dmBitsPerPel == current.dmBitsPerPel &&
                    (mode.dmDisplayFlags & DM_INTERLACED) == (current.dmDisplayFlags & DM_INTERLACED) && mode.dmDisplayFrequency > 1)
                {
                    rates.Add(mode.dmDisplayFrequency);
                }
                mode = NewDevMode();
            }
            return rates.ToList();
        }  // 可用的更新頻率
        private void FillRefreshItems(ToolStripItemCollection items)
        {
            items.Clear();
            string device = SelectedScreenDevice;
            if (device == null || !TryGetCurrentMode(device, out DEVMODE current))
            {
                items.Add(new ToolStripMenuItem("無法讀取目前螢幕") { Enabled = false });
                return;
            }
            items.Add(new ToolStripMenuItem($"{ScreenLabel(device)}　{current.dmPelsWidth} x {current.dmPelsHeight}") { Enabled = false });
            foreach (int hz in SupportedRefreshRates(device))
            {
                var item = new ToolStripMenuItem($"{hz} Hz") { Checked = hz == current.dmDisplayFrequency };
                item.Click += (s, e) => ChangeRefreshRate(device, hz);
                items.Add(item);
            }
        }  // 填入更新頻率選項
        private void ChangeRefreshRate(string device, int hz)
        {
            if (!TryGetCurrentMode(device, out DEVMODE old) || old.dmDisplayFrequency == hz) return;
            ChangeDisplayMode(device, old.dmPelsWidth, old.dmPelsHeight, hz, old.dmDisplayOrientation, $"{hz} Hz", "更新頻率");
        }  // 變更更新頻率（可還原）
        private void ChangeDisplayMode(string device, int width, int height, int hz, int orientation, string description, string caption)
        {
            // 解析度、更新頻率、旋轉共用：套用後倒數確認，沒按「保留」就還原成原本的模式
            if (!TryGetCurrentMode(device, out DEVMODE old)) return;
            int result = SetDisplayMode(device, width, height, hz, orientation);
            if (result != 0)
            {
                MessageBox.Show($"無法把 {ScreenLabel(device)} 改成 {description}：{DisplayChangeError(result)}", caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            UpdateRefreshRate();
            if (ConfirmDisplayChange($"{ScreenLabel(device)} 已改成 {description}"))
            {
                ssrStatus_lbl.Text = $"{caption}：{ScreenLabel(device)} {description}";
                return;
            }
            SetDisplayMode(device, old.dmPelsWidth, old.dmPelsHeight, old.dmDisplayFrequency, old.dmDisplayOrientation);
            UpdateRefreshRate();
            ssrStatus_lbl.Text = $"{caption}已還原（{old.dmPelsWidth} x {old.dmPelsHeight} @ {old.dmDisplayFrequency} Hz）";
        }  // 變更顯示模式（可還原）
        private static int SetDisplayMode(string device, int width, int height, int hz, int orientation)
        {
            // 先測試再寫入（寫進登錄，重開機後維持）；hz ≤ 1 時由驅動程式選擇；回傳 0 為成功
            DEVMODE mode = NewDevMode();
            mode.dmFields = DM_PELSWIDTH | DM_PELSHEIGHT | DM_DISPLAYORIENTATION | (hz > 1 ? DM_DISPLAYFREQUENCY : 0);
            mode.dmPelsWidth = width;
            mode.dmPelsHeight = height;
            mode.dmDisplayOrientation = orientation;
            mode.dmDisplayFrequency = hz;
            int result = ChangeDisplaySettingsEx(device, ref mode, IntPtr.Zero, CDS_TEST, IntPtr.Zero);
            return result != 0 ? result : ChangeDisplaySettingsEx(device, ref mode, IntPtr.Zero, CDS_UPDATEREGISTRY, IntPtr.Zero);
        }  // 設定顯示模式
        // 解析度與旋轉--------------------------------------------------------------------------------------------------
        private const int DM_PELSWIDTH = 0x80000, DM_PELSHEIGHT = 0x100000, DM_DISPLAYORIENTATION = 0x80;
        private static readonly string[] RotationTexts = { "橫向（0°）", "直向（90°）", "橫向翻轉（180°）", "直向翻轉（270°）" };  // DMDO_DEFAULT / 90 / 180 / 270
        private static List<(int width, int height, List<int> rates)> SupportedResolutions(string device)
        {
            // 與目前色彩深度相同的所有解析度（大到小）與各自可用的更新頻率
            var modes = new Dictionary<(int, int), SortedSet<int>>();
            if (!TryGetCurrentMode(device, out DEVMODE current)) return new List<(int, int, List<int>)>();
            DEVMODE mode = NewDevMode();
            for (int i = 0; EnumDisplaySettings(device, i, ref mode); i++)
            {
                if (mode.dmBitsPerPel == current.dmBitsPerPel && (mode.dmDisplayFlags & DM_INTERLACED) == 0)
                {
                    var key = (mode.dmPelsWidth, mode.dmPelsHeight);
                    if (!modes.TryGetValue(key, out var rates)) modes[key] = rates = new SortedSet<int>();
                    if (mode.dmDisplayFrequency > 1) rates.Add(mode.dmDisplayFrequency);
                }
                mode = NewDevMode();
            }
            // 旋轉成直向時，有些驅動程式仍以橫向列出：比對不到目前解析度就把寬高對調
            bool swap = !modes.ContainsKey((current.dmPelsWidth, current.dmPelsHeight)) && modes.ContainsKey((current.dmPelsHeight, current.dmPelsWidth));
            return modes.Select(m => swap ? (m.Key.Item2, m.Key.Item1, m.Value.ToList()) : (m.Key.Item1, m.Key.Item2, m.Value.ToList()))
                .OrderByDescending(m => m.Item1 * m.Item2).ThenByDescending(m => m.Item1).ToList();
        }  // 可用的解析度
        private void FillResolutionItems(ToolStripItemCollection items)
        {
            items.Clear();
            string device = SelectedScreenDevice;
            if (device == null || !TryGetCurrentMode(device, out DEVMODE current))
            {
                items.Add(new ToolStripMenuItem("無法讀取目前螢幕") { Enabled = false });
                return;
            }
            items.Add(new ToolStripMenuItem(ScreenLabel(device)) { Enabled = false });
            foreach (var (width, height, rates) in SupportedResolutions(device))
            {
                // 換解析度時盡量維持目前的更新頻率，不支援就交給驅動程式選
                int hz = rates.Contains(current.dmDisplayFrequency) ? current.dmDisplayFrequency : 0;
                var item = new ToolStripMenuItem($"{width} x {height}") { Checked = width == current.dmPelsWidth && height == current.dmPelsHeight, ToolTipText = $"可用 {string.Join("、", rates)} Hz" };
                item.Click += (s, e) => ChangeDisplayMode(device, width, height, hz, current.dmDisplayOrientation, $"{width} x {height}", "解析度");
                items.Add(item);
            }
        }  // 填入解析度選項
        private void FillRotationItems(ToolStripItemCollection items)
        {
            items.Clear();
            string device = SelectedScreenDevice;
            if (device == null || !TryGetCurrentMode(device, out DEVMODE current))
            {
                items.Add(new ToolStripMenuItem("無法讀取目前螢幕") { Enabled = false });
                return;
            }
            for (int orientation = 0; orientation < 4; orientation++)
            {
                int target = orientation;
                // 橫向與直向互換時寬高對調
                bool swap = target % 2 != current.dmDisplayOrientation % 2;
                int width = swap ? current.dmPelsHeight : current.dmPelsWidth, height = swap ? current.dmPelsWidth : current.dmPelsHeight;
                var item = new ToolStripMenuItem(RotationTexts[target]) { Checked = target == current.dmDisplayOrientation };
                item.Click += (s, e) => ChangeDisplayMode(device, width, height, current.dmDisplayFrequency, target, RotationTexts[target], "旋轉");
                items.Add(item);
            }
        }  // 填入旋轉選項
        private static string DisplayChangeError(int code)
        {
            switch (code)
            {
                case 1: return "需要重新開機才會生效";
                case -1: return "顯示卡驅動程式拒絕這個設定";
                case -2: return "螢幕不支援這個模式";
                case -3: return "無法寫入登錄";
                default: return $"錯誤碼 {code}";
            }
        }  // 變更失敗原因
        private bool ConfirmDisplayChange(string message, int seconds = DisplayChangeSeconds)
        {
            // 像 Windows 一樣倒數：畫面變黑看不到時不用操作，時間到自動還原
            using (var timer = new Timer { Interval = 1000 })
            using (var form = new Form
            {
                Text = "顯示器", FormBorderStyle = FormBorderStyle.FixedDialog, StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false, MaximizeBox = false, ShowInTaskbar = false, TopMost = true, ClientSize = new Size(360, 104), Font = Font,
            })
            {
                var label = new Label { Bounds = new Rectangle(12, 12, 336, 48) };
                var keep = new Button { Text = "保留變更", DialogResult = DialogResult.OK, Bounds = new Rectangle(180, 68, 80, 26) };
                var revert = new Button { Text = "還原", DialogResult = DialogResult.Cancel, Bounds = new Rectangle(268, 68, 80, 26) };
                form.Controls.AddRange(new Control[] { label, keep, revert });
                form.AcceptButton = keep;
                form.CancelButton = revert;
                int left = seconds;
                void ShowLeft() => label.Text = $"{message}\r\n畫面正常嗎？{left} 秒後自動還原。";
                ShowLeft();
                timer.Tick += (s, e) =>
                {
                    if (--left <= 0) form.DialogResult = DialogResult.Cancel;
                    else ShowLeft();
                };
                form.Shown += (s, e) => timer.Start();
                Localize(form);
                return form.ShowDialog(this) == DialogResult.OK;
            }
        }  // 確認是否保留顯示設定
        // 縮放比例----------------------------------------------------------------------------------------------------
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT { public int Left, Top, Right, Bottom; }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct MONITORINFOEX
        {
            public int cbSize;
            public RECT rcMonitor, rcWork;
            public int dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string szDevice;
        }
        private delegate bool MonitorEnumProc(IntPtr monitor, IntPtr hdc, ref RECT rect, IntPtr data);
        [DllImport("user32.dll")]
        private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr clip, MonitorEnumProc callback, IntPtr data);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool GetMonitorInfo(IntPtr monitor, ref MONITORINFOEX info);
        [DllImport("user32.dll")]
        private static extern IntPtr SetThreadDpiAwarenessContext(IntPtr context);
        [DllImport("shcore.dll")]
        private static extern int GetDpiForMonitor(IntPtr monitor, int dpiType, out uint dpiX, out uint dpiY);
        private static IntPtr EnterPerMonitorDpi()
        {
            // 程式本身沒有宣告 DPI 感知：暫時把這個執行緒切成個別螢幕感知，回傳原本的設定（失敗時為 Zero）
            try
            {
                IntPtr previous = SetThreadDpiAwarenessContext(new IntPtr(-4));  // PER_MONITOR_AWARE_V2（Windows 10 1703 起）
                return previous != IntPtr.Zero ? previous : SetThreadDpiAwarenessContext(new IntPtr(-3));  // PER_MONITOR_AWARE（1607 起）
            }
            catch (EntryPointNotFoundException)
            {
                return IntPtr.Zero;  // 舊版 Windows
            }
        }  // 切成個別螢幕 DPI 感知
        private sealed class DpiScope : IDisposable
        {
            private readonly IntPtr _previous;
            public DpiScope(bool enable) { _previous = enable ? EnterPerMonitorDpi() : IntPtr.Zero; }
            public bool Active => _previous != IntPtr.Zero;
            public void Dispose() { if (_previous != IntPtr.Zero) SetThreadDpiAwarenessContext(_previous); }
        }  // 範圍內為個別螢幕 DPI 感知
        private sealed class MainDpiContext : IDisposable
        {
            private readonly IntPtr _previous;
            public MainDpiContext(IntPtr context)
            {
                if (context == IntPtr.Zero) return;
                try
                {
                    _previous = SetThreadDpiAwarenessContext(context);
                }
                catch (EntryPointNotFoundException)
                {
                    // 舊版 Windows 沒有混合 DPI，不用切換
                }
            }
            public void Dispose() { if (_previous != IntPtr.Zero) SetThreadDpiAwarenessContext(_previous); }
        }
        public static IDisposable MainDpiScope() => new MainDpiContext(new IntPtr(-1));  // DPI_AWARENESS_CONTEXT_UNAWARE：與主視窗相同
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDpiAwarenessContext(IntPtr hwnd);
        public static IDisposable WindowDpiScope(Control control)
        {
            // 執行緒切到某個視窗自己的 DPI 模式：從其他模式（主視窗、計時器）呼叫 Invalidate 等傳座標的 API 時，座標才不會被 Windows 換算
            IntPtr context = IntPtr.Zero;
            try
            {
                if (control != null && control.IsHandleCreated) context = GetWindowDpiAwarenessContext(control.Handle);
            }
            catch (EntryPointNotFoundException)
            {
                // 舊版 Windows
            }
            return new MainDpiContext(context);
        }
        // 從全螢幕視窗（個別螢幕 DPI 感知）呼叫主視窗時用：Windows 處理全螢幕視窗的訊息期間，執行緒是全螢幕視窗的 DPI 模式，
        // 這時直接操作主視窗（換螢幕、選清單、重新產生）會讓主視窗在錯誤的 DPI 下排版（例如 150% 螢幕上分頁被縮成 2/3），
        // 所以先切回主視窗的模式（無感知）再執行
        // 自動調整縮放：全螢幕視窗改成個別螢幕 DPI 感知、寬高用實際解析度，縮放不是 100% 也能 1:1 輸出
        public static bool AutoDpiScaling { get; private set; } = true;
        public static IDisposable FullScreenDpiScope() => new DpiScope(AutoDpiScaling);  // 建立、擺放全螢幕視窗時使用
        private static Rectangle? PhysicalMonitorBounds(string device)
        {
            // 螢幕的實際像素範圍（虛擬桌面座標）
            using (var scope = new DpiScope(true))
            {
                if (!scope.Active) return null;
                Rectangle? bounds = null;
                EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (IntPtr monitor, IntPtr hdc, ref RECT rect, IntPtr data) =>
                {
                    var info = new MONITORINFOEX { cbSize = Marshal.SizeOf(typeof(MONITORINFOEX)) };
                    if (GetMonitorInfo(monitor, ref info) && info.szDevice == device)
                    {
                        bounds = Rectangle.FromLTRB(info.rcMonitor.Left, info.rcMonitor.Top, info.rcMonitor.Right, info.rcMonitor.Bottom);
                    }
                    return true;
                }, IntPtr.Zero);
                return bounds;
            }
        }  // 螢幕實際像素範圍
        public static Rectangle FullScreenBounds(Screen screen) =>
            AutoDpiScaling ? PhysicalMonitorBounds(screen.DeviceName) ?? screen.Bounds : screen.Bounds;  // 全螢幕視窗的位置（座標配合 FullScreenDpiScope）
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hwnd, IntPtr insertAfter, int x, int y, int cx, int cy, uint flags);
        private const uint SWP_NOZORDER = 0x4, SWP_NOACTIVATE = 0x10;
        public static void PlaceFullScreenWindow(Form window, Screen screen)
        {
            // 把全螢幕視窗擺到螢幕上（還沒最大化）；個別螢幕 DPI 範圍只包住設定位置的系統呼叫，
            // Show、最大化與它們觸發的事件都在範圍外執行，避免主視窗的排版在錯誤的 DPI 下被縮放
            Rectangle target = FullScreenBounds(screen);
            if (window.WindowState != FormWindowState.Normal) window.WindowState = FormWindowState.Normal;
            window.FormBorderStyle = FormBorderStyle.None;
            window.StartPosition = FormStartPosition.Manual;
            if (!window.IsHandleCreated || !AutoDpiScaling)
            {
                window.Bounds = target;  // 還沒建立：建立視窗時（CreateHandle 內為個別螢幕 DPI）直接用這個位置
                return;
            }
            using (FullScreenDpiScope())
            {
                SetWindowPos(window.Handle, IntPtr.Zero, target.X, target.Y, target.Width, target.Height, SWP_NOZORDER | SWP_NOACTIVATE);
            }
        }  // 擺放全螢幕視窗
        public static Size ScreenPixelSize(Screen screen)
        {
            // 圖片要產生的寬高：自動調整時用實際解析度
            Size physical = AutoDpiScaling ? PhysicalResolution(screen.DeviceName) : Size.Empty;
            return physical.IsEmpty ? screen.Bounds.Size : physical;
        }  // 螢幕的圖片寬高
        private void SetAutoDpiScaling(bool enable)
        {
            if (AutoDpiScaling == enable) return;
            AutoDpiScaling = enable;
            CloseAllFullScreen();  // 已開的全螢幕視窗 DPI 感知在建立時決定，關掉後下次重新建立
            menuToolStripCmb_SelectedIndexChanged(mnsScreenlist_cmb, EventArgs.Empty);  // 寬高改成對應的解析度並重新產生
            _lastScaleWarning = null;
            UpdateRefreshRate();
            if (_lastScaleWarning == null) ssrStatus_lbl.Text = $"自動調整縮放：{(enable ? "開啟" : "關閉")}";
        }  // 切換自動調整縮放
        public static int MonitorScalePercent(string device)
        {
            // Windows「縮放」比例（100、125、150…）；取不到時為 0
            // 讀取時暫時把這個執行緒切成個別螢幕感知，才拿得到真正的 DPI
            IntPtr previous = EnterPerMonitorDpi();
            try
            {
                int percent = 0;
                EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (IntPtr monitor, IntPtr hdc, ref RECT rect, IntPtr data) =>
                {
                    var info = new MONITORINFOEX { cbSize = Marshal.SizeOf(typeof(MONITORINFOEX)) };
                    if (GetMonitorInfo(monitor, ref info) && info.szDevice == device && GetDpiForMonitor(monitor, 0, out uint dpi, out _) == 0)
                    {
                        percent = (int)Math.Round(dpi * 100 / 96.0);  // 0：MDT_EFFECTIVE_DPI
                    }
                    return true;
                }, IntPtr.Zero);
                if (previous != IntPtr.Zero && percent > 0) return percent;
            }
            catch (DllNotFoundException)
            {
                // Windows 8 以前沒有 shcore.dll
            }
            finally
            {
                if (previous != IntPtr.Zero) SetThreadDpiAwarenessContext(previous);
            }
            // 推算：實際解析度 ÷ 程式看到的寬度
            Screen screen = Screen.AllScreens.FirstOrDefault(s => s.DeviceName == device);
            if (screen == null || !TryGetCurrentMode(device, out DEVMODE mode) || screen.Bounds.Width == 0) return 0;
            return (int)Math.Round(mode.dmPelsWidth * 100.0 / screen.Bounds.Width);
        }  // 螢幕縮放比例
        public static Size PhysicalResolution(string device) =>
            TryGetCurrentMode(device, out DEVMODE mode) ? new Size(mode.dmPelsWidth, mode.dmPelsHeight) : Size.Empty;  // 實際解析度
        public static int TextScalePercent()
        {
            // Windows「協助工具 > 文字大小」（100~225），沒設定過為 100
            try
            {
                return Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Accessibility", "TextScaleFactor", 100) is int value ? value : 100;
            }
            catch (Exception)
            {
                return 100;
            }
        }  // 文字大小比例
        private void UpdateScaleIndicator(string device, int hz)
        {
            // 工具列更新率旁邊顯示會影響圖樣的狀態：縮放（紅色，全螢幕圖樣會被放大）；
            // HDR、夜燈、非預設色彩描述檔會改變輸出顏色、文字大小只影響介面文字（橘色）
            int scale = MonitorScalePercent(device), text = TextScalePercent();
            DisplayTargetInfo target = DisplayTarget(device);
            bool? nightLight = NightLightOn();
            string profile = ColorProfileName(device);
            bool scaled = scale > 0 && scale != 100, textScaled = text != 100, hdr = target?.HdrEnabled == true, night = nightLight == true, icc = !IsDefaultColorProfile(profile);
            var notes = new List<string>();
            if (scaled) notes.Add(English ? $"Scale {scale}%" : $"縮放 {scale}%");
            if (hdr) notes.Add("HDR");
            if (night) notes.Add(English ? "Night light" : "夜燈");
            if (icc) notes.Add("ICC");
            if (textScaled) notes.Add(English ? $"Text {text}%" : $"文字 {text}%");
            mnsRefresh_lbl.Text = (hz > 1 ? $"{hz} Hz" : "? Hz") + (notes.Count > 0 ? "｜" + string.Join(" ", notes) : "");
            mnsRefresh_lbl.ForeColor = scaled ? Color.Red : hdr || night || icc || textScaled ? Color.DarkOrange : SystemColors.ControlText;
            Size physical = PhysicalResolution(device);
            string scaleNote = !scaled ? "" : AutoDpiScaling
                ? $"：已自動調整，以實際解析度 {physical.Width} x {physical.Height} 1:1 輸出"
                : "：全螢幕圖樣會被 Windows 放大，1px 圖樣不準確（可勾選「顯示器 > 自動調整縮放」）";
            mnsRefresh_lbl.ToolTipText = $"{ScreenLabel(device)}{(target?.FriendlyName != null ? $"（{target.FriendlyName}）" : "")} 更新率 {(hz > 1 ? $"{hz} Hz" : "未知")}" +
                $"{(target?.ExactHz > 0 ? $"（精確 {target.ExactHz:0.000} Hz）" : "")}（點一下切換）\r\n" +
                $"縮放 {(scale > 0 ? $"{scale}%" : "未知")}{scaleNote}\r\n" +
                $"{target?.ColorText ?? "色彩：未知"}{(hdr ? "：HDR 開啟時 SDR 圖樣的亮度、顏色會被轉換" : "")}\r\n" +
                $"夜燈 {(nightLight == null ? "未知" : night ? "開啟：畫面偏暖，顏色不準" : "關閉")}\r\n" +
                $"色彩描述檔 {profile ?? "未知"}{(icc ? "：非 Windows 預設 sRGB，色彩管理的程式顏色會被調整" : "")}\r\n" +
                $"文字大小 {text}%{(textScaled ? "：只影響介面文字，不影響圖樣" : "")}";
            if (English)
            {
                string scaleNoteEn = !scaled ? "" : AutoDpiScaling
                    ? $": auto adjusted, 1:1 at the native {physical.Width} x {physical.Height}"
                    : ": full screen is upscaled by Windows, 1px patterns are inaccurate (enable Display > Auto DPI adjust)";
                mnsRefresh_lbl.ToolTipText = $"{ScreenLabel(device)}{(target?.FriendlyName != null ? $" ({target.FriendlyName})" : "")} refresh {(hz > 1 ? $"{hz} Hz" : "unknown")}" +
                    $"{(target?.ExactHz > 0 ? $" (exact {target.ExactHz:0.000} Hz)" : "")} (click to change)\r\n" +
                    $"Scale {(scale > 0 ? $"{scale}%" : "unknown")}{scaleNoteEn}\r\n" +
                    $"{target?.ColorText ?? "Color: unknown"}{(hdr ? ": SDR patterns are tone-mapped while HDR is on" : "")}\r\n" +
                    $"Night light {(nightLight == null ? "unknown" : night ? "on: warmer image, colors are off" : "off")}\r\n" +
                    $"Color profile {profile ?? "unknown"}{(icc ? ": not the Windows default sRGB" : "")}\r\n" +
                    $"Text size {text}%{(textScaled ? ": affects UI text only" : "")}";
            }
            // 狀態列只在螢幕或比例改變時提醒一次（視窗每次取得焦點都會更新提示）
            string warning = scaled ? $"注意：{ScreenLabel(device)} 的縮放為 {scale}%{scaleNote}" : null;
            if (warning != null && warning != _lastScaleWarning) ssrStatus_lbl.Text = warning;
            _lastScaleWarning = warning;
        }  // 更新縮放提示
        private string _lastScaleWarning;
        private void ShowScaleReport()
        {
            // 列出每個螢幕的型號、介面、解析度、更新率、縮放、HDR / 色深與色彩描述檔
            var lines = new List<string>();
            bool anyScaled = false, anyColor = false;
            List<DisplayTargetInfo> targets = QueryDisplayTargets();
            foreach (Screen screen in Screen.AllScreens.OrderBy(s => s.DeviceName))
            {
                int scale = MonitorScalePercent(screen.DeviceName), hz = GetRefreshRate(screen.DeviceName);
                Size physical = PhysicalResolution(screen.DeviceName);
                DisplayTargetInfo target = targets.FirstOrDefault(t => t.Device == screen.DeviceName);
                string profile = ColorProfileName(screen.DeviceName);
                bool scaled = scale > 0 && scale != 100;
                anyScaled |= scaled;
                anyColor |= target?.HdrEnabled == true || !IsDefaultColorProfile(profile);
                lines.Add($"{ScreenLabel(screen.DeviceName)}{(screen.Primary ? "（主螢幕）" : "")}{(target?.FriendlyName != null ? $"　{target.FriendlyName}" : "")}{(target?.Connector != null ? $"　{target.Connector}" : "")}");
                lines.Add($"　解析度 {physical.Width} x {physical.Height}{(hz > 1 ? $"，{hz} Hz" : "")}{(target?.ExactHz > 0 ? $"（精確 {target.ExactHz:0.000} Hz）" : "")}");
                lines.Add(!scaled ? $"　縮放 {(scale > 0 ? $"{scale}%" : "未知")}（圖樣 1:1）"
                    : AutoDpiScaling ? $"　縮放 {scale}%（程式看到 {screen.Bounds.Width} x {screen.Bounds.Height}，已自動調整為 {physical.Width} x {physical.Height} 1:1 輸出）"
                    : $"　縮放 {scale}% ⚠ 程式看到 {screen.Bounds.Width} x {screen.Bounds.Height}，全螢幕圖樣會被放大成 {physical.Width} x {physical.Height}（像素不準確）");
                lines.Add($"　{target?.ColorText ?? "色彩：未知"}{(target?.HdrEnabled == true ? " ⚠ SDR 圖樣的亮度、顏色會被轉換" : "")}");
                lines.Add($"　色彩描述檔 {profile ?? "未知"}{(!IsDefaultColorProfile(profile) ? " ⚠ 非預設 sRGB" : "")}");
            }
            int text = TextScalePercent();
            bool? night = NightLightOn();
            lines.Add("");
            lines.Add($"夜燈：{(night == null ? "未知" : night == true ? "開啟 ⚠ 畫面偏暖，顏色不準" : "關閉")}");
            lines.Add($"文字大小：{text}%{(text != 100 ? "（只影響介面文字，不影響圖樣）" : "")}");
            lines.Add($"自動調整縮放：{(AutoDpiScaling ? "開啟" : "關閉")}　全螢幕時防止螢幕關閉：{(KeepAwakeEnabled ? "開啟" : "關閉")}");
            if (anyScaled && !AutoDpiScaling) lines.Add("\r\n要讓圖樣 1:1，請勾選「顯示器 > 自動調整縮放」，或把 Windows 縮放改成 100%。");
            bool warn = anyScaled && !AutoDpiScaling || anyColor || night == true;
            MessageBox.Show(string.Join("\r\n", lines), "螢幕資訊", MessageBoxButtons.OK, warn ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
        }  // 螢幕資訊報告
        // 投影模式（Win+P）-------------------------------------------------------------------------------------------
        [DllImport("user32.dll")]
        private static extern int GetDisplayConfigBufferSizes(uint flags, out uint pathCount, out uint modeCount);
        [DllImport("user32.dll")]
        private static extern int QueryDisplayConfig(uint flags, ref uint pathCount, IntPtr paths, ref uint modeCount, IntPtr modes, out int topology);
        [DllImport("user32.dll")]
        private static extern int SetDisplayConfig(uint pathCount, IntPtr paths, uint modeCount, IntPtr modes, uint flags);
        private const uint QDC_DATABASE_CURRENT = 0x4, SDC_APPLY = 0x80;
        private const int PathInfoSize = 72, ModeInfoSize = 64;  // DISPLAYCONFIG_PATH_INFO、DISPLAYCONFIG_MODE_INFO 的大小
        private static readonly (int topology, string text)[] ProjectionModes =
        {
            (1, "僅電腦螢幕"), (2, "複製"), (4, "延伸"), (8, "僅第二螢幕"),  // DISPLAYCONFIG_TOPOLOGY_INTERNAL / CLONE / EXTEND / EXTERNAL
        };
        public static int CurrentProjection()
        {
            // 目前的投影模式（1 僅電腦螢幕、2 複製、4 延伸、8 僅第二螢幕）；取不到時為 0
            if (GetDisplayConfigBufferSizes(QDC_DATABASE_CURRENT, out uint pathCount, out uint modeCount) != 0) return 0;
            IntPtr paths = Marshal.AllocHGlobal((int)Math.Max(1, pathCount) * PathInfoSize);
            IntPtr modes = Marshal.AllocHGlobal((int)Math.Max(1, modeCount) * ModeInfoSize);
            try
            {
                return QueryDisplayConfig(QDC_DATABASE_CURRENT, ref pathCount, paths, ref modeCount, modes, out int topology) == 0 ? topology : 0;
            }
            finally
            {
                Marshal.FreeHGlobal(paths);
                Marshal.FreeHGlobal(modes);
            }
        }  // 目前的投影模式
        private static string ProjectionText(int topology) => ProjectionModes.FirstOrDefault(m => m.topology == topology).text ?? "未知";
        private void FillProjectionItems(ToolStripItemCollection items)
        {
            items.Clear();
            int current = CurrentProjection();
            foreach (var (topology, text) in ProjectionModes)
            {
                var item = new ToolStripMenuItem(text) { Checked = topology == current };
                item.Click += (s, e) => ChangeProjection(topology);
                items.Add(item);
            }
        }  // 填入投影模式選項
        private void ChangeProjection(int topology)
        {
            int old = CurrentProjection();
            if (old == topology) return;
            int result = SetDisplayConfig(0, IntPtr.Zero, 0, IntPtr.Zero, SDC_APPLY | (uint)topology);
            if (result != 0)
            {
                MessageBox.Show($"無法切換成「{ProjectionText(topology)}」（錯誤碼 {result}）\r\n可能只連接一台螢幕，或顯示卡不支援。", "投影模式", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (ConfirmDisplayChange($"投影模式已切換成「{ProjectionText(topology)}」"))
            {
                ssrStatus_lbl.Text = $"投影模式：{ProjectionText(topology)}";
                return;
            }
            if (old != 0) SetDisplayConfig(0, IntPtr.Zero, 0, IntPtr.Zero, SDC_APPLY | (uint)old);
            ssrStatus_lbl.Text = $"投影模式已還原為「{ProjectionText(old)}」";
        }  // 切換投影模式（可還原）
    }
}
