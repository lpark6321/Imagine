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
            cmbFunclist.SelectedIndex = 3; // �w�]��ܲĤ@�Ӷ���
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
                return _currentBitmap; // �^�ǹϤ�
            }
        }

        private void cmbFunclist_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ���éҦ� GroupBox
            gbxFrame.Visible = false;
            groupBoxGradient.Visible = false;
            groupBoxChess.Visible = false;
            groupBoxWindow.Visible = false;
            groupBoxMask.Visible = false;
            groupBoxAdjust.Visible = false;
            groupBoxMassImage.Visible = false;

            // �ھڿ�ܪ�������ܹ����� GroupBox
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
                    Gradrdovway.Checked = true; // �w�]��� Gradient �覡
                    comboBox1.Text = "256"; // �w�]��� 256 ��
                    comboBox2.Text = "1"; // �w�]��� 1 ��
                    comboBox3.Text = "0"; // �w�]�q 0 ���}�l
                    comboBox4.Text = "255"; // �w�]�� 255 ������
                    break;
                case "Chess":
                    groupBoxChess.Visible = true;
                    groupBoxChess.Dock = DockStyle.Fill;
                    break;
                case "Window":
                    groupBoxWindow.Visible = true;
                    groupBoxWindow.Dock = DockStyle.Fill;
                    //radioButton11.Checked = true; // �w�]��ܯ¦�I��
                    //radioButton16.Checked = true; // �w�]��ܯ¦���
                    radioButton17.Checked = true; // �w�]��ܦʤ���j�p
                    numericUpDown6.Value = 50; // �w�]��� 50% �j�p
                    radioButton19.Checked = true; // �w�]��ܸm����m
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
                    // �p�G�S���ǰt�����ءA�h����ܥ��� GroupBox
                    break;
            }
        }

        private void Generate_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(msTxtW.Text, out int width) || !int.TryParse(msTxtH.Text, out int height))
            {
                MessageBox.Show("�п�J���Ī��e�׻P���סI");
                return;
            }

            // �ھ� cmbFunclist ����ܨӨM�w type  
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
                pictureLblsize.Text = $"��e�Ϥ��j�p�G{msTxtW.Text} x {msTxtH.Text}"; // ��s���A�C��ܷ�e�Ϥ��j�p  
            }
            catch (Exception ex)
            {
                MessageBox.Show($"�Ϥ��ͦ����ѡG{ex.Message}");
            }
        }

        private void FullScreen(object sender, EventArgs e)
        {
            // �����ܪ��ù�  
            string selectedScreen = msCmblist.SelectedItem?.ToString();
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
                Image = pictureBox.Image, // �ϥέ�l PictureBox ���Ϥ�  
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
            msCmblist.Items.Clear();
            // �K�[�ù��ﶵ��U�Կ��
            foreach (Screen screen in screens)
            {
                msCmblist.Items.Add(screen.DeviceName);
            }


        }
        private void menuToolStripCmb_SelectedIndexChanged(object sender, EventArgs e)
        {

            // �����ܪ��ù�
            string selectedScreen = msCmblist.SelectedItem.ToString();
            Screen[] screens = Screen.AllScreens;
            // �ھڿ�ܪ��ù���s�e�׻P����
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
                ListViewItem selectedItem = listView.SelectedItems[0]; // ����襤�� ListViewItem  
                string imagePath = selectedItem.Tag as string; // �q Tag �����X�Ϥ����|  

                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    // ���J��l�Ϥ��� PictureBox  
                    pictureBox.Image = Image.FromFile(imagePath);
                    pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureLblsize.Text = $"��e�Ϥ��j�p�G{selectedItem.SubItems[2].Text} x {selectedItem.SubItems[3].Text}"; // ��s���A�C��ܷ�e�Ϥ��j�p  
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
            listView.Items.Clear();
            listView.Groups.Clear();
            listView.LargeImageList = new ImageList
            {
                ImageSize = new Size(90, 90) // �]�m�Y���Ϥj�p
            };

            // �w�q����
            ListViewGroup jpgGroup = new ListViewGroup("JPG �Ϥ�", HorizontalAlignment.Left);
            ListViewGroup pngGroup = new ListViewGroup("PNG �Ϥ�", HorizontalAlignment.Left);
            ListViewGroup bmpGroup = new ListViewGroup("BMP �Ϥ�", HorizontalAlignment.Left);

            listView.Groups.Add(jpgGroup);
            listView.Groups.Add(pngGroup);
            listView.Groups.Add(bmpGroup);

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
                        listView.LargeImageList.Images.Add(fileName, new Bitmap(img));

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
                        listView.Items.Add(item);
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
            gbxfLbllocation.Text = gbxfHsblocation.Value.ToString();  // frame �����b���ܧ�
        }

        private void Framechkother_CheckStateChanged(object sender, EventArgs e)
        {
            if (gbxfChkother.Checked)  // frame �����b�����
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
                        // ��R�I���C��
                        g.Clear(gbxfBtnbackcolor.BackColor);

                        // ø�s�ؽu
                        if (gbxfChkother.Checked)
                        {
                            using (Pen pen = new Pen(Color.Black, 2)) // �w�]�u���C��M�e��
                            {
                                foreach (string item in gbxfLst.Items)
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
                        if (gbxfChkoutside.Checked)
                        {
                            using (Pen borderPen = new Pen(gbxfBtnLine.BackColor, 1)) // �~�ؽu�C��M�e��
                            {
                                g.DrawRectangle(borderPen, 0, 0, width - 1, height - 1);
                            }
                        }

                        // ø�s���߹��u�]�p�G�襤�^
                        if (gbxfChkcenter.Checked)
                        {
                            using (Pen centerPen = new Pen(gbxfBtnLine.BackColor, 1)) // ���߽u�C��M�e��
                            {
                                g.DrawLine(centerPen, width / 2, 0, width / 2, height); // �������߽u
                                g.DrawLine(centerPen, 0, height / 2, width, height / 2); // �������߽u
                            }
                        }

                        // ø�s�E�I���u�]�p�G�襤�^
                        if (gbxfChknine.Checked)
                        {
                            using (Pen ninePointPen = new Pen(gbxfBtnLine.BackColor, 1)) // �E�I�u�C��M�e��
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
                        int gradientSteps = int.Parse(comboBox1.Text); // ������h����  
                        int divisions = int.Parse(comboBox2.Text); // ����e��������  
                        int startColorValue = int.Parse(comboBox3.Text); // �}�l�C���  
                        int endColorValue = int.Parse(comboBox4.Text); // �����C���  

                        int stepValue = (endColorValue - startColorValue >= 0) ?
                            ((endColorValue - startColorValue) + 1) / gradientSteps :
                            ((endColorValue - startColorValue) - 1) / gradientSteps;

                        if (Gradrdohway.Checked) // �p�G��ܤF��V���h  
                        {
                            int divisionHeight = int.Parse(msTxtH.Text) / divisions; // �C���������� 
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
                        else if (Gradrdovway.Checked) // �p�G��ܤF�a�V���h
                        {
                            int divisionWidth = int.Parse(msTxtW.Text) / divisions; // �C�������e�� 
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
                        else if (radioButton3.Checked) // �p�G��ܤFcolorbar
                        {

                        }

                        mypicture.Setpicture(CurrentBitmap);
                        break;
                    case "chess":
                        for (int y = 0; y < height; y += height / (int)numericUpDown3.Value)
                        {
                            for (int x = 0; x < width; x += width / (int)numericUpDown2.Value)
                            {
                                // �p���C��`�L
                                int intensity = hScrollBar1.Value;
                                Color baseColor = label12.BackColor;
                                Color adjustedColor = Color.FromArgb(
                                    Math.Min(baseColor.R, intensity),
                                    Math.Min(baseColor.G, intensity),
                                    Math.Min(baseColor.B, intensity)
                                );

                                // �M�w�϶��C��
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
                        if (radioButton11.Checked) // �¦�I��
                        {
                            g.Clear(pictureBox1.BackColor);
                        }
                        else if (radioButton12.Checked) // �Ϥ��I��
                        {
                            g.DrawImage(pictureBox1.BackgroundImage, 0, 0, width, height);
                        }
                        int percent = (int)numericUpDown6.Value; // ����ʤ���j�p
                        // �ʤ���j�p // ��ڤj�p
                        int newWidth = radioButton17.Checked ? int.Parse(msTxtW.Text) * percent / 100: int.Parse(numericUpDown7.Text);
                        int newHeight = radioButton17.Checked ? int.Parse(msTxtH.Text) * percent / 100 : int.Parse(numericUpDown8.Text);
                        int xstart = 0;
                        int ystart = 0;
                        if (radioButton19.Checked) // �m��
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
                            if (radioButton16.Checked) // �¦���
                            {
                                g.FillRectangle(new SolidBrush(pictureBox2.BackColor), xstart, ystart, newWidth, newHeight);
                            }
                            else if (radioButton15.Checked)// ø�s�Ϥ����
                            {
                                g.DrawImage(pictureBox2.BackgroundImage, xstart, ystart, newWidth, newHeight);
                            }
                            xstart = width / 5 * 3;
                            ystart = height / 5 * 3;
                        }

                        if (radioButton16.Checked) // �¦���
                        {
                            g.FillRectangle(new SolidBrush(pictureBox2.BackColor), xstart, ystart, newWidth, newHeight);
                        }
                        else if (radioButton15.Checked)// ø�s�Ϥ����
                        {
                            g.DrawImage(pictureBox2.BackgroundImage, xstart, ystart, newWidth, newHeight);
                        }

                        mypicture.Setpicture(CurrentBitmap);
                        break;
                    case "mask":
                        // �o�̥i�H�K�[�B�n���ͦ��޿�
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




        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {

        }

        private async void button25_Click(object sender, EventArgs e)
        {

            toolStripProgressBar1.Visible = true;
            for (int i = 0; i <= 100; i++)
            {
                toolStripProgressBar1.Value = i;
                await Task.Delay(100); // ��s�i�ױ�������
            }
            toolStripProgressBar1.Visible = false; // �b�j�駹�������öi�ױ�
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
                        openFileDialog.Multiselect = true; // �ҥΦh��\��  
                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            foreach (string fileName in openFileDialog.FileNames)
                            {
                                checkedListBox1.Items.Add(fileName); // �N��ܪ��ɮץ[�J�� CheckedListBox
                            }
                        }
                    }
                }
                else
                {
                    // �N���J���Ϥ��]�m�� PictureBox
                    pictureBox.Image = mypicture.OpenImage();
                    pictureBox.SizeMode = PictureBoxSizeMode.Zoom; // �]�m�Ϥ��Y��Ҧ�
                    pictureLblsize.Text = $"�Ϥ��j�p�G{mypicture.Getsize()}"; // ��s���A�C��ܹϤ��j�p
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
                    pictureBox.Image = mypicture.Setpicture(Image.FromFile(filePath));
                    pictureBox.SizeMode = PictureBoxSizeMode.Zoom; // �]�m�Ϥ��Y��Ҧ�
                    pictureLblsize.Text = $"�Ϥ��j�p�G{mypicture.Getsize()}"; // ��s���A�C��ܹϤ��j�p
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"�L�k���J�Ϥ��G{ex.Message}", "���~", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void button6_Click(object sender, EventArgs e)
        {
            panel2.Enabled = !panel2.Enabled;  // ���Gradient����L�C��
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
                        targetButton.BackgroundImage = null; // �M�����s�W���Ϥ�
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
                    if (!string.IsNullOrEmpty(gbxfCmbhnv.Text))
                    {
                        string itemText = $"{gbxfCmbhnv.Text.Substring(0, 1)}:{gbxfLbllocation.Text},{gbxfBtnlinecolor.BackColor.R},{gbxfBtnlinecolor.BackColor.G},{gbxfBtnlinecolor.BackColor.B}";
                        if (!gbxfLst.Items.Contains(itemText))
                        {
                            // �N�榡�Ʀr��s�W�� gbxfLst  
                            gbxfLst.Items.Add(itemText);
                        }
                    }
                }
                else if (targetbutton.Text == "Clear")
                {
                    //�M��Framelst�����e
                    gbxfLst.Items.Clear();
                }
            }
        }

        private void gbxfCmbhnv_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gbxfCmbhnv.Text == "HLine")
            {
                gbxfHsblocation.Maximum = (int.Parse(msTxtH.Text)); // �N msTxtH.Text �ഫ�� int  
            }
            else if (gbxfCmbhnv.Text == "VLine")
            {
                gbxfHsblocation.Maximum = (int.Parse(msTxtW.Text)); // �N msTxtW.Text �ഫ�� int  
            }
        }


        private void Save_Click(object sender, EventArgs e)
        {
            if (gbxfChkmass.Checked & cmbFunclist.Text == "Frame&CrossLine&Border")
            {
                // ��q�x�s�Ϥ�
                Color co = gbxfBtnbackcolor.BackColor;
                for (int i = 0; i <= Math.Max(gbxfBtnbackcolor.BackColor.R, Math.Max(gbxfBtnbackcolor.BackColor.G, gbxfBtnbackcolor.BackColor.B)); i += (int)numericUpDown11.Value)
                {
                    string fileName = $"{toolStripTextBox1.Text}_{i}.png"; // �]�w�ɦW
                    string filePath = Path.Combine(Application.StartupPath, fileName); // �x�s���|
                    using (Bitmap bitmap = new Bitmap(int.Parse(msTxtW.Text), int.Parse(msTxtH.Text)))
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
            label12.Text = $"Gary: {hScrollBar1.Value.ToString()}"; // ��s Label ��ܷ�e��
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
                    label5.Text = "�򩳦�m";
                }
                panel2.Enabled = false; // �����C���ܭ��O
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
                            colorDialog.FullOpen = true; // ��ܶi���ﶵ  
                                                         // ����C���ܹ�ܮ�  
                            if (colorDialog.ShowDialog() == DialogResult.OK)
                            {
                                targetPictureBox.BackColor = colorDialog.Color;
                                targetPictureBox.BackgroundImage = null; // �M�����s�W���Ϥ�
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
                            colorDialog.FullOpen = true; // ��ܶi���ﶵ  
                                                         // ����C���ܹ�ܮ�  
                            if (colorDialog.ShowDialog() == DialogResult.OK)
                            {
                                targetPictureBox.BackColor = colorDialog.Color;
                                targetPictureBox.BackgroundImage = null; // �M�����s�W���Ϥ�
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
                            colorDialog.FullOpen = true; // ��ܶi���ﶵ  
                                                         // ����C���ܹ�ܮ�  
                            if (colorDialog.ShowDialog() == DialogResult.OK)
                            {
                                pictureBox1.BackColor = colorDialog.Color;
                                pictureBox1.BackgroundImage = null; // �M�����s�W���Ϥ�
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
                            colorDialog.FullOpen = true; // ��ܶi���ﶵ  
                                                         // ����C���ܹ�ܮ�  
                            if (colorDialog.ShowDialog() == DialogResult.OK)
                            {
                                pictureBox2.BackColor = colorDialog.Color;
                                pictureBox2.BackgroundImage = null; // �M�����s�W���Ϥ�
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