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
        // Testpattern 加強：雜訊數量 / 顆粒、Clipping 截斷條、幾何圖、銳利度、暗部彩色漸層--------------------------------------
        private RadioButton tabitClipping_rdo, tabitGeometry_rdo, tabitSharpness_rdo, tabitDarkGradient_rdo;
        private ComboBox tabitNoiseDensity_cmb, tabitNoiseGrain_cmb, tabitClipping_cmb, tabitDarkGradient_cmb;
        private CheckBox tabitClippingFlash_chk;
        private static readonly string[] NoiseDensities = { "100 點", "1%", "10%", "50%", "100%" };
        private static readonly string[] NoiseDensityTokens = { "100pt", "1%", "10%", "50%", "100%" };  // config 寫法
        private static readonly int[] NoiseGrains = { 1, 2, 4, 8 };
        private static readonly string[] ClippingKinds = { "黑 0~25", "白 230~255" };
        private static readonly int[] DarkGradientRanges = { 32, 64 };
        private Bitmap _blinkAlternate;  // 閃爍用的另一張圖（Clipping 閃爍時條紋消失的那張）
        private Bitmap _blinkFor;        // _blinkAlternate 對應的圖片（mypicture 換圖後就不再閃爍）
        private void InitTestPatternPlus()
        {
            // 雜訊：種類、數量、顆粒排成一列
            tabitNoise_cmb.Location = new Point(440, 53);
            tabitNoise_cmb.Width = 55;
            tabitNoiseDensity_cmb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(500, 53), Width = 60, ImeMode = ImeMode.Disable };
            tabitNoiseDensity_cmb.Items.AddRange(NoiseDensities);
            tabitNoiseDensity_cmb.SelectedIndex = NoiseDensities.Length - 1;
            tabitNoiseGrain_cmb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(565, 53), Width = 55, ImeMode = ImeMode.Disable };
            tabitNoiseGrain_cmb.Items.AddRange(NoiseGrains.Select(g => (object)$"{g} px").ToArray());
            tabitNoiseGrain_cmb.SelectedIndex = 0;
            // 右欄繼續往下
            tabitClipping_rdo = new RadioButton { Text = "Clipping 截斷條", AutoSize = true, Location = new Point(340, 115) };
            tabitClipping_cmb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(470, 113), Width = 90, ImeMode = ImeMode.Disable };
            tabitClipping_cmb.Items.AddRange(ClippingKinds);
            tabitClipping_cmb.SelectedIndex = 0;
            tabitClippingFlash_chk = new CheckBox { Text = "閃爍", AutoSize = true, Location = new Point(565, 115) };
            tabitGeometry_rdo = new RadioButton { Text = "幾何（格線與正圓）", AutoSize = true, Location = new Point(340, 145) };
            tabitSharpness_rdo = new RadioButton { Text = "銳利度環邊檢查", AutoSize = true, Location = new Point(340, 175) };
            tabitDarkGradient_rdo = new RadioButton { Text = "暗部彩色漸層", AutoSize = true, Location = new Point(340, 205) };
            tabitDarkGradient_cmb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(470, 203), Width = 70, ImeMode = ImeMode.Disable };
            tabitDarkGradient_cmb.Items.AddRange(DarkGradientRanges.Select(r => (object)$"0~{r}").ToArray());
            tabitDarkGradient_cmb.SelectedIndex = 0;
            tabimgeTest.Controls.AddRange(new Control[]
            {
                tabitNoiseDensity_cmb, tabitNoiseGrain_cmb, tabitClipping_rdo, tabitClipping_cmb, tabitClippingFlash_chk,
                tabitGeometry_rdo, tabitSharpness_rdo, tabitDarkGradient_rdo, tabitDarkGradient_cmb,
            });
            Label note = tabimgeTest.Controls.OfType<Label>().First(l => l.Text.StartsWith("Gamma："));
            note.Text += "\r\nClipping：每條一個階數，分得出（或看得到閃爍）代表該階沒被壓掉；幾何：圓要正圓、格線要等距\r\n銳利度：細線與邊緣兩側有沒有多出亮邊 / 暗邊；暗部彩色漸層：只用最暗的幾十階，看暗部色階斷層";
            toolTip.SetToolTip(tabitNoiseDensity_cmb, "雜訊數量：整面隨機 100 點，或約 1% / 10% / 50% / 100% 的位置有雜訊（其餘為黑）");
            toolTip.SetToolTip(tabitNoiseGrain_cmb, "雜訊顆粒大小：每一點的邊長");
            toolTip.SetToolTip(tabitClipping_rdo, "AVS HD 709 樣式：黑側 1~25 階在黑底上、白側 230~254 階在白底上，每條標示階數");
            toolTip.SetToolTip(tabitClippingFlash_chk, "全螢幕時條紋每 0.5 秒在該階與背景之間切換，看得到閃爍就代表那一階分得出來");
            toolTip.SetToolTip(tabitGeometry_rdo, "黑底白線：等距格線、中心大圓、四角小圓、對角線與中心十字，看畫面比例與幾何失真");
            toolTip.SetToolTip(tabitSharpness_rdo, "灰底上的黑白寬條、1px 細線與細框：邊緣兩側多出亮邊或暗邊代表有銳化 / 過衝處理");
            toolTip.SetToolTip(tabitDarkGradient_rdo, "黑 → 深藍 / 深紅 / 深綠 / 深灰 / 深青 / 深洋紅 / 深黃，每一階一個 code，看暗部斷層與抖色");
        }  // 建立 Testpattern 加強
        private bool DrawTestPatternPlus(Graphics g, int width, int height)
        {
            // 新增的測試圖（含新的雜訊）；不是這些時回傳 false 交給原本的繪圖
            if (tabitNoise_rdo.Checked) DrawNoise(g, width, height, tabitNoise_cmb.SelectedIndex == 1, tabitNoiseDensity_cmb.SelectedIndex, NoiseGrains[tabitNoiseGrain_cmb.SelectedIndex]);
            else if (tabitClipping_rdo.Checked) DrawClipping(g, width, height, tabitClipping_cmb.SelectedIndex == 1, false);
            else if (tabitGeometry_rdo.Checked) DrawGeometry(g, width, height);
            else if (tabitSharpness_rdo.Checked) DrawSharpness(g, width, height);
            else if (tabitDarkGradient_rdo.Checked) DrawDarkGradient(g, width, height, DarkGradientRanges[tabitDarkGradient_cmb.SelectedIndex]);
            else return false;
            return true;
        }  // 畫新增的測試圖
        private void UpdateTestBlink(int width, int height)
        {
            // Clipping 閃爍：另外產生條紋消失的那張，全螢幕時與原圖交替
            _blinkAlternate?.Dispose();
            _blinkAlternate = null;
            _blinkFor = null;
            if (!tabitClipping_rdo.Checked || !tabitClippingFlash_chk.Checked || mypicture.Getpicture() == null) return;
            _blinkAlternate = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(_blinkAlternate)) DrawClipping(g, width, height, tabitClipping_cmb.SelectedIndex == 1, true);
            _blinkFor = mypicture.Getpicture();
        }  // 更新閃爍圖
        public Bitmap BlinkAlternateFor(Bitmap picture) => _blinkFor != null && ReferenceEquals(picture, _blinkFor) ? _blinkAlternate : null;  // 這張圖要閃爍時的另一張
        private static void DrawNoise(Graphics g, int width, int height, bool color, int density, int grain)
        {
            // 固定亂數種子：同一個設定每次產生的雜訊都一樣；沒有雜訊的位置為黑
            var random = new Random(20261012);
            int columns = (width + grain - 1) / grain, rows = (height + grain - 1) / grain;
            const int black = unchecked((int)0xFF000000);
            int Next() { if (!color) { int v = random.Next(256); return black | v << 16 | v << 8 | v; } return black | random.Next(1 << 24); }
            var cells = new int[columns * rows];
            if (density >= NoiseDensities.Length - 1)
            {
                for (int i = 0; i < cells.Length; i++) cells[i] = Next();  // 100%：與原本的雜訊相同（顆粒 1px 時逐像素）
            }
            else
            {
                for (int i = 0; i < cells.Length; i++) cells[i] = black;
                if (density == 0)
                {
                    // 整面 100 點（不重複的位置）
                    foreach (int i in Enumerable.Range(0, cells.Length).OrderBy(_ => random.Next()).Take(Math.Min(100, cells.Length))) cells[i] = Next();
                }
                else
                {
                    double ratio = density == 1 ? 0.01 : density == 2 ? 0.1 : 0.5;
                    for (int i = 0; i < cells.Length; i++) if (random.NextDouble() < ratio) cells[i] = Next();
                }
            }
            DrawPixels(g, width, height, (x, y) => cells[y / grain * columns + x / grain]);
        }  // 隨機雜訊
        private static void DrawClipping(Graphics g, int width, int height, bool white, bool hideBars)
        {
            // 黑側：黑底上 1~25 階；白側：白底上 230~254 階；每條下方標示階數（hideBars 為閃爍時條紋消失的那張）
            int background = white ? 255 : 0;
            int first = white ? 230 : 1, count = 25;
            g.Clear(Color.FromArgb(background, background, background));
            int top = height / 5, bottom = height * 3 / 4;
            float barWidth = width * 0.9f / count, left = width * 0.05f;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            using (Font font = new Font("Arial", Math.Max(10f, Math.Min(barWidth / 2.2f, height / 40f)), GraphicsUnit.Pixel))
            using (Brush label = new SolidBrush(Color.FromArgb(128, 128, 128)))
            using (var format = new StringFormat { Alignment = StringAlignment.Center })
            {
                for (int i = 0; i < count; i++)
                {
                    int level = first + i;
                    int x0 = (int)Math.Round(left + i * barWidth), x1 = (int)Math.Round(left + (i + 0.8f) * barWidth);  // 條紋之間留背景色的間隔
                    if (!hideBars)
                    {
                        using (Brush brush = new SolidBrush(Color.FromArgb(level, level, level))) g.FillRectangle(brush, x0, top, x1 - x0, bottom - top);
                    }
                    g.DrawString(level.ToString(), font, label, (x0 + x1) / 2f, bottom + font.Size * 0.5f, format);
                }
                g.DrawString(white ? "White clipping 230 ~ 254" : "Black clipping 1 ~ 25", font, label, width / 2f, top - font.Size * 2.5f, format);
            }
        }  // Clipping 截斷條
        private static void DrawGeometry(Graphics g, int width, int height)
        {
            // 黑底白線：以中心為準的等距方格、外框、對角線、中心十字、中心大圓與四角小圓
            g.Clear(Color.Black);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            float cx = width / 2f, cy = height / 2f;
            int spacing = Math.Max(8, height / 10);
            using (var grid = new Pen(Color.FromArgb(160, 160, 160)))
            using (var line = new Pen(Color.White))
            {
                g.SmoothingMode = SmoothingMode.None;
                for (float x = cx % spacing; x < width; x += spacing) g.DrawLine(grid, x, 0, x, height);
                for (float y = cy % spacing; y < height; y += spacing) g.DrawLine(grid, 0, y, width, y);
                g.DrawRectangle(line, 0, 0, width - 1, height - 1);
                g.DrawLine(line, cx, 0, cx, height);
                g.DrawLine(line, 0, cy, width, cy);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawLine(line, 0, 0, width, height);
                g.DrawLine(line, width, 0, 0, height);
                float big = height * 0.9f, small = height / 4f, inset = spacing / 4f;
                g.DrawEllipse(line, cx - big / 2, cy - big / 2, big, big);
                foreach (var (x, y) in new[] { (inset, inset), (width - inset - small, inset), (inset, height - inset - small), (width - inset - small, height - inset - small) })
                {
                    g.DrawEllipse(line, x, y, small, small);
                }
            }
            int divisor = Gcd(width, height);
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            using (Font font = new Font("Arial", Math.Max(12f, height / 45f), GraphicsUnit.Pixel))
            using (var format = new StringFormat { Alignment = StringAlignment.Center })
            {
                g.DrawString($"{width} x {height}  ({width / divisor}:{height / divisor})", font, Brushes.White, cx, cy + spacing / 3f, format);
            }
        }  // 幾何圖
        private static int Gcd(int a, int b) => b == 0 ? Math.Max(1, a) : Gcd(b, a % b);
        private static void DrawSharpness(Graphics g, int width, int height)
        {
            // 128 灰底：上排黑白寬條（看邊緣光暈）、中排 1px 細線組、下排 1px 細框與小點
            g.Clear(Color.FromArgb(128, 128, 128));
            g.SmoothingMode = SmoothingMode.None;
            int margin = width / 20;
            // 上排：黑、白寬條交錯，條與條之間留灰
            int barTop = height / 10, barHeight = height / 4, bars = 8;
            float slot = (width - margin * 2f) / bars;
            for (int i = 0; i < bars; i++)
            {
                g.FillRectangle(i % 2 == 0 ? Brushes.Black : Brushes.White, margin + i * slot + slot * 0.2f, barTop, slot * 0.6f, barHeight);
            }
            // 中排：1px 直線組與橫線組（黑 / 白，間距 2、4、8 px）
            int lineTop = barTop + barHeight + height / 12, lineHeight = height / 6;
            int groupWidth = (width - margin * 2) / 6;
            int[] gaps = { 2, 4, 8 };
            for (int i = 0; i < 6; i++)
            {
                Pen pen = i % 2 == 0 ? Pens.Black : Pens.White;
                int gap = gaps[i / 2], x0 = margin + i * groupWidth + groupWidth / 8, x1 = margin + (i + 1) * groupWidth - groupWidth / 8;
                if (i < 3)
                {
                    for (int x = x0; x < x1; x += gap) g.DrawLine(pen, x, lineTop, x, lineTop + lineHeight);
                }
                else
                {
                    for (int y = lineTop; y < lineTop + lineHeight; y += gap) g.DrawLine(pen, x0, y, x1, y);
                }
            }
            // 下排：1px 細框（黑 / 白）與 2px 小點
            int boxTop = lineTop + lineHeight + height / 12, boxSize = Math.Max(20, height / 8);
            for (int i = 0; i < 8; i++)
            {
                int x = margin + i * (width - margin * 2) / 8 + boxSize / 4;
                Pen pen = i % 2 == 0 ? Pens.Black : Pens.White;
                Brush dot = i % 2 == 0 ? Brushes.Black : Brushes.White;
                if (i < 4) g.DrawRectangle(pen, x, boxTop, boxSize, boxSize);
                else for (int dy = 4; dy < boxSize; dy += 8) for (int dx = 4; dx < boxSize; dx += 8) g.FillRectangle(dot, x + dx, boxTop + dy, 2, 2);
            }
        }  // 銳利度環邊檢查
        private static readonly (string name, int r, int g, int b)[] DarkGradientColors =
        {
            ("Blue", 0, 0, 1), ("Red", 1, 0, 0), ("Green", 0, 1, 0), ("Gray", 1, 1, 1), ("Cyan", 0, 1, 1), ("Magenta", 1, 0, 1), ("Yellow", 1, 1, 0),
        };
        private static void DrawDarkGradient(Graphics g, int width, int height, int range)
        {
            // 每列一個顏色，從黑到該顏色的第 range 階；每一階一個 code（寬度平均分配）
            int rows = DarkGradientColors.Length;
            g.Clear(Color.Black);
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            using (Font font = new Font("Arial", Math.Max(10f, height / 60f), GraphicsUnit.Pixel))
            using (Brush label = new SolidBrush(Color.FromArgb(160, 160, 160)))
            {
                for (int row = 0; row < rows; row++)
                {
                    var (name, r, gr, b) = DarkGradientColors[row];
                    int y0 = height * row / rows, y1 = height * (row + 1) / rows;
                    for (int level = 0; level <= range; level++)
                    {
                        int x0 = width * level / (range + 1), x1 = width * (level + 1) / (range + 1);
                        using (Brush brush = new SolidBrush(Color.FromArgb(r * level, gr * level, b * level))) g.FillRectangle(brush, x0, y0, x1 - x0, y1 - y0 - 2);
                    }
                    g.DrawString($"{name} 0~{range}", font, label, 4, y0 + 2);
                }
            }
        }  // 暗部彩色漸層
        private bool FuncTestPlus(string[] parts)
        {
            // 解析新增的測試圖（含新的雜訊寫法）；不是這些時回傳 false
            switch (parts[0])
            {
                case "noise":
                    // noise[-c][-100pt|-1%|-10%|-50%|-100%][-g1|-g2|-g4|-g8]
                    tabitNoise_rdo.Checked = true;
                    tabitNoise_cmb.SelectedIndex = 0;
                    tabitNoiseDensity_cmb.SelectedIndex = NoiseDensities.Length - 1;
                    tabitNoiseGrain_cmb.SelectedIndex = 0;
                    foreach (string token in parts.Skip(1))
                    {
                        int density = Array.IndexOf(NoiseDensityTokens, token);
                        if (token == "c") tabitNoise_cmb.SelectedIndex = 1;
                        else if (density >= 0) tabitNoiseDensity_cmb.SelectedIndex = density;
                        else if (token.StartsWith("g") && int.TryParse(token.Substring(1), out int grain) && Array.IndexOf(NoiseGrains, grain) >= 0) tabitNoiseGrain_cmb.SelectedIndex = Array.IndexOf(NoiseGrains, grain);
                        else throw new FormatException($"雜訊選項「{token}」只能是 c（彩色）、{string.Join(" / ", NoiseDensityTokens)}（數量）、g1 / g2 / g4 / g8（顆粒）");
                    }
                    return true;
                case "clipping":
                    // clipping-black / clipping-white，結尾加 -f 閃爍
                    tabitClipping_rdo.Checked = true;
                    string kind = parts.Length > 1 ? parts[1] : "black";
                    if (kind != "black" && kind != "white") throw new FormatException($"Clipping「{kind}」只能是 black 或 white");
                    tabitClipping_cmb.SelectedIndex = kind == "white" ? 1 : 0;
                    if (parts.Length > 2 && parts[2] != "f") throw new FormatException($"Clipping 選項「{parts[2]}」只能是 f（閃爍）");
                    tabitClippingFlash_chk.Checked = parts.Length > 2;
                    return true;
                case "geometry":
                    tabitGeometry_rdo.Checked = true;
                    return true;
                case "sharpness":
                    tabitSharpness_rdo.Checked = true;
                    return true;
                case "darkgrad":
                    tabitDarkGradient_rdo.Checked = true;
                    int range = parts.Length > 1 && int.TryParse(parts[1], out int value) ? value : 32;
                    if (Array.IndexOf(DarkGradientRanges, range) < 0) throw new FormatException($"暗部彩色漸層範圍「{range}」只能是 32 或 64");
                    tabitDarkGradient_cmb.SelectedIndex = Array.IndexOf(DarkGradientRanges, range);
                    return true;
                default:
                    return false;
            }
        }  // 解析新增的測試圖
        private bool TestPlusToConfig(out string param)
        {
            param = null;
            if (tabitNoise_rdo.Checked)
            {
                param = "noise" + (tabitNoise_cmb.SelectedIndex == 1 ? "-c" : "")
                      + (tabitNoiseDensity_cmb.SelectedIndex < NoiseDensities.Length - 1 ? "-" + NoiseDensityTokens[tabitNoiseDensity_cmb.SelectedIndex] : "")
                      + (tabitNoiseGrain_cmb.SelectedIndex > 0 ? $"-g{NoiseGrains[tabitNoiseGrain_cmb.SelectedIndex]}" : "");
            }
            else if (tabitClipping_rdo.Checked) param = $"clipping-{(tabitClipping_cmb.SelectedIndex == 1 ? "white" : "black")}{(tabitClippingFlash_chk.Checked ? "-f" : "")}";
            else if (tabitGeometry_rdo.Checked) param = "geometry";
            else if (tabitSharpness_rdo.Checked) param = "sharpness";
            else if (tabitDarkGradient_rdo.Checked) param = $"darkgrad-{DarkGradientRanges[tabitDarkGradient_cmb.SelectedIndex]}";
            return param != null;
        }  // 新增的測試圖轉 config
    }
}
