using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Thunder
{
    public partial class pictureWindow : Form
    {
        private Bitmap _bitmap;

        public pictureWindow(Bitmap bitMap)
        {
            InitializeComponent();
            _bitmap = bitMap; // 將接收到的 Bitmap 儲存到類別屬性中
            this.Paint += Form2_show; // 綁定 Paint 事件  
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
            _bitmap = bitmap;
            picwinPicture_pic.Image = _bitmap;

        }
        private void pictureWindow_DoubleClick(object sender, EventArgs e)
        {
            //if (FormBorderStyle == FormBorderStyle.None)
            if (WindowState == FormWindowState.Maximized)
            {
                // 退出全螢幕，並將尺寸設為所在螢幕的 1/4 大小
                FormBorderStyle = FormBorderStyle.Sizable;
                WindowState = FormWindowState.Normal;

                // 計算所在螢幕的尺寸
                Screen currentScreen = Screen.FromControl(this);
                int newWidth = currentScreen.WorkingArea.Width / 3;
                int newHeight = currentScreen.WorkingArea.Height / 3;

                // 設定 Form 的大小和位置
                Size = new Size(newWidth, newHeight);
                Location = new Point(
                    currentScreen.WorkingArea.Left + (currentScreen.WorkingArea.Width - newWidth) / 3,
                    currentScreen.WorkingArea.Top + (currentScreen.WorkingArea.Height - newHeight) / 3
                );
            }
            else
            {
                // 進入全螢幕
                WindowState = FormWindowState.Maximized;
                FormBorderStyle = FormBorderStyle.None;
            }

        }

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
        }

        private void cmsShowLoc_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem && menuItem.Owner is ContextMenuStrip contextMenu)
            {
                if (cmsShowLoc.ForeColor != Color.Red)
                {
                    // 取消訂閱 PictureBox 的 MouseMove 事件  
                    MouseMove += PictureBox_MouseMove;
                    cmsShowLoc.ForeColor = Color.Red;
                }
                else
                {
                    // 訂閱 PictureBox 的 MouseMove 事件  
                    MouseMove -= PictureBox_MouseMove;
                    cmsShowLoc.ForeColor = Color.Black;
                }
            }
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is PictureBox targerBox)
            {
                // 更新 ToolTip 的內容為滑鼠座標
                toolTip.SetToolTip(targerBox, $"{e.Location}");
            }
        }

        private void pictureWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is Form targetform)
            {
                // 按下 ESC 鍵退出全螢幕  
                if (e.KeyCode == Keys.Escape)
                {
                    Close();
                }
                else if (e.KeyCode == Keys.Right)
                {
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

                    }
                }
                else if (e.KeyCode == Keys.Left)
                {
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

                    }
                }
            }
        }

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
        }

        private void cmsToBeContinued_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cmsScreenlist_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 獲取選擇的螢幕  
            string selectedScreen = cmsScreenlist.SelectedItem?.ToString() ?? Screen.AllScreens[0].DeviceName;
            Screen secondScreen = Screen.AllScreens.FirstOrDefault(screen => screen.DeviceName == selectedScreen) ??
                                  Screen.AllScreens[0];
            // 設置窗口位置和大小  
            StartPosition = FormStartPosition.Manual;
            Location = secondScreen.Bounds.Location;
            Size = secondScreen.Bounds.Size;
        }

        private void cmsAdjust_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem )
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
        }
    }
}