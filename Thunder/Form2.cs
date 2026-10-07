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
        public pictureWindow()
        {
            InitializeComponent();
            InitScaleMenu();
            InitDrawMenu();
            InitSlideshowMenu();
            InitCaption();
            MouseWheel += pictureWindow_MouseWheel;  // PictureBox 不會取得焦點，滾輪事件由視窗接收
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
            if (_bitmap != null)
            {
                _bitmap.Dispose();
                _bitmap = null;
            }
            _bitmap = (Bitmap)bitmap.Clone();
            picwinPicture_pic.Image = _bitmap;

        }  //設定_bitmap
        //pictureWindow操控--------------------------------------------------------------------------------------------
        private void pictureWindow_DoubleClick(object sender, EventArgs e)
        {
            if (sender is PictureBox && _drawTool != DrawTool.None)
            {
                return;  // 畫圖時雙擊不切換全螢幕
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
                toolTip.SetToolTip(targerBox, p.HasValue ? $"X: {p.Value.X}, Y: {p.Value.Y}" : "圖片範圍外");
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
            // 與方向鍵相同：往下滾 = 下一張（→），往上滾 = 上一張（←）
            _wheelDelta += e.Delta;
            while (Math.Abs(_wheelDelta) >= SystemInformation.MouseWheelScrollDelta)
            {
                bool previous = _wheelDelta > 0;
                _wheelDelta -= Math.Sign(_wheelDelta) * SystemInformation.MouseWheelScrollDelta;
                pictureWindow_KeyDown(this, new KeyEventArgs(previous ? Keys.Left : Keys.Right));
            }
        }  //滾輪切圖
        private void pictureWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z)
            {
                UndoDrawing();
                return;
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
                    timerSlideshow.Interval = interval;
                    timerSlideshow.Tag = "KeyRight";
                    timerSlideshow.Start();
                }
            }
        }  //開始幻燈片放映
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
                InvalidateCaption();
            }
        }  // 目前圖片名稱（DirList 檔名 / ImgList 名稱；null 不顯示）
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
            if (!_captionVisible || string.IsNullOrEmpty(_caption)) return;
            // 127 灰底 50% 透明；黑色粗體字加白框（用路徑描邊再填色）
            Graphics g = e.Graphics;
            float emSize = CaptionFont.SizeInPoints * g.DpiY / 72f;  // 路徑以像素為單位
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddString(_caption, CaptionFont.FontFamily, (int)CaptionFont.Style, emSize, new PointF(8, 4), StringFormat.GenericTypographic);
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
            cmsSlideshow3.Tag = 3000;
            cmsSlideshow5.Tag = 5000;
            cmsSlideshow10.Tag = 10000;
            var halfSecond = new ToolStripMenuItem("每0.5秒", null, Slideshow) { Name = "cmsSlideshow05", Tag = 500 };
            var twoSeconds = new ToolStripMenuItem("每2秒", null, Slideshow) { Name = "cmsSlideshow2", Tag = 2000 };
            cmsSlideshow.DropDownItems.Insert(cmsSlideshow.DropDownItems.IndexOf(cmsSlideshow1), halfSecond);
            cmsSlideshow.DropDownItems.Insert(cmsSlideshow.DropDownItems.IndexOf(cmsSlideshow1) + 1, twoSeconds);
        }  //幻燈片間隔選項
        private void timer_Tick(object sender, EventArgs e)
        {
            if (sender is Timer t && t.Tag is string st)
            {
                if (st == "KeyRight")
                {
                    // 模擬按下向右鍵  
                    // With the corrected line:  
                    pictureWindow_KeyDown(this, new KeyEventArgs(Keys.Right));
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

            if (picwinPicture_pic.Image != null)
            {
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