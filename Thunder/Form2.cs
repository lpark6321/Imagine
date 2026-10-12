using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace Thunder
{
    public partial class pictureWindow : Form
    {
        //全局變數-----------------------------------------------------------------------------------------------------
        private Bitmap _bitmap;
        //pictureWindow類別--------------------------------------------------------------------------------------------
        protected override void CreateHandle()
        {
            // 自動調整縮放：視窗與子控制項都建立成個別螢幕 DPI 感知，Windows 不再放大畫面，圖樣 1:1 顯示
            using (mainWindow.FullScreenDpiScope())
            {
                base.CreateHandle();
                CreateChildHandles(this);
            }
        }  // 建立視窗
        private static void CreateChildHandles(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                _ = child.Handle;  // 在同一個 DPI 範圍內建立
                CreateChildHandles(child);
            }
        }  // 建立子控制項
        public pictureWindow()
        {
            InitializeComponent();
            InitScaleMenu();
            InitDrawMenu();
            InitSlideshowMenu();
            InitCaption();
            InitLens();
            InitAutoHideCursor();
            InitGrayScanMenu();
            InitDragWindow();
            InitRuler();
            InitBurnTimer();
            picwinPicture_pic.PaintScreenOverlay += picwinPicture_PaintInfo;  // I 鍵資訊列
            picwinPicture_pic.PaintOverlay += picwinPicture_PaintGrid;        // G 鍵格線（圖片座標）
            picwinPicture_pic.PaintScreenOverlay += picwinPicture_PaintGridLabels;
            FormClosed += (s, e) => { _previousBitmap?.Dispose(); _previousBitmap = null; StopRefreshMeter(); };
            // 全螢幕顯示時防止螢幕關閉 / 休眠（由主視窗統一判斷）
            VisibleChanged += (s, e) => mainWindow.Instance?.UpdateKeepAwake();
            FormClosed += (s, e) => mainWindow.Instance?.UpdateKeepAwake();
            MouseWheel += pictureWindow_MouseWheel;  // PictureBox 不會取得焦點，滾輪事件由視窗接收
            Load += (s, e) => mainWindow.Localize(this);  // 英文介面
            //_bitmap = bitMap; // 將接收到的 Bitmap 儲存到類別屬性中
            //this.Paint += Form2_show; // 綁定 Paint 事件  
        }
        private void pictureWindow_Load(object sender, EventArgs e)
        {
            Screenlist(sender, e);
        }
        private void Form2_show(object sender, PaintEventArgs e)
        {
            if (_bitmap != null)
            {
                e.Graphics.DrawImage(_bitmap, 0, 0); // 將 Bitmap 繪製到 Form2 上
            }
        }
        public void setBitmap(Bitmap bitmap)
        {
            SetBlink(null);    // 換圖時停止閃爍（呼叫端需要時再設定）
            KeepAsPrevious();  // 換圖前的那張留給 B 鍵比對
            _inverted = false;
            _bitmap = (Bitmap)bitmap.Clone();
            picwinPicture_pic.Image = _bitmap;

        }  //設定_bitmap
        //pictureWindow操控--------------------------------------------------------------------------------------------
        private void pictureWindow_DoubleClick(object sender, EventArgs e)
        {
            if (sender is PictureBox && (_drawTool != DrawTool.None || _pickerOn || _dragWindowMode || ShowLocOn))
            {
                return;  // 畫圖或吸管時雙擊不切換全螢幕
            }
            if (WindowState == FormWindowState.Maximized)
            {
                // 退出全螢幕，並將尺寸設為所在螢幕的 1/4 大小
                FormBorderStyle = FormBorderStyle.SizableToolWindow;
                WindowState = FormWindowState.Normal;

                // 計算所在螢幕的尺寸
                Screen currentScreen = Screen.FromControl(this);
                int newWidth = currentScreen.WorkingArea.Width / 3;
                int newHeight = currentScreen.WorkingArea.Height / 3;

                // 設定 Form 的大小和位置
                Width = newWidth;
                Height = newHeight;
                //Size = new Size(newWidth, newHeight);
                Location = new Point(
                    currentScreen.WorkingArea.Left + (currentScreen.WorkingArea.Width - newWidth) / 3,
                    currentScreen.WorkingArea.Top + (currentScreen.WorkingArea.Height - newHeight) / 3
                );
            }
            else
            {
                // 進入全螢幕，去除邊框
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
                Width = Screen.FromControl(this).WorkingArea.Width;
                Height = Screen.FromControl(this).WorkingArea.Height;
                //Size = new Size(Screen.FromControl(this).WorkingArea.Width, Screen.FromControl(this).WorkingArea.Height);
            }
        }  //雙擊全螢幕
        private void cmsShowLoc_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem && menuItem.Owner is ContextMenuStrip contextMenu)
            {
                if (cmsShowLoc.ForeColor != Color.Red)
                {
                    // 取消訂閱 PictureBox 的 MouseMove 事件  
                    picwinPicture_pic.MouseMove += PictureBox_MouseMove;
                    cmsShowLoc.ForeColor = Color.Red;
                    Cursor = Cursors.Cross; // 將游標設為 Cross
                }
                else
                {
                    // 訂閱 PictureBox 的 MouseMove 事件  
                    picwinPicture_pic.MouseMove -= PictureBox_MouseMove;
                    cmsShowLoc.ForeColor = Color.Black;
                    _rulerStart = _rulerMouse = null;  // 關閉座標時一併清除量尺
                    picwinPicture_pic.Invalidate();
                    Cursor = _drawTool != DrawTool.None ? Cursors.Cross : Cursors.Hand; // 恢復游標為預設
                }
            }
        }  //顯示座標
        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
                //MessageBox.Show("");
            if (sender is ScalablePictureBox targerBox)
            {
                // 更新 ToolTip 的內容為圖片像素座標（已換算縮放）
                Point? p = targerBox.ToImagePoint(e.Location);
                toolTip.SetToolTip(targerBox, p.HasValue ? RulerText(p.Value) : "圖片範圍外");  // 有量尺起點時加上距離
            }
        }  //滑鼠移動顯示座標 tooltip
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // 輸入法攔截的按鍵（空白、=、-、WASD 等）在這裡還原處理，並回傳 true 讓輸入法收不到
            if ((keyData & Keys.KeyCode) == Keys.ProcessKey)
            {
                Keys realKey = mainWindow.GetImeOriginalKey(ref msg);
                if (realKey != Keys.None)
                {
                    pictureWindow_KeyDown(this, new KeyEventArgs(realKey | ModifierKeys));
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }  //處理被輸入法攔截的按鍵
        private int _wheelDelta;  // 累積滾動量，觸控板的小幅捲動湊滿一格才切圖
        private void pictureWindow_MouseWheel(object sender, MouseEventArgs e)
        {
            // 與方向鍵相同：往下滾 = 下一張（→），往上滾 = 上一張（←）；拖曳窗口時 Ctrl+滾輪調整窗口大小
            _wheelDelta += e.Delta;
            while (Math.Abs(_wheelDelta) >= SystemInformation.MouseWheelScrollDelta)
            {
                bool previous = _wheelDelta > 0;
                _wheelDelta -= Math.Sign(_wheelDelta) * SystemInformation.MouseWheelScrollDelta;
                if (_dragWindowMode && ModifierKeys.HasFlag(Keys.Control)) ResizeWindowPatternBy((previous ? 1 : -1) * (ModifierKeys.HasFlag(Keys.Shift) ? 1 : 5));  // Ctrl+滾輪調整窗口大小（往上放大），再按 Shift 微調 1%
                else pictureWindow_KeyDown(this, new KeyEventArgs(previous ? Keys.Left : Keys.Right));
            }
        }  //滾輪切圖
        private void pictureWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z)
            {
                UndoDrawing();
                return;
            }
            if (e.KeyCode == Keys.Back && _rulerStart != null)
            {
                ClearRuler();  // Backspace 清除量尺起點
                return;
            }
            if (e.KeyCode == Keys.K && e.Modifiers == Keys.None)
            {
                ToggleBurn();  // K 開始 / 停止燒機計時
                return;
            }
            if (e.Alt && e.KeyCode == Keys.R)
            {
                ToggleGrayScan();  // Alt+R 自動掃灰階
                e.SuppressKeyPress = true;
                return;
            }
            if (_dragWindowMode && (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Up || e.KeyCode == Keys.Down))
            {
                NudgeWindowPattern(e);  // 拖曳窗口模式：方向鍵移動窗口（Shift 一次 10px）
                return;
            }
            if (HandleLevelInput(e))
            {
                return;  // 數字鍵輸入階數（輸入中 Esc 只取消輸入，不關閉視窗）
            }
            if (sender is Form targetform)
            {
                Point currentPosition = Cursor.Position;
                switch (e.KeyCode)
                {
                    case Keys.F1:
                        //MessageBox.Show("F1 鍵被按下");
                        pictureWindow_DoubleClick(sender, e);
                        break;
                    case Keys.Escape:
                        //MessageBox.Show("ESC 鍵被按下");
                        Close();
                        break;
                    case Keys.Right:
                        //MessageBox.Show("Right 鍵被按下");
                        if (mainWindow.Instance != null)
                        {
                            TabControl tabControl = mainWindow.Instance.MainTabControl;
                            //切換到 DirList 分頁 (index = 1)
                            if (tabControl.SelectedIndex == 1)
                            {
                                // 確保 DirList 分頁下有 ListView 並切換選項  
                                if (tabControl.TabPages[1].Controls["tabdlPatternList_lvw"] is System.Windows.Forms.ListView
                                        listView && listView.Items.Count > 0)
                                {
                                    int currentIndex = listView.SelectedIndices.Count > 0
                                        ? listView.SelectedIndices[0]
                                        : -1;
                                    //MessageBox.Show($"{currentIndex}");
                                    if (currentIndex != -1)
                                    {
                                        listView.Items[currentIndex].Selected = false;
                                        listView.Items[currentIndex].Focused = false;
                                    }

                                    int nextIndex = (currentIndex + 1) % listView.Items.Count;
                                    listView.Items[nextIndex].Selected = true;
                                    listView.Items[nextIndex].Focused = true;
                                    mainWindow.Instance.FullScreen(sender, e);
                                }

                            }
                            else if (tabControl.SelectedIndex == 2) //切換到 ImgList 分頁 (index = 2)
                            {
                                mainWindow.Instance.SelectImgListRelative(+1); // 選擇下一個項目
                                mainWindow.Instance.FullScreen(sender, e);
                            }
                        }
                        break;
                    case Keys.Left:
                        //MessageBox.Show("Left 鍵被按下");
                        if (mainWindow.Instance != null)
                        {
                            TabControl tabControl = mainWindow.Instance.MainTabControl;
                            // 切換到 DirList 分頁 (index = 1)  
                            if (tabControl.SelectedIndex == 1)
                            {
                                // 確保 DirList 分頁下有 ListView 並切換選項  
                                if (tabControl.TabPages[1].Controls["tabdlPatternList_lvw"] is System.Windows.Forms.ListView
                                        listView && listView.Items.Count > 0)
                                {
                                    int currentIndex = listView.SelectedIndices.Count > 0
                                        ? listView.SelectedIndices[0]
                                        : -1;
                                    if (currentIndex == -1)
                                    {
                                        currentIndex = 0;  // 沒有選取時從第一張往前，即最後一張
                                    }
                                    else
                                    {
                                        listView.Items[currentIndex].Selected = false;
                                        listView.Items[currentIndex].Focused = false;
                                    }

                                    int prevIndex = (currentIndex - 1 + listView.Items.Count) % listView.Items.Count;
                                    listView.Items[prevIndex].Selected = true;
                                    listView.Items[prevIndex].Focused = true;
                                    mainWindow.Instance.FullScreen(sender, e);
                                }
                            }
                            else if (tabControl.SelectedIndex == 2) //切換到 ImgList 分頁 (index = 2)
                            {
                                mainWindow.Instance.SelectImgListRelative(-1); // 選擇上一個項目
                                mainWindow.Instance.FullScreen(sender, e);
                            }
                        }
                        break;
                    case Keys.W:
                        //MessageBox.Show("W 鍵被按下");
                        // 使用 Point 結構來修改游標位置  
                        Cursor.Position = new Point(currentPosition.X, currentPosition.Y - 1);
                        break;
                    case Keys.A:
                        //MessageBox.Show("A 鍵被按下");
                        // 使用 Point 結構來修改游標位置  
                        Cursor.Position = new Point(currentPosition.X - 1, currentPosition.Y);
                        break;
                    case Keys.S:
                        //MessageBox.Show("S 鍵被按下");
                        // 使用 Point 結構來修改游標位置  
                        Cursor.Position = new Point(currentPosition.X, currentPosition.Y + 1);
                        break;
                    case Keys.D:
                        //MessageBox.Show("D 鍵被按下");
                        // 使用 Point 結構來修改游標位置  
                        Cursor.Position = new Point(currentPosition.X + 1, currentPosition.Y);
                        break;
                    case Keys.M:
                        SetMagnifier(!_magnifierOn);
                        break;
                    case Keys.I:
                        ToggleInfo();
                        break;
                    case Keys.D1:
                    case Keys.D2:
                    case Keys.D3:
                    case Keys.D4:
                        SetChannel(e.KeyCode - Keys.D0);  // 1 R、2 G、3 B、4 灰階
                        break;
                    case Keys.B:
                        ToggleCompare();
                        break;
                    case Keys.G:
                        ToggleGrid();
                        break;
                    case Keys.Q:
                        SetDragWindowMode(!_dragWindowMode);  // 拖曳窗口
                        break;
                    case Keys.N:
                        ToggleInvert();  // 反相（負片）
                        break;
                    case Keys.R:
                        // 每 1 秒幻燈片：播放中按 R 停止
                        _scanTimer?.Stop();  // 不和自動掃灰階同時跑
                        if (timerSlideshow.Enabled) timerSlideshow.Stop();
                        else Slideshow(cmsSlideshow1, EventArgs.Empty);
                        FlashCaption(timerSlideshow.Enabled ? "幻燈片 每 1 秒" : "幻燈片 停止");
                        break;
                    case Keys.T:
                    case Keys.E:
                        // 幻燈片播放中：T 間隔加 1 秒、E 減 1 秒（最少 1 秒）
                        if (timerSlideshow.Enabled)
                        {
                            int interval = _slideInterval + (e.KeyCode == Keys.T ? 1000 : -1000);
                            _slideInterval = Math.Max(Math.Min(1000, _slideInterval), interval);
                            if (timerSlideshow.Interval != CurrentSlideInterval()) timerSlideshow.Interval = CurrentSlideInterval();  // 這張有自己的停留時間時不變
                            FlashCaption($"幻燈片 每 {_slideInterval / 1000.0:0.#} 秒");
                        }
                        break;
                    case Keys.Up:
                        AdjustSolidLevel(+1);
                        break;
                    case Keys.Down:
                        AdjustSolidLevel(-1);
                        break;
                    case Keys.PageUp:
                        AdjustSolidLevel(+16);
                        break;
                    case Keys.PageDown:
                        AdjustSolidLevel(-16);
                        break;
                    case Keys.C:
                        if (!e.Control) SetPicker(!_pickerOn);
                        break;
                    case Keys.Space:
                        if (mainWindow.Instance == null || !mainWindow.Instance.IsDynamicMode)
                        {
                            // 非 Dynamic 模式：等同 Esc
                            Close();
                        }
                        else if (mainWindow.Instance.timerDynamic.Enabled)
                        {
                            mainWindow.Instance.timerDynamic.Stop();
                            mainWindow.Instance.timerDynamic.Tag = "stop";
                        }
                        else
                        {
                            mainWindow.Instance.timerDynamic.Start();
                            mainWindow.Instance.timerDynamic.Tag = "run";
                        }
                        break;
                    case Keys.Add:      // 數字鍵盤 +
                    case Keys.Oemplus:  // 主鍵盤 =
                        // 切換螢幕（主視窗會依分頁重新產生圖片），並把全螢幕移到新螢幕
                        mainWindow.Instance.SwitchScreen(+1);
                        mainWindow.Instance.FullScreen(sender, e);
                        break;
                    case Keys.Subtract: // 數字鍵盤 -
                    case Keys.OemMinus: // 主鍵盤 -
                        mainWindow.Instance.SwitchScreen(-1);
                        mainWindow.Instance.FullScreen(sender, e);
                        break;
                }
               
            }
        }  //鍵盤事件
        //右鍵選單-----------------------------------------------------------------------------------------------------
        private void contextMenuStrip_Opened(object sender, EventArgs e)
        {
            if (picwinPicture_pic.ToImagePoint(picwinPicture_pic.PointToClient(Cursor.Position)) is Point mousePosition)
            {
                cmsShowLoc.Text = $"切換顯示座標_X: {mousePosition.X}, Y: {mousePosition.Y}";
            }
            else
            {
                cmsShowLoc.Text = "顯示座標(無法取得座標)";
            }
        }  //右鍵選單顯示座標
        private void Screenlist(object sender, EventArgs e)
        {
            // 獲取所有螢幕
            Screen[] screens = Screen.AllScreens;
            // 清空下拉選單
            cmsScreenlist.Items.Clear();
            // 添加螢幕選項到下拉選單
            foreach (Screen screen in screens)
            {
                cmsScreenlist.Items.Add(screen.DeviceName);
            }
        }    //獲取所有螢幕
        private void cmsScreenlist_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            mainWindow.Instance.mnsScreenlist_cmb.SelectedText = cmsScreenlist.SelectedItem?.ToString();
            //// 獲取選擇的螢幕  
            string selectedScreen = cmsScreenlist.SelectedItem?.ToString() ?? Screen.AllScreens[0].DeviceName;
            Screen targetScreen = Screen.AllScreens.FirstOrDefault(screen => screen.DeviceName == selectedScreen) ??
                                  Screen.AllScreens[0];
            mainWindow.Instance.mnsScreenlist_cmb.SelectedItem = selectedScreen;  // 觸發主視窗更新寬高並依分頁重新產生圖片
            mainWindow.Instance.FullScreen(sender, e);

            //MessageBox.Show(selectedScreen);


            // 設置窗口位置和大小
            //StartPosition = FormStartPosition.Manual;
            //Location = targetScreen.Bounds.Location;
            //Size = targetScreen.Bounds.Size;
            //WindowState = FormWindowState.Minimized;

            //// 切換到全螢幕模式
            //FormBorderStyle = FormBorderStyle.None;
            //WindowState = FormWindowState.Maximized;
            ////pictureWindow_DoubleClick(sender, e);
            

            contextMenuStrip.Visible = false;
        }   //選擇螢幕
        private void cmsAdjust_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem)
            {
                if (menuItem.Name == "cmsAdjustRotate90")
                {
                    _bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                }
                else if (menuItem.Name == "cmsAdjustRotate180")
                {
                    _bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                }
                else if (menuItem.Name == "cmsAdjustRotate270")
                {
                    _bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                }
                else if (menuItem.Name == "cmsAdjustHFlip")
                {
                    _bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                }
                else if (menuItem.Name == "cmsAdjustVFlip")
                {
                    _bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                }

                Invalidate(); // 重新繪製窗口以顯示更新的圖片
                picwinPicture_pic.Image = _bitmap; // 更新 PictureBox 的圖片
            }
        }  //調整圖片
        private void cmsToBeContinued_Click(object sender, EventArgs e)
        {
            Close();
        }  //關閉視窗
        //開始幻燈片放映-----------------------------------------------------------------------------------------------
        private void Slideshow(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem)
            {
                if (menuItem.Name == "cmsSlideshowStop")
                {
                    // 停止幻燈片放映  
                    timerSlideshow.Stop();
                }
                else
                {
                    int interval;  // 毫秒
                    if (menuItem == cmsSlideshowTextButton)
                    {
                        if (!int.TryParse(cmsSlideshowText.Text, out interval) || interval <= 0)
                        {
                            MessageBox.Show(this, "請在上方輸入大於 0 的毫秒數", "幻燈片");
                            return;
                        }
                    }
                    else if (menuItem.Tag is int milliseconds)
                    {
                        interval = milliseconds;  // 各秒數選項的間隔存在 Tag（InitSlideshowMenu）
                    }
                    else
                    {
                        return;
                    }
                    // 開始幻燈片放映
                    _slideInterval = interval;
                    timerSlideshow.Interval = CurrentSlideInterval();
                    timerSlideshow.Tag = "KeyRight";
                    timerSlideshow.Start();
                }
            }
        }  //開始幻燈片放映
        private int _slideInterval = 1000;  // 全域間隔（毫秒）；ImgList 那行有寫 @秒數s 時改用那行的停留時間
        private int CurrentSlideInterval() => mainWindow.Instance?.SelectedSlideDwell() ?? _slideInterval;  // 目前這張要停留多久
        //左上角圖片名稱------------------------------------------------------------------------------------------
        private static readonly Font CaptionFont = new Font("Microsoft JhengHei", 36F, FontStyle.Bold);
        private static readonly Rectangle CaptionTriggerArea = new Rectangle(0, 0, 200, 100);  // 滑鼠在此範圍內才顯示
        private string _caption;
        private bool _captionVisible;
        private Rectangle _captionBounds;  // 上次畫名稱的範圍，用來局部重繪
        public string Caption
        {
            get => _caption;
            set
            {
                if (_caption == value) return;
                _caption = value;
                if (!timerSlideshow.Enabled) FlashCaption(null);  // 開啟全螢幕或換圖時顯示 1 秒（幻燈片播放中不顯示，避免一直蓋著）
                InvalidateCaption();
            }
        }  // 目前圖片名稱（DirList 檔名 / ImgList 名稱；null 不顯示）
        public string ScreenDevice { get; set; }  // 多螢幕模式：這個視窗負責的螢幕
        public object Selection { get; set; }     // 分離模式：這個螢幕目前顯示的分頁與項目（由主視窗記錄 / 還原）
        public void FlashMessage(string message) => FlashCaption(message);
        private Timer _flashTimer;
        private string _flashText;  // 暫時顯示的訊息（灰階、幻燈片間隔）；null 時顯示名稱
        private bool _flashing;
        private void FlashCaption(string message, int milliseconds = 1000)
        {
            // 左上角顯示一段時間（預設 1 秒）後自動隱藏；message 為 null 時顯示圖片名稱
            if (message == null && string.IsNullOrEmpty(_caption)) return;
            if (_flashTimer == null)
            {
                _flashTimer = new Timer();
                _flashTimer.Tick += (s, e) => { _flashTimer.Stop(); _flashing = false; _flashText = null; InvalidateCaption(); };
            }
            _flashText = message;
            _flashing = true;
            _flashTimer.Stop();
            _flashTimer.Interval = milliseconds;
            _flashTimer.Start();
            InvalidateCaption();
        }  //左上角短暫顯示
        private void InvalidateCaption()
        {
            // 名稱長度會變，重繪整條頂端
            picwinPicture_pic.Invalidate(new Rectangle(0, 0, picwinPicture_pic.Width, Math.Max(_captionBounds.Height, CaptionTriggerArea.Height)));
        }  //重繪名稱區域
        private void InitCaption()
        {
            picwinPicture_pic.PaintScreenOverlay += picwinPicture_PaintCaption;
            picwinPicture_pic.MouseMove += (s, e) => SetCaptionVisible(CaptionTriggerArea.Contains(e.Location));
            picwinPicture_pic.MouseLeave += (s, e) => SetCaptionVisible(false);
        }  //建立左上角名稱
        private void SetCaptionVisible(bool visible)
        {
            if (_captionVisible == visible) return;
            _captionVisible = visible;
            InvalidateCaption();
        }  //顯示 / 隱藏名稱
        private void picwinPicture_PaintCaption(object sender, PaintEventArgs e)
        {
            string text = _flashing && _flashText != null ? _flashText : _caption;
            if (!(_captionVisible || _flashing) || string.IsNullOrEmpty(text)) return;
            // 127 灰底 50% 透明；黑色粗體字加白框（用路徑描邊再填色）
            Graphics g = e.Graphics;
            float emSize = CaptionFont.SizeInPoints * g.DpiY / 72f;  // 路徑以像素為單位
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddString(text, CaptionFont.FontFamily, (int)CaptionFont.Style, emSize, new PointF(8, 4), StringFormat.GenericTypographic);
                RectangleF textBounds = path.GetBounds();
                _captionBounds = new Rectangle(0, 0, (int)Math.Ceiling(textBounds.Right) + 10, (int)Math.Ceiling(textBounds.Bottom) + 10);
                using (Brush background = new SolidBrush(Color.FromArgb(128, 127, 127, 127)))
                {
                    g.FillRectangle(background, _captionBounds);
                }
                GraphicsState state = g.Save();
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (Pen outline = new Pen(Color.White, 4f) { LineJoin = LineJoin.Round })
                {
                    g.DrawPath(outline, path);
                }
                g.FillPath(Brushes.Black, path);
                g.Restore(state);
            }
        }  //畫左上角名稱
        private void InitSlideshowMenu()
        {
            // 間隔（毫秒）存在 Tag，不再從名稱解析秒數
            cmsSlideshow1.Tag = 1000;
            cmsSlideshow1.ShortcutKeyDisplayString = "R";  // R 開始 / 停止每 1 秒幻燈片
            cmsSlideshow3.Tag = 3000;
            cmsSlideshow5.Tag = 5000;
            cmsSlideshow10.Tag = 10000;
            var halfSecond = new ToolStripMenuItem("每0.5秒", null, Slideshow) { Name = "cmsSlideshow05", Tag = 500 };
            var twoSeconds = new ToolStripMenuItem("每2秒", null, Slideshow) { Name = "cmsSlideshow2", Tag = 2000 };
            cmsSlideshow.DropDownItems.Insert(cmsSlideshow.DropDownItems.IndexOf(cmsSlideshow1), halfSecond);
            cmsSlideshow.DropDownItems.Insert(cmsSlideshow.DropDownItems.IndexOf(cmsSlideshow1) + 1, twoSeconds);
            // 隨機順序：每輪打亂、不重複（所有全螢幕視窗共用這個設定）
            var random = new ToolStripMenuItem("隨機順序") { CheckOnClick = true, ToolTipText = "幻燈片每一輪打亂順序，同一輪不重複" };
            random.CheckedChanged += (s, e) => SlideRandom = random.Checked;
            cmsSlideshow.DropDownOpening += (s, e) => random.Checked = SlideRandom;
            cmsSlideshow.DropDownItems.AddRange(new ToolStripItem[] { new ToolStripSeparator(), random });
        }  //幻燈片間隔選項
        public static bool SlideRandom;  // 幻燈片隨機順序
        private void timer_Tick(object sender, EventArgs e)
        {
            if (sender is Timer t && t.Tag is string st)
            {
                if (st == "KeyRight")
                {
                    // 模擬按下向右鍵；隨機順序時改由主視窗挑下一張
                    if (SlideRandom && mainWindow.Instance != null && mainWindow.Instance.ShowRandomSlide(this)) { }
                    else pictureWindow_KeyDown(this, new KeyEventArgs(Keys.Right));
                    if (t.Enabled && t.Interval != CurrentSlideInterval()) t.Interval = CurrentSlideInterval();  // 下一張有寫 @秒數s 時停留指定時間
                }
            }
        } //定時器事件
        //動態-------------------------------------------------------------------------------------------------
        public void UpdatePictureBox(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException(nameof(bitmap), "Bitmap 不能為 null。");
            }
            if (_blinkImage != null) SetBlink(null);  // 動畫播放時不閃爍

            if (picwinPicture_pic.Image != null)
            {
                if (ReferenceEquals(picwinPicture_pic.Image, _bitmap)) _bitmap = null;  // 已釋放，不能再留給 B 鍵比對
                picwinPicture_pic.Image.Dispose(); // 釋放舊的圖片資源
            }

            //picwinPicture_pic.Image = bitmap; // 更新 PictureBox 的圖片
            picwinPicture_pic.Image = (Bitmap)bitmap.Clone(); // 更新 PictureBox 的圖片
        }  //更新圖片
        public Bitmap getpicturebox()
        {
            if (picwinPicture_pic.Image != null)
            {
                return (Bitmap)picwinPicture_pic.Image.Clone();
            }
            else
            {
                return null;
            }
        }  //取得picturebox圖片
        private void pictureWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainWindow.Instance.timerDynamic.Stop();
            mainWindow.Instance.timerDynamic.Tag = "stop";
        }  //關閉視窗執行
        //縮放-------------------------------------------------------------------------------------------------
        private static ImageScaleMode _scaleMode = ImageScaleMode.Fit;  // 關閉視窗後仍保留選擇
        private static bool _smoothScaling = false;
        private ToolStripMenuItem cmsScale;
        private ToolStripMenuItem cmsScaleSmooth;
        private void InitScaleMenu()
        {
            cmsScale = new ToolStripMenuItem("縮放") { Name = "cmsScale" };
            var modes = new (ImageScaleMode mode, string text)[]
            {
                (ImageScaleMode.Fit, "符合畫面（等比例）"),
                (ImageScaleMode.Stretch, "拉伸填滿"),
                (ImageScaleMode.Fill, "填滿裁切（等比例）"),
                (ImageScaleMode.OneToOneCenter, "1:1 置中"),
                (ImageScaleMode.OneToOneTopLeft, "1:1 左上對齊"),
                (ImageScaleMode.Integer, "整數倍放大"),
            };
            foreach (var (mode, text) in modes)
            {
                cmsScale.DropDownItems.Add(new ToolStripMenuItem(text, null, cmsScale_Click) { Tag = mode });
            }
            cmsScale.DropDownItems.Add(new ToolStripSeparator());
            cmsScaleSmooth = new ToolStripMenuItem("平滑縮放（看照片用）", null, cmsScale_Click) { Name = "cmsScaleSmooth" };
            cmsScale.DropDownItems.Add(cmsScaleSmooth);
            contextMenuStrip.Items.Insert(contextMenuStrip.Items.IndexOf(cmsAdjust) + 1, cmsScale);
            ApplyScale();
        }  //建立縮放選單
        private void cmsScale_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem)
            {
                if (menuItem == cmsScaleSmooth)
                {
                    _smoothScaling = !_smoothScaling;
                }
                else if (menuItem.Tag is ImageScaleMode mode)
                {
                    _scaleMode = mode;
                }
                ApplyScale();
            }
        }  //選擇縮放方式
        private void ApplyScale()
        {
            picwinPicture_pic.ScaleMode = _scaleMode;
            picwinPicture_pic.Smooth = _smoothScaling;
            foreach (ToolStripItem item in cmsScale.DropDownItems)
            {
                if (item is ToolStripMenuItem mi && mi.Tag is ImageScaleMode mode)
                {
                    mi.Checked = mode == _scaleMode;
                }
            }
            cmsScaleSmooth.Checked = _smoothScaling;
        }  //套用縮放方式
        //畫圖-------------------------------------------------------------------------------------------------
        private const int MaxUndo = 10;
        private static readonly (string name, Color color)[] DrawColors =
        {
            ("黑", Color.Black), ("白", Color.White), ("紅", Color.Red), ("綠", Color.Lime),
            ("藍", Color.Blue), ("黃", Color.Yellow), ("紫", Color.Magenta), ("天藍", Color.Cyan),
        };
        private static Color _drawColor = Color.Red;  // 關閉視窗後仍保留顏色
        private DrawTool _drawTool = DrawTool.None;
        private List<DrawShape> _shapes = new List<DrawShape>();
        private readonly List<List<DrawShape>> _undoHistory = new List<List<DrawShape>>();  // 每次變更前的快照，最多 MaxUndo 筆
        private DrawShape _drawingShape;  // 拖曳中的預覽
        private ToolStripMenuItem cmsDraw;
        private ToolStripMenuItem cmsDrawColor;
        private ToolStripMenuItem cmsDrawUndo;
        private ToolStripMenuItem cmsDrawClear;
        private ToolStripMenuItem cmsDrawSave;
        private void InitDrawMenu()
        {
            cmsDraw = new ToolStripMenuItem("畫圖") { Name = "cmsDraw" };
            var tools = new (DrawTool tool, string text)[]
            {
                (DrawTool.None, "不畫圖"),
                (DrawTool.Ellipse, "圓形"),
                (DrawTool.Rectangle, "矩形"),
                (DrawTool.Arrow, "箭頭"),
            };
            foreach (var (tool, text) in tools)
            {
                cmsDraw.DropDownItems.Add(new ToolStripMenuItem(text, null, cmsDrawTool_Click) { Tag = tool });
            }
            cmsDraw.DropDownItems.Add(new ToolStripSeparator());
            cmsDrawColor = new ToolStripMenuItem("顏色");
            foreach (var (name, color) in DrawColors)
            {
                cmsDrawColor.DropDownItems.Add(new ToolStripMenuItem(name, ColorSwatch(color), cmsDrawColor_Click) { Tag = color });
            }
            cmsDraw.DropDownItems.Add(cmsDrawColor);
            cmsDraw.DropDownItems.Add(new ToolStripSeparator());
            cmsDrawUndo = new ToolStripMenuItem("還原", null, (s, e) => UndoDrawing()) { ShortcutKeyDisplayString = "Ctrl+Z" };
            cmsDrawClear = new ToolStripMenuItem("全部清除", null, (s, e) => ClearDrawing());
            cmsDrawSave = new ToolStripMenuItem("另存新檔（含畫圖）...", null, (s, e) => SaveDrawing());
            cmsDraw.DropDownItems.AddRange(new ToolStripItem[] { cmsDrawUndo, cmsDrawClear, new ToolStripSeparator(), cmsDrawSave });
            contextMenuStrip.Items.Insert(contextMenuStrip.Items.IndexOf(cmsScale) + 1, cmsDraw);

            picwinPicture_pic.PaintOverlay += picwinPicture_PaintOverlay;
            picwinPicture_pic.MouseDown += picwinPicture_DrawMouseDown;
            picwinPicture_pic.MouseMove += picwinPicture_DrawMouseMove;
            picwinPicture_pic.MouseUp += picwinPicture_DrawMouseUp;
            FormClosing += pictureWindow_FormClosing;
            contextMenuStrip.Opening += (s, e) => UpdateDrawMenu();  // 圖片可能已更換，開啟選單時更新狀態
            UpdateDrawMenu();
        }  //建立畫圖選單
        private static Bitmap ColorSwatch(Color color)
        {
            var swatch = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(swatch))
            {
                g.Clear(color);
                g.DrawRectangle(Pens.Gray, 0, 0, 15, 15);
            }
            return swatch;
        }  //選單顏色色塊
        private void cmsDrawTool_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem && menuItem.Tag is DrawTool tool)
            {
                if (tool != DrawTool.None) SetPicker(false);  // 畫圖與吸管都用左鍵，只能擇一
                _drawTool = tool;
                Cursor = tool != DrawTool.None || cmsShowLoc.ForeColor == Color.Red ? Cursors.Cross : Cursors.Hand;
                UpdateDrawMenu();
            }
        }  //選擇畫圖工具
        private void cmsDrawColor_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem && menuItem.Tag is Color color)
            {
                _drawColor = color;
                UpdateDrawMenu();
            }
        }  //選擇畫圖顏色
        private void UpdateDrawMenu()
        {
            foreach (ToolStripItem item in cmsDraw.DropDownItems)
            {
                if (item is ToolStripMenuItem mi && mi.Tag is DrawTool tool)
                {
                    mi.Checked = tool == _drawTool;
                }
            }
            foreach (ToolStripMenuItem mi in cmsDrawColor.DropDownItems)
            {
                mi.Checked = (Color)mi.Tag == _drawColor;
            }
            cmsDrawColor.Image = ColorSwatch(_drawColor);
            cmsDrawUndo.Text = $"還原（可還原 {_undoHistory.Count} 步）";
            cmsDrawUndo.Enabled = _undoHistory.Count > 0;
            cmsDrawClear.Enabled = _shapes.Count > 0;
            cmsDrawSave.Enabled = picwinPicture_pic.Image != null;
        }  //更新畫圖選單狀態
        private void PushUndo()
        {
            _undoHistory.Add(new List<DrawShape>(_shapes));
            if (_undoHistory.Count > MaxUndo)
            {
                _undoHistory.RemoveAt(0);
            }
        }  //記錄變更前的狀態
        private void UndoDrawing()
        {
            if (_undoHistory.Count == 0) return;
            _shapes = _undoHistory[_undoHistory.Count - 1];
            _undoHistory.RemoveAt(_undoHistory.Count - 1);
            picwinPicture_pic.Invalidate();
            UpdateDrawMenu();
        }  //還原一步
        private void ClearDrawing()
        {
            if (_shapes.Count == 0) return;
            PushUndo();
            _shapes = new List<DrawShape>();
            picwinPicture_pic.Invalidate();  // 圖形只疊在畫面上，清除後重新渲染即可
            UpdateDrawMenu();
        }  //全部清除
        private float DrawPenWidth()
        {
            Image img = picwinPicture_pic.Image;
            return img == null ? 3f : Math.Max(2f, Math.Max(img.Width, img.Height) / 480f);  // 1920 寬約 4px
        }  //依圖片大小決定線寬
        private void picwinPicture_DrawMouseDown(object sender, MouseEventArgs e)
        {
            if (_drawTool == DrawTool.None || e.Button != MouseButtons.Left || picwinPicture_pic.Image == null) return;
            PointF p = picwinPicture_pic.ClientToImage(e.Location);
            _drawingShape = new DrawShape { Tool = _drawTool, Color = _drawColor, Width = DrawPenWidth(), Start = p, End = p };
        }  //開始拖曳
        private void picwinPicture_DrawMouseMove(object sender, MouseEventArgs e)
        {
            if (_drawingShape == null) return;
            _drawingShape.End = picwinPicture_pic.ClientToImage(e.Location);
            picwinPicture_pic.Invalidate();
        }  //拖曳中預覽
        private void picwinPicture_DrawMouseUp(object sender, MouseEventArgs e)
        {
            if (_drawingShape == null || e.Button != MouseButtons.Left) return;
            DrawShape shape = _drawingShape;
            _drawingShape = null;
            shape.End = picwinPicture_pic.ClientToImage(e.Location);
            if (Math.Abs(shape.End.X - shape.Start.X) >= 3 || Math.Abs(shape.End.Y - shape.Start.Y) >= 3)  // 太小視為點擊
            {
                PushUndo();
                _shapes.Add(shape);
                UpdateDrawMenu();
            }
            picwinPicture_pic.Invalidate();
        }  //完成一個圖形
        private void picwinPicture_PaintOverlay(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            foreach (DrawShape shape in _shapes)
            {
                shape.Draw(e.Graphics);
            }
            _drawingShape?.Draw(e.Graphics);
        }  //繪製所有圖形
        private Bitmap RenderDrawing()
        {
            // 以圖片原始解析度合成，與縮放方式無關
            Image img = picwinPicture_pic.Image;
            Bitmap output = new Bitmap(img.Width, img.Height);
            using (Graphics g = Graphics.FromImage(output))
            {
                g.DrawImage(img, 0, 0, img.Width, img.Height);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                foreach (DrawShape shape in _shapes)
                {
                    shape.Draw(g);
                }
            }
            return output;
        }  //合成圖片與畫圖
        private bool SaveDrawing()
        {
            Image img = picwinPicture_pic.Image;
            if (img == null) return false;
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PNG|*.png|BMP|*.bmp|JPEG|*.jpg";
                saveFileDialog.FileName = $"{mainWindow.Instance?.PatternName ?? "Pattern"}_draw";
                if (saveFileDialog.ShowDialog(this) != DialogResult.OK) return false;
                try
                {
                    using (Bitmap output = RenderDrawing())
                    {
                        string extension = System.IO.Path.GetExtension(saveFileDialog.FileName).ToLower();
                        System.Drawing.Imaging.ImageFormat format =
                            extension == ".bmp" ? System.Drawing.Imaging.ImageFormat.Bmp :
                            extension == ".jpg" ? System.Drawing.Imaging.ImageFormat.Jpeg :
                            System.Drawing.Imaging.ImageFormat.Png;
                        output.Save(saveFileDialog.FileName, format);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"圖片儲存失敗：{ex.Message}");
                    return false;
                }
            }
        }  //另存新檔（含畫圖）
        private void pictureWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_shapes.Count == 0) return;
            DialogResult result = MessageBox.Show(this, "畫面上有畫圖，是否另存新檔？", "PatternShow",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Cancel || (result == DialogResult.Yes && !SaveDrawing()))
            {
                e.Cancel = true;  // 取消或存檔沒有完成就不關閉
            }
        }  //關閉前詢問是否存檔
        //放大鏡與顏色吸管---------------------------------------------------------------------------------------------
        private static bool _magnifierOn;  // 關閉視窗後仍保留設定
        private static bool _pickerOn;
        private static int _magnifierZoom = 8;
        private float UiScale => DeviceDpi / 96f;  // 自動調整縮放時全螢幕視窗是 DPI 感知，介面尺寸跟著螢幕縮放
        private int LensTarget => (int)(280 * UiScale);  // 放大鏡大約邊長（畫面像素）
        private int InfoHeight => TextRenderer.MeasureText("X 0\nRGB 0", LensFont).Height + (int)(16 * UiScale);  // 兩行文字的高度（字型大小跟著 DPI）
        private int InfoMinWidth => InfoHeight + TextRenderer.MeasureText("X 00000, Y 00000  已複製 255.255.255", LensFont).Width + (int)(24 * UiScale);
        private static readonly Font LensFont = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold);
        private Point _lensMouse = new Point(-1, -1);  // 滑鼠位置（畫面座標），-1 表示不在圖片上
        private Rectangle _lensDrawn;                  // 上次畫的範圍，用來局部重繪
        private string _pickerCopied;                  // 剛複製的顏色（滑鼠移到別的像素後清除）
        private Point? _pickerCopiedAt;
        private ToolStripMenuItem cmsMagnifier;
        private ToolStripMenuItem cmsMagnifierOn;
        private ToolStripMenuItem cmsPicker;
        private void InitLens()
        {
            cmsMagnifier = new ToolStripMenuItem("放大鏡") { Name = "cmsMagnifier" };
            cmsMagnifierOn = new ToolStripMenuItem("開啟", null, (s, e) => SetMagnifier(!_magnifierOn)) { ShortcutKeyDisplayString = "M" };
            cmsMagnifier.DropDownItems.Add(cmsMagnifierOn);
            cmsMagnifier.DropDownItems.Add(new ToolStripSeparator());
            foreach (int zoom in new[] { 4, 8, 16, 32 })
            {
                cmsMagnifier.DropDownItems.Add(new ToolStripMenuItem($"{zoom} 倍", null, (s, e) => { _magnifierZoom = zoom; SetMagnifier(true); }) { Tag = zoom });
            }
            cmsPicker = new ToolStripMenuItem("顏色吸管（左鍵複製 R.G.B）", null, (s, e) => SetPicker(!_pickerOn)) { Name = "cmsPicker", ShortcutKeyDisplayString = "C" };
            contextMenuStrip.Items.Insert(contextMenuStrip.Items.IndexOf(cmsDraw) + 1, cmsMagnifier);
            contextMenuStrip.Items.Insert(contextMenuStrip.Items.IndexOf(cmsMagnifier) + 1, cmsPicker);
            picwinPicture_pic.PaintScreenOverlay += picwinPicture_PaintLens;  // 在名稱之後加入，畫在最上層
            picwinPicture_pic.MouseMove += (s, e) => MoveLens(e.Location);
            picwinPicture_pic.MouseLeave += (s, e) => MoveLens(new Point(-1, -1));
            picwinPicture_pic.MouseDown += picwinPicture_PickerMouseDown;
            UpdateLensMenu();
        }  //建立放大鏡與吸管選單
        private void SetMagnifier(bool on)
        {
            _magnifierOn = on;
            UpdateLensMenu();
            RefreshLens();
        }  //開關放大鏡
        private void SetPicker(bool on)
        {
            if (on && _drawTool != DrawTool.None)
            {
                _drawTool = DrawTool.None;  // 吸管與畫圖都用左鍵，開吸管時停止畫圖
                UpdateDrawMenu();
            }
            _pickerOn = on;
            Cursor = on || _drawTool != DrawTool.None || cmsShowLoc.ForeColor == Color.Red ? Cursors.Cross : Cursors.Hand;
            UpdateLensMenu();
            RefreshLens();
        }  //開關顏色吸管
        private void UpdateLensMenu()
        {
            cmsMagnifierOn.Checked = _magnifierOn;
            foreach (ToolStripItem item in cmsMagnifier.DropDownItems)
            {
                if (item is ToolStripMenuItem mi && mi.Tag is int zoom) mi.Checked = zoom == _magnifierZoom;
            }
            cmsMagnifier.Checked = _magnifierOn;
            cmsPicker.Checked = _pickerOn;
        }  //更新選單勾選
        private void MoveLens(Point location)
        {
            _lensMouse = location;
            if (_magnifierOn || _pickerOn) RefreshLens();
        }  //滑鼠移動時更新
        private void RefreshLens()
        {
            // 只重繪上次與這次的範圍，避免整張圖重畫
            Rectangle now = LensLayout(out _, out _);
            picwinPicture_pic.Invalidate(_lensDrawn);
            picwinPicture_pic.Invalidate(now);
            _lensDrawn = now;
        }  //重繪放大鏡
        private int LensPixels() => Math.Max(5, (LensTarget / _magnifierZoom) | 1);  // 放大鏡內顯示幾個像素（奇數，中心為游標像素）
        private Rectangle LensLayout(out Rectangle lens, out Rectangle info)
        {
            // 放大鏡在游標右下方，超出畫面時改放左方 / 上方；資訊列接在放大鏡下方
            lens = info = Rectangle.Empty;
            if ((!_magnifierOn && !_pickerOn) || _lensMouse.X < 0 || picwinPicture_pic.Image == null) return Rectangle.Empty;
            int side = _magnifierOn ? LensPixels() * _magnifierZoom : 0;
            int infoHeight = InfoHeight, width = Math.Max(side, InfoMinWidth), height = side + infoHeight;
            int x = _lensMouse.X + 24, y = _lensMouse.Y + 24;
            if (x + width > picwinPicture_pic.ClientSize.Width) x = _lensMouse.X - 24 - width;
            if (y + height > picwinPicture_pic.ClientSize.Height) y = _lensMouse.Y - 24 - height;
            if (_magnifierOn) lens = new Rectangle(x, y, side, side);
            info = new Rectangle(x, y + side, width, infoHeight);
            return Rectangle.Inflate(Rectangle.Union(_magnifierOn ? lens : info, info), 3, 3);
        }  //計算放大鏡位置
        private void picwinPicture_PaintLens(object sender, PaintEventArgs e)
        {
            LensLayout(out Rectangle lens, out Rectangle info);
            if (info.IsEmpty || !(picwinPicture_pic.Image is Bitmap bitmap)) return;
            Point? pixel = picwinPicture_pic.ToImagePoint(_lensMouse);
            Graphics g = e.Graphics;
            GraphicsState state = g.Save();
            if (!lens.IsEmpty)
            {
                // 以游標像素為中心，最近鄰放大；超出圖片的部分為深灰
                int n = LensPixels(), zoom = _magnifierZoom;
                g.FillRectangle(Brushes.DimGray, lens);
                if (pixel.HasValue)
                {
                    Rectangle source = new Rectangle(pixel.Value.X - n / 2, pixel.Value.Y - n / 2, n, n);
                    Rectangle visible = Rectangle.Intersect(source, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
                    if (!visible.IsEmpty)
                    {
                        Rectangle dest = new Rectangle(lens.X + (visible.X - source.X) * zoom, lens.Y + (visible.Y - source.Y) * zoom, visible.Width * zoom, visible.Height * zoom);
                        g.InterpolationMode = InterpolationMode.NearestNeighbor;
                        g.PixelOffsetMode = PixelOffsetMode.Half;
                        g.DrawImage(bitmap, dest, visible, GraphicsUnit.Pixel);
                        g.PixelOffsetMode = PixelOffsetMode.Default;
                    }
                    if (zoom >= 8)
                    {
                        using (Pen grid = new Pen(Color.FromArgb(70, 128, 128, 128)))
                        {
                            for (int i = 1; i < n; i++)
                            {
                                g.DrawLine(grid, lens.X + i * zoom, lens.Y, lens.X + i * zoom, lens.Bottom);
                                g.DrawLine(grid, lens.X, lens.Y + i * zoom, lens.Right, lens.Y + i * zoom);
                            }
                        }
                    }
                    // 中心像素框：黑白雙框，任何底色都看得到
                    Rectangle center = new Rectangle(lens.X + n / 2 * zoom, lens.Y + n / 2 * zoom, zoom, zoom);
                    g.DrawRectangle(Pens.Black, Rectangle.Inflate(center, 1, 1));
                    g.DrawRectangle(Pens.White, center.X, center.Y, center.Width - 1, center.Height - 1);
                }
                g.DrawRectangle(Pens.White, lens.X - 1, lens.Y - 1, lens.Width + 1, lens.Height + 1);
                g.DrawRectangle(Pens.Black, lens.X - 2, lens.Y - 2, lens.Width + 3, lens.Height + 3);
            }
            // 資訊列：座標、顏色值與色塊
            using (Brush background = new SolidBrush(Color.FromArgb(220, 20, 20, 20)))
            {
                g.FillRectangle(background, info);
            }
            if (pixel.HasValue)
            {
                Color c = bitmap.GetPixel(pixel.Value.X, pixel.Value.Y);
                int pad = (int)(6 * UiScale);
                Rectangle swatch = new Rectangle(info.X + pad, info.Y + pad, info.Height - pad * 2, info.Height - pad * 2);
                using (Brush brush = new SolidBrush(Color.FromArgb(255, c)))
                {
                    g.FillRectangle(brush, swatch);
                }
                g.DrawRectangle(Pens.White, swatch);
                string copied = _pickerCopiedAt == pixel ? $"  已複製 {_pickerCopied}" : "";
                string text = $"X {pixel.Value.X}, Y {pixel.Value.Y}{copied}\nRGB {c.R}.{c.G}.{c.B}   #{c.R:X2}{c.G:X2}{c.B:X2}";
                TextRenderer.DrawText(g, text, LensFont, new Rectangle(swatch.Right + pad, info.Y, info.Right - swatch.Right - pad * 2, info.Height),
                    Color.White, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }
            else
            {
                TextRenderer.DrawText(g, "圖片範圍外", LensFont, info, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
            g.Restore(state);
        }  //畫放大鏡與顏色資訊
        private void picwinPicture_PickerMouseDown(object sender, MouseEventArgs e)
        {
            // 吸管：左鍵複製 R.G.B（與 config 的顏色寫法相同）
            if (!_pickerOn || e.Button != MouseButtons.Left || !(picwinPicture_pic.Image is Bitmap bitmap)) return;
            Point? pixel = picwinPicture_pic.ToImagePoint(e.Location);
            if (!pixel.HasValue) return;
            Color c = bitmap.GetPixel(pixel.Value.X, pixel.Value.Y);
            _pickerCopied = $"{c.R}.{c.G}.{c.B}";
            _pickerCopiedAt = pixel;
            try
            {
                Clipboard.SetText(_pickerCopied);
            }
            catch (System.Runtime.InteropServices.ExternalException)
            {
                _pickerCopied = "失敗（剪貼簿忙碌）";
            }
            RefreshLens();
        }  //吸管複製顏色
        //純色灰階調整-------------------------------------------------------------------------------------------------
        private Bitmap _solidChecked;   // 已檢查過的圖片（換圖後重新檢查）
        private bool _solidIsUniform;
        private bool[] _solidChannels;  // 要調整的色版（R、G、B）：灰階三個一起，紅色只調 R
        private int _solidLevel;
        private bool PrepareSolid(bool grayIfNotSolid)
        {
            // 檢查目前是否為純色圖並記下色版；grayIfNotSolid 時非純色圖改用灰階（自動掃描用）
            if (!(picwinPicture_pic.Image is Bitmap bitmap)) return false;
            if (!ReferenceEquals(bitmap, _solidChecked))
            {
                _solidChecked = bitmap;
                _solidIsUniform = IsUniform(bitmap, out Color c);
                if (_solidIsUniform)
                {
                    _solidLevel = Math.Max(c.R, Math.Max(c.G, c.B));
                    _solidChannels = _solidLevel == 0
                        ? new[] { true, true, true }  // 黑色視為灰階
                        : new[] { c.R == _solidLevel, c.G == _solidLevel, c.B == _solidLevel };
                }
            }
            if (!_solidIsUniform && grayIfNotSolid)
            {
                _solidIsUniform = true;
                _solidChannels = new[] { true, true, true };
                _solidLevel = 0;
            }
            return _solidIsUniform;
        }  //準備調整純色圖
        private void AdjustSolidLevel(int delta)
        {
            if (!PrepareSolid(false))
            {
                FlashCaption("不是純色圖，無法調整灰階");
                return;
            }
            SetSolidLevel(_solidLevel + delta);
        }  //純色圖調整階數
        private void SetSolidLevel(int level, string note = "")
        {
            if (!(picwinPicture_pic.Image is Bitmap bitmap)) return;
            _solidLevel = Math.Max(0, Math.Min(255, level));
            Color color = Color.FromArgb(_solidChannels[0] ? _solidLevel : 0, _solidChannels[1] ? _solidLevel : 0, _solidChannels[2] ? _solidLevel : 0);
            // 換成同尺寸的新純色圖（只影響全螢幕，不改主視窗）
            var solid = new Bitmap(bitmap.Width, bitmap.Height);
            using (Graphics g = Graphics.FromImage(solid))
            {
                g.Clear(color);
            }
            picwinPicture_pic.Image = solid;
            KeepAsPrevious();  // 調整前的那張留給 B 鍵比對
            _bitmap = solid;
            _solidChecked = solid;  // 新圖仍是純色，保留色版設定
            bool gray = _solidChannels.All(on => on);
            FlashCaption((gray ? $"灰階 {_solidLevel}" : $"R{color.R} G{color.G} B{color.B}") + note);
        }  //設定純色圖階數
        //輸入階數------------------------------------------------------------------------------------------------------
        private string _levelInput = "";
        private DateTime _levelInputTime;
        private static int DigitOf(Keys key) =>
            key >= Keys.D0 && key <= Keys.D9 ? key - Keys.D0 : key >= Keys.NumPad0 && key <= Keys.NumPad9 ? key - Keys.NumPad0 : -1;
        private bool HandleLevelInput(KeyEventArgs e)
        {
            // 數字鍵輸入階數、Enter 套用、Backspace 刪一碼、Esc 取消；3 秒沒輸入自動清除
            if ((DateTime.Now - _levelInputTime).TotalSeconds > 3) _levelInput = "";
            int digit = DigitOf(e.KeyCode);
            bool numPad = e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9;
            // 數字鍵盤開始輸入階數；主鍵盤數字只在輸入中接續（沒在輸入時 1~4 用來切換色版）
            if (digit >= 0 && !e.Alt && !e.Control && (numPad || _levelInput != ""))
            {
                _levelInput = _levelInput.Length >= 3 ? digit.ToString() : _levelInput + digit;
            }
            else if (_levelInput == "")
            {
                return false;
            }
            else if (e.KeyCode == Keys.Back)
            {
                _levelInput = _levelInput.Substring(0, _levelInput.Length - 1);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                _levelInput = "";
                FlashCaption("取消輸入");
                return true;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                int level = int.Parse(_levelInput);
                _levelInput = "";
                if (!PrepareSolid(false))
                {
                    FlashCaption("不是純色圖，無法調整灰階");
                    return true;
                }
                SetSolidLevel(level, level > 255 ? "（上限 255）" : "");
                return true;
            }
            else
            {
                return false;
            }
            _levelInputTime = DateTime.Now;
            FlashCaption($"輸入階數 {_levelInput}_（Enter 確定）", 3000);
            return true;
        }  //數字鍵輸入階數
        //自動掃灰階----------------------------------------------------------------------------------------------------
        private static int _scanStep = 1;         // 預設每次 +1 階；關閉視窗後仍保留設定
        private static int _scanInterval = 500;   // 預設每 0.5 秒（毫秒）
        private Timer _scanTimer;
        private ToolStripMenuItem cmsScan;
        private ToolStripMenuItem cmsScanToggle;
        private void InitGrayScanMenu()
        {
            // 右鍵選單「幻燈片」下方：開始 / 停止、步進、間隔
            cmsScan = new ToolStripMenuItem("自動掃灰階") { Name = "cmsScan" };
            cmsScanToggle = new ToolStripMenuItem("開始", null, (s, e) => ToggleGrayScan()) { ShortcutKeyDisplayString = "Alt+R" };
            cmsScan.DropDownItems.Add(cmsScanToggle);
            cmsScan.DropDownItems.Add(new ToolStripSeparator());
            foreach (int step in new[] { 1, 4, 8, 16, 32 })
            {
                cmsScan.DropDownItems.Add(new ToolStripMenuItem($"每次 +{step} 階", null, (s, e) => { _scanStep = step; UpdateScanMenu(); }) { Tag = "step" + step });
            }
            cmsScan.DropDownItems.Add(new ToolStripSeparator());
            foreach (int interval in new[] { 500, 1000, 2000, 3000 })
            {
                cmsScan.DropDownItems.Add(new ToolStripMenuItem($"每 {interval / 1000.0:0.#} 秒", null, (s, e) =>
                {
                    _scanInterval = interval;
                    if (_scanTimer != null) _scanTimer.Interval = interval;
                    UpdateScanMenu();
                }) { Tag = "interval" + interval });
            }
            contextMenuStrip.Items.Insert(contextMenuStrip.Items.IndexOf(cmsSlideshowStop) + 1, cmsScan);  // 幻燈片、停止幻燈片之後
            contextMenuStrip.Opening += (s, e) => UpdateScanMenu();
            FormClosed += (s, e) => _scanTimer?.Stop();
            UpdateScanMenu();
        }  //建立自動掃灰階選單
        private void UpdateScanMenu()
        {
            cmsScanToggle.Text = _scanTimer != null && _scanTimer.Enabled ? "停止" : "開始（目前亮度掃到 255）";
            foreach (ToolStripItem item in cmsScan.DropDownItems)
            {
                if (item is ToolStripMenuItem mi && mi.Tag is string tag)
                {
                    mi.Checked = tag == "step" + _scanStep || tag == "interval" + _scanInterval;
                }
            }
        }  //更新選單勾選
        private void ToggleGrayScan()
        {
            if (_scanTimer != null && _scanTimer.Enabled)
            {
                _scanTimer.Stop();
                FlashCaption($"停止掃描，停在 {_solidLevel} 階");
                UpdateScanMenu();
                return;
            }
            if (!PrepareSolid(true)) return;  // 非純色圖改用灰階
            if (_scanTimer == null)
            {
                _scanTimer = new Timer();
                _scanTimer.Tick += (s, e) =>
                {
                    if (_solidLevel >= 255)
                    {
                        _scanTimer.Stop();
                        FlashCaption("掃描完成（255）");
                        return;
                    }
                    SetSolidLevel(Math.Min(255, _solidLevel + _scanStep), "（自動）");
                };
            }
            timerSlideshow.Stop();  // 不和幻燈片同時跑
            _scanTimer.Interval = _scanInterval;
            if (_solidLevel >= 255)
            {
                FlashCaption("已經是 255，先輸入較低的階數再開始");  // 從目前亮度往上掃，已到頂就不開始
                return;
            }
            SetSolidLevel(_solidLevel, "（自動）");  // 從目前亮度開始，不必每次從 0 跑
            _scanTimer.Start();
            UpdateScanMenu();
        }  //開始 / 停止自動掃灰階
        //資訊列--------------------------------------------------------------------------------------------------------
        private static bool _infoOn;  // 關閉視窗後仍保留
        private void ToggleInfo()
        {
            _infoOn = !_infoOn;
            if (_infoOn) StartRefreshMeter();
            else StopRefreshMeter();
            picwinPicture_pic.Invalidate();
        }  //開關資訊列
        private System.Threading.CancellationTokenSource _meterCancel;
        private string _meterText, _colorText;  // 資訊列：實測更新率、HDR / 色深（開啟資訊列時取得，不在每次重畫時查詢）
        private async void StartRefreshMeter()
        {
            // 資訊列開著時持續量測這個螢幕的實際更新率，約每秒更新一次
            StopRefreshMeter();
            var cancel = _meterCancel = new System.Threading.CancellationTokenSource();
            string device = Screen.FromControl(this).DeviceName;
            _colorText = mainWindow.DisplayTarget(device)?.ColorText;
            _meterText = "實測：量測中…";
            int hz = Math.Max(30, mainWindow.GetRefreshRate(device));
            while (!cancel.IsCancellationRequested && !IsDisposed)
            {
                RefreshResult result = await RefreshMeter.MeasureAsync(device, hz, 5000, cancel.Token);
                if (cancel.IsCancellationRequested || IsDisposed) break;
                _meterText = result.ShortText;
                picwinPicture_pic.Invalidate();
                if (!result.Ok) await System.Threading.Tasks.Task.Delay(1000);  // 量不到時（螢幕關閉）不要一直重試
            }
        }  // 開始量測更新率
        private void StopRefreshMeter()
        {
            _meterCancel?.Cancel();
            _meterCancel = null;
        }  // 停止量測更新率
        private void picwinPicture_PaintInfo(object sender, PaintEventArgs e)
        {
            // 左下角：位置 / 名稱、圖片與螢幕尺寸、更新率、是否 1:1 顯示
            if (!_infoOn || picwinPicture_pic.Image == null) return;
            Image image = picwinPicture_pic.Image;
            Screen screen = Screen.FromControl(this);
            Rectangle shown = picwinPicture_pic.GetImageRectangle();
            bool oneToOne = shown.Width == image.Width && shown.Height == image.Height;
            string position = mainWindow.Instance?.FullScreenPositionText(this) ?? Caption ?? "";
            int hz = mainWindow.GetRefreshRate(screen.DeviceName);
            int windowsScale = mainWindow.MonitorScalePercent(screen.DeviceName);
            Size physical = mainWindow.PhysicalResolution(screen.DeviceName);
            var lines = new List<string>
            {
                position,
                $"圖片 {image.Width} x {image.Height}　螢幕 {screen.Bounds.Width} x {screen.Bounds.Height}{(hz > 1 ? $" @ {hz} Hz" : "")}",
                oneToOne ? "顯示 1:1" : $"⚠ 縮放 {shown.Width / (double)image.Width:0.###} x {shown.Height / (double)image.Height:0.###}（不是 1:1，1px 圖樣可能失真）",
            };
            lines.Add(string.Join("　", new[] { _colorText, _meterText }.Where(s => s != null)));  // HDR / 色深、實測更新率
            if (windowsScale > 0 && windowsScale != 100)
            {
                // 視窗看到的螢幕大小等於實際解析度，代表已自動調整（個別螢幕 DPI 感知）
                lines.Add(screen.Bounds.Size == physical
                    ? $"Windows 縮放 {windowsScale}%（已自動調整，1:1 輸出）"
                    : $"⚠ Windows 縮放 {windowsScale}%：畫面再被放大成 {physical.Width} x {physical.Height}，1px 圖樣不準確");
            }
            Graphics g = e.Graphics;
            var sizes = lines.Select(l => TextRenderer.MeasureText(l, LensFont)).ToList();
            int width = sizes.Max(s => s.Width) + 16, height = sizes.Sum(s => s.Height) + 12;
            var box = new Rectangle(8, picwinPicture_pic.ClientSize.Height - height - 8, width, height);
            using (Brush background = new SolidBrush(Color.FromArgb(200, 20, 20, 20)))
            {
                g.FillRectangle(background, box);
            }
            int y = box.Y + 6;
            for (int i = 0; i < lines.Count; i++)
            {
                Color color = i == 2 && !oneToOne ? Color.FromArgb(255, 210, 0) : Color.White;
                TextRenderer.DrawText(g, lines[i], LensFont, new Point(box.X + 8, y), color, TextFormatFlags.NoPrefix);
                y += sizes[i].Height;
            }
        }  //畫資訊列
        //色版切換------------------------------------------------------------------------------------------------------
        private void SetChannel(int key)
        {
            // 1 R、2 G、3 B、4 灰階（三色一起），之後 ↑↓、輸入階數、Alt+R 都作用在選的色版
            if (!PrepareSolid(false))
            {
                FlashCaption("不是純色圖，無法切換色版");
                return;
            }
            _solidChannels = key == 4 ? new[] { true, true, true } : new[] { key == 1, key == 2, key == 3 };
            SetSolidLevel(_solidLevel);
        }  //切換色版
        //A/B 比對-------------------------------------------------------------------------------------------------------
        private Bitmap _previousBitmap;  // 上一張（換圖或調整階數前）
        private string _previousCaption;
        private void KeepAsPrevious()
        {
            // 目前這張移到「上一張」，原本的上一張釋放
            if (_bitmap == null) return;
            if (_previousBitmap != null && !ReferenceEquals(_previousBitmap, picwinPicture_pic.Image)) _previousBitmap.Dispose();
            _previousBitmap = _bitmap;
            _previousCaption = _caption;
            _bitmap = null;
        }  //保留上一張
        private void ToggleCompare()
        {
            // B：目前這張與上一張互換，再按一次換回來
            if (_previousBitmap == null || _bitmap == null)
            {
                FlashCaption("沒有上一張可以比對");
                return;
            }
            Bitmap current = _bitmap;
            string currentCaption = _caption;
            _bitmap = _previousBitmap;
            _caption = _previousCaption;  // 名稱跟著圖走（不經過 Caption 屬性，避免再閃一次名稱）
            _previousBitmap = current;
            _previousCaption = currentCaption;
            picwinPicture_pic.Image = _bitmap;
            FlashCaption($"B 比對：{PreviousLabel(_bitmap)}");
        }  //A/B 比對
        private string PreviousLabel(Bitmap bitmap)
        {
            // 比對時顯示這張是什麼：純色顯示階數，其他顯示名稱
            if (IsUniform(bitmap, out Color c))
            {
                return c.R == c.G && c.G == c.B ? $"灰階 {c.R}" : $"R{c.R} G{c.G} B{c.B}";
            }
            return _caption ?? "上一張";
        }  //比對名稱
        //拖曳窗口----------------------------------------------------------------------------------------------------
        private bool _dragWindowMode;    // Q：滑鼠拖曳 / 方向鍵移動 Window 的窗口
        private bool _draggingWindow;
        private Point _dragOffset;       // 按下時滑鼠相對窗口左上角的位置
        private readonly System.Diagnostics.Stopwatch _dragThrottle = System.Diagnostics.Stopwatch.StartNew();
        private ToolStripMenuItem cmsDragWindow;
        private void InitDragWindow()
        {
            cmsDragWindow = new ToolStripMenuItem("拖曳窗口（Window 圖樣）", null, (s, e) => SetDragWindowMode(!_dragWindowMode)) { ShortcutKeyDisplayString = "Q" };
            contextMenuStrip.Items.Insert(contextMenuStrip.Items.IndexOf(cmsPicker) + 1, cmsDragWindow);
            picwinPicture_pic.MouseDown += (s, e) =>
            {
                if (!_dragWindowMode || e.Button != MouseButtons.Left || !(picwinPicture_pic.Image is Image image)) return;
                Point? point = picwinPicture_pic.ToImagePoint(e.Location);
                if (!point.HasValue) return;
                Rectangle window = mainWindow.Instance.WindowPatternBounds(image.Width, image.Height);
                // 點在窗口內就照原本的相對位置拖；點在外面就把窗口中心移到滑鼠位置
                _dragOffset = window.Contains(point.Value)
                    ? new Point(point.Value.X - window.X, point.Value.Y - window.Y)
                    : new Point(window.Width / 2, window.Height / 2);
                _draggingWindow = true;
                MoveWindowPatternTo(point.Value.X - _dragOffset.X, point.Value.Y - _dragOffset.Y);
            };
            picwinPicture_pic.MouseMove += (s, e) =>
            {
                if (!_draggingWindow || _dragThrottle.ElapsedMilliseconds < 30) return;  // 最多每 30ms 重新產生一次
                Point? point = picwinPicture_pic.ToImagePoint(e.Location);
                if (point.HasValue) MoveWindowPatternTo(point.Value.X - _dragOffset.X, point.Value.Y - _dragOffset.Y);
            };
            picwinPicture_pic.MouseUp += (s, e) =>
            {
                if (!_draggingWindow) return;
                _draggingWindow = false;
                Point? point = picwinPicture_pic.ToImagePoint(e.Location);
                if (point.HasValue) MoveWindowPatternTo(point.Value.X - _dragOffset.X, point.Value.Y - _dragOffset.Y);
            };
            contextMenuStrip.Opening += (s, e) => cmsDragWindow.Checked = _dragWindowMode;
        }  //建立拖曳窗口
        private void SetDragWindowMode(bool on)
        {
            if (on && (mainWindow.Instance == null || !mainWindow.Instance.CanDragWindowPattern))
            {
                FlashCaption("目前不是可移動的 Window 圖樣（斜角、兩組窗口不能移動）", 2000);
                return;
            }
            if (on)
            {
                // 拖曳與畫圖、吸管都用左鍵，只能擇一
                if (_drawTool != DrawTool.None) { _drawTool = DrawTool.None; UpdateDrawMenu(); }
                if (_pickerOn) SetPicker(false);
            }
            _dragWindowMode = on;
            Cursor = on ? Cursors.SizeAll : (_drawTool != DrawTool.None || _pickerOn || cmsShowLoc.ForeColor == Color.Red ? Cursors.Cross : Cursors.Hand);
            FlashCaption(on ? "拖曳窗口：滑鼠拖曳，方向鍵移動 1px（Shift 10px），Ctrl+滾輪調整大小 5%（Ctrl+Shift 1%），Q 結束" : "結束拖曳窗口", on ? 2500 : 1000);
        }  //開關拖曳窗口
        private void NudgeWindowPattern(KeyEventArgs e)
        {
            if (!(picwinPicture_pic.Image is Image image)) return;
            int step = e.Shift ? 10 : 1;
            Rectangle window = mainWindow.Instance.WindowPatternBounds(image.Width, image.Height);
            int dx = e.KeyCode == Keys.Left ? -step : e.KeyCode == Keys.Right ? step : 0;
            int dy = e.KeyCode == Keys.Up ? -step : e.KeyCode == Keys.Down ? step : 0;
            MoveWindowPatternTo(window.X + dx, window.Y + dy);
        }  //方向鍵移動窗口
        private void MoveWindowPatternTo(int x, int y)
        {
            if (!(picwinPicture_pic.Image is Image image) || mainWindow.Instance == null) return;
            int width = image.Width, height = image.Height;  // 舊圖等一下會釋放，先記下尺寸
            Bitmap moved = mainWindow.Instance.MoveWindowPattern(x, y, width, height);
            Bitmap old = _bitmap;
            _bitmap = moved;
            picwinPicture_pic.Image = moved;
            old?.Dispose();
            _dragThrottle.Restart();
            Rectangle window = mainWindow.Instance.WindowPatternBounds(width, height);
            FlashCaption($"窗口 X {window.X}, Y {window.Y}（{window.Width} x {window.Height}）", 1500);
        }  //移動窗口並更新畫面
        //反相------------------------------------------------------------------------------------------------------
        private bool _inverted;  // 目前畫面是否為反相（換圖後重設）
        private void ToggleInvert()
        {
            // N：RGB 全部反轉（負片），再按一次反轉回來
            if (!(picwinPicture_pic.Image is Bitmap source)) return;
            var result = new Bitmap(source.Width, source.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var rect = new Rectangle(0, 0, source.Width, source.Height);
            var from = source.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var to = result.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            try
            {
                int[] row = new int[source.Width];
                for (int y = 0; y < source.Height; y++)
                {
                    System.Runtime.InteropServices.Marshal.Copy(from.Scan0 + y * from.Stride, row, 0, row.Length);
                    for (int x = 0; x < row.Length; x++) row[x] ^= 0x00FFFFFF;  // 保留透明度，RGB 反轉
                    System.Runtime.InteropServices.Marshal.Copy(row, 0, to.Scan0 + y * to.Stride, row.Length);
                }
            }
            finally
            {
                source.UnlockBits(from);
                result.UnlockBits(to);
            }
            picwinPicture_pic.Image = result;
            if (ReferenceEquals(source, _bitmap)) source.Dispose();
            _bitmap = result;
            _inverted = !_inverted;
            FlashCaption(_inverted ? "反相" : "恢復正片");
        }  //反相
        //格線------------------------------------------------------------------------------------------------------
        private static int _gridMode;  // 0 關、1 每 100px、2 每 10px（每 100px 加深）
        private void ToggleGrid()
        {
            _gridMode = (_gridMode + 1) % 3;
            picwinPicture_pic.Invalidate();
            FlashCaption(_gridMode == 0 ? "格線 關" : _gridMode == 1 ? "格線 每 100px" : "格線 每 10px");
        }  //切換格線
        private void picwinPicture_PaintGrid(object sender, PaintEventArgs e)
        {
            // 圖片座標：線條對齊圖片像素，縮放時一起縮放；線寬固定 1 個螢幕像素
            if (_gridMode == 0 || picwinPicture_pic.Image == null) return;
            Image image = picwinPicture_pic.Image;
            Rectangle shown = picwinPicture_pic.GetImageRectangle();
            if (shown.Width <= 0) return;
            float penWidth = (float)image.Width / shown.Width;
            Graphics g = e.Graphics;
            GraphicsState state = g.Save();
            g.SmoothingMode = SmoothingMode.None;
            using (Pen major = new Pen(Color.FromArgb(220, 0, 255, 255), penWidth))
            using (Pen minor = new Pen(Color.FromArgb(90, 0, 255, 255), penWidth))
            {
                int step = _gridMode == 1 ? 100 : 10;
                for (int x = step; x < image.Width; x += step) g.DrawLine(x % 100 == 0 ? major : minor, x, 0, x, image.Height);
                for (int y = step; y < image.Height; y += step) g.DrawLine(y % 100 == 0 ? major : minor, 0, y, image.Width, y);
            }
            g.Restore(state);
        }  //畫格線
        private void picwinPicture_PaintGridLabels(object sender, PaintEventArgs e)
        {
            // 畫面座標：沿上緣、左緣標示每 100px 的座標
            if (_gridMode == 0 || picwinPicture_pic.Image == null) return;
            Image image = picwinPicture_pic.Image;
            Rectangle shown = picwinPicture_pic.GetImageRectangle();
            if (shown.Width <= 0) return;
            Graphics g = e.Graphics;
            using (Brush background = new SolidBrush(Color.FromArgb(160, 0, 0, 0)))
            {
                void Label(string text, float x, float y)
                {
                    Size size = TextRenderer.MeasureText(text, LensFont);
                    g.FillRectangle(background, x, y, size.Width, size.Height);
                    TextRenderer.DrawText(g, text, LensFont, new Point((int)x, (int)y), Color.Cyan, TextFormatFlags.NoPrefix);
                }
                for (int x = 100; x < image.Width; x += 100) Label(x.ToString(), shown.X + x * (float)shown.Width / image.Width + 2, Math.Max(0, shown.Y) + 2);
                for (int y = 100; y < image.Height; y += 100) Label(y.ToString(), Math.Max(0, shown.X) + 2, shown.Y + y * (float)shown.Height / image.Height + 2);
            }
        }  //格線座標
        private static bool IsUniform(Bitmap bitmap, out Color color)
        {
            // 逐列比對整張圖是否同一個顏色
            color = bitmap.GetPixel(0, 0);
            var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            try
            {
                int[] row = new int[bitmap.Width];
                int first = color.ToArgb();
                for (int y = 0; y < bitmap.Height; y++)
                {
                    System.Runtime.InteropServices.Marshal.Copy(data.Scan0 + y * data.Stride, row, 0, row.Length);
                    foreach (int pixel in row)
                    {
                        if (pixel != first) return false;
                    }
                }
                return true;
            }
            finally
            {
                bitmap.UnlockBits(data);
            }
        }  //是否為純色圖
        //自動隱藏游標---------------------------------------------------------------------------------------------------
        private Timer _cursorTimer;
        private bool _cursorHidden;
        private Point _lastCursor;
        private void InitAutoHideCursor()
        {
            // 滑鼠 2 秒沒動就隱藏；畫圖、吸管時不隱藏
            _cursorTimer = new Timer { Interval = 2000 };
            _cursorTimer.Tick += (s, e) =>
            {
                _cursorTimer.Stop();
                if (!_cursorHidden && _drawTool == DrawTool.None && !_pickerOn && !_dragWindowMode && !contextMenuStrip.Visible && Bounds.Contains(Cursor.Position))
                {
                    Cursor.Hide();
                    _cursorHidden = true;
                }
            };
            picwinPicture_pic.MouseMove += (s, e) => CursorMoved();
            MouseMove += (s, e) => CursorMoved();
            contextMenuStrip.Opening += (s, e) => ShowCursorAgain();
            // 隱藏是整個程式共用的，離開全螢幕視窗（移到主視窗）時一定要恢復
            picwinPicture_pic.MouseLeave += (s, e) => ShowCursorAgain();
            MouseLeave += (s, e) => ShowCursorAgain();
            Deactivate += (s, e) => ShowCursorAgain();
            FormClosed += (s, e) => { _cursorTimer.Stop(); ShowCursorAgain(); };
            _cursorTimer.Start();
        }  //建立自動隱藏游標
        private void CursorMoved()
        {
            // 顯示 / 隱藏游標本身也會觸發 MouseMove，位置沒變就不算移動
            if (Cursor.Position == _lastCursor) return;
            _lastCursor = Cursor.Position;
            ShowCursorAgain();
            _cursorTimer.Stop();
            _cursorTimer.Start();
        }  //滑鼠移動時顯示游標
        private void ShowCursorAgain()
        {
            if (!_cursorHidden) return;
            Cursor.Show();
            _cursorHidden = false;
        }  //恢復游標
    }

    public enum DrawTool
    {
        None,
        Ellipse,
        Rectangle,
        Arrow
    }

    // 畫在全螢幕圖片上的圖形，座標為圖片像素
    public class DrawShape
    {
        public DrawTool Tool;
        public Color Color;
        public float Width;
        public PointF Start;
        public PointF End;
        public void Draw(Graphics g)
        {
            using (Pen pen = new Pen(Color, Width))
            {
                RectangleF r = RectangleF.FromLTRB(Math.Min(Start.X, End.X), Math.Min(Start.Y, End.Y),
                                                   Math.Max(Start.X, End.X), Math.Max(Start.Y, End.Y));
                switch (Tool)
                {
                    case DrawTool.Ellipse:
                        g.DrawEllipse(pen, r);
                        break;
                    case DrawTool.Rectangle:
                        g.DrawRectangle(pen, r.X, r.Y, r.Width, r.Height);
                        break;
                    case DrawTool.Arrow:
                        using (AdjustableArrowCap cap = new AdjustableArrowCap(5, 6))  // 單位為線寬倍數
                        {
                            pen.CustomEndCap = cap;
                            g.DrawLine(pen, Start, End);
                        }
                        break;
                }
            }
        }
    }

    public enum ImageScaleMode
    {
        Fit,             // 符合畫面（等比例，留黑邊）
        Stretch,         // 拉伸填滿
        Fill,            // 填滿裁切（等比例）
        OneToOneCenter,  // 1:1 置中
        OneToOneTopLeft, // 1:1 左上對齊
        Integer          // 最大整數倍放大
    }

    // 可選縮放方式的 PictureBox，預設用最近鄰縮放，避免 1px 圖樣被模糊
    public class ScalablePictureBox : PictureBox
    {
        private ImageScaleMode _scaleMode = ImageScaleMode.Fit;
        private bool _smooth;
        public ImageScaleMode ScaleMode
        {
            get => _scaleMode;
            set { _scaleMode = value; Invalidate(); }
        }
        public bool Smooth
        {
            get => _smooth;
            set { _smooth = value; Invalidate(); }
        }
        public Rectangle GetImageRectangle()
        {
            if (Image == null) return Rectangle.Empty;
            int cw = ClientSize.Width, ch = ClientSize.Height;
            int iw = Image.Width, ih = Image.Height;
            switch (_scaleMode)
            {
                case ImageScaleMode.Stretch:
                    return new Rectangle(0, 0, cw, ch);
                case ImageScaleMode.OneToOneCenter:
                    return new Rectangle((cw - iw) / 2, (ch - ih) / 2, iw, ih);
                case ImageScaleMode.OneToOneTopLeft:
                    return new Rectangle(0, 0, iw, ih);
                case ImageScaleMode.Integer:
                    {
                        int n = Math.Max(1, Math.Min(cw / iw, ch / ih));
                        return new Rectangle((cw - iw * n) / 2, (ch - ih * n) / 2, iw * n, ih * n);
                    }
                default:
                    {
                        double sx = (double)cw / iw, sy = (double)ch / ih;
                        double s = _scaleMode == ImageScaleMode.Fill ? Math.Max(sx, sy) : Math.Min(sx, sy);
                        int w = (int)Math.Round(iw * s), h = (int)Math.Round(ih * s);
                        return new Rectangle((cw - w) / 2, (ch - h) / 2, w, h);
                    }
            }
        }  //計算圖片在畫面上的位置與大小
        public event PaintEventHandler PaintOverlay;  // 在圖片上方繪製，座標為圖片像素
        public event PaintEventHandler PaintScreenOverlay;  // 最上層繪製，座標為畫面（不隨縮放）
        public PointF ClientToImage(Point clientPoint)
        {
            Rectangle r = GetImageRectangle();
            if (r.Width <= 0 || r.Height <= 0) return PointF.Empty;
            return new PointF((clientPoint.X - r.X) * (float)Image.Width / r.Width,
                              (clientPoint.Y - r.Y) * (float)Image.Height / r.Height);
        }  //畫面座標轉換成圖片座標（可超出圖片範圍）
        public Point? ToImagePoint(Point clientPoint)
        {
            Rectangle r = GetImageRectangle();
            if (r.Width <= 0 || r.Height <= 0 || !r.Contains(clientPoint)) return null;
            return new Point((clientPoint.X - r.X) * Image.Width / r.Width,
                             (clientPoint.Y - r.Y) * Image.Height / r.Height);
        }  //畫面座標轉換成圖片像素座標
        protected override void OnPaint(PaintEventArgs pe)
        {
            if (Image == null)
            {
                base.OnPaint(pe);
                PaintScreenOverlay?.Invoke(this, pe);
                return;
            }
            Graphics g = pe.Graphics;
            g.InterpolationMode = _smooth ? InterpolationMode.HighQualityBicubic : InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half;  // 避免最近鄰縮放時邊緣偏移半個像素
            Rectangle r = GetImageRectangle();
            g.DrawImage(Image, r);
            if (PaintOverlay != null && r.Width > 0 && r.Height > 0)
            {
                GraphicsState state = g.Save();
                g.PixelOffsetMode = PixelOffsetMode.Default;
                g.TranslateTransform(r.X, r.Y);
                g.ScaleTransform((float)r.Width / Image.Width, (float)r.Height / Image.Height);
                PaintOverlay(this, pe);
                g.Restore(state);
            }
            PaintScreenOverlay?.Invoke(this, pe);
        }
    }
}
