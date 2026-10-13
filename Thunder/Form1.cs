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
            InitConfigSavePrompt();
            InitWindowOverlayList();
            InitEditorToConfig();
            InitMaskQuickButtons();
            InitFrameTextMode();
            InitFrameRedBlue();
            InitGradientReverse();
            InitGradientExtras();
            InitGradientPlus();
            InitWindowDiagonal();
            InitWindowTabs();
            InitQuickButtons();
            InitResizeMode();
            InitPreviewPanel();
            InitDirListReload();
            InitMassTooltip();
            InitMaskZoom();
            InitImgListExport();
            InitTestPatternPage();
            InitTestPatternPlus();
            InitDynamicBlur();
            InitDynamicPlus();
            InitImageDiff();
            InitImgListTools();
            InitImgListView();
            InitMaskTemplates();
            InitMontage();
            InitSubPixelOrder();
            InitUserMaskControls();
            InitMultiScreen();
            InitDisplayMenu();
            InitMainLayout();  // 最外層改用固定欄寬的表格
            Text = $"PatternMagic_v{System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}";  // 標題帶版本號（取自 AssemblyInfo）
            InitLanguage();  // 最後執行：上面建立的介面一起翻譯
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
            // 預設輸出到主螢幕以外的第一個螢幕（由左到右）；只有一個螢幕時用主螢幕
            Screen second = OtherScreens().FirstOrDefault();
            if (second != null && mnsScreenlist_cmb.Items.Contains(second.DeviceName))
            {
                mnsScreenlist_cmb.SelectedItem = second.DeviceName;
            }
            else if (mnsScreenlist_cmb.Items.Contains(Screen.PrimaryScreen.DeviceName))
            {
                mnsScreenlist_cmb.SelectedItem = Screen.PrimaryScreen.DeviceName;
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
                else if (direction == "b" || direction == "r")
                {
                    startX += moveSpeedX;
                    startY += moveSpeedY;
                }
                int speedX = moveSpeedX, speedY = moveSpeedY;  // 反彈前的速度（判斷有沒有碰到邊界）
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
                if (direction == "r" && (speedX != moveSpeedX || speedY != moveSpeedY))
                {
                    // 隨機方向：碰到邊界時換一個隨機角度（20°~70°），速度不變，方向仍離開邊界
                    int speed = Math.Max(Math.Abs(speedX), Math.Abs(speedY));
                    double angle = (20 + DirectionRandom.NextDouble() * 50) * Math.PI / 180;
                    moveSpeedX = Math.Sign(moveSpeedX == 0 ? 1 : moveSpeedX) * Math.Max(1, (int)Math.Round(speed * Math.Cos(angle)));
                    moveSpeedY = Math.Sign(moveSpeedY == 0 ? 1 : moveSpeedY) * Math.Max(1, (int)Math.Round(speed * Math.Sin(angle)));
                    startX = Math.Max(0, Math.Min(_width - objectSize * 2, startX));
                    startY = Math.Max(0, Math.Min(_height - objectSize * 2, startY));
                }
            }
            private static readonly Random DirectionRandom = new Random();
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
            if (tabimgeTest != null) tabimgeTest.Visible = false;

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
                case "Testpattern":
                    tabimgeTest.Visible = true;
                    tabimgeTest.Dock = DockStyle.Fill;
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
                ssrStatus_lbl.Text = _dirListFolder;
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
                case "Testpattern":
                    type = "test";
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
                            if (tabiefTextBoom_chk.Checked)
                            {
                                DrawBoomText(g, tileText, tabiefTextColor_btn.BackColor, width, height, size => FrameTextFont(size, GraphicsUnit.Point, "Arial", FontStyle.Regular));  // 爆炸模式
                            }
                            else
                            using (Font tileFont = FrameTextFont((float)tabiefTextSize_nud.Value, GraphicsUnit.Pixel, "Microsoft JhengHei", FontStyle.Bold))
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
                        if (tabigSaturation_chk.Checked)
                        {
                            CurrentBitmap = DrawSaturationLadder(width, height);  // 飽和度階梯
                            mypicture.Setpicture(CurrentBitmap);
                            break;
                        }
                        if (AllLevelGrid)
                        {
                            // 全畫面格狀全階：每一階一格並標示階數
                            CurrentBitmap = DrawAllLevels(width, height, int.Parse(tabigAll_cmb.Text));
                            mypicture.Setpicture(CurrentBitmap);
                            break;
                        }
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
                            int stepNum = Math.Min(GradientStepCount, divisionSize); // 獲取漸層階數（依漸層條件的全階不超過漸層分幾階）
                            int startColorValue = int.Parse(tabigFirstLevel_cmb.Text); // 開始顏色值  
                            int endColorValue = int.Parse(tabigLastLevel_cmb.Text); // 結束顏色值  
                            // 先算好每一階的數值再逐列合成（起點色為雙色漸層）；底色圖用完就釋放
                            Bitmap rendered = GradientRadial
                                ? RenderRadialGradient(backBitmap, startColorValue, endColorValue, GradientStepCount, GradientCurve, GradientFromColor)
                                : RenderGradient(backBitmap, startColorValue, endColorValue, tabigHWay_rdo.Checked, stepNum, divisions, tabigReverse_chk.Checked, GradientCurve, GradientFromColor);
                            backBitmap.Dispose();
                            backBitmap = rendered;
                            if (AllLevelGradient && !GradientRadial)
                            {
                                DrawGradientLabels(backBitmap, tabigHWay_rdo.Checked);
                            }
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
                        DrawMontage(g, width, height);  // 蒙太奇（config 也可指定，所以一律畫）
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
                                                            Color combinedColor = CombineSubPixels(panel1, panel2, panel3);  // 三格各自的色版合成一個像素（RGB / BGR 皆可）
                                                            pixelBuffer[index] = combinedColor.B;      // 藍色
                                                            pixelBuffer[index + 1] = combinedColor.G;  // 綠色
                                                            pixelBuffer[index + 2] = combinedColor.R;  // 紅色
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
                                                    Color combinedColor = CombineSubPixels(panel1, panel2, panel3);  // 三格各自的色版合成一個像素（RGB / BGR 皆可）

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
                                                Color combinedColor = CombineSubPixels(panel1, panel2, panel3);  // 三格各自的色版合成一個像素（RGB / BGR 皆可）

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
                    case "test":
                        DrawTestPattern(g, width, height);  // Gamma / 黑位白位 / 邊緣刻度
                        mypicture.Setpicture(CurrentBitmap);
                        UpdateTestBlink(width, height);  // Clipping 閃爍的另一張
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
                        DrawDynamicBackgroundPlus(g, width, height);  // Overdrive：多種灰階的橫帶
                        mypicture.SetDymanic((int)tabiedMoveSpeed_nud.Value, (int)tabiedMoveSpeed_nud.Value, (int)tabiedObjectSize_nud.Value, 0, 0);
                        ResetDynamicPlus(width);

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
                if (sender is Button targetbutton && targetbutton == tabieaImport_btn)
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
            _pictureWindow.SetBlink(BlinkAlternateFor(mypicture.Getpicture()));  // Clipping 閃爍
            _pictureWindow.Caption = CurrentImageName();  // 全螢幕左上角顯示的名稱

            if (_pictureWindow != null && !_pictureWindow.IsDisposed)  //存在才執行
            {
                // 換螢幕或第一次顯示時重新擺放（自動調整縮放時位置為實際像素座標）
                if (!_pictureWindow.Visible || _pictureWindow.Location != FullScreenBounds(secondScreen).Location)
                {
                    PlaceFullScreenWindow(_pictureWindow, secondScreen);
                }
                if (!_pictureWindow.Visible)
                {
                    _pictureWindow.Show();
                    _pictureWindow.BringToFront();
                }
                _pictureWindow.WindowState = FormWindowState.Maximized;
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
            // 獲取選擇的螢幕（重建清單時會暫時沒有選擇）
            if (mnsScreenlist_cmb.SelectedItem == null) return;
            string selectedScreen = mnsScreenlist_cmb.SelectedItem.ToString();
            Screen[] screens = Screen.AllScreens;
            // 根據選擇的螢幕更新寬度與高度
            foreach (Screen screen in screens)
            {
                if (screen.DeviceName == selectedScreen)
                {
                    mnsW_txt.Text = ScreenPixelSize(screen).Width.ToString();
                    mnsH_txt.Text = ScreenPixelSize(screen).Height.ToString();
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
                Process.Start("explorer.exe", $"\"{(tabControl.SelectedTab == tabDirList ? _dirListFolder : Application.StartupPath)}\"");  // DirList 開圖片資料夾，其他開程式資料夾
            }
            catch (Exception ex)
            {
                MessageBox.Show($"無法開啟資料夾：{ex.Message}");
            }
        }  // 狀態列點擊開啟程式資料夾
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern int StrCmpLogicalW(string x, string y);  // 與檔案總管相同的自然排序 (1,2,...,9,10)
        private void tabilPatternList_lst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is ListView targetList && !_movingConfigLine)
            {
                if (targetList.SelectedItems.Count > 0)
                {
                    FillConfigEditor(targetList.SelectedItems[0]);  // 帶入下方編輯區
                    _configDraftFocused = false;  // 選清單後 Generate 不再預覽編輯區草稿
                    _shownItem = null;
                    ListViewItem item = targetList.SelectedItems[0];
                    string line = item.Tag.ToString();
                    var errors = new List<string>();
                    if (ApplyConfigLine(line, errors))
                    {
                        MarkConfigItem(item, null);
                        AddRecent(RecentItem.FromConfig(line));
                    }
                    else
                    {
                        string error = errors.FirstOrDefault() ?? "無法產生圖片";
                        MarkConfigItem(item, error);  // 修改後才發現的錯誤也標紅
                        MessageBox.Show(error, "Config 錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }  // ImgList選擇圖片
        private bool ApplyConfigLine(string line, List<string> errors = null, bool parseOnly = false)
        {
            // 套用一行 config 並產生預覽圖（ImgList 選取、編輯區預覽、最近使用共用）；成功回傳 true
            // errors 不為 null 時錯誤記錄在清單（批次匯出用），否則跳出訊息；parseOnly 只檢查格式、不產生圖片
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
            RefreshUserMasks();  // 自訂 mask 名稱依 ImgList 目前內容
            string[] patternVar = StripDwell(line, out int? dwell).Split(',');  // 結尾的 @秒數s 是幻燈片停留時間，不屬於各類型的選項
            if (dwell != null && (dwell < MinDwell || dwell > MaxDwell))
            {
                Report($"「{patternVar[0].Trim()}」的停留時間需為 0.1~3600 秒，例：@5s", "Config 錯誤");
                return false;
            }
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
                    case "usermask":
                        // 自訂 mask：檢查名稱與格式，畫面與 Mask 相同
                        CheckUserMaskLine(patternVar[0], patternVar[2], patternVar[3]);
                        tabimgeFuncList.SelectedIndex = 4;
                        funcMask(trimlower(patternVar[2]), trimlower(patternVar[3]));
                        break;
                    case "dynamic":
                        tabimgeFuncList.SelectedIndex = tabimgeFuncList.Items.IndexOf("Dynamic");
                        funcDynamic(trimlower(patternVar[2]), patternVar[3].Trim());  // 選項保留大小寫（跑馬燈文字），比對時才轉小寫
                        break;
                    case "test":
                        tabimgeFuncList.SelectedIndex = tabimgeFuncList.Items.IndexOf("Testpattern");
                        funcTest(trimlower(patternVar[2]));
                        break;
                    case "text":
                        tabimgeFuncList.SelectedIndex = 0;  // 文字模式屬於 Frame
                        funcText(trimlower(patternVar[2]), patternVar[3].Trim());  // 文字保留大小寫
                        break;
                    case "image":
                        // 圖檔：直接讀檔縮放到目前寬高，不經過 ImgEditor
                        if (!LoadConfigImage(patternVar[2].Trim(), trimlower(patternVar[3]), width, height, parseOnly)) return true;  // 只檢查格式
                        showimgPicture_pic.Image = mypicture.Getpicture();
                        showimgPicture_pic.SizeMode = PictureBoxSizeMode.Zoom;
                        ShowPreviewInfo(width, height, line);
                        return true;
                    default:
                        Report($"「{patternVar[0].Trim()}」的類型「{patternVar[1].Trim()}」不支援！");
                        return false;
                }
                if (parseOnly) return true;  // 格式檢查到此為止（產生圖片較慢）
                string tag = patternVar[0];
                _generatingFromConfig = true;  // Dynamic 只預覽（空白鍵 / Esc 全螢幕時才播放）；Window 不疊加 ImgEditor 的清單
                try
                {
                    GenerateImage(type == "text" ? "solid" : type == "usermask" ? "mask" : type, width, height, tag); // 重新生成圖片（Text 由 Frame 產生、自訂 mask 由 Mask 產生）
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
            if (IsFiltering) return;  // 搜尋中保留原本的編號
            SyncGroupedOrder();
            for (int i = 0; i < tabilPatternList_lst.Items.Count; i++)
            {
                tabilPatternList_lst.Items[i].Text = (i + 1).ToString();
            }
            UpdateGroupHeaders();
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
            ClearSearchSilently();  // 換 config 時清除搜尋
            PopulateListBox(configText);
            _configUndo.Clear();  // 換了 config 檔，復原紀錄不再適用
            _savedConfigLines = ConfigLinesWithGroups();  // 之後與這份比較，判斷關閉前要不要提醒儲存
            tabilPatternList_lst.Tag = source;  // 記錄 config 來源
            if (cfgFile_cmb != null) RefreshConfigFiles();  // 檔案下拉選單顯示目前來源
            // 載入時不逐行檢查（行數多時很花時間）：點選項目時才檢查並標紅，右鍵「檢查全部設定」可一次檢查
            if (showimgPicture_pic != null) showimgPicture_pic.Image = null;
            if (showimgSize_lbl != null) showimgSize_lbl.Text = "";
            if (showimgConfig_lbl != null) ShowConfigNote(null);
            ssrStatus_lbl.Text = $"Config：{source}（{tabilPatternList_lst.Items.Count} 行，點選時檢查格式；右鍵可檢查全部設定）";
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
            FillPatternList(configfile.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries));  // 含 #== 群組 == 標題
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
            // 跳過收合群組裡的項目（幻燈片只播放展開的群組）；全部收合時維持原本的選擇
            for (int tries = 0; tries < count && !IsSlideVisible(tabilPatternList_lst.Items[next]); tries++) next = ((next + step) % count + count) % count;
            if (!IsSlideVisible(tabilPatternList_lst.Items[next])) return;
            tabilPatternList_lst.Items[next].Selected = true;
            tabilPatternList_lst.Items[next].Focused = true;
            tabilPatternList_lst.EnsureVisible(next);
        }  // ImgList 上一個 / 下一個（全螢幕左右鍵使用）
        // mainWindow操控----------------------------------------------------------------------------------------------
        private void mainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            //MessageBox.Show($"按下了其他鍵：{e.KeyCode}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            // ImgList 分頁 Ctrl+Z 復原清單變更（文字框裡的 Ctrl+Z 維持原本的復原文字）
            if (e.Control && e.KeyCode == Keys.Z && tabControl.SelectedTab == tabImgList && !IsTextInputFocused())
            {
                UndoConfigEdit();
                e.Handled = e.SuppressKeyPress = true;
                return;
            }
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
                case Keys.F8:
                    tabControl.SelectedTab = tabImgEditor;
                    tabimgeFuncList.SelectedItem = "Testpattern";  // 第 8 個功能
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
            if (DropConfigItem(e)) return;  // 清單內拖拉排序
            if (e.Data.GetData(DataFormats.FileDrop) is string[] dropped && !dropped.Any(file => Path.GetExtension(file).ToLower() == ".txt") && dropped.Any(IsImageFile))
            {
                AddImageFilesToList(dropped);  // 只拖入圖檔：每張加一行 Image
                return;
            }
            // 獲取拖入的檔案  
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                // 只接受 .txt（匯出的 config 可能改過檔名），多個檔案時載入第一個
                string filePath = files.FirstOrDefault(file => Path.GetExtension(file).ToLower() == ".txt");
                if (filePath != null)
                {
                    if (!ConfirmDiscardConfig()) return;  // 目前的 config 有變更：先問要不要儲存
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
            if (e.Data.GetDataPresent(typeof(ListViewItem)))
            {
                e.Effect = DragDropEffects.Move;  // 清單內拖拉排序
                return;
            }
            // 檢查檔案是否為支援的 config.txt  
            bool IsSupportedFile(string filePath)
            {
                return Path.GetExtension(filePath).ToLower() == ".txt" || IsImageFile(filePath);  // config 或圖檔（加成 Image 行）
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


    }
}
