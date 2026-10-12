using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Thunder
{
    public class EdidWindow : Form
    {
        //全局變數-----------------------------------------------------------------------------------------------------
        private const int BlockSize = 128;
        private const int BytesPerLine = 16;
        private const int HexColumnStart = 6;  // 每行 "000:  " 之後才是 hex
        private readonly TextBox edidHex_txt;
        private readonly ToolTip toolTip = new ToolTip { InitialDelay = 300, ReshowDelay = 100 };
        private readonly Dictionary<int, int> _lineToOffset = new Dictionary<int, int>();  // hex 行號 → 該行第一個 byte 的 offset
        private byte[] _edid;
        private int _lastTipOffset = -1;
        //EdidWindow類別-----------------------------------------------------------------------------------------------
        public EdidWindow(string screenDeviceName)
        {
            Text = $"EDID - {screenDeviceName}";
            Size = new Size(560, 600);
            StartPosition = FormStartPosition.CenterParent;
            ShowIcon = false;
            KeyPreview = true;
            KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) Close(); };
            Load += (s, e) => mainWindow.Localize(this);  // 英文介面

            edidHex_txt = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                WordWrap = false,
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10F),
                BackColor = SystemColors.Window,
            };
            edidHex_txt.MouseMove += edidHex_txt_MouseMove;
            Controls.Add(edidHex_txt);

            ShowEdid(screenDeviceName);
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            // TextBox 取得焦點時會自動全選，開啟後取消反白並回到最上方
            edidHex_txt.Select(0, 0);
            edidHex_txt.ScrollToCaret();
        }
        //顯示---------------------------------------------------------------------------------------------------------
        private void ShowEdid(string screenDeviceName)
        {
            var sb = new StringBuilder();
            List<EdidReader.MonitorEdid> monitors = EdidReader.Read(screenDeviceName);
            if (monitors.Count == 0)
            {
                sb.AppendLine($"找不到 {screenDeviceName} 的 EDID。");
                sb.AppendLine("可能是虛擬螢幕、遠端桌面，或驅動程式沒有提供 EDID。");
                edidHex_txt.Text = sb.ToString();
                return;
            }
            // 一個輸出通常只接一台螢幕；若有多台（例如鏡像）依序列出，tooltip 以第一台為準
            _edid = monitors[0].Edid;
            foreach (EdidReader.MonitorEdid monitor in monitors)
            {
                AppendMonitor(sb, monitor, monitor == monitors[0]);
            }
            edidHex_txt.Text = sb.ToString();
        }  //顯示 EDID 內容
        private void AppendMonitor(StringBuilder sb, EdidReader.MonitorEdid monitor, bool trackLines)
        {
            byte[] edid = monitor.Edid;
            int declaredExt = edid.Length > 0x7E ? edid[0x7E] : 0;
            // Windows 有時會在宣告長度之後補 0，去掉超出宣告範圍且全為 0 的 block
            // （超出宣告但有內容的 block 照常顯示，例如 HDMI 2.1 EEODB）
            int length = edid.Length;
            while (length > (1 + declaredExt) * BlockSize && length % BlockSize == 0 && IsAllZero(edid, length - BlockSize, BlockSize))
            {
                length -= BlockSize;
            }
            int blocks = (length + BlockSize - 1) / BlockSize;
            sb.AppendLine($"螢幕: {monitor.MonitorId}");
            sb.AppendLine($"來源: {monitor.RegistryPath}");
            sb.AppendLine($"長度: {length} bytes（{blocks} block）  延伸區塊數 (byte 0x7E): {declaredExt}");
            if (length < edid.Length)
            {
                sb.AppendLine($"登錄檔另有 {edid.Length - length} bytes 補零資料，未列出");
            }
            if (monitor.HasOverride)
            {
                sb.AppendLine("注意: 此螢幕有 EDID_OVERRIDE，Windows 實際使用的可能是覆寫內容");
            }
            for (int block = 0; block < blocks; block++)
            {
                int start = block * BlockSize;
                int blockLength = Math.Min(BlockSize, length - start);
                sb.AppendLine();
                sb.AppendLine($"[Block {block}]  0x{start:X3}-0x{start + blockLength - 1:X3}  checksum {ChecksumText(edid, start, blockLength)}");
                sb.AppendLine("      00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F");
                for (int offset = start; offset < start + blockLength; offset += BytesPerLine)
                {
                    if (trackLines)
                    {
                        _lineToOffset[CountLines(sb)] = offset;
                    }
                    sb.Append($"{offset:X3}:  ");
                    for (int i = offset; i < Math.Min(offset + BytesPerLine, start + blockLength); i++)
                    {
                        sb.Append($"{edid[i]:X2} ");
                    }
                    sb.AppendLine();
                }
            }
            sb.AppendLine();
        }  //單台螢幕的 hex 內容
        private static string ChecksumText(byte[] edid, int start, int length)
        {
            if (length < BlockSize)
            {
                return "（不足 128 bytes，無法檢查）";
            }
            int sum = 0;
            for (int i = start; i < start + BlockSize; i++)
            {
                sum += edid[i];
            }
            return (sum & 0xFF) == 0 ? "OK" : $"錯誤（總和 0x{sum & 0xFF:X2}）";
        }  //檢查 block checksum
        private static bool IsAllZero(byte[] data, int start, int length)
        {
            for (int i = start; i < start + length; i++)
            {
                if (data[i] != 0) return false;
            }
            return true;
        }  //區段是否全為 0
        private static int CountLines(StringBuilder sb)
        {
            int lines = 0;
            for (int i = 0; i < sb.Length; i++)
            {
                if (sb[i] == '\n') lines++;
            }
            return lines;
        }  //目前行數
        //滑鼠提示-----------------------------------------------------------------------------------------------------
        private void edidHex_txt_MouseMove(object sender, MouseEventArgs e)
        {
            int offset = GetOffsetAt(e.Location);
            if (offset == _lastTipOffset) return;
            _lastTipOffset = offset;
            toolTip.SetToolTip(edidHex_txt, offset >= 0 ? GetByteDescription(offset) : null);
        }  //滑鼠移到 byte 上顯示說明
        private int GetOffsetAt(Point location)
        {
            if (_edid == null) return -1;
            int charIndex = edidHex_txt.GetCharIndexFromPosition(location);
            int line = edidHex_txt.GetLineFromCharIndex(charIndex);
            if (!_lineToOffset.TryGetValue(line, out int lineOffset)) return -1;
            int column = charIndex - edidHex_txt.GetFirstCharIndexFromLine(line) - HexColumnStart;
            if (column < 0) return -1;
            int offset = lineOffset + column / 3;  // 每個 byte 佔 "XX " 三個字元
            return offset < _edid.Length && column / 3 < BytesPerLine ? offset : -1;
        }  //滑鼠位置對應的 byte offset
        private string GetByteDescription(int offset)
        {
            // TODO: 依 EDID 規格補上各欄位說明（廠商 ID、時序、CTA-861 等）
            return $"0x{offset:X3} = 0x{_edid[offset]:X2}";
        }  //byte 說明（之後擴充）
    }

    // 從 Windows 登錄檔讀取螢幕 EDID（Windows 在螢幕連接時讀取並快取的內容）
    public static class EdidReader
    {
        public class MonitorEdid
        {
            public string MonitorId;     // 例如 DISPLAY\VSCA939\4&2b729ab&0&UID8261
            public string RegistryPath;
            public byte[] Edid;
            public bool HasOverride;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DISPLAY_DEVICE
        {
            public int cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string DeviceString;
            public int StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string DeviceKey;
        }
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool EnumDisplayDevices(string lpDevice, int iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, int dwFlags);
        private const int EDD_GET_DEVICE_INTERFACE_NAME = 0x1;
        private const int DISPLAY_DEVICE_ACTIVE = 0x1;

        public static List<MonitorEdid> Read(string screenDeviceName)
        {
            var result = new List<MonitorEdid>();
            var device = new DISPLAY_DEVICE { cb = Marshal.SizeOf(typeof(DISPLAY_DEVICE)) };
            // 列出這個輸出 (\\.\DISPLAYn) 底下的螢幕，DeviceID 形如 \\?\DISPLAY#VSCA939#4&2b729ab&0&UID8261#{guid}
            for (int i = 0; EnumDisplayDevices(screenDeviceName, i, ref device, EDD_GET_DEVICE_INTERFACE_NAME); i++)
            {
                if ((device.StateFlags & DISPLAY_DEVICE_ACTIVE) != 0)
                {
                    string[] parts = device.DeviceID.Split('#');
                    if (parts.Length >= 3)
                    {
                        MonitorEdid monitor = ReadRegistry(parts[1], parts[2]);
                        if (monitor != null) result.Add(monitor);
                    }
                }
                device.cb = Marshal.SizeOf(typeof(DISPLAY_DEVICE));
            }
            return result;
        }  //讀取指定輸出的 EDID
        private static MonitorEdid ReadRegistry(string model, string instance)
        {
            string path = $@"SYSTEM\CurrentControlSet\Enum\DISPLAY\{model}\{instance}\Device Parameters";
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(path))
            {
                if (!(key?.GetValue("EDID") is byte[] edid) || edid.Length == 0) return null;
                bool hasOverride;
                using (RegistryKey overrideKey = key.OpenSubKey("EDID_OVERRIDE"))
                {
                    hasOverride = overrideKey != null && overrideKey.ValueCount > 0;
                }
                return new MonitorEdid
                {
                    MonitorId = $@"DISPLAY\{model}\{instance}",
                    RegistryPath = $@"HKLM\{path}\EDID",
                    Edid = edid,
                    HasOverride = hasOverride,
                };
            }
        }  //從登錄檔讀取原始 EDID
    }
}
