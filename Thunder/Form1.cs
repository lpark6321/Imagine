using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Object = System.Object;
using String = System.String;


namespace Thunder
{
    public partial class Mainwindow : Form
    {
        public Mainwindow()
        {
            InitializeComponent();
            Mainwindow_Load(this, EventArgs.Empty);
            mypicture = new MyPicture(this);
        }
        private void Mainwindow_Load(object sender, EventArgs e)
        {
            Screenlist(this, EventArgs.Empty);
            if (mnsScreenlist_cmb.Items.Count > 1)
            {
                mnsScreenlist_cmb.SelectedIndex = 1;
            }
            else if (mnsScreenlist_cmb.Items.Count > 0)
            {
                mnsScreenlist_cmb.SelectedIndex = 0;
            }
            tabimgeFuncList.SelectedIndex = 1; // 功能預設選擇第一個項目
            tabieaResize_rdo.Select(); //圖片館例預設選取RESIZE
            MaskPanelCreat(sender, e); //先產生MaskPanel
        }
        public MyPicture mypicture { get; private set; }

        public class MyPicture
        {
            private Bitmap _currentBitmap;
            public string tag;
            private int _width;
            private int _height;

            private Mainwindow m1 { get; }
            public MyPicture(Mainwindow m0)
            {
                m1 = m0 ?? throw new ArgumentNullException(nameof(m0));
                _width = m1.mnsW_txt.Text != "" ? int.Parse(m1.mnsW_txt.Text) : 1920;
                _height = m1.mnsH_txt.Text != "" ? int.Parse(m1.mnsH_txt.Text) : 1080;
                _currentBitmap = new Bitmap(_width, _height);
                tag = "";
            }
            public void SaveImage(string filename = "")
            {
                if (_currentBitmap == null)
                {
                    MessageBox.Show("沒有圖片可以儲存！");
                    return;
                }
                if (filename == "")
                {
                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.Filter = "BMP|*.bmp|PNG|*.png|JPEG|*.jpg";
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            try
                            {

                                if (_currentBitmap == null)
                                {
                                    throw new InvalidOperationException("沒有圖片可以儲存！");
                                }

                                string extension = Path.GetExtension(saveFileDialog.FileName).ToLower();
                                // C#7.3支援
                                ImageFormat imageFormat;
                                switch (extension)
                                {
                                    case ".jpg":
                                        imageFormat = ImageFormat.Jpeg;
                                        break;
                                    case ".png":
                                        imageFormat = ImageFormat.Png;
                                        break;
                                    case ".bmp":
                                        imageFormat = ImageFormat.Bmp;
                                        break;
                                    default:
                                        throw new NotSupportedException("不支援的檔案格式！");
                                }
                                // C#8.0支援
                                //ImageFormat imageFormat = extension switch
                                //{
                                //    ".jpg" => ImageFormat.Jpeg,
                                //    ".png" => ImageFormat.Png,
                                //    ".bmp" => ImageFormat.Bmp,
                                //    _ => throw new NotSupportedException("不支援的檔案格式！")
                                //};
                                _currentBitmap.Save(saveFileDialog.FileName, imageFormat);

                                //MessageBox.Show("圖片已儲存成功！");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"圖片儲存失敗：{ex.Message}");
                            }
                        }
                    }
                }
            }

            public Image OpenImage()
            {

                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    // 設定檔案過濾器，只允許選擇 JPG、PNG 和 BMP 格式的圖片
                    openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                    openFileDialog.Title = "選擇圖片檔案";

                    // 顯示對話框並檢查用戶是否選擇了檔案
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            // 使用 Image.FromFile 載入圖片
                            _currentBitmap = (Bitmap)Image.FromFile(openFileDialog.FileName);
                            return _currentBitmap; // 回傳圖片
                        }
                        catch (OutOfMemoryException)
                        {
                            using (FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                            {
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    fs.CopyTo(ms);
                                    ms.Position = 0;
                                    _currentBitmap = (Bitmap)Image.FromStream(ms);
                                    tag = "image";
                                    return _currentBitmap; // 回傳圖片
                                }
                            }
                            MessageBox.Show("無法載入圖片，可能是圖片過大或格式不支援！(可以另存為JPEG、BMP、PNG)");
                        }
                        catch (Exception ex)
                        {
                            // 如果載入失敗，顯示錯誤訊息
                            MessageBox.Show($"無法載入圖片：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }

                return null; // 如果用戶取消或載入失敗，回傳 null
            }
            public Bitmap Getpicture()
            {
                return _currentBitmap;
            }

            public string Getsize()
            {
                _width = _currentBitmap.Width;
                _height = _currentBitmap.Height;
                return $"{_width} x {_height}"; // 更新圖片大小
            }

            public Image Setpicture(Image image)
            {
                _currentBitmap = image as Bitmap;
                if (_currentBitmap == null)
                {
                    throw new InvalidCastException("無法將 Image 轉換為 Bitmap。請確保提供的圖片是 Bitmap 類型。");
                }
                tag = "image";
                return _currentBitmap; // 回傳圖片
            }
        }

        private void cmbFunclist_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 隱藏所有 GroupBox
            tabimgeFrame.Visible = false;
            tabimgeGradient.Visible = false;
            tabimgeChess.Visible = false;
            tabimgeWindow.Visible = false;
            tabimgeMask.Visible = false;
            tabimgeAdjust.Visible = false;
            tabimgeMass.Visible = false;

            // 根據選擇的項目顯示對應的 GroupBox
            //switch (comboBoxfunc.SelectedItem.ToString())
            switch (tabimgeFuncList.Text)
            {
                case "Frame&CrossLine&Border":
                    tabimgeFrame.Visible = true;
                    tabimgeFrame.Dock = DockStyle.Fill;
                    break;
                case "Gradient&ColorBar":
                    tabimgeGradient.Visible = true;
                    tabimgeGradient.Dock = DockStyle.Fill;
                    //tabigVWay_rdo.Checked = true; // 預設選擇 Gradient 方式
                    //tabigStep_cmb.Text = "256"; // 預設選擇 256 階
                    //tabigDivid_cmb.Text = "1"; // 預設選擇 1 階
                    //tabigFistLevel_cmb.Text = "0"; // 預設從 0 階開始
                    //tabigLastLevel_cmb.Text = "255"; // 預設到 255 階結束
                    break;
                case "Chess":
                    tabimgeChess.Visible = true;
                    tabimgeChess.Dock = DockStyle.Fill;
                    break;
                case "Window":
                    tabimgeWindow.Visible = true;
                    tabimgeWindow.Dock = DockStyle.Fill;
                    //radioButton11.Checked = true; // 預設選擇純色背景
                    //radioButton16.Checked = true; // 預設選擇純色方塊
                    //tabiwWinSizePercent_rdo.Checked = true; // 預設選擇百分比大小
                    //tabiwWinSizePercent_nud.Value = 50; // 預設選擇 50% 大小
                    //tabiwWinLocCenter_rdo.Checked = true; // 預設選擇置中位置
                    tabiwWinSizePixelW_nud.Maximum = int.Parse(mnsW_txt.Text);
                    tabiwWinSizePixelH_nud.Maximum = int.Parse(mnsH_txt.Text);
                    tabiwWinLocPixcelX_nud.Maximum = int.Parse(mnsW_txt.Text);
                    tabiwWinLocPixcelY_nud.Maximum = int.Parse(mnsH_txt.Text);
                    break;
                case "Mask(FlickerPattern)":
                    tabimgeMask.Visible = true;
                    tabimgeMask.Dock = DockStyle.Fill;
                    //tabiemPixel_rdo.Checked = true;
                    //tabiemHNum_nud.Value = 2;
                    //tabiemWNum_nud.Value = 2;
                    //MaskPanelCreat(sender, e);
                    break;
                case "ImageAdjust":
                    tabimgeAdjust.Visible = true;
                    tabimgeAdjust.Dock = DockStyle.Fill;
                    //tabieaResize_rdo.Checked = true;
                    break;
                case "MassImage":
                    tabimgeMass.Visible = true;
                    tabimgeMass.Dock = DockStyle.Fill;
                    break;
                default:
                    // 如果沒有匹配的項目，則不顯示任何 GroupBox
                    break;
            }
        }

        private void Generate_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(mnsW_txt.Text, out int width) || !int.TryParse(mnsH_txt.Text, out int height))
            {
                MessageBox.Show("請輸入有效的寬度與高度！");
                return;
            }
            string type;
            switch (tabimgeFuncList.Text)
            {
                case "Frame&CrossLine&Border":
                    type = "solid";
                    break;
                case "Gradient&ColorBar":
                    type = "gradient";
                    break;
                case "Chess":
                    type = "chess";
                    break;
                case "Window":
                    type = "window";
                    break;
                case "Mask(FlickerPattern)":
                    type = "mask";
                    break;
                case "ImageAdjust":
                    type = "adjust";
                    break;
                case "MassImage":
                    type = "mass";
                    break;
                default:
                    MessageBox.Show("請選擇一個有效的功能！");
                    return;
            }
            // 根據 cmbFunclist 的選擇來決定 type  
            // C#8.0支援
            //string type = tabimgeFuncList.Text switch
            //{
            //    "Frame&CrossLine&Border" => "solid",
            //    "Gradient&ColorBar" => "gradient",
            //    "Chess" => "chess",
            //    "Window" => "window",
            //    "Mask(FlickerPattern)" => "mask",
            //    "ImageAdjust" => "adjust",
            //    "MassImage" => "mass"
            //};

            try
            {
                GenerateImage(type, width, height);
                showimgPicture_pic.Image = mypicture.Getpicture();
                showimgPicture_pic.SizeMode = PictureBoxSizeMode.Zoom;
                showimgSize_btn.Text = $"當前圖片大小：{mnsW_txt.Text} x {mnsH_txt.Text}"; // 更新狀態列顯示當前圖片大小  
            }
            catch (Exception ex)
            {
                MessageBox.Show($"圖片生成失敗：{ex.Message}");
            }
        }


        private void FullScreen(object sender, EventArgs e)
        {
            // 獲取選擇的螢幕  
            string selectedScreen = mnsScreenlist_cmb.SelectedItem?.ToString();
            Screen secondScreen = Screen.AllScreens.FirstOrDefault(screen => screen.DeviceName == selectedScreen);

            // 創建全螢幕表單  
            Form fullScreenForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None, // 無邊框  
                WindowState = FormWindowState.Maximized, // 最大化  
                StartPosition = FormStartPosition.Manual, // 手動設置位置  
                Location = secondScreen.Bounds.Location, // 設置到選擇的螢幕  
                Size = secondScreen.Bounds.Size, // 設置大小為選擇螢幕大小  
                //BackColor = Color.Black, // 背景設置為黑色  
                KeyPreview = true // 啟用按鍵事件預覽  
            };

            // 創建一個 PictureBox 並設置圖片  
            PictureBox fullScreenPictureBox = new PictureBox
            {
                Dock = DockStyle.Fill, // 填充整個表單  
                Image = showimgPicture_pic.Image, // 使用原始 PictureBox 的圖片  
                SizeMode = PictureBoxSizeMode.Zoom, // 縮放圖片以適應螢幕  
            };
            // 將 PictureBox 添加到表單  
            fullScreenForm.Controls.Add(fullScreenPictureBox);
            // 雙擊退出全螢幕  
            fullScreenPictureBox.DoubleClick += (s, args) => fullScreenForm.Close();

            // 按下 ESC 鍵退出全螢幕  
            fullScreenForm.KeyDown += (s, args) =>
            {
                if (args.KeyCode == Keys.Escape)
                {
                    fullScreenForm.Close();
                }
            };
            // 綁定右鍵
            fullScreenPictureBox.ContextMenuStrip = contextMenuStrip;
            // 顯示全螢幕表單  
            fullScreenForm.ShowDialog();
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is PictureBox targerBox)
            {
                // 更新 ToolTip 的內容為滑鼠座標
                toolTip.SetToolTip(targerBox, $"{e.Location}");
            }
        }

        private void Screenlist(object sender, EventArgs e)
        {
            // 獲取所有螢幕
            Screen[] screens = Screen.AllScreens;
            // 清空下拉選單
            mnsScreenlist_cmb.Items.Clear();
            // 添加螢幕選項到下拉選單
            foreach (Screen screen in screens)
            {
                mnsScreenlist_cmb.Items.Add(screen.DeviceName);
            }


        }
        private void menuToolStripCmb_SelectedIndexChanged(object sender, EventArgs e)
        {

            // 獲取選擇的螢幕
            string selectedScreen = mnsScreenlist_cmb.SelectedItem.ToString();
            Screen[] screens = Screen.AllScreens;
            // 根據選擇的螢幕更新寬度與高度
            foreach (Screen screen in screens)
            {
                if (screen.DeviceName == selectedScreen)
                {
                    mnsW_txt.Text = screen.Bounds.Width.ToString();
                    mnsH_txt.Text = screen.Bounds.Height.ToString();
                    break;
                }
            }
        }
        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabdlPatternList_lvw.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = tabdlPatternList_lvw.SelectedItems[0]; // 獲取選中的 ListViewItem  
                string imagePath = selectedItem.Tag as string; // 從 Tag 中取出圖片路徑  

                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    // 載入原始圖片到 PictureBox  
                    showimgPicture_pic.Image = Image.FromFile(imagePath);
                    showimgPicture_pic.SizeMode = PictureBoxSizeMode.Zoom;
                    showimgSize_btn.Text = $"當前圖片大小：{selectedItem.SubItems[2].Text} x {selectedItem.SubItems[3].Text}"; // 更新狀態列顯示當前圖片大小  
                }
                else
                {
                    MessageBox.Show($"找不到圖片：{imagePath}");
                }
            }
        }

        private void PopulateListView()
        {
            // 清空現有項目
            tabdlPatternList_lvw.Items.Clear();
            tabdlPatternList_lvw.Groups.Clear();
            tabdlPatternList_lvw.LargeImageList = new ImageList
            {
                ImageSize = new Size(90, 90) // 設置縮略圖大小
            };

            // 定義分組
            ListViewGroup jpgGroup = new ListViewGroup("JPG 圖片", HorizontalAlignment.Left);
            ListViewGroup pngGroup = new ListViewGroup("PNG 圖片", HorizontalAlignment.Left);
            ListViewGroup bmpGroup = new ListViewGroup("BMP 圖片", HorizontalAlignment.Left);

            tabdlPatternList_lvw.Groups.Add(jpgGroup);
            tabdlPatternList_lvw.Groups.Add(pngGroup);
            tabdlPatternList_lvw.Groups.Add(bmpGroup);

            // 獲取程式所在的資料夾
            string folderPath = Application.StartupPath;

            // 獲取資料夾中的所有 JPG、PNG 和 BMP 檔案
            string[] imageFiles = Directory.GetFiles(folderPath, "*.*")
                                            .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                                           file.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                                                           file.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
                                            .ToArray();

            // 添加圖片到 ListView
            foreach (string imagePath in imageFiles)
            {
                try
                {
                    // 加載圖片
                    using (Image img = Image.FromFile(imagePath))
                    {
                        string fileName = Path.GetFileName(imagePath);
                        string extension = Path.GetExtension(imagePath).ToLower();
                        int width = img.Width;
                        int height = img.Height;

                        // 添加到 ImageList
                        tabdlPatternList_lvw.LargeImageList.Images.Add(fileName, new Bitmap(img));

                        // 創建 ListViewItem
                        ListViewItem item = new ListViewItem(fileName)
                        {
                            Tag = imagePath, // 存儲完整路徑
                            ImageKey = fileName // 對應 ImageList 的鍵值
                        };

                        // 添加子項目（檔案名稱、寬、高）
                        item.SubItems.Add(fileName); // 名稱
                        item.SubItems.Add(width.ToString()); // 寬
                        item.SubItems.Add(height.ToString()); // 高

                        // 根據副檔名分組
                        if (extension == ".jpg")
                        {
                            item.Group = jpgGroup;
                        }
                        else if (extension == ".png")
                        {
                            item.Group = pngGroup;
                        }
                        else if (extension == ".bmp")
                        {
                            item.Group = bmpGroup;
                        }

                        // 添加到 ListView
                        tabdlPatternList_lvw.Items.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"無法加載圖片：{imagePath}\n錯誤：{ex.Message}");
                }
            }
        }



        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == tabDirList)
            {
                // 當選擇 ImgList 標籤時，執行 PopulateListView 方法
                PopulateListView();
            }
            else if (tabControl.SelectedTab == tabImgEditor)
            {
                // 當選擇 ImgEditor 標籤時，執行其他邏輯
            }
        }


        private void gbxfHsblocation_ValueChanged(object sender, EventArgs e)
        {
            tabiefOtherLoc_lbl.Text = tabiefOtherLoc_hsc.Value.ToString();  // frame 的卷軸值變更
        }

        private void Framechkother_CheckStateChanged(object sender, EventArgs e)
        {
            if (tabiefOther_chk.Checked)  // frame 的卷軸值顯示
            {
                tabiefOther_pnl.Enabled = true;
                //tabiefOtherLoc_lbl.Text = tabiefOtherLoc_hsc.Value.ToString();
            }
            else
            {
                tabiefOther_pnl.Enabled = false;
                //tabiefOtherLoc_lbl.Text = "";
            }
        }


        public void GenerateImage(string type, int width, int height)
        {
            Bitmap CurrentBitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(CurrentBitmap))
            {
                g.SmoothingMode = SmoothingMode.None; // 抗鋸齒模式
                g.CompositingMode = CompositingMode.SourceCopy; // 合成模式
                g.CompositingQuality = CompositingQuality.HighSpeed; //
                g.InterpolationMode = InterpolationMode.NearestNeighbor;


                switch (type)
                {
                    case "solid":
                        if (mypicture.tag == "image")
                        {
                            //使用圖片
                            g.DrawImage(mypicture.Getpicture(), 0, 0, width, height);
                        }
                        else
                        {
                            // 填充背景顏色
                            g.Clear(tabiefBack_btn.BackColor);
                        }
                        // 繪製框線
                        if (tabiefOther_chk.Checked)
                        {
                            using (Pen pen = new Pen(Color.Black, 1)) // 預設線條顏色和寬度
                            {
                                foreach (string item in tabiefOther_lst.Items)
                                {
                                    // 解析格式化字串，例如 "H:100,255,0,0"
                                    string[] parts = item.Split(':');
                                    if (parts.Length == 2)
                                    {
                                        string HVtype = parts[0]; // H 或 V
                                        string[] values = parts[1].Split(',');
                                        if (values.Length == 4)
                                        {
                                            int location = int.Parse(values[0]); // 線條位置
                                            int r = int.Parse(values[1]); // 紅色分量
                                            int gValue = int.Parse(values[2]); // 綠色分量
                                            int b = int.Parse(values[3]); // 藍色分量

                                            // 設置線條顏色
                                            pen.Color = Color.FromArgb(r, gValue, b);

                                            // 繪製水平線或垂直線
                                            if (HVtype == "H") // 水平線
                                            {
                                                g.DrawLine(pen, 0, location, width, location);
                                            }
                                            else if (HVtype == "V") // 垂直線
                                            {
                                                g.DrawLine(pen, location, 0, location, height);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // 繪製外框線（如果選中）
                        if (tabiefOutside_chk.Checked)
                        {
                            using (Pen borderPen = new Pen(tabiefLineColor_btn.BackColor, 1)) // 外框線顏色和寬度
                            {
                                g.DrawRectangle(borderPen, 0, 0, width - 1, height - 1);
                            }
                        }

                        // 繪製中心對位線（如果選中）
                        if (tabiefCenter_chk.Checked)
                        {
                            using (Pen centerPen = new Pen(tabiefLineColor_btn.BackColor, 1)) // 中心線顏色和寬度
                            {
                                g.DrawLine(centerPen, width / 2, 0, width / 2, height); // 垂直中心線
                                g.DrawLine(centerPen, 0, height / 2, width, height / 2); // 水平中心線
                            }
                        }

                        // 繪製九點對位線（如果選中）
                        if (tabiefNine_chk.Checked)
                        {
                            using (Pen ninePointPen = new Pen(tabiefLineColor_btn.BackColor, 1)) // 九點線顏色和寬度
                            {
                                g.DrawLine(ninePointPen, width / 8, 0, width / 8, height); // 左垂直線
                                g.DrawLine(ninePointPen, 7 * width / 8, 0, 7 * width / 8, height); // 右垂直線
                                g.DrawLine(ninePointPen, 0, height / 8, width, height / 8); // 上水平線
                                g.DrawLine(ninePointPen, 0, 7 * height / 8, width, 7 * height / 8); // 下水平線
                            }
                        }

                        mypicture.Setpicture(CurrentBitmap);
                        break;
                    case "gradient":
                        int tabigStep = int.Parse(tabigStep_cmb.Text); // 獲取漸層階數
                        int divisions = int.Parse(tabigDivid_cmb.Text); // 獲取畫面等分數  
                        int startColorValue = int.Parse(tabigFirstLevel_cmb.Text); // 開始顏色值  
                        int endColorValue = int.Parse(tabigLastLevel_cmb.Text); // 結束顏色值  // 獲取漸層階數  
                        int gradientSteps = endColorValue - startColorValue < 0 ? endColorValue - startColorValue + 1 : startColorValue - endColorValue +1; // 獲取漸層階數  
                        int divisionHeight = int.Parse(mnsH_txt.Text) / divisions; // 每等分的高度 

                        int stepValue = 00;
                        MessageBox.Show($"stepValue={stepValue}"); // 測試用
                        if (tabigHWay_rdo.Checked) // 如果選擇了橫向漸層  
                        {
                            for (int i = 0; i < divisions; i++)
                            {
                                for (int j = 0; j < gradientSteps; j++)
                                    {
                                        int currentValue = startColorValue + j * stepValue;
                                        Color gradientColor = Color.FromArgb(
                                            Math.Min(tabigBaseColor_lbl.BackColor.R, currentValue),
                                            Math.Min(tabigBaseColor_lbl.BackColor.G, currentValue),
                                            Math.Min(tabigBaseColor_lbl.BackColor.B, currentValue)
                                        );
                                        using (Brush brush = new SolidBrush(gradientColor))
                                        {
                                            int yStart = i * divisionHeight + (j * divisionHeight / gradientSteps);
                                            int yEnd = yStart + (divisionHeight / gradientSteps);
                                            g.FillRectangle(brush, 0, yStart, int.Parse(mnsW_txt.Text), yEnd - yStart + 1);
                                        }
                                    }
                            }
                        }
                        else if (tabigVWay_rdo.Checked) // 如果選擇了縱向漸層
                        {
                            int divisionWidth = int.Parse(mnsW_txt.Text) / divisions; // 每等分的寬度 
                            for (int i = 0; i < divisions; i++)
                            {
                                for (int j = 0; j < gradientSteps; j++)
                                {
                                    int currentValue = startColorValue + j * stepValue;
                                    Color gradientColor = Color.FromArgb(
                                        Math.Min(tabigBaseColor_lbl.BackColor.R, currentValue),
                                        Math.Min(tabigBaseColor_lbl.BackColor.G, currentValue),
                                        Math.Min(tabigBaseColor_lbl.BackColor.B, currentValue)
                                    );
                                    using (Brush brush = new SolidBrush(gradientColor))
                                    {
                                        int xStart = i * divisionWidth + (j * divisionWidth / gradientSteps);
                                        int xEnd = xStart + (divisionWidth / gradientSteps);
                                        g.FillRectangle(brush, xStart, 0, xEnd - xStart + 1, int.Parse(mnsH_txt.Text));
                                    }
                                }
                            }
                        }
                        else if (tabigColorBar_rdo.Checked) // 如果選擇了colorbar
                        {

                        }

                        mypicture.Setpicture(CurrentBitmap);
                        break;
                    case "chess":
                        for (int y = 0; y < height; y += height / (int)tabiecVNum_nud.Value)
                        {
                            for (int x = 0; x < width; x += width / (int)tabiecHNum_nud.Value)
                            {
                                // 計算顏色深淺
                                int intensity = tabiecGray_hsc.Value;
                                Color baseColor = tabiecGray_lbl.BackColor;
                                Color adjustedColor = Color.FromArgb(
                                    Math.Min(baseColor.R, intensity),
                                    Math.Min(baseColor.G, intensity),
                                    Math.Min(baseColor.B, intensity)
                                );

                                // 決定區塊顏色
                                Color blockColor = ((x / (width / (int)tabiecHNum_nud.Value)) + (y / (height / (int)tabiecVNum_nud.Value))) % 2 == 0 ? adjustedColor : Color.Black;

                                using (Brush brush = new SolidBrush(blockColor))
                                {
                                    int blockWidth = Math.Min(width / (int)tabiecHNum_nud.Value, width - x);
                                    int blockHeight = Math.Min(height / (int)tabiecVNum_nud.Value, height - y);
                                    g.FillRectangle(brush, x, y, blockWidth, blockHeight);
                                }
                            }
                        }
                        mypicture.Setpicture(CurrentBitmap);
                        break;
                    case "window":
                        if (tabiwBackColor_rdo.Checked) // 純色背景
                        {
                            g.Clear(tabiwBack_pic.BackColor);
                        }
                        else if (tabiwBackImg_rdo.Checked) // 圖片背景
                        {
                            g.DrawImage(tabiwBack_pic.Image, 0, 0, width, height);
                        }
                        int percent = (int)tabiwWinSizePercent_nud.Value; // 獲取百分比大小
                                                                          // 百分比大小 // 實際大小
                        int newWidth = tabiwWinSizePercent_rdo.Checked ? int.Parse(mnsW_txt.Text) * percent / 100 : int.Parse(tabiwWinSizePixelW_nud.Text);
                        int newHeight = tabiwWinSizePercent_rdo.Checked ? int.Parse(mnsH_txt.Text) * percent / 100 : int.Parse(tabiwWinSizePixelH_nud.Text);
                        int xstart = 0;
                        int ystart = 0;
                        if (tabiwWinLocCenter_rdo.Checked) // 置中
                        {
                            xstart = (width - newWidth) / 2;
                            ystart = (height - newHeight) / 2;
                        }
                        else if (tabiwWinLocPixcel_rdo.Checked)
                        {
                            xstart = (int)tabiwWinLocPixcelX_nud.Value;
                            ystart = (int)tabiwWinLocPixcelY_nud.Value;
                        }
                        else if (tabiwWinLocTwo_rdo.Checked)
                        {
                            xstart = width / 5;
                            ystart = height / 5;
                            newWidth = xstart;
                            newHeight = ystart;
                            if (tabiwWinColor_rdo.Checked) // 純色方塊
                            {
                                g.FillRectangle(new SolidBrush(tabiwWin_pic.BackColor), xstart, ystart, newWidth, newHeight);
                            }
                            else if (tabiwWinImg_rdo.Checked)// 繪製圖片方塊
                            {
                                g.DrawImage(tabiwWin_pic.Image, xstart, ystart, newWidth, newHeight);
                            }
                            xstart = width / 5 * 3;
                            ystart = height / 5 * 3;
                        }

                        if (tabiwWinColor_rdo.Checked) // 純色方塊
                        {
                            g.FillRectangle(new SolidBrush(tabiwWin_pic.BackColor), xstart, ystart, newWidth, newHeight);
                        }
                        else if (tabiwWinImg_rdo.Checked)// 繪製圖片方塊
                        {
                            g.DrawImage(tabiwWin_pic.Image, xstart, ystart, newWidth, newHeight);
                        }

                        mypicture.Setpicture(CurrentBitmap);
                        break;
                    case "mask":
                        // 這裡可以添加遮罩的生成邏輯
                        if (tabiemPixel_rdo.Checked)
                        {
                            int pixelW = (int)tabiemWNum_nud.Value;
                            int pixelH = (int)tabiemHNum_nud.Value;

                            using (Bitmap pixelimage = new Bitmap(pixelW, pixelH))
                            {
                                // 遍歷 tabiemPixelPanel_pnl 的子 Panel  
                                foreach (Control control in tabiemPixelPanel_pnl.Controls)
                                {
                                    if (control is Panel panel)
                                    {
                                        // 解析 Panel 的座標 (Tag 格式為 "x,y")  
                                        string[] coordinates = panel.Tag.ToString().Split(',');
                                        int x = int.Parse(coordinates[0]);
                                        int y = int.Parse(coordinates[1]);

                                        // 使用 Panel 的背景顏色填充對應的像素  
                                        pixelimage.SetPixel(x, y, panel.BackColor);
                                    }
                                }

                                // 將 pixelimage 填滿 currentBitmap  
                                for (int j = 0; j < CurrentBitmap.Height; j += pixelH)
                                {
                                    for (int i = 0; i < CurrentBitmap.Width; i += pixelW)
                                    {
                                        g.DrawImage(pixelimage, i, j);
                                    }
                                }
                            }
                        }
                        else if (tabiemSubPixel_rdo.Checked)
                        {
                            //123
                            int pixelW = (int)tabiemWNum_nud.Value;
                            int pixelH = (int)tabiemHNum_nud.Value;

                            using (Bitmap pixelimage = new Bitmap(pixelW, pixelH))
                            {

                                // 遍歷 tabiemPixelPanel_pnl 的子 Panel，每三個一組組合成 Color.FromArgb() 的三個參數  
                                for (int i = 0; i < tabiemPixelPanel_pnl.Controls.Count; i += 3)
                                {
                                    if (i + 2 < tabiemPixelPanel_pnl.Controls.Count)
                                    {
                                        // 獲取三個 Panel  
                                        Panel panel1 = tabiemPixelPanel_pnl.Controls[i] as Panel;
                                        Panel panel2 = tabiemPixelPanel_pnl.Controls[i + 1] as Panel;
                                        Panel panel3 = tabiemPixelPanel_pnl.Controls[i + 2] as Panel;

                                        if (panel1 != null && panel2 != null && panel3 != null)
                                        {
                                            // 根據 Panel 的背景顏色組合成 Color  
                                            int subpixelr = panel1.BackColor.R;
                                            int subpixelg = panel2.BackColor.G;
                                            int subpixelb = panel3.BackColor.B;

                                            Color combinedColor = Color.FromArgb(subpixelr, subpixelg, subpixelb);

                                            // 解析 Panel 的座標 (Tag 格式為 "x,y")  
                                            string[] coordinates = panel1.Tag.ToString().Split(',');
                                            int x = int.Parse(coordinates[0]) / 3;
                                            int y = int.Parse(coordinates[1]);
                                            // 在這裡可以使用 combinedColor，例如：  
                                            pixelimage.SetPixel(x, y, combinedColor);
                                        }
                                    }
                                }


                                // 將 pixelimage 填滿 currentBitmap  
                                for (int j = 0; j < CurrentBitmap.Height; j += pixelH)
                                {
                                    for (int i = 0; i < CurrentBitmap.Width; i += pixelW)
                                    {
                                        g.DrawImage(pixelimage, i, j);
                                    }
                                }
                            }
                        }
                        mypicture.Setpicture(CurrentBitmap);
                        break;
                    case "adjust":
                        // 這裡可以添加調整圖片的邏輯
                        break;
                    case "mass":
                        // 這裡可以添加批量處理圖片的邏輯
                        break;
                    default:
                        throw new NotSupportedException("不支援的圖片類型！");
                }
            }
        }

        private async void tabieaTrans_btn_Click(object sender, EventArgs e)
        {
            ssrProgressbar_prg.Visible = true;
            ssrProgressbar_prg.Maximum = tabieaPatternList_chl.CheckedItems.Count;
            ssrProgressbar_prg.Value = 0; // 初始化進度條值

            using (Bitmap bitmap = new Bitmap(int.Parse(mnsW_txt.Text), int.Parse(mnsH_txt.Text)))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    if (tabieaUpDown_rdo.Checked)
                    {
                        // 取得上下圖片
                        Bitmap upperImage = new Bitmap(tabieaUpDownU_pic.Image);
                        Bitmap lowerImage = new Bitmap(tabieaUpDownD_pic.Image);
                        // 建立組合後的圖片
                        // 繪製上方圖片
                        g.DrawImage(upperImage, 0, 0, bitmap.Width, bitmap.Height / 2);
                        // 繪製下方圖片
                        g.DrawImage(lowerImage, 0, bitmap.Height / 2, bitmap.Width, bitmap.Height / 2);
                        // 更新檔案儲存路徑
                        using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                        {
                            saveFileDialog.Filter = "BMP|*.bmp|PNG|*.png|JPEG|*.jpg";
                            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                try
                                {
                                    if (upperImage == null || lowerImage == null)
                                    {
                                        throw new InvalidOperationException("沒有圖片可以儲存！");
                                    }
                                    string extension = Path.GetExtension(saveFileDialog.FileName).ToLower();
                                    // C# 8.0支援
                                    //ImageFormat imageFormat = extension switch
                                    //{
                                    //    ".bmp" => ImageFormat.Bmp,
                                    //    ".png" => ImageFormat.Png,
                                    //    ".jpg" => ImageFormat.Jpeg,
                                    //    _ => throw new NotSupportedException("不支援的檔案格式！")
                                    //};
                                    // Replace the switch expression with a switch statement to make it compatible with C# 7.3.  
                                    // Original code:  
                                    // ImageFormat imageFormat = extension switch  
                                    // {  
                                    //     ".bmp" => ImageFormat.Bmp,  
                                    //     ".png" => ImageFormat.Png,  
                                    //     ".jpg" => ImageFormat.Jpeg,  
                                    //     _ => throw new NotSupportedException("不支援的檔案格式！")  
                                    // };  

                                    ImageFormat imageFormat;
                                    switch (extension)
                                    {
                                        case ".bmp":
                                            imageFormat = ImageFormat.Bmp;
                                            break;
                                        case ".png":
                                            imageFormat = ImageFormat.Png;
                                            break;
                                        case ".jpg":
                                            imageFormat = ImageFormat.Jpeg;
                                            break;
                                        default:
                                            throw new NotSupportedException("不支援的檔案格式！");
                                    }
                                    bitmap.Save(saveFileDialog.FileName, imageFormat);
                                    //MessageBox.Show("圖片已儲存成功！");
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show($"圖片儲存失敗：{ex.Message}");
                                }
                            }
                        }

                    }
                    else if (tabieaLeftRight_rdo.Checked)
                    {
                        // 取得上下圖片
                        Bitmap leftImage = new Bitmap(tabieaLeftRightL_pic.Image);
                        Bitmap rightImage = new Bitmap(tabieaLeftRightR_pic.Image);
                        // 建立組合後的圖片
                        // 繪製左方圖片
                        g.DrawImage(leftImage, 0, 0, bitmap.Width / 2, bitmap.Height);
                        // 繪製右方圖片
                        g.DrawImage(rightImage, bitmap.Width / 2, 0, bitmap.Width / 2, bitmap.Height);
                        // 更新檔案儲存路徑
                        using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                        {
                            saveFileDialog.Filter = "BMP|*.bmp|PNG|*.png|JPEG|*.jpg";
                            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                try
                                {
                                    if (leftImage == null || rightImage == null)
                                    {
                                        throw new InvalidOperationException("沒有圖片可以儲存！");
                                    }
                                    string extension = Path.GetExtension(saveFileDialog.FileName).ToLower();
                                    ImageFormat imageFormat;
                                    switch (extension)
                                    {
                                        case ".bmp":
                                            imageFormat = ImageFormat.Bmp;
                                            break;
                                        case ".png":
                                            imageFormat = ImageFormat.Png;
                                            break;
                                        case ".jpg":
                                            imageFormat = ImageFormat.Jpeg;
                                            break;
                                        default:
                                            throw new NotSupportedException("不支援的檔案格式！");
                                    }
                                    ;
                                    //ImageFormat imageFormat = extension switch
                                    //{
                                    //    ".bmp" => ImageFormat.Bmp,
                                    //    ".png" => ImageFormat.Png,
                                    //    ".jpg" => ImageFormat.Jpeg,
                                    //    _ => throw new NotSupportedException("不支援的檔案格式！")
                                    //};
                                    bitmap.Save(saveFileDialog.FileName, imageFormat);
                                    //MessageBox.Show("圖片已儲存成功！");
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show($"圖片儲存失敗：{ex.Message}");
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (object iteamChecked in tabieaPatternList_chl.CheckedItems)
                        {
                            string[] words = iteamChecked.ToString().Split('.');
                            string filePath = $"{words[0]}_new.{words[1]}"; // 儲存路徑

                            if (tabieaResize_rdo.Checked)
                            {
                                g.DrawImage(Image.FromFile(iteamChecked.ToString()), 0, 0, int.Parse(mnsW_txt.Text), int.Parse(mnsH_txt.Text));
                                filePath = $"{words[0]}_{mnsW_txt}x{mnsH_txt}.{words[1]}";
                            }
                            else if (tabieaRotate90_rdo.Checked)
                            {
                                using (Bitmap originalBitmap = new Bitmap(iteamChecked.ToString()))
                                {
                                    originalBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                                    g.DrawImage(originalBitmap, 0, 0, int.Parse(mnsW_txt.Text), int.Parse(mnsH_txt.Text));
                                }
                                filePath = $"{words[0]}_Rotate90_{mnsW_txt}x{mnsH_txt}.{words[1]}";
                            }
                            else if (tabieaRotate180_rdo.Checked)
                            {
                                using (Bitmap originalBitmap = new Bitmap(iteamChecked.ToString()))
                                {
                                    originalBitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                                    g.DrawImage(originalBitmap, 0, 0, int.Parse(mnsW_txt.Text), int.Parse(mnsH_txt.Text));
                                }
                                filePath = $"{words[0]}_Rotate180_{mnsW_txt}x{mnsH_txt}.{words[1]}";
                            }
                            else if (tabieaRotate270_rdo.Checked)
                            {
                                using (Bitmap originalBitmap = new Bitmap(iteamChecked.ToString()))
                                {
                                    originalBitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                    g.DrawImage(originalBitmap, 0, 0, int.Parse(mnsW_txt.Text), int.Parse(mnsH_txt.Text));
                                }
                                filePath = $"{words[0]}_Rotate270_{mnsW_txt}x{mnsH_txt}.{words[1]}";
                            }
                            else if (tabieaHFlip_rdo.Checked)
                            {
                                using (Bitmap originalBitmap = new Bitmap(iteamChecked.ToString()))
                                {
                                    originalBitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                                    g.DrawImage(originalBitmap, 0, 0, int.Parse(mnsW_txt.Text), int.Parse(mnsH_txt.Text));
                                }
                                filePath = $"{words[0]}_FlipX_{mnsW_txt}x{mnsH_txt}.{words[1]}";
                            }
                            else if (tabieaVFlip_rdo.Checked)
                            {
                                using (Bitmap originalBitmap = new Bitmap(iteamChecked.ToString()))
                                {
                                    originalBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                                    g.DrawImage(originalBitmap, 0, 0, int.Parse(mnsW_txt.Text), int.Parse(mnsH_txt.Text));
                                }
                                filePath = $"{words[0]}_FlipY_{mnsW_txt}x{mnsH_txt}.{words[1]}";
                            }
                            switch (words[1])
                            {
                                case "bmp":
                                    bitmap.Save(filePath, ImageFormat.Bmp);
                                    break;
                                case "png":
                                    bitmap.Save(filePath, ImageFormat.Png); // 儲存圖片
                                    break;
                                case "jpeg":
                                case "jpg":
                                    bitmap.Save(filePath, ImageFormat.Jpeg);
                                    break;
                                default:
                                    break;
                            }

                            ssrProgressbar_prg.Value++;
                        }
                        for (int i = tabieaPatternList_chl.CheckedItems.Count - 1; i >= 0; i--)
                        {
                            tabieaPatternList_chl.Items.Remove(tabieaPatternList_chl.CheckedItems[i]);
                        }
                    }
                }
            }
            ssrProgressbar_prg.Visible = false; // 在迴圈完成後隱藏進度條
        }

        private void import_Click(object sender, EventArgs e)
        {
            try
            {
                if (sender is Button targetbutton && targetbutton.Text == "導入圖片")
                {
                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.Filter = "Image Files|*.bmp;*.png;*.jpg;*.jpeg";
                        openFileDialog.Multiselect = true; // 啟用多選功能  
                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            foreach (string fileName in openFileDialog.FileNames)
                            {
                                if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
                                {
                                    tabieaPatternList_chl.Items.Add(fileName);
                                    tabieaPatternList_chl.SetItemChecked(tabieaPatternList_chl.Items.Count - 1, true);
                                }
                            }
                        }
                    }
                }
                else
                {
                    // 將載入的圖片設置到 PictureBox
                    showimgPicture_pic.Image = mypicture.OpenImage();
                    showimgPicture_pic.SizeMode = PictureBoxSizeMode.Zoom; // 設置圖片縮放模式
                    showimgSize_btn.Text = $"圖片大小：{mypicture.Getsize()}"; // 更新狀態列顯示圖片大小
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"無法載入圖片：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void pictureBox_DragEnter(object sender, DragEventArgs e)
        {
            // 檢查檔案是否為支援的圖片格式
            bool IsSupportedImageFile(string filePath)
            {
                string extension = Path.GetExtension(filePath).ToLower();
                return extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".bmp";
            }
            // 檢查拖入的資料是否包含檔案，並且是支援的圖片格式
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0 && IsSupportedImageFile(files[0]))
                {
                    e.Effect = DragDropEffects.Copy; // 設置拖放效果為複製
                }
                else
                {
                    e.Effect = DragDropEffects.None; // 不允許拖放
                }
            }
            else
            {
                e.Effect = DragDropEffects.None; // 不允許拖放
            }
        }

        private void pictureBox_DragDrop(object sender, DragEventArgs e)
        {
            // 獲取拖入的檔案
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                string filePath = files[0];
                try
                {
                    // 載入圖片並顯示在 PictureBox 中
                    showimgPicture_pic.Image = mypicture.Setpicture(Image.FromFile(filePath));
                    showimgPicture_pic.SizeMode = PictureBoxSizeMode.Zoom; // 設置圖片縮放模式
                    showimgSize_btn.Text = $"圖片大小：{mypicture.Getsize()}"; // 更新狀態列顯示圖片大小
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"無法載入圖片：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void tabigOther_btn_Click(object sender, EventArgs e)
        {
            tabigOther_pnl.Enabled = !tabigOther_pnl.Enabled;  // 顯示Gradient的其他顏色
        }
        private void SetButtonBackgroundColor(object sender, EventArgs e)
        {
            // 將 targetButton 修改為事件的觸發按鈕  
            if (sender is Button targetButton)
            {
                using (ColorDialog colorDialog = new ColorDialog())
                {
                    colorDialog.FullOpen = true; // 顯示進階選項  
                                                 // 顯示顏色選擇對話框  
                    if (colorDialog.ShowDialog() == DialogResult.OK)
                    {
                        // 將選擇的顏色設置為目標按鈕的背景顏色  
                        targetButton.BackColor = colorDialog.Color;
                        mypicture.tag = "color";
                        targetButton.Image = null; // 清除按鈕上的圖片
                    }
                }
            }
        }

        private void gbxfBtnaddclear_Click(object sender, EventArgs e)
        {
            if (sender is Button targetbutton)
            {
                if (targetbutton.Text == "Add")
                {
                    // Frame 增加其他線 
                    if (!string.IsNullOrEmpty(tabiefOther_cmb.Text))
                    {
                        string itemText = $"{tabiefOther_cmb.Text.Substring(0, 1)}:{tabiefOtherLoc_lbl.Text},{tabiefOtherColor_btn.BackColor.R},{tabiefOtherColor_btn.BackColor.G},{tabiefOtherColor_btn.BackColor.B}";
                        if (!tabiefOther_lst.Items.Contains(itemText))
                        {
                            // 將格式化字串新增到 gbxfLst  
                            tabiefOther_lst.Items.Add(itemText);
                        }
                    }
                }
                else if (targetbutton.Text == "Clear")
                {
                    //清空Framelst的內容
                    tabiefOther_lst.Items.Clear();
                }
            }
        }

        private void tabiefOther_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabiefOther_cmb.Text == "HLine")
            {
                tabiefOtherLoc_hsc.Maximum = (int.Parse(mnsH_txt.Text)); // 將 msTxtH.Text 轉換為 int  
            }
            else if (tabiefOther_cmb.Text == "VLine")
            {
                tabiefOtherLoc_hsc.Maximum = (int.Parse(mnsW_txt.Text)); // 將 msTxtW.Text 轉換為 int  
            }
        }


        private void Save_Click(object sender, EventArgs e)
        {
            if (tabiefMass_chk.Checked && tabimgeFuncList.Text == "Frame&CrossLine&Border")
            {
                // 批量儲存圖片
                Color co = tabiefBack_btn.BackColor;
                for (int i = 0; i <= Math.Max(tabiefBack_btn.BackColor.R, Math.Max(tabiefBack_btn.BackColor.G, tabiefBack_btn.BackColor.B)); i += (int)tabiefMassNum_nud.Value)
                {
                    string fileName = $"{mnsPattername_txt.Text}_{i}.png"; // 設定檔名
                    string filePath = Path.Combine(Application.StartupPath, fileName); // 儲存路徑
                    using (Bitmap bitmap = new Bitmap(int.Parse(mnsW_txt.Text), int.Parse(mnsH_txt.Text)))
                    {
                        using (Graphics g = Graphics.FromImage(bitmap))
                        {
                            g.Clear(Color.FromArgb(i <= co.R ? i : co.R, i <= co.G ? i : co.G, i <= co.B ? i : co.B)); // 填充背景顏色)); // 填充背景顏色
                        }
                        bitmap.Save(filePath, ImageFormat.Png); // 儲存圖片
                    }
                }
            }
            else
            {
                // 儲存單張圖片
                mypicture.SaveImage();
            }
        }

        private void hScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            tabiecGray_lbl.Text = $"Gary: {tabiecGray_hsc.Value.ToString()}"; // 更新 Label 顯示當前值
        }

        private void GetButtonBackgroundColor(object sender, EventArgs e)
        {
            if (sender is Button targetButton)
            {
                if (targetButton.Text == "")
                {
                    SetButtonBackgroundColor(sender, e);
                }
                if ((targetButton.BackColor.R + targetButton.BackColor.G + targetButton.BackColor.B) / 3 < 127)
                {
                    tabiecGray_lbl.ForeColor = Color.White;
                }
                else
                {
                    tabiecGray_lbl.ForeColor = Color.Black;
                }
                tabiecGray_lbl.BackColor = targetButton.BackColor;
                tabiecGray_hsc.Maximum = Math.Max(targetButton.BackColor.R, Math.Max(targetButton.BackColor.G, targetButton.BackColor.B));
                tabiecGray_hsc.Value = tabiecGray_hsc.Maximum;
            }
        }

        private void GradientColor_Click(object sender, EventArgs e)
        {
            if (sender is Button targetbutton)
            {
                if (targetbutton.Text == "")
                {
                    tabigBaseColor_lbl.BackColor = Color.White;
                    tabigBaseColor_lbl.Text = "        ";
                }
                else
                {
                    tabigBaseColor_lbl.Image = null;
                    tabigBaseColor_lbl.BackColor = targetbutton.BackColor;
                    tabigBaseColor_lbl.Text = "基底色彩";
                }
                tabigOther_pnl.Enabled = false; // 隱藏顏色選擇面板
            }
        }

        private void windowPictureBox_Click(object sender, EventArgs e)
        {
            if (sender is PictureBox targetPictureBox)
            {
                if (targetPictureBox.Name == "tabiwBack_pic")
                {
                    if (tabiwBackColor_rdo.Checked)
                    {
                        using (ColorDialog colorDialog = new ColorDialog())
                        {
                            colorDialog.FullOpen = true; // 顯示進階選項  
                                                         // 顯示顏色選擇對話框  
                            if (colorDialog.ShowDialog() == DialogResult.OK)
                            {
                                targetPictureBox.BackColor = colorDialog.Color;
                                targetPictureBox.Image = null; // 清除按鈕上的圖片
                            }
                        }
                    }
                    else if (tabiwBackImg_rdo.Checked)
                    {
                        using (OpenFileDialog openFileDialog = new OpenFileDialog())
                        {
                            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                            if (openFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                targetPictureBox.Image = Image.FromFile(openFileDialog.FileName);
                                targetPictureBox.BackColor = Color.Transparent;
                                targetPictureBox.Text = "";
                            }
                        }
                    }
                }
                else if (targetPictureBox.Name == "tabiwWin_pic")
                {
                    if (tabiwWinColor_rdo.Checked)
                    {
                        using (ColorDialog colorDialog = new ColorDialog())
                        {
                            colorDialog.FullOpen = true; // 顯示進階選項  
                                                         // 顯示顏色選擇對話框  
                            if (colorDialog.ShowDialog() == DialogResult.OK)
                            {
                                targetPictureBox.BackColor = colorDialog.Color;
                                targetPictureBox.Image = null; // 清除按鈕上的圖片
                            }
                        }
                    }
                    else if (tabiwWinImg_rdo.Checked)
                    {
                        using (OpenFileDialog openFileDialog = new OpenFileDialog())
                        {
                            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                            if (openFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                targetPictureBox.Image = Image.FromFile(openFileDialog.FileName);
                                targetPictureBox.BackColor = Color.Transparent;
                                targetPictureBox.Text = "";
                            }
                        }
                    }
                }

            }
            else if (sender is RadioButton targetRadio)
            {
                if (targetRadio.Name == "tabiwBackColor_rdo")
                {
                    using (ColorDialog colorDialog = new ColorDialog())
                    {
                        colorDialog.FullOpen = true; // 顯示進階選項  
                                                     // 顯示顏色選擇對話框  
                        if (colorDialog.ShowDialog() == DialogResult.OK)
                        {
                            tabiwBack_pic.BackColor = colorDialog.Color;
                            tabiwBack_pic.Image = null; // 清除按鈕上的圖片
                        }
                    }
                }
                else if (targetRadio.Name == "tabiwBackImg_rdo")
                {
                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {

                            tabiwBack_pic.Image = Image.FromFile(openFileDialog.FileName);
                            tabiwBack_pic.BackColor = Color.Transparent;
                        }
                    }
                }
                else if (targetRadio.Name == "tabiwWinColor_rdo")
                {
                    using (ColorDialog colorDialog = new ColorDialog())
                    {
                        colorDialog.FullOpen = true; // 顯示進階選項  
                                                     // 顯示顏色選擇對話框  
                        if (colorDialog.ShowDialog() == DialogResult.OK)
                        {
                            tabiwWin_pic.BackColor = colorDialog.Color;
                            tabiwWin_pic.Image = null; // 清除按鈕上的圖片
                        }
                    }
                }
                else if (targetRadio.Name == "tabiwWinImg_rdo")
                {
                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            tabiwWin_pic.Image = Image.FromFile(openFileDialog.FileName);
                            tabiwWin_pic.BackColor = Color.Transparent;
                        }
                    }
                }

            }
        }

        private void tabieaPatternList_chL_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (sender is CheckedListBox targetcheck && targetcheck.Text != "")
            {
                showimgPicture_pic.Image = Image.FromFile(targetcheck.Text);
                showimgPicture_pic.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }

        private void tabieaClear_btn_Click(object sender, EventArgs e)
        {
            tabieaPatternList_chl.Items.Clear();
        }

        private void tabiea_rdo_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton targetradio)
            {
                if (targetradio.Name == "tabieaUpDown_rdo")
                {
                    tabieaImport_btn.Enabled = false;
                    tabieaClear_btn.Enabled = false;
                    tabieaPatternList_chl.Visible = false;
                    tabieaLeftRight_pnl.Visible = false;
                    tabieaUpDown_pnl.Dock = DockStyle.Fill;
                    tabieaUpDown_pnl.Visible = true;

                }
                else if (targetradio.Name == "tabieaLeftRight_rdo")
                {
                    tabieaImport_btn.Enabled = false;
                    tabieaClear_btn.Enabled = false;
                    tabieaPatternList_chl.Visible = false;
                    tabieaUpDown_pnl.Visible = false;
                    tabieaLeftRight_pnl.Dock = DockStyle.Fill;
                    tabieaLeftRight_pnl.Visible = true;
                }
                else
                {
                    tabieaImport_btn.Enabled = true;
                    tabieaClear_btn.Enabled = true;
                    tabieaUpDown_pnl.Visible = false;
                    tabieaLeftRight_pnl.Visible = false;
                    tabieaPatternList_chl.Dock = DockStyle.Fill;
                    tabieaPatternList_chl.Visible = true;
                }
            }
        }

        private void tabiea_pic_Click(object sender, EventArgs e)
        {
            if (sender is PictureBox targetPicture)
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        targetPicture.Image = Image.FromFile(openFileDialog.FileName);
                    }
                }
        }

        private void MaskPanelCreat(object sender, EventArgs e)
        {
            if (sender is Control control && control != null)
            {
                Panel targetPanel;
                int width;
                int height;
                int blockWidth;
                int blockHeight;
                string pixelType;
                VScrollBar brightnessvsc;
                if (control.Parent != null && control.Parent.Name == "tabimgeMask")
                {
                    // 取得並清空面板
                    targetPanel = tabiemPixelPanel_pnl;
                    targetPanel.Controls.Clear();
                    // 取得寬度與高度
                    width = (int)tabiemWNum_nud.Value;
                    height = (int)tabiemHNum_nud.Value;
                    pixelType = tabiemPixel_rdo.Checked ? "Pixel" : "SubPixel";
                    // 設定亮度來源
                    brightnessvsc = tabiemPixelGray_vsc;
                }
                else
                {
                    // 取得並清空面板
                    targetPanel = tabiwCMaskPixelPanel_pnl;
                    targetPanel.Controls.Clear();
                    // 取得寬度與高度
                    width = (int)tabiwCMaskWNum_nud.Value;
                    height = (int)tabiwCMaskHNum_nud.Value;
                    pixelType = tabiwCMaskPixel_rdo.Checked ? "Pixel" : "SubPixel";
                    // 設定亮度來源
                    brightnessvsc = tabiwCMaskPixelGray_vsc;
                }
                // 設定每個區塊的H
                blockHeight = targetPanel.Height / height;

                if (pixelType == "Pixel")
                {
                    // 設定每個區塊的W
                    blockWidth = targetPanel.Width / width;
                    // 生成Pixel區塊
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            Panel block = new Panel
                            {
                                Size = new Size(blockWidth, blockHeight),
                                Location = new Point(x * blockWidth, y * blockHeight),
                                BorderStyle = BorderStyle.FixedSingle,
                                Tag = $"{x},{y}" // 儲存區塊的座標
                            };

                            // 設定點擊事件以填充色 設定滑鼠右鍵事件以清除顏色
                            block.MouseDown += (s, args) =>
                            {
                                if (args.Button == MouseButtons.Right)
                                {
                                    block.BackColor = Color.Transparent;
                                }
                                else if (args.Button == MouseButtons.Left)
                                {
                                    if (block.Parent is Panel parentpanel)
                                    {
                                        if (parentpanel.Parent is GroupBox grandbox)
                                        {
                                            Button targetButton = grandbox.Controls.OfType<Button>()
                                            .FirstOrDefault(button => button.Name.Contains("PixelColor")); // 替換為目標 Label 的名稱
                                            block.BackColor = targetButton.BackColor;
                                        }
                                    }
                                }
                            };
                            block.MouseEnter += (s, args) =>
                            {
                                if (block.Parent is Panel parentpanel)
                                {
                                    if (parentpanel.Parent is GroupBox grandbox)
                                    {
                                        Label targetLabel = grandbox.Controls.OfType<Label>()
                                        .FirstOrDefault(label => label.Name.Contains("PixelLoc_lbl")); // 替換為目標 Label 的名稱
                                        targetLabel.Text = $"{block.Tag}"; // 更新 Label 顯示區塊座標
                                    }
                                }
                            };

                            targetPanel.Controls.Add(block);
                        }
                    }
                }
                else if (pixelType == "SubPixel")
                {
                    width = width * 3;
                    // 設定每個區塊的W
                    blockWidth = targetPanel.Width / width;

                    // 生成SubPixel區塊
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            Panel block = new Panel
                            {
                                Size = new Size(blockWidth, blockHeight),
                                Location = new Point(x * blockWidth, y * blockHeight),
                                BackColor = x % 3 == 0 ? Color.FromArgb(255 - brightnessvsc.Value, 0, 0) : x % 3 == 1 ? Color.FromArgb(0, 255 - brightnessvsc.Value, 0) : Color.FromArgb(0, 0, 255 - brightnessvsc.Value),
                                BorderStyle = BorderStyle.FixedSingle,
                                Tag = $"{x},{y}" // 儲存區塊的座標
                            };

                            // 設定右鍵點擊事件以填充色 設定滑鼠左鍵事件以清除顏色
                            block.MouseDown += (s, args) =>
                            {
                                if (args.Button == MouseButtons.Right)
                                {
                                    int locx = int.Parse(block.Tag.ToString().Split(',')[0]);
                                    block.BackColor = locx % 3 == 0 ? Color.FromArgb(255 - brightnessvsc.Value, 0, 0) : locx % 3 == 1 ? Color.FromArgb(0, 255 - brightnessvsc.Value, 0) : Color.FromArgb(0, 0, 255 - brightnessvsc.Value);
                                }
                                else if (args.Button == MouseButtons.Left)
                                {
                                    block.BackColor = Color.Black;
                                }
                            };
                            block.MouseEnter += (s, args) =>
                            {
                                if (block.Parent is Panel parentpanel)
                                {
                                    if (parentpanel.Parent is GroupBox grandbox)
                                    {
                                        Label targetLabel = grandbox.Controls.OfType<Label>()
                                        .FirstOrDefault(label => label.Name.Contains("PixelLoc_lbl")); // 替換為目標 Label 的名稱
                                        targetLabel.Text = $"{block.Tag}"; // 更新 Label 顯示區塊座標
                                    }
                                }
                            };
                            targetPanel.Controls.Add(block);
                        }
                    }
                }
            }
        }

        private void Pixel_rdo_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton targetradio)
            {
                if (targetradio.Parent is GroupBox parentbox)
                {
                    VScrollBar targetVsc = parentbox.Controls.OfType<VScrollBar>()
                    .FirstOrDefault(vsc => vsc.Name.Contains("PixelGray"));
                    targetVsc.Visible = targetradio.Name.Contains("SubPixel");

                    Label targetLbl = parentbox.Controls.OfType<Label>()
                    .FirstOrDefault(lbl => lbl.Name.Contains("PixelGray"));
                    targetLbl.Visible = targetradio.Name.Contains("SubPixel");

                    targetLbl = parentbox.Controls.OfType<Label>()
                    .FirstOrDefault(lbl => lbl.Name.Contains("PixelColor"));
                    targetLbl.Visible = !targetradio.Name.Contains("SubPixel");

                    Button targetBtn = parentbox.Controls.OfType<Button>()
                    .FirstOrDefault(btn => btn.Name.Contains("PixelColor"));
                    targetBtn.Visible = !targetradio.Name.Contains("SubPixel");
                }
            }
            MaskPanelCreat(sender, e);
        }

        private void tabiwCustom_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is ComboBox targetcombo)
            {
                if (targetcombo.Name == "tabiwCustom_cmb")
                {
                    if (targetcombo.Text == "Mask")
                    {
                        tabiwCustomGrad_grp.Visible = false;
                        tabiwCustomMask_grp.Visible = true;
                    }
                    else
                    {
                        tabiwCustomMask_grp.Visible = false;
                        tabiwCustomGrad_grp.Visible = true;
                    }
                }
            }
        }

        private void PixelGray_vsc_ValueChanged(object sender, EventArgs e)
        {
            if (sender is VScrollBar targetvsc)
            {
                if (targetvsc.Parent is GroupBox parentbox)
                {
                    Label targetLbl = parentbox.Controls.OfType<Label>()
                    .FirstOrDefault(lbl => lbl.Name.Contains("PixelGray"));
                    targetLbl.Text = $"Gray: {255 - targetvsc.Value}"; // 更新 Label 顯示當前值

                    Panel targetPnl = parentbox.Controls.OfType<Panel>()
                    .FirstOrDefault(pnl => pnl.Name.Contains("PixelPanel"));
                    foreach (Control control in targetPnl.Controls)
                    {
                        if (control is Panel panel && panel.BackColor != Color.Transparent)
                        {
                            int x = int.Parse(panel.Tag.ToString().Split(',')[0]);
                            panel.BackColor = x % 3 == 0 ? Color.FromArgb(255 - targetvsc.Value, 0, 0) : x % 3 == 1 ? Color.FromArgb(0, 255 - targetvsc.Value, 0) : Color.FromArgb(0, 0, 255 - targetvsc.Value);
                        }
                    }
                }
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
            if (sender is ToolStripMenuItem menuItem && menuItem.Owner is ContextMenuStrip contextMenu && contextMenu.SourceControl is PictureBox targetPictureBox)
            {
                if (cmsShowLoc.ForeColor != Color.Red)
                {
                    // 取消訂閱 PictureBox 的 MouseMove 事件  
                    targetPictureBox.MouseMove -= PictureBox_MouseMove;
                    cmsShowLoc.ForeColor = Color.Red;
                }
                else
                {
                    // 訂閱 PictureBox 的 MouseMove 事件  
                    targetPictureBox.MouseMove += PictureBox_MouseMove;
                    cmsShowLoc.ForeColor = Color.Black;
                }
            }

        }
        private bool IsMouseMoveBound(Object targetObject, String targerString)
        {
            var fieldInfo = typeof(Control).GetField(targerString, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            if (fieldInfo != null)
            {
                var eventKey = fieldInfo.GetValue(null);
                var eventHandlerList = targetObject.GetType().GetProperty("Events", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(targetObject) as EventHandlerList;

                if (eventHandlerList != null)
                {
                    var eventDelegate = eventHandlerList[eventKey] as Delegate;
                    return eventDelegate != null && eventDelegate.GetInvocationList().Length > 0;
                }
            }
            return false;
        }

    }
}