using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Thunder
{
    // 螢幕的連接資訊（DisplayConfig）：名稱、介面、精確更新率、HDR、色深
    public class DisplayTargetInfo
    {
        public string Device;          // \\.\DISPLAY1
        public string FriendlyName;    // 螢幕型號（EDID）
        public string Connector;       // HDMI、DisplayPort、內建…
        public double ExactHz;         // 精確更新率，例如 59.940
        public bool HdrSupported, HdrEnabled;
        public int BitsPerChannel;     // 每色 bit 數（0 為未知）
        public string ColorEncoding;   // RGB、YCbCr444…

        public string ColorText => mainWindow.English
            ? $"{(HdrEnabled ? "HDR on" : HdrSupported ? "HDR off (supported)" : "SDR")}{(BitsPerChannel > 0 ? $", {BitsPerChannel} bit" : "")}{(ColorEncoding != null ? $", {ColorEncoding}" : "")}"
            : $"{(HdrEnabled ? "HDR 開" : HdrSupported ? "HDR 關（支援）" : "SDR")}{(BitsPerChannel > 0 ? $"，{BitsPerChannel} bit" : "")}{(ColorEncoding != null ? $"，{ColorEncoding}" : "")}";
    }

    // 實際更新率量測結果
    public class RefreshResult
    {
        public bool Ok;
        public string Error;
        public double Hz, IntervalMs, JitterMs;
        public int Frames, Dropped;

        public string Text => Ok ? $"實測 {Hz:0.00} Hz，每幀 {IntervalMs:0.000} ms（抖動 ±{JitterMs:0.000} ms），{Frames} 幀，掉幀 {Dropped}" : $"無法量測：{Error}";
        public string ShortText => Ok ? $"實測 {Hz:0.00} Hz ±{JitterMs:0.00} ms{(Dropped > 0 ? $" 掉幀 {Dropped}" : "")}" : "實測：無法量測";
    }

    // 以顯示卡的垂直同步事件量測螢幕實際的更新率（不需要 DirectX）
    public static class RefreshMeter
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct D3DKMT_OPENADAPTERFROMHDC { public IntPtr hDc; public uint hAdapter; public uint AdapterLuidLow; public int AdapterLuidHigh; public uint VidPnSourceId; }
        [StructLayout(LayoutKind.Sequential)]
        private struct D3DKMT_WAITFORVERTICALBLANKEVENT { public uint hAdapter; public uint hDevice; public uint VidPnSourceId; }
        [StructLayout(LayoutKind.Sequential)]
        private struct D3DKMT_CLOSEADAPTER { public uint hAdapter; }
        [DllImport("gdi32.dll")] private static extern int D3DKMTOpenAdapterFromHdc(ref D3DKMT_OPENADAPTERFROMHDC data);
        [DllImport("gdi32.dll")] private static extern int D3DKMTWaitForVerticalBlankEvent(ref D3DKMT_WAITFORVERTICALBLANKEVENT data);
        [DllImport("gdi32.dll")] private static extern int D3DKMTCloseAdapter(ref D3DKMT_CLOSEADAPTER data);
        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)] private static extern IntPtr CreateDC(string driver, string device, string output, IntPtr initData);
        [DllImport("gdi32.dll")] private static extern bool DeleteDC(IntPtr hdc);

        public static RefreshResult Measure(string device, int frames, CancellationToken token)
        {
            // 連續等待 frames 次垂直同步，依間隔算出更新率、抖動與掉幀（會阻塞，請在背景執行緒呼叫）
            IntPtr hdc = CreateDC(null, device, null, IntPtr.Zero);
            if (hdc == IntPtr.Zero) return new RefreshResult { Error = "無法開啟螢幕" };
            var open = new D3DKMT_OPENADAPTERFROMHDC { hDc = hdc };
            int status = D3DKMTOpenAdapterFromHdc(ref open);
            DeleteDC(hdc);
            if (status != 0) return new RefreshResult { Error = $"無法開啟顯示卡（0x{status:X8}）" };
            var close = new D3DKMT_CLOSEADAPTER { hAdapter = open.hAdapter };
            try
            {
                var wait = new D3DKMT_WAITFORVERTICALBLANKEVENT { hAdapter = open.hAdapter, VidPnSourceId = open.VidPnSourceId };
                if ((status = D3DKMTWaitForVerticalBlankEvent(ref wait)) != 0) return new RefreshResult { Error = $"無法等待垂直同步（0x{status:X8}）" };
                var intervals = new List<double>(frames);
                var watch = Stopwatch.StartNew();
                long last = watch.ElapsedTicks;
                while (intervals.Count < frames && !token.IsCancellationRequested)
                {
                    if ((status = D3DKMTWaitForVerticalBlankEvent(ref wait)) != 0) return new RefreshResult { Error = $"垂直同步中斷（0x{status:X8}）" };
                    long now = watch.ElapsedTicks;
                    intervals.Add((now - last) * 1000.0 / Stopwatch.Frequency);
                    last = now;
                }
                if (intervals.Count < 2) return new RefreshResult { Error = "量測時間太短" };
                // 間隔超過中位數 1.5 倍視為漏掉（掉幀）；平均間隔以「總時間 ÷ 實際經過的幀數」計算
                double median = intervals.OrderBy(v => v).ElementAt(intervals.Count / 2);
                int dropped = intervals.Sum(v => v > median * 1.5 ? (int)Math.Round(v / median) - 1 : 0);
                double mean = intervals.Sum() / (intervals.Count + dropped);
                var normal = intervals.Where(v => v <= median * 1.5).ToList();
                double jitter = Math.Sqrt(normal.Sum(v => (v - normal.Average()) * (v - normal.Average())) / normal.Count);
                return new RefreshResult { Ok = true, Hz = 1000 / mean, IntervalMs = mean, JitterMs = jitter, Frames = intervals.Count, Dropped = dropped };
            }
            finally
            {
                D3DKMTCloseAdapter(ref close);
            }
        }  // 量測更新率
        public static async Task<RefreshResult> MeasureAsync(string device, int frames, int timeoutMs, CancellationToken token)
        {
            // 螢幕關閉時垂直同步可能永遠等不到：逾時就放棄（背景執行緒不影響結束程式）
            var measure = Task.Factory.StartNew(() => Measure(device, frames, token), token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            return await Task.WhenAny(measure, Task.Delay(timeoutMs)) == measure ? measure.Result : new RefreshResult { Error = "等不到垂直同步（螢幕可能關閉或休眠）" };
        }  // 背景量測（含逾時）
    }

    public partial class mainWindow
    {
        // DisplayConfig：每個作用中螢幕的連接資訊------------------------------------------------------------------------
        [StructLayout(LayoutKind.Sequential)]
        private struct LUID { public uint LowPart; public int HighPart; }
        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_PATH_SOURCE_INFO { public LUID adapterId; public uint id; public uint modeInfoIdx; public uint statusFlags; }
        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_PATH_TARGET_INFO
        {
            public LUID adapterId; public uint id; public uint modeInfoIdx; public uint outputTechnology; public uint rotation; public uint scaling;
            public uint refreshNumerator, refreshDenominator; public uint scanLineOrdering; public int targetAvailable; public uint statusFlags;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_PATH_INFO { public DISPLAYCONFIG_PATH_SOURCE_INFO sourceInfo; public DISPLAYCONFIG_PATH_TARGET_INFO targetInfo; public uint flags; }
        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_MODE_INFO { public uint infoType; public uint id; public LUID adapterId; [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)] public byte[] modeInfo; }
        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_DEVICE_INFO_HEADER { public uint type; public uint size; public LUID adapterId; public uint id; }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DISPLAYCONFIG_SOURCE_DEVICE_NAME { public DISPLAYCONFIG_DEVICE_INFO_HEADER header; [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string viewGdiDeviceName; }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DISPLAYCONFIG_TARGET_DEVICE_NAME
        {
            public DISPLAYCONFIG_DEVICE_INFO_HEADER header; public uint flags; public uint outputTechnology; public ushort edidManufactureId; public ushort edidProductCodeId; public uint connectorInstance;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)] public string monitorFriendlyDeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string monitorDevicePath;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO { public DISPLAYCONFIG_DEVICE_INFO_HEADER header; public uint value; public uint colorEncoding; public uint bitsPerColorChannel; }
        [DllImport("user32.dll", EntryPoint = "QueryDisplayConfig")]
        private static extern int QueryDisplayConfigPaths(uint flags, ref uint pathCount, [Out] DISPLAYCONFIG_PATH_INFO[] paths, ref uint modeCount, [Out] DISPLAYCONFIG_MODE_INFO[] modes, IntPtr topology);
        [DllImport("user32.dll")] private static extern int DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_SOURCE_DEVICE_NAME info);
        [DllImport("user32.dll")] private static extern int DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_TARGET_DEVICE_NAME info);
        [DllImport("user32.dll")] private static extern int DisplayConfigGetDeviceInfo(ref DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO info);
        private const uint QDC_ONLY_ACTIVE_PATHS = 0x2;
        public static List<DisplayTargetInfo> QueryDisplayTargets()
        {
            var result = new List<DisplayTargetInfo>();
            try
            {
                if (GetDisplayConfigBufferSizes(QDC_ONLY_ACTIVE_PATHS, out uint pathCount, out uint modeCount) != 0) return result;
                var paths = new DISPLAYCONFIG_PATH_INFO[pathCount];
                var modes = new DISPLAYCONFIG_MODE_INFO[modeCount];
                if (QueryDisplayConfigPaths(QDC_ONLY_ACTIVE_PATHS, ref pathCount, paths, ref modeCount, modes, IntPtr.Zero) != 0) return result;
                foreach (DISPLAYCONFIG_PATH_INFO path in paths.Take((int)pathCount))
                {
                    var source = new DISPLAYCONFIG_SOURCE_DEVICE_NAME();
                    source.header = new DISPLAYCONFIG_DEVICE_INFO_HEADER { type = 1, size = (uint)Marshal.SizeOf(source), adapterId = path.sourceInfo.adapterId, id = path.sourceInfo.id };
                    if (DisplayConfigGetDeviceInfo(ref source) != 0) continue;
                    var target = new DISPLAYCONFIG_TARGET_DEVICE_NAME();
                    target.header = new DISPLAYCONFIG_DEVICE_INFO_HEADER { type = 2, size = (uint)Marshal.SizeOf(target), adapterId = path.targetInfo.adapterId, id = path.targetInfo.id };
                    bool named = DisplayConfigGetDeviceInfo(ref target) == 0;
                    var color = new DISPLAYCONFIG_GET_ADVANCED_COLOR_INFO();
                    color.header = new DISPLAYCONFIG_DEVICE_INFO_HEADER { type = 9, size = (uint)Marshal.SizeOf(color), adapterId = path.targetInfo.adapterId, id = path.targetInfo.id };
                    bool colored = DisplayConfigGetDeviceInfo(ref color) == 0;
                    result.Add(new DisplayTargetInfo
                    {
                        Device = source.viewGdiDeviceName,
                        FriendlyName = named && !string.IsNullOrWhiteSpace(target.monitorFriendlyDeviceName) ? target.monitorFriendlyDeviceName : null,
                        Connector = ConnectorText(path.targetInfo.outputTechnology),
                        ExactHz = path.targetInfo.refreshDenominator > 0 ? path.targetInfo.refreshNumerator / (double)path.targetInfo.refreshDenominator : 0,
                        HdrSupported = colored && (color.value & 0x1) != 0,
                        HdrEnabled = colored && (color.value & 0x2) != 0,
                        BitsPerChannel = colored ? (int)color.bitsPerColorChannel : 0,
                        ColorEncoding = colored ? EncodingText(color.colorEncoding) : null,
                    });
                }
            }
            catch (Exception ex) when (ex is EntryPointNotFoundException || ex is DllNotFoundException)
            {
                // Windows 7 以前沒有 DisplayConfig
            }
            return result;
        }  // 所有作用中螢幕的連接資訊
        public static DisplayTargetInfo DisplayTarget(string device) => QueryDisplayTargets().FirstOrDefault(t => t.Device == device);
        private static string ConnectorText(uint technology)
        {
            switch (technology)
            {
                case 0: return "VGA";
                case 4: return "DVI";
                case 5: return "HDMI";
                case 10: return "DisplayPort";
                case 11: return "內建 DisplayPort";
                case 15: return "Miracast";
                case 16: return "內建 USB-C";
                case 0x80000000: return "內建";
                default: return $"其他（{technology}）";
            }
        }  // 連接介面
        private static string EncodingText(uint encoding)
        {
            switch (encoding)
            {
                case 0: return "RGB";
                case 1: return "YCbCr444";
                case 2: return "YCbCr422";
                case 3: return "YCbCr420";
                default: return null;
            }
        }  // 色彩編碼
        // 夜燈與色彩描述檔------------------------------------------------------------------------------------------------
        public static bool? NightLightOn()
        {
            // Windows 沒有公開 API：讀取設定 App 存的狀態（第 19 個位元組 0x15 = 開、0x13 = 關），讀不到為 null
            // 從沒開過夜燈時只有 13 bytes 的預設內容，視為關閉
            try
            {
                const string key = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\CloudStore\Store\DefaultAccount\Current\default$windows.data.bluelightreduction.bluelightreductionstate\windows.data.bluelightreduction.bluelightreductionstate";
                if (!(Registry.GetValue(key, "Data", null) is byte[] data)) return null;
                if (data.Length < 19) return false;
                return data[18] == 0x15 ? true : data[18] == 0x13 ? (bool?)false : null;
            }
            catch (Exception)
            {
                return null;
            }
        }  // 夜燈是否開啟
        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)] private static extern IntPtr CreateDC(string driver, string device, string output, IntPtr initData);
        [DllImport("gdi32.dll")] private static extern bool DeleteDC(IntPtr hdc);
        [DllImport("gdi32.dll", CharSet = CharSet.Unicode)] private static extern bool GetICMProfile(IntPtr hdc, ref uint size, StringBuilder fileName);
        public static string ColorProfileName(string device)
        {
            // 螢幕目前使用的色彩描述檔（ICC）檔名，取不到時為 null
            IntPtr hdc = CreateDC(null, device, null, IntPtr.Zero);
            if (hdc == IntPtr.Zero) return null;
            try
            {
                uint size = 260;
                var name = new StringBuilder((int)size);
                return GetICMProfile(hdc, ref size, name) ? System.IO.Path.GetFileName(name.ToString()) : null;
            }
            finally
            {
                DeleteDC(hdc);
            }
        }  // 色彩描述檔
        public static bool IsDefaultColorProfile(string profile) => profile == null || profile.StartsWith("sRGB Color Space Profile", StringComparison.OrdinalIgnoreCase);  // Windows 預設的 sRGB
        // 量測實際更新率--------------------------------------------------------------------------------------------------
        private async void MeasureSelectedRefresh()
        {
            string device = SelectedScreenDevice;
            if (device == null) return;
            int hz = GetRefreshRate(device);
            mnsDisplay_btn.Enabled = false;
            ssrStatus_lbl.Text = $"量測 {ScreenLabel(device)} 的實際更新率中…（約 2 秒）";
            try
            {
                RefreshResult result = await RefreshMeter.MeasureAsync(device, Math.Max(60, hz * 2), 10000, CancellationToken.None);
                DisplayTargetInfo target = DisplayTarget(device);
                string header = $"{ScreenLabel(device)}{(target?.FriendlyName != null ? $"（{target.FriendlyName}）" : "")}";
                string exact = target != null && target.ExactHz > 0 ? $"（精確 {target.ExactHz:0.000} Hz）" : "";
                string verdict = result.Ok && hz > 1 && Math.Abs(result.Hz - (target?.ExactHz > 0 ? target.ExactHz : hz)) > 0.5
                    ? "\r\n\r\n⚠ 實測與設定不符，螢幕可能沒有跑在設定的頻率" : "";
                ssrStatus_lbl.Text = $"{ScreenLabel(device)}：設定 {hz} Hz，{result.ShortText}";
                MessageBox.Show($"{header}\r\n設定：{hz} Hz{exact}\r\n{result.Text}{verdict}", "實際更新率",
                    MessageBoxButtons.OK, result.Ok && verdict == "" ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            }
            finally
            {
                mnsDisplay_btn.Enabled = true;
            }
        }  // 量測目前螢幕的實際更新率
        // 螢幕識別--------------------------------------------------------------------------------------------------------
        private void IdentifyScreens()
        {
            // 每個螢幕左上角顯示大字編號與資訊，3 秒後或點一下關閉
            List<DisplayTargetInfo> targets = QueryDisplayTargets();
            foreach (Screen screen in Screen.AllScreens)
            {
                DisplayTargetInfo target = targets.FirstOrDefault(t => t.Device == screen.DeviceName);
                Size physical = PhysicalResolution(screen.DeviceName);
                string number = new string(screen.DeviceName.Reverse().TakeWhile(char.IsDigit).Reverse().ToArray());
                var form = new Form
                {
                    FormBorderStyle = FormBorderStyle.None, StartPosition = FormStartPosition.Manual, ShowInTaskbar = false, TopMost = true,
                    BackColor = Color.FromArgb(30, 30, 30), ForeColor = Color.White, Bounds = new Rectangle(screen.Bounds.X + 40, screen.Bounds.Y + 40, 420, 240),
                };
                var big = new Label { Text = number, Font = new Font("Arial", 80f, FontStyle.Bold), AutoSize = false, Bounds = new Rectangle(10, 0, 160, 150), TextAlign = ContentAlignment.MiddleCenter };
                var detail = new Label
                {
                    AutoSize = false, Bounds = new Rectangle(175, 20, 240, 210), Font = new Font(Font.FontFamily, 11f),
                    Text = string.Join("\r\n", new[]
                    {
                        $"{ScreenLabel(screen.DeviceName)}{(screen.Primary ? "（主螢幕）" : "")}", target?.FriendlyName, target?.Connector,
                        $"{physical.Width} x {physical.Height} @ {GetRefreshRate(screen.DeviceName)} Hz", $"縮放 {MonitorScalePercent(screen.DeviceName)}%", target?.ColorText,
                    }.Where(s => !string.IsNullOrEmpty(s))),
                };
                form.Controls.AddRange(new Control[] { big, detail });
                foreach (Control c in new Control[] { form, big, detail }) c.Click += (s, e) => form.Close();
                var timer = new System.Windows.Forms.Timer { Interval = 3000 };
                timer.Tick += (s, e) => form.Close();
                form.FormClosed += (s, e) => { timer.Dispose(); form.Dispose(); };
                form.Show(this);
                timer.Start();
            }
        }  // 螢幕識別
        // 全螢幕時防止螢幕關閉 / 休眠---------------------------------------------------------------------------------------
        [DllImport("kernel32.dll")] private static extern uint SetThreadExecutionState(uint flags);
        private const uint ES_CONTINUOUS = 0x80000000, ES_SYSTEM_REQUIRED = 0x1, ES_DISPLAY_REQUIRED = 0x2;
        public static bool KeepAwakeEnabled { get; private set; } = true;
        private bool _keepingAwake;
        public void UpdateKeepAwake()
        {
            // 視窗關閉過程中還在 OpenForms 裡，延後到訊息處理完再判斷
            BeginInvoke((Action)(() =>
            {
                bool need = KeepAwakeEnabled && Application.OpenForms.OfType<pictureWindow>().Any(w => w.Visible && !w.IsDisposed);
                if (need == _keepingAwake) return;
                _keepingAwake = need;
                SetThreadExecutionState(need ? ES_CONTINUOUS | ES_SYSTEM_REQUIRED | ES_DISPLAY_REQUIRED : ES_CONTINUOUS);
            }));
        }  // 依全螢幕視窗開關更新防休眠
        private void SetKeepAwake(bool enable)
        {
            KeepAwakeEnabled = enable;
            UpdateKeepAwake();
            ssrStatus_lbl.Text = $"全螢幕時防止螢幕關閉：{(enable ? "開啟" : "關閉")}";
        }  // 切換防休眠
    }
}
