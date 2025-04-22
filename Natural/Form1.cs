using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Natural;
using Natural.Properties;
using Natural1;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.DataFormats;


namespace Natural
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
            if (msCmblist.Items.Count > 1)
            {
                msCmblist.SelectedIndex = 1;
            }
            else if (msCmblist.Items.Count > 0)
            {
                msCmblist.SelectedIndex = 0;
            }
            cmbFunclist.SelectedIndex = 3; // 預設選擇第一個項目
        }
        public MyPicture mypicture { get; private set; }

        public class MyPicture
        {
            private Bitmap _currentBitmap;
            //private int _width;
            private int _width;
            private int _height;
            private Mainwindow m1 { get; }
            public MyPicture(Mainwindow m0)
            {
                m1 = m0 ?? throw new ArgumentNullException(nameof(m0));
                _width = int.Parse(m1.msTxtW.Text);
                _height = int.Parse(m1.msTxtH.Text);
                _currentBitmap = new Bitmap(_width, _height);
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
                        saveFileDialog.Filter = "JPEG|*.jpg|PNG|*.png|BMP|*.bmp";
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            try
                            {

                                if (_currentBitmap == null)
                                {
                                    throw new InvalidOperationException("沒有圖片可以儲存！");
                                }

                                string extension = Path.GetExtension(saveFileDialog.FileName).ToLower();
                                ImageFormat imageFormat = extension switch
                                {
                                    ".jpg" => ImageFormat.Jpeg,
                                    ".png" => ImageFormat.Png,
                                    ".bmp" => ImageFormat.Bmp,
                                    _ => throw new NotSupportedException("不支援的檔案格式！")
                                };
                                _currentBitmap.Save(saveFileDialog.FileName, imageFormat);

                                MessageBox.Show("圖片已儲存成功！");
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
                return _currentBitmap; // 回傳圖片
            }
        }

        private void cmbFunclist_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 隱藏所有 GroupBox
            gbxFrame.Visible = false;
            groupBoxGradient.Visible = false;
            groupBoxChess.Visible = false;
            groupBoxWindow.Visible = false;
            groupBoxMask.Visible = false;
            groupBoxAdjust.Visible = false;
            groupBoxMassImage.Visible = false;

            // 根據選擇的項目顯示對應的 GroupBox
            //switch (comboBoxfunc.SelectedItem.ToString())
            switch (cmbFunclist.Text)
            {
                case "Frame&CrossLine&Border":
                    gbxFrame.Visible = true;
                    gbxFrame.Dock = DockStyle.Fill;
                    break;
                case "Gradient&ColorBar":
                    groupBoxGradient.Visible = true;
                    groupBoxGradient.Dock = DockStyle.Fill;
                    Gradrdovway.Checked = true; // 預設選擇 Gradient 方式
                    comboBox1.Text = "256"; // 預設選擇 256 階
                    comboBox2.Text = "1"; // 預設選擇 1 階
                    comboBox3.Text = "0"; // 預設從 0 階開始
                    comboBox4.Text = "255"; // 預設到 255 階結束
                    break;
                case "Chess":
                    groupBoxChess.Visible = true;
                    groupBoxChess.Dock = DockStyle.Fill;
                    break;
                case "Window":
                    groupBoxWindow.Visible = true;
                    groupBoxWindow.Dock = DockStyle.Fill;
                    //radioButton11.Checked = true; // 預設選擇純色背景
                    //radioButton16.Checked = true; // 預設選擇純色方塊
                    radioButton17.Checked = true; // 預設選擇百分比大小
                    numericUpDown6.Value = 50; // 預設選擇 50% 大小
                    radioButton19.Checked = true; // 預設選擇置中位置
                    numericUpDown7.Maximum = int.Parse(msTxtW.Text);
                    numericUpDown8.Maximum = int.Parse(msTxtH.Text);
                    numericUpDown10.Maximum = int.Parse(msTxtW.Text);
                    numericUpDown9.Maximum = int.Parse(msTxtH.Text);
                    break;
                case "Mask(FlickerPattern)":
                    groupBoxMask.Visible = true;
                    groupBoxMask.Dock = DockStyle.Fill;
                    break;
                case "ImageAdjust":
                    groupBoxAdjust.Visible = true;
                    groupBoxAdjust.Dock = DockStyle.Fill;
                    break;
                case "MassImage":
                    groupBoxMassImage.Visible = true;
                    groupBoxMassImage.Dock = DockStyle.Fill;
                    break;
                default:
                    // 如果沒有匹配的項目，則不顯示任何 GroupBox
                    break;
            }
        }

        private void Generate_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(msTxtW.Text, out int width) || !int.TryParse(msTxtH.Text, out int height))
            {
                MessageBox.Show("請輸入有效的寬度與高度！");
                return;
            }

            // 根據 cmbFunclist 的選擇來決定 type  
            string type = cmbFunclist.Text switch
            {
                "Frame&CrossLine&Border" => "solid",
                "Gradient&ColorBar" => "gradient",
                "Chess" => "chess",
                "Window" => "window",
                "Mask(FlickerPattern)" => "mask",
                "ImageAdjust" => "adjust",
                "MassImage" => "mass"
            };

            try
            {
                GenerateImage(type, width, height);
                pictureBox.Image = mypicture.Getpicture();
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                pictureLblsize.Text = $"當前圖片大小：{msTxtW.Text} x {msTxtH.Text}"; // 更新狀態列顯示當前圖片大小  
            }
            catch (Exception ex)
            {
                MessageBox.Show($"圖片生成失敗：{ex.Message}");
            }
        }

        private void FullScreen(object sender, EventArgs e)
        {
            // 獲取選擇的螢幕  
            string selectedScreen = msCmblist.SelectedItem?.ToString();
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
                Image = pictureBox.Image, // 使用原始 PictureBox 的圖片  
                SizeMode = PictureBoxSizeMode.Zoom // 縮放圖片以適應螢幕  
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

            // 顯示全螢幕表單  
            fullScreenForm.ShowDialog();
        }

        private void Screenlist(object sender, EventArgs e)
        {
            // 獲取所有螢幕
            Screen[] screens = Screen.AllScreens;
            // 清空下拉選單
            msCmblist.Items.Clear();
            // 添加螢幕選項到下拉選單
            foreach (Screen screen in screens)
            {
                msCmblist.Items.Add(screen.DeviceName);
            }


        }
        private void menuToolStripCmb_SelectedIndexChanged(object sender, EventArgs e)
        {

            // 獲取選擇的螢幕
            string selectedScreen = msCmblist.SelectedItem.ToString();
            Screen[] screens = Screen.AllScreens;
            // 根據選擇的螢幕更新寬度與高度
            foreach (Screen screen in screens)
            {
                if (screen.DeviceName == selectedScreen)
                {
                    msTxtW.Text = screen.Bounds.Width.ToString();
                    msTxtH.Text = screen.Bounds.Height.ToString();
                    break;
                }
            }
        }
        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = listView.SelectedItems[0]; // 獲取選中的 ListViewItem  
                string imagePath = selectedItem.Tag as string; // 從 Tag 中取出圖片路徑  

                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    // 載入原始圖片到 PictureBox  
                    pictureBox.Image = Image.FromFile(imagePath);
                    pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureLblsize.Text = $"當前圖片大小：{selectedItem.SubItems[2].Text} x {selectedItem.SubItems[3].Text}"; // 更新狀態列顯示當前圖片大小  
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
            listView.Items.Clear();
            listView.Groups.Clear();
            listView.LargeImageList = new ImageList
            {
                ImageSize = new Size(90, 90) // 設置縮略圖大小
            };

            // 定義分組
            ListViewGroup jpgGroup = new ListViewGroup("JPG 圖片", HorizontalAlignment.Left);
            ListViewGroup pngGroup = new ListViewGroup("PNG 圖片", HorizontalAlignment.Left);
            ListViewGroup bmpGroup = new ListViewGroup("BMP 圖片", HorizontalAlignment.Left);

            listView.Groups.Add(jpgGroup);
            listView.Groups.Add(pngGroup);
            listView.Groups.Add(bmpGroup);

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
                        listView.LargeImageList.Images.Add(fileName, new Bitmap(img));

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
                        listView.Items.Add(item);
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
            gbxfLbllocation.Text = gbxfHsblocation.Value.ToString();  // frame 的卷軸值變更
        }

        private void Framechkother_CheckStateChanged(object sender, EventArgs e)
        {
            if (gbxfChkother.Checked)  // frame 的卷軸值顯示
            {
                gbxfLbllocation.Text = gbxfHsblocation.Value.ToString();
            }
            else
            {
                gbxfLbllocation.Text = "";
            }
        }


        public void GenerateImage(string type, int width, int height)
        {
            Bitmap CurrentBitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(CurrentBitmap))
            {
                switch (type)
                {
                    case "solid":
                        // 填充背景顏色
                        g.Clear(gbxfBtnbackcolor.BackColor);

                        // 繪製框線
                        if (gbxfChkother.Checked)
                        {
                            using (Pen pen = new Pen(Color.Black, 2)) // 預設線條顏色和寬度
                            {
                                foreach (string item in gbxfLst.Items)
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
                        if (gbxfChkoutside.Checked)
                        {
                            using (Pen borderPen = new Pen(gbxfBtnLine.BackColor, 1)) // 外框線顏色和寬度
                            {
                                g.DrawRectangle(borderPen, 0, 0, width - 1, height - 1);
                            }
                        }

                        // 繪製中心對位線（如果選中）
                        if (gbxfChkcenter.Checked)
                        {
                            using (Pen centerPen = new Pen(gbxfBtnLine.BackColor, 1)) // 中心線顏色和寬度
                            {
                                g.DrawLine(centerPen, width / 2, 0, width / 2, height); // 垂直中心線
                                g.DrawLine(centerPen, 0, height / 2, width, height / 2); // 水平中心線
                            }
                        }

                        // 繪製九點對位線（如果選中）
                        if (gbxfChknine.Checked)
                        {
                            using (Pen ninePointPen = new Pen(gbxfBtnLine.BackColor, 1)) // 九點線顏色和寬度
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
                        int gradientSteps = int.Parse(comboBox1.Text); // 獲取漸層階數  
                        int divisions = int.Parse(comboBox2.Text); // 獲取畫面等分數  
                        int startColorValue = int.Parse(comboBox3.Text); // 開始顏色值  
                        int endColorValue = int.Parse(comboBox4.Text); // 結束顏色值  

                        int stepValue = (endColorValue - startColorValue >= 0) ?
                            ((endColorValue - startColorValue) + 1) / gradientSteps :
                            ((endColorValue - startColorValue) - 1) / gradientSteps;

                        if (Gradrdohway.Checked) // 如果選擇了橫向漸層  
                        {
                            int divisionHeight = int.Parse(msTxtH.Text) / divisions; // 每等分的高度 
                            for (int i = 0; i < divisions; i++)
                            {
                                for (int j = 0; j < gradientSteps; j++)
                                {
                                    int currentValue = startColorValue + j * stepValue;
                                    Color gradientColor = Color.FromArgb(
                                        Math.Min(label5.BackColor.R, currentValue),
                                        Math.Min(label5.BackColor.G, currentValue),
                                        Math.Min(label5.BackColor.B, currentValue)
                                    );
                                    using (Brush brush = new SolidBrush(gradientColor))
                                    {
                                        int yStart = i * divisionHeight + (j * divisionHeight / gradientSteps);
                                        int yEnd = yStart + (divisionHeight / gradientSteps);
                                        g.FillRectangle(brush, 0, yStart, int.Parse(msTxtW.Text), yEnd - yStart + 1);
                                    }
                                }
                            }
                        }
                        else if (Gradrdovway.Checked) // 如果選擇了縱向漸層
                        {
                            int divisionWidth = int.Parse(msTxtW.Text) / divisions; // 每等分的寬度 
                            for (int i = 0; i < divisions; i++)
                            {
                                for (int j = 0; j < gradientSteps; j++)
                                {
                                    int currentValue = startColorValue + j * stepValue;
                                    Color gradientColor = Color.FromArgb(
                                        Math.Min(label5.BackColor.R, currentValue),
                                        Math.Min(label5.BackColor.G, currentValue),
                                        Math.Min(label5.BackColor.B, currentValue)
                                    );
                                    using (Brush brush = new SolidBrush(gradientColor))
                                    {
                                        int xStart = i * divisionWidth + (j * divisionWidth / gradientSteps);
                                        int xEnd = xStart + (divisionWidth / gradientSteps);
                                        g.FillRectangle(brush, xStart, 0, xEnd - xStart + 1, int.Parse(msTxtH.Text));
                                    }
                                }
                            }
                        }
                        else if (radioButton3.Checked) // 如果選擇了colorbar
                        {

                        }

                        mypicture.Setpicture(CurrentBitmap);
                        break;
                    case "chess":
                        for (int y = 0; y < height; y += height / (int)numericUpDown3.Value)
                        {
                            for (int x = 0; x < width; x += width / (int)numericUpDown2.Value)
                            {
                                // 計算顏色深淺
                                int intensity = hScrollBar1.Value;
                                Color baseColor = label12.BackColor;
                                Color adjustedColor = Color.FromArgb(
                                    Math.Min(baseColor.R, intensity),
                                    Math.Min(baseColor.G, intensity),
                                    Math.Min(baseColor.B, intensity)
                                );

                                // 決定區塊顏色
                                Color blockColor = ((x / (width / (int)numericUpDown2.Value)) + (y / (height / (int)numericUpDown3.Value))) % 2 == 0 ? Color.Black : adjustedColor;

                                using (Brush brush = new SolidBrush(blockColor))
                                {
                                    int blockWidth = Math.Min(width / (int)numericUpDown2.Value, width - x);
                                    int blockHeight = Math.Min(height / (int)numericUpDown3.Value, height - y);
                                    g.FillRectangle(brush, x, y, blockWidth, blockHeight);
                                }
                            }
                        }
                        mypicture.Setpicture(CurrentBitmap);
                        break;
                    case "window":
                        if (radioButton11.Checked) // 純色背景
                        {
                            g.Clear(pictureBox1.BackColor);
                        }
                        else if (radioButton12.Checked) // 圖片背景
                        {
                            g.DrawImage(pictureBox1.BackgroundImage, 0, 0, width, height);
                        }
                        int percent = (int)numericUpDown6.Value; // 獲取百分比大小
                        // 百分比大小 // 實際大小
                        int newWidth = radioButton17.Checked ? int.Parse(msTxtW.Text) * percent / 100: int.Parse(numericUpDown7.Text);
                        int newHeight = radioButton17.Checked ? int.Parse(msTxtH.Text) * percent / 100 : int.Parse(numericUpDown8.Text);
                        int xstart = 0;
                        int ystart = 0;
                        if (radioButton19.Checked) // 置中
                        {
                            xstart = (width - newWidth) / 2;
                            ystart = (height - newHeight) / 2;
                        }else if (radioButton21.Checked)
                        {
                            xstart = (int)numericUpDown10.Value;
                            ystart = (int)numericUpDown9.Value;
                        } else if (radioButton20.Checked)
                        {
                            xstart = width / 5;
                            ystart = height / 5;
                            newWidth = xstart;
                            newHeight = ystart;
                            if (radioButton16.Checked) // 純色方塊
                            {
                                g.FillRectangle(new SolidBrush(pictureBox2.BackColor), xstart, ystart, newWidth, newHeight);
                            }
                            else if (radioButton15.Checked)// 繪製圖片方塊
                            {
                                g.DrawImage(pictureBox2.BackgroundImage, xstart, ystart, newWidth, newHeight);
                            }
                            xstart = width / 5 * 3;
                            ystart = height / 5 * 3;
                        }

                        if (radioButton16.Checked) // 純色方塊
                        {
                            g.FillRectangle(new SolidBrush(pictureBox2.BackColor), xstart, ystart, newWidth, newHeight);
                        }
                        else if (radioButton15.Checked)// 繪製圖片方塊
                        {
                            g.DrawImage(pictureBox2.BackgroundImage, xstart, ystart, newWidth, newHeight);
                        }

                        mypicture.Setpicture(CurrentBitmap);
                        break;
                    case "mask":
                        // 這裡可以添加遮罩的生成邏輯
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




        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {

        }

        private async void button25_Click(object sender, EventArgs e)
        {

            toolStripProgressBar1.Visible = true;
            for (int i = 0; i <= 100; i++)
            {
                toolStripProgressBar1.Value = i;
                await Task.Delay(100); // 更新進度條的延遲
            }
            toolStripProgressBar1.Visible = false; // 在迴圈完成後隱藏進度條
        }

        private void import_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbFunclist.Text == "ImageAdjust")
                {
                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                        openFileDialog.Multiselect = true; // 啟用多選功能  
                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            foreach (string fileName in openFileDialog.FileNames)
                            {
                                checkedListBox1.Items.Add(fileName); // 將選擇的檔案加入到 CheckedListBox
                            }
                        }
                    }
                }
                else
                {
                    // 將載入的圖片設置到 PictureBox
                    pictureBox.Image = mypicture.OpenImage();
                    pictureBox.SizeMode = PictureBoxSizeMode.Zoom; // 設置圖片縮放模式
                    pictureLblsize.Text = $"圖片大小：{mypicture.Getsize()}"; // 更新狀態列顯示圖片大小
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
                    pictureBox.Image = mypicture.Setpicture(Image.FromFile(filePath));
                    pictureBox.SizeMode = PictureBoxSizeMode.Zoom; // 設置圖片縮放模式
                    pictureLblsize.Text = $"圖片大小：{mypicture.Getsize()}"; // 更新狀態列顯示圖片大小
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"無法載入圖片：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void button6_Click(object sender, EventArgs e)
        {
            panel2.Enabled = !panel2.Enabled;  // 顯示Gradient的其他顏色
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
                        targetButton.BackgroundImage = null; // 清除按鈕上的圖片
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
                    if (!string.IsNullOrEmpty(gbxfCmbhnv.Text))
                    {
                        string itemText = $"{gbxfCmbhnv.Text.Substring(0, 1)}:{gbxfLbllocation.Text},{gbxfBtnlinecolor.BackColor.R},{gbxfBtnlinecolor.BackColor.G},{gbxfBtnlinecolor.BackColor.B}";
                        if (!gbxfLst.Items.Contains(itemText))
                        {
                            // 將格式化字串新增到 gbxfLst  
                            gbxfLst.Items.Add(itemText);
                        }
                    }
                }
                else if (targetbutton.Text == "Clear")
                {
                    //清空Framelst的內容
                    gbxfLst.Items.Clear();
                }
            }
        }

        private void gbxfCmbhnv_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gbxfCmbhnv.Text == "HLine")
            {
                gbxfHsblocation.Maximum = (int.Parse(msTxtH.Text)); // 將 msTxtH.Text 轉換為 int  
            }
            else if (gbxfCmbhnv.Text == "VLine")
            {
                gbxfHsblocation.Maximum = (int.Parse(msTxtW.Text)); // 將 msTxtW.Text 轉換為 int  
            }
        }


        private void Save_Click(object sender, EventArgs e)
        {
            if (gbxfChkmass.Checked & cmbFunclist.Text == "Frame&CrossLine&Border")
            {
                // 批量儲存圖片
                Color co = gbxfBtnbackcolor.BackColor;
                for (int i = 0; i <= Math.Max(gbxfBtnbackcolor.BackColor.R, Math.Max(gbxfBtnbackcolor.BackColor.G, gbxfBtnbackcolor.BackColor.B)); i += (int)numericUpDown11.Value)
                {
                    string fileName = $"{toolStripTextBox1.Text}_{i}.png"; // 設定檔名
                    string filePath = Path.Combine(Application.StartupPath, fileName); // 儲存路徑
                    using (Bitmap bitmap = new Bitmap(int.Parse(msTxtW.Text), int.Parse(msTxtH.Text)))
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
            label12.Text = $"Gary: {hScrollBar1.Value.ToString()}"; // 更新 Label 顯示當前值
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
                    label12.ForeColor = Color.White;
                }
                else
                {
                    label12.ForeColor = Color.Black;
                }
                label12.BackColor = targetButton.BackColor;
                hScrollBar1.Maximum = Math.Max(targetButton.BackColor.R, Math.Max(targetButton.BackColor.G, targetButton.BackColor.B));
                hScrollBar1.Value = hScrollBar1.Maximum;
            }
        }

        private void GradientColor_Click(object sender, EventArgs e)
        {
            if (sender is Button targetbutton)
            {
                if (targetbutton.Text == "")
                {
                    label5.BackColor = Color.White;
                    label5.Image = Resources.colorbar;
                    label5.Text = "        ";
                }
                else
                {
                    label5.Image = null;
                    label5.BackColor = targetbutton.BackColor;
                    label5.Text = "基底色彩";
                }
                panel2.Enabled = false; // 隱藏顏色選擇面板
            }
        }

        private void windowPictureBox_Click(object sender, EventArgs e)
        {
            if (sender is PictureBox targetPictureBox)
            {
                if (targetPictureBox.Name == "pictureBox1")
                {
                    if (radioButton11.Checked)
                    {
                        using (ColorDialog colorDialog = new ColorDialog())
                        {
                            colorDialog.FullOpen = true; // 顯示進階選項  
                                                         // 顯示顏色選擇對話框  
                            if (colorDialog.ShowDialog() == DialogResult.OK)
                            {
                                targetPictureBox.BackColor = colorDialog.Color;
                                targetPictureBox.BackgroundImage = null; // 清除按鈕上的圖片
                            }
                        }
                    }
                    else if (radioButton12.Checked)
                    {
                        using (OpenFileDialog openFileDialog = new OpenFileDialog())
                        {
                            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                            if (openFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                targetPictureBox.BackgroundImage = Image.FromFile(openFileDialog.FileName);
                                targetPictureBox.BackColor = Color.Transparent;
                                targetPictureBox.Text = "";
                            }
                        }
                    }
                }
                else if (targetPictureBox.Name == "pictureBox2")
                {
                    if (radioButton16.Checked)
                    {
                        using (ColorDialog colorDialog = new ColorDialog())
                        {
                            colorDialog.FullOpen = true; // 顯示進階選項  
                                                         // 顯示顏色選擇對話框  
                            if (colorDialog.ShowDialog() == DialogResult.OK)
                            {
                                targetPictureBox.BackColor = colorDialog.Color;
                                targetPictureBox.BackgroundImage = null; // 清除按鈕上的圖片
                            }
                        }
                    }
                    else if (radioButton15.Checked)
                    {
                        using (OpenFileDialog openFileDialog = new OpenFileDialog())
                        {
                            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                            if (openFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                targetPictureBox.BackgroundImage = Image.FromFile(openFileDialog.FileName);
                                targetPictureBox.BackColor = Color.Transparent;
                                targetPictureBox.Text = "";
                            }
                        }
                    }
                }

            }
            else if (sender is RadioButton targetRadio)
            {
                if (targetRadio.Name == "radioButton11" || targetRadio.Name == "radioButton12")
                {
                    if (radioButton11.Checked)
                    {
                        using (ColorDialog colorDialog = new ColorDialog())
                        {
                            colorDialog.FullOpen = true; // 顯示進階選項  
                                                         // 顯示顏色選擇對話框  
                            if (colorDialog.ShowDialog() == DialogResult.OK)
                            {
                                pictureBox1.BackColor = colorDialog.Color;
                                pictureBox1.BackgroundImage = null; // 清除按鈕上的圖片
                            }
                        }
                    }
                    else if (radioButton12.Checked)
                    {
                        using (OpenFileDialog openFileDialog = new OpenFileDialog())
                        {
                            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                            if (openFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                pictureBox1.BackgroundImage = Image.FromFile(openFileDialog.FileName);
                                pictureBox1.BackColor = Color.Transparent;
                                pictureBox1.Text = "";
                            }
                        }
                    }
                }
                if (targetRadio.Name == "radioButton16" || targetRadio.Name == "radioButton15")
                { 
                    if (radioButton16.Checked)
                    {
                        using (ColorDialog colorDialog = new ColorDialog())
                        {
                            colorDialog.FullOpen = true; // 顯示進階選項  
                                                         // 顯示顏色選擇對話框  
                            if (colorDialog.ShowDialog() == DialogResult.OK)
                            {
                                pictureBox2.BackColor = colorDialog.Color;
                                pictureBox2.BackgroundImage = null; // 清除按鈕上的圖片
                            }
                        }
                    }
                    else if (radioButton15.Checked)
                    {
                        using (OpenFileDialog openFileDialog = new OpenFileDialog())
                        {
                            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                            if (openFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                pictureBox2.BackgroundImage = Image.FromFile(openFileDialog.FileName);
                                pictureBox2.BackColor = Color.Transparent;
                                pictureBox2.Text = "";
                            }
                        }
                    }
                }
            }
        }
    }
}