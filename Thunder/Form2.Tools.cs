using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Thunder
{
    public partial class pictureWindow
    {
        // 量尺（顯示座標模式下）-----------------------------------------------------------------------------------------
        private Point? _rulerStart, _rulerMouse;  // 圖片座標
        private bool ShowLocOn => cmsShowLoc.ForeColor == Color.Red;
        private void InitRuler()
        {
            // 顯示座標開著時，左鍵點一下設定起點，之後提示改成與起點的距離；再點一下換新起點，Backspace 清除
            picwinPicture_pic.MouseDown += (s, e) =>
            {
                if (!ShowLocOn || e.Button != MouseButtons.Left || _drawTool != DrawTool.None || _pickerOn || _dragWindowMode) return;
                Point? point = picwinPicture_pic.ToImagePoint(e.Location);
                if (!point.HasValue) return;
                _rulerStart = _rulerMouse = point;
                picwinPicture_pic.Invalidate();
                FlashCaption($"量尺起點 ({point.Value.X}, {point.Value.Y})：移動滑鼠看距離，再點一下換起點，Backspace 清除", 2500);
            };
            picwinPicture_pic.MouseMove += (s, e) =>
            {
                if (_rulerStart == null || !ShowLocOn) return;
                _rulerMouse = picwinPicture_pic.ToImagePoint(e.Location);
                picwinPicture_pic.Invalidate();
            };
            picwinPicture_pic.PaintOverlay += PaintRuler;
        }  // 建立量尺
        private void ClearRuler()
        {
            if (_rulerStart == null) return;
            _rulerStart = _rulerMouse = null;
            picwinPicture_pic.Invalidate();
            FlashCaption("清除量尺起點", 1000);
        }  // 清除量尺
        private string RulerText(Point point)
        {
            // 座標提示：有起點時加上距離（Δx、Δy 含起點與終點的像素數 = 差值 + 1，方便量線寬與窗口大小）
            if (_rulerStart == null) return $"X: {point.X}, Y: {point.Y}";
            Point start = _rulerStart.Value;
            int dx = point.X - start.X, dy = point.Y - start.Y;
            return $"X: {point.X}, Y: {point.Y}\r\n距起點 ({start.X}, {start.Y})：Δx {dx}、Δy {dy}、直線 {Math.Sqrt(dx * dx + dy * dy):0.#} px\r\n範圍 {Math.Abs(dx) + 1} x {Math.Abs(dy) + 1} px";
        }  // 座標與距離文字
        private void PaintRuler(object sender, PaintEventArgs e)
        {
            // 圖片座標：虛線框標出起點到滑鼠的範圍，實線為直線距離；線寬固定 1 個螢幕像素
            if (_rulerStart == null || _rulerMouse == null || !ShowLocOn || !(picwinPicture_pic.Image is Image image)) return;
            Rectangle shown = picwinPicture_pic.GetImageRectangle();
            if (shown.Width <= 0) return;
            float penWidth = (float)image.Width / shown.Width;
            Point a = _rulerStart.Value, b = _rulerMouse.Value;
            Graphics g = e.Graphics;
            GraphicsState state = g.Save();
            g.SmoothingMode = SmoothingMode.None;
            using (var box = new Pen(Color.FromArgb(220, 255, 255, 0), penWidth) { DashStyle = DashStyle.Dash })
            using (var line = new Pen(Color.FromArgb(255, 255, 0, 255), penWidth))
            {
                // 像素中心對中心
                g.DrawRectangle(box, Math.Min(a.X, b.X) + 0.5f, Math.Min(a.Y, b.Y) + 0.5f, Math.Abs(b.X - a.X), Math.Abs(b.Y - a.Y));
                g.DrawLine(line, a.X + 0.5f, a.Y + 0.5f, b.X + 0.5f, b.Y + 0.5f);
                float mark = penWidth * 6;
                g.DrawLine(line, a.X + 0.5f - mark, a.Y + 0.5f, a.X + 0.5f + mark, a.Y + 0.5f);
                g.DrawLine(line, a.X + 0.5f, a.Y + 0.5f - mark, a.X + 0.5f, a.Y + 0.5f + mark);
            }
            g.Restore(state);
        }  // 畫量尺
        // 燒機 / 殘影計時------------------------------------------------------------------------------------------------
        public static int BurnMinutes;        // 0 = 不限時間
        public static int BurnAction;         // 時間到：0 只提示、1 切到 50% 灰觀察殘影、2 關閉全螢幕
        private static readonly string[] BurnActions = { "只提示", "切到 50% 灰（觀察殘影）", "關閉全螢幕" };
        private readonly Timer _burnTimer = new Timer { Interval = 1000 };
        private DateTime _burnStart;
        private bool _burnObserving, _burnDone;  // 殘影觀察中、時間到（只提示）
        private bool BurnActive => _burnTimer.Enabled || _burnDone;
        private static readonly Font BurnFont = new Font("Microsoft JhengHei UI", 16F, FontStyle.Bold);
        private ToolStripMenuItem cmsBurn;
        private void InitBurnTimer()
        {
            cmsBurn = new ToolStripMenuItem("燒機 / 殘影計時…", null, (s, e) => ShowBurnDialog()) { ShortcutKeyDisplayString = "K" };
            contextMenuStrip.Items.Insert(contextMenuStrip.Items.IndexOf(cmsDragWindow) + 1, cmsBurn);
            contextMenuStrip.Opening += (s, e) => cmsBurn.Checked = BurnActive;
            _burnTimer.Tick += (s, e) => BurnTick();
            picwinPicture_pic.PaintScreenOverlay += PaintBurnClock;
            FormClosed += (s, e) => _burnTimer.Dispose();
        }  // 建立燒機計時
        private void ToggleBurn()
        {
            // K：依目前設定開始，計時中再按一次停止
            if (BurnActive) StopBurn();
            else StartBurn();
        }  // 開始 / 停止計時
        private void StartBurn()
        {
            _burnStart = DateTime.Now;
            _burnObserving = _burnDone = false;
            _burnTimer.Start();
            picwinPicture_pic.Invalidate();
            FlashCaption(BurnMinutes > 0 ? $"開始燒機計時：{BurnMinutes} 分鐘後{BurnActions[BurnAction]}" : "開始燒機計時（不限時間），K 停止", 2500);
        }  // 開始計時
        private void StopBurn()
        {
            _burnTimer.Stop();
            _burnObserving = _burnDone = false;
            picwinPicture_pic.Invalidate();
            FlashCaption("停止燒機計時", 1000);
        }  // 停止計時
        private void BurnTick()
        {
            TimeSpan elapsed = DateTime.Now - _burnStart;
            if (!_burnObserving && BurnMinutes > 0 && elapsed.TotalMinutes >= BurnMinutes)
            {
                switch (BurnAction)
                {
                    case 1:
                        // 切到 50% 灰，從頭計算殘影觀察時間
                        ShowGrayForAfterImage();
                        _burnObserving = true;
                        _burnStart = DateTime.Now;
                        FlashCaption($"燒機 {BurnMinutes} 分鐘時間到，切到 50% 灰觀察殘影", 3000);
                        break;
                    case 2:
                        _burnTimer.Stop();
                        Close();
                        return;
                    default:
                        _burnTimer.Stop();
                        _burnDone = true;
                        FlashCaption($"燒機 {BurnMinutes} 分鐘時間到", 5000);
                        break;
                }
            }
            picwinPicture_pic.Invalidate();
        }  // 每秒更新
        private void ShowGrayForAfterImage()
        {
            if (!(picwinPicture_pic.Image is Image image)) return;
            var gray = new Bitmap(image.Width, image.Height);
            using (Graphics g = Graphics.FromImage(gray)) g.Clear(Color.FromArgb(128, 128, 128));
            Bitmap old = _bitmap;
            _bitmap = gray;
            picwinPicture_pic.Image = gray;
            if (old != null && !ReferenceEquals(old, gray)) old.Dispose();
        }  // 顯示 50% 灰
        private static string FormatElapsed(TimeSpan time) => $"{(int)time.TotalHours}:{time.Minutes:00}:{time.Seconds:00}";
        private void PaintBurnClock(object sender, PaintEventArgs e)
        {
            // 右上角：燒機經過時間 / 設定時間，或殘影觀察時間
            if (!BurnActive) return;
            TimeSpan elapsed = (_burnDone ? TimeSpan.FromMinutes(BurnMinutes) : DateTime.Now - _burnStart);
            string text = _burnObserving ? $"殘影觀察 {FormatElapsed(elapsed)}"
                : $"燒機 {FormatElapsed(elapsed)}{(BurnMinutes > 0 ? $" / {FormatElapsed(TimeSpan.FromMinutes(BurnMinutes))}" : "")}{(_burnDone ? " 時間到" : "")}";
            Size size = TextRenderer.MeasureText(text, BurnFont);
            var box = new Rectangle(picwinPicture_pic.ClientSize.Width - size.Width - 28, 12, size.Width + 16, size.Height + 8);
            using (Brush background = new SolidBrush(Color.FromArgb(190, 20, 20, 20))) e.Graphics.FillRectangle(background, box);
            TextRenderer.DrawText(e.Graphics, text, BurnFont, box, _burnDone ? Color.Gold : Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }  // 畫計時
        private void ShowBurnDialog()
        {
            // 設定時間與時間到的動作，按「開始」依新設定重新計時
            using (var form = new Form
            {
                Text = "燒機 / 殘影計時", FormBorderStyle = FormBorderStyle.FixedDialog, StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false, MaximizeBox = false, ShowInTaskbar = false, ClientSize = new Size(330, 130), TopMost = true,
            })
            {
                var minutes = new NumericUpDown { Minimum = 0, Maximum = 100000, Value = BurnMinutes, Bounds = new Rectangle(110, 14, 80, 23) };
                var action = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Bounds = new Rectangle(110, 46, 200, 23) };
                action.Items.AddRange(BurnActions);
                action.SelectedIndex = BurnAction;
                var start = new Button { Text = "開始", DialogResult = DialogResult.OK, Bounds = new Rectangle(70, 90, 76, 26) };
                var stop = new Button { Text = "停止", DialogResult = DialogResult.Abort, Bounds = new Rectangle(152, 90, 76, 26), Enabled = BurnActive };
                var cancel = new Button { Text = "取消", DialogResult = DialogResult.Cancel, Bounds = new Rectangle(234, 90, 76, 26) };
                form.Controls.AddRange(new Control[]
                {
                    new Label { Text = "時間（分鐘）", AutoSize = true, Location = new Point(14, 18) }, minutes,
                    new Label { Text = "0 = 不限", AutoSize = true, Location = new Point(196, 18), ForeColor = Color.DimGray },
                    new Label { Text = "時間到之後", AutoSize = true, Location = new Point(14, 50) }, action, start, stop, cancel,
                });
                form.AcceptButton = start;
                form.CancelButton = cancel;
                mainWindow.Localize(form);
                DialogResult result = form.ShowDialog(this);
                if (result == DialogResult.Abort) StopBurn();
                if (result != DialogResult.OK) return;
                BurnMinutes = (int)minutes.Value;
                BurnAction = action.SelectedIndex;
                StartBurn();
            }
        }  // 計時設定
        // 閃爍（Clipping 截斷條等）：目前的圖與另一張每 0.5 秒交替---------------------------------------------------------
        private Bitmap _blinkImage;
        private bool _blinkShowingAlternate;
        private Timer _blinkTimer;
        public void SetBlink(Bitmap alternate)
        {
            // alternate 為 null 時停止閃爍並恢復原圖
            _blinkTimer?.Stop();
            if (_blinkShowingAlternate && _bitmap != null) picwinPicture_pic.Image = _bitmap;
            _blinkShowingAlternate = false;
            _blinkImage?.Dispose();
            _blinkImage = null;
            if (alternate == null) return;
            _blinkImage = new Bitmap(alternate);
            if (_blinkTimer == null)
            {
                _blinkTimer = new Timer { Interval = 500 };
                _blinkTimer.Tick += (s, e) => ToggleBlink();
                FormClosed += (s, e) => { _blinkTimer.Dispose(); _blinkImage?.Dispose(); };
            }
            _blinkTimer.Start();
        }  // 設定閃爍
        private void ToggleBlink()
        {
            // 畫面被其他功能換掉（反相、比對、畫圖存檔…）時停止閃爍
            Image shown = picwinPicture_pic.Image;
            if (_blinkImage == null || (!ReferenceEquals(shown, _bitmap) && !ReferenceEquals(shown, _blinkImage)))
            {
                _blinkTimer.Stop();
                return;
            }
            _blinkShowingAlternate = !_blinkShowingAlternate;
            picwinPicture_pic.Image = _blinkShowingAlternate ? _blinkImage : _bitmap;
        }  // 切換閃爍
        // 拖曳窗口時以滾輪調整大小------------------------------------------------------------------------------------------
        private void ResizeWindowPatternBy(int percent)
        {
            if (!(picwinPicture_pic.Image is Image image) || mainWindow.Instance == null) return;
            int width = image.Width, height = image.Height;  // 舊圖等一下會釋放，先記下尺寸
            Bitmap resized = mainWindow.Instance.ResizeWindowPattern(percent, width, height);
            if (resized == null) return;
            Bitmap old = _bitmap;
            _bitmap = resized;
            picwinPicture_pic.Image = resized;
            old?.Dispose();
            Rectangle window = mainWindow.Instance.WindowPatternBounds(width, height);
            FlashCaption($"窗口 {mainWindow.Instance.WindowPatternPercent}%（{window.Width} x {window.Height}，X {window.X}, Y {window.Y}）", 1500);
        }  // 調整窗口大小並更新畫面
    }
}
