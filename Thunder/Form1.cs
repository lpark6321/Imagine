using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Deployment.Application;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Thunder.Properties;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
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
        public bool IsDynamicMode => tabimgeFuncList.Text == "Dynamic"; // 目前功能是否為 Dynamic
        public string PatternName => mnsPattername_txt.Text; // 上方檔名欄位
        //mainWindow---------------------------------------------------------------------------------------------------
        public mainWindow()
        {
            InitializeComponent();
            // 停用輸入法，否則中文輸入法會把空白鍵、=、- 等快捷鍵攔截成 ProcessKey；
            // 只設 Form.ImeMode 不會套用到子控制項，必須逐一設定。檔名欄位保留輸入法可打中文
            DisableIme(this);
            mnsPattername_txt.TextBox.ImeMode = ImeMode.NoControl;
            SetupPatternList();
            InitConfigEditor();
            InitWindowOverlayList();
            InitEditorToConfig();
            InitMaskQuickButtons();
            InitFrameTextMode();
            InitFrameRedBlue();
            InitGradientReverse();
            InitWindowDiagonal();
            InitWindowTabs();
            InitQuickButtons();
            InitResizeMode();
            InitPreviewPanel();
            InitDirListReload();
            InitMassTooltip();
            InitMaskZoom();
            InitImgListExport();
            InitMultiScreen();
            //Mainwindow_Load(this, EventArgs.Empty);
            mypicture = new MyPicture(this);
            Instance = this; // 初始化 Singleton 實例
        }
        [DllImport("imm32.dll")]
        private static extern int ImmGetVirtualKey(IntPtr hWnd);
        [DllImport("imm32.dll")]
        private static extern IntPtr ImmGetContext(IntPtr hWnd);
        [DllImport("imm32.dll")]
        private static extern bool ImmNotifyIME(IntPtr hIMC, int dwAction, int dwIndex, int dwValue);
        [DllImport("imm32.dll")]
        private static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);
        [DllImport("user32.dll")]
        private static extern int MapVirtualKey(int uCode, int uMapType);
        internal static Keys GetImeOriginalKey(ref Message msg)
        {
            // 輸入法攔截的按鍵 wParam 會變成 VK_PROCESSKEY (229)；需在 ProcessCmdKey 階段（TranslateMessage 之前）呼叫
            if ((msg.Msg != 0x100 && msg.Msg != 0x104) || (int)msg.WParam != (int)Keys.ProcessKey) return Keys.None;
            int vk = ImmGetVirtualKey(msg.HWnd);
            if (vk == 0 || vk == (int)Keys.ProcessKey)
            {
                int scanCode = (int)((long)msg.LParam >> 16) & 0xFF;
                vk = MapVirtualKey(scanCode, 1);  // MAPVK_VSC_TO_VK：由掃描碼換回按鍵
            }
            if (vk == 0) return Keys.None;
            IntPtr context = ImmGetContext(msg.HWnd);
            if (context != IntPtr.Zero)
            {
                ImmNotifyIME(context, 0x15, 0x4, 0);  // NI_COMPOSITIONSTR, CPS_CANCEL：取消輸入法組字
                ImmReleaseContext(msg.HWnd, context);
            }
            return (Keys)vk;
        }  // 還原被輸入法攔截的按鍵
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Alt | Keys.Space))
            {
                Generate_Click(showimgGenerate_btn, EventArgs.Empty);  // 原本由工具列 Generate 提供的快捷鍵
                return true;
            }
            // 輸入法攔截的快捷鍵在這裡還原處理，並回傳 true 讓輸入法收不到（文字欄位除外）
            if ((keyData & Keys.KeyCode) == Keys.ProcessKey && !IsTextInputFocused())
            {
                Keys realKey = GetImeOriginalKey(ref msg);
                if (realKey != Keys.None)
                {
                    mainWindow_KeyDown(this, new KeyEventArgs(realKey | ModifierKeys));
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }  // 處理被輸入法攔截的快捷鍵
        private static void DisableIme(Control parent)
        {
            foreach (Control child in parent.Controls)
            {
                child.ImeMode = ImeMode.Disable;
                DisableIme(child);
            }
        }  // 停用所有子控制項的輸入法
        private void Mainwindow_Load(object sender, EventArgs e)
        {
            //var numericUpDownItem = new ToolStripNumericUpDown();
            //numericUpDownItem.NumericUpDownControl.ValueChanged += (s, ev) =>
            //{
            //    MessageBox.Show($"當前值: {numericUpDownItem.NumericUpDownControl.Value}");
            //};
            //menuStrip.Items.Add(numericUpDownItem);
            //AddNumericUpDownToMenuStrip();

            LoadDefaultConfig();  // 啟動時自動載入程式資料夾的 config.txt
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
            public Bitmap _currentBitmap;
            public string tag { get; set; }
            public int _width;
            public int _height;
            public int startX { get; set; } // 球體的初始 X 座標 
            public int startY { get; set; } // 球體的初始 Y 座標
            public int moveSpeedX { get; set; } // 球體的水平移動速度
            public int moveSpeedY { get; set; } // 球體的垂直移動速度
            public int objectSize { get; set; } // 球體的半徑

            private mainWindow m1 { get; }  // 取得mainWindow的實例
            public MyPicture(mainWindow m0)
            {
                m1 = m0 ?? throw new ArgumentNullException(nameof(m0));
                _width = m1.mnsW_txt.Text != "" ? int.Parse(m1.mnsW_txt.Text) : 1920;
                _height = m1.mnsH_txt.Text != "" ? int.Parse(m1.mnsH_txt.Text) : 1080;
                _currentBitmap = new Bitmap(_width, _height);
                tag = "";
                startX = 50; startY = 50;
            }  // 建構子
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
                else
                {
                    _currentBitmap.Save(filename, ImageFormat.Bmp);
                }
            }  // 儲存圖片

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
            }  // 開啟圖片
            public string Getsize()
            {
                _width = _currentBitmap.Width;
                _height = _currentBitmap.Height;
                return $"{_width} x {_height}"; // 更新圖片大小
            }  // 取得圖片大小
            public Bitmap Getpicture()
            {
                if (_currentBitmap == null || m1.IsImageDisposed(_currentBitmap))
                {
                    //MessageBox.Show("圖片尚未初始化！");
                    return null;
                }
                return _currentBitmap;
            }  // 取得圖片
            public Image Setpicture(Image image, String stringtag = "")
            {
                if (_currentBitmap != null)
                {
                    _currentBitmap.Dispose(); // 釋放舊的 Bitmap
                    _currentBitmap = null;
                }

                _currentBitmap = (Bitmap)image.Clone(); // 複製新的 Bitmap
                _width = _currentBitmap.Width;
                _height = _currentBitmap.Height;
                tag = stringtag;
                return _currentBitmap; // 回傳圖片
            }  // 設定圖片
            public void SetDymanic(int speedx, int speedy, int size, int x, int y)
            {
                moveSpeedX = speedx;
                moveSpeedY = speedy;
                objectSize = size;
                startX = x;
                startY = y;
            }  // 設定dymanic參數
            public (int X, int Y, int loc) Getloc()
            {
                return (startX, startY, objectSize);
            }  // 取得動態參數
            public void Move(string direction,bool strip = false)
            {
                // 更新球體位置
                //startX += moveSpeedX;
                //startY += moveSpeedY;

                if (direction == "h")
                {
                    startX += moveSpeedX;
                }
                else if (direction == "v")
                {
                    startY += moveSpeedY;
                }
                else if (direction == "b")
                {
                    startX += moveSpeedX;
                    startY += moveSpeedY;
                }
                // 碰撞檢測 (邊界反彈)
                if (strip)
                {
                    if (startX <= 0 || startX + objectSize >= _width)
                    {
                        moveSpeedX = -moveSpeedX; // 水平反彈
                    }
                    if (startY <= 0 || startY + objectSize >= _height)
                    {
                        moveSpeedY = -moveSpeedY; // 垂直反彈
                    }
                }
                else
                {
                    if (startX <= 0 || startX + objectSize * 2 >= _width)
                    {
                        moveSpeedX = -moveSpeedX; // 水平反彈
                    }
                    if (startY <= 0 || startY + objectSize * 2 >= _height)
                    {
                        moveSpeedY = -moveSpeedY; // 垂直反彈
                    }
                }
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
            UpdateRefreshRate();
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
            tabimgeDynamic.Visible = false;

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
                case "Dynamic":
                    tabimgeDynamic.Visible = true;
                    tabimgeDynamic.Dock = DockStyle.Fill;
                    break;
                default:
                    // 如果沒有匹配的項目，則不顯示任何 GroupBox
                    break;
            }
        }
        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            _configDraftFocused = false;
            _shownItem = null;  // 換分頁後空白鍵依新分頁重新產生
            if (tabControl.SelectedTab == tabDirList)
            {
                ssrStatus_lbl.Text = Application.StartupPath;
                // 只在第一次切換時讀取資料夾，之後按「重新讀取」更新
                if (!_dirListLoaded)
                {
                    _dirListLoaded = true;
                    PopulateListView();
                }
            }
            else if (tabControl.SelectedTab == tabImgList)
            {
                ssrStatus_lbl.Text = $"Config：{tabilPatternList_lst.Tag}";  // 顯示目前 config 來源（啟動時已載入）
            }
        }  // 切換tab
        // 產生圖片----------------------------------------------------------------------------------------------------
        public void Generate_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == tabImgList && _configDraftFocused)
            {
                PreviewConfigDraft();  // 正在撰寫 config：預覽編輯區的內容
                return;
            }
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
                case "Dynamic":
                    type = "dynamic";
                    break;
                default:
                    //MessageBox.Show("請選擇一個有效的功能！");
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

                //if (type != "dynamic")
                //{
                    showimgPicture_pic.Image = mypicture.Getpicture();
                    showimgPicture_pic.SizeMode = PictureBoxSizeMode.Zoom;
                //}
                // 顯示加入 ImgList 時的 config；無法轉換時顯示不支援
                var ignored = new List<string>();
                string line = EditorToConfigLine(ignored, out string error);
                ShowPreviewInfo(width, height, line);
                if (line == null)
                {
                    ShowConfigNote(error == null ? "不支援輸出到 ImgList" : $"不支援輸出到 ImgList：{error}");
                }
                else if (ignored.Count > 0)
                {
                    ShowConfigNote($"{showimgConfig_lbl.Text}\n（不會寫入：{string.Join("、", ignored)}）");
                }
                _shownItem = null;
                if (tabControl.SelectedTab == tabImgEditor && line != null)
                {
                    AddRecent(RecentItem.FromEditor(line));  // 最近使用以「類型_參數_選項」記錄
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"圖片生成失敗：{ex.Message}");
            }
        }  // 按下生成圖片
        private Bitmap _dynamicBackground;  // Dynamic 產生的背景（mypicture 內的同一張圖），FullScreen 時據此開始動畫
        private bool _generatingFromConfig; // true：由 ImgList 的 config 產生（Dynamic 只預覽不自動全螢幕、Window 不疊加 ImgEditor 清單）
        public void GenerateImage(string type, int width, int height, string tag="")
        {
            // 產生非動態圖片時停止動畫，避免小球繼續畫在新圖上
            if (type != "dynamic")
            {
                timerDynamic.Stop();
            }
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
                        // 文字模式：文字依格狀座標鋪滿畫面（畫在框線之下）
                        if (tabiefText_chk.Checked && tabiefText_txt.Text.Trim() != "")
                        {
                            string tileText = tabiefText_txt.Text.Trim();
                            using (Font tileFont = new Font("Microsoft JhengHei", (float)tabiefTextSize_nud.Value, FontStyle.Bold, GraphicsUnit.Pixel))
                            {
                                DrawTiledText(g, tileText.Length > 10 ? tileText.Substring(0, 10) : tileText, tileFont, tabiefTextColor_btn.BackColor,
                                    (float)tabiefTextCharGap_nud.Value, (float)tabiefTextLineGap_nud.Value, (float)tabiefTextStringGap_nud.Value, width, height);
                            }
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
                        // 左紅右藍：上下白線、左紅線、右藍線，固定 1 pixel（左右畫滿整個高度，角落為紅 / 藍）
                        if (tabiefRedBlue_chk.Checked)
                        {
                            g.FillRectangle(Brushes.White, 0, 0, width, 1);
                            g.FillRectangle(Brushes.White, 0, height - 1, width, 1);
                            g.FillRectangle(Brushes.Red, 0, 0, 1, height);
                            g.FillRectangle(Brushes.Blue, width - 1, 0, 1, height);
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
                        for (int y = 0; y < height; y += (int)Math.Ceiling((double)height / (int)tabiecVNum_nud.Value))
                        {
                            for (int x = 0; x < width; x += (int)Math.Ceiling((double)width / (int)tabiecHNum_nud.Value))
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
                            DrawScreenshot(g, tabiwBack_pic.Image, 0, 0, width, height);
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
                                // 依斜角下拉選單畫出包含該角落的三角形（以對角線切開畫面）
                                Point topLeft = new Point(0, 0), topRight = new Point(width, 0), bottomRight = new Point(width, height), bottomLeft = new Point(0, height);
                                Point[] trianglePoints;
                                switch (tabiwDiagonal_cmb.SelectedIndex)
                                {
                                    case 1: trianglePoints = new[] { topLeft, topRight, bottomLeft }; break;      // 左上
                                    case 3: trianglePoints = new[] { topRight, bottomRight, bottomLeft }; break;  // 右下
                                    case 4: trianglePoints = new[] { topLeft, bottomRight, bottomLeft }; break;   // 左下
                                    default: trianglePoints = new[] { topLeft, topRight, bottomRight }; break;    // 右上
                                }
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
                                    DrawScreenshot(g, tabiwWin_pic.Image, xstart, ystart, newWidth, newHeight);
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
                                DrawScreenshot(g, tabiwWin_pic.Image, xstart, ystart, newWidth, newHeight);
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
                        // 疊加清單中的窗口與線條（依清單順序，config 產生時不使用）
                        if (!_generatingFromConfig)
                        {
                            foreach (ListViewItem overlayItem in tabiwOverlay_lvw.Items)
                            {
                                ((WindowOverlay)overlayItem.Tag).Draw(g, width, height);
                            }
                        }
                        mypicture.Setpicture(CurrentBitmap);
                        break;
                    case "mask":
                        if (true) // 如果True 就用Lockbit 如果False就用Setpixel
                        {
                            int pixelW = (int)tabiemWNum_nud.Value;
                            int pixelH = (int)tabiemHNum_nud.Value;
                            if (pixelW * pixelH <= 9216)  //像素太少要加速
                            {
                                int minhw = Math.Min(256 / pixelW, 144 / pixelH);
                                pixelW = minhw * pixelW;
                                pixelH = minhw * pixelH;
                                if (tabiemPixel_rdo.Checked)
                                {
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

                                                for (int i = 0; i < pixelH; i += (int)tabiemHNum_nud.Value)
                                                {
                                                    for (int j = 0; j < pixelW; j += (int)tabiemWNum_nud.Value)
                                                    {
                                                        // 計算像素在數組中的索引
                                                        //int index = (y * stride) + (x * 4);
                                                        int index = (y * stride) + (x * 4) + i * stride + j*4;

                                                        // 設置像素顏色
                                                        Color color = panel.BackColor;
                                                        pixelBuffer[index] = color.B;       // 藍色
                                                        pixelBuffer[index + 1] = color.G;  // 綠色
                                                        pixelBuffer[index + 2] = color.R;  // 紅色
                                                        pixelBuffer[index + 3] = color.A;  // 透明度
                                                    }
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
                                else if (tabiemSubPixel_rdo.Checked)
                                {
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
                                        for (int k = 0; k < tabiemPixelPanel_pnl.Controls.Count; k += 3)
                                        {
                                            if (k + 2 < tabiemPixelPanel_pnl.Controls.Count)
                                            {
                                                // 獲取三個 Panel
                                                Panel panel1 = tabiemPixelPanel_pnl.Controls[k] as Panel;
                                                Panel panel2 = tabiemPixelPanel_pnl.Controls[k + 1] as Panel;
                                                Panel panel3 = tabiemPixelPanel_pnl.Controls[k + 2] as Panel;

                                                if (panel1 != null && panel2 != null && panel3 != null)
                                                {
                                                    //// 根據 Panel 的背景顏色組合成 Color
                                                    //int subpixelr = panel1.BackColor.R;
                                                    //int subpixelg = panel2.BackColor.G;
                                                    //int subpixelb = panel3.BackColor.B;

                                                    //Color combinedColor = Color.FromArgb(subpixelr, subpixelg, subpixelb);

                                                    // 解析 Panel 的座標 (Tag 格式為 "x,y")
                                                    string[] coordinates = panel1.Tag.ToString().Split(',');
                                                    int x = int.Parse(coordinates[0]) / 3;
                                                    int y = int.Parse(coordinates[1]);
                                                    for (int i = 0; i < pixelH; i += (int)tabiemHNum_nud.Value)
                                                    {
                                                        for (int j = 0; j < pixelW; j += (int)tabiemWNum_nud.Value)
                                                        {
                                                            // 計算像素在數組中的索引
                                                            //int index = (y * stride) + (x * 4);
                                                            int index = (y * stride) + (x * 4) + i * stride + j * 4;

                                                            // 設置像素顏色
                                                            pixelBuffer[index] = panel3.BackColor.B;      // 藍色
                                                            pixelBuffer[index + 1] = panel2.BackColor.G;  // 綠色
                                                            pixelBuffer[index + 2] = panel1.BackColor.R;  // 紅色
                                                            pixelBuffer[index + 3] = 255;  // 透明度不透明
                                                        }
                                                    }
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
                            else //像素多的不加速
                            {
                                if (tabiemPixel_rdo.Checked)
                                {

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
                        }
                        else
                        {
                            // 這裡可以添加遮罩的生成邏輯 Setpixel
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
                        }  // Setpixel方法
                        mypicture.Setpicture(CurrentBitmap);
                        break;
                    case "adjust":
                        // 這裡可以添加調整圖片的邏輯
                        break;
                    case "dynamic":
                        if (mypicture.tag == "image")
                        {
                            // 使用圖片
                            g.DrawImage(mypicture.Getpicture(), 0, 0, width, height);
                        }
                        else
                        {
                            // 填充背景顏色
                            g.Clear(tabiedBackColor_btn.BackColor);
                        }
                        mypicture.SetDymanic((int)tabiedMoveSpeed_nud.Value, (int)tabiedMoveSpeed_nud.Value, (int)tabiedObjectSize_nud.Value, 0, 0);

                        //// 繪製縮圖
                        //using (Brush ballBrush = new SolidBrush(tabiedObjectColor_btn.BackColor))
                        //{
                        //    g.FillEllipse(ballBrush, mypicture.startX, mypicture.startY, mypicture.objectSize * 2, mypicture.objectSize * 2); // 繪製球體
                        //}

                        //if (CurrentBitmap == null)
                        //{
                        //    throw new InvalidOperationException("CurrentBitmap 尚未初始化！");
                        //}
                        mypicture.Setpicture(CurrentBitmap, mypicture.tag);
                        _dynamicBackground = mypicture.Getpicture();  // 記住動態背景，FullScreen 時才開始播放
                        if (!_generatingFromConfig)
                        {
                            FullScreen(showimgGenerate_btn, EventArgs.Empty);
                        }
                        break;
                    default:
                        throw new NotSupportedException("不支援的圖片類型！");
                }
                // 設置圖片的標籤
                //mypicture.tag = tag != "" ? tag : mypicture.tag;
            }
            // 釋放資源
            if (CurrentBitmap != null)
            {
                CurrentBitmap.Dispose();
                CurrentBitmap = null;
            }

        }  // 生成圖片

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            throw new NotImplementedException();
        }

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

                    if (pictureBox.Name == "tabiedBackImg_pic")
                    {
                        timerDynamic.Tag = "openpic";
                        //timerDynamic.Stop();
                    }
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
                    showimgSize_lbl.Text = $"圖片大小：{mypicture.Getsize()}"; // 更新狀態列顯示圖片大小
                    if (pictureBox.Name == "tabiedBackImg_pic")
                    {
                        timerDynamic.Tag = "";
                        //_pictureWindow.Close();
                        //Generate_Click(showimgGenerate_btn, EventArgs.Empty); // 生成圖片
                        //GenerateImage("dynamic", int.Parse(mnsW_txt.Text), int.Parse(mnsH_txt.Text));
                        //timerDynamic.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"無法載入圖片：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }  // 圖片導入
        private void Save_Click(object sender, EventArgs e)
        {
            if (sender is Button targetbutton && targetbutton.Name == "showimgSave_btn")
            {
                // Save 按鈕：一律跳出視窗選擇存檔名稱
                mypicture.SaveImage();
            }
            else if (tabiefMass_chk.Checked && tabimgeFuncList.Text == "Frame&CrossLine&Border")
            {
                // AutoSave + 批量：依步進輸出各灰階，最後一階補到最大值
                Color co = tabiefBack_btn.BackColor;
                int max = Math.Max(co.R, Math.Max(co.G, co.B));
                int step = Math.Max(1, (int)tabiefMassNum_nud.Value);
                for (int i = 0; ; i += step)
                {
                    int level = Math.Min(i, max);
                    string baseName = $"{mnsPattername_txt.Text}_{level}";
                    string filePath = Path.Combine(Application.StartupPath, $"{baseName}.png"); // 儲存路徑
                    // 檔案已存在時加上 (2)、(3)…，避免覆蓋
                    for (int n = 2; File.Exists(filePath); n++)
                    {
                        filePath = Path.Combine(Application.StartupPath, $"{baseName}({n}).png");
                    }
                    using (Bitmap bitmap = new Bitmap(int.Parse(mnsW_txt.Text), int.Parse(mnsH_txt.Text)))
                    {
                        using (Graphics g = Graphics.FromImage(bitmap))
                        {
                            g.Clear(Color.FromArgb(Math.Min(level, co.R), Math.Min(level, co.G), Math.Min(level, co.B))); // 填充背景顏色
                        }
                        bitmap.Save(filePath, ImageFormat.Png); // 儲存圖片
                    }
                    if (level == max) break;
                }
            }
            else
            {
                string fileName = $"{mnsPattername_txt.Text}.bmp"; // 設定檔名
                string filePath = Path.Combine(Application.StartupPath, fileName); // 儲存路徑

                // 判斷檔案是否存在
                while (File.Exists(filePath))
                {
                    // 判斷檔名是否包含 "_" 並且後面是數字
                    string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                    string[] parts = nameWithoutExtension.Split('_');
                    if (parts.Length > 1 && int.TryParse(parts[parts.Length - 1], out int number)) // 修改索引運算子
                    {
                        // 如果是數字，遞增
                        parts[parts.Length - 1] = (number + 1).ToString(); // 修改索引運算子
                        nameWithoutExtension = string.Join("_", parts);
                    }
                    else
                    {
                        // 如果不是數字，新增 "_2"
                        nameWithoutExtension += "_2";
                    }

                    // 更新檔名與路徑
                    fileName = $"{nameWithoutExtension}.bmp";
                    filePath = Path.Combine(Application.StartupPath, fileName);
                }

                // 儲存圖片
                mypicture.SaveImage(filePath);
                // 檔名欄位跟著顯示這次存檔的編號
                mnsPattername_txt.Text = Path.GetFileNameWithoutExtension(fileName);
            }
        }  // 儲存圖片
        // 圖片顯示----------------------------------------------------------------------------------------------------
        public void FullScreen(object sender, EventArgs e)
        {
            // 多螢幕模式（主螢幕以外至少一個螢幕）改由 FullScreenMulti 處理
            List<Screen> others = OtherScreens();
            if (_multiMode != MultiScreenMode.Single && others.Count > 0)
            {
                FullScreenMulti(others);
                return;
            }
            // 獲取選擇的螢幕
            string selectedScreen = mnsScreenlist_cmb.SelectedItem?.ToString() ?? Screen.AllScreens[0].DeviceName;
            Screen secondScreen = Screen.AllScreens.FirstOrDefault(screen => screen.DeviceName == selectedScreen) ?? Screen.AllScreens[0];

            if (_pictureWindow == null || _pictureWindow.IsDisposed)
            {
                // 如果 _pictureWindow 尚未初始化或已被釋放，則創建新的實例  
                _pictureWindow = new pictureWindow();
                _pictureWindow.FormClosed += (s, ev) => OnScreenWindowClosed((pictureWindow)s);
            }

            //_pictureWindow.setBitmap((Bitmap)showimgPicture_pic.Image);
            _pictureWindow.setBitmap(mypicture.Getpicture());
            _pictureWindow.Caption = CurrentImageName();  // 全螢幕左上角顯示的名稱

            if (_pictureWindow != null && !_pictureWindow.IsDisposed)  //存在才執行
            {
                if (_pictureWindow.Location != secondScreen.Bounds.Location)
                {
                    //currentScreen.DeviceName
                    _pictureWindow.WindowState = FormWindowState.Normal;
                    _pictureWindow.Location = secondScreen.Bounds.Location;
                }
                _pictureWindow.WindowState = FormWindowState.Maximized;
                _pictureWindow.FormBorderStyle = FormBorderStyle.None;
                if (!_pictureWindow.Visible)
                {
                    // 設置窗口位置和大小  
                    _pictureWindow.WindowState = FormWindowState.Maximized;
                    _pictureWindow.StartPosition = FormStartPosition.Manual;
                    //_pictureWindow.Location = secondScreen.Bounds.Location;
                    _pictureWindow.Size = secondScreen.Bounds.Size;

                    // 顯示窗口
                    _pictureWindow.Show();
                    _pictureWindow.BringToFront();
                    //MessageBox.Show("Form 未顯示！");
                }
                // 目前圖片是 Dynamic 產生的背景時，全螢幕才開始播放動畫
                if (_dynamicBackground != null && ReferenceEquals(mypicture.Getpicture(), _dynamicBackground) && !timerDynamic.Enabled)
                {
                    timerDynamic.Start();
                }
            }
            else
            {
                MessageBox.Show("Form 尚未初始化或已被釋放！");
            }
        }  // 全螢幕顯示
        // 多螢幕-------------------------------------------------------------------------------------------------------
        private enum MultiScreenMode { Single, SyncOthers, SeparateOthers, SequenceOthers }
        private MultiScreenMode _multiMode = MultiScreenMode.Single;
        private ToolStripComboBox mnsMultiScreen_cmb;
        private readonly Dictionary<string, pictureWindow> _screenWindows = new Dictionary<string, pictureWindow>();
        private bool _updatingScreens;      // 產生 / 顯示多螢幕時，忽略視窗啟用事件
        private bool _closingAllScreens;
        private bool _restoringSelection;   // 還原分離螢幕的選取時，DirList 不要再觸發全螢幕
        private bool _skipScreenRegenerate; // 切換作用中螢幕時先不重新產生（之後還原選取會產生）
        private class ScreenSelection
        {
            public TabPage Tab;
            public int Index = -1;
            public RecentItem Shown;
        }  // 分離模式每個螢幕記住的分頁與項目
        private void InitMultiScreen()
        {
            // 螢幕清單左邊的多螢幕模式；預設單一螢幕，與原本相同
            mnsMultiScreen_cmb = new ToolStripComboBox
            {
                Alignment = ToolStripItemAlignment.Right, DropDownStyle = ComboBoxStyle.DropDownList, AutoSize = false, Size = new Size(130, 23),
                ToolTipText = "單一螢幕：全螢幕顯示在右邊選的螢幕\r\n" +
                              "主螢幕之外同步：主螢幕以外的螢幕全部顯示同一張圖、一起換圖\r\n" +
                              "分離第二視窗：主螢幕以外每個螢幕各自顯示，點哪個螢幕就控制哪個（主視窗會跳到那個螢幕的圖）\r\n" +
                              "主螢幕之外依序：第二螢幕顯示目前的圖、第三螢幕顯示下一張…，← → 一起往下換\r\n" +
                              "多螢幕時各螢幕依自己的解析度產生圖片；任一個全螢幕關閉時全部關閉",
            };
            mnsMultiScreen_cmb.ComboBox.ImeMode = ImeMode.Disable;
            mnsMultiScreen_cmb.Items.AddRange(new object[] { "單一螢幕", "主螢幕之外同步", "分離第二視窗", "主螢幕之外依序" });
            mnsMultiScreen_cmb.SelectedIndex = 0;
            mnsMultiScreen_cmb.SelectedIndexChanged += (s, e) =>
            {
                CloseAllFullScreen();  // 換模式時關掉目前的全螢幕，下次全螢幕依新模式開啟
                _multiMode = (MultiScreenMode)mnsMultiScreen_cmb.SelectedIndex;
                int count = OtherScreens().Count;
                ssrStatus_lbl.Text = _multiMode == MultiScreenMode.Single || count > 0
                    ? $"全螢幕模式：{mnsMultiScreen_cmb.Text}{(_multiMode == MultiScreenMode.Single ? "" : $"（主螢幕以外 {count} 個螢幕）")}"
                    : "只有主螢幕，多螢幕模式會改用單一螢幕";
            };
            // 右對齊項目依集合順序由右往左排，插在螢幕清單後面 = 顯示在它左邊
            menuStrip.Items.Insert(menuStrip.Items.IndexOf(mnsScreenlist_cmb) + 1, mnsMultiScreen_cmb);
        }  // 建立多螢幕選單
        private static List<Screen> OtherScreens() =>
            Screen.AllScreens.Where(s => !s.Primary).OrderBy(s => s.Bounds.X).ThenBy(s => s.Bounds.Y).ToList();  // 主螢幕以外，由左到右
        private static string ScreenLabel(string device) => device?.TrimStart('\\', '.') ?? "";
        private void FullScreenMulti(List<Screen> others)
        {
            string selected = mnsScreenlist_cmb.SelectedItem?.ToString();
            Screen active = others.FirstOrDefault(s => s.DeviceName == selected) ?? others[0];
            // 要產生的螢幕與偏移（依序模式第 n 個螢幕顯示後面第 n 張）
            var renders = new List<(Screen screen, int offset)>();
            switch (_multiMode)
            {
                case MultiScreenMode.SyncOthers:
                    renders.AddRange(others.Select(s => (s, 0)));
                    break;
                case MultiScreenMode.SequenceOthers:
                    renders.AddRange(others.Select((s, i) => (s, i)));
                    break;
                default:  // 分離：只更新作用中螢幕；還沒開的螢幕先顯示同一張
                    renders.AddRange(others.Where(s => s == active || !HasScreenWindow(s.DeviceName)).Select(s => (s, 0)));
                    break;
            }
            // 主視窗預覽要停在「偏移 0、作用中螢幕」的結果，所以它排最後產生
            renders = renders.OrderBy(r => r.offset == 0 ? 1 : 0).ThenBy(r => r.screen == active ? 1 : 0).ToList();
            string width = mnsW_txt.Text, height = mnsH_txt.Text;
            _updatingScreens = true;
            try
            {
                Screen lastScreen = null;
                foreach (var (screen, offset) in renders)
                {
                    using (Bitmap bitmap = RenderForScreen(screen, offset, out string caption))
                    {
                        pictureWindow window = ShowOnScreen(screen, bitmap, caption);
                        if (_multiMode == MultiScreenMode.SeparateOthers) window.Selection = CaptureSelection();
                    }
                    lastScreen = screen;
                }
                mnsW_txt.Text = width;
                mnsH_txt.Text = height;
                if (lastScreen != null && (lastScreen.Bounds.Width.ToString() != width || lastScreen.Bounds.Height.ToString() != height))
                {
                    RegenerateCurrent();  // 選的螢幕解析度不同（例如選主螢幕）：主視窗預覽恢復成原本寬高
                }
                _pictureWindow = _screenWindows.TryGetValue(active.DeviceName, out var activeWindow) ? activeWindow : _pictureWindow;
                _pictureWindow?.Activate();
            }
            finally
            {
                _updatingScreens = false;
            }
            if (_dynamicBackground != null && ReferenceEquals(mypicture.Getpicture(), _dynamicBackground) && !timerDynamic.Enabled)
            {
                timerDynamic.Start();  // 動畫只在作用中螢幕播放
            }
        }  // 多螢幕全螢幕
        private bool HasScreenWindow(string device) => _screenWindows.TryGetValue(device, out var w) && !w.IsDisposed;
        private Bitmap RenderForScreen(Screen screen, int offset, out string caption)
        {
            // 依螢幕解析度產生目前的圖；offset > 0 時改用清單後面第 offset 項（ImgList / DirList）
            mnsW_txt.Text = screen.Bounds.Width.ToString();
            mnsH_txt.Text = screen.Bounds.Height.ToString();
            if (offset > 0 && _shownItem == null)
            {
                if (tabControl.SelectedTab == tabImgList && tabilPatternList_lst.SelectedIndices.Count > 0)
                {
                    var item = tabilPatternList_lst.Items[(tabilPatternList_lst.SelectedIndices[0] + offset) % tabilPatternList_lst.Items.Count];
                    caption = item.SubItems[1].Text;
                    if (ApplyConfigLine(item.Tag.ToString(), new List<string>()) && mypicture.Getpicture() != null)
                    {
                        return new Bitmap(showimgPicture_pic.Image);  // Dynamic 時是含物件的預覽
                    }
                    return ErrorBitmap(screen, $"{caption}：設定有誤");
                }
                if (tabControl.SelectedTab == tabDirList && tabdlPatternList_lvw.SelectedIndices.Count > 0)
                {
                    var item = tabdlPatternList_lvw.Items[(tabdlPatternList_lvw.SelectedIndices[0] + offset) % tabdlPatternList_lvw.Items.Count];
                    caption = item.Text;
                    try
                    {
                        using (var stream = new FileStream(item.Tag.ToString(), FileMode.Open, FileAccess.Read))
                        using (var image = Image.FromStream(stream))
                        {
                            return new Bitmap(image);
                        }
                    }
                    catch (Exception ex)
                    {
                        return ErrorBitmap(screen, $"{caption}：{ex.Message}");
                    }
                }
            }
            RegenerateCurrent();
            caption = CurrentImageName();
            Bitmap current = mypicture.Getpicture();
            return current != null ? new Bitmap(current) : ErrorBitmap(screen, "沒有圖片");
        }  // 產生某個螢幕要顯示的圖
        private static Bitmap ErrorBitmap(Screen screen, string message)
        {
            var bitmap = new Bitmap(screen.Bounds.Width, screen.Bounds.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            using (Font font = new Font("新細明體", 24f))
            {
                g.Clear(Color.Black);
                g.DrawString(message, font, Brushes.White, 20, 20);
            }
            return bitmap;
        }  // 無法產生時顯示的圖
        private pictureWindow ShowOnScreen(Screen screen, Bitmap bitmap, string caption)
        {
            if (!_screenWindows.TryGetValue(screen.DeviceName, out pictureWindow window) || window.IsDisposed)
            {
                window = new pictureWindow { ScreenDevice = screen.DeviceName, StartPosition = FormStartPosition.Manual };
                window.FormClosed += (s, e) => OnScreenWindowClosed((pictureWindow)s);
                window.Activated += (s, e) => OnScreenWindowActivated((pictureWindow)s);
                _screenWindows[screen.DeviceName] = window;
            }
            window.setBitmap(bitmap);
            window.Caption = caption;
            if (!window.Visible)
            {
                window.FormBorderStyle = FormBorderStyle.None;
                window.Location = screen.Bounds.Location;
                window.Size = screen.Bounds.Size;
                window.Show();
                window.WindowState = FormWindowState.Maximized;
            }
            return window;
        }  // 顯示到指定螢幕
        private ScreenSelection CaptureSelection()
        {
            var selection = new ScreenSelection { Tab = tabControl.SelectedTab, Shown = _shownItem };
            if (selection.Tab == tabImgList && tabilPatternList_lst.SelectedIndices.Count > 0) selection.Index = tabilPatternList_lst.SelectedIndices[0];
            if (selection.Tab == tabDirList && tabdlPatternList_lvw.SelectedIndices.Count > 0) selection.Index = tabdlPatternList_lvw.SelectedIndices[0];
            return selection;
        }  // 記錄目前分頁與項目
        private void OnScreenWindowActivated(pictureWindow window)
        {
            // 分離模式：點哪個螢幕就控制哪個，主視窗跳回那個螢幕顯示的圖
            if (_updatingScreens || _closingAllScreens || _multiMode != MultiScreenMode.SeparateOthers) return;
            if (ReferenceEquals(window, _pictureWindow)) return;
            _pictureWindow = window;
            _skipScreenRegenerate = true;
            try
            {
                mnsScreenlist_cmb.SelectedItem = window.ScreenDevice;  // 寬高改成這個螢幕的解析度
            }
            finally
            {
                _skipScreenRegenerate = false;
            }
            if (window.Selection is ScreenSelection selection)
            {
                _restoringSelection = true;
                try
                {
                    tabControl.SelectedTab = selection.Tab;
                    ListView list = selection.Tab == tabImgList ? tabilPatternList_lst : selection.Tab == tabDirList ? tabdlPatternList_lvw : null;
                    if (list != null && selection.Index >= 0 && selection.Index < list.Items.Count)
                    {
                        list.Items[selection.Index].Selected = true;
                        list.Items[selection.Index].Focused = true;
                        list.Items[selection.Index].EnsureVisible();
                    }
                    _shownItem = selection.Shown;
                    RegenerateCurrent();
                }
                finally
                {
                    _restoringSelection = false;
                }
            }
            window.FlashMessage($"{ScreenLabel(window.ScreenDevice)} 作用中");
        }  // 分離模式切換作用中螢幕
        private void OnScreenWindowClosed(pictureWindow window)
        {
            if (window.ScreenDevice != null && _screenWindows.TryGetValue(window.ScreenDevice, out var stored) && ReferenceEquals(stored, window))
            {
                _screenWindows.Remove(window.ScreenDevice);
            }
            if (!_closingAllScreens) CloseAllFullScreen();  // 關掉其中一個（Esc）時全部關閉
        }  // 全螢幕視窗關閉
        private void CloseAllFullScreen()
        {
            _closingAllScreens = true;
            try
            {
                foreach (pictureWindow window in _screenWindows.Values.ToList())
                {
                    if (!window.IsDisposed) window.Close();
                }
                if (_pictureWindow != null && !_pictureWindow.IsDisposed) _pictureWindow.Close();
            }
            finally
            {
                _closingAllScreens = false;
            }
        }  // 關閉所有全螢幕
        private string CurrentImageName()
        {
            // DirList 顯示檔名、ImgList 顯示 config 名稱；ImgEditor 不顯示；最近使用 / 草稿顯示其名稱
            if (_shownItem != null)
            {
                return _shownItem.Name;
            }
            if (tabControl.SelectedTab == tabDirList && tabdlPatternList_lvw.SelectedItems.Count > 0)
            {
                return tabdlPatternList_lvw.SelectedItems[0].Text;
            }
            if (tabControl.SelectedTab == tabImgList && tabilPatternList_lst.SelectedItems.Count > 0)
            {
                return tabilPatternList_lst.SelectedItems[0].SubItems[1].Text;
            }
            return null;
        }  // 目前圖片名稱
        private void Edid_Click(object sender, EventArgs e)
        {
            // 與全螢幕相同，讀取目前選擇的螢幕
            string selectedScreen = mnsScreenlist_cmb.SelectedItem?.ToString() ?? Screen.AllScreens[0].DeviceName;
            new EdidWindow(selectedScreen).Show(this);
        }  // 讀取 EDID
        private void About_Click(object sender, EventArgs e)
        {
            using (AboutWindow about = new AboutWindow())
            {
                about.ShowDialog(this);
            }
        }  // 版本與快捷鍵說明
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
            UpdateRefreshRate();
            if (!_skipScreenRegenerate) RegenerateCurrent();
        }  // 螢幕選擇
        public void SwitchScreen(int step)
        {
            int count = mnsScreenlist_cmb.Items.Count;
            if (count == 0) return;
            // 循環切換，觸發 menuToolStripCmb_SelectedIndexChanged 更新寬高並重新產生圖片
            mnsScreenlist_cmb.SelectedIndex = ((mnsScreenlist_cmb.SelectedIndex + step) % count + count) % count;
        }  // 切換輸出螢幕（step = +1 下一個、-1 上一個）
        public void RegenerateCurrent()
        {
            // 依目前分頁用最新的寬高重新產生圖片
            if (_shownItem != null)
            {
                // 目前顯示的是最近使用 / 編輯區草稿：重新套用同一行 config（圖檔不重新產生）
                if (_shownItem.Line != null) ApplyConfigLine(_shownItem.Line);
                return;
            }
            if (tabControl.SelectedTab == tabImgList)
            {
                if (tabilPatternList_lst.SelectedItems.Count > 0)
                {
                    tabilPatternList_lst_SelectedIndexChanged(tabilPatternList_lst, EventArgs.Empty);
                }
            }
            else if (tabControl.SelectedTab == tabImgEditor)
            {
                Generate_Click(showimgGenerate_btn, EventArgs.Empty);
            }
            // DirList 顯示的是圖檔，不重新產生
        }  // 依分頁重新產生目前圖片
        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabdlPatternList_lvw.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = tabdlPatternList_lvw.SelectedItems[0]; // 獲取選中的 ListViewItem
                string imagePath = selectedItem.Tag as string; // 從 Tag 中取出圖片路徑
                if (LoadImageFile(imagePath))
                {
                    _shownItem = null;
                    AddRecent(RecentItem.FromFile(imagePath));
                    // 如果需要全屏顯示，更新 _pictureWindow
                    if (_pictureWindow != null && !_pictureWindow.IsDisposed && !_restoringSelection)
                    {
                        FullScreen(sender, e);
                    }
                }
            }
        }  // listview圖片選擇
        private bool LoadImageFile(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
            {
                MessageBox.Show($"找不到圖片：{imagePath}");
                return false;
            }
            try
            {
                // 釋放舊圖片
                if (showimgPicture_pic.Image != null)
                {
                    showimgPicture_pic.Image.Dispose();
                    showimgPicture_pic.Image = null;
                }
                // 使用 FileStream 加載圖片
                using (FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                {
                    using (Image originalImage = Image.FromStream(fs))
                    {
                        showimgPicture_pic.Image = mypicture.Setpicture(originalImage, "image");
                        showimgPicture_pic.SizeMode = PictureBoxSizeMode.Zoom;
                        ShowPreviewInfo(originalImage.Width, originalImage.Height, null);  // 圖檔沒有 config
                        ssrStatus_lbl.Text = imagePath;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"無法載入圖片：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }  // 載入圖檔到預覽
        private void statusStrip_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("explorer.exe", $"\"{Application.StartupPath}\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"無法開啟資料夾：{ex.Message}");
            }
        }  // 狀態列點擊開啟程式資料夾
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern int StrCmpLogicalW(string x, string y);  // 與檔案總管相同的自然排序 (1,2,...,9,10)
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
            // 依分組 (JPG→PNG→BMP) 再依檔名自然排序，讓全螢幕左右切換的順序與畫面一致
            string[] groupOrder = { ".jpg", ".png", ".bmp" };
            Array.Sort(imageFiles, (a, b) =>
            {
                int byGroup = Array.IndexOf(groupOrder, Path.GetExtension(a).ToLower())
                    .CompareTo(Array.IndexOf(groupOrder, Path.GetExtension(b).ToLower()));
                return byGroup != 0 ? byGroup : StrCmpLogicalW(Path.GetFileName(a), Path.GetFileName(b));
            });

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
            if (sender is ListView targetList && !_movingConfigLine)
            {
                if (targetList.SelectedItems.Count > 0)
                {
                    FillConfigEditor(targetList.SelectedItems[0]);  // 帶入下方編輯區
                    _configDraftFocused = false;  // 選清單後 Generate 不再預覽編輯區草稿
                    _shownItem = null;
                    string line = targetList.SelectedItems[0].Tag.ToString();
                    if (ApplyConfigLine(line))
                    {
                        AddRecent(RecentItem.FromConfig(line));
                    }
                }
            }
        }  // ImgList選擇圖片
        private bool ApplyConfigLine(string line, List<string> errors = null)
        {
            // 套用一行 config 並產生預覽圖（ImgList 選取、編輯區預覽、最近使用共用）；成功回傳 true
            // errors 不為 null 時錯誤記錄在清單（批次匯出用），否則跳出訊息
            string trimlower(string st)
            {
                return st.Trim().ToLower();
            }
            void Report(string message, string caption = "")
            {
                if (errors != null) errors.Add(message);
                else if (caption == "") MessageBox.Show(message);
                else MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            string[] patternVar = line.Split(',');
            if (patternVar.Length < 4)
            {
                // 選項欄可省略
                int oldLength = patternVar.Length;
                Array.Resize(ref patternVar, 4);
                for (int i = oldLength; i < 4; i++) patternVar[i] = "";
            }
            if (!int.TryParse(mnsW_txt.Text, out int width) || !int.TryParse(mnsH_txt.Text, out int height))
            {
                Report("請輸入有效的寬度與高度！");
                return false;
            }
            try
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
                    case "dynamic":
                        tabimgeFuncList.SelectedIndex = tabimgeFuncList.Items.IndexOf("Dynamic");
                        funcDynamic(trimlower(patternVar[2]), trimlower(patternVar[3]));
                        break;
                    case "text":
                        tabimgeFuncList.SelectedIndex = 0;  // 文字模式屬於 Frame
                        funcText(trimlower(patternVar[2]), patternVar[3].Trim());  // 文字保留大小寫
                        break;
                    default:
                        Report($"「{patternVar[0].Trim()}」的類型「{patternVar[1].Trim()}」不支援！");
                        return false;
                }
                string tag = patternVar[0];
                _generatingFromConfig = true;  // Dynamic 只預覽（空白鍵 / Esc 全螢幕時才播放）；Window 不疊加 ImgEditor 的清單
                try
                {
                    GenerateImage(type == "text" ? "solid" : type, width, height, tag); // 重新生成圖片（Text 由 Frame 產生）
                }
                finally
                {
                    _generatingFromConfig = false;
                }
                if (type == "dynamic")
                {
                    // 預覽圖另外複製一份畫上物件起始位置，不影響動畫用的背景
                    Bitmap preview = new Bitmap(mypicture.Getpicture());
                    using (Graphics g = Graphics.FromImage(preview))
                    {
                        DrawDynamicObject(g, preview.Width, preview.Height);
                    }
                    showimgPicture_pic.Image = preview;
                    showimgPicture_pic.SizeMode = PictureBoxSizeMode.Zoom;
                    ShowPreviewInfo(width, height, line);
                    return true;
                }
            }
            catch (Exception ex)
            {
                // config 格式錯誤時提示，不讓程式當掉
                Report($"「{patternVar[0].Trim()}」的設定有誤：{ex.Message}", "Config 錯誤");
                return false;
            }
            showimgPicture_pic.Image = mypicture.Getpicture();
            showimgPicture_pic.SizeMode = PictureBoxSizeMode.Zoom;
            ShowPreviewInfo(width, height, line);
            return true;
        }  // 套用一行 config
        private static readonly string[] ConfigColumns = { "名稱", "類型", "參數", "選項" };
        private void SetupPatternList()
        {
            // 第一欄為編號（等同 index，不寫入 config），其後為 config 的各欄
            tabilPatternList_lst.Columns.Add("編號", 50, HorizontalAlignment.Right);
            int[] widths = { 160, 90, 210, 190 };
            for (int i = 0; i < ConfigColumns.Length; i++)
            {
                tabilPatternList_lst.Columns.Add(ConfigColumns[i], widths[i]);
            }
        }  // ImgList 欄名
        private void RenumberPatternList()
        {
            for (int i = 0; i < tabilPatternList_lst.Items.Count; i++)
            {
                tabilPatternList_lst.Items[i].Text = (i + 1).ToString();
            }
        }  // 重新編號
        private static string ReadConfigText(string path)
        {
            // 匯出的 config 為 UTF-8，舊版 config.txt 為 Big5：先用 UTF-8 嚴格解碼，失敗再改用 Big5
            byte[] bytes = File.ReadAllBytes(path);
            try
            {
                return new UTF8Encoding(false, true).GetString(bytes).TrimStart((char)0xFEFF);  // 去掉 UTF-8 BOM
            }
            catch (DecoderFallbackException)
            {
                return Encoding.GetEncoding(950).GetString(bytes);
            }
        }  // 讀取 config（自動判斷編碼）
        private void LoadConfig(string configText, string source)
        {
            PopulateListBox(configText);
            tabilPatternList_lst.Tag = source;  // 記錄 config 來源
            if (cfgFile_cmb != null) RefreshConfigFiles();  // 檔案下拉選單顯示目前來源
        }  // 載入 config
        private void LoadDefaultConfig()
        {
            // 啟動時優先讀取程式資料夾的 config.txt，沒有則用內建設定
            string configFilePath = Path.Combine(Application.StartupPath, "config.txt");
            if (File.Exists(configFilePath))
            {
                try
                {
                    LoadConfig(ReadConfigText(configFilePath), configFilePath);
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"讀取 config.txt 失敗，改用內建設定：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            LoadConfig(Resources.config, "內建設定");
        }  // 載入預設 config
        private void PopulateListBox(string configfile)
        {
            tabilPatternList_lst.BeginUpdate();
            tabilPatternList_lst.Items.Clear();
            foreach (string rawLine in configfile.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries))
            {
                ListViewItem item = CreateConfigItem(rawLine);
                if (item != null)
                {
                    tabilPatternList_lst.Items.Add(item);
                }
            }
            RenumberPatternList();
            tabilPatternList_lst.EndUpdate();
        }  // ImgList 載入 config
        private static ListViewItem CreateConfigItem(string rawLine)
        {
            string line = rawLine.TrimEnd();
            if (line.Trim().Length == 0 || line.TrimStart().StartsWith("#")) return null;  // 空行與註解
            string[] fields = line.Split(',');
            if (fields[0].Trim() == ConfigColumns[0]) return null;  // 欄名列
            ListViewItem item = new ListViewItem("") { Tag = line };  // 第一欄編號由 RenumberPatternList 填入；Tag 保留原始內容，匯出時原樣寫回
            item.SubItems.Add(fields[0].Trim());
            for (int i = 1; i < ConfigColumns.Length; i++)
            {
                // 最後一欄收納剩下的所有欄位
                string value = i < ConfigColumns.Length - 1
                    ? (fields.Length > i ? fields[i] : "")
                    : string.Join(",", fields.Skip(i));
                item.SubItems.Add(value.Trim().Trim(',').Trim());
            }
            return item;
        }  // 一行 config 轉成 ImgList 項目（空行、註解、欄名列回傳 null）
        public void SelectImgListRelative(int step)
        {
            int count = tabilPatternList_lst.Items.Count;
            if (count == 0) return;
            int current = tabilPatternList_lst.SelectedIndices.Count > 0 ? tabilPatternList_lst.SelectedIndices[0] : (step > 0 ? -1 : 0);
            int next = ((current + step) % count + count) % count;
            tabilPatternList_lst.Items[next].Selected = true;
            tabilPatternList_lst.Items[next].Focused = true;
            tabilPatternList_lst.EnsureVisible(next);
        }  // ImgList 上一個 / 下一個（全螢幕左右鍵使用）
        // ImgEditor 設定加入 ImgList------------------------------------------------------------------------------
        private Button tabimgeAddToList_btn;
        private void InitEditorToConfig()
        {
            // 放在 ImgEditor 右上角：版面加一欄，功能下拉選單同一列的右側；ImageAdjust 時隱藏
            tabimgeAddToList_btn = new Button { Text = "新增至 ImgList", AutoSize = true, Anchor = AnchorStyles.Top | AnchorStyles.Right, Margin = new Padding(3, 0, 3, 0) };
            tabimgeAddToList_btn.Click += (s, e) => AddEditorSettingToList();
            toolTip.SetToolTip(tabimgeAddToList_btn, "把目前頁面的設定轉成一行 config，加到 ImgList 最後一列");
            tabimgeTableLayoutPanel.ColumnCount = 2;
            tabimgeTableLayoutPanel.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 100);  // 第一欄填滿，按鈕靠右
            tabimgeTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tabimgeTableLayoutPanel.Controls.Add(tabimgeAddToList_btn, 1, 0);
            tabimgeTableLayoutPanel.SetColumnSpan(tabimgePanel, 2);
            tabimgeFuncList.SelectedIndexChanged += (s, e) => tabimgeAddToList_btn.Visible = tabimgeFuncList.Text != "ImageAdjust";

            // Window 頁：背景與方塊一鍵設成同一純色
            var presets = new (string text, Color color, Color fore)[]
            {
                ("黑", Color.Black, Color.White), ("白", Color.White, Color.Black), ("灰", Color.FromArgb(127, 127, 127), Color.White),
            };
            for (int i = 0; i < presets.Length; i++)
            {
                var (text, color, fore) = presets[i];
                var button = new Button { Text = text, BackColor = color, ForeColor = fore, FlatStyle = FlatStyle.Flat, Location = new Point(167, 90 + i * 30), Size = new Size(46, 26) };
                button.Click += (s, e) => SetWindowSolidColor(color);
                toolTip.SetToolTip(button, $"背景與方塊都設成{text}色（純色）");
                tabimgeWindow.Controls.Add(button);
            }
        }  // 建立「新增至 ImgList」與 Window 顏色按鈕
        private void InitMaskQuickButtons()
        {
            // ImgEditor Mask：Pixel 選色在格子下方一排，Sub Pixel 亮度在捲軸右側
            AddMaskQuickButtons(tabimgeMask, tabiemPixelColor_btn, tabiemPixelGray_vsc, tabiemPixel_rdo, tabiemSubPixel_rdo,
                i => new Rectangle(10 + i * 35, 322, 34, 24), i => new Rectangle(266, 70 + i * 49, 40, 24));
            // Window 自製圖 Mask：空間較小，兩種按鈕都放在捲軸右側一欄（同時只顯示一種）
            AddMaskQuickButtons(tabiwCustomMask_grp, tabiwCMaskPixelColor_btn, tabiwCMaskPixelGray_vsc, tabiwCMaskPixel_rdo, tabiwCMaskSubPixel_rdo,
                i => new Rectangle(246, 58 + i * 28, 48, 24), i => new Rectangle(246, 58 + i * 44, 48, 24));
        }  // Mask 快速選色與亮度按鈕
        private void AddMaskQuickButtons(Control box, Button colorButton, VScrollBar grayScroll, RadioButton pixelRadio, RadioButton subPixelRadio,
            Func<int, Rectangle> pixelBounds, Func<int, Rectangle> subPixelBounds)
        {
            // Pixel：快速選色（設定「選擇顏色」，之後點格子即填入）
            var colors = new (string text, Color color)[]
            {
                ("黑", Color.Black), ("白", Color.White), ("紅", Color.Red), ("綠", Color.Lime), ("藍", Color.Blue),
                ("64", Color.FromArgb(64, 64, 64)), ("127", Color.FromArgb(127, 127, 127)),
            };
            var pixelButtons = new List<Button>();
            for (int i = 0; i < colors.Length; i++)
            {
                var (text, color) = colors[i];
                bool dark = color.R * 0.299 + color.G * 0.587 + color.B * 0.114 < 128;  // 依亮度決定字色
                var button = new Button
                {
                    Text = text, BackColor = color, ForeColor = dark ? Color.White : Color.Black, FlatStyle = FlatStyle.Flat,
                    Bounds = pixelBounds(i), Margin = Padding.Empty,
                };
                button.Click += (s, e) => colorButton.BackColor = color;
                toolTip.SetToolTip(button, $"選擇顏色設為{(text.All(char.IsDigit) ? text + " 灰" : text)}（{color.R},{color.G},{color.B}）");
                pixelButtons.Add(button);
            }
            // Sub Pixel：快速設定亮度（捲軸值 = 255 - 亮度）
            int[] levels = { 255, 192, 127, 64, 0 };
            var subPixelButtons = new List<Button>();
            for (int i = 0; i < levels.Length; i++)
            {
                int level = levels[i];
                var button = new Button { Text = level.ToString(), Bounds = subPixelBounds(i) };
                button.Click += (s, e) => grayScroll.Value = 255 - level;
                toolTip.SetToolTip(button, $"子像素亮度設為 {level}");
                subPixelButtons.Add(button);
            }
            box.Controls.AddRange(pixelButtons.Concat(subPixelButtons).ToArray<Control>());
            void UpdateVisible()
            {
                bool subPixel = subPixelRadio.Checked;
                pixelButtons.ForEach(b => b.Visible = !subPixel);
                subPixelButtons.ForEach(b => b.Visible = subPixel);
            }
            pixelRadio.CheckedChanged += (s, e) => UpdateVisible();
            subPixelRadio.CheckedChanged += (s, e) => UpdateVisible();
            UpdateVisible();
        }  // 加入 Mask 快速按鈕
        private void InitGradientReverse()
        {
            // 反向是讓奇數等分反轉漸層方向，畫面等分 < 2 時沒有作用，停用並取消勾選
            void Update()
            {
                bool usable = int.TryParse(tabigDivid_cmb.Text, out int divisions) && divisions >= 2;
                tabigReverse_chk.Enabled = usable;
                if (!usable) tabigReverse_chk.Checked = false;
            }
            tabigDivid_cmb.TextChanged += (s, e) => Update();
            toolTip.SetToolTip(tabigReverse_chk, "奇數等分反轉漸層方向（畫面幾等分 ≥ 2 才能使用）");
            Update();
        }  // 漸層反向的啟用條件
        private void InitWindowTabs()
        {
            // 自製圖與窗口 / 線條清單合併成分頁，放在原自製圖的位置
            var tabs = new TabControl { Location = new Point(tabiwCustom_grp.Left, 8), Size = new Size(tabiwCustom_grp.Width + 5, tabiwCustom_grp.Bottom - 8 + 14) };
            var customPage = new TabPage("自製圖") { UseVisualStyleBackColor = true };
            var overlayPage = new TabPage("窗口 / 線條") { UseVisualStyleBackColor = true };
            tabs.TabPages.AddRange(new[] { customPage, overlayPage });
            tabimgeWindow.Controls.Add(tabs);
            tabimgeWindow.Controls.Remove(tabiwCustom_grp);
            tabiwCustom_grp.Dock = DockStyle.Fill;
            customPage.Controls.Add(tabiwCustom_grp);
            // 窗口 / 線條清單與按鈕移入第二頁
            var overlayButtons = tabimgeWindow.Controls.OfType<Button>().Where(b => b.Text == "新增" || b.Text == "修改" || b.Text == "移除").OrderBy(b => b.Left).ToList();
            // 用 Dock 排版：清單填滿，按鈕列在底部（不依賴分頁當下的尺寸）
            overlayPage.Padding = new Padding(6);
            tabimgeWindow.Controls.Remove(tabiwOverlay_lvw);
            tabiwOverlay_lvw.Dock = DockStyle.Fill;
            tabiwOverlay_lvw.Columns[0].Width = 70;
            tabiwOverlay_lvw.Columns[1].Width = 280;
            var buttonBar = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 34, Padding = new Padding(0, 4, 0, 0) };
            foreach (Button button in overlayButtons)
            {
                tabimgeWindow.Controls.Remove(button);
                button.Size = new Size(70, 26);
                buttonBar.Controls.Add(button);
            }
            overlayPage.Controls.Add(tabiwOverlay_lvw);
            overlayPage.Controls.Add(buttonBar);  // 後加入的先 Dock，按鈕列先佔底部，清單填滿其餘
        }  // Window 自製圖與窗口 / 線條分頁
        private ComboBox tabiwDiagonal_cmb;
        private void InitWindowDiagonal()
        {
            // 斜角改為下拉選單；原勾選框隱藏保留，作為「有斜角」的狀態（沿用其停用視窗位置等邏輯）
            tabiwDiagonal_btn.Visible = false;
            var label = new Label { Text = "斜角", AutoSize = true, Location = new Point(10, 312) };
            tabiwDiagonal_cmb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(10, 327), Width = 58, ImeMode = ImeMode.Disable };
            tabiwDiagonal_cmb.Items.AddRange(new object[] { "無", "左上", "右上", "右下", "左下" });
            tabiwDiagonal_cmb.SelectedIndexChanged += (s, e) => tabiwDiagonal_btn.Checked = tabiwDiagonal_cmb.SelectedIndex > 0;
            tabiwDiagonal_cmb.SelectedIndex = tabiwDiagonal_btn.Checked ? 2 : 0;
            toolTip.SetToolTip(tabiwDiagonal_cmb, "以對角線切開畫面，在選擇的角落畫出方塊顏色的三角形");
            tabimgeWindow.Controls.AddRange(new Control[] { label, tabiwDiagonal_cmb });
            // 騰出空間：線顏色按鈕移到標籤右側（窗口 / 線條清單已移到右側分頁）
            tabiwLineColor_lbl.Location = new Point(10, 357);
            tabiwLineColor_btn.Location = new Point(54, 352);
        }  // 建立斜角下拉選單
        private CheckBox tabiefRedBlue_chk;
        private void InitFrameRedBlue()
        {
            // 「其他(H&V)」右側
            tabiefRedBlue_chk = new CheckBox { Text = "左紅右藍", AutoSize = true, Location = new Point(tabiefOther_chk.Right + 8, tabiefOther_chk.Top) };
            toolTip.SetToolTip(tabiefRedBlue_chk, "畫面四邊：上下白線、左紅線、右藍線（固定 1 pixel）");
            tabimgeFrame.Controls.Add(tabiefRedBlue_chk);
            tabiefRedBlue_chk.BringToFront();
        }  // 建立左紅右藍
        private CheckBox tabiefText_chk;
        private TextBox tabiefText_txt;
        private NumericUpDown tabiefTextSize_nud, tabiefTextCharGap_nud, tabiefTextLineGap_nud, tabiefTextStringGap_nud;
        private Button tabiefTextColor_btn;
        private Control[] _frameTextSettings;  // 未勾選文字模式時停用的設定
        private void InitFrameTextMode()
        {
            // Frame 頁：框線顏色 / 粗細右側
            const int x = 340;
            Label Caption(string text) => new Label { Text = text, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(0, 3, 3, 3) };
            NumericUpDown Nud(decimal min, decimal max, int decimals) => new NumericUpDown
            {
                Width = 60, Minimum = min, Maximum = max, DecimalPlaces = decimals, Increment = decimals > 0 ? 0.5m : 1m,
                ImeMode = ImeMode.Disable, Anchor = AnchorStyles.Left, Margin = new Padding(0, 2, 6, 2),
            };
            tabiefText_chk = new CheckBox { Text = "文字模式", AutoSize = true, Location = new Point(x, 57) };
            tabiefText_txt = new TextBox { MaxLength = 10, Width = 150, Location = new Point(x, 80), ImeMode = ImeMode.NoControl };  // 最多 10 個字，可打中文
            tabiefTextSize_nud = Nud(1, 500, 0);            // 大小 1~500 px
            tabiefTextCharGap_nud = Nud(0, 500, 1);         // 間距 0~500 px（以筆畫邊緣計算，0 = 貼齊）
            tabiefTextLineGap_nud = Nud(0, 500, 1);
            tabiefTextStringGap_nud = Nud(0, 500, 1);
            tabiefTextColor_btn = new Button { Size = new Size(24, 24), BackColor = Color.White, FlatStyle = FlatStyle.Flat, Anchor = AnchorStyles.Left, Margin = new Padding(0, 2, 0, 2) };
            var labels = new[] { Caption("大小"), Caption("顏色"), Caption("字距"), Caption("行距"), Caption("串距") };
            // 用表格自動排版，不同縮放比例下標籤也不會被擋住
            var grid = new TableLayoutPanel { Location = new Point(x, 105), AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, ColumnCount = 4, RowCount = 4, Padding = Padding.Empty, Margin = Padding.Empty };
            grid.Controls.Add(labels[0], 0, 0); grid.Controls.Add(tabiefTextSize_nud, 1, 0); grid.Controls.Add(labels[1], 2, 0); grid.Controls.Add(tabiefTextColor_btn, 3, 0);
            grid.Controls.Add(labels[2], 0, 1); grid.Controls.Add(tabiefTextCharGap_nud, 1, 1);
            grid.Controls.Add(labels[3], 0, 2); grid.Controls.Add(tabiefTextLineGap_nud, 1, 2);
            grid.Controls.Add(labels[4], 0, 3); grid.Controls.Add(tabiefTextStringGap_nud, 1, 3);
            tabiefTextSize_nud.Value = 12;
            ResetFrameTextGaps();
            // 改大小時間距依比例重設為預設值（之後仍可個別調整）
            tabiefTextSize_nud.ValueChanged += (s, e) => ResetFrameTextGaps();
            tabiefTextColor_btn.Click += (s, e) =>
            {
                using (ColorDialog dialog = new ColorDialog { Color = tabiefTextColor_btn.BackColor, FullOpen = true })
                {
                    if (dialog.ShowDialog(this) == DialogResult.OK) tabiefTextColor_btn.BackColor = dialog.Color;
                }
            };
            _frameTextSettings = new Control[] {tabiefText_txt, tabiefTextSize_nud, tabiefTextColor_btn, tabiefTextCharGap_nud, tabiefTextLineGap_nud, tabiefTextStringGap_nud }.Concat(labels).ToArray();
            tabiefText_chk.CheckedChanged += (s, e) => UpdateFrameTextEnabled();
            UpdateFrameTextEnabled();
            toolTip.SetToolTip(tabiefText_chk, "勾選後以文字依座標重複鋪滿整個畫面");
            toolTip.SetToolTip(tabiefText_txt, "最多 10 個字（不能有逗號 , 與斜線 /）");
            toolTip.SetToolTip(tabiefTextSize_nud, "字的大小（像素），改變時間距會依比例重設");
            toolTip.SetToolTip(tabiefTextCharGap_nud, "字與字筆畫之間的空白，0 = 貼齊（預設 大小/5）");
            toolTip.SetToolTip(tabiefTextLineGap_nud, "上下兩行筆畫之間的空白，0 = 貼齊（預設 大小/3）");
            toolTip.SetToolTip(tabiefTextStringGap_nud, "重複字串之間的空白，0 = 貼齊（預設 大小/2）");
            tabimgeFrame.Controls.AddRange(new Control[] { tabiefText_chk, tabiefText_txt, grid });
            foreach (Control control in new Control[] { tabiefText_chk, tabiefText_txt, grid }) control.BringToFront();  // 避免被其他控制項蓋住
            // 逗號是 config 欄位分隔、/ 是 Text 選項分隔，文字裡不能有
            tabiefText_txt.KeyPress += (s, e) => { if (e.KeyChar == ',' || e.KeyChar == '/') e.Handled = true; };
            tabiefText_txt.TextChanged += (s, e) =>
            {
                string clean = tabiefText_txt.Text.Replace(",", "").Replace("/", "");
                if (clean != tabiefText_txt.Text) tabiefText_txt.Text = clean;  // 貼上時移除
            };
        }  // 建立 Frame 文字模式
        private void ResetFrameTextGaps()
        {
            decimal size = tabiefTextSize_nud.Value;
            tabiefTextCharGap_nud.Value = Math.Round(size / 5, 1);
            tabiefTextLineGap_nud.Value = Math.Round(size / 3, 1);
            tabiefTextStringGap_nud.Value = Math.Round(size / 2, 1);
        }  // 間距預設值
        private void UpdateFrameTextEnabled()
        {
            foreach (Control control in _frameTextSettings) control.Enabled = tabiefText_chk.Checked;
        }  // 未勾選時停用設定
        private static void DrawTiledText(Graphics g, string text, Font font, Color color, float charGap, float lineGap, float stringGap, int width, int height)
        {
            // 間距以「筆畫實際範圍」計算：字型本身左右留白與行高預留都不算，間距 0 即筆畫貼齊
            // 不做反鋸齒，畫面只有背景色與文字色
            var textHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            using (Brush brush = new SolidBrush(color))
            using (StringFormat format = (StringFormat)StringFormat.GenericTypographic.Clone())
            {
                format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;  // 空白字元也要算寬度
                g.TextRenderingHint = textHint;
                // 量每個字的筆畫範圍（相對於繪製原點）；空白字元沒有筆畫，用字寬
                int pad = (int)Math.Ceiling(font.GetHeight(g));
                var ink = new Rectangle[text.Length];
                var advance = new float[text.Length];
                for (int i = 0; i < text.Length; i++)
                {
                    string ch = text[i].ToString();
                    advance[i] = g.MeasureString(ch, font, PointF.Empty, format).Width;
                    using (Bitmap probe = new Bitmap((int)Math.Ceiling(advance[i]) + pad * 2, pad * 3))
                    using (Graphics pg = Graphics.FromImage(probe))
                    {
                        pg.Clear(Color.White);
                        pg.TextRenderingHint = textHint;
                        pg.DrawString(ch, font, Brushes.Black, pad, pad, format);
                        ink[i] = InkBounds(probe, pad);
                    }
                }
                var drawn = ink.Where(r => !r.IsEmpty).ToList();
                if (drawn.Count == 0) return;  // 只有空白
                int inkTop = drawn.Min(r => r.Top), inkBottom = drawn.Max(r => r.Bottom);
                float stepY = Math.Max(1f, inkBottom - inkTop + lineGap);  // 行與行：筆畫高度 + 行距
                // 一個字串內各字的 X 位置（以筆畫左緣對齊游標）
                var charX = new float[text.Length];
                float cursor = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    if (ink[i].IsEmpty) { charX[i] = cursor; cursor += advance[i] + charGap; continue; }
                    charX[i] = cursor - ink[i].Left;
                    cursor += ink[i].Width + charGap;
                }
                float stepX = Math.Max(1f, cursor - charGap + stringGap);  // 字串與字串：最後一字筆畫右緣 + 串距
                for (float y = 0; y < height; y += stepY)
                {
                    for (float x = 0; x < width; x += stepX)
                    {
                        for (int i = 0; i < text.Length; i++)
                        {
                            if (ink[i].IsEmpty) continue;
                            // 取整數座標，筆畫點陣與量測時一致
                            g.DrawString(text[i].ToString(), font, brush, (float)Math.Round(x + charX[i]), (float)Math.Round(y - inkTop), format);
                        }
                    }
                }
            }
        }  // 文字鋪滿畫面
        private static Rectangle InkBounds(Bitmap probe, int origin)
        {
            // 找出非白色像素的範圍，回傳相對於繪製原點 (origin, origin) 的矩形；沒有筆畫回傳 Empty
            int left = int.MaxValue, top = int.MaxValue, right = -1, bottom = -1;
            for (int y = 0; y < probe.Height; y++)
            {
                for (int x = 0; x < probe.Width; x++)
                {
                    if (probe.GetPixel(x, y).R < 128)
                    {
                        if (x < left) left = x;
                        if (x > right) right = x;
                        if (y < top) top = y;
                        if (y > bottom) bottom = y;
                    }
                }
            }
            return right < 0 ? Rectangle.Empty : Rectangle.FromLTRB(left - origin, top - origin, right - origin + 1, bottom - origin + 1);
        }  // 量測筆畫範圍
        private void SetWindowSolidColor(Color color)
        {
            tabiwBackColor_rdo.Checked = true;
            tabiwBack_pic.Image = null;
            tabiwBack_pic.BackColor = color;
            tabiwWinColor_rdo.Checked = true;
            tabiwWin_pic.Image = null;
            tabiwWin_pic.BackColor = color;
        }  // Window 背景與方塊設成同一純色
        private static string ConfigColorText(Color c)
        {
            // 一般顏色名稱直接寫名稱，其餘寫 R.G.B（系統色如 Control 也寫成 R.G.B）
            return c.IsKnownColor && !c.IsSystemColor ? c.Name : $"{c.R}.{c.G}.{c.B}";
        }  // 顏色轉 config 寫法
        private void AddEditorSettingToList()
        {
            var ignored = new List<string>();
            string line = EditorToConfigLine(ignored, out string error);
            if (line == null && error == null) return;  // ImageAdjust 等不支援的功能
            if (error != null)
            {
                MessageBox.Show($"無法加入 ImgList：{error}", "新增至 ImgList", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (ignored.Count > 0 &&
                MessageBox.Show("以下設定 config 無法描述，加入後會被忽略：\r\n\r\n・" + string.Join("\r\n・", ignored) + "\r\n\r\n仍要加入嗎？",
                    "新增至 ImgList", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }
            ListViewItem item = CreateConfigItem(line);
            tabilPatternList_lst.Items.Add(item);
            RenumberPatternList();
            item.EnsureVisible();
            ssrStatus_lbl.Text = $"已加入 ImgList 第 {item.Index + 1} 列：{item.SubItems[1].Text}";
        }  // 目前 ImgEditor 設定加入 ImgList
        private string EditorToConfigLine(List<string> ignored, out string error)
        {
            // 目前 ImgEditor 設定轉成一行 config（名稱為預設名稱）；不支援的功能回傳 null 且 error 為 null
            string type, param, option;
            switch (tabimgeFuncList.Text)
            {
                case "Frame&CrossLine&Border":
                    // 勾選文字模式時存成 Text 類型
                    if (tabiefText_chk.Checked) { type = "Text"; error = TextToConfig(ignored, out param, out option); }
                    else { type = "Solid"; error = FrameToConfig(ignored, out param, out option); }
                    break;
                case "Gradient&ColorBar": type = "Gradient"; error = GradientToConfig(ignored, out param, out option); break;
                case "Chess": type = "Chess"; error = ChessToConfig(out param, out option); break;
                case "Window": type = "Window"; error = WindowToConfig(ignored, out param, out option); break;
                case "Mask(FlickerPattern)": type = "Mask"; error = MaskToConfig(out param, out option); break;
                case "Dynamic": type = "Dynamic"; error = DynamicToConfig(out param, out option); break;
                default: error = null; return null;
            }
            if (error != null) return null;
            string name = $"{type} {param}{(option != "" ? " " + option : "")}";  // 預設名稱，可在 ImgList 編輯區修改
            return string.Join(",", name, type, param, option);
        }  // ImgEditor 設定轉 config
        private string FrameToConfig(List<string> ignored, out string param, out string option)
        {
            param = option = "";
            if (mypicture.tag == "image") return "背景使用匯入的截圖，config 只能描述純色背景";
            if (tabiefOther_chk.Checked) ignored.Add("其他(H&V) 線條");
            if (tabiefLineSize_nud.Value != 1) ignored.Add($"框線粗細 {tabiefLineSize_nud.Value}（config 固定為 1）");
            param = ConfigColorText(tabiefBack_btn.BackColor);
            string lines = (tabiefCenter_chk.Checked ? "c" : "") + (tabiefOutside_chk.Checked ? "o" : "") + (tabiefNine_chk.Checked ? "n" : "");
            if (lines != "") option = $"{lines}{(tabiefRedBlue_chk.Checked ? "r" : "")}-{ConfigColorText(tabiefLineColor_btn.BackColor)}";
            else if (tabiefRedBlue_chk.Checked) option = "r";  // 左紅右藍不需要線色
            return null;
        }  // Frame 轉 config
        private string TextToConfig(List<string> ignored, out string param, out string option)
        {
            param = option = "";
            if (mypicture.tag == "image") return "背景使用匯入的截圖，config 只能描述純色背景";
            string text = tabiefText_txt.Text.Trim();
            if (text == "") return "文字模式沒有輸入文字";
            if (tabiefCenter_chk.Checked || tabiefOutside_chk.Checked || tabiefNine_chk.Checked || tabiefRedBlue_chk.Checked) ignored.Add("中心對位 / 外框線 / 九點對位 / 左紅右藍");
            if (tabiefOther_chk.Checked) ignored.Add("其他(H&V) 線條");
            string Num(NumericUpDown nud) => nud.Value.ToString("0.#", System.Globalization.CultureInfo.InvariantCulture);
            param = $"{ConfigColorText(tabiefBack_btn.BackColor)}-{ConfigColorText(tabiefTextColor_btn.BackColor)}";
            option = $"{text}/{Num(tabiefTextSize_nud)}/{Num(tabiefTextCharGap_nud)}/{Num(tabiefTextLineGap_nud)}/{Num(tabiefTextStringGap_nud)}";
            return null;
        }  // Frame 文字模式轉 config
        private string GradientToConfig(List<string> ignored, out string param, out string option)
        {
            param = option = "";
            if (tabigOther_chk.Checked)
            {
                // 自訂 ColorBar：colorbar-方向-數量.色1.色2…（依 Tag 的序號排列）
                var panels = tabigOtherSplit_pnl.Controls.OfType<Panel>()
                    .Select(p => new { Panel = p, Tag = p.Tag?.ToString().Split(',') })
                    .Where(p => p.Tag != null && p.Tag.Length == 2)
                    .OrderBy(p => int.Parse(p.Tag[1]))
                    .ToList();
                if (panels.Count == 0) return "ColorBar 沒有任何色塊";
                if (panels.Any(p => p.Panel.BackColor == Color.Transparent)) return "ColorBar 有透明（未設定）的色塊";
                string direction = panels[0].Tag[0] == "H" ? "h" : "v";
                param = $"colorbar-{direction}-{panels.Count}." + string.Join(".", panels.Select(p => ConfigColorText(p.Panel.BackColor)));
            }
            else
            {
                param = ConfigColorText(tabigBaseColor_lbl.BackColor);
            }
            if (!tabigColorBar_rdo.Checked)
            {
                option = $"{(tabigReverse_chk.Checked && tabigReverse_chk.Enabled ? "r-" : "")}{(tabigHWay_rdo.Checked ? "h" : "v")}-{tabigStep_cmb.Text}-{tabigDivid_cmb.Text}";
                // 起始.結束灰階不是預設 0.255 時才寫（例：255.0 由亮到暗）
                if (tabigFirstLevel_cmb.Text.Trim() != "0" || tabigLastLevel_cmb.Text.Trim() != "255")
                {
                    option += $"-{tabigFirstLevel_cmb.Text.Trim()}.{tabigLastLevel_cmb.Text.Trim()}";
                }
            }
            return null;
        }  // Gradient 轉 config
        private string ChessToConfig(out string param, out string option)
        {
            // 實際格子顏色 = 顏色與灰階捲軸取較小值
            Color baseColor = tabiecGray_lbl.BackColor;
            int intensity = tabiecGray_hsc.Value;
            Color color = Color.FromArgb(Math.Min(baseColor.R, intensity), Math.Min(baseColor.G, intensity), Math.Min(baseColor.B, intensity));
            param = $"{tabiecHNum_nud.Value}*{tabiecVNum_nud.Value}";
            string flip = (tabiecHFlip_chk.Checked ? "h" : "") + (tabiecVFlip_chk.Checked ? "v" : "");
            option = color.ToArgb() == Color.White.ToArgb() ? flip : ConfigColorText(color) + (flip != "" ? "-" + flip : "");
            return null;
        }  // Chess 轉 config
        private string WindowToConfig(List<string> ignored, out string param, out string option)
        {
            param = option = "";
            if (!tabiwBackColor_rdo.Checked) return "背景使用截圖或自製圖，config 只能描述純色背景與方塊（自製圖請改用 Window 的 Custom / excel 寫法手動輸入）";
            if (!tabiwWinColor_rdo.Checked) return "方塊使用截圖或自製圖，config 只能描述純色背景與方塊";
            if (!tabiwWinSizePercent_rdo.Checked) return "視窗大小使用指定 Pixel，config 只支援百分比";
            if (tabiwLineOutside_btn.Checked) ignored.Add("外框");
            if (tabiwLineCross_btn.Checked) ignored.Add("十字");
            if (tabiwDiagonal_btn.Checked) ignored.Add($"斜角（{tabiwDiagonal_cmb.Text}）");
            if (tabiwOverlay_lvw.Items.Count > 0) ignored.Add($"右側疊加清單的 {tabiwOverlay_lvw.Items.Count} 個窗口 / 線條");
            param = $"{ConfigColorText(tabiwBack_pic.BackColor)}-{ConfigColorText(tabiwWin_pic.BackColor)}";
            string location = tabiwWinLocPixcel_rdo.Checked ? $"xy_{tabiwWinLocPixcelX_nud.Value}.{tabiwWinLocPixcelY_nud.Value}"
                : tabiwWinLocTwo_rdo.Checked ? "two" : "c";
            option = $"{tabiwWinSizePercent_nud.Value}%{location}";
            return null;
        }  // Window 轉 config
        private string MaskToConfig(out string param, out string option)
        {
            param = option = "";
            int w = (int)tabiemWNum_nud.Value, h = (int)tabiemHNum_nud.Value;
            bool subPixel = tabiemSubPixel_rdo.Checked;
            int columns = subPixel ? w * 3 : w;
            var colors = new Color[columns * h];
            for (int i = 0; i < colors.Length; i++) colors[i] = Color.Black;
            foreach (Panel panel in tabiemPixelPanel_pnl.Controls.OfType<Panel>())
            {
                string[] xy = panel.Tag?.ToString().Split(',');
                if (xy == null || xy.Length != 2 || !int.TryParse(xy[0], out int x) || !int.TryParse(xy[1], out int y) || x >= columns || y >= h) continue;
                colors[y * columns + x] = panel.BackColor;
            }
            param = subPixel ? $"s-{w}*{h}-{255 - tabiemPixelGray_vsc.Value}" : $"p-{w}*{h}";
            if (subPixel)
            {
                // 子像素：非黑即亮
                option = string.Join(".", Enumerable.Range(0, h).Select(row =>
                    new string(colors.Skip(row * columns).Take(columns).Select(c => c.ToArgb() == Color.Black.ToArgb() ? '0' : '1').ToArray())));
                return null;
            }
            char Code(Color c) => c.ToArgb() == Color.Black.ToArgb() ? '0' : c.ToArgb() == Color.White.ToArgb() ? '1' : c.ToArgb() == Color.Gray.ToArgb() ? '2' : '?';
            if (colors.All(c => Code(c) != '?'))
            {
                // 只有黑、白、灰時用精簡寫法，例：10.01
                option = string.Join(".", Enumerable.Range(0, h).Select(row => new string(colors.Skip(row * w).Take(w).Select(Code).ToArray())));
            }
            else
            {
                // 其他顏色用顏色寫法，例：red lime/blue 127.127.127
                option = string.Join("/", Enumerable.Range(0, h).Select(row => string.Join(" ", colors.Skip(row * w).Take(w).Select(ConfigColorText))));
                if (!IsColorMaskOption(option)) option += "/";  // 只有 1 格且為 R.G.B 時補 /，避免被當成精簡寫法
            }
            return null;
        }  // Mask 轉 config
        private string DynamicToConfig(out string param, out string option)
        {
            param = option = "";
            if (mypicture.tag == "image") return "背景使用匯入的截圖，config 只能描述純色背景";
            param = $"{ConfigColorText(tabiedBackColor_btn.BackColor)}-{ConfigColorText(tabiedObjectColor_btn.BackColor)}";
            string shape = tabiedObjectCube_rdo.Checked ? "cube" : tabiedObjectStrip_rdo.Checked ? "strip" : "ball";
            string direction = tabiedMoveH_rdo.Checked ? "h" : tabiedMoveV_rdo.Checked ? "v" : "b";
            option = $"{shape}-{direction}-size{tabiedObjectSize_nud.Value}-speed{tabiedMoveSpeed_nud.Value}";
            return null;
        }  // Dynamic 轉 config
        // Window 疊加清單--------------------------------------------------------------------------------------------
        private ListView tabiwOverlay_lvw;
        private void InitWindowOverlayList()
        {
            // 放在線條選項（外框、十字、斜角、線顏色）右側
            tabiwOverlay_lvw = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                HideSelection = false,
                MultiSelect = false,
                HeaderStyle = ColumnHeaderStyle.Nonclickable,
                Location = new Point(68, 268),
                Size = new Size(147, 86),
                ImeMode = ImeMode.Disable,
                ShowItemToolTips = true,
            };
            tabiwOverlay_lvw.Columns.Add("類型", 44);
            tabiwOverlay_lvw.Columns.Add("設定", 99);  // 剛好填滿清單寬度，完整內容用 tooltip 顯示
            var add = new Button { Text = "新增", Location = new Point(68, 357), Size = new Size(47, 23) };
            var edit = new Button { Text = "修改", Location = new Point(118, 357), Size = new Size(47, 23) };
            var remove = new Button { Text = "移除", Location = new Point(168, 357), Size = new Size(47, 23) };
            add.Click += (s, e) => EditWindowOverlay(null);
            edit.Click += (s, e) => { if (tabiwOverlay_lvw.SelectedItems.Count > 0) EditWindowOverlay(tabiwOverlay_lvw.SelectedItems[0]); };
            remove.Click += (s, e) => RemoveWindowOverlay();
            tabiwOverlay_lvw.DoubleClick += (s, e) => { if (tabiwOverlay_lvw.SelectedItems.Count > 0) EditWindowOverlay(tabiwOverlay_lvw.SelectedItems[0]); };
            tabiwOverlay_lvw.KeyDown += (s, e) => { if (e.KeyCode == Keys.Delete) { RemoveWindowOverlay(); e.Handled = true; } };
            toolTip.SetToolTip(add, "新增額外疊加的窗口或線條，按 Generate 時依清單順序畫上（雙擊項目修改、Delete 移除）");
            tabimgeWindow.Controls.AddRange(new Control[] { tabiwOverlay_lvw, add, edit, remove });
        }  // 建立 Window 疊加清單
        private void EditWindowOverlay(ListViewItem item)
        {
            int imageW = int.TryParse(mnsW_txt.Text, out int w) ? w : 1920;
            int imageH = int.TryParse(mnsH_txt.Text, out int h) ? h : 1080;
            // 新增時預設：窗口在畫面中央、四分之一大小，白色
            WindowOverlay initial = item?.Tag as WindowOverlay ?? new WindowOverlay
            {
                Kind = OverlayKind.Window,
                X = imageW / 4, Y = imageH / 4, Width = imageW / 2, Height = imageH / 2,
                Color = Color.White,
            };
            using (WindowOverlayDialog dialog = new WindowOverlayDialog(initial))
            {
                if (dialog.ShowDialog(this) != DialogResult.OK || dialog.Result == null) return;
                WindowOverlay result = dialog.Result;
                if (item == null)
                {
                    item = new ListViewItem(new[] { "", "" });
                    tabiwOverlay_lvw.Items.Add(item);
                }
                item.Tag = result;
                item.SubItems[0].Text = WindowOverlay.KindText(result.Kind);
                item.SubItems[1].Text = result.Description();
                item.ToolTipText = $"{item.SubItems[0].Text} {item.SubItems[1].Text}";
                item.Selected = true;
                item.EnsureVisible();
            }
        }  // 新增 / 修改疊加項目
        private void RemoveWindowOverlay()
        {
            if (tabiwOverlay_lvw.SelectedItems.Count == 0) return;
            int index = tabiwOverlay_lvw.SelectedIndices[0];
            tabiwOverlay_lvw.Items.RemoveAt(index);
            if (tabiwOverlay_lvw.Items.Count > 0)
            {
                tabiwOverlay_lvw.Items[Math.Min(index, tabiwOverlay_lvw.Items.Count - 1)].Selected = true;
            }
        }  // 移除疊加項目
        // ImgList 編輯區--------------------------------------------------------------------------------------------
        private const string ConfigColorHelp = "顏色寫法：名稱（White、Red、Lime、Blue、Yellow、Aqua、Magenta…）、R.G.B（58.110.165）、灰階（50%Gray）";
        private static readonly Dictionary<string, (string param, string option)> ConfigHelp = new Dictionary<string, (string, string)>
        {
            ["Solid"] = (
                "參數：底色\r\n" + ConfigColorHelp + "\r\n例：White、58.110.165、50%Gray",
                "選項：框線（可省略）\r\n格式：線種-線色\r\n線種可組合：c 中心十字、o 外框、n 九點、r 左紅右藍（上下白線）\r\nr 不使用線色，只有 r 時可省略線色\r\n例：con-white、c-red、r、cr-yellow"),
            ["Gradient"] = (
                "參數：漸層顏色或 ColorBar\r\n單色：White（" + ConfigColorHelp + "）\r\n四色 ColorBar：colorbar-four-h、colorbar-four-v\r\n八色 ColorBar：colorbar-eight-h、colorbar-eight-v\r\n自訂 ColorBar：colorbar-h-數量.色1.色2…\r\n　例：colorbar-h-4.White.Red.Lime.Blue",
                "選項：漸層（省略則不漸層，只畫 ColorBar）\r\n格式：[r-]方向-階數-分割[-起始.結束]\r\nr-：反向，奇數等分反轉漸層方向（分割需 ≥ 2）\r\n方向：v（Vertical 漸層）、h（Horizontal 漸層）\r\n起始.結束：灰階 0~255，用 . 連接，省略為 0.255\r\n例：v-256-1、h-8-1、h-256-1-255.0（由亮到暗）、r-h-256-2"),
            ["Chess"] = (
                "參數：格數  寬*高\r\n例：8*8、5*5、1*2",
                "選項：可省略，多項用 - 連接\r\n顏色：格子顏色，另一色固定黑色，預設白色\r\n" + ConfigColorHelp + "\r\n翻轉：h 水平、v 垂直、hv 兩者\r\n例：Red、255.255.0-hv、v"),
            ["Window"] = (
                "參數：背景與窗口\r\n純色：背景色-窗口色，例：Gray-Black、Black-White\r\n自製圖：Custom-背景-窗口\r\n　背景：dotG、vstripeG、vstripeW、dotDell、rowW 或顏色\r\n　窗口：Black、Gray、dotW 或顏色\r\n　例：Custom-dotG-Black\r\nCrossTalk：excel1 ~ excel7",
                "選項：窗口大小%＋位置\r\nc：置中，例：50%c\r\nxy_X.Y：窗口左上角像素，例：50%xy_0.0\r\nxyq_象限：++ 右上、-+ 左上、-- 左下、+- 右下，例：50%xyq_++\r\ntwo：兩組窗口，例：50%two"),
            ["Mask"] = (
                "參數：單位與大小\r\np-寬*高：以像素為單位，例：p-2*2\r\ns-寬*高-亮度：以子像素 (RGB) 為單位，亮度 0~255，例：s-2*2-127",
                "選項：每一格的顏色\r\np 顏色寫法：每格寫一個顏色，格之間空格、列之間 /\r\n　" + ConfigColorHelp + "\r\n　也可寫 0 黑、1 白、2 灰，例：red lime/blue 127.127.127\r\n　（只有 1 格且用 R.G.B 時，結尾加 /，例：255.0.0/）\r\np 精簡寫法：每格 1 個字元 0 黑、1 白、2 灰，列之間 .，例：10.01\r\ns：每像素 3 個字元 (R G B)，1 亮、0 暗，列之間 .，例：101010.010101"),
            ["Text"] = (
                "參數：背景色-文字色\r\n" + ConfigColorHelp + "\r\n例：Black-White、Navy-Yellow",
                "選項：文字/大小/字距/行距/串距（大小之後可省略）\r\n文字：最多 10 個字，不能有逗號 , 與斜線 /，保留大小寫\r\n大小：像素 1~500（預設 12）\r\n間距：筆畫之間的空白像素 0~500，0 = 貼齊\r\n　預設 字距 大小/5、行距 大小/3、串距 大小/2\r\n例：測試ABC、ABC/24、ABC/24/0/0/10"),
            ["Dynamic"] = (
                "參數：背景色-物件色（只寫一色時物件為白色）\r\n" + ConfigColorHelp + "\r\n例：Black-White、Gray-Red",
                "選項：可省略，多項用 - 連接（選取時只預覽，按空白鍵或 Esc 全螢幕播放）\r\n形狀：ball 球、cube 方塊、strip 長條（預設 ball）\r\n方向：h 水平、v 垂直、b 斜角（預設 b，長條不能斜角）\r\n大小：size 加數字 1~200（預設 50）\r\n速度：speed 加數字 1~256（預設 50）\r\n例：ball-b-size50-speed50、strip-h-size20-speed10"),
        };
        private ToolTip configHelp_tip;
        private TextBox cfgName_txt, cfgParam_txt, cfgOption_txt;
        private ComboBox cfgType_cmb;
        private void InitConfigEditor()
        {
            configHelp_tip = new ToolTip { AutoPopDelay = 30000, InitialDelay = 200, ReshowDelay = 100, ShowAlways = true };
            var panel = new FlowLayoutPanel { Dock = DockStyle.Bottom, AutoSize = true, WrapContents = true, Padding = new Padding(2) };
            Label Caption(string text) => new Label { Text = text, AutoSize = true, Margin = new Padding(6, 7, 0, 0) };
            cfgName_txt = new TextBox { Width = 120, ImeMode = ImeMode.NoControl };  // 名稱可打中文
            cfgType_cmb = new ComboBox { Width = 85, DropDownStyle = ComboBoxStyle.DropDownList, ImeMode = ImeMode.Disable };
            cfgType_cmb.Items.AddRange(ConfigHelp.Keys.ToArray());
            cfgParam_txt = new TextBox { Width = 190, ImeMode = ImeMode.Disable };
            cfgOption_txt = new TextBox { Width = 190, ImeMode = ImeMode.Disable };
            var add = new Button { Text = "新增", Width = 50 };
            var update = new Button { Text = "修改", Width = 50 };
            var delete = new Button { Text = "刪除", Width = 50 };
            var moveUp = new Button { Text = "上移", Width = 50 };
            var moveDown = new Button { Text = "下移", Width = 50 };
            add.Click += (s, e) => SaveConfigLine(false);
            update.Click += (s, e) => SaveConfigLine(true);
            delete.Click += (s, e) => DeleteConfigLine();
            moveUp.Click += (s, e) => MoveConfigLine(-1);
            moveDown.Click += (s, e) => MoveConfigLine(+1);
            cfgType_cmb.SelectedIndexChanged += (s, e) => UpdateConfigHelp();
            // 第一行：名稱、類型、按鈕；第二行：參數、選項、config 檔案
            cfgFile_cmb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 150, ImeMode = ImeMode.Disable };
            cfgFile_cmb.DropDown += (s, e) => RefreshConfigFiles();  // 展開時重新掃描資料夾
            cfgFile_cmb.SelectionChangeCommitted += (s, e) => LoadSelectedConfigFile();
            configHelp_tip.SetToolTip(cfgFile_cmb, "程式資料夾中 config 開頭的 .txt；選擇後載入該檔案");
            // 「檔案」標籤與選單包成一組，換行時一起移動
            var fileGroup = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Margin = Padding.Empty };
            fileGroup.Controls.AddRange(new Control[] { Caption("檔案"), cfgFile_cmb });
            panel.Controls.AddRange(new Control[] {
                Caption("名稱"), cfgName_txt, Caption("類型"), cfgType_cmb, add, update, delete, moveUp, moveDown,
                Caption("參數"), cfgParam_txt, Caption("選項"), cfgOption_txt, fileGroup });
            panel.SetFlowBreak(moveDown, true);
            configHelp_tip.SetToolTip(moveUp, "選取項目往上移一列");
            configHelp_tip.SetToolTip(moveDown, "選取項目往下移一列");
            configHelp_tip.SetToolTip(cfgName_txt, "顯示在清單上的名稱，不能包含逗號");
            configHelp_tip.SetToolTip(add, "加到目前選取項目的下一列並預覽");
            configHelp_tip.SetToolTip(update, "以編輯區內容取代目前選取的項目");
            configHelp_tip.SetToolTip(delete, "刪除目前選取的項目（右鍵 SaveConfig 存檔後才會寫入檔案）");
            tabImgList.Controls.Add(panel);
            cfgType_cmb.SelectedIndex = 0;
        }  // 建立 config 編輯區
        private ComboBox cfgFile_cmb;
        private const string BuiltinConfigName = "內建設定";
        private void RefreshConfigFiles()
        {
            // 列出程式資料夾中 config 開頭的 .txt，加上內建設定與目前載入的來源
            string current = tabilPatternList_lst.Tag?.ToString();
            var items = new List<string> { BuiltinConfigName };
            items.AddRange(Directory.GetFiles(Application.StartupPath, "config*.txt")
                .Where(path => Path.GetFileName(path).StartsWith("config", StringComparison.OrdinalIgnoreCase))
                .OrderBy(path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(current) && !items.Contains(current, StringComparer.OrdinalIgnoreCase))
            {
                items.Add(current);  // 從其他位置開啟或拖入的檔案
            }
            cfgFile_cmb.Items.Clear();
            foreach (string item in items)
            {
                cfgFile_cmb.Items.Add(new ConfigFileItem(item));
            }
            SyncConfigFileCombo();
        }  // 掃描 config 檔案
        private void SyncConfigFileCombo()
        {
            // 下拉選單顯示目前載入的來源（不重新載入）
            string current = tabilPatternList_lst.Tag?.ToString();
            cfgFile_cmb.SelectedItem = cfgFile_cmb.Items.Cast<ConfigFileItem>().FirstOrDefault(i => string.Equals(i.Path, current, StringComparison.OrdinalIgnoreCase));
        }  // 同步目前來源
        private void LoadSelectedConfigFile()
        {
            if (!(cfgFile_cmb.SelectedItem is ConfigFileItem item)) return;
            try
            {
                if (item.Path == BuiltinConfigName)
                {
                    LoadConfig(Resources.config, BuiltinConfigName);
                }
                else
                {
                    LoadConfig(ReadConfigText(item.Path), item.Path);
                }
                ssrStatus_lbl.Text = $"Config：{item.Path}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"讀取檔案失敗：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SyncConfigFileCombo();
            }
        }  // 載入選擇的 config
        private class ConfigFileItem
        {
            public string Path { get; }
            public ConfigFileItem(string path) { Path = path; }
            public override string ToString() => Path == BuiltinConfigName ? Path : System.IO.Path.GetFileName(Path);
        }  // 下拉選單項目：顯示檔名，保留完整路徑
        private void UpdateConfigHelp()
        {
            // 參數與選項的 tooltip 依類型顯示對應寫法
            string type = cfgType_cmb.SelectedItem?.ToString();
            if (type == null || !ConfigHelp.TryGetValue(type, out var help)) return;
            configHelp_tip.SetToolTip(cfgType_cmb, $"{type}：滑鼠移到參數、選項欄位可看寫法");
            configHelp_tip.SetToolTip(cfgParam_txt, help.param);
            configHelp_tip.SetToolTip(cfgOption_txt, help.option);
        }  // 更新寫法提示
        private void FillConfigEditor(ListViewItem item)
        {
            // SubItems：0 編號、1 名稱、2 類型、3 參數、4 選項
            cfgName_txt.Text = item.SubItems[1].Text;
            string type = ConfigHelp.Keys.FirstOrDefault(k => k.Equals(item.SubItems[2].Text, StringComparison.OrdinalIgnoreCase));
            if (type != null) cfgType_cmb.SelectedItem = type;
            cfgParam_txt.Text = item.SubItems[3].Text;
            cfgOption_txt.Text = item.SubItems[4].Text;
        }  // 選取項目帶入編輯區
        private void SaveConfigLine(bool replaceSelected)
        {
            string[] values = { cfgName_txt.Text.Trim(), cfgType_cmb.SelectedItem?.ToString() ?? "", cfgParam_txt.Text.Trim(), cfgOption_txt.Text.Trim() };
            if (values[0] == "" || values[2] == "")
            {
                MessageBox.Show("請填寫名稱與參數！", "Config", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (values.Any(v => v.Contains(",")))
            {
                MessageBox.Show("欄位內容不能包含逗號（逗號是欄位分隔符號）！", "Config", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            ListViewItem item = CreateConfigItem(string.Join(",", values));
            int selected = tabilPatternList_lst.SelectedIndices.Count > 0 ? tabilPatternList_lst.SelectedIndices[0] : -1;
            if (replaceSelected)
            {
                if (selected < 0)
                {
                    MessageBox.Show("請先在清單選取要修改的項目！", "Config", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                tabilPatternList_lst.Items[selected] = item;
            }
            else
            {
                // 加在選取項目的下一列；沒有選取時加在最後
                tabilPatternList_lst.Items.Insert(selected >= 0 ? selected + 1 : tabilPatternList_lst.Items.Count, item);
            }
            RenumberPatternList();
            item.Selected = true;  // 觸發預覽，格式錯誤會提示
            item.Focused = true;
            item.EnsureVisible();
        }  // 新增 / 修改 config 項目
        private void DeleteConfigLine()
        {
            if (tabilPatternList_lst.SelectedItems.Count == 0) return;
            int index = tabilPatternList_lst.SelectedIndices[0];
            tabilPatternList_lst.Items.RemoveAt(index);
            RenumberPatternList();
            if (tabilPatternList_lst.Items.Count > 0)
            {
                tabilPatternList_lst.Items[Math.Min(index, tabilPatternList_lst.Items.Count - 1)].Selected = true;
            }
        }  // 刪除 config 項目
        // 預覽區：config 文字、最近使用、螢幕更新率-----------------------------------------------------------------
        private Label showimgConfig_lbl;
        private GroupBox recent_grp;
        private ListBox recent_lst;
        private ToolStripLabel mnsRefresh_lbl;
        private bool _configDraftFocused;  // 焦點在 ImgList 編輯區後，Enter / Generate 預覽草稿
        private RecentItem _shownItem;     // 目前預覽來自最近使用 / 草稿（null 表示來自目前分頁）
        private bool _dirListLoaded;       // DirList 只在第一次切換時自動讀取
        private const int RecentMax = 10;
        private class RecentItem
        {
            public string Name { get; private set; }
            public string Line { get; private set; }  // config 一行；圖檔為 null
            public string Path { get; private set; }  // 圖檔路徑；config 為 null
            // 相同內容只留一筆：config 比對名稱以外的欄位，圖檔比對路徑
            public string Key => Line != null
                ? "cfg:" + string.Join(",", Line.Split(',').Skip(1).Select(v => v.Trim().ToLower()))
                : "file:" + Path.ToLower();
            public string Source { get; private set; }  // 來源分頁：ImgList / ImgEditor / DirList
            public static RecentItem FromConfig(string line) => new RecentItem { Source = "ImgList", Name = line.Split(',')[0].Trim(), Line = line };
            public static RecentItem FromEditor(string line)
            {
                // ImgEditor 沒有名稱：以「類型_參數_選項」顯示（選項空白時省略）
                string[] v = line.Split(',').Select(s => s.Trim()).ToArray();
                string name = string.Join("_", v.Skip(1).Where(s => s != ""));
                return new RecentItem { Source = "ImgEditor", Name = name, Line = line };
            }
            public static RecentItem FromFile(string path) => new RecentItem { Source = "DirList", Name = System.IO.Path.GetFileName(path), Path = path };
            public override string ToString() => $"{Source} / {Name}";  // 清單顯示「分頁 / 名稱」；全螢幕左上角只用 Name
        }  // 最近使用項目
        private void InitPreviewPanel()
        {
            // Showimage_flp 由下往上排列（index 越大越上面）：縮圖上方插入 config 文字，最上方為最近使用
            int width = showimgPicture_pic.Width;
            showimgConfig_lbl = new Label
            {
                AutoSize = true, MaximumSize = new Size(width, 0), UseMnemonic = false, Visible = false,
                ForeColor = Color.DimGray, Margin = new Padding(3, 0, 3, 2)
            };
            float fontSize = showimgConfig_lbl.Font.Size == 12f ? 14f : showimgConfig_lbl.Font.Size + 2f;  // 12 調到 14，其他大 2 號
            showimgConfig_lbl.Font = new Font(showimgConfig_lbl.Font.FontFamily, fontSize, showimgConfig_lbl.Font.Style);
            Showimage_flp.Controls.Add(showimgConfig_lbl);
            Showimage_flp.Controls.SetChildIndex(showimgConfig_lbl, Showimage_flp.Controls.GetChildIndex(showimgSize_lbl));
            recent_lst = new ListBox { Dock = DockStyle.Fill, IntegralHeight = false, HorizontalScrollbar = true, DrawMode = DrawMode.OwnerDrawFixed };
            recent_lst.Font = new Font(recent_lst.Font.FontFamily, 11f);
            recent_lst.ItemHeight = recent_lst.Font.Height + 2;
            recent_lst.DrawItem += (s, e) =>
            {
                // 第 2、4、6… 項用 225 灰底區分；選取時用系統選取色
                if (e.Index < 0) return;
                bool selected = (e.State & DrawItemState.Selected) != 0;
                Color back = selected ? SystemColors.Highlight : e.Index % 2 == 1 ? Color.FromArgb(225, 225, 225) : recent_lst.BackColor;
                using (Brush brush = new SolidBrush(back))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }
                TextRenderer.DrawText(e.Graphics, recent_lst.Items[e.Index].ToString(), recent_lst.Font, e.Bounds,
                    selected ? SystemColors.HighlightText : recent_lst.ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            };
            recent_lst.DoubleClick += (s, e) => OpenRecent();
            recent_lst.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; OpenRecent(); }
            };
            toolTip.SetToolTip(recent_lst, "最近開啟 / 編輯的 10 張圖片：單擊選取、雙擊或 Enter 開啟");
            recent_grp = new GroupBox { Text = "最近使用", Width = width, Height = 100, Margin = new Padding(3, 2, 3, 2) };
            recent_grp.Controls.Add(recent_lst);
            Showimage_flp.Controls.Add(recent_grp);  // 最後加入 = 最上方
            Showimage_flp.Resize += (s, e) => LayoutRecent();
            // 工具列最右邊顯示目前螢幕的更新率（右對齊項目中第一個在最右邊）
            mnsRefresh_lbl = new ToolStripLabel { Alignment = ToolStripItemAlignment.Right, ToolTipText = "目前螢幕的更新率" };
            menuStrip.Items.Insert(0, mnsRefresh_lbl);
            // ImgList 編輯區：取得焦點後 Enter 或 Generate 預覽草稿
            foreach (Control field in new Control[] { cfgName_txt, cfgType_cmb, cfgParam_txt, cfgOption_txt })
            {
                field.Enter += (s, e) => _configDraftFocused = true;
                field.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; PreviewConfigDraft(); }
                };
            }
            tabilPatternList_lst.Enter += (s, e) => _configDraftFocused = false;  // 回到清單後 Generate 依清單選取
        }  // 建立預覽區的 config 文字、最近使用與更新率
        private void LayoutRecent()
        {
            // 最近使用填滿上方剩餘的高度
            if (recent_grp == null) return;
            int used = Showimage_flp.Controls.Cast<Control>()
                .Where(c => c != recent_grp && (c != showimgConfig_lbl || showimgConfig_lbl.Text != ""))
                .Sum(c => c.Height + c.Margin.Vertical);
            recent_grp.Height = Math.Max(60, Showimage_flp.ClientSize.Height - Showimage_flp.Padding.Vertical - used - recent_grp.Margin.Vertical);
        }  // 調整最近使用高度
        private void ShowPreviewInfo(int width, int height, string line)
        {
            // 圖片大小與 config（line 為 null 時隱藏 config 文字）
            showimgSize_lbl.Text = $"當前圖片大小：{width} x {height}";
            if (line != null)
            {
                line = string.Join(",", line.Split(',').Select(v => v.Trim())).TrimEnd(',');  // 去掉對齊用的 Tab 與空白
            }
            ShowConfigNote(line);
        }  // 更新預覽資訊
        private void ShowConfigNote(string text)
        {
            // 縮圖上方的文字（null 或空字串時隱藏）
            showimgConfig_lbl.Text = text ?? "";
            showimgConfig_lbl.Visible = !string.IsNullOrEmpty(text);
            toolTip.SetToolTip(showimgConfig_lbl, text);
            LayoutRecent();
        }  // 顯示 config 文字
        private void AddRecent(RecentItem item)
        {
            var existing = recent_lst.Items.Cast<RecentItem>().FirstOrDefault(i => i.Key == item.Key);
            if (existing != null) recent_lst.Items.Remove(existing);
            recent_lst.Items.Insert(0, item);
            while (recent_lst.Items.Count > RecentMax)
            {
                recent_lst.Items.RemoveAt(recent_lst.Items.Count - 1);
            }
            // 自繪清單不會自動計算水平捲軸寬度
            recent_lst.HorizontalExtent = recent_lst.Items.Cast<object>().Max(i => TextRenderer.MeasureText(i.ToString(), recent_lst.Font).Width) + 4;
        }  // 加入最近使用（最新的在最上面）
        private void OpenRecent()
        {
            if (!(recent_lst.SelectedItem is RecentItem item)) return;
            bool ok = item.Line != null ? ApplyConfigLine(item.Line) : LoadImageFile(item.Path);
            if (!ok) return;
            _shownItem = item;
            AddRecent(item);
            recent_lst.SelectedItem = item;
            ssrStatus_lbl.Text = $"最近使用：{item.Path ?? item.Line}";
            if (_pictureWindow != null && !_pictureWindow.IsDisposed)
            {
                FullScreen(recent_lst, EventArgs.Empty);  // 全螢幕開著時一起更新
            }
        }  // 開啟最近使用
        private void PreviewConfigDraft()
        {
            // 依 ImgList 編輯區目前內容產生預覽（不寫入清單）
            string name = cfgName_txt.Text.Trim();
            string[] values = { name == "" ? "預覽" : name, cfgType_cmb.SelectedItem?.ToString() ?? "", cfgParam_txt.Text.Trim(), cfgOption_txt.Text.Trim() };
            if (values[2] == "")
            {
                ssrStatus_lbl.Text = "預覽：請先填寫參數";
                return;
            }
            if (values.Any(v => v.Contains(",")))
            {
                MessageBox.Show("欄位內容不能包含逗號（逗號是欄位分隔符號）！", "Config", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string line = string.Join(",", values);
            if (!ApplyConfigLine(line)) return;
            var item = RecentItem.FromConfig(line);
            _shownItem = item;
            AddRecent(item);
            ssrStatus_lbl.Text = $"預覽草稿：{line}";
        }  // 預覽編輯區草稿
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string dmDeviceName;
            public short dmSpecVersion, dmDriverVersion, dmSize, dmDriverExtra;
            public int dmFields, dmPositionX, dmPositionY, dmDisplayOrientation, dmDisplayFixedOutput;
            public short dmColor, dmDuplex, dmYResolution, dmTTOption, dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel, dmPelsWidth, dmPelsHeight, dmDisplayFlags, dmDisplayFrequency;
            public int dmICMMethod, dmICMIntent, dmMediaType, dmDitherType, dmReserved1, dmReserved2, dmPanningWidth, dmPanningHeight;
        }
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);
        private void UpdateRefreshRate()
        {
            // 顯示下拉選單目前螢幕的更新率（沒有選擇時保留原本顯示）
            string device = mnsScreenlist_cmb.SelectedItem?.ToString();
            if (device == null || mnsRefresh_lbl == null) return;
            var mode = new DEVMODE { dmSize = (short)Marshal.SizeOf(typeof(DEVMODE)) };
            mnsRefresh_lbl.Text = EnumDisplaySettings(device, -1, ref mode) && mode.dmDisplayFrequency > 1  // -1：ENUM_CURRENT_SETTINGS
                ? $"{mode.dmDisplayFrequency} Hz"
                : "? Hz";
        }  // 更新螢幕更新率
        private void InitDirListReload()
        {
            var reload = new Button { Text = "重新讀取", AutoSize = true, Dock = DockStyle.Top };
            reload.Click += (s, e) => { PopulateListView(); ssrStatus_lbl.Text = $"已重新讀取：{Application.StartupPath}"; };
            toolTip.SetToolTip(reload, "重新掃描程式資料夾的圖檔（新增 / 刪除檔案後使用）");
            tabDirList.Controls.Add(reload);
            tabdlPatternList_lvw.BringToFront();  // 清單最後 Dock，填滿按鈕以外的空間
        }  // DirList 重新讀取按鈕
        private void InitImgListExport()
        {
            var export = new ToolStripMenuItem("全部匯出成 PNG...", null, (s, e) => ExportImgList());
            export.ToolTipText = "依目前寬高把 ImgList 每一項存成「編號_名稱.png」";
            tabilPatternList_cms.Items.Add(new ToolStripSeparator());
            tabilPatternList_cms.Items.Add(export);
        }  // ImgList 右鍵匯出
        private void ExportImgList()
        {
            if (tabilPatternList_lst.Items.Count == 0) return;
            string folder;
            using (var dialog = new FolderBrowserDialog { Description = $"選擇匯出資料夾（{mnsW_txt.Text} x {mnsH_txt.Text}，共 {tabilPatternList_lst.Items.Count} 張）", SelectedPath = Application.StartupPath })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                folder = dialog.SelectedPath;
            }
            ExportImgListTo(folder);
        }  // 選擇資料夾後匯出
        private int ExportImgListTo(string folder)
        {
            // 逐項產生並存檔；有問題的項目略過，最後一次列出
            var errors = new List<string>();
            int saved = 0, total = tabilPatternList_lst.Items.Count;
            char[] invalid = Path.GetInvalidFileNameChars();
            ssrProgressbar_prg.Maximum = total;
            Cursor = Cursors.WaitCursor;
            try
            {
                foreach (ListViewItem item in tabilPatternList_lst.Items)
                {
                    string name = new string(item.SubItems[1].Text.Trim().Select(ch => invalid.Contains(ch) ? '_' : ch).ToArray());
                    string fileName = $"{item.Index + 1:000}_{name}.png";
                    int before = errors.Count;
                    if (ApplyConfigLine(item.Tag.ToString(), errors) && showimgPicture_pic.Image != null)
                    {
                        showimgPicture_pic.Image.Save(Path.Combine(folder, fileName), ImageFormat.Png);
                        saved++;
                    }
                    else if (errors.Count == before)
                    {
                        errors.Add($"「{name}」無法產生圖片");
                    }
                    ssrProgressbar_prg.Value = item.Index + 1;
                    ssrStatus_lbl.Text = $"匯出中 {item.Index + 1}/{total}：{fileName}";
                    Application.DoEvents();  // 更新進度列
                }
            }
            finally
            {
                Cursor = Cursors.Default;
                ssrProgressbar_prg.Value = 0;
            }
            ssrStatus_lbl.Text = $"已匯出 {saved}/{total} 張到 {folder}";
            if (errors.Count > 0)
            {
                MessageBox.Show($"以下 {errors.Count} 項沒有匯出：\r\n\r\n・" + string.Join("\r\n・", errors), "全部匯出", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            // 預覽恢復成目前選取的項目
            if (tabilPatternList_lst.SelectedItems.Count > 0)
            {
                ApplyConfigLine(tabilPatternList_lst.SelectedItems[0].Tag.ToString());
            }
            return saved;
        }  // 匯出 ImgList 全部圖片
        private static void DrawScreenshot(Graphics g, Image image, int x, int y, int width, int height)
        {
            // Window 匯入的截圖：比目標大時 1:1 從左上裁切，比目標小時拉伸（寬、高分開判斷）
            if (image == null || width <= 0 || height <= 0) return;
            var source = new Rectangle(0, 0, Math.Min(image.Width, width), Math.Min(image.Height, height));
            GraphicsState state = g.Save();
            g.InterpolationMode = InterpolationMode.NearestNeighbor;  // 拉伸時保持像素銳利
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.DrawImage(image, new Rectangle(x, y, width, height), source, GraphicsUnit.Pixel);
            g.Restore(state);
        }  // 畫截圖（大裁切、小拉伸）
        private void InitMassTooltip()
        {
            string tip = "勾選後按 AutoSave（Ctrl+S）批量輸出純色圖：\r\n" +
                         "・從 0 開始，每隔設定的階數輸出一張，最後一張為背景色\r\n" +
                         "　例：背景白 (255)、每 32 階 → 0、32、64 … 224、255\r\n" +
                         "・各色版不超過背景色（背景 255.0.0 → 只輸出紅色階）\r\n" +
                         "・檔名為「檔名_灰階.png」，存在程式資料夾，同名時加 (2)、(3)…\r\n" +
                         "・只輸出背景純色，框線、文字等設定不會畫上";
            toolTip.SetToolTip(tabiefMass_chk, tip);
            toolTip.SetToolTip(tabiefMassNum_nud, "每幾階輸出一張（1 = 每一階都輸出）");
        }  // 大量建圖說明
        private void InitMaskZoom()
        {
            // Mask 格子與 Gradient 色條面板旁加「放大」：移到可拖拉大小的視窗編輯，關閉後放回原位
            foreach (var (panel, box, clear) in new[] {
                (tabiemPixelPanel_pnl, (Control)tabimgeMask, tabiemPixelClear_btn),
                (tabiwCMaskPixelPanel_pnl, (Control)tabiwCustomMask_grp, tabiwCMaskPixelClear_btn) })
            {
                AddZoomButton(panel, box, new Rectangle(clear.Right + 6, clear.Top, 44, 24), "Mask");
            }
            AddZoomButton(tabigOtherSplit_pnl, tabigOther_grp, new Rectangle(285, 160, 62, 24), "ColorBar");
            // Window 自製圖的 ColorBar 空間較小：四色 / 八色按鈕稍微上移、群組加高放入放大按鈕
            tabiwCGradOther_grp.Height += 5;
            foreach (Button button in new[] { tabiwCGradFourColor_btn, tabiwCGradEightColor_btn })
            {
                button.SetBounds(button.Left, 128, button.Width, 22);
            }
            AddZoomButton(tabiwCGradOtherSplit_pnl, tabiwCGradOther_grp, new Rectangle(280, 152, 62, 20), "ColorBar");
        }  // Mask / ColorBar 放大按鈕
        private void AddZoomButton(Panel panel, Control box, Rectangle bounds, string title)
        {
            var zoom = new Button { Text = "放大", Bounds = bounds };
            zoom.Click += (s, e) => ZoomBlockPanel(panel, title);
            toolTip.SetToolTip(zoom, $"開啟可拖拉大小的視窗編輯 {title}（格子多時比較好點選）");
            box.Controls.Add(zoom);
            panel.Resize += (s, e) => LayoutBlocks(panel);
        }  // 加入放大按鈕
        private static (int x, int y) BlockCell(Control block)
        {
            // Mask 格子 Tag 為「x,y」；ColorBar 色條為「V,序號」（直排）或「H,序號」（橫排）
            string[] xy = block.Tag.ToString().Split(',');
            if (xy[0] == "V") return (int.Parse(xy[1]), 0);
            if (xy[0] == "H") return (0, int.Parse(xy[1]));
            return (int.Parse(xy[0]), int.Parse(xy[1]));
        }  // 格子座標
        private static void LayoutBlocks(Panel panel)
        {
            // 依面板大小重新排列格子（保留顏色）
            var blocks = panel.Controls.OfType<Panel>().ToList();
            if (blocks.Count == 0) return;
            int columns = blocks.Max(b => BlockCell(b).x) + 1, rows = blocks.Max(b => BlockCell(b).y) + 1;
            int blockWidth = Math.Max(1, panel.Width / columns), blockHeight = Math.Max(1, panel.Height / rows);
            panel.SuspendLayout();
            foreach (Panel block in blocks)
            {
                var (x, y) = BlockCell(block);
                block.SetBounds(x * blockWidth, y * blockHeight, blockWidth, blockHeight);
            }
            panel.ResumeLayout();
        }  // 重新排列格子
        private void ZoomBlockPanel(Panel panel, string title)
        {
            // 每次開啟建立新視窗、關閉即釋放；格子本身直接搬過去用，不另外複製
            var blocks = panel.Controls.OfType<Panel>().ToList();
            if (blocks.Count == 0) return;
            int columns = blocks.Max(b => BlockCell(b).x) + 1, rows = blocks.Max(b => BlockCell(b).y) + 1;
            Control parent = panel.Parent;
            Rectangle bounds = panel.Bounds;
            int index = parent.Controls.GetChildIndex(panel);
            var coord = new Label { Dock = DockStyle.Bottom, Height = 22, TextAlign = ContentAlignment.MiddleLeft, Text = "左鍵填色、右鍵清除；拖拉視窗邊緣可調整大小，Esc 關閉" };
            EventHandler showCoord = (s, e) => coord.Text = $"座標 {((Control)s).Tag}";
            Rectangle area = Screen.FromControl(this).WorkingArea;
            Size client;
            if (columns > 1 && rows > 1)
            {
                // Mask：格子盡量為正方形
                int cell = Math.Max(6, Math.Min((area.Width - 80) / columns, (area.Height - 120) / rows));
                cell = Math.Min(cell, Math.Max(24, 640 / Math.Max(columns, rows)));
                client = new Size(columns * cell, rows * cell);
            }
            else
            {
                // ColorBar：直排色條用橫長視窗、橫排用直長視窗
                client = columns > 1 ? new Size(720, 400) : new Size(400, Math.Min(640, area.Height - 120));
            }
            using (var form = new Form
            {
                Text = $"{title} 放大編輯（{columns} x {rows}）", ShowIcon = false, ShowInTaskbar = false, MinimizeBox = false,
                StartPosition = FormStartPosition.CenterParent, KeyPreview = true,
                ClientSize = new Size(client.Width, client.Height + coord.Height), MinimumSize = new Size(200, 200),
            })
            {
                form.KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) form.Close(); };
                blocks.ForEach(b => b.MouseEnter += showCoord);
                parent.Controls.Remove(panel);
                panel.Dock = DockStyle.Fill;
                form.Controls.Add(panel);
                form.Controls.Add(coord);  // 後加入的先 Dock，座標列在下方
                try
                {
                    form.ShowDialog(this);
                }
                finally
                {
                    form.Controls.Remove(panel);
                    blocks.ForEach(b => b.MouseEnter -= showCoord);
                    panel.Dock = DockStyle.None;
                    panel.Bounds = bounds;
                    parent.Controls.Add(panel);
                    parent.Controls.SetChildIndex(panel, index);
                    LayoutBlocks(panel);
                }
            }
        }  // 放大編輯視窗
        private bool _movingConfigLine;  // 移動時重新選取不重新產生圖片
        private void MoveConfigLine(int step)
        {
            if (tabilPatternList_lst.SelectedItems.Count == 0) return;
            int index = tabilPatternList_lst.SelectedIndices[0];
            int target = index + step;
            if (target < 0 || target >= tabilPatternList_lst.Items.Count) return;
            ListViewItem item = tabilPatternList_lst.Items[index];
            _movingConfigLine = true;
            try
            {
                tabilPatternList_lst.Items.RemoveAt(index);
                tabilPatternList_lst.Items.Insert(target, item);
                RenumberPatternList();
                item.Selected = true;
                item.Focused = true;
                item.EnsureVisible();
            }
            finally
            {
                _movingConfigLine = false;
            }
        }  // 上移 / 下移 config 項目
        private void ConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem targetbutton)
            {
                if (targetbutton.Name == "openConfigToolStripMenuItem")
                {
                    // 使用 OpenFileDialog 選擇配置檔案
                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.Filter = "Text Files|*.txt";
                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            try
                            {
                                LoadConfig(ReadConfigText(openFileDialog.FileName), openFileDialog.FileName);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"讀取檔案失敗：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                else if (targetbutton.Name == "saveConfigToolStripMenuItem")
                {
                    // 儲存配置檔案
                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.Filter = "Text Files|*.txt";
                        saveFileDialog.FileName = "config.txt";
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            // 第一行寫欄名，其餘照原始內容寫回；UTF-8（含 BOM）方便記事本與 Excel 開啟
                            var lines = new[] { string.Join(",", ConfigColumns) }
                                .Concat(tabilPatternList_lst.Items.Cast<ListViewItem>().Select(item => item.Tag.ToString()));
                            try
                            {
                                File.WriteAllText(saveFileDialog.FileName, string.Join(Environment.NewLine, lines), Encoding.UTF8);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"儲存失敗：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
        }  // 設定檔案載入與儲存
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
                        mypicture.tag = "color";
                        //if (targetButton.Name.Contains("tabiefBack_btn"))
                        //{
                        //}
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
            tabiecGray_lbl.Text = $"Gray: {tabiecGray_hsc.Value}"; // 更新 Label 顯示當前值
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
                // 計算每個分區的長度
                int dimension = isH ? resultBitmap.Width : resultBitmap.Height;
                int segmentLength = isH ? resultBitmap.Height : resultBitmap.Width;
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
                        int positionIndex = isH ? x / (resultBitmap.Width / segments) : y / (resultBitmap.Height / segments);

                        // 計算當前像素在分區內的位置
                        int positionInSegment = position % segmentLength;
                        // 如果分區是奇數，反向透明度
                        int stepIndex;
                        byte alpha;
                        if (positionIndex % 2 == 1)
                        {
                            stepIndex = (int)((float)positionInSegment / segmentLength * (steps));
                            alpha = (byte)(endAlpha - alphaStep * stepIndex);
                        }
                        else // 計算透明度
                        {
                            stepIndex = (int)((float)positionInSegment / segmentLength * (steps));
                            alpha = (byte)(startAlpha + alphaStep * stepIndex);
                        }

                        // 修改 Alpha 值
                        pixelValues[index + 3] = alpha; // Alpha 通道
                    }
                }
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

                        // 計算當前像素在分區內的位置
                        int positionInSegment = position % segmentLength;

                        // 計算透明度
                        int stepIndex = (int)((float)positionInSegment / segmentLength * (steps));
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

                Control box = targetPanel.Parent;  // 先記下群組：放大編輯時面板會暫時移到其他視窗
                RadioButton targetRdo = box.Controls.OfType<RadioButton>()
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
                            Label targetLabel = box.Controls.OfType<Label>()
                            .FirstOrDefault(label => label.Name.Contains("Color_lbl"));
                            block.BackColor = targetLabel.BackColor;
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
                        Label targetLabel = box.Controls.OfType<Label>()
                        .FirstOrDefault(label => label.Name.Contains("Loc_lbl"));
                        targetLabel.Text = $"{block.Tag}"; // 更新 Label 顯示區塊座標
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
                        int outW = bitmap.Width, outH = bitmap.Height;
                        var done = new List<object>();
                        var failed = new List<string>();
                        foreach (object iteamChecked in tabieaPatternList_chl.CheckedItems)
                        {
                            string sourcePath = iteamChecked.ToString();
                            // 用 Path 取副檔名（資料夾名稱含 . 也不會切錯，大小寫皆可）
                            string extension = Path.GetExtension(sourcePath).ToLower();
                            ImageFormat format = extension == ".bmp" ? ImageFormat.Bmp
                                : extension == ".png" ? ImageFormat.Png
                                : extension == ".jpg" || extension == ".jpeg" ? ImageFormat.Jpeg : null;
                            if (format == null)
                            {
                                failed.Add($"{Path.GetFileName(sourcePath)}：不支援的格式 {extension}");
                                ssrProgressbar_prg.Value++;
                                continue;
                            }
                            try
                            {
                                Bitmap source;
                                using (FileStream stream = File.OpenRead(sourcePath))  // 讀進記憶體後即釋放檔案，不鎖檔
                                using (Image image = Image.FromStream(stream))
                                {
                                    source = new Bitmap(image);
                                }
                                string suffix;
                                using (source)
                                {
                                    g.Clear(Color.Black);  // 每張先清空，避免殘留上一張；留邊處為黑色
                                    if (tabieaResize_rdo.Checked)
                                    {
                                        g.DrawImage(source, ResizeRectangle(source.Size, outW, outH, tabieaResizeMode_cmb.SelectedIndex));
                                        string[] modeSuffix = { "", "Fit_", "Fill_", "1to1_" };
                                        suffix = $"{modeSuffix[Math.Max(0, tabieaResizeMode_cmb.SelectedIndex)]}{outW}x{outH}";
                                    }
                                    else
                                    {
                                        var (rotate, rotateName) =
                                              tabieaRotate90_rdo.Checked ? (RotateFlipType.Rotate90FlipNone, "Rotate90")
                                            : tabieaRotate180_rdo.Checked ? (RotateFlipType.Rotate180FlipNone, "Rotate180")
                                            : tabieaRotate270_rdo.Checked ? (RotateFlipType.Rotate270FlipNone, "Rotate270")
                                            : tabieaHFlip_rdo.Checked ? (RotateFlipType.RotateNoneFlipX, "FlipX")
                                            : (RotateFlipType.RotateNoneFlipY, "FlipY");
                                        source.RotateFlip(rotate);
                                        g.DrawImage(source, 0, 0, outW, outH);
                                        suffix = $"{rotateName}_{outW}x{outH}";
                                    }
                                }
                                string outputPath = Path.Combine(Path.GetDirectoryName(sourcePath), $"{Path.GetFileNameWithoutExtension(sourcePath)}_{suffix}{extension}");
                                bitmap.Save(outputPath, format);
                                done.Add(iteamChecked);
                            }
                            catch (Exception ex)
                            {
                                failed.Add($"{Path.GetFileName(sourcePath)}：{ex.Message}");
                            }
                            ssrProgressbar_prg.Value++;
                        }
                        // 只移除轉換成功的項目，失敗的留在清單上
                        foreach (object item in done)
                        {
                            tabieaPatternList_chl.Items.Remove(item);
                        }
                        if (failed.Count > 0)
                        {
                            MessageBox.Show($"以下 {failed.Count} 個檔案轉換失敗：\r\n\r\n" + string.Join("\r\n", failed), "ImageAdjust", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            ssrProgressbar_prg.Visible = false; // 在迴圈完成後隱藏進度條
        }  // Adjust 調整圖片
        private static Rectangle ResizeRectangle(Size source, int width, int height, int mode)
        {
            // 0 拉伸填滿、1 等比例（留黑邊）、2 填滿裁切（等比例，超出裁掉）、3 1:1 置中
            if (mode == 3) return new Rectangle((width - source.Width) / 2, (height - source.Height) / 2, source.Width, source.Height);
            if (mode == 1 || mode == 2)
            {
                double sx = (double)width / source.Width, sy = (double)height / source.Height;
                double scale = mode == 1 ? Math.Min(sx, sy) : Math.Max(sx, sy);
                int w = (int)Math.Round(source.Width * scale), h = (int)Math.Round(source.Height * scale);
                return new Rectangle((width - w) / 2, (height - h) / 2, w, h);
            }
            return new Rectangle(0, 0, width, height);
        }  // Resize 縮放模式的繪製範圍
        private void InitQuickButtons()
        {
            // Gradient：FirstLevel / LastLevel 右側，數值對調（例 0/255 → 255/0）
            var swap = new Button { Text = "對調", Location = new Point(368, 45), Size = new Size(44, 24) };
            swap.Click += (s, e) =>
            {
                string first = tabigFirstLevel_cmb.Text;
                tabigFirstLevel_cmb.Text = tabigLastLevel_cmb.Text;
                tabigLastLevel_cmb.Text = first;
            };
            toolTip.SetToolTip(swap, "FirstLevel 與 LastLevel 數值對調（例：0 / 255 → 255 / 0）");
            tabimgeGradient.Controls.Add(swap);
            swap.BringToFront();

            // Chess：灰階捲軸下方，快速設定 水平×垂直 格數
            var sizes = new (int h, int v)[] { (2, 2), (5, 5), (8, 8), (32, 32), (8, 5) };
            for (int i = 0; i < sizes.Length; i++)
            {
                var (h, v) = sizes[i];
                var button = new Button { Text = $"{h}×{v}", Location = new Point(85 + i * 52, 148), Size = new Size(50, 24) };
                button.Click += (s, e) =>
                {
                    tabiecHNum_nud.Value = Math.Min(h, tabiecHNum_nud.Maximum);
                    tabiecVNum_nud.Value = Math.Min(v, tabiecVNum_nud.Maximum);
                };
                toolTip.SetToolTip(button, $"水平 {h} 格、垂直 {v} 格");
                tabimgeChess.Controls.Add(button);
                button.BringToFront();
            }
        }  // Gradient 對調、Chess 快速格數
        private ComboBox tabieaResizeMode_cmb;
        private void InitResizeMode()
        {
            // ReSize 右側：縮放模式（選 ReSize 時才可用）
            tabieaResizeMode_cmb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(75, 42), Width = 85, ImeMode = ImeMode.Disable };
            tabieaResizeMode_cmb.Items.AddRange(new object[] { "拉伸填滿", "等比例", "填滿裁切", "1:1 置中" });
            tabieaResizeMode_cmb.SelectedIndex = 0;
            toolTip.SetToolTip(tabieaResizeMode_cmb, "拉伸填滿：拉成目前寬高\r\n等比例：完整顯示，留黑邊\r\n填滿裁切：等比例填滿，超出裁掉\r\n1:1 置中：原尺寸置中，不縮放");
            tabimgeAdjust.Controls.Add(tabieaResizeMode_cmb);
            tabieaResizeMode_cmb.BringToFront();
            void Update() => tabieaResizeMode_cmb.Enabled = tabieaResize_rdo.Checked;
            tabieaResize_rdo.CheckedChanged += (s, e) => Update();
            Update();
        }  // 建立縮放模式選單
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
                GroupBox box;  // 顏色按鈕與座標標籤所在的群組（面板放大編輯時 Parent 會暫時改變，不能用 Parent 找）
                if (control.Parent != null && control.Parent.Name == "tabimgeMask")
                {
                    box = tabimgeMask;
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
                    box = tabiwCustomMask_grp;
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
                                    Button targetButton = box.Controls.OfType<Button>()
                                    .FirstOrDefault(button => button.Name.Contains("PixelColor"));
                                    block.BackColor = targetButton.BackColor;
                                }
                            };
                            block.MouseEnter += (s, args) =>
                            {
                                Label targetLabel = box.Controls.OfType<Label>()
                                .FirstOrDefault(label => label.Name.Contains("Loc_lbl"));
                                targetLabel.Text = $"{block.Tag}"; // 更新 Label 顯示區塊座標
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
                                Label targetLabel = box.Controls.OfType<Label>()
                                .FirstOrDefault(label => label.Name.Contains("PixelLoc_lbl"));
                                targetLabel.Text = $"{block.Tag}"; // 更新 Label 顯示區塊座標
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
                    int pixelW = (int)tabiwCMaskWNum_nud.Value;
                    int pixelH = (int)tabiwCMaskHNum_nud.Value;
                    if (pixelW * pixelH <= 9216)  //太小的像素要加速
                    {
                        int minhw = Math.Min(256 / pixelW, 144 / pixelH);
                        pixelW = minhw * pixelW;
                        pixelH = minhw * pixelH;
                        if (tabiwCMaskPixel_rdo.Checked)
                        {
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

                                        for (int i = 0; i < pixelH; i += (int)tabiwCMaskHNum_nud.Value)
                                        {
                                            for (int j = 0; j < pixelW; j += (int)tabiwCMaskWNum_nud.Value)
                                            {
                                                // 計算像素在數組中的索引
                                                //int index = (y * stride) + (x * 4);
                                                int index = (y * stride) + (x * 4) + i * stride + j * 4;

                                                // 設置像素顏色
                                                Color color = panel.BackColor;
                                                pixelBuffer[index] = color.B;       // 藍色
                                                pixelBuffer[index + 1] = color.G;  // 綠色
                                                pixelBuffer[index + 2] = color.R;  // 紅色
                                                pixelBuffer[index + 3] = color.A;  // 透明度
                                            }
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
                        else if (tabiwCMaskSubPixel_rdo.Checked)
                        {
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
                                for (int k = 0; k < tabiwCMaskPixelPanel_pnl.Controls.Count; k += 3)
                                {
                                    if (k + 2 < tabiwCMaskPixelPanel_pnl.Controls.Count)
                                    {
                                        // 獲取三個 Panel
                                        Panel panel1 = tabiwCMaskPixelPanel_pnl.Controls[k] as Panel;
                                        Panel panel2 = tabiwCMaskPixelPanel_pnl.Controls[k + 1] as Panel;
                                        Panel panel3 = tabiwCMaskPixelPanel_pnl.Controls[k + 2] as Panel;

                                        if (panel1 != null && panel2 != null && panel3 != null)
                                        {
                                            //// 根據 Panel 的背景顏色組合成 Color
                                            //int subpixelr = panel1.BackColor.R;
                                            //int subpixelg = panel2.BackColor.G;
                                            //int subpixelb = panel3.BackColor.B;

                                            // 解析 Panel 的座標 (Tag 格式為 "x,y")
                                            string[] coordinates = panel1.Tag.ToString().Split(',');
                                            int x = int.Parse(coordinates[0]) / 3;
                                            int y = int.Parse(coordinates[1]);
                                            for (int i = 0; i < pixelH; i += (int)tabiemHNum_nud.Value)
                                            {
                                                for (int j = 0; j < pixelW; j += (int)tabiemWNum_nud.Value)
                                                {
                                                    // 計算像素在數組中的索引
                                                    //int index = (y * stride) + (x * 4);
                                                    int index = (y * stride) + (x * 4) + i * stride + j * 4;

                                                    // 設置像素顏色
                                                    pixelBuffer[index] = panel3.BackColor.B;      // 藍色
                                                    pixelBuffer[index + 1] = panel2.BackColor.G;  // 綠色
                                                    pixelBuffer[index + 2] = panel1.BackColor.R;  // 紅色
                                                    pixelBuffer[index + 3] = 255;  // 透明度不透明
                                                }
                                            }
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
                    else  //不加速
                    { 
                        pixelW = (int)tabiwCMaskWNum_nud.Value;
                        pixelH = (int)tabiwCMaskHNum_nud.Value;
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
        private void tabiedObjectStrip_rdo_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton targetRdo)
            {
                if (targetRdo.Checked)
                {
                    tabiedMoveH_rdo.Checked = true;
                    tabiedMoveBevel_rdo.Enabled = false;
                }
                else
                {
                    tabiedMoveBevel_rdo.Enabled = true;
                }
            }
        }  //Dymenic物件移動方式選擇
        private void DrawDynamicObject(Graphics g, int width, int height)
        {
            // 依 Dynamic 設定在目前位置畫出物件（動畫與預覽共用）
            using (Brush brush = new SolidBrush(tabiedObjectColor_btn.BackColor))
            {
                if (tabiedObjectBall_rdo.Checked)
                {
                    g.FillEllipse(brush, mypicture.startX, mypicture.startY, mypicture.objectSize * 2, mypicture.objectSize * 2);
                }
                else if (tabiedObjectCube_rdo.Checked)
                {
                    g.FillRectangle(brush, mypicture.startX, mypicture.startY, mypicture.objectSize * 2, mypicture.objectSize * 2);
                }
                else if (tabiedObjectStrip_rdo.Checked)
                {
                    if (tabiedMoveH_rdo.Checked)
                    {
                        g.FillRectangle(brush, mypicture.startX, 0, mypicture.objectSize, height);
                    }
                    else if (tabiedMoveV_rdo.Checked)
                    {
                        g.FillRectangle(brush, 0, mypicture.startY, width, mypicture.objectSize);
                    }
                }
            }
        }  // 畫動態物件
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            // 更新球體位置
            string direction = "b";
            if (tabiedMoveH_rdo.Checked)
            {
                direction = "h";
            }
            else if (tabiedMoveV_rdo.Checked)
            {
                direction = "v";
            }
            else if (tabiedMoveBevel_rdo.Checked)
            {
                direction = "b";
            }
            mypicture.Move(direction, tabiedObjectStrip_rdo.Checked);

            Bitmap bitmap;
            if (timerDynamic.Tag != null && timerDynamic.Tag.ToString() == "openpic")
            {
                if (showimgPicture_pic.Image == null)
                {
                    mainWindow.Instance.timerDynamic.Stop();
                    return;
                }
                bitmap = new Bitmap(showimgPicture_pic.Image);
            }
            else
            {
                if (mypicture.Getpicture() != null)
                {
                    // 繪製動畫
                    //Bitmap bitmap = (Bitmap)mypicture.Getpicture().Clone();
                    bitmap = new Bitmap(mypicture.Getpicture());
                    //bitmap = _pictureWindow.getpicturebox();
                }
                else
                { 
                    bitmap = new Bitmap(int.Parse(mnsW_txt.Text), int.Parse(mnsH_txt.Text));
                }
            }

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                DrawDynamicObject(g, bitmap.Width, bitmap.Height);
            }

            // 更新到 PictureBox
            if (_pictureWindow != null)
            {
                _pictureWindow.UpdatePictureBox(bitmap);
                timerDynamic.Tag = "run";
            }
            else
            {
                throw new InvalidOperationException("沒有圖片可以顯示！");
            }
            if (bitmap != null)
            {
                bitmap.Dispose();
                bitmap = null;
            }
        }  // AnimationTimer_Tick
        // mainWindow操控----------------------------------------------------------------------------------------------
        private void mainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            //MessageBox.Show($"按下了其他鍵：{e.KeyCode}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            // 判斷是否按下鍵
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    FullScreen(sender, e);
                    break;
                case Keys.Space:
                    // 空白鍵：ImgEditor / ImgList 用目前寬高重新產生後全螢幕；DirList 直接全螢幕（正在文字框輸入時不攔截）
                    if (e.Modifiers == Keys.None && !IsTextInputFocused())
                    {
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        RegenerateCurrent();
                        FullScreen(sender, e);
                    }
                    break;
                case Keys.F1:
                    tabControl.SelectedTab = tabImgEditor; // 切換到第一個 Tab 頁面
                    tabimgeFuncList.SelectedIndex = 0; // 選擇第1個功能
                    break;
                case Keys.F2:
                    tabControl.SelectedTab = tabImgEditor; // 切換到第一個 Tab 頁面
                    tabimgeFuncList.SelectedIndex = 1; // 選擇第2個功能
                    break;
                case Keys.F3:
                    tabControl.SelectedTab = tabImgEditor; // 切換到第一個 Tab 頁面
                    tabimgeFuncList.SelectedIndex = 2; // 選擇第3個功能
                    break;
                case Keys.F4:
                    tabControl.SelectedTab = tabImgEditor; // 切換到第一個 Tab 頁面
                    tabimgeFuncList.SelectedIndex = 3; // 選擇第4個功能
                    break;
                case Keys.F5:
                    tabControl.SelectedTab = tabImgEditor; // 切換到第一個 Tab 頁面
                    tabimgeFuncList.SelectedIndex = 4; // 選擇第5個功能
                    break;
                case Keys.F6:
                    tabControl.SelectedTab = tabImgEditor; // 切換到第一個 Tab 頁面
                    tabimgeFuncList.SelectedIndex = 5; // 選擇第6個功能
                    break;
                case Keys.F7:
                    tabControl.SelectedTab = tabImgEditor; // 切換到第一個 Tab 頁面
                    tabimgeFuncList.SelectedIndex = 6; // 選擇第7個功能
                    break;
                case Keys.F11:
                    tabControl.SelectedTab = tabDirList; // 切換到第二個 Tab 頁面
                    break;
                case Keys.F12:
                    tabControl.SelectedTab = tabImgList; // 切換到第三個 Tab 頁面
                    break;
                case Keys.Add: // 數字鍵盤的加號
                case Keys.Oemplus: // 主鍵盤的加號
                    SwitchScreen(+1);
                    break;
                case Keys.Subtract: // 數字鍵盤的減號
                case Keys.OemMinus: // 主鍵盤的減號
                default:
                    //switch (e.KeyValue)
                    //{
                    //    case 229:
                    //        if (mnsScreenlist_cmb.SelectedIndex < mnsScreenlist_cmb.Items.Count - 1)
                    //        {
                    //            mnsScreenlist_cmb.SelectedIndex += 1;
                    //        }
                    //        else
                    //        {
                    //            mnsScreenlist_cmb.SelectedIndex = 0;
                    //        }
                    //        break;
                    //    default:
                    //        break;
                    //}
                    break;
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
                    showimgSize_lbl.Text = $"圖片大小：{mypicture.Getsize()}"; // 更新狀態列顯示圖片大小
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
                // 只接受 .txt（匯出的 config 可能改過檔名），多個檔案時載入第一個
                string filePath = files.FirstOrDefault(file => Path.GetExtension(file).ToLower() == ".txt");
                if (filePath != null)
                {
                    try
                    {
                        LoadConfig(ReadConfigText(filePath), filePath);
                        ssrStatus_lbl.Text = $"Config：{filePath}";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"讀取檔案失敗：{ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("只允許拖入 .txt 的 config 檔案！", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }  // tabilPatternList_lst拖放後執行
        private void tabilPatternList_lst_DragEnter(object sender, DragEventArgs e)
        {
            // 檢查檔案是否為支援的 config.txt  
            bool IsSupportedFile(string filePath)
            {
                return Path.GetExtension(filePath).ToLower() == ".txt";
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
            showimgPicture_pic.Image = Resources.run;
            showimgPicture_pic.SizeMode = PictureBoxSizeMode.Zoom; // 設置圖片縮放模式

            //mypicture._currentBitmap = new Bitmap(mypicture._currentBitmap);
            //using (Graphics g = Graphics.FromImage(mypicture._currentBitmap))
            //{
            //    g.Clear(Color.Red);
            //}
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
        }  // 測試用
        private void testform(Bitmap bitmap)
        {
            Form testform = new Form
            {
                Size = new Size(400, 400),
                BackgroundImage = (Bitmap)bitmap.Clone(),
                BackgroundImageLayout = ImageLayout.Stretch,
                FormBorderStyle = FormBorderStyle.None,
                WindowState = FormWindowState.Maximized
            };
            testform.KeyDown += (s, args) =>
            {
                if (args.KeyCode == Keys.Escape)
                {
                    testform.Close();
                }
            };
            testform.Show();
            testform.TopMost = true; // 設置為最上層窗口
        }  // 測試用
        bool IsImageDisposed(Image image)
        {
            try
            {
                // 嘗試訪問圖片的屬性
                int width = image.Width;
                int height = image.Height;
                return false; // 如果沒有異常，圖片未被處置
            }
            catch (ObjectDisposedException)
            {
                return true; // 圖片已被處置
            }
            catch (ArgumentException)
            {
                return true; // 圖片已被處置或無效
            }
        }  // 測試用
        [DllImport("user32.dll")]
        private static extern IntPtr GetFocus();
        private bool IsTextInputFocused()
        {
            // 文字框、NumericUpDown、可輸入的 ComboBox 需要保留空白鍵
            Control focused = Control.FromChildHandle(GetFocus());
            return focused is TextBoxBase
                || (focused is ComboBox combo && combo.DropDownStyle != ComboBoxStyle.DropDownList);
        }  // 目前焦點是否在可輸入文字的控制項
        protected override bool ProcessKeyPreview(ref Message m)
        {
            if (m.Msg == 0x100 || m.Msg == 0x104) // WM_KEYDOWN 或 WM_SYSKEYDOWN
            {
                Keys keyData = (Keys)m.WParam.ToInt32();
                if (keyData == Keys.Add || keyData == Keys.Subtract)
                {
                    // 如果攔截了，返回 true
                    return true;
                }
            }
            return base.ProcessKeyPreview(ref m);
        }  // 檢查是否覆寫了 ProcessKeyPreview 或 ProcessCmdKey
        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Add || e.KeyCode == Keys.Subtract)
            {
                // 檢查是否有攔截邏輯
            }
        }  // 檢查是否有其他事件處理邏輯攔截了鍵盤事件

        // listbox功能-----------------------------------------------------------------------------------
        private Color switchcolor(string sc)
        {
            if (sc.Contains("."))
            {
                // 這裡的 Name 是從 ListBox 中獲取的顏色名稱
                string[] colorParts = sc.Split('.');
                return Color.FromArgb(int.Parse(colorParts[0]), int.Parse(colorParts[1]), int.Parse(colorParts[2])); // 修改 Form1 的背景顏色
            }
            else if (sc.EndsWith("%gray") || sc.EndsWith("%gary"))  // 例如 50%Gray（相容舊的拼法 Gary）
            {
                int level = Math.Max(0, Math.Min(255, (int)(float.Parse(sc.Split('%')[0]) / 100 * 256) - 1));
                return Color.FromArgb(level, level, level);
            }
            else
            {
                // 根據傳入的顏色名稱來設置顏色；Color.FromName 遇到不認識的名稱不會丟例外，會回傳透明黑
                Color color = Color.FromName(sc);
                if (!color.IsKnownColor)
                {
                    throw new FormatException($"無法識別顏色「{sc}」");
                }
                return color;
            }
        }
        private void funcFrame(string color, string line)
        {
            // 先重設 config 沒有指定的 ImgEditor 狀態，避免沿用使用者手動的設定
            tabiefOther_chk.Checked = false;  // 額外的 H/V 線
            tabiefText_chk.Checked = false;   // 文字模式
            tabiefLineSize_nud.Value = 1;
            mypicture.tag = "";  // 匯入背景圖後 tag 會是 "image"，會畫出匯入的圖而不是純色
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
            // 格式：線種-線色；線種可組合 c 中心、o 外框、n 九點、r 左紅右藍（r 不用線色，只有 r 時可省略線色）
            string[] li = line.Split('-');
            string kinds = li[0].Trim();
            tabiefCenter_chk.Checked = kinds.Contains("c");
            tabiefOutside_chk.Checked = kinds.Contains("o");
            tabiefNine_chk.Checked = kinds.Contains("n");
            tabiefRedBlue_chk.Checked = kinds.Contains("r");
            tabiefLineColor_btn.BackColor = li.Length >= 2 ? switchcolor(li[1]) : Color.White;
        } // listbox控制Frame
        private void funcChess(string num, string option)
        {
            // 選項以 - 分隔：h / v / hv 為翻轉，其他視為格子顏色（預設白色，另一色固定黑色）
            // 例如 "v"、"red"、"255.0.0-v"、"50%gray"
            Color color = Color.White;
            bool hFlip = false, vFlip = false;
            foreach (string token in option.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string t = token.Trim();
                if (t == "h" || t == "v" || t == "hv" || t == "vh")
                {
                    hFlip |= t.Contains("h");
                    vFlip |= t.Contains("v");
                }
                else
                {
                    color = switchcolor(t);
                }
            }
            string[] numParts = num.Split('*');
            if (numParts.Length < 2)
            {
                throw new FormatException($"格數「{num}」格式應為 寬*高，例如 8*8");
            }
            tabiecHNum_nud.Value = int.Parse(numParts[0]);
            tabiecVNum_nud.Value = int.Parse(numParts[1]);
            tabiecHFlip_chk.Checked = hFlip;
            tabiecVFlip_chk.Checked = vFlip;
            // 產生棋盤格時讀的是 tabiecGray_lbl 與灰階捲軸，兩者都要設定，否則會沿用 ImgEditor 的狀態
            tabiecBaseColor_lbl.BackColor = color;
            tabiecGray_lbl.BackColor = color;
            tabiecGray_lbl.ForeColor = (color.R + color.G + color.B) / 3 < 127 ? Color.White : Color.Black;
            tabiecGray_hsc.Maximum = Math.Max(color.R, Math.Max(color.G, color.B));
            tabiecGray_hsc.Value = tabiecGray_hsc.Maximum;
        }  // listbox控制棋盤格
        private void funcText(string colors, string option)
        {
            // 參數：背景色-文字色；選項：文字/大小/字距/行距/串距（大小以後可省略，用預設值）
            string[] colorParts = colors.Split('-');
            funcFrame(colorParts[0], "");  // 背景與框線（重設為無線條、關閉文字模式）
            string[] parts = option.Split('/');
            string text = parts[0].Trim();
            if (text == "") throw new FormatException("沒有文字（選項第一段為文字）");
            if (text.Length > 10) throw new FormatException($"文字最多 10 個字，目前 {text.Length} 個");
            decimal Number(int index, string name, NumericUpDown range)
            {
                if (!decimal.TryParse(parts[index].Trim(), System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out decimal value))
                {
                    throw new FormatException($"{name}「{parts[index]}」不是數字");
                }
                if (value < range.Minimum || value > range.Maximum) throw new FormatException($"{name}需在 {range.Minimum}~{range.Maximum}");
                return value;
            }
            tabiefTextSize_nud.Value = parts.Length > 1 ? Number(1, "大小", tabiefTextSize_nud) : 12;  // 會依比例重設間距
            if (parts.Length > 2) tabiefTextCharGap_nud.Value = Number(2, "字距", tabiefTextCharGap_nud);
            if (parts.Length > 3) tabiefTextLineGap_nud.Value = Number(3, "行距", tabiefTextLineGap_nud);
            if (parts.Length > 4) tabiefTextStringGap_nud.Value = Number(4, "串距", tabiefTextStringGap_nud);
            tabiefTextColor_btn.BackColor = colorParts.Length > 1 ? switchcolor(colorParts[1]) : Color.White;
            tabiefText_txt.Text = text;
            tabiefText_chk.Checked = true;
        }  // listbox控制文字模式
        private void funcDynamic(string colors, string option)
        {
            // 參數：背景色-物件色；選項：形狀(ball/cube/strip)、方向(h/v/b)、size數字、speed數字，以 - 連接
            mypicture.tag = "";  // 不使用匯入的背景圖
            string[] colorParts = colors.Split('-');
            tabiedBackColor_btn.BackColor = switchcolor(colorParts[0]);
            tabiedObjectColor_btn.BackColor = colorParts.Length > 1 ? switchcolor(colorParts[1]) : Color.White;
            RadioButton shape = tabiedObjectBall_rdo;
            RadioButton direction = tabiedMoveBevel_rdo;
            decimal size = 50, speed = 50;
            foreach (string token in option.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string t = token.Trim();
                if (t == "ball") shape = tabiedObjectBall_rdo;
                else if (t == "cube") shape = tabiedObjectCube_rdo;
                else if (t == "strip") shape = tabiedObjectStrip_rdo;
                else if (t == "h") direction = tabiedMoveH_rdo;
                else if (t == "v") direction = tabiedMoveV_rdo;
                else if (t == "b") direction = tabiedMoveBevel_rdo;
                else if (t.StartsWith("size") && decimal.TryParse(t.Substring(4), out decimal s)) size = s;
                else if (t.StartsWith("speed") && decimal.TryParse(t.Substring(5), out decimal v)) speed = v;
                else throw new FormatException($"無法識別選項「{t}」");
            }
            if (size < tabiedObjectSize_nud.Minimum || size > tabiedObjectSize_nud.Maximum)
            {
                throw new FormatException($"size 需在 {tabiedObjectSize_nud.Minimum}~{tabiedObjectSize_nud.Maximum}");
            }
            if (speed < tabiedMoveSpeed_nud.Minimum || speed > tabiedMoveSpeed_nud.Maximum)
            {
                throw new FormatException($"speed 需在 {tabiedMoveSpeed_nud.Minimum}~{tabiedMoveSpeed_nud.Maximum}");
            }
            shape.Checked = true;  // 先設形狀：選長條時會強制水平並停用斜角
            if (shape == tabiedObjectStrip_rdo && direction == tabiedMoveBevel_rdo)
            {
                direction = tabiedMoveH_rdo;  // 長條不能斜角移動
            }
            direction.Checked = true;
            tabiedObjectSize_nud.Value = size;
            tabiedMoveSpeed_nud.Value = speed;
        }  // listbox控制動態
        private void funcGrad(string color, string hv)
        {
            // 先重設 config 沒有指定的 ImgEditor 狀態（起訖灰階、反向）
            tabigFirstLevel_cmb.Text = "0";
            tabigLastLevel_cmb.Text = "255";
            tabigReverse_chk.Checked = false;
            // 格式：[r-]方向-階數-分割[-起始.結束]；r- 為反向（分割需 ≥ 2），起始.結束 為灰階（例：255.0 由亮到暗）
            bool reverse = hv.StartsWith("r-");
            if (reverse) hv = hv.Substring(2);
            string[] hvs = hv.Split('-');
            if (hv.Trim() != "" && hvs.Length < 3)
            {
                throw new FormatException($"漸層選項「{hv}」格式應為 方向-階數-分割，例如 v-256-1");
            }
            if (hvs.Length >= 3)
            {
                tabigHWay_rdo.Checked = hvs[0].Contains("h") ? true : false;
                tabigVWay_rdo.Checked = hvs[0].Contains("v") ? true : false;
                tabigStep_cmb.Text = hvs[1];
                tabigDivid_cmb.Text = hvs[2];
                if (hvs.Length >= 5)
                {
                    throw new FormatException($"起始、結束灰階請用 . 連接，例如 {hvs[0]}-{hvs[1]}-{hvs[2]}-{hvs[3]}.{hvs[4]}");
                }
                if (hvs.Length == 4)
                {
                    // 第 4 段為「起始.結束」灰階（例：255.0 由亮到暗）
                    string[] levels = hvs[3].Split('.');
                    if (levels.Length != 2 || !int.TryParse(levels[0], out int first) || !int.TryParse(levels[1], out int last)
                        || first < 0 || first > 255 || last < 0 || last > 255)
                    {
                        throw new FormatException($"起始.結束灰階「{hvs[3]}」需為兩個 0~255 的整數，用 . 連接，例如 255.0");
                    }
                    tabigFirstLevel_cmb.Text = first.ToString();
                    tabigLastLevel_cmb.Text = last.ToString();
                }
                if (reverse)
                {
                    if (!int.TryParse(hvs[2], out int divisions) || divisions < 2)
                    {
                        throw new FormatException("反向 (r-) 需要畫面等分 ≥ 2");
                    }
                    tabigReverse_chk.Checked = true;
                }
            }
            else if (reverse)
            {
                throw new FormatException("反向 (r-) 後面需要漸層設定，例如 r-h-256-2");
            }
            else
            {
                tabigHWay_rdo.Checked = false;
                tabigVWay_rdo.Checked = false;
                tabigColorBar_rdo.Checked = true;
                tabigStep_cmb.Text = "256";
                tabigDivid_cmb.Text = "1";
            }

            if (color.StartsWith("colorbar"))
            {
                tabigOther_chk.Checked = true;
                if (color.Contains("-h"))
                {
                    tabigOtherHSplit_rdo.Checked = true;
                }
                else if (color.Contains("-v"))
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
        private static bool IsColorMaskOption(string option)
        {
            // 舊寫法只有 0~2 與 . （例：10.01）；出現 / 、空格、英文字母或 % 就是顏色寫法
            return option.IndexOfAny(new[] { '/', ' ', '%' }) >= 0 || option.Any(char.IsLetter);
        }  // 判斷 Mask 選項是否為顏色寫法
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
                if (numParts[0].Contains("p") && IsColorMaskOption(onoff))
                {
                    // 顏色寫法：每格一個顏色（名稱、R.G.B、N%Gray，或 0/1/2），格之間空格、列之間 /
                    tabiemPixel_rdo.Checked = true;
                    int w = (int)tabiemWNum_nud.Value, h = (int)tabiemHNum_nud.Value;
                    string[] rows = onoff.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    if (rows.Length != h)
                    {
                        throw new FormatException($"高度為 {h}，選項需要 {h} 列（以 / 分隔），實際 {rows.Length} 列");
                    }
                    var colors = new Color[w * h];
                    for (int y = 0; y < h; y++)
                    {
                        string[] cells = rows[y].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (cells.Length != w)
                        {
                            throw new FormatException($"寬度為 {w}，第 {y + 1} 列需要 {w} 格（以空格分隔），實際 {cells.Length} 格");
                        }
                        for (int x = 0; x < w; x++)
                        {
                            colors[y * w + x] = cells[x] == "0" ? Color.Black : cells[x] == "1" ? Color.White : cells[x] == "2" ? Color.Gray : switchcolor(cells[x]);
                        }
                    }
                    foreach (Panel targetPanel in tabiemPixelPanel_pnl.Controls.OfType<Panel>())
                    {
                        string[] coordinates = targetPanel.Tag.ToString().Split(',');
                        targetPanel.BackColor = colors[int.Parse(coordinates[1]) * w + int.Parse(coordinates[0])];
                    }
                    return;
                }
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
                else if (numParts[0].Contains("s"))
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
            // 先重設 config 沒有指定的 ImgEditor 狀態，避免沿用使用者手動的設定
            tabiwDiagonal_cmb.SelectedIndex = 0;  // 斜角：無
            tabiwDiagonal_btn.Checked = false;    // 選單原本就是「無」時不會觸發同步，直接重設
            tabiwLineOutside_btn.Checked = false;
            tabiwLineCross_btn.Checked = false;
            tabiwCMaskPixel_rdo.Checked = true;  // 自製圖固定用 Pixel 格，下面的 onoff 以像素排列
            // 視窗大小要在 Custom_btn_Click 產生窗口圖之前設定，否則窗口圖會用舊尺寸產生再被縮放
            tabiwWinSizePercent_rdo.Checked = true;
            tabiwWinSizePercent_nud.Value = sizeloc.Contains("%") ? int.Parse(sizeloc.Split('%')[0]) : 50;
            if (color.Contains("excel"))
            {
                tabiwBackColor_rdo.Checked = true;
                tabiwWinCustom_rdo.Checked = true;
                windowPictureBox_Click(tabiwWinCustom_rdo, EventArgs.Empty);
                tabiwCustom_cmb.SelectedIndex = 0;
                tabiwBack_pic.BackColor = Color.Gray;
                int wnum = 2;
                int hnum = 2;
                string onoff = "10.01";
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
                        onoff = "000111000111000000000111.222222222222222222222222";
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
                MaskPanelCreat(tabiwCMaskPixelClear_btn, EventArgs.Empty);
                for (int i = 0; i < hnum; i++)
                {
                    for (int j = 0; j < wnum; j++)
                    {
                        Panel targetPanel =
                            tabiwCMaskPixelPanel_pnl.Controls[i * wnum + j] as Panel;
                        if (targetPanel != null)
                        {
                            if (i == 1)
                            {
                                i = i;
                            }
                            // 解析 Panel 的座標 (Tag 格式為 "x,y")
                            string[] coordinates = targetPanel.Tag.ToString().Split(',');
                            // 計算在 onoff 字符串中的索引
                            int index = (int.Parse(coordinates[1]) * wnum + int.Parse(coordinates[0]));
                            // 設置 Panel 的顏色
                            if (onoff[index] == '1')
                            {
                                targetPanel.BackColor = Color.White;
                            }
                            else if (onoff[index] == '2')
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
            else if (color.Contains("custom"))
            {
                string[] colorParts = color.Split('-');
                if (colorParts.Length >= 3)
                {
                    int wnum = 0;
                    int hnum = 0;
                    string onoff = "";
                    if (colorParts[2] == "black")
                    {
                        tabiwWinColor_rdo.Checked = true;
                        tabiwWin_pic.Image = null;
                        tabiwWin_pic.BackColor = Color.Black; // 修改 Form1 的背景顏色
                    }
                    else if (colorParts[2] == "gray")
                    {
                        tabiwWinColor_rdo.Checked = true;
                        tabiwWin_pic.Image = null;
                        tabiwWin_pic.BackColor = Color.Gray; // 修改 Form1 的背景顏色
                    }
                    else if (colorParts[2] == "dotw")
                    {
                        tabiwWinCustom_rdo.Checked = true;
                        tabiwWin_pic.Image = null;
                        windowPictureBox_Click(tabiwWinCustom_rdo, EventArgs.Empty);
                        tabiwCustom_cmb.SelectedIndex = 0;
                        wnum = 2;
                        hnum = 2;
                        onoff = "10.01";

                        onoff = onoff.Replace(".", "");
                        tabiwCMaskWNum_nud.Value = wnum;
                        tabiwCMaskHNum_nud.Value = hnum;
                        MaskPanelCreat(tabiwCMaskPixelClear_btn, EventArgs.Empty);
                        for (int i = 0; i < hnum; i++)
                        {
                            for (int j = 0; j < wnum; j++)
                            {
                                Panel targetPanel =
                                    tabiwCMaskPixelPanel_pnl.Controls[i * wnum + j] as Panel;
                                if (targetPanel != null)
                                {
                                    if (i == 1)
                                    {
                                        i = i;
                                    }
                                    // 解析 Panel 的座標 (Tag 格式為 "x,y")
                                    string[] coordinates = targetPanel.Tag.ToString().Split(',');
                                    // 計算在 onoff 字符串中的索引
                                    int index = (int.Parse(coordinates[1]) * wnum + int.Parse(coordinates[0]));
                                    // 設置 Panel 的顏色
                                    if (onoff[index] == '1')
                                    {
                                        targetPanel.BackColor = Color.White;
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
                        tabiwWinColor_rdo.Checked = true;
                        tabiwWin_pic.Image = null;
                        tabiwWin_pic.BackColor = switchcolor(colorParts[2]); // Custom-背景-窗口：第 3 段為窗口顏色
                    }


                    if (colorParts[1] == "dotg")
                    {
                        tabiwBackCustom_rdo.Checked = true;
                        tabiwBack_pic.Image = null;
                        windowPictureBox_Click(tabiwBackCustom_rdo, EventArgs.Empty);
                        tabiwCustom_cmb.SelectedIndex = 0;
                        wnum = 2;
                        hnum = 2;
                        onoff = "20.02";
                    }
                    else if(colorParts[1] == "vstripeg")
                    {
                        tabiwBackCustom_rdo.Checked = true;
                        tabiwBack_pic.Image = null;
                        windowPictureBox_Click(tabiwBackCustom_rdo, EventArgs.Empty);
                        tabiwCustom_cmb.SelectedIndex = 0;
                        wnum = 2;
                        hnum = 2;
                        onoff = "45.54";
                    }
                    else if(colorParts[1] == "vstripew")
                    {
                        tabiwBackCustom_rdo.Checked = true;
                        tabiwBack_pic.Image = null;
                        windowPictureBox_Click(tabiwBackCustom_rdo, EventArgs.Empty);
                        tabiwCustom_cmb.SelectedIndex = 0;
                        wnum = 2;
                        hnum = 2;
                        onoff = "67.76";
                    }
                    else if (colorParts[1] == "dotdell")
                    {
                        tabiwBackCustom_rdo.Checked = true;
                        tabiwBack_pic.Image = null;
                        windowPictureBox_Click(tabiwBackCustom_rdo, EventArgs.Empty);
                        tabiwCustom_cmb.SelectedIndex = 0;
                        wnum = 2;
                        hnum = 2;
                        onoff = "30.03";
                    }
                    else if (colorParts[1] == "roww")
                    {
                        tabiwBackCustom_rdo.Checked = true;
                        tabiwBack_pic.Image = null;
                        windowPictureBox_Click(tabiwBackCustom_rdo, EventArgs.Empty);
                        tabiwCustom_cmb.SelectedIndex = 0;
                        wnum = 2;
                        hnum = 2;
                        onoff = "11.00";
                    }
                    else
                    {
                        tabiwBackColor_rdo.Checked = true;
                        tabiwBack_pic.Image = null;
                        tabiwBack_pic.BackColor = switchcolor(colorParts[1]); // Custom-背景-窗口：第 2 段為背景顏色
                    }
                    if (tabiwBackCustom_rdo.Checked == true && onoff != "")
                    {
                        onoff = onoff.Replace(".", "");
                        tabiwCMaskWNum_nud.Value = wnum;
                        tabiwCMaskHNum_nud.Value = hnum;
                        MaskPanelCreat(tabiwCMaskPixelClear_btn, EventArgs.Empty);
                        for (int i = 0; i < hnum; i++)
                        {
                            for (int j = 0; j < wnum; j++)
                            {
                                Panel targetPanel =
                                    tabiwCMaskPixelPanel_pnl.Controls[i * wnum + j] as Panel;
                                if (targetPanel != null)
                                {
                                    if (i == 1)
                                    {
                                        i = i;
                                    }
                                    // 解析 Panel 的座標 (Tag 格式為 "x,y")
                                    string[] coordinates = targetPanel.Tag.ToString().Split(',');
                                    // 計算在 onoff 字符串中的索引
                                    int index = (int.Parse(coordinates[1]) * wnum + int.Parse(coordinates[0]));
                                    // 設置 Panel 的顏色
                                    switch (onoff[index])
                                    {
                                        case '0':
                                            targetPanel.BackColor = Color.Black;
                                            break;
                                        case '1':
                                            targetPanel.BackColor = Color.White;
                                            break;
                                        case '2':
                                            targetPanel.BackColor = Color.Gray;
                                            break;
                                        case '3':
                                            targetPanel.BackColor = Color.FromArgb(0, 127, 127);
                                            break;
                                        case '4':
                                            targetPanel.BackColor = Color.FromArgb(127, 0, 127);
                                            break;
                                        case '5':
                                            targetPanel.BackColor = Color.FromArgb(0, 127, 0);
                                            break;
                                        case '6':
                                            targetPanel.BackColor = Color.FromArgb(255, 0, 255);
                                            break;
                                        case '7':
                                            targetPanel.BackColor = Color.FromArgb(0, 255, 0);
                                            break;
                                        default:
                                            targetPanel.BackColor = Color.Black;
                                            break;
                                    }
                                }
                            }
                        }
                        Custom_btn_Click(tabiwCustom_btn, EventArgs.Empty);
                    }
                }
            }
            else
            {
                string[] colorParts = color.Split('-');
                if (colorParts.Length >= 2)
                {
                    tabiwBackColor_rdo.Checked = true;
                    tabiwWinColor_rdo.Checked = true;
                    tabiwBack_pic.Image = null;
                    tabiwBack_pic.BackColor = switchcolor(colorParts[0]); // 修改 Form1 的背景顏色
                    tabiwWin_pic.Image = null;
                    tabiwWin_pic.BackColor = switchcolor(colorParts[1]); // 修改 Form1 的背景顏色
                }
            }

            if (sizeloc.Contains("xy"))  //loc center
            {
                tabiwWinLocPixcel_rdo.Checked = true;
                int x = 0;
                int y = 0;
                if (sizeloc.Contains("q"))  //loc center
                {
                    if (sizeloc.Contains("++"))
                    {
                        x = int.Parse(mnsW_txt.Text) / 2;
                        y = 0;
                    }
                    else if (sizeloc.Contains("-+"))
                    {
                        x = 0;
                        y = 0;
                    }
                    else if (sizeloc.Contains("--"))
                    {
                        x = 0;
                        y = int.Parse(mnsH_txt.Text) / 2;
                    }
                    else if (sizeloc.Contains("+-"))
                    {
                        x = int.Parse(mnsW_txt.Text) / 2;
                        y = int.Parse(mnsH_txt.Text) / 2;
                    }
                } 
                else
                {
                    x = int.Parse(sizeloc.Split('_')[1].Split('.')[0]);
                    y = int.Parse(sizeloc.Split('_')[1].Split('.')[1]);
                }
                tabiwWinLocPixcelX_nud.Value = x;
                tabiwWinLocPixcelY_nud.Value = y;
            }
            else if(sizeloc.Contains("two"))  //loc outside
            {
                tabiwWinLocTwo_rdo.Checked = true;
            }
            else  //loc center
            {
                tabiwWinLocCenter_rdo.Checked = true;
            }
        } // listbox控制窗口

    }
}
