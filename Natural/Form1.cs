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
            if (mnsScreenlist_cmb.Items.Count > 1)
            {
                mnsScreenlist_cmb.SelectedIndex = 1;
            }
            else if (mnsScreenlist_cmb.Items.Count > 0)
            {
                mnsScreenlist_cmb.SelectedIndex = 0;
            }
            tabimgeFuncList.SelectedIndex = 4; // �w�]��ܲĤ@�Ӷ���
        }
        public MyPicture mypicture { get; private set; }

        public class MyPicture
        {
            private Bitmap _currentBitmap;
            public string tag;
            //private int _width;
            private int _width;
            private int _height;
            private Mainwindow m1 { get; }
            public MyPicture(Mainwindow m0)
            {
                m1 = m0 ?? throw new ArgumentNullException(nameof(m0));
                _width = int.Parse(m1.mnsW_txt.Text);
                _height = int.Parse(m1.mnsH_txt.Text);
                _currentBitmap = new Bitmap(_width, _height);
                tag = "";
            }
            public void SaveImage(string filename = "")
            {
                if (_currentBitmap == null)
                {
                    MessageBox.Show("�S���Ϥ��i�H�x�s�I");
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
                                    throw new InvalidOperationException("�S���Ϥ��i�H�x�s�I");
                                }

                                string extension = Path.GetExtension(saveFileDialog.FileName).ToLower();
                                ImageFormat imageFormat = extension switch
                                {
                                    ".jpg" => ImageFormat.Jpeg,
                                    ".png" => ImageFormat.Png,
                                    ".bmp" => ImageFormat.Bmp,
                                    _ => throw new NotSupportedException("���䴩���ɮ׮榡�I")
                                };
                                _currentBitmap.Save(saveFileDialog.FileName, imageFormat);

                                MessageBox.Show("�Ϥ��w�x�s���\�I");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"�Ϥ��x�s���ѡG{ex.Message}");
                            }
                        }
                    }
                }
            }

            public Image OpenImage()
            {

                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    // �]�w�ɮ׹L�o���A�u���\��� JPG�BPNG �M BMP �榡���Ϥ�
                    openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                    openFileDialog.Title = "��ܹϤ��ɮ�";

                    // ��ܹ�ܮب��ˬd�Τ�O�_��ܤF�ɮ�
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            // �ϥ� Image.FromFile ���J�Ϥ�
                            _currentBitmap = (Bitmap)Image.FromFile(openFileDialog.FileName);
                            return _currentBitmap; // �^�ǹϤ�
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
                                    return _currentBitmap; // �^�ǹϤ�
                                }
                            }
                            MessageBox.Show("�L�k���J�Ϥ��A�i��O�Ϥ��L�j�ή榡���䴩�I(�i�H�t�s��JPEG�BBMP�BPNG)");
                        }
                        catch (Exception ex)
                        {
                            // �p�G���J���ѡA��ܿ��~�T��
                            MessageBox.Show($"�L�k���J�Ϥ��G{ex.Message}", "���~", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }

                return null; // �p�G�Τ�����θ��J���ѡA�^�� null
            }
            public Bitmap Getpicture()
            {
                return _currentBitmap;
            }

            public string Getsize()
            {
                _width = _currentBitmap.Width;
                _height = _currentBitmap.Height;
                return $"{_width} x {_height}"; // ��s�Ϥ��j�p
            }

            public Image Setpicture(Image image)
            {
                _currentBitmap = image as Bitmap;
                if (_currentBitmap == null)
                {
                    throw new InvalidCastException("�L�k�N Image �ഫ�� Bitmap�C�нT�O���Ѫ��Ϥ��O Bitmap �����C");
                }
                tag = "image";
                return _currentBitmap; // �^�ǹϤ�
            }
        }

        private void cmbFunclist_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ���éҦ� GroupBox
            tabimgeFrame.Visible = false;
            tabimgeGradient.Visible = false;
            tabimgeChess.Visible = false;
            tabimgeWindow.Visible = false;
            tabimgeMask.Visible = false;
            tabimgeAdjust.Visible = false;
            tabimgeMass.Visible = false;

            // �ھڿ�ܪ�������ܹ����� GroupBox
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
                    tabigVWay_rdo.Checked = true; // �w�]��� Gradient �覡
                    tabigStep_cmb.Text = "256"; // �w�]��� 256 ��
                    tabigDivid_cmb.Text = "1"; // �w�]��� 1 ��
                    tabigFistLevel_cmb.Text = "0"; // �w�]�q 0 ���}�l
                    tabigLastLevel_cmb.Text = "255"; // �w�]�� 255 ������
                    break;
                case "Chess":
                    tabimgeChess.Visible = true;
                    tabimgeChess.Dock = DockStyle.Fill;
                    break;
                case "Window":
                    tabimgeWindow.Visible = true;
                    tabimgeWindow.Dock = DockStyle.Fill;
                    //radioButton11.Checked = true; // �w�]��ܯ¦�I��
                    //radioButton16.Checked = true; // �w�]��ܯ¦���
                    tabiwWinSizePercent_rdo.Checked = true; // �w�]��ܦʤ���j�p
                    tabiwWinSizePercent_nud.Value = 50; // �w�]��� 50% �j�p
                    tabiwWinLocCenter_rdo.Checked = true; // �w�]��ܸm����m
                    tabiwWinSizePixelW_nud.Maximum = int.Parse(mnsW_txt.Text);
                    tabiwWinSizePixelH_nud.Maximum = int.Parse(mnsH_txt.Text);
                    tabiwWinLocPixcelX_nud.Maximum = int.Parse(mnsW_txt.Text);
                    tabiwWinLocPixcelY_nud.Maximum = int.Parse(mnsH_txt.Text);
                    break;
                case "Mask(FlickerPattern)":
                    tabimgeMask.Visible = true;
                    tabimgeMask.Dock = DockStyle.Fill;
                    tabiemPixel_rdo.Checked = true;
                    tabiemHNum_nud.Value = 2;
                    tabiemWNum_nud.Value = 2;
                    MaskPanelCreat(sender, e);
                    break;
                case "ImageAdjust":
                    tabimgeAdjust.Visible = true;
                    tabimgeAdjust.Dock = DockStyle.Fill;
                    tabieaResize_rdo.Checked = true;
                    break;
                case "MassImage":
                    tabimgeMass.Visible = true;
                    tabimgeMass.Dock = DockStyle.Fill;
                    break;
                default:
                    // �p�G�S���ǰt�����ءA�h����ܥ��� GroupBox
                    break;
            }
        }

        private void Generate_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(mnsW_txt.Text, out int width) || !int.TryParse(mnsH_txt.Text, out int height))
            {
                MessageBox.Show("�п�J���Ī��e�׻P���סI");
                return;
            }

            // �ھ� cmbFunclist ����ܨӨM�w type  
            string type = tabimgeFuncList.Text switch
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
                showimgPicture_pic.Image = mypicture.Getpicture();
                showimgPicture_pic.SizeMode = PictureBoxSizeMode.Zoom;
                showimgSize_btn.Text = $"��e�Ϥ��j�p�G{mnsW_txt.Text} x {mnsH_txt.Text}"; // ��s���A�C��ܷ�e�Ϥ��j�p  
            }
            catch (Exception ex)
            {
                MessageBox.Show($"�Ϥ��ͦ����ѡG{ex.Message}");
            }
        }

        private void FullScreen(object sender, EventArgs e)
        {
            // �����ܪ��ù�  
            string selectedScreen = mnsScreenlist_cmb.SelectedItem?.ToString();
            Screen secondScreen = Screen.AllScreens.FirstOrDefault(screen => screen.DeviceName == selectedScreen);

            // �Ыإ��ù����  
            Form fullScreenForm = new Form
            {
                FormBorderStyle = FormBorderStyle.None, // �L���  
                WindowState = FormWindowState.Maximized, // �̤j��  
                StartPosition = FormStartPosition.Manual, // ��ʳ]�m��m  
                Location = secondScreen.Bounds.Location, // �]�m���ܪ��ù�  
                Size = secondScreen.Bounds.Size, // �]�m�j�p����ܿù��j�p  
                //BackColor = Color.Black, // �I���]�m���¦�  
                KeyPreview = true // �ҥΫ���ƥ�w��  
            };

            // �Ыؤ@�� PictureBox �ó]�m�Ϥ�  
            PictureBox fullScreenPictureBox = new PictureBox
            {
                Dock = DockStyle.Fill, // ��R��Ӫ��  
                Image = showimgPicture_pic.Image, // �ϥέ�l PictureBox ���Ϥ�  
                SizeMode = PictureBoxSizeMode.Zoom // �Y��Ϥ��H�A���ù�  
            };

            // �N PictureBox �K�[����  
            fullScreenForm.Controls.Add(fullScreenPictureBox);

            // �����h�X���ù�  
            fullScreenPictureBox.DoubleClick += (s, args) => fullScreenForm.Close();

            // ���U ESC ��h�X���ù�  
            fullScreenForm.KeyDown += (s, args) =>
            {
                if (args.KeyCode == Keys.Escape)
                {
                    fullScreenForm.Close();
                }
            };

            // ��ܥ��ù����  
            fullScreenForm.ShowDialog();
        }

        private void Screenlist(object sender, EventArgs e)
        {
            // ����Ҧ��ù�
            Screen[] screens = Screen.AllScreens;
            // �M�ŤU�Կ��
            mnsScreenlist_cmb.Items.Clear();
            // �K�[�ù��ﶵ��U�Կ��
            foreach (Screen screen in screens)
            {
                mnsScreenlist_cmb.Items.Add(screen.DeviceName);
            }


        }
        private void menuToolStripCmb_SelectedIndexChanged(object sender, EventArgs e)
        {

            // �����ܪ��ù�
            string selectedScreen = mnsScreenlist_cmb.SelectedItem.ToString();
            Screen[] screens = Screen.AllScreens;
            // �ھڿ�ܪ��ù���s�e�׻P����
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
                ListViewItem selectedItem = tabdlPatternList_lvw.SelectedItems[0]; // ����襤�� ListViewItem  
                string imagePath = selectedItem.Tag as string; // �q Tag �����X�Ϥ����|  

                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    // ���J��l�Ϥ��� PictureBox  
                    showimgPicture_pic.Image = Image.FromFile(imagePath);
                    showimgPicture_pic.SizeMode = PictureBoxSizeMode.Zoom;
                    showimgSize_btn.Text = $"��e�Ϥ��j�p�G{selectedItem.SubItems[2].Text} x {selectedItem.SubItems[3].Text}"; // ��s���A�C��ܷ�e�Ϥ��j�p  
                }
                else
                {
                    MessageBox.Show($"�䤣��Ϥ��G{imagePath}");
                }
            }
        }

        private void PopulateListView()
        {
            // �M�Ų{������
            tabdlPatternList_lvw.Items.Clear();
            tabdlPatternList_lvw.Groups.Clear();
            tabdlPatternList_lvw.LargeImageList = new ImageList
            {
                ImageSize = new Size(90, 90) // �]�m�Y���Ϥj�p
            };

            // �w�q����
            ListViewGroup jpgGroup = new ListViewGroup("JPG �Ϥ�", HorizontalAlignment.Left);
            ListViewGroup pngGroup = new ListViewGroup("PNG �Ϥ�", HorizontalAlignment.Left);
            ListViewGroup bmpGroup = new ListViewGroup("BMP �Ϥ�", HorizontalAlignment.Left);

            tabdlPatternList_lvw.Groups.Add(jpgGroup);
            tabdlPatternList_lvw.Groups.Add(pngGroup);
            tabdlPatternList_lvw.Groups.Add(bmpGroup);

            // ����{���Ҧb����Ƨ�
            string folderPath = Application.StartupPath;

            // �����Ƨ������Ҧ� JPG�BPNG �M BMP �ɮ�
            string[] imageFiles = Directory.GetFiles(folderPath, "*.*")
                                            .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                                           file.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                                                           file.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
                                            .ToArray();

            // �K�[�Ϥ��� ListView
            foreach (string imagePath in imageFiles)
            {
                try
                {
                    // �[���Ϥ�
                    using (Image img = Image.FromFile(imagePath))
                    {
                        string fileName = Path.GetFileName(imagePath);
                        string extension = Path.GetExtension(imagePath).ToLower();
                        int width = img.Width;
                        int height = img.Height;

                        // �K�[�� ImageList
                        tabdlPatternList_lvw.LargeImageList.Images.Add(fileName, new Bitmap(img));

                        // �Ы� ListViewItem
                        ListViewItem item = new ListViewItem(fileName)
                        {
                            Tag = imagePath, // �s�x������|
                            ImageKey = fileName // ���� ImageList �����
                        };

                        // �K�[�l���ء]�ɮצW�١B�e�B���^
                        item.SubItems.Add(fileName); // �W��
                        item.SubItems.Add(width.ToString()); // �e
                        item.SubItems.Add(height.ToString()); // ��

                        // �ھڰ��ɦW����
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

                        // �K�[�� ListView
                        tabdlPatternList_lvw.Items.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"�L�k�[���Ϥ��G{imagePath}\n���~�G{ex.Message}");
                }
            }
        }



        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == tabDirList)
            {
                // ���� ImgList ���ҮɡA���� PopulateListView ��k
                PopulateListView();
            }
            else if (tabControl.SelectedTab == tabImgEditor)
            {
                // ���� ImgEditor ���ҮɡA�����L�޿�
            }
        }


        private void gbxfHsblocation_ValueChanged(object sender, EventArgs e)
        {
            tabiefOtherLoc_lbl.Text = tabiefOtherLoc_hsc.Value.ToString();  // frame �����b���ܧ�
        }

        private void Framechkother_CheckStateChanged(object sender, EventArgs e)
        {
            if (tabiefOther_chk.Checked)  // frame �����b�����
            {
                tabiefOtherLoc_lbl.Text = tabiefOtherLoc_hsc.Value.ToString();
            }
            else
            {
                tabiefOtherLoc_lbl.Text = "";
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
                        if (mypicture.tag == "color")
                        {
                            // ��R�I���C��
                            g.Clear(tabiefBack_btn.BackColor);
                        }
                        else if (mypicture.tag == "image")
                        {
                            //�ϥιϤ�
                            g.DrawImage(mypicture.Getpicture(), 0, 0, width, height);
                        }
                        // ø�s�ؽu
                        if (tabiefOther_chk.Checked)
                        {
                            using (Pen pen = new Pen(Color.Black, 2)) // �w�]�u���C��M�e��
                            {
                                foreach (string item in tabiefOther_lst.Items)
                                {
                                    // �ѪR�榡�Ʀr��A�Ҧp "H:100,255,0,0"
                                    string[] parts = item.Split(':');
                                    if (parts.Length == 2)
                                    {
                                        string HVtype = parts[0]; // H �� V
                                        string[] values = parts[1].Split(',');
                                        if (values.Length == 4)
                                        {
                                            int location = int.Parse(values[0]); // �u����m
                                            int r = int.Parse(values[1]); // ������q
                                            int gValue = int.Parse(values[2]); // �����q
                                            int b = int.Parse(values[3]); // �Ŧ���q

                                            // �]�m�u���C��
                                            pen.Color = Color.FromArgb(r, gValue, b);

                                            // ø�s�����u�Ϋ����u
                                            if (HVtype == "H") // �����u
                                            {
                                                g.DrawLine(pen, 0, location, width, location);
                                            }
                                            else if (HVtype == "V") // �����u
                                            {
                                                g.DrawLine(pen, location, 0, location, height);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // ø�s�~�ؽu�]�p�G�襤�^
                        if (tabiefOutside_chk.Checked)
                        {
                            using (Pen borderPen = new Pen(tabiefLineColor_btn.BackColor, 1)) // �~�ؽu�C��M�e��
                            {
                                g.DrawRectangle(borderPen, 0, 0, width - 1, height - 1);
                            }
                        }

                        // ø�s���߹��u�]�p�G�襤�^
                        if (tabiefCenter_chk.Checked)
                        {
                            using (Pen centerPen = new Pen(tabiefLineColor_btn.BackColor, 1)) // ���߽u�C��M�e��
                            {
                                g.DrawLine(centerPen, width / 2, 0, width / 2, height); // �������߽u
                                g.DrawLine(centerPen, 0, height / 2, width, height / 2); // �������߽u
                            }
                        }

                        // ø�s�E�I���u�]�p�G�襤�^
                        if (tabiefNine_chk.Checked)
                        {
                            using (Pen ninePointPen = new Pen(tabiefLineColor_btn.BackColor, 1)) // �E�I�u�C��M�e��
                            {
                                g.DrawLine(ninePointPen, width / 8, 0, width / 8, height); // �������u
                                g.DrawLine(ninePointPen, 7 * width / 8, 0, 7 * width / 8, height); // �k�����u
                                g.DrawLine(ninePointPen, 0, height / 8, width, height / 8); // �W�����u
                                g.DrawLine(ninePointPen, 0, 7 * height / 8, width, 7 * height / 8); // �U�����u
                            }
                        }

                        mypicture.Setpicture(CurrentBitmap);
                        break;
                    case "gradient":
                        int gradientSteps = int.Parse(tabigStep_cmb.Text); // ������h����  
                        int divisions = int.Parse(tabigDivid_cmb.Text); // ����e��������  
                        int startColorValue = int.Parse(tabigFistLevel_cmb.Text); // �}�l�C���  
                        int endColorValue = int.Parse(tabigLastLevel_cmb.Text); // �����C���  

                        int stepValue = (endColorValue - startColorValue >= 0) ?
                            ((endColorValue - startColorValue) + 1) / gradientSteps :
                            ((endColorValue - startColorValue) - 1) / gradientSteps;

                        if (tabigHWay_rdo.Checked) // �p�G��ܤF��V���h  
                        {
                            int divisionHeight = int.Parse(mnsH_txt.Text) / divisions; // �C���������� 
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
                        else if (tabigVWay_rdo.Checked) // �p�G��ܤF�a�V���h
                        {
                            int divisionWidth = int.Parse(mnsW_txt.Text) / divisions; // �C�������e�� 
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
                        else if (tabigColorBar_rdo.Checked) // �p�G��ܤFcolorbar
                        {

                        }

                        mypicture.Setpicture(CurrentBitmap);
                        break;
                    case "chess":
                        for (int y = 0; y < height; y += height / (int)tabiecVNum_nud.Value)
                        {
                            for (int x = 0; x < width; x += width / (int)tabiecHNum_nud.Value)
                            {
                                // �p���C��`�L
                                int intensity = tabiecGray_hsc.Value;
                                Color baseColor = tabiecGray_lbl.BackColor;
                                Color adjustedColor = Color.FromArgb(
                                    Math.Min(baseColor.R, intensity),
                                    Math.Min(baseColor.G, intensity),
                                    Math.Min(baseColor.B, intensity)
                                );

                                // �M�w�϶��C��
                                Color blockColor = ((x / (width / (int)tabiecHNum_nud.Value)) + (y / (height / (int)tabiecVNum_nud.Value))) % 2 == 0 ? Color.Black : adjustedColor;

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
                        if (tabiwBackColor_rdo.Checked) // �¦�I��
                        {
                            g.Clear(tabiwBack_pic.BackColor);
                        }
                        else if (tabiwBackImg_rdo.Checked) // �Ϥ��I��
                        {
                            g.DrawImage(tabiwBack_pic.Image, 0, 0, width, height);
                        }
                        int percent = (int)tabiwWinSizePercent_nud.Value; // ����ʤ���j�p
                        // �ʤ���j�p // ��ڤj�p
                        int newWidth = tabiwWinSizePercent_rdo.Checked ? int.Parse(mnsW_txt.Text) * percent / 100 : int.Parse(tabiwWinSizePixelW_nud.Text);
                        int newHeight = tabiwWinSizePercent_rdo.Checked ? int.Parse(mnsH_txt.Text) * percent / 100 : int.Parse(tabiwWinSizePixelH_nud.Text);
                        int xstart = 0;
                        int ystart = 0;
                        if (tabiwWinLocCenter_rdo.Checked) // �m��
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
                            if (tabiwWinColor_rdo.Checked) // �¦���
                            {
                                g.FillRectangle(new SolidBrush(tabiwWin_pic.BackColor), xstart, ystart, newWidth, newHeight);
                            }
                            else if (tabiwWinImg_rdo.Checked)// ø�s�Ϥ����
                            {
                                g.DrawImage(tabiwWin_pic.Image, xstart, ystart, newWidth, newHeight);
                            }
                            xstart = width / 5 * 3;
                            ystart = height / 5 * 3;
                        }

                        if (tabiwWinColor_rdo.Checked) // �¦���
                        {
                            g.FillRectangle(new SolidBrush(tabiwWin_pic.BackColor), xstart, ystart, newWidth, newHeight);
                        }
                        else if (tabiwWinImg_rdo.Checked)// ø�s�Ϥ����
                        {
                            g.DrawImage(tabiwWin_pic.Image, xstart, ystart, newWidth, newHeight);
                        }

                        mypicture.Setpicture(CurrentBitmap);
                        break;
                    case "mask":
                        // �o�̥i�H�K�[�B�n���ͦ��޿�
                        if (tabiemPixel_rdo.Checked)
                        {
                            //123
                            int pixelW = (int)tabiemWNum_nud.Value;
                            int pixelH = (int)tabiemHNum_nud.Value;

                            using (Bitmap pixelimage = new Bitmap(pixelW, pixelH))
                            {
                                using (Graphics g2 = Graphics.FromImage(pixelimage))
                                {
                                    // �M�� tabiemPanel_pnl ���l Panel  
                                    foreach (Control control in tabiemPanel_pnl.Controls)
                                    {
                                        if (control is Panel panel)
                                        {
                                            // �ϥ� Panel ���I���C���R�������ϰ�  
                                            using (Brush brush = new SolidBrush(panel.BackColor))
                                            {
                                                g2.FillRectangle(brush, 0, 0, 1, 1);
                                            }
                                        }
                                    }

                                    //if (pixelimage != null)
                                    //{
                                    //    pictureBox1.Image = pixelimage; // ��ܹ����Ϥ�
                                    //}

                                    for (int j = 0; j < height; j += pixelH)
                                    {
                                        for (int i = 0; i < width; i += pixelW)
                                        {
                                            g.DrawImage(pixelimage, i, j);
                                        }
                                    }
                                }

                            }
                        }
                        else if (tabiemSubPixel_rdo.Checked)
                        {
                            //
                        }
                        mypicture.Setpicture(CurrentBitmap);
                        break;
                    case "adjust":
                        // �o�̥i�H�K�[�վ�Ϥ����޿�
                        break;
                    case "mass":
                        // �o�̥i�H�K�[��q�B�z�Ϥ����޿�
                        break;
                    default:
                        throw new NotSupportedException("���䴩���Ϥ������I");
                }
            }
        }

        private async void tabieaTrans_btn_Click(object sender, EventArgs e)
        {
            ssrProgressbar_prg.Visible = true;
            ssrProgressbar_prg.Maximum = tabieaPatternList_chl.CheckedItems.Count;
            ssrProgressbar_prg.Value = 0; // ��l�ƶi�ױ���

            using (Bitmap bitmap = new Bitmap(int.Parse(mnsW_txt.Text), int.Parse(mnsH_txt.Text)))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    if (tabieaUpDown_rdo.Checked)
                    {
                        // ���o�W�U�Ϥ�
                        Bitmap upperImage = new Bitmap(tabieaUpDownU_pic.Image);
                        Bitmap lowerImage = new Bitmap(tabieaUpDownD_pic.Image);
                        // �إ߲զX�᪺�Ϥ�
                        // ø�s�W��Ϥ�
                        g.DrawImage(upperImage, 0, 0, bitmap.Width, bitmap.Height / 2);
                        // ø�s�U��Ϥ�
                        g.DrawImage(lowerImage, 0, bitmap.Height / 2, bitmap.Width, bitmap.Height / 2);
                        // ��s�ɮ��x�s���|
                        using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                        {
                            saveFileDialog.Filter = "JPEG|*.jpg|PNG|*.png|BMP|*.bmp";
                            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                try
                                {
                                    if (upperImage == null || lowerImage == null)
                                    {
                                        throw new InvalidOperationException("�S���Ϥ��i�H�x�s�I");
                                    }
                                    string extension = Path.GetExtension(saveFileDialog.FileName).ToLower();
                                    ImageFormat imageFormat = extension switch
                                    {
                                        ".jpg" => ImageFormat.Jpeg,
                                        ".png" => ImageFormat.Png,
                                        ".bmp" => ImageFormat.Bmp,
                                        _ => throw new NotSupportedException("���䴩���ɮ׮榡�I")
                                    };
                                    bitmap.Save(saveFileDialog.FileName, imageFormat);
                                    //MessageBox.Show("�Ϥ��w�x�s���\�I");
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show($"�Ϥ��x�s���ѡG{ex.Message}");
                                }
                            }
                        }

                    }
                    else if (tabieaLeftRight_rdo.Checked)
                    {
                        // ���o�W�U�Ϥ�
                        Bitmap leftImage = new Bitmap(tabieaLeftRightL_pic.Image);
                        Bitmap rightImage = new Bitmap(tabieaLeftRightR_pic.Image);
                        // �إ߲զX�᪺�Ϥ�
                        // ø�s����Ϥ�
                        g.DrawImage(leftImage, 0, 0, bitmap.Width / 2, bitmap.Height);
                        // ø�s�k��Ϥ�
                        g.DrawImage(rightImage, bitmap.Width / 2, 0, bitmap.Width / 2, bitmap.Height);
                        // ��s�ɮ��x�s���|
                        using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                        {
                            saveFileDialog.Filter = "JPEG|*.jpg|PNG|*.png|BMP|*.bmp";
                            if (saveFileDialog.ShowDialog() == DialogResult.OK)
                            {
                                try
                                {
                                    if (leftImage == null || rightImage == null)
                                    {
                                        throw new InvalidOperationException("�S���Ϥ��i�H�x�s�I");
                                    }
                                    string extension = Path.GetExtension(saveFileDialog.FileName).ToLower();
                                    ImageFormat imageFormat = extension switch
                                    {
                                        ".jpg" => ImageFormat.Jpeg,
                                        ".png" => ImageFormat.Png,
                                        ".bmp" => ImageFormat.Bmp,
                                        _ => throw new NotSupportedException("���䴩���ɮ׮榡�I")
                                    };
                                    bitmap.Save(saveFileDialog.FileName, imageFormat);
                                    //MessageBox.Show("�Ϥ��w�x�s���\�I");
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show($"�Ϥ��x�s���ѡG{ex.Message}");
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (object iteamChecked in tabieaPatternList_chl.CheckedItems)
                        {
                            string[] words = iteamChecked.ToString().Split('.');
                            string filePath = $"{words[0]}_new.{words[1]}"; // �x�s���|

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
                                case "png":
                                    bitmap.Save(filePath, ImageFormat.Png); // �x�s�Ϥ�
                                    break;
                                case "jpeg" or "jpg":
                                    bitmap.Save(filePath, ImageFormat.Jpeg);
                                    break;
                                case "bmp":
                                    bitmap.Save(filePath, ImageFormat.Bmp);
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
            ssrProgressbar_prg.Visible = false; // �b�j�駹�������öi�ױ�
        }

        private void import_Click(object sender, EventArgs e)
        {
            try
            {
                if (sender is Button targetbutton && targetbutton.Text == "�ɤJ�Ϥ�")
                {
                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                        openFileDialog.Multiselect = true; // �ҥΦh��\��  
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
                    // �N���J���Ϥ��]�m�� PictureBox
                    showimgPicture_pic.Image = mypicture.OpenImage();
                    showimgPicture_pic.SizeMode = PictureBoxSizeMode.Zoom; // �]�m�Ϥ��Y��Ҧ�
                    showimgSize_btn.Text = $"�Ϥ��j�p�G{mypicture.Getsize()}"; // ��s���A�C��ܹϤ��j�p
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"�L�k���J�Ϥ��G{ex.Message}", "���~", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void pictureBox_DragEnter(object sender, DragEventArgs e)
        {
            // �ˬd�ɮ׬O�_���䴩���Ϥ��榡
            bool IsSupportedImageFile(string filePath)
            {
                string extension = Path.GetExtension(filePath).ToLower();
                return extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".bmp";
            }
            // �ˬd��J����ƬO�_�]�t�ɮסA�åB�O�䴩���Ϥ��榡
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0 && IsSupportedImageFile(files[0]))
                {
                    e.Effect = DragDropEffects.Copy; // �]�m���ĪG���ƻs
                }
                else
                {
                    e.Effect = DragDropEffects.None; // �����\���
                }
            }
            else
            {
                e.Effect = DragDropEffects.None; // �����\���
            }
        }

        private void pictureBox_DragDrop(object sender, DragEventArgs e)
        {
            // �����J���ɮ�
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                string filePath = files[0];
                try
                {
                    // ���J�Ϥ�����ܦb PictureBox ��
                    showimgPicture_pic.Image = mypicture.Setpicture(Image.FromFile(filePath));
                    showimgPicture_pic.SizeMode = PictureBoxSizeMode.Zoom; // �]�m�Ϥ��Y��Ҧ�
                    showimgSize_btn.Text = $"�Ϥ��j�p�G{mypicture.Getsize()}"; // ��s���A�C��ܹϤ��j�p
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"�L�k���J�Ϥ��G{ex.Message}", "���~", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void tabigOther_btn_Click(object sender, EventArgs e)
        {
            tabigOther_pnl.Enabled = !tabigOther_pnl.Enabled;  // ���Gradient����L�C��
        }
        private void SetButtonBackgroundColor(object sender, EventArgs e)
        {
            // �N targetButton �קאּ�ƥ�Ĳ�o���s  
            if (sender is Button targetButton)
            {
                using (ColorDialog colorDialog = new ColorDialog())
                {
                    colorDialog.FullOpen = true; // ��ܶi���ﶵ  
                                                 // ����C���ܹ�ܮ�  
                    if (colorDialog.ShowDialog() == DialogResult.OK)
                    {
                        // �N��ܪ��C��]�m���ؼЫ��s���I���C��  
                        targetButton.BackColor = colorDialog.Color;
                        mypicture.tag = "color";
                        targetButton.Image = null; // �M�����s�W���Ϥ�
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
                    // Frame �W�[��L�u 
                    if (!string.IsNullOrEmpty(tabiefOther_cmb.Text))
                    {
                        string itemText = $"{tabiefOther_cmb.Text.Substring(0, 1)}:{tabiefOtherLoc_lbl.Text},{tabiefOtherColor_btn.BackColor.R},{tabiefOtherColor_btn.BackColor.G},{tabiefOtherColor_btn.BackColor.B}";
                        if (!tabiefOther_lst.Items.Contains(itemText))
                        {
                            // �N�榡�Ʀr��s�W�� gbxfLst  
                            tabiefOther_lst.Items.Add(itemText);
                        }
                    }
                }
                else if (targetbutton.Text == "Clear")
                {
                    //�M��Framelst�����e
                    tabiefOther_lst.Items.Clear();
                }
            }
        }

        private void tabiefOther_cmb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabiefOther_cmb.Text == "HLine")
            {
                tabiefOtherLoc_hsc.Maximum = (int.Parse(mnsH_txt.Text)); // �N msTxtH.Text �ഫ�� int  
            }
            else if (tabiefOther_cmb.Text == "VLine")
            {
                tabiefOtherLoc_hsc.Maximum = (int.Parse(mnsW_txt.Text)); // �N msTxtW.Text �ഫ�� int  
            }
        }


        private void Save_Click(object sender, EventArgs e)
        {
            if (tabiefMass_chk.Checked && tabimgeFuncList.Text == "Frame&CrossLine&Border")
            {
                // ��q�x�s�Ϥ�
                Color co = tabiefBack_btn.BackColor;
                for (int i = 0; i <= Math.Max(tabiefBack_btn.BackColor.R, Math.Max(tabiefBack_btn.BackColor.G, tabiefBack_btn.BackColor.B)); i += (int)tabiefMassNum_nud.Value)
                {
                    string fileName = $"{mnsPattername_txt.Text}_{i}.png"; // �]�w�ɦW
                    string filePath = Path.Combine(Application.StartupPath, fileName); // �x�s���|
                    using (Bitmap bitmap = new Bitmap(int.Parse(mnsW_txt.Text), int.Parse(mnsH_txt.Text)))
                    {
                        using (Graphics g = Graphics.FromImage(bitmap))
                        {
                            g.Clear(Color.FromArgb(i <= co.R ? i : co.R, i <= co.G ? i : co.G, i <= co.B ? i : co.B)); // ��R�I���C��)); // ��R�I���C��
                        }
                        bitmap.Save(filePath, ImageFormat.Png); // �x�s�Ϥ�
                    }
                }
            }
            else
            {
                // �x�s��i�Ϥ�
                mypicture.SaveImage();
            }
        }

        private void hScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            tabiecGray_lbl.Text = $"Gary: {tabiecGray_hsc.Value.ToString()}"; // ��s Label ��ܷ�e��
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
                    tabigBaseColor_lbl.Image = Resources.colorbar;
                    tabigBaseColor_lbl.Text = "        ";
                }
                else
                {
                    tabigBaseColor_lbl.Image = null;
                    tabigBaseColor_lbl.BackColor = targetbutton.BackColor;
                    tabigBaseColor_lbl.Text = "�򩳦�m";
                }
                tabigOther_pnl.Enabled = false; // �����C���ܭ��O
            }
        }

        private void windowPictureBox_Click(object sender, EventArgs e)
        {
            if (sender is PictureBox targetPictureBox)
            {
                if (targetPictureBox.Name == "pictureBox1")
                {
                    if (tabiwBackColor_rdo.Checked)
                    {
                        using (ColorDialog colorDialog = new ColorDialog())
                        {
                            colorDialog.FullOpen = true; // ��ܶi���ﶵ  
                                                         // ����C���ܹ�ܮ�  
                            if (colorDialog.ShowDialog() == DialogResult.OK)
                            {
                                targetPictureBox.BackColor = colorDialog.Color;
                                targetPictureBox.Image = null; // �M�����s�W���Ϥ�
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
                else if (targetPictureBox.Name == "pictureBox2")
                {
                    if (tabiwWinColor_rdo.Checked)
                    {
                        using (ColorDialog colorDialog = new ColorDialog())
                        {
                            colorDialog.FullOpen = true; // ��ܶi���ﶵ  
                                                         // ����C���ܹ�ܮ�  
                            if (colorDialog.ShowDialog() == DialogResult.OK)
                            {
                                targetPictureBox.BackColor = colorDialog.Color;
                                targetPictureBox.Image = null; // �M�����s�W���Ϥ�
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
                        colorDialog.FullOpen = true; // ��ܶi���ﶵ  
                                                     // ����C���ܹ�ܮ�  
                        if (colorDialog.ShowDialog() == DialogResult.OK)
                        {
                            tabiwBack_pic.BackColor = colorDialog.Color;
                            tabiwBack_pic.Image = null; // �M�����s�W���Ϥ�
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
                        colorDialog.FullOpen = true; // ��ܶi���ﶵ  
                                                     // ����C���ܹ�ܮ�  
                        if (colorDialog.ShowDialog() == DialogResult.OK)
                        {
                            tabiwWin_pic.BackColor = colorDialog.Color;
                            tabiwWin_pic.Image = null; // �M�����s�W���Ϥ�
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
            // �M�ŭ��O
            tabiemPanel_pnl.Controls.Clear();

            // ���o�e�׻P����
            int width = (int)tabiemWNum_nud.Value;
            int height = (int)tabiemHNum_nud.Value;

            // �]�w�C�Ӱ϶����j�p
            int blockWidth = tabiemPanel_pnl.Width / width;
            int blockHeight = tabiemPanel_pnl.Height / height;

            // �ͦ��϶�
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Panel block = new Panel
                    {
                        Size = new Size(blockWidth, blockHeight),
                        Location = new Point(j * blockWidth, i * blockHeight),
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    // �]�w�I���ƥ�H��R����
                    block.Click += (s, args) =>
                    {
                        block.BackColor = tabiemPanelColor_btn.BackColor;
                    };

                    tabiemPanel_pnl.Controls.Add(block);
                }
            }
        }

    }
}