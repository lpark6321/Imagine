using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Deployment.Application;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Thunder.Properties;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using Object = System.Object;
using String = System.String;


namespace Thunder
{
    public partial class mainWindow : Form
    {
        //全局變數---------------------------------------------------------------------------------------------------------
        private bool _isMouseDown = false;
        private List<ImageItem> imageItems;
        // 確保有一個全局的 pictureWindow 實例
        private pictureWindow _pictureWindow;
        public static mainWindow Instance { get; private set; }
        public TabControl MainTabControl => tabControl; // 提供 TabControl 的訪問
        //mainWindow---------------------------------------------------------------------------------------------------
        public mainWindow()
        {
            InitializeComponent();
            //Mainwindow_Load(this, EventArgs.Empty);
            mypicture = new MyPicture(this);
            Instance = this; // 初始化 Singleton 實例
        }
        private void Mainwindow_Load(object sender, EventArgs e)
        {
            //var numericUpDownItem = new ToolStripNumericUpDown();
            //numericUpDownItem.NumericUpDownControl.ValueChanged += (s, ev) =>
            //{
            //    MessageBox.Show($"當前值: {numericUpDownItem.NumericUpDownControl.Value}");
            //};
            //menuStrip.Items.Add(numericUpDownItem);
            //AddNumericUpDownToMenuStrip();

            Screenlist(this, EventArgs.Empty);
            if (mnsScreenlist_cmb.Items.Count > 1)
            {
                mnsScreenlist_cmb.SelectedIndex = 1;
            }
            else if (mnsScreenlist_cmb.Items.Count > 0)
            {
                mnsScreenlist_cmb.SelectedIndex = 0;
            }
            tabimgeFuncList.SelectedIndex = 0; // 功能預設選擇第一個項目
            tabieaResize_rdo.Select(); //圖片館例預設選取RESIZE
            MaskPanelCreat(tabiemPixelClear_btn, e); //先產生MaskPanel
            MaskPanelCreat(tabiwCMaskPixelClear_btn, e); //先產生MaskPanel
            SplitPanelCreat(tabigOtherSplit_nud, e);//先產生GradSplitPanel
            SplitPanelCreat(tabiwCGradOtherSplit_nud, e);//先產生GradSplitPanel
            tabControl.SelectedIndex = 2;
            //tabControl_SelectedIndexChanged(sender, e);
        }
        //Class--------------------------------------------------------------------------------------------------------
        public MyPicture mypicture { get; private set; }
        public class MyPicture
        {
            private Bitmap _currentBitmap;
            public string tag;
            private int _width;
            private int _height;

            private mainWindow m1 { get; }
            public MyPicture(mainWindow m0)
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
                            tag = "image";
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
            public string Getsize()
            {
                _width = _currentBitmap.Width;
                _height = _currentBitmap.Height;
                return $"{_width} x {_height}"; // 更新圖片大小
            }
            public Bitmap Getpicture()
            {
                if (_currentBitmap == null)
                {
                    MessageBox.Show("圖片尚未初始化！");
                    return null;
                }
                return _currentBitmap;
            }
            public Image Setpicture(Image image, String stringtag = "")
            {
                if (_currentBitmap != null)
                {
                    _currentBitmap.Dispose(); // 釋放舊的 Bitmap
                }

                _currentBitmap = new Bitmap(image); // 複製新的 Bitmap
                tag = stringtag;
                return _currentBitmap; // 回傳圖片
            }
        }
        //mainwindow本體操控-------------------------------------------------------------------------------------------
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
        private void cmbFunclist_SelectedIndexChanged(object sender, EventArgs e)  // 切換功能時，顯示對應的 GroupBox
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
            switch (tabimgeFuncList.Text)
            {
                case "Frame&CrossLine&Border":
                    tabimgeFrame.Visible = true;
                    tabimgeFrame.Dock = DockStyle.Fill;
                    break;
                case "Gradient&ColorBar":
                    tabimgeGradient.Visible = true;
                    tabimgeGradient.Dock = DockStyle.Fill;
                    break;
                case "Chess":
                    tabimgeChess.Visible = true;
                    tabimgeChess.Dock = DockStyle.Fill;
                    break;
                case "Window":
                    tabimgeWindow.Visible = true;
                    tabimgeWindow.Dock = DockStyle.Fill;
                    tabiwWinSizePixelW_nud.Maximum = int.Parse(mnsW_txt.Text);
                    tabiwWinSizePixelH_nud.Maximum = int.Parse(mnsH_txt.Text);
                    tabiwWinLocPixcelX_nud.Maximum = int.Parse(mnsW_txt.Text);
                    tabiwWinLocPixcelY_nud.Maximum = int.Parse(mnsH_txt.Text);
                    break;
                case "Mask(FlickerPattern)":
                    tabimgeMask.Visible = true;
                    tabimgeMask.Dock = DockStyle.Fill;
                    break;
                case "ImageAdjust":
                    tabimgeAdjust.Visible = true;
                    tabimgeAdjust.Dock = DockStyle.Fill;
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
        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == tabDirList)
            {
                // 當選擇 ImgList 標籤時，執行 PopulateListView 方法
                PopulateListView();
            }
            else if (tabControl.SelectedTab == tabImgList)
            {
                // 當選擇 ImgEditor 標籤時，執行其他邏輯
                // 獲取程式所在資料夾的 config.txt 檔案
                string configFilePath = Directory.GetFiles(Application.StartupPath, "config.txt").FirstOrDefault();

                // 如果找到 config.txt，將其內容讀取為字串並傳入 PopulateListBox 函式
                if (!string.IsNullOrEmpty(configFilePath))
                {
                    try
                    {
                        PopulateListBox(File.ReadAllText(configFilePath)); // 將字串傳入 PopulateListBox
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"讀取檔案失敗：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    PopulateListBox(Resources.config);
                }
            }
        }  // 切換tab
        // 產生圖片----------------------------------------------------------------------------------------------------
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
                if (showimgPicture_pic.Image != null)
                {
                    showimgPicture_pic.Image.Dispose();
                    showimgPicture_pic.Image = null;
                }
                showimgPicture_pic.Image = mypicture.Getpicture();
                showimgPicture_pic.SizeMode = PictureBoxSizeMode.Zoom;
                showimgSize_btn.Text = $"當前圖片大小：{mnsW_txt.Text} x {mnsH_txt.Text}"; // 更新狀態列顯示當前圖片大小  
            }
            catch (Exception ex)
            {
                MessageBox.Show($"圖片生成失敗：{ex.Message}");
            }
        }  // 按下生成圖片
        public void GenerateImage(string type, int width, int height, string tag="")
        {
            Bitmap CurrentBitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(CurrentBitmap))
            {
                //g.SmoothingMode = SmoothingMode.None; // 抗鋸齒模式
                //g.CompositingMode = CompositingMode.SourceCopy; // 合成模式
                //g.CompositingQuality = CompositingQuality.HighSpeed; //
                //g.InterpolationMode = InterpolationMode.NearestNeighbor;

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
                            using (Pen borderPen = new Pen(tabiefLineColor_btn.BackColor, (int)tabiefLineSize_nud.Value)) // 外框線顏色和寬度
                            {
                                g.DrawRectangle(borderPen, 0, 0, width - 1, height - 1);
                            }
                        }

                        // 繪製中心對位線（如果選中）
                        if (tabiefCenter_chk.Checked)
                        {
                            using (Pen centerPen = new Pen(tabiefLineColor_btn.BackColor, (int)tabiefLineSize_nud.Value)) // 中心線顏色和寬度
                            {
                                g.DrawLine(centerPen, width / 2, 0, width / 2, height); // 垂直中心線
                                g.DrawLine(centerPen, 0, height / 2, width, height / 2); // 水平中心線
                            }
                        }

                        // 繪製九點對位線（如果選中）
                        if (tabiefNine_chk.Checked)
                        {
                            using (Pen ninePointPen = new Pen(tabiefLineColor_btn.BackColor, (int)tabiefLineSize_nud.Value)) // 九點線顏色和寬度
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
                        Bitmap backBitmap = new Bitmap(width, height);
                        int divisions = int.Parse(tabigDivid_cmb.Text); // 獲取畫面等分數  
                        int divisionSize = tabigHWay_rdo.Checked ? width / divisions : height / divisions; // 計算每個分區的寬度或高度
                        using (Graphics g2 = Graphics.FromImage(backBitmap))
                        {
                            if (tabigOther_chk.Checked) // 如果選擇了colorbar  
                            {
                                int blockHeight = height / divisions;
                                int blockWidth = width / divisions;
                                int split = (int)tabigOtherSplit_nud.Value;
                                for (int i = 0; i < divisions; i++)
                                {
                                    foreach (Control control in tabigOtherSplit_pnl.Controls)
                                    {
                                        if (control is Panel panel && panel.BackColor != Color.Transparent)
                                        {
                                            string[] tagParts = panel.Tag.ToString().Split(',');
                                            if (tagParts.Length == 2)
                                            {
                                                string direction = tagParts[0];
                                                int index = int.Parse(tagParts[1]);
                                                if (direction == "H") // 水平分割  
                                                {
                                                    if (tabigHWay_rdo.Checked)
                                                    {
                                                        g2.FillRectangle(new SolidBrush(panel.BackColor), 0, index * blockHeight / split + i * blockHeight, width, blockHeight / split);
                                                    }
                                                    else
                                                    {
                                                        g2.FillRectangle(new SolidBrush(panel.BackColor), 0, index * height / split, width, height / split);
                                                    }
                                                }
                                                else if (direction == "V") // 垂直分割  
                                                {
                                                    if (tabigVWay_rdo.Checked)
                                                    {
                                                        g2.FillRectangle(new SolidBrush(panel.BackColor), index * blockWidth / split + i * blockWidth, 0, blockWidth / split, height);
                                                    }
                                                    else
                                                    {
                                                        g2.FillRectangle(new SolidBrush(panel.BackColor), index * width / split, 0, width / split, height);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                g2.Clear(tabigBaseColor_lbl.BackColor); // 填滿整個backBitmap  
                            }
                        }

                        if (!tabigColorBar_rdo.Checked)
                        {
                            int stepNum = Math.Min(int.Parse(tabigStep_cmb.Text), divisionSize); // 獲取漸層階數
                            int startColorValue = int.Parse(tabigFirstLevel_cmb.Text); // 開始顏色值  
                            int endColorValue = int.Parse(tabigLastLevel_cmb.Text); // 結束顏色值  
                            backBitmap = ApplyTransparency(backBitmap, startColorValue, endColorValue, tabigHWay_rdo.Checked, stepNum, divisions, tabigReverse_chk.Checked);
                            backBitmap = ConvertARGBToRGB(backBitmap);
                        }
                        CurrentBitmap = backBitmap;
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
                        if (tabiecHFlip_chk.Checked)
                        {
                            // 水平翻轉
                            CurrentBitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }
                        if (tabiecVFlip_chk.Checked)
                        {
                            // 垂直翻轉
                            CurrentBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
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
                        else if (tabiwBackCustom_rdo.Checked) // 圖片背景
                        {
                            g.DrawImage(tabiwBack_pic.Image, 0, 0, width, height);
                        }
                        int percent = (int)tabiwWinSizePercent_nud.Value; // 獲取百分比大小
                                                                          // 百分比大小 // 實際大小
                        int newWidth = tabiwWinSizePercent_rdo.Checked ? int.Parse(mnsW_txt.Text) * percent / 100 : int.Parse(tabiwWinSizePixelW_nud.Text);
                        int newHeight = tabiwWinSizePercent_rdo.Checked ? int.Parse(mnsH_txt.Text) * percent / 100 : int.Parse(tabiwWinSizePixelH_nud.Text);
                        int xstart = 0;
                        int ystart = 0;
                        if (tabiwDiagonal_btn.Checked)
                        {
                            using (Brush brush = new SolidBrush(tabiwWin_pic.BackColor))
                            {

                                // 定義三角形的三個頂點  
                                Point[] trianglePoints =
                                {
                                   new Point(width, 0), // 右上角  
                                   new Point(width, height), // 右下角  
                                   new Point(0, 0) // 左上角  
                               };

                                // 繪製右上角的三角形  
                                g.FillPolygon(brush, trianglePoints);
                            }
                        }
                        else
                        {
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
                                else if (tabiwWinCustom_rdo.Checked)
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
                            else if (tabiwWinCustom_rdo.Checked)
                            {
                                g.DrawImage(tabiwWin_pic.Image, xstart, ystart, newWidth, newHeight);
                            }
                        }
                        


                        // 繪製外框線（如果選中）
                        if (tabiwLineOutside_btn.Checked)
                        {
                            using (Pen borderPen = new Pen(tabiwLineColor_btn.BackColor, 1)) // 外框線顏色和寬度
                            {
                                g.DrawRectangle(borderPen, 0, 0, width - 1, height - 1);
                            }
                        }

                        // 繪製中心對位線（如果選中）
                        if (tabiwLineCross_btn.Checked)
                        {
                            using (Pen centerPen = new Pen(tabiwLineColor_btn.BackColor, 1)) // 中心線顏色和寬度
                            {
                                g.DrawLine(centerPen, width / 2, 0, width / 2, height); // 垂直中心線
                                g.DrawLine(centerPen, 0, height / 2, width, height / 2); // 水平中心線
                            }
                        }
                        mypicture.Setpicture(CurrentBitmap);
                        break;
                    case "mask":
                        if (true) // 如果True 就用Lockbit 如果False就用Setpixel
                        {
                            if (tabiemPixel_rdo.Checked)
                            {
                                int pixelW = (int)tabiemWNum_nud.Value;
                                int pixelH = (int)tabiemHNum_nud.Value;

                                using (Bitmap pixelimage = new Bitmap(pixelW, pixelH))
                                {
                                    // 鎖定位圖的像素數據
                                    BitmapData pixelData = pixelimage.LockBits(
                                        new Rectangle(0, 0, pixelimage.Width, pixelimage.Height),
                                        ImageLockMode.WriteOnly,
                                        PixelFormat.Format32bppArgb);

                                    int stride = pixelData.Stride;
                                    IntPtr ptr = pixelData.Scan0;
                                    byte[] pixelBuffer = new byte[stride * pixelH];

                                    // 初始化像素數據
                                    foreach (Control control in tabiemPixelPanel_pnl.Controls)
                                    {
                                        if (control is Panel panel)
                                        {
                                            // 解析 Panel 的座標 (Tag 格式為 "x,y")
                                            string[] coordinates = panel.Tag.ToString().Split(',');
                                            int x = int.Parse(coordinates[0]);
                                            int y = int.Parse(coordinates[1]);

                                            // 計算像素在數組中的索引
                                            int index = (y * stride) + (x * 4);

                                            // 設置像素顏色
                                            Color color = panel.BackColor;
                                            pixelBuffer[index] = color.B;       // 藍色
                                            pixelBuffer[index + 1] = color.G;  // 綠色
                                            pixelBuffer[index + 2] = color.R;  // 紅色
                                            pixelBuffer[index + 3] = color.A;  // 透明度
                                        }
                                    }

                                    // 將像素數據寫回位圖
                                    System.Runtime.InteropServices.Marshal.Copy(pixelBuffer, 0, ptr, pixelBuffer.Length);
                                    pixelimage.UnlockBits(pixelData);

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
                                int pixelW = (int)tabiemWNum_nud.Value;
                                int pixelH = (int)tabiemHNum_nud.Value;

                                using (Bitmap pixelimage = new Bitmap(pixelW, pixelH))
                                {
                                    // 鎖定位圖的像素數據
                                    BitmapData pixelData = pixelimage.LockBits(
                                        new Rectangle(0, 0, pixelimage.Width, pixelimage.Height),
                                        ImageLockMode.WriteOnly,
                                        PixelFormat.Format32bppArgb);

                                    int stride = pixelData.Stride;
                                    IntPtr ptr = pixelData.Scan0;
                                    byte[] pixelBuffer = new byte[stride * pixelH];

                                    // 初始化像素數據
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

                                                // 計算像素在數組中的索引
                                                int index = (y * stride) + (x * 4);

                                                // 設置像素顏色
                                                pixelBuffer[index] = combinedColor.B;       // 藍色
                                                pixelBuffer[index + 1] = combinedColor.G;  // 綠色
                                                pixelBuffer[index + 2] = combinedColor.R;  // 紅色
                                                pixelBuffer[index + 3] = combinedColor.A;  // 透明度
                                            }
                                        }
                                    }

                                    // 將像素數據寫回位圖
                                    System.Runtime.InteropServices.Marshal.Copy(pixelBuffer, 0, ptr, pixelBuffer.Length);
                                    pixelimage.UnlockBits(pixelData);

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
                        }
                        else
                        {
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
                // 設置圖片的標籤
                mypicture.tag = tag != "" ? tag : mypicture.tag;
            }
            // 釋放資源
            CurrentBitmap.Dispose();
        }  // 生成圖片
        // 圖片控制----------------------------------------------------------------------------------------------------
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
                else if (sender is PictureBox pictureBox)
                {
                    if (showimgPicture_pic.Image != null)
                    {
                        showimgPicture_pic.Image.Dispose();
                        showimgPicture_pic.Image = null;
                    }
                    // 將載入的圖片設置到 PictureBox
                    showimgPicture_pic.Image = mypicture.OpenImage();
                    showimgPicture_pic.SizeMode = PictureBoxSizeMode.Zoom; // 設置圖片縮放模式
                    pictureBox.Image = showimgPicture_pic.Image;
                    pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                    showimgSize_btn.Text = $"圖片大小：{mypicture.Getsize()}"; // 更新狀態列顯示圖片大小
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"無法載入圖片：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }  // 圖片導入
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
        }  // 儲存圖片
        // 圖片顯示----------------------------------------------------------------------------------------------------
        public void FullScreen(object sender, EventArgs e)
        {
            // 獲取選擇的螢幕  
            string selectedScreen = mnsScreenlist_cmb.SelectedItem?.ToString() ?? Screen.AllScreens[0].DeviceName;
            Screen secondScreen = Screen.AllScreens.FirstOrDefault(screen => screen.DeviceName == selectedScreen) ?? Screen.AllScreens[0];

            if (_pictureWindow == null || _pictureWindow.IsDisposed)
            {
                // 如果 _pictureWindow 尚未初始化或已被釋放，則創建新的實例  
                _pictureWindow = new pictureWindow((Bitmap)showimgPicture_pic.Image);
            }

            _pictureWindow.setBitmap((Bitmap)showimgPicture_pic.Image);
            if (_pictureWindow != null && !_pictureWindow.IsDisposed)
            {
                _pictureWindow.WindowState = FormWindowState.Maximized;
                _pictureWindow.FormBorderStyle = FormBorderStyle.None;
                if (!_pictureWindow.Visible)
                {
                    // 設置窗口位置和大小  
                    _pictureWindow.WindowState = FormWindowState.Maximized;
                    _pictureWindow.StartPosition = FormStartPosition.Manual;
                    _pictureWindow.Location = secondScreen.Bounds.Location;
                    _pictureWindow.Size = secondScreen.Bounds.Size;

                    // 顯示窗口  
                    _pictureWindow.Show();
                    _pictureWindow.BringToFront();
                    _pictureWindow.BringToFront();
                    //MessageBox.Show("Form 未顯示！");
                }
            }
            else
            {
                MessageBox.Show("Form 尚未初始化或已被釋放！");
            }
        }  // 全螢幕顯示
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
        }  // 螢幕選擇
        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabdlPatternList_lvw.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = tabdlPatternList_lvw.SelectedItems[0]; // 獲取選中的 ListViewItem  
                string imagePath = selectedItem.Tag as string; // 從 Tag 中取出圖片路徑  

                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    try
                    {
                        // 釋放舊圖片
                        if (showimgPicture_pic.Image != null)
                        {
                            showimgPicture_pic.Image.Dispose();
                            showimgPicture_pic.Image = null;
                        }

                        // 使用 FileStream 加載圖片，並生成縮圖
                        using (FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                        {
                            using (Image originalImage = Image.FromStream(fs))
                            {
                                ////// 生成縮圖
                                //int thumbnailWidth = 800; // 設定縮圖寬度
                                //int thumbnailHeight = 600; // 設定縮圖高度
                                //Image thumbnail = originalImage.GetThumbnailImage(thumbnailWidth, thumbnailHeight, null, IntPtr.Zero);

                                //// 設定縮圖到 PictureBox
                                //showimgPicture_pic.Image = thumbnail;
                                showimgPicture_pic.Image = mypicture.Setpicture(originalImage, "image");
                                showimgPicture_pic.SizeMode = PictureBoxSizeMode.Zoom;

                                // 更新圖片大小顯示
                                showimgSize_btn.Text = $"當前圖片大小：{originalImage.Width} x {originalImage.Height}";
                            }
                        }

                        // 如果需要全屏顯示，更新 _pictureWindow
                        if (_pictureWindow != null && !_pictureWindow.IsDisposed)
                        {
                            FullScreen(sender, e);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"無法載入圖片：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show($"找不到圖片：{imagePath}");
                }
            }
        }  // listview圖片選擇
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
                        // 生成縮圖
                        Image thumbnail = img.GetThumbnailImage(56, 56, null, IntPtr.Zero);
                        // 添加到 ImageList
                        tabdlPatternList_lvw.LargeImageList.Images.Add(fileName, thumbnail);

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
        }  // listview圖片導入列表
        private void tabilPatternList_lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            string trimlower(string st)
            {
                return st.Trim().ToLower();
            }
            if (sender is ListBox targetList)
            {
                if (targetList.SelectedItem != null)
                {
                    // 這裡可以根據選中的項目來顯示圖片  
                    string[] patternVar = targetList.SelectedItem.ToString().Split(',');
                    if (patternVar.Length >= 3)
                    {
                        string type = trimlower(patternVar[1]);
                        //char[] TrimChars = { '\t', '\n', ' ' };
                        switch (type)
                        {
                            case "solid":
                                tabimgeFuncList.SelectedIndex = 0;
                                funcFrame(trimlower(patternVar[2]), trimlower(patternVar[3]));
                                break;
                            case "gradient":
                                tabimgeFuncList.SelectedIndex = 1;
                                funcGrad(trimlower(patternVar[2]), trimlower(patternVar[3]));
                                break;
                            case "chess":
                                tabimgeFuncList.SelectedIndex = 2;
                                funcChess(trimlower(patternVar[2]), trimlower(patternVar[3]));
                                break;
                            case "window":
                                tabimgeFuncList.SelectedIndex = 3;
                                funcWind(trimlower(patternVar[2]), trimlower(patternVar[3]));
                                break;
                            case "mask":
                                tabimgeFuncList.SelectedIndex = 4;
                                funcMask(trimlower(patternVar[2]), trimlower(patternVar[3]));
                                break;
                            default:
                                MessageBox.Show("不支援的圖片類型！");
                                break;
                        }
                        string tag = patternVar[0];
                        GenerateImage(type, int.Parse(mnsW_txt.Text), int.Parse(mnsH_txt.Text), tag); // 重新生成圖片
                    }
                    showimgPicture_pic.Image = mypicture.Getpicture();
                    showimgPicture_pic.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
        }  // ListBox選擇圖片
        private void PopulateListBox(string configfile)
        {
            //byte[] config = Resources.config;
            tabilPatternList_lst.Items.Clear();
            foreach (string line in configfile.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                tabilPatternList_lst.Items.Add(line);
            }
        }  // ListBox載入圖片
        // 圖片處理 groupbox 功能--------------------------------------------------------------------------------------
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
                        targetButton.Image = null; // 清除按鈕上的圖片
                        if (targetButton.Name.Contains("tabiefBack_btn"))
                        {
                            mypicture.tag = "color";
                        }
                    }
                }
            }
        }  // 設定按鈕顏色
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
        }  //Chess按鈕顏色
        private void GradientColor_Click(object sender, EventArgs e)
        {
            if (sender is Button targetbutton)
            {
                Color color = targetbutton.BackColor;
                if (targetbutton.Name.Contains("tabigBaseColor"))
                {

                    if (targetbutton.Text == "")
                    {
                        GetButtonBackgroundColor(sender, e);
                    }
                    tabigBaseColor_lbl.BackColor = color;
                    tabigBaseColor_lbl.ForeColor = (color.R + color.G + color.B) / 3 < 127 ? Color.White : Color.Black;
                }
                else if (targetbutton.Name.Contains("tabigOtherColor"))
                {
                    if (targetbutton.Text == "")
                    {
                        GetButtonBackgroundColor(sender, e);
                    }
                    tabigOtherColor_lbl.BackColor = color;
                    tabigOtherColor_lbl.ForeColor = (color.R + color.G + color.B) / 3 < 127 ? Color.White : Color.Black;
                }
                else if (targetbutton.Name.Contains("tabiecBaseColor"))
                {
                    if (targetbutton.Text == "")
                    {
                        GetButtonBackgroundColor(sender, e);
                    }
                    tabiecGray_lbl.BackColor = color;
                    tabiecGray_lbl.ForeColor = (color.R + color.G + color.B) / 3 < 127 ? Color.White : Color.Black;
                }
                else if (targetbutton.Name.Contains("tabiwCGradBaseColor"))
                {
                    if (targetbutton.Text == "")
                    {
                        GetButtonBackgroundColor(sender, e);
                    }
                    tabiwCGradBaseColor_lbl.BackColor = color;
                    tabiwCGradBaseColor_lbl.ForeColor = (color.R + color.G + color.B) / 3 < 127 ? Color.White : Color.Black;
                }
                else if (targetbutton.Name.Contains("tabiwCGradOtherColor"))
                {
                    if (targetbutton.Text == "")
                    {
                        GetButtonBackgroundColor(sender, e);
                    }
                    tabiwCGradOtherColor_lbl.BackColor = color;
                    tabiwCGradOtherColor_lbl.ForeColor = (color.R + color.G + color.B) / 3 < 127 ? Color.White : Color.Black;
                }
            }
        }  // 按鈕設定Label設定顏色
        private void gbxfHsblocation_ValueChanged(object sender, EventArgs e)
        {
            tabiefOtherLoc_lbl.Text = tabiefOtherLoc_hsc.Value.ToString();  // frame 的卷軸值變更
        }  // Frame 其他線的卷軸值變更
        private void hScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            tabiecGray_lbl.Text = $"Gary: {tabiecGray_hsc.Value.ToString()}"; // 更新 Label 顯示當前值
        }  // Chess卷軸值變更

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
        }  // Frame 其他線的顯示
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
        }  // Frame 其他線listbox的增加與清除
        private void tabiefOther_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabiefOther_cmb.Text == "HLine")
            {
                tabiefOtherLoc_hsc.Maximum = (int.Parse(mnsH_txt.Text)-1); // 將 msTxtH.Text 轉換為 int  
            }
            else if (tabiefOther_cmb.Text == "VLine")
            {
                tabiefOtherLoc_hsc.Maximum = (int.Parse(mnsW_txt.Text)-1); // 將 msTxtW.Text 轉換為 int  
            }
        }  // Frame 其他線的選擇HLine/VLine
        private Bitmap ApplyTransparency(Bitmap originalBitmap, int startAlpha, int endAlpha, bool isH, int steps, int segments, bool reverse)
        {
            // 複製原始圖片，避免直接修改
            //Bitmap resultBitmap = new Bitmap(originalBitmap);
            Bitmap resultBitmap = originalBitmap;

            // 鎖定圖片的像素資料
            Rectangle rect = new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height);
            BitmapData bitmapData = resultBitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            // 取得像素資料的指標
            IntPtr ptr = bitmapData.Scan0;
            int bytes = Math.Abs(bitmapData.Stride) * resultBitmap.Height;
            byte[] pixelValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, pixelValues, 0, bytes);


            if (reverse)
            {
                int dimension = isH ? resultBitmap.Width : resultBitmap.Height;
                int segmentLength = isH ? resultBitmap.Height : resultBitmap.Width;
            }
            else
            {
                // 計算每個分區的長度
                int dimension = isH ? resultBitmap.Height : resultBitmap.Width;
                int segmentLength = dimension / segments;
                // 處理分階數大於分區長度的情況
                steps = Math.Min(steps, segmentLength);

                // 計算每個階段的透明度變化量
                int alphaRange = endAlpha - startAlpha;
                float alphaStep = (float)alphaRange / (steps - 1);

                // 遍歷每個像素
                for (int y = 0; y < resultBitmap.Height; y++)
                {
                    for (int x = 0; x < resultBitmap.Width; x++)
                    {
                        int index = (y * bitmapData.Stride) + (x * 4); // 每個像素佔 4 個位元組 (BGRA)

                        // 計算當前像素所在的分區
                        int position = isH ? y : x;
                        //int segmentIndex = position / segmentLength;

                        // 計算當前像素在分區內的位置
                        int positionInSegment = position % segmentLength;

                        // 計算透明度
                        int stepIndex = (int)((float)positionInSegment / segmentLength * (steps));
                        //// 如果分區是奇數，反向透明度
                        //if (reverse && segmentIndex % 2 == 1)
                        //{
                        //    stepIndex = steps - 1 - stepIndex;
                        //}
                        byte alpha = (byte)(startAlpha + alphaStep * stepIndex);


                        // 修改 Alpha 值
                        pixelValues[index + 3] = alpha; // Alpha 通道
                    }
                }
            }
            

            // 將修改後的像素資料寫回圖片
            System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, ptr, bytes);
            resultBitmap.UnlockBits(bitmapData);

            return resultBitmap;
        }  // Grad 透明度處理
        private Bitmap ConvertARGBToRGB(Bitmap originalBitmap)
        {
            // 建立與原始圖片相同大小的 RGB Bitmap
            Bitmap rgbBitmap = new Bitmap(originalBitmap.Width, originalBitmap.Height, PixelFormat.Format24bppRgb);

            // 鎖定原始圖片的像素資料
            Rectangle rect = new Rectangle(0, 0, originalBitmap.Width, originalBitmap.Height);
            BitmapData originalData = originalBitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            // 鎖定新圖片的像素資料
            BitmapData rgbData = rgbBitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            // 取得像素資料的指標
            IntPtr originalPtr = originalData.Scan0;
            IntPtr rgbPtr = rgbData.Scan0;

            // 計算像素資料的大小
            int originalBytes = Math.Abs(originalData.Stride) * originalBitmap.Height;
            int rgbBytes = Math.Abs(rgbData.Stride) * rgbBitmap.Height;

            // 建立緩衝區
            byte[] originalPixels = new byte[originalBytes];
            byte[] rgbPixels = new byte[rgbBytes];

            // 複製像素資料到緩衝區
            System.Runtime.InteropServices.Marshal.Copy(originalPtr, originalPixels, 0, originalBytes);

            // 遍歷每個像素
            for (int y = 0; y < originalBitmap.Height; y++)
            {
                for (int x = 0; x < originalBitmap.Width; x++)
                {
                    // 計算像素索引
                    int originalIndex = y * originalData.Stride + x * 4; // ARGB 每像素 4 bytes
                    int rgbIndex = y * rgbData.Stride + x * 3; // RGB 每像素 3 bytes

                    // 取得原始像素的 ARGB 值
                    byte b = originalPixels[originalIndex];
                    byte g = originalPixels[originalIndex + 1];
                    byte r = originalPixels[originalIndex + 2];
                    byte a = originalPixels[originalIndex + 3];

                    // 計算與黑色背景混合後的 RGB 值
                    byte finalR = (byte)((r * a) / 255);
                    byte finalG = (byte)((g * a) / 255);
                    byte finalB = (byte)((b * a) / 255);

                    // 寫入 RGB 像素資料
                    rgbPixels[rgbIndex] = finalB;
                    rgbPixels[rgbIndex + 1] = finalG;
                    rgbPixels[rgbIndex + 2] = finalR;
                }
            }

            // 將修改後的像素資料寫回新圖片
            System.Runtime.InteropServices.Marshal.Copy(rgbPixels, 0, rgbPtr, rgbBytes);

            // 解鎖像素資料
            originalBitmap.UnlockBits(originalData);
            rgbBitmap.UnlockBits(rgbData);

            return rgbBitmap;
        }  // Grad ARGB 轉 RGB
        private void SplitPanelCreat(object sender, EventArgs e)
        {
            if (sender is Control control && control != null)
            {
                Panel targetPanel;
                int splitnum;
                int blockWidth;
                int blockHeight;
                string splitType;
                if (control.Parent != null && control.Parent.Name == "tabigOther_grp")
                {
                    // 取得並清空面板
                    targetPanel = tabigOtherSplit_pnl;
                    targetPanel.Controls.Clear();
                    // 取得寬度與高度 
                    splitnum = (int)tabigOtherSplit_nud.Value;
                    if (tabigOtherVSplit_rdo.Checked)
                    {
                        blockWidth = targetPanel.Width / splitnum;
                        blockHeight = targetPanel.Height;
                    }
                    else
                    {
                        blockWidth = targetPanel.Width;
                        blockHeight = targetPanel.Height / splitnum;
                    }
                }
                else
                {
                    // 取得並清空面板
                    targetPanel = tabiwCGradOtherSplit_pnl;
                    targetPanel.Controls.Clear();
                    // 取得寬度與高度
                    splitnum = (int)tabiwCGradOtherSplit_nud.Value;
                    splitType = tabiwCGradOtherVSplit_rdo.Checked ? "Vsplit" : "Hplit";
                    if (tabiwCGradOtherVSplit_rdo.Checked)
                    {
                        blockWidth = targetPanel.Width / splitnum;
                        blockHeight = targetPanel.Height;
                    }
                    else
                    {
                        blockWidth = targetPanel.Width;
                        blockHeight = targetPanel.Height / splitnum;
                    }
                }

                RadioButton targetRdo = targetPanel.Parent.Controls.OfType<RadioButton>()
                                    .FirstOrDefault(rdo => rdo.Name.Contains("HSplit_rdo")); // 替換為目標 Label 的名稱

                // 生成分割區塊
                for (int i = 0; i < splitnum; i++)
                {
                    Panel block = new Panel
                    {
                        Size = new Size(blockWidth, blockHeight),
                        Location = targetRdo.Checked
                            ? new Point(0, i * blockHeight)
                            : new Point(i * blockWidth, 0),
                        BorderStyle = BorderStyle.FixedSingle,
                        Tag = $"{(targetRdo.Checked ? "H" : "V")},{i}" // 儲存區塊的方向與索引
                    };

                    // 設定點擊事件以填充顏色
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
                                    Label targetLabel = grandbox.Controls.OfType<Label>()
                                    .FirstOrDefault(label => label.Name.Contains("Color_lbl")); // 替換為目標 Label 的名稱
                                    block.BackColor = targetLabel.BackColor;
                                }
                            }
                        }
                    };
                    block.MouseUp += (s, args) =>
                    {
                        if (args.Button == MouseButtons.Left && s is Panel panel)
                        {
                            _isMouseDown = false;
                        }
                    };

                    // 設定滑鼠移入事件以顯示座標
                    block.MouseEnter += (s, args) =>
                    {
                        if (block.Parent is Panel parentpanel)
                        {
                            if (parentpanel.Parent is GroupBox grandbox)
                            {
                                Label targetLabel = grandbox.Controls.OfType<Label>()
                                .FirstOrDefault(label => label.Name.Contains("Loc_lbl")); // 替換為目標 Label 的名稱
                                targetLabel.Text = $"{block.Tag}"; // 更新 Label 顯示區塊座標
                            }
                        }
                    };

                    targetPanel.Controls.Add(block);
                }
            }
        }  // Grad Colorbar生成區塊
        private void otherColor_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is Control target)
            {
                if (target.Name.Contains("tabig"))
                {
                    if (tabigColorBar_rdo.Checked)
                    {
                        tabigOther_chk.Checked = true;
                    }
                    if (tabigOther_chk.Checked)
                    {
                        tabigBaseColor_pnl.Enabled = false;
                        tabigOther_grp.Enabled = true; // 顯示顏色選擇面板
                    }
                    else
                    {
                        tabigBaseColor_pnl.Enabled = true;
                        tabigOther_grp.Enabled = false; // 隱藏顏色選擇面板
                    }
                }
                else if (target.Name.Contains("tabiwCGrad"))//
                {
                    //MessageBox.Show("123");
                    if (tabiwCGradColorBar_rdo.Checked)
                    {
                        tabiwCGradOther_chk.Checked = true;
                    }
                    if (tabiwCGradOther_chk.Checked)
                    {
                        tabiwCGradBaseColor_pnl.Enabled = false;
                        tabiwCGradOther_grp.Enabled = true; // 顯示顏色選擇面板
                    }
                    else
                    {
                        tabiwCGradBaseColor_pnl.Enabled = true;
                        tabiwCGradOther_grp.Enabled = false; // 隱藏顏色選擇面板
                    }
                }
            }
        }  // Grad Colorbar顏色選擇
        private void ColorBar_btn_Click(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                if (button.Name.Contains("tabig"))
                {
                    tabigOtherSplit_nud.Value = button.Name.Contains("FourColor_btn") ? 4 : 8;
                }
                else if (button.Name.Contains("tabiwCGrad"))
                {
                    tabiwCGradOtherSplit_nud.Value = button.Name.Contains("FourColor_btn") ? 4 : 8;
                }
                if (button.Parent is GroupBox parent)
                {
                    Color[] colors = { Color.White, Color.Red, Color.Lime, Color.Blue, Color.Black, Color.Aqua, Color.Yellow, Color.Magenta };
                    int colorIndex = 0;
                    Panel targetpanel = parent.Controls.OfType<Panel>()
                    .FirstOrDefault(panel => panel.Name.Contains("OtherSplit_pnl")); // 替換為目標 Panel 的名稱
                    foreach (Panel panel in targetpanel.Controls)
                    {
                        panel.BackColor = colors[colorIndex % colors.Length];
                        colorIndex++;
                    }
                }
            }

        }  // Grad ColorBar 標準四色/八色
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
        }  // Adjust 調整圖片
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
                    else if (tabiwBackCustom_rdo.Checked)
                    {
                        Custom_btn_Click(sender, e); // 觸發 Custom 按鈕的事件
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
                    else if (tabiwBackCustom_rdo.Checked)
                    {
                        Custom_btn_Click(sender, e); // 觸發 Custom 按鈕的事件
                    }
                }
            }
            else if (sender is RadioButton targetRadio)
            {
                if (targetRadio.Name.Contains("Color_rdo"))
                {
                    using (ColorDialog colorDialog = new ColorDialog())
                    {
                        colorDialog.FullOpen = true; // 顯示進階選項  
                                                     // 顯示顏色選擇對話框  
                        if (colorDialog.ShowDialog() == DialogResult.OK)
                        {
                            if (targetRadio.Name.Contains("Back"))
                            {
                                tabiwBack_pic.BackColor = colorDialog.Color;
                                tabiwBack_pic.Image = null; // 清除按鈕上的圖片
                            }
                            else
                            {
                                tabiwWin_pic.BackColor = colorDialog.Color;
                                tabiwWin_pic.Image = null; // 清除按鈕上的圖片
                            }
                        }
                    }
                    tabiwCustom_grp.Enabled = false;
                }
                else if (targetRadio.Name.Contains("Img_rdo"))
                {
                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            if (targetRadio.Name.Contains("Back"))
                            {
                                tabiwBack_pic.Image = Image.FromFile(openFileDialog.FileName);
                                tabiwBack_pic.BackColor = Color.Transparent;
                            }
                            else
                            {
                                tabiwWin_pic.Image = Image.FromFile(openFileDialog.FileName);
                                tabiwWin_pic.BackColor = Color.Transparent;
                            }
                        }
                    }
                    tabiwCustom_grp.Enabled = false;
                }
                else if (targetRadio.Name.Contains("Custom_rdo"))
                {
                    tabiwCustom_grp.Enabled = true;
                    tabiwCustom_btn.Text = targetRadio.Name.Contains("Back") ? "背景生成" : "窗口生成";
                }
            }
        }  // Window 純色、圖片、自製選擇
        private void tabieaPatternList_chL_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (sender is CheckedListBox targetcheck && targetcheck.Text != "")
            {
                if (showimgPicture_pic.Image != null)
                {
                    //showimgPicture_pic.Image.Dispose();
                    showimgPicture_pic.Image = null;
                }
                showimgPicture_pic.Image = Image.FromFile(targetcheck.Text);
                showimgPicture_pic.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }  // Adjust 圖片列表選擇
        private void tabieaClear_btn_Click(object sender, EventArgs e)
        {
            tabieaPatternList_chl.Items.Clear();
        }  // Adjust 清除圖片列表
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
        }  // Adjust 調整功能選擇
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
        }  // Adjust 圖片選擇
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
                                        .FirstOrDefault(label => label.Name.Contains("Loc_lbl")); // 替換為目標 Label 的名稱
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
        }  // Mask 生成區塊
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
        }  // Mask Pixel/SubPixel選擇 切換控件
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
                        if (control is Panel panel && panel.BackColor != Color.Black)
                        {
                            int x = int.Parse(panel.Tag.ToString().Split(',')[0]);
                            panel.BackColor = x % 3 == 0 ? Color.FromArgb(255 - targetvsc.Value, 0, 0) : x % 3 == 1 ? Color.FromArgb(0, 255 - targetvsc.Value, 0) : Color.FromArgb(0, 0, 255 - targetvsc.Value);
                        }
                    }
                }
            }
        }  // Mask 亮度調整
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
        }  // Window Mask/Grad 選擇
        private void Custom_btn_Click(object sender, EventArgs e)
        {
            int width = 0;
            int height = 0;
            if (!int.TryParse(mnsW_txt.Text, out width) || !int.TryParse(mnsH_txt.Text, out height))
            {
                MessageBox.Show("請輸入有效的寬度與高度！");
                return;
            }
            if (width != 0 || height != 0)
            {
                if (tabiwCustom_btn.Text == "窗口生成")
                {
                    if (tabiwWinSizePercent_rdo.Checked)
                    {
                        width = width * (int)tabiwWinSizePercent_nud.Value / 100;
                        height = height * (int)tabiwWinSizePercent_nud.Value / 100;
                    }
                    else
                    {
                        width = (int)tabiwWinSizePixelW_nud.Value;
                        height = (int)tabiwWinSizePixelH_nud.Value;
                    }
                }
            }
            Bitmap bitmap = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                if (tabiwCustom_cmb.Text == "Mask")
                {
                    if (true)
                    {
                        if (tabiwCMaskPixel_rdo.Checked)
                        {
                            int pixelW = (int)tabiwCMaskWNum_nud.Value;
                            int pixelH = (int)tabiwCMaskHNum_nud.Value;

                            using (Bitmap pixelimage = new Bitmap(pixelW, pixelH))
                            {
                                // 鎖定位圖的像素數據
                                BitmapData pixelData = pixelimage.LockBits(
                                    new Rectangle(0, 0, pixelimage.Width, pixelimage.Height),
                                    ImageLockMode.WriteOnly,
                                    PixelFormat.Format32bppArgb);

                                int stride = pixelData.Stride;
                                IntPtr ptr = pixelData.Scan0;
                                byte[] pixelBuffer = new byte[stride * pixelH];

                                // 初始化像素數據
                                foreach (Control control in tabiwCMaskPixelPanel_pnl.Controls)
                                {
                                    if (control is Panel panel)
                                    {
                                        // 解析 Panel 的座標 (Tag 格式為 "x,y")
                                        string[] coordinates = panel.Tag.ToString().Split(',');
                                        int x = int.Parse(coordinates[0]);
                                        int y = int.Parse(coordinates[1]);

                                        // 計算像素在數組中的索引
                                        int index = (y * stride) + (x * 4);

                                        // 設置像素顏色
                                        Color color = panel.BackColor;
                                        pixelBuffer[index] = color.B;       // 藍色
                                        pixelBuffer[index + 1] = color.G;  // 綠色
                                        pixelBuffer[index + 2] = color.R;  // 紅色
                                        pixelBuffer[index + 3] = color.A;  // 透明度
                                    }
                                }

                                // 將像素數據寫回位圖
                                System.Runtime.InteropServices.Marshal.Copy(pixelBuffer, 0, ptr, pixelBuffer.Length);
                                pixelimage.UnlockBits(pixelData);

                                // 將 pixelimage 填滿 bitmap
                                for (int j = 0; j < bitmap.Height; j += pixelH)
                                {
                                    for (int i = 0; i < bitmap.Width; i += pixelW)
                                    {
                                        g.DrawImage(pixelimage, i, j);
                                    }
                                }
                            }
                        }
                        else if (tabiwCMaskSubPixel_rdo.Checked)
                        {
                            int pixelW = (int)tabiwCMaskWNum_nud.Value;
                            int pixelH = (int)tabiwCMaskHNum_nud.Value;

                            using (Bitmap pixelimage = new Bitmap(pixelW, pixelH))
                            {
                                // 鎖定位圖的像素數據
                                BitmapData pixelData = pixelimage.LockBits(
                                    new Rectangle(0, 0, pixelimage.Width, pixelimage.Height),
                                    ImageLockMode.WriteOnly,
                                    PixelFormat.Format32bppArgb);

                                int stride = pixelData.Stride;
                                IntPtr ptr = pixelData.Scan0;
                                byte[] pixelBuffer = new byte[stride * pixelH];

                                // 初始化像素數據
                                for (int i = 0; i < tabiwCMaskPixelPanel_pnl.Controls.Count; i += 3)
                                {
                                    if (i + 2 < tabiwCMaskPixelPanel_pnl.Controls.Count)
                                    {
                                        // 獲取三個 Panel
                                        Panel panel1 = tabiwCMaskPixelPanel_pnl.Controls[i] as Panel;
                                        Panel panel2 = tabiwCMaskPixelPanel_pnl.Controls[i + 1] as Panel;
                                        Panel panel3 = tabiwCMaskPixelPanel_pnl.Controls[i + 2] as Panel;

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

                                            // 計算像素在數組中的索引
                                            int index = (y * stride) + (x * 4);

                                            // 設置像素顏色
                                            pixelBuffer[index] = combinedColor.B;       // 藍色
                                            pixelBuffer[index + 1] = combinedColor.G;  // 綠色
                                            pixelBuffer[index + 2] = combinedColor.R;  // 紅色
                                            pixelBuffer[index + 3] = combinedColor.A;  // 透明度
                                        }
                                    }
                                }

                                // 將像素數據寫回位圖
                                System.Runtime.InteropServices.Marshal.Copy(pixelBuffer, 0, ptr, pixelBuffer.Length);
                                pixelimage.UnlockBits(pixelData);

                                // 將 pixelimage 填滿 bitmap
                                for (int j = 0; j < bitmap.Height; j += pixelH)
                                {
                                    for (int i = 0; i < bitmap.Width; i += pixelW)
                                    {
                                        g.DrawImage(pixelimage, i, j);
                                    }
                                }
                            }
                        }

                    }
                    else 
                    { 
                    int pixelW = (int)tabiwCMaskWNum_nud.Value;
                    int pixelH = (int)tabiwCMaskHNum_nud.Value;
                    Panel targetPanel = tabiwCMaskPixelPanel_pnl;
                        // 使用遮罩圖片
                        using (Bitmap pixelimage = new Bitmap(pixelW, pixelH))
                        {
                            if (tabiwCMaskPixel_rdo.Checked)
                            {
                                // 遍歷 tabiemPixelPanel_pnl 的子 Panel  
                                foreach (Control control in targetPanel.Controls)
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
                            }
                            else if (tabiwCMaskSubPixel_rdo.Checked)
                            {
                                // 遍歷 tabiemPixelPanel_pnl 的子 Panel，每三個一組組合成 Color.FromArgb() 的三個參數  
                                for (int i = 0; i < targetPanel.Controls.Count; i += 3)
                                {
                                    if (i + 2 < targetPanel.Controls.Count)
                                    {
                                        // 獲取三個 Panel  
                                        Panel panel1 = targetPanel.Controls[i] as Panel;
                                        Panel panel2 = targetPanel.Controls[i + 1] as Panel;
                                        Panel panel3 = targetPanel.Controls[i + 2] as Panel;

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
                            }
                            // 將 pixelimage 填滿 currentBitmap  
                            for (int j = 0; j < bitmap.Height; j += pixelH)
                            {
                                for (int i = 0; i < bitmap.Width; i += pixelW)
                                {
                                    g.DrawImage(pixelimage, i, j);
                                }
                            }
                        }
                    }
                }
                else if (tabiwCustom_cmb.Text == "Gradient")
                {
                    int divisions = int.Parse(tabiwCGradDivid_cmb.Text); // 獲取畫面等分數  
                    int divisionSize = tabiwCGradHWay_rdo.Checked ? width / divisions : height / divisions; // 計算每個分區的寬度或高度
                    if (tabiwCGradOther_chk.Checked) // 如果選擇了colorbar  
                    {
                        int blockHeight = height / divisions;
                        int blockWidth = width / divisions;
                        int split = (int)tabiwCGradOtherSplit_nud.Value;
                        for (int i = 0; i < divisions; i++)
                        {
                            foreach (Control control in tabiwCGradOtherSplit_pnl.Controls)
                            {
                                if (control is Panel panel && panel.BackColor != Color.Transparent)
                                {
                                    string[] tagParts = panel.Tag.ToString().Split(',');
                                    if (tagParts.Length == 2)
                                    {
                                        string direction = tagParts[0];
                                        int index = int.Parse(tagParts[1]);
                                        if (direction == "H") // 水平分割  
                                        {
                                            if (tabiwCGradHWay_rdo.Checked)
                                            {
                                                g.FillRectangle(new SolidBrush(panel.BackColor), 0, index * blockHeight / split + i * blockHeight, width, blockHeight / split);
                                            }
                                            else
                                            {
                                                g.FillRectangle(new SolidBrush(panel.BackColor), 0, index * height / split, width, height / split);
                                            }
                                        }
                                        else if (direction == "V") // 垂直分割  
                                        {
                                            if (tabiwCGradVWay_rdo.Checked)
                                            {
                                                g.FillRectangle(new SolidBrush(panel.BackColor), index * blockWidth / split + i * blockWidth, 0, blockWidth / split, height);
                                            }
                                            else
                                            {
                                                //MessageBox.Show("123");
                                                g.FillRectangle(new SolidBrush(panel.BackColor), index * width / split, 0, width / split, height);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        g.Clear(tabiwCGradBaseColor_lbl.BackColor); // 填滿整個backBitmap  
                    }
                    if (!tabiwCGradColorBar_rdo.Checked)
                    {
                        int stepNum = Math.Min(int.Parse(tabiwCGradStep_cmb.Text), divisionSize); // 獲取漸層階數
                        int startColorValue = int.Parse(tabiwCGradFirstLevel_cmb.Text); // 開始顏色值  
                        int endColorValue = int.Parse(tabiwCGradLastLevel_cmb.Text); // 結束顏色值  
                        bitmap = ApplyTransparency(bitmap, startColorValue, endColorValue, tabiwCGradHWay_rdo.Checked, stepNum, divisions, tabiwCGradReverse_chk.Checked);
                        bitmap = ConvertARGBToRGB(bitmap);
                    }

                    // 使用漸層顏色
                }
            }
            if (tabiwCustom_btn.Text == "背景生成")
            {
                tabiwBack_pic.Image = bitmap;
            }
            else
            {
                tabiwWin_pic.Image = bitmap;
            }
        }  // Window 自製圖
        // mainWindow操控----------------------------------------------------------------------------------------------
        private void mainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                FullScreen(sender, e);
                //// 切換全螢幕模式
                //if (this.FormBorderStyle == FormBorderStyle.None)
                //{
                //    this.FormBorderStyle = FormBorderStyle.Sizable;
                //    this.WindowState = FormWindowState.Normal;
                //}
                //else
                //{
                //    this.FormBorderStyle = FormBorderStyle.None;
                //    this.WindowState = FormWindowState.Maximized;
                //}
            }
        }  //mainWindow_KeyDown
        // 拖放事件--------------------------------------------------------------------------------------------------------
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
        }  // pictureBox允許jpg、bmp、png拖放
        private void pictureBox_DragDrop(object sender, DragEventArgs e)
        {
            // 獲取拖入的檔案
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                string filePath = files[0];
                try
                {
                    if (showimgPicture_pic.Image != null)
                    {
                        showimgPicture_pic.Image.Dispose();
                        showimgPicture_pic.Image = null;
                    }
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
        }  // pictureBox拖放後執行
        private void tabilPatternList_lst_DragDrop(object sender, DragEventArgs e)
        {
            // 獲取拖入的檔案  
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                // 過濾只允許檔名為 config.txt 的檔案  
                var configFiles = files.Where(file => Path.GetFileName(file).ToLower() == "config.txt").ToArray();

                if (configFiles.Length > 0)
                {
                    foreach (var filePath in configFiles)
                    {
                        try
                        {
                            MessageBox.Show(filePath);
                            // 在這裡處理 config.txt 檔案的邏輯  
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"無法載入文件：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("只允許拖入檔名為 config.txt 的檔案！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }  // tabilPatternList_lst拖放後執行
        private void tabilPatternList_lst_DragEnter(object sender, DragEventArgs e)
        {
            // 檢查檔案是否為支援的 config.txt  
            bool IsSupportedFile(string filePath)
            {
                string filename = Path.GetFileName(filePath).ToLower();
                return filename == "config.txt" || filename.EndsWith(".json") || filename.EndsWith(".xml");
            }

            // 檢查拖入的資料是否包含檔案，並且是支援的 config.txt  
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Any(file => IsSupportedFile(file)))
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
        }  // tabilPatternList_lst允許txt拖放
        // 其他--------------------------------------------------------------------------------------------------------
        private void button1_Click(object sender, EventArgs e)
        {
            //showimgPicture_pic.Image = Resources.run;
            //showimgPicture_pic.SizeMode = PictureBoxSizeMode.Zoom; // 設置圖片縮放模式

            MessageBox.Show($"{tabiemPixelGray_vsc.Value}");
        }  //menustripAAA 測試用按鍵
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
        // listbox功能-----------------------------------------------------------------------------------
        private Color switchcolor(string sc)
        {
            if (sc.Contains("."))
            {
                // 這裡的 Name 是從 ListBox 中獲取的顏色名稱
                string[] colorParts = sc.Split('.');
                return Color.FromArgb(int.Parse(colorParts[0]), int.Parse(colorParts[1]), int.Parse(colorParts[2])); // 修改 Form1 的背景顏色
            }
            else
            {
                // 根據傳入的顏色名稱來設置顏色
                try
                {
                    return Color.FromName(sc);
                }
                catch
                {
                    // 如果顏色名稱無效，則使用 Color.FromName 方法將其轉換為顏色
                    MessageBox.Show($"無法識別顏色：{sc}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return Color.Gray;
                }
            }
        }
        private void funcFrame(string color, string line)
        {
            if (color.EndsWith("%gary"))
            {
                int cc = (int)((float.Parse(color.Split('%')[0]) / 100) * 256) - 1;
                tabiefBack_btn.BackColor = Color.FromArgb(cc, cc, cc);
            }
            else
            {
                // 操作 Form1 的控件，例如更改 Label 的文字
                tabiefBack_btn.BackColor = switchcolor(color);
            }
            // 根據傳入的 line 參數來設置其他控件的狀態
            string[] li = line.Split('-');
            if (li.Length >= 2)
            {
                tabiefCenter_chk.Checked = li[0].Contains("c") ? true : false;
                tabiefOutside_chk.Checked = li[0].Contains("o") ? true : false;
                tabiefNine_chk.Checked = li[0].Contains("n") ? true : false;
                tabiefLineColor_btn.BackColor = switchcolor(li[1]);
            }
            else
            {
                tabiefCenter_chk.Checked = false;
                tabiefOutside_chk.Checked = false;
                tabiefNine_chk.Checked = false;
            }
        } // listbox控制Frame
        private void funcChess(string num, string hv)
        {
            string[] numParts = num.Split('*');
            if (numParts.Length >= 2)
            {
                tabiecBaseColor_lbl.BackColor = Color.White;
                tabiecHNum_nud.Value = int.Parse(numParts[0]);
                tabiecVNum_nud.Value = int.Parse(numParts[1]); 
                tabiecHFlip_chk.Checked = hv.Contains("h") ? true : false;
                tabiecVFlip_chk.Checked = hv.Contains("v") ? true : false;
            }
            else 
            {
                tabiecHFlip_chk.Checked = false;
                tabiecVFlip_chk.Checked = false;
            }
        }  // listbox控制棋盤格
        private void funcGrad(string color, string hv)
        {
            if (hv.StartsWith("h"))
            {
                tabigHWay_rdo.Checked = true;
            }
            else if (hv.StartsWith("v"))
            {
                tabigVWay_rdo.Checked = true;
            }
            else
            {
                tabigColorBar_rdo.Checked = true;
            }
            if (color.StartsWith("colorbar"))
            {
                tabigOther_chk.Checked = true;
                if (color.EndsWith("h"))
                {
                    tabigOtherHSplit_rdo.Checked = true;
                }
                else if (color.EndsWith("v"))
                {
                    tabigOtherVSplit_rdo.Checked = true;
                }
                SplitPanelCreat(tabigOtherSplit_nud, EventArgs.Empty); // 修正 'e' 為 'EventArgs.Empty'
                if (color.Contains("four"))
                {
                    ColorBar_btn_Click(tabigFourColor_btn, EventArgs.Empty); // 修正 'e' 為 'EventArgs.Empty'  
                }
                else if(color.Contains("eight"))
                {
                    ColorBar_btn_Click(tabigEightColor_btn, EventArgs.Empty); // 修正 'e' 為 'EventArgs.Empty'  
                }
                else
                {
                    string[] colorbars = color.Split('-');
                    if (colorbars.Length >= 3)
                    {
                        tabigOtherVSplit_rdo.Checked = colorbars[1].Contains("v");
                        string[] barcolor = colorbars[2].Split('.');
                        tabigOtherSplit_nud.Value = int.Parse(barcolor[0]);
                        for (int i = 0; i < barcolor.Length - 1; i++)
                        {
                            tabigOtherSplit_pnl.Controls[i].BackColor = switchcolor(barcolor[i + 1]);
                        }
                    }
                }
            }
            else
            {
                tabigOther_chk.Checked = false;
                tabigBaseColor_lbl.BackColor = switchcolor(color); // 修改 Form1 的背景顏色  
            }
        }  // listbox控制漸層
        private void funcMask(string num, string onoff)
        {
            string[] numParts = num.Split('-');
            if (numParts.Length >= 2)
            {
                string[] numhw = numParts[1].Split('*');
                if (numhw.Length >= 2)
                {
                    tabiemWNum_nud.Value = int.Parse(numhw[0]);
                    tabiemHNum_nud.Value = int.Parse(numhw[1]);
                }
                else
                {
                    tabiemWNum_nud.Value = int.Parse(numParts[1]);
                    tabiemHNum_nud.Value = int.Parse(numParts[1]);
                }

                MaskPanelCreat(tabiemPixelClear_btn, EventArgs.Empty);
                onoff = onoff.Replace(".", "");
                if (numParts[0].Contains("p"))
                {
                    tabiemPixel_rdo.Checked = true;
                    for (int i = 0; i < tabiemHNum_nud.Value; i++)
                    {
                        for (int j = 0; j < tabiemWNum_nud.Value; j++)
                        {
                            Panel targetPanel =
                                tabiemPixelPanel_pnl.Controls[i * (int)tabiemWNum_nud.Value + j] as Panel;
                            if (targetPanel != null)
                            {
                                // 解析 Panel 的座標 (Tag 格式為 "x,y")
                                string[] coordinates = targetPanel.Tag.ToString().Split(',');
                                // 計算在 onoff 字符串中的索引
                                int index = (int.Parse(coordinates[1]) * (int)tabiemWNum_nud.Value) + int.Parse(coordinates[0]);
                                // 設置 Panel 的顏色
                                targetPanel.BackColor = onoff[index] == '0' ? Color.Black : 
                                    onoff[index] == '1' ? Color.White : Color.Gray;
                            }
                        }
                    }
                }
                else
                {
                    tabiemSubPixel_rdo.Checked = true;
                    //MaskPanelCreat(tabiemPixelClear_btn, EventArgs.Empty);
                    if (numParts.Length >= 3)
                    {
                        tabiemPixelGray_vsc.Value = 255 - int.Parse(numParts[2]);
                    }

                    for (int i = 0; i < tabiemHNum_nud.Value; i++)
                    {
                        for (int j = 0; j < tabiemWNum_nud.Value * 3; j++)
                        {
                            Panel targetPanel =
                                tabiemPixelPanel_pnl.Controls[i * (int)tabiemWNum_nud.Value * 3 + j] as Panel;
                            if (targetPanel != null)
                            {
                                // 解析 Panel 的座標 (Tag 格式為 "x,y")
                                string[] coordinates = targetPanel.Tag.ToString().Split(',');
                                // 計算在 onoff 字符串中的索引
                                int index = (int.Parse(coordinates[1]) * (int)tabiemWNum_nud.Value * 3) + int.Parse(coordinates[0]);
                                // 設置 Panel 的顏色
                                if (onoff[index] == '0')
                                {
                                    targetPanel.BackColor = Color.Black;
                                }
                            }
                        }
                    }
                }
            }
        }  // listbox控制遮罩
        private void funcWind(string color, string sizeloc)
        {
            if (color.Contains("excel"))
            {
                tabiwBackColor_rdo.Checked = true;
                tabiwWinCustom_rdo.Checked = true;
                windowPictureBox_Click(tabiwWinCustom_rdo, EventArgs.Empty);
                tabiwCustom_cmb.SelectedIndex = 0;
                tabiwBack_pic.BackColor = Color.Gray;
                int wnum = 2;
                int hnum = 2;
                string onoff = "1010";
                switch (color)
                {
                    case "excel1":
                        onoff = "10.22";
                        wnum = hnum = 2;
                        break;
                    case "excel2":
                        onoff = "0011.2222";
                        wnum = 4;
                        hnum = 2;
                        break;
                    case "excel3":
                        onoff = "0001.2222";
                        wnum = 4;
                        hnum = 2;
                        break;
                    case "excel4":
                        onoff = "000000000111.222222222222";
                        wnum = 12;
                        hnum = 2;
                        break;
                    case "excel5":
                        onoff = "000111000111000000.222222222222222222";
                        wnum = 18;
                        hnum = 2;
                        break;
                    case "excel6":
                        onoff = "000111000111000000000111,222222222222222222222222";
                        wnum = 24;
                        hnum = 2;
                        break;
                    case "excel7":
                        onoff = "000000100000100000.222222222222222222.222222222222222222.000011000011000000";
                        wnum = 18;
                        hnum = 4;
                        break;
                    default:
                        break;
                }

                onoff = onoff.Replace(".", "");
                tabiwCMaskWNum_nud.Value = wnum;
                tabiwCMaskHNum_nud.Value = hnum;
                for (int i = 0; i < hnum; i++)
                {
                    for (int j = 0; j < wnum; j++)
                    {
                        Panel targetPanel =
                            tabiwCMaskPixelPanel_pnl.Controls[i * 2 + j] as Panel;
                        if (targetPanel != null)
                        {
                            // 解析 Panel 的座標 (Tag 格式為 "x,y")
                            string[] coordinates = targetPanel.Tag.ToString().Split(',');
                            // 計算在 onoff 字符串中的索引
                            int index = (int.Parse(coordinates[1]) * (int)tabiwCMaskWNum_nud.Value) + int.Parse(coordinates[0]);
                            // 設置 Panel 的顏色
                            if (onoff[i * 2 + j] == '1')
                            {
                                targetPanel.BackColor = Color.White;
                            }
                            else if (onoff[i * 2 + j] == '2')
                            {
                                targetPanel.BackColor = Color.Gray;
                            }
                            else
                            {
                                targetPanel.BackColor = Color.Black;
                            }
                        }
                    }
                }
                Custom_btn_Click(tabiwCustom_btn, EventArgs.Empty);
            }
            else
            {
                string[] colorParts = color.Split('-');
                if (colorParts.Length >= 2)
                {
                    tabiwBackColor_rdo.Checked = true;
                    tabiwWinColor_rdo.Checked = true;
                    tabiwBack_pic.BackColor = switchcolor(colorParts[0]); // 修改 Form1 的背景顏色
                    tabiwBack_pic.Image = null;
                    tabiwWin_pic.BackColor = switchcolor(colorParts[1]); // 修改 Form1 的背景顏色
                    tabiwWin_pic.Image = null;
                }
            }

            if (sizeloc.Contains("c"))
            {
                tabiwWinLocCenter_rdo.Checked = true;
            }

            if (sizeloc.Contains("%"))
            {
                tabiwWinSizePercent_rdo.Checked = true;
                tabiwWinSizePercent_nud.Value = int.Parse(sizeloc.Split('%')[0]);
            }
            else 
            {
                tabiwWinSizePercent_rdo.Checked = true;
                tabiwWinSizePercent_nud.Value = 50;
            }
        } // listbox控制窗口

        private void tabiwDiagonal_btn_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox targetchk)
            {
                if (targetchk.Checked)
                {
                    tabiwWinLoc_grp.Enabled = false;
                    tabiwWinImg_rdo.Enabled = false;
                    tabiwWinCustom_rdo.Enabled = false;
                }
                else
                {
                    tabiwWinLoc_grp.Enabled = true;
                    tabiwWinImg_rdo.Enabled = true;
                    tabiwWinCustom_rdo.Enabled = true;
                }
            }
        }  // Window對角線選擇
    }
}