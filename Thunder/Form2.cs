using System;
using System.Drawing;
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
                    Cursor = Cursors.Hand; // 恢復游標為預設
                }
            }
        }  //顯示座標
        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
                //MessageBox.Show("");
            if (sender is PictureBox targerBox)
            {
                // 更新 ToolTip 的內容為滑鼠座標
                toolTip.SetToolTip(targerBox, $"{e.Location}");
            }
        }  //滑鼠移動顯示座標 tooltip
        private void pictureWindow_KeyDown(object sender, KeyEventArgs e)
        {
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
                                // 確保 ImgList 分頁下有 ListBox 並切換選項  
                                if (tabControl.TabPages[2].Controls["tabilPatternList_lst"] is ListBox
                                        listBox && listBox.Items.Count > 0)
                                {
                                    int currentIndex = listBox.SelectedIndex;
                                    if (currentIndex != -1)
                                    {
                                        listBox.SelectedIndex = -1; // 取消目前選擇
                                    }

                                    int nextIndex = (currentIndex + 1) % listBox.Items.Count;
                                    listBox.SelectedIndex = nextIndex; // 選擇下一個項目
                                    mainWindow.Instance.FullScreen(sender, e);
                                }
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
                                        : listView.Items.Count;
                                    if (currentIndex != -1)
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
                                // 確保 ImgList 分頁下有 ListBox 並切換選項  
                                if (tabControl.TabPages[2].Controls["tabilPatternList_lst"] is ListBox
                                        listBox && listBox.Items.Count > 0)
                                {
                                    int currentIndex = listBox.SelectedIndex;
                                    if (currentIndex != -1)
                                    {
                                        listBox.SelectedIndex = -1; // 取消目前選擇
                                    }

                                    int nextIndex = (currentIndex - 1 + listBox.Items.Count) % listBox.Items.Count % listBox.Items.Count;
                                    listBox.SelectedIndex = nextIndex; // 選擇下一個項目
                                    mainWindow.Instance.FullScreen(sender, e);
                                }
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
                        if (mainWindow.Instance.timerDynamic.Tag.ToString() == "stop")
                        {
                            mainWindow.Instance.timerDynamic.Start();
                            mainWindow.Instance.timerDynamic.Tag = "run";
                        }
                        else
                        {
                            mainWindow.Instance.timerDynamic.Stop();
                            mainWindow.Instance.timerDynamic.Tag = "stop";
                        }
                        break;
                    case Keys.Add:
                        if (mainWindow.Instance.mnsScreenlist_cmb.SelectedIndex < mainWindow.Instance.mnsScreenlist_cmb.Items.Count - 1)
                        {
                            mainWindow.Instance.mnsScreenlist_cmb.SelectedIndex += 1;
                        }
                        else
                        {
                            mainWindow.Instance.mnsScreenlist_cmb.SelectedIndex = 0;
                        }
                        mainWindow.Instance.Generate_Click(sender, e);
                        mainWindow.Instance.FullScreen(sender, e);
                        break;
                    case Keys.Subtract:
                        if (mainWindow.Instance.mnsScreenlist_cmb.SelectedIndex == 0)
                        {
                            mainWindow.Instance.mnsScreenlist_cmb.SelectedIndex = mainWindow.Instance.mnsScreenlist_cmb.Items.Count - 1;

                        }
                        else
                        {
                            mainWindow.Instance.mnsScreenlist_cmb.SelectedIndex -= 1;
                        }
                        mainWindow.Instance.Generate_Click(sender, e);
                        mainWindow.Instance.FullScreen(sender, e);
                        break;
                }
               
            }
        }  //鍵盤事件
        //右鍵選單-----------------------------------------------------------------------------------------------------
        private void contextMenuStrip_Opened(object sender, EventArgs e)
        {
            if (sender is ContextMenuStrip cms && cms.SourceControl is Control sourceControl)
            {
                Point mousePosition = sourceControl.PointToClient(Cursor.Position);
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
            mainWindow.Instance.mnsScreenlist_cmb.SelectedItem = selectedScreen;
            mainWindow.Instance.Generate_Click(sender, e);
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
                    int interval;
                    if (menuItem.Name == "cmsSlideshowTextButton" && int.TryParse(cmsSlideshowText.Text, out interval))
                    {
                        // 開始幻燈片放映  
                        timerSlideshow.Interval = interval; // 設定間隔時間為 interval毫秒  
                    }
                    else if (int.TryParse(menuItem.Name.Substring(12), out interval))
                    {
                        // 開始幻燈片放映  
                        timerSlideshow.Interval = interval * 1000; // 設定間隔時間秒  
                    }
                    timerSlideshow.Tag = "KeyRight";
                    timerSlideshow.Start();
                }
            }
        }  //開始幻燈片放映
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
    }
}