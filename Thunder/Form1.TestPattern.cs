using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;

namespace Thunder
{
    public partial class mainWindow
    {
        // ImgEditor「Testpattern」頁：Gamma 檢查、黑位 / 白位（PLUGE）、邊緣刻度（Overscan）
        private GroupBox tabimgeTest;
        private RadioButton tabitGamma_rdo, tabitPluge_rdo, tabitOverscan_rdo, tabitResolution_rdo, tabitHue_rdo, tabitShading_rdo, tabitBanding_rdo;
        private ComboBox tabitGammaColor_cmb, tabitOverscan_cmb, tabitHue_cmb;
        private static readonly string[] HueDirections = { "水平", "垂直", "環狀" };
        private static readonly string[] HueConfigNames = { "h", "v", "ring" };
        private static readonly double[] GammaChecks = { 1.6, 1.8, 2.0, 2.2, 2.4, 2.6, 2.8 };
        private static readonly string[] GammaChannels = { "灰", "R", "G", "B" };
        private void InitTestPatternPage()
        {
            tabimgeTest = new GroupBox { Text = "Testpattern", Visible = false };
            tabimgeFrame.Parent.Controls.Add(tabimgeTest);
            tabimgeFuncList.Items.Add("Testpattern");
            tabitGamma_rdo = new RadioButton { Text = "Gamma 檢查", AutoSize = true, Location = new Point(15, 25), Checked = true };
            tabitGammaColor_cmb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(235, 23), Width = 60, ImeMode = ImeMode.Disable };
            tabitGammaColor_cmb.Items.AddRange(GammaChannels);
            tabitGammaColor_cmb.SelectedIndex = 0;
            tabitPluge_rdo = new RadioButton { Text = "黑位 / 白位（PLUGE）", AutoSize = true, Location = new Point(15, 55) };
            tabitOverscan_rdo = new RadioButton { Text = "邊緣刻度（Overscan）", AutoSize = true, Location = new Point(15, 85) };
            tabitOverscan_cmb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(235, 83), Width = 60, ImeMode = ImeMode.Disable };
            tabitOverscan_cmb.Items.AddRange(new object[] { "10", "20", "50" });
            tabitOverscan_cmb.SelectedItem = "20";
            tabitResolution_rdo = new RadioButton { Text = "解析度（清晰度）", AutoSize = true, Location = new Point(15, 115) };
            tabitHue_rdo = new RadioButton { Text = "色相漸層", AutoSize = true, Location = new Point(15, 145) };
            tabitHue_cmb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(235, 143), Width = 60, ImeMode = ImeMode.Disable };
            tabitHue_cmb.Items.AddRange(HueDirections);
            tabitHue_cmb.SelectedIndex = 0;
            tabitShading_rdo = new RadioButton { Text = "高頻 Shading", AutoSize = true, Location = new Point(15, 175) };
            tabitBanding_rdo = new RadioButton { Text = "高對比 Banding", AutoSize = true, Location = new Point(15, 205) };
            tabimgeTest.Controls.AddRange(new Control[] { tabitHue_rdo, tabitHue_cmb, tabitShading_rdo, tabitBanding_rdo, new Label { Text = "方向", AutoSize = true, Location = new Point(200, 147) } });
            toolTip.SetToolTip(tabitHue_cmb, "水平 / 垂直：色相 0~360° 沿畫面變化；環狀：以畫面中心為圓心繞一圈");
            var note = new Label
            {
                AutoSize = true, Location = new Point(15, 240), ForeColor = Color.DimGray,
                Text = "Gamma：左右是 1px 黑白橫線（等效 50% 亮度），中間純色塊與它一樣亮的那一格就是螢幕的 gamma\r\n" +
                       "PLUGE：上半部黑底放 1~20 階、下半部白底放 235~254 階，看從第幾階開始分得出來\r\n" +
                       "Overscan：四邊從外往內每 1px 一條線（紅 = 5 的倍數、黃 = 10 的倍數），看邊緣被吃掉幾 px\r\n" +
                       "解析度：左邊西門子星（放射狀黑白扇形，看中心多近開始糊掉），右邊 1~4px 的水平 / 垂直 / 斜向線對\r\n" +
                       "色相漸層：色相 0~360° 連續變化，看色彩過渡有沒有斷層\r\n" +
                       "高頻 Shading：灰底上四條由細到粗的條紋 / 棋盤；高對比 Banding：三種灰底上的黑白橫條",
            };
            tabimgeTest.Controls.AddRange(new Control[] { tabitGamma_rdo, tabitGammaColor_cmb, tabitPluge_rdo, tabitOverscan_rdo, tabitOverscan_cmb, tabitResolution_rdo, note,
                new Label { Text = "色版", AutoSize = true, Location = new Point(200, 27) }, new Label { Text = "範圍", AutoSize = true, Location = new Point(200, 87) }, new Label { Text = "px", AutoSize = true, Location = new Point(298, 87) } });
            toolTip.SetToolTip(tabitGammaColor_cmb, "Gamma 檢查使用的色版（灰 = 三色一起）");
            toolTip.SetToolTip(tabitOverscan_cmb, "從邊緣往內畫幾 px 的刻度");
        }  // 建立 Testpattern 頁
        private void DrawTestPattern(Graphics g, int width, int height)
        {
            if (tabitPluge_rdo.Checked) DrawPluge(g, width, height);
            else if (tabitOverscan_rdo.Checked) DrawOverscan(g, width, height, int.Parse(tabitOverscan_cmb.Text));
            else if (tabitResolution_rdo.Checked) DrawResolution(g, width, height);
            else if (tabitHue_rdo.Checked) DrawHue(g, width, height, tabitHue_cmb.SelectedIndex);
            else if (tabitShading_rdo.Checked) DrawShading(g, width, height);
            else if (tabitBanding_rdo.Checked) DrawBanding(g, width, height);
            else DrawGammaCheck(g, width, height, tabitGammaColor_cmb.SelectedIndex);
        }  // 依選擇畫測試圖
        private static Color HueColor(double hue)
        {
            // HSV（飽和度、明度 100%）轉 RGB
            hue = (hue % 360 + 360) % 360;
            double x = 1 - Math.Abs(hue / 60 % 2 - 1);
            double r, g, b;
            if (hue < 60) { r = 1; g = x; b = 0; }
            else if (hue < 120) { r = x; g = 1; b = 0; }
            else if (hue < 180) { r = 0; g = 1; b = x; }
            else if (hue < 240) { r = 0; g = x; b = 1; }
            else if (hue < 300) { r = x; g = 0; b = 1; }
            else { r = 1; g = 0; b = x; }
            return Color.FromArgb((int)Math.Round(r * 255), (int)Math.Round(g * 255), (int)Math.Round(b * 255));
        }  // 色相轉顏色
        private static void DrawHue(Graphics g, int width, int height, int direction)
        {
            // 0 水平、1 垂直：沿畫面 0~360°；2 環狀：以中心為圓心依角度（0° 在右方，順時針）
            if (direction == 2)
            {
                using (var bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    var data = bitmap.LockBits(new Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    int[] row = new int[width];
                    double cx = width / 2.0, cy = height / 2.0;
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++) row[x] = HueColor(Math.Atan2(y + 0.5 - cy, x + 0.5 - cx) * 180 / Math.PI).ToArgb();
                        System.Runtime.InteropServices.Marshal.Copy(row, 0, data.Scan0 + y * data.Stride, width);
                    }
                    bitmap.UnlockBits(data);
                    g.DrawImageUnscaled(bitmap, 0, 0);
                }
                return;
            }
            int length = direction == 0 ? width : height;
            for (int i = 0; i < length; i++)
            {
                using (Brush brush = new SolidBrush(HueColor(i * 360.0 / length)))
                {
                    if (direction == 0) g.FillRectangle(brush, i, 0, 1, height);
                    else g.FillRectangle(brush, 0, i, width, 1);
                }
            }
        }  // 色相漸層
        private static void FillTile(Graphics g, Rectangle area, int tileWidth, int tileHeight, Func<int, int, bool> isWhite)
        {
            // 以區塊左上角為原點，用週期磚塊鋪滿（白 / 黑）
            using (var tile = new Bitmap(tileWidth, tileHeight))
            {
                for (int y = 0; y < tileHeight; y++) for (int x = 0; x < tileWidth; x++) tile.SetPixel(x, y, isWhite(x, y) ? Color.White : Color.Black);
                using (var brush = new TextureBrush(tile, System.Drawing.Drawing2D.WrapMode.Tile))
                {
                    brush.TranslateTransform(area.X, area.Y);
                    g.FillRectangle(brush, area);
                }
            }
        }  // 磚塊鋪滿
        private static void DrawTitle(Graphics g, int width, int height, string title, Color color)
        {
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            using (Font font = new Font("Arial", Math.Max(12f, height / 60f), GraphicsUnit.Pixel))
            using (Brush brush = new SolidBrush(color))
            {
                SizeF size = g.MeasureString(title, font);
                g.DrawString(title, font, brush, (width - size.Width) / 2, height * 0.015f);
            }
        }  // 上方置中標題
        private static void DrawShading(Graphics g, int width, int height)
        {
            // 128 灰底，四條由細到粗：1px 直條、4px 直條、窄棋盤（兩列）、方棋盤（兩列）
            g.Clear(Color.FromArgb(128, 128, 128));
            DrawTitle(g, width, height, "Shading - High Frequency", Color.White);
            int bandHeight = height / 8, left = width * 3 / 40, bandWidth = width - left * 2;
            for (int i = 0; i < 4; i++)
            {
                var band = new Rectangle(left, (int)(height * (0.1 + i * 0.22)), bandWidth, bandHeight);
                int half = Math.Max(1, bandHeight / 2);
                switch (i)
                {
                    case 0: FillTile(g, band, 2, 1, (x, y) => x == 0); break;                                         // 1px 直條
                    case 1: FillTile(g, band, 8, 1, (x, y) => x < 4); break;                                          // 4px 直條
                    case 2: FillTile(g, band, 24, half * 2, (x, y) => (x / 12 + y / half) % 2 == 0); break;          // 12px 寬棋盤
                    default: FillTile(g, band, half * 2, half * 2, (x, y) => (x / half + y / half) % 2 == 0); break;  // 方棋盤
                }
            }
        }  // 高頻 Shading
        private static void DrawBanding(Graphics g, int width, int height)
        {
            // 上中下三區（192 / 128 / 64 灰底），每區左右各一塊黑白橫條：左邊粗、右邊細
            int[] backgrounds = { 192, 128, 64 };
            for (int i = 0; i < 3; i++)
            {
                int top = height * i / 3, bottom = height * (i + 1) / 3;
                using (Brush back = new SolidBrush(Color.FromArgb(backgrounds[i], backgrounds[i], backgrounds[i])))
                {
                    g.FillRectangle(back, 0, top, width, bottom - top);
                }
                if (i > 0) g.FillRectangle(Brushes.Black, 0, top, width, 2);  // 區塊分隔線
                int blockTop = top + (bottom - top) / 8, blockHeight = (bottom - top) * 3 / 4;
                int stripeThick = Math.Max(1, blockHeight / 11), stripeThin = Math.Max(1, blockHeight / 21);
                FillTile(g, new Rectangle(width * 12 / 100, blockTop, width * 34 / 100, blockHeight), 1, stripeThick * 2, (x, y) => y < stripeThick);
                FillTile(g, new Rectangle(width * 54 / 100, blockTop, width * 34 / 100, blockHeight), 1, stripeThin * 2, (x, y) => y < stripeThin);
            }
            DrawTitle(g, width, height, "Banding - High Contrast", Color.Black);
        }  // 高對比 Banding
        private static Color ChannelColor(int channel, int level) =>
            channel == 1 ? Color.FromArgb(level, 0, 0) : channel == 2 ? Color.FromArgb(0, level, 0) : channel == 3 ? Color.FromArgb(0, 0, level) : Color.FromArgb(level, level, level);
        private static void DrawGammaCheck(Graphics g, int width, int height, int channel)
        {
            // 底：1px 亮 / 暗 交錯橫線（平均亮度 50%）；中間一排純色塊，亮度 = 255 × 0.5^(1/γ)
            using (Brush on = new SolidBrush(ChannelColor(channel, 255)))
            {
                g.Clear(Color.Black);
                for (int y = 0; y < height; y += 2) g.FillRectangle(on, 0, y, width, 1);
            }
            int count = GammaChecks.Length;
            int patchWidth = width / (count + 2), patchHeight = height / 3;
            int gap = (width - patchWidth * count) / (count + 1);
            int top = (height - patchHeight) / 2;
            using (Font font = new Font("Arial", Math.Max(12f, height / 40f), FontStyle.Bold, GraphicsUnit.Pixel))
            {
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                for (int i = 0; i < count; i++)
                {
                    double gamma = GammaChecks[i];
                    int level = (int)Math.Round(255 * Math.Pow(0.5, 1 / gamma));
                    int x = gap + i * (patchWidth + gap);
                    using (Brush brush = new SolidBrush(ChannelColor(channel, level)))
                    {
                        g.FillRectangle(brush, x, top, patchWidth, patchHeight);
                    }
                    // 標示放在色塊下方的黑底上，避免干擾比較
                    string label = $"γ{gamma:0.0}\n{level}";
                    SizeF size = g.MeasureString(label, font);
                    var box = new RectangleF(x, top + patchHeight + 8, patchWidth, size.Height + 6);
                    g.FillRectangle(Brushes.Black, box);
                    using (var format = new StringFormat { Alignment = StringAlignment.Center })
                    {
                        g.DrawString(label, font, Brushes.Gray, new RectangleF(box.X, box.Y + 3, box.Width, box.Height), format);
                    }
                }
            }
        }  // Gamma 檢查圖
        private static void DrawPluge(Graphics g, int width, int height)
        {
            // 上半部黑底（0）放 1~20 階，下半部白底（255）放 235~254 階，各兩排十格
            void Half(int top, int halfHeight, int background, int firstLevel, Brush labelBrush)
            {
                using (Brush back = new SolidBrush(Color.FromArgb(background, background, background)))
                {
                    g.FillRectangle(back, 0, top, width, halfHeight);
                }
                int columns = 10;
                int cellWidth = width / (columns + 2), cellHeight = halfHeight / 4;
                int gapX = (width - cellWidth * columns) / (columns + 1);
                using (Font font = new Font("Arial", Math.Max(11f, cellHeight / 5f), GraphicsUnit.Pixel))
                using (var format = new StringFormat { Alignment = StringAlignment.Center })
                {
                    for (int i = 0; i < 20; i++)
                    {
                        int level = firstLevel + i;
                        int column = i % columns, row = i / columns;
                        int x = gapX + column * (cellWidth + gapX);
                        int y = top + halfHeight / 8 + row * (cellHeight * 3 / 2 + halfHeight / 16);
                        using (Brush brush = new SolidBrush(Color.FromArgb(level, level, level)))
                        {
                            g.FillRectangle(brush, x, y, cellWidth, cellHeight);
                        }
                        g.DrawString(level.ToString(), font, labelBrush, new RectangleF(x, y + cellHeight + 2, cellWidth, font.Height + 2), format);
                    }
                }
            }
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            using (Brush darkLabel = new SolidBrush(Color.FromArgb(96, 96, 96)))
            using (Brush lightLabel = new SolidBrush(Color.FromArgb(160, 160, 160)))
            {
                Half(0, height / 2, 0, 1, darkLabel);
                Half(height / 2, height - height / 2, 255, 235, lightLabel);
            }
        }  // 黑位 / 白位
        private static void DrawOverscan(Graphics g, int width, int height, int range)
        {
            // 四邊從外往內每 1px 一條框線：白 / 灰交錯，5 的倍數紅色、10 的倍數黃色；四邊中間標示 0、10、20…
            g.Clear(Color.Black);
            for (int d = 0; d < range && d * 2 < Math.Min(width, height); d++)
            {
                Color color = d % 10 == 0 ? Color.Yellow : d % 5 == 0 ? Color.Red : d % 2 == 0 ? Color.White : Color.FromArgb(110, 110, 110);
                using (Pen pen = new Pen(color, 1))
                {
                    g.DrawRectangle(pen, d, d, width - 1 - d * 2, height - 1 - d * 2);
                }
            }
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            using (Font font = new Font("Arial", 9f, GraphicsUnit.Pixel))
            using (Font titleFont = new Font("Arial", Math.Max(16f, height / 30f), GraphicsUnit.Pixel))
            {
                for (int d = 0; d < range; d += 10)
                {
                    string label = d.ToString();
                    SizeF size = g.MeasureString(label, font);
                    // 上下兩邊：標在中線右側；左右兩邊：標在中線下方
                    float cx = width / 2f + 6, cy = height / 2f + 6;
                    void Label(float x, float y)
                    {
                        g.FillRectangle(Brushes.Black, x, y, size.Width, size.Height);
                        g.DrawString(label, font, Brushes.Yellow, x, y);
                    }
                    Label(cx, d - size.Height / 2 + 0.5f);
                    Label(cx, height - 1 - d - size.Height / 2 + 0.5f);
                    Label(d + 0.5f - size.Width / 2, cy);
                    Label(width - 1 - d + 0.5f - size.Width / 2, cy);
                }
                string title = $"Overscan 0~{range - 1} px（紅 = 5 的倍數、黃 = 10 的倍數）";
                SizeF titleSize = g.MeasureString(title, titleFont);
                g.DrawString(title, titleFont, Brushes.White, (width - titleSize.Width) / 2, height / 2f - titleSize.Height * 1.5f);
            }
        }  // 邊緣刻度
        private static void DrawResolution(Graphics g, int width, int height)
        {
            // 左：西門子星（72 道黑白扇形）；右：1~4px 線對，分水平、垂直、斜向三列；不做反鋸齒，只有黑白兩色
            g.Clear(Color.Black);
            g.SmoothingMode = SmoothingMode.None;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            int radius = (int)Math.Min(width * 0.22, height * 0.42);
            var center = new Point((int)(width * 0.26), height / 2);
            var star = new Rectangle(center.X - radius, center.Y - radius, radius * 2, radius * 2);
            for (int i = 0; i < 36; i++) g.FillPie(Brushes.White, star, i * 10f, 5f);
            g.DrawEllipse(Pens.Gray, star);
            using (Font font = new Font("Arial", Math.Max(12f, height / 50f), GraphicsUnit.Pixel))
            using (Brush label = new SolidBrush(Color.FromArgb(170, 170, 170)))
            {
                g.DrawString("Siemens star（72 spokes）", font, label, star.X, star.Bottom + 6);
                // 線對區
                int[] pitches = { 1, 2, 3, 4 };
                string[] rowNames = { "水平", "垂直", "斜向" };
                int left = (int)(width * 0.52), top = (int)(height * 0.12);
                int areaWidth = width - left - (int)(width * 0.04), areaHeight = (int)(height * 0.76);
                int labelWidth = (int)(font.Size * 3);
                int gap = Math.Max(8, height / 60);
                int blockWidth = (areaWidth - labelWidth - gap * pitches.Length) / pitches.Length;
                int blockHeight = (areaHeight - (int)(font.Height * 1.5) - gap * rowNames.Length) / rowNames.Length;
                for (int c = 0; c < pitches.Length; c++)
                {
                    g.DrawString($"{pitches[c]}px", font, label, left + labelWidth + c * (blockWidth + gap), top);
                }
                for (int r = 0; r < rowNames.Length; r++)
                {
                    int y0 = top + (int)(font.Height * 1.5) + r * (blockHeight + gap);
                    g.DrawString(rowNames[r], font, label, left, y0 + blockHeight / 2 - font.Height / 2);
                    for (int c = 0; c < pitches.Length; c++)
                    {
                        int p = pitches[c];
                        int x0 = left + labelWidth + c * (blockWidth + gap);
                        // 一個週期的磚塊（亮 p、暗 p），以區塊左上角為原點鋪滿
                        using (var tile = new Bitmap(2 * p, 2 * p))
                        {
                            for (int ty = 0; ty < 2 * p; ty++)
                            {
                                for (int tx = 0; tx < 2 * p; tx++)
                                {
                                    int phase = r == 0 ? ty : r == 1 ? tx : tx + ty;
                                    tile.SetPixel(tx, ty, (phase / p) % 2 == 0 ? Color.White : Color.Black);
                                }
                            }
                            using (var brush = new TextureBrush(tile, WrapMode.Tile))
                            {
                                brush.TranslateTransform(x0, y0);
                                g.FillRectangle(brush, x0, y0, blockWidth, blockHeight);
                            }
                        }
                    }
                }
            }
        }  // 解析度測試圖
        private void funcTest(string param)
        {
            // 參數：gamma[-r|-g|-b]、pluge、overscan[-範圍]
            string[] parts = param.Split('-');
            switch (parts[0])
            {
                case "gamma":
                    tabitGamma_rdo.Checked = true;
                    string channel = parts.Length > 1 ? parts[1] : "";
                    int index = channel == "" ? 0 : Array.IndexOf(new[] { "", "r", "g", "b" }, channel);
                    if (index < 0) throw new FormatException($"Gamma 色版「{channel}」只能是 r、g、b 或省略（灰）");
                    tabitGammaColor_cmb.SelectedIndex = index;
                    break;
                case "resolution":
                    tabitResolution_rdo.Checked = true;
                    break;
                case "hue":
                    tabitHue_rdo.Checked = true;
                    string direction = parts.Length > 1 ? parts[1] : "h";
                    int directionIndex = Array.IndexOf(HueConfigNames, direction);
                    if (directionIndex < 0) throw new FormatException($"色相漸層方向「{direction}」只能是 h、v、ring");
                    tabitHue_cmb.SelectedIndex = directionIndex;
                    break;
                case "shading":
                    tabitShading_rdo.Checked = true;
                    break;
                case "banding":
                    tabitBanding_rdo.Checked = true;
                    break;
                case "pluge":
                    tabitPluge_rdo.Checked = true;
                    break;
                case "overscan":
                    tabitOverscan_rdo.Checked = true;
                    string range = parts.Length > 1 ? parts[1] : "20";
                    if (!tabitOverscan_cmb.Items.Contains(range)) throw new FormatException($"Overscan 範圍「{range}」只能是 10、20、50");
                    tabitOverscan_cmb.SelectedItem = range;
                    break;
                default:
                    throw new FormatException($"測試圖「{param}」只能是 gamma、pluge、overscan、resolution、hue、shading、banding");
            }
        }  // listbox控制測試圖
        private string TestToConfig(out string param, out string option)
        {
            option = "";
            if (tabitPluge_rdo.Checked) param = "pluge";
            else if (tabitOverscan_rdo.Checked) param = $"overscan-{tabitOverscan_cmb.Text}";
            else if (tabitResolution_rdo.Checked) param = "resolution";
            else if (tabitHue_rdo.Checked) param = $"hue-{HueConfigNames[tabitHue_cmb.SelectedIndex]}";
            else if (tabitShading_rdo.Checked) param = "shading";
            else if (tabitBanding_rdo.Checked) param = "banding";
            else param = "gamma" + (tabitGammaColor_cmb.SelectedIndex == 0 ? "" : "-" + "rgb"[tabitGammaColor_cmb.SelectedIndex - 1]);
            return null;
        }  // 測試圖轉 config
        // Dynamic 模糊測試條----------------------------------------------------------------------------------------
        private RadioButton tabiedObjectBlur_rdo;
        private void InitDynamicBlur()
        {
            // 移動物件多一種「模糊條」：寬度 = 物件大小的條紋光柵，移動時看拖影（只能水平或垂直移動）
            tabiedObject_grp.Height += 25;
            tabiedObjectBlur_rdo = new RadioButton { Text = "模糊條", AutoSize = true, Location = new Point(15, 95) };
            tabiedObject_grp.Controls.Add(tabiedObjectBlur_rdo);
            toolTip.SetToolTip(tabiedObjectBlur_rdo, "移動模糊測試：寬度為物件大小的條紋（2px 物件色 / 2px 背景），水平或垂直移動，看拖影與殘影");
            void UpdateDirection(object sender, EventArgs e)
            {
                // 長條與模糊條都不能斜角移動
                bool lineOnly = tabiedObjectStrip_rdo.Checked || tabiedObjectBlur_rdo.Checked;
                tabiedMoveBevel_rdo.Enabled = !lineOnly;
                if (lineOnly && tabiedMoveBevel_rdo.Checked) tabiedMoveH_rdo.Checked = true;
            }
            tabiedObjectBlur_rdo.CheckedChanged += UpdateDirection;
            tabiedObjectStrip_rdo.CheckedChanged += UpdateDirection;  // 在原本的處理之後重新判斷
        }  // 建立模糊測試條
        private bool DynamicIsLine => tabiedObjectStrip_rdo.Checked || (tabiedObjectBlur_rdo != null && tabiedObjectBlur_rdo.Checked);
        private void DrawBlurBar(Graphics g, Brush brush, int width, int height)
        {
            // 條紋光柵：每 4px 一組（2px 物件色 + 2px 背景），整條寬度 = 物件大小
            int size = mypicture.objectSize;
            for (int offset = 0; offset < size; offset += 4)
            {
                if (tabiedMoveV_rdo.Checked) g.FillRectangle(brush, 0, mypicture.startY + offset, width, Math.Min(2, size - offset));
                else g.FillRectangle(brush, mypicture.startX + offset, 0, Math.Min(2, size - offset), height);
            }
        }  // 畫模糊測試條
        // ImageAdjust 差異比對------------------------------------------------------------------------------------
        private RadioButton tabieaDiff_rdo;
        private void InitImageDiff()
        {
            tabieaDiff_rdo = new RadioButton { Text = "差異比對", AutoSize = true, Location = new Point(15, 205) };
            tabieaDiff_rdo.CheckedChanged += tabiea_rdo_CheckedChanged;  // 與其他選項相同：顯示圖片清單
            tabimgeAdjust.Controls.Add(tabieaDiff_rdo);
            tabieaTrans_btn.Top = 230;
            toolTip.SetToolTip(tabieaDiff_rdo, "勾選清單中的兩張圖，按 Translate 產生差異圖（相同為黑、不同為紅）並統計差異");
        }  // 建立差異比對
        private void RunImageDiff()
        {
            var paths = tabieaPatternList_chl.CheckedItems.Cast<object>().Select(i => i.ToString()).ToList();
            if (paths.Count != 2)
            {
                MessageBox.Show($"請在清單中勾選剛好兩張圖片（目前勾選 {paths.Count} 張）", "差異比對", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Bitmap Load(string path)
            {
                using (var stream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                using (var image = Image.FromStream(stream))
                {
                    return new Bitmap(image);
                }
            }
            try
            {
                using (Bitmap a = Load(paths[0]))
                using (Bitmap b = Load(paths[1]))
                {
                    if (a.Size != b.Size)
                    {
                        MessageBox.Show($"兩張圖片大小不同（{a.Width}x{a.Height} / {b.Width}x{b.Height}），無法逐點比對", "差異比對", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    Bitmap diff = DiffImages(a, b, out long count, out int maxDiff, out Rectangle bounds);
                    string name = $"{System.IO.Path.GetFileNameWithoutExtension(paths[0])}_vs_{System.IO.Path.GetFileNameWithoutExtension(paths[1])}_diff.png";
                    string output = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(paths[0]), name);
                    diff.Save(output, System.Drawing.Imaging.ImageFormat.Png);
                    showimgPicture_pic.Image = mypicture.Setpicture(diff, "image");
                    showimgPicture_pic.SizeMode = PictureBoxSizeMode.Zoom;
                    ShowPreviewInfo(diff.Width, diff.Height, null);
                    diff.Dispose();
                    double percent = count * 100.0 / ((long)a.Width * a.Height);
                    string summary = count == 0
                        ? "兩張圖片完全相同"
                        : $"不同的像素：{count:N0} 個（{percent:0.###}%）\r\n最大差值：{maxDiff}（RGB 中差最多的色版）\r\n範圍：({bounds.Left},{bounds.Top}) ~ ({bounds.Right - 1},{bounds.Bottom - 1})";
                    ssrStatus_lbl.Text = $"差異比對：{(count == 0 ? "完全相同" : $"{count:N0} 個像素不同")}，已存 {name}";
                    MessageBox.Show($"{summary}\r\n\r\n差異圖已存到：\r\n{output}", "差異比對", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"差異比對失敗：{ex.Message}", "差異比對", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }  // 兩張圖片差異比對
        private static Bitmap DiffImages(Bitmap a, Bitmap b, out long count, out int maxDiff, out Rectangle bounds)
        {
            // 相同的像素為黑，不同的像素為紅（差越大越亮，最暗 96 以免看不到）
            int width = a.Width, height = a.Height;
            var rect = new Rectangle(0, 0, width, height);
            var format = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
            var result = new Bitmap(width, height, format);
            var dataA = a.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, format);
            var dataB = b.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, format);
            var dataR = result.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, format);
            count = 0;
            maxDiff = 0;
            int left = width, top = height, right = -1, bottom = -1;
            try
            {
                int[] rowA = new int[width], rowB = new int[width], rowR = new int[width];
                for (int y = 0; y < height; y++)
                {
                    System.Runtime.InteropServices.Marshal.Copy(dataA.Scan0 + y * dataA.Stride, rowA, 0, width);
                    System.Runtime.InteropServices.Marshal.Copy(dataB.Scan0 + y * dataB.Stride, rowB, 0, width);
                    for (int x = 0; x < width; x++)
                    {
                        int pa = rowA[x], pb = rowB[x];
                        int d = Math.Max(Math.Abs(((pa >> 16) & 255) - ((pb >> 16) & 255)),
                                Math.Max(Math.Abs(((pa >> 8) & 255) - ((pb >> 8) & 255)), Math.Abs((pa & 255) - (pb & 255))));
                        if (d == 0)
                        {
                            rowR[x] = unchecked((int)0xFF000000);
                            continue;
                        }
                        count++;
                        maxDiff = Math.Max(maxDiff, d);
                        int red = Math.Min(255, 96 + d);
                        rowR[x] = unchecked((int)0xFF000000) | (red << 16);
                        left = Math.Min(left, x); right = Math.Max(right, x);
                        top = Math.Min(top, y); bottom = Math.Max(bottom, y);
                    }
                    System.Runtime.InteropServices.Marshal.Copy(rowR, 0, dataR.Scan0 + y * dataR.Stride, width);
                }
            }
            finally
            {
                a.UnlockBits(dataA);
                b.UnlockBits(dataB);
                result.UnlockBits(dataR);
            }
            bounds = count == 0 ? Rectangle.Empty : Rectangle.FromLTRB(left, top, right + 1, bottom + 1);
            return result;
        }  // 逐點比對
    }
}
