using System;
using System.Windows.Forms;

namespace Natural
{
    partial class Mainwindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);    
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Menustrip = new MenuStrip();
            mnsImport_btn = new ToolStripMenuItem();
            mnsFullscreen_btn = new ToolStripMenuItem();
            mnsH_txt = new ToolStripTextBox();
            mnsX_lbl = new ToolStripMenuItem();
            mnsW_txt = new ToolStripTextBox();
            mnsScreenlist_cmb = new ToolStripComboBox();
            mnsPattername_txt = new ToolStripTextBox();
            tabControl = new TabControl();
            tabImgEditor = new TabPage();
            tabimgePanel = new Panel();
            tabimgeMask = new GroupBox();
            button1 = new Button();
            tabiemPanel_pnl = new Panel();
            tabiemHNum_nud = new NumericUpDown();
            tabiemWNum_nud = new NumericUpDown();
            tabiemHNum_lbl = new Label();
            tabiemWNum_lbl = new Label();
            tabiemSubPixel_rdo = new RadioButton();
            tabiemPixel_rdo = new RadioButton();
            tabimgeAdjust = new GroupBox();
            tabieaPanel_pnl = new Panel();
            tabieaLeftRight_pnl = new Panel();
            tabieaLeftRightL_pic = new PictureBox();
            tabieaLeftRightR_pic = new PictureBox();
            tabieaUpDown_pnl = new Panel();
            tabieaUpDownU_pic = new PictureBox();
            tabieaUpDownD_pic = new PictureBox();
            tabieaPatternList_chl = new CheckedListBox();
            tabieaClear_btn = new Button();
            tabieaVFlip_rdo = new RadioButton();
            tabieaHFlip_rdo = new RadioButton();
            tabieaResize_rdo = new RadioButton();
            tabieaImport_btn = new Button();
            tabieaTrans_btn = new Button();
            tabieaRotate270_rdo = new RadioButton();
            tabieaRotate180_rdo = new RadioButton();
            tabieaRotate90_rdo = new RadioButton();
            tabieaLeftRight_rdo = new RadioButton();
            tabieaUpDown_rdo = new RadioButton();
            tabimgeFrame = new GroupBox();
            tabiefBack_btn = new Button();
            tabiefLineColor_lbl = new Label();
            tabiefBack_pic = new PictureBox();
            tabiefMassNum_nud = new NumericUpDown();
            tabiefMass_chk = new CheckBox();
            tabiefOtherColor_lbl = new Label();
            tabiefOtherLoc_lbl = new Label();
            tabiefOtherColor_btn = new Button();
            tabiefOtherClear_btn = new Button();
            tabiefOtherAdd_btn = new Button();
            tabiefOther_lst = new ListBox();
            tabiefOtherLoc_hsc = new HScrollBar();
            tabiefOther_cmb = new ComboBox();
            tabiefOther_chk = new CheckBox();
            tabiefNine_chk = new CheckBox();
            tabiefOutside_chk = new CheckBox();
            tabiefCenter_chk = new CheckBox();
            tabiefLineColor_btn = new Button();
            tabiefBack_lbl = new Label();
            tabimgeGradient = new GroupBox();
            tabigColorBar_rdo = new RadioButton();
            tabigOther_pnl = new Panel();
            tabigOtherSplit_lst = new ListBox();
            tabigOtherColorC_lbl = new Label();
            tabigOtherColorC_btn = new Button();
            tabigOtherColorM_btn = new Button();
            tabigOtherColorA_btn = new Button();
            tabigOtherColorY_btn = new Button();
            tabigOtherColorD_btn = new Button();
            tabigOtherColorB_btn = new Button();
            tabigOtherColorG_btn = new Button();
            tabigOtherColorR_btn = new Button();
            tabigOtherColorW_btn = new Button();
            tabigOtherSplit_lbl = new Label();
            tabigOtherSplit_nud = new NumericUpDown();
            tabigOtherHsplit_rdo = new RadioButton();
            tabigOtherVSplit_rdo = new RadioButton();
            tabigOther_btn = new Button();
            tabigBaseColorCustom_btn = new Button();
            tabigBaseColorB_btn = new Button();
            tabigBaseColorG_btn = new Button();
            tabigBaseColorR_btn = new Button();
            tabigBaseColorW_btn = new Button();
            tabigBaseColor_lbl = new Label();
            tabigLastLevel_cmb = new ComboBox();
            tabigFistLevel_cmb = new ComboBox();
            tabigLastLevel_lbl = new Label();
            tabigFistLevel_lbl = new Label();
            tabigDivid_cmb = new ComboBox();
            tabigStep_cmb = new ComboBox();
            tabigDivid_lbl = new Label();
            tabigStep_lbl = new Label();
            tabigHWay_rdo = new RadioButton();
            tabigVWay_rdo = new RadioButton();
            tabimgeChess = new GroupBox();
            tabiecBaseColorCustom_lbl = new Label();
            tabiecGray_lbl = new Label();
            tabiecBaseColorCustom_btn = new Button();
            tabiecGray_hsc = new HScrollBar();
            tabiecBaseColorM_btn = new Button();
            tabiecVNum_nud = new NumericUpDown();
            tabiecBaseColorA_btn = new Button();
            tabiecHNum_nud = new NumericUpDown();
            tabiecBaseColorY_btn = new Button();
            tabiecBaseColor_lbl = new Label();
            tabiecVNum_lbl = new Label();
            tabiecBaseColorB_btn = new Button();
            tabiecHNum_lbl = new Label();
            tabiecBaseColorG_btn = new Button();
            tabiecBaseColorW_btn = new Button();
            tabiecBaseColorR_btn = new Button();
            tabimgeWindow = new GroupBox();
            tabiwWin_grp = new GroupBox();
            tabiwWin_pic = new PictureBox();
            tabiwWinColor_rdo = new RadioButton();
            tabiwWinCustom_rdo = new RadioButton();
            tabiwWinImg_rdo = new RadioButton();
            tabiwBack_grp = new GroupBox();
            tabiwBack_pic = new PictureBox();
            tabiwBackColor_rdo = new RadioButton();
            tabiwBackCustom_rdo = new RadioButton();
            tabiwBackImg_rdo = new RadioButton();
            tabiwCustom_pnl = new Panel();
            tabiwCustom_lbl = new Label();
            tabiwCustom_cmb = new ComboBox();
            tabiwLineColor_lbl = new Label();
            tabiwLineColor_btn = new Button();
            tabiwLineCross_btn = new CheckBox();
            tabiwLineOutside_btn = new CheckBox();
            tabiwWinLoc_grp = new GroupBox();
            tabiwWinLocPixcelY_nud = new NumericUpDown();
            tabiwWinLocTwo_rdo = new RadioButton();
            tabiwWinLocPixcelX_nud = new NumericUpDown();
            tabiwWinLocCenter_rdo = new RadioButton();
            tabiwWinLocPixcel_rdo = new RadioButton();
            tabiwWinSize_grp = new GroupBox();
            tabiwWinSizePixelH_nud = new NumericUpDown();
            tabiwWinSizePixelW_nud = new NumericUpDown();
            tabiwWinSizePixel_rdo = new RadioButton();
            tabiwWinSizePercent_nud = new NumericUpDown();
            tabiwWinSizePercent_rdo = new RadioButton();
            tabimgeMass = new GroupBox();
            tabimgeFuncList = new ComboBox();
            tabDirList = new TabPage();
            tabdlPatternList_lvw = new ListView();
            tabImgList = new TabPage();
            tabilPatternList_lst = new ListBox();
            Showimage_flp = new FlowLayoutPanel();
            showimgSave_btn = new Button();
            showimgGenerate_btn = new Button();
            showimgPicture_pic = new PictureBox();
            showimgSize_btn = new Label();
            Statusstrip = new StatusStrip();
            ssrStatus_lbl = new ToolStripStatusLabel();
            ssrProgressbar_prg = new ToolStripProgressBar();
            Menustrip.SuspendLayout();
            tabControl.SuspendLayout();
            tabImgEditor.SuspendLayout();
            tabimgePanel.SuspendLayout();
            tabimgeMask.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tabiemHNum_nud).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tabiemWNum_nud).BeginInit();
            tabimgeAdjust.SuspendLayout();
            tabieaPanel_pnl.SuspendLayout();
            tabieaLeftRight_pnl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tabieaLeftRightL_pic).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tabieaLeftRightR_pic).BeginInit();
            tabieaUpDown_pnl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tabieaUpDownU_pic).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tabieaUpDownD_pic).BeginInit();
            tabimgeFrame.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tabiefBack_pic).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tabiefMassNum_nud).BeginInit();
            tabimgeGradient.SuspendLayout();
            tabigOther_pnl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tabigOtherSplit_nud).BeginInit();
            tabimgeChess.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tabiecVNum_nud).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tabiecHNum_nud).BeginInit();
            tabimgeWindow.SuspendLayout();
            tabiwWin_grp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tabiwWin_pic).BeginInit();
            tabiwBack_grp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tabiwBack_pic).BeginInit();
            tabiwCustom_pnl.SuspendLayout();
            tabiwWinLoc_grp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tabiwWinLocPixcelY_nud).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tabiwWinLocPixcelX_nud).BeginInit();
            tabiwWinSize_grp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)tabiwWinSizePixelH_nud).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tabiwWinSizePixelW_nud).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tabiwWinSizePercent_nud).BeginInit();
            tabDirList.SuspendLayout();
            tabImgList.SuspendLayout();
            Showimage_flp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)showimgPicture_pic).BeginInit();
            Statusstrip.SuspendLayout();
            SuspendLayout();
            // 
            // Menustrip
            // 
            Menustrip.Items.AddRange(new ToolStripItem[] { mnsImport_btn, mnsFullscreen_btn, mnsH_txt, mnsX_lbl, mnsW_txt, mnsScreenlist_cmb, mnsPattername_txt });
            Menustrip.Location = new Point(0, 0);
            Menustrip.Name = "Menustrip";
            Menustrip.Size = new Size(999, 27);
            Menustrip.TabIndex = 12;
            Menustrip.Text = "menuStrip";
            // 
            // mnsImport_btn
            // 
            mnsImport_btn.Name = "mnsImport_btn";
            mnsImport_btn.Size = new Size(57, 23);
            mnsImport_btn.Text = "Import";
            mnsImport_btn.Click += import_Click;
            // 
            // mnsFullscreen_btn
            // 
            mnsFullscreen_btn.Name = "mnsFullscreen_btn";
            mnsFullscreen_btn.Size = new Size(76, 23);
            mnsFullscreen_btn.Text = "FullScreen";
            mnsFullscreen_btn.Click += FullScreen;
            // 
            // mnsH_txt
            // 
            mnsH_txt.Alignment = ToolStripItemAlignment.Right;
            mnsH_txt.Name = "mnsH_txt";
            mnsH_txt.Size = new Size(100, 23);
            mnsH_txt.TextBoxTextAlign = HorizontalAlignment.Center;
            // 
            // mnsX_lbl
            // 
            mnsX_lbl.Alignment = ToolStripItemAlignment.Right;
            mnsX_lbl.Enabled = false;
            mnsX_lbl.Name = "mnsX_lbl";
            mnsX_lbl.ShowShortcutKeys = false;
            mnsX_lbl.Size = new Size(25, 23);
            mnsX_lbl.Text = "x";
            // 
            // mnsW_txt
            // 
            mnsW_txt.Alignment = ToolStripItemAlignment.Right;
            mnsW_txt.Name = "mnsW_txt";
            mnsW_txt.Size = new Size(100, 23);
            mnsW_txt.TextBoxTextAlign = HorizontalAlignment.Center;
            // 
            // mnsScreenlist_cmb
            // 
            mnsScreenlist_cmb.Alignment = ToolStripItemAlignment.Right;
            mnsScreenlist_cmb.DropDownStyle = ComboBoxStyle.DropDownList;
            mnsScreenlist_cmb.Name = "mnsScreenlist_cmb";
            mnsScreenlist_cmb.Size = new Size(121, 23);
            mnsScreenlist_cmb.SelectedIndexChanged += menuToolStripCmb_SelectedIndexChanged;
            mnsScreenlist_cmb.Click += Screenlist;
            // 
            // mnsPattername_txt
            // 
            mnsPattername_txt.Alignment = ToolStripItemAlignment.Right;
            mnsPattername_txt.Name = "mnsPattername_txt";
            mnsPattername_txt.Size = new Size(150, 23);
            mnsPattername_txt.Text = "PatternName";
            // 
            // tabControl
            // 
            tabControl.Controls.Add(tabImgEditor);
            tabControl.Controls.Add(tabDirList);
            tabControl.Controls.Add(tabImgList);
            tabControl.Dock = DockStyle.Left;
            tabControl.Location = new Point(0, 27);
            tabControl.Margin = new Padding(0);
            tabControl.Multiline = true;
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(668, 512);
            tabControl.TabIndex = 6;
            tabControl.SelectedIndexChanged += tabControl_SelectedIndexChanged;
            // 
            // tabImgEditor
            // 
            tabImgEditor.Controls.Add(tabimgePanel);
            tabImgEditor.Controls.Add(tabimgeFuncList);
            tabImgEditor.Location = new Point(4, 24);
            tabImgEditor.Name = "tabImgEditor";
            tabImgEditor.Padding = new Padding(3);
            tabImgEditor.Size = new Size(660, 484);
            tabImgEditor.TabIndex = 0;
            tabImgEditor.Text = "ImgEditor";
            tabImgEditor.UseVisualStyleBackColor = true;
            // 
            // tabimgePanel
            // 
            tabimgePanel.Controls.Add(tabimgeMask);
            tabimgePanel.Controls.Add(tabimgeAdjust);
            tabimgePanel.Controls.Add(tabimgeFrame);
            tabimgePanel.Controls.Add(tabimgeGradient);
            tabimgePanel.Controls.Add(tabimgeChess);
            tabimgePanel.Controls.Add(tabimgeWindow);
            tabimgePanel.Controls.Add(tabimgeMass);
            tabimgePanel.Dock = DockStyle.Bottom;
            tabimgePanel.Location = new Point(3, 26);
            tabimgePanel.Name = "tabimgePanel";
            tabimgePanel.Size = new Size(654, 455);
            tabimgePanel.TabIndex = 11;
            // 
            // tabimgeMask
            // 
            tabimgeMask.Controls.Add(button1);
            tabimgeMask.Controls.Add(tabiemPanel_pnl);
            tabimgeMask.Controls.Add(tabiemHNum_nud);
            tabimgeMask.Controls.Add(tabiemWNum_nud);
            tabimgeMask.Controls.Add(tabiemHNum_lbl);
            tabimgeMask.Controls.Add(tabiemWNum_lbl);
            tabimgeMask.Controls.Add(tabiemSubPixel_rdo);
            tabimgeMask.Controls.Add(tabiemPixel_rdo);
            tabimgeMask.Location = new Point(23, 6);
            tabimgeMask.Name = "tabimgeMask";
            tabimgeMask.Size = new Size(606, 369);
            tabimgeMask.TabIndex = 0;
            tabimgeMask.TabStop = false;
            tabimgeMask.Text = "Mask(FlickerPattern)";
            // 
            // button1
            // 
            button1.Location = new Point(363, 116);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 10;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += MaskPanelCreat;
            // 
            // tabiemPanel_pnl
            // 
            tabiemPanel_pnl.Cursor = Cursors.Cross;
            tabiemPanel_pnl.Location = new Point(28, 85);
            tabiemPanel_pnl.Name = "tabiemPanel_pnl";
            tabiemPanel_pnl.Size = new Size(223, 170);
            tabiemPanel_pnl.TabIndex = 9;
            // 
            // tabiemHNum_nud
            // 
            tabiemHNum_nud.Location = new Point(185, 49);
            tabiemHNum_nud.Name = "tabiemHNum_nud";
            tabiemHNum_nud.Size = new Size(38, 23);
            tabiemHNum_nud.TabIndex = 5;
            tabiemHNum_nud.Value = new decimal(new int[] { 2, 0, 0, 0 });
            tabiemHNum_nud.ValueChanged += MaskPanelCreat;
            // 
            // tabiemWNum_nud
            // 
            tabiemWNum_nud.Location = new Point(71, 50);
            tabiemWNum_nud.Name = "tabiemWNum_nud";
            tabiemWNum_nud.Size = new Size(40, 23);
            tabiemWNum_nud.TabIndex = 4;
            tabiemWNum_nud.Value = new decimal(new int[] { 2, 0, 0, 0 });
            tabiemWNum_nud.ValueChanged += MaskPanelCreat;
            // 
            // tabiemHNum_lbl
            // 
            tabiemHNum_lbl.AutoSize = true;
            tabiemHNum_lbl.Location = new Point(132, 52);
            tabiemHNum_lbl.Name = "tabiemHNum_lbl";
            tabiemHNum_lbl.Size = new Size(45, 15);
            tabiemHNum_lbl.TabIndex = 3;
            tabiemHNum_lbl.Text = "Height";
            // 
            // tabiemWNum_lbl
            // 
            tabiemWNum_lbl.AutoSize = true;
            tabiemWNum_lbl.Location = new Point(17, 55);
            tabiemWNum_lbl.Name = "tabiemWNum_lbl";
            tabiemWNum_lbl.Size = new Size(41, 15);
            tabiemWNum_lbl.TabIndex = 2;
            tabiemWNum_lbl.Text = "Width";
            // 
            // tabiemSubPixel_rdo
            // 
            tabiemSubPixel_rdo.AutoSize = true;
            tabiemSubPixel_rdo.Location = new Point(96, 24);
            tabiemSubPixel_rdo.Name = "tabiemSubPixel_rdo";
            tabiemSubPixel_rdo.Size = new Size(105, 19);
            tabiemSubPixel_rdo.TabIndex = 1;
            tabiemSubPixel_rdo.TabStop = true;
            tabiemSubPixel_rdo.Text = "Sub Pixel Base";
            tabiemSubPixel_rdo.UseVisualStyleBackColor = true;
            // 
            // tabiemPixel_rdo
            // 
            tabiemPixel_rdo.AutoSize = true;
            tabiemPixel_rdo.Location = new Point(10, 25);
            tabiemPixel_rdo.Name = "tabiemPixel_rdo";
            tabiemPixel_rdo.Size = new Size(80, 19);
            tabiemPixel_rdo.TabIndex = 0;
            tabiemPixel_rdo.TabStop = true;
            tabiemPixel_rdo.Text = "Pixel Base";
            tabiemPixel_rdo.UseVisualStyleBackColor = true;
            // 
            // tabimgeAdjust
            // 
            tabimgeAdjust.Controls.Add(tabieaPanel_pnl);
            tabimgeAdjust.Controls.Add(tabieaClear_btn);
            tabimgeAdjust.Controls.Add(tabieaVFlip_rdo);
            tabimgeAdjust.Controls.Add(tabieaHFlip_rdo);
            tabimgeAdjust.Controls.Add(tabieaResize_rdo);
            tabimgeAdjust.Controls.Add(tabieaImport_btn);
            tabimgeAdjust.Controls.Add(tabieaTrans_btn);
            tabimgeAdjust.Controls.Add(tabieaRotate270_rdo);
            tabimgeAdjust.Controls.Add(tabieaRotate180_rdo);
            tabimgeAdjust.Controls.Add(tabieaRotate90_rdo);
            tabimgeAdjust.Controls.Add(tabieaLeftRight_rdo);
            tabimgeAdjust.Controls.Add(tabieaUpDown_rdo);
            tabimgeAdjust.Location = new Point(454, 139);
            tabimgeAdjust.Name = "tabimgeAdjust";
            tabimgeAdjust.Size = new Size(146, 114);
            tabimgeAdjust.TabIndex = 0;
            tabimgeAdjust.TabStop = false;
            tabimgeAdjust.Text = "  ImgAdjus";
            // 
            // tabieaPanel_pnl
            // 
            tabieaPanel_pnl.Controls.Add(tabieaLeftRight_pnl);
            tabieaPanel_pnl.Controls.Add(tabieaUpDown_pnl);
            tabieaPanel_pnl.Controls.Add(tabieaPatternList_chl);
            tabieaPanel_pnl.Location = new Point(219, 21);
            tabieaPanel_pnl.Name = "tabieaPanel_pnl";
            tabieaPanel_pnl.Size = new Size(320, 320);
            tabieaPanel_pnl.TabIndex = 12;
            // 
            // tabieaLeftRight_pnl
            // 
            tabieaLeftRight_pnl.Controls.Add(tabieaLeftRightL_pic);
            tabieaLeftRight_pnl.Controls.Add(tabieaLeftRightR_pic);
            tabieaLeftRight_pnl.Location = new Point(183, 14);
            tabieaLeftRight_pnl.Name = "tabieaLeftRight_pnl";
            tabieaLeftRight_pnl.Size = new Size(97, 123);
            tabieaLeftRight_pnl.TabIndex = 9;
            // 
            // tabieaLeftRightL_pic
            // 
            tabieaLeftRightL_pic.BackgroundImage = Properties.Resources.Img;
            tabieaLeftRightL_pic.BackgroundImageLayout = ImageLayout.Zoom;
            tabieaLeftRightL_pic.BorderStyle = BorderStyle.FixedSingle;
            tabieaLeftRightL_pic.Dock = DockStyle.Left;
            tabieaLeftRightL_pic.Location = new Point(0, 0);
            tabieaLeftRightL_pic.Name = "tabieaLeftRightL_pic";
            tabieaLeftRightL_pic.Size = new Size(160, 123);
            tabieaLeftRightL_pic.SizeMode = PictureBoxSizeMode.Zoom;
            tabieaLeftRightL_pic.TabIndex = 0;
            tabieaLeftRightL_pic.TabStop = false;
            tabieaLeftRightL_pic.Click += tabiea_pic_Click;
            // 
            // tabieaLeftRightR_pic
            // 
            tabieaLeftRightR_pic.BackgroundImage = Properties.Resources.Img;
            tabieaLeftRightR_pic.BackgroundImageLayout = ImageLayout.Zoom;
            tabieaLeftRightR_pic.BorderStyle = BorderStyle.FixedSingle;
            tabieaLeftRightR_pic.Dock = DockStyle.Right;
            tabieaLeftRightR_pic.Location = new Point(-63, 0);
            tabieaLeftRightR_pic.Name = "tabieaLeftRightR_pic";
            tabieaLeftRightR_pic.Size = new Size(160, 123);
            tabieaLeftRightR_pic.SizeMode = PictureBoxSizeMode.Zoom;
            tabieaLeftRightR_pic.TabIndex = 1;
            tabieaLeftRightR_pic.TabStop = false;
            tabieaLeftRightR_pic.Click += tabiea_pic_Click;
            // 
            // tabieaUpDown_pnl
            // 
            tabieaUpDown_pnl.Controls.Add(tabieaUpDownU_pic);
            tabieaUpDown_pnl.Controls.Add(tabieaUpDownD_pic);
            tabieaUpDown_pnl.Location = new Point(19, 19);
            tabieaUpDown_pnl.Name = "tabieaUpDown_pnl";
            tabieaUpDown_pnl.Size = new Size(130, 116);
            tabieaUpDown_pnl.TabIndex = 8;
            // 
            // tabieaUpDownU_pic
            // 
            tabieaUpDownU_pic.BackgroundImage = Properties.Resources.Img;
            tabieaUpDownU_pic.BackgroundImageLayout = ImageLayout.Zoom;
            tabieaUpDownU_pic.BorderStyle = BorderStyle.FixedSingle;
            tabieaUpDownU_pic.Dock = DockStyle.Top;
            tabieaUpDownU_pic.Location = new Point(0, 0);
            tabieaUpDownU_pic.Name = "tabieaUpDownU_pic";
            tabieaUpDownU_pic.Size = new Size(130, 160);
            tabieaUpDownU_pic.SizeMode = PictureBoxSizeMode.Zoom;
            tabieaUpDownU_pic.TabIndex = 0;
            tabieaUpDownU_pic.TabStop = false;
            tabieaUpDownU_pic.Click += tabiea_pic_Click;
            // 
            // tabieaUpDownD_pic
            // 
            tabieaUpDownD_pic.BackgroundImage = Properties.Resources.Img;
            tabieaUpDownD_pic.BackgroundImageLayout = ImageLayout.Zoom;
            tabieaUpDownD_pic.BorderStyle = BorderStyle.FixedSingle;
            tabieaUpDownD_pic.Dock = DockStyle.Bottom;
            tabieaUpDownD_pic.Location = new Point(0, -44);
            tabieaUpDownD_pic.Name = "tabieaUpDownD_pic";
            tabieaUpDownD_pic.Size = new Size(130, 160);
            tabieaUpDownD_pic.SizeMode = PictureBoxSizeMode.Zoom;
            tabieaUpDownD_pic.TabIndex = 1;
            tabieaUpDownD_pic.TabStop = false;
            tabieaUpDownD_pic.Click += tabiea_pic_Click;
            // 
            // tabieaPatternList_chl
            // 
            tabieaPatternList_chl.CheckOnClick = true;
            tabieaPatternList_chl.FormattingEnabled = true;
            tabieaPatternList_chl.HorizontalScrollbar = true;
            tabieaPatternList_chl.Location = new Point(19, 171);
            tabieaPatternList_chl.Name = "tabieaPatternList_chl";
            tabieaPatternList_chl.Size = new Size(291, 94);
            tabieaPatternList_chl.TabIndex = 7;
            tabieaPatternList_chl.ItemCheck += tabieaPatternList_chL_ItemCheck;
            // 
            // tabieaClear_btn
            // 
            tabieaClear_btn.Location = new Point(116, 19);
            tabieaClear_btn.Name = "tabieaClear_btn";
            tabieaClear_btn.Size = new Size(75, 23);
            tabieaClear_btn.TabIndex = 11;
            tabieaClear_btn.Text = "Clear";
            tabieaClear_btn.UseVisualStyleBackColor = true;
            tabieaClear_btn.Click += tabieaClear_btn_Click;
            // 
            // tabieaVFlip_rdo
            // 
            tabieaVFlip_rdo.AutoSize = true;
            tabieaVFlip_rdo.Location = new Point(21, 225);
            tabieaVFlip_rdo.Name = "tabieaVFlip_rdo";
            tabieaVFlip_rdo.Size = new Size(73, 19);
            tabieaVFlip_rdo.TabIndex = 10;
            tabieaVFlip_rdo.TabStop = true;
            tabieaVFlip_rdo.Text = "垂直翻轉";
            tabieaVFlip_rdo.UseVisualStyleBackColor = true;
            tabieaVFlip_rdo.CheckedChanged += tabiea_rdo_CheckedChanged;
            // 
            // tabieaHFlip_rdo
            // 
            tabieaHFlip_rdo.AutoSize = true;
            tabieaHFlip_rdo.Location = new Point(20, 200);
            tabieaHFlip_rdo.Name = "tabieaHFlip_rdo";
            tabieaHFlip_rdo.Size = new Size(73, 19);
            tabieaHFlip_rdo.TabIndex = 9;
            tabieaHFlip_rdo.TabStop = true;
            tabieaHFlip_rdo.Text = "水平翻轉";
            tabieaHFlip_rdo.UseVisualStyleBackColor = true;
            tabieaHFlip_rdo.CheckedChanged += tabiea_rdo_CheckedChanged;
            // 
            // tabieaResize_rdo
            // 
            tabieaResize_rdo.AutoSize = true;
            tabieaResize_rdo.Location = new Point(20, 50);
            tabieaResize_rdo.Name = "tabieaResize_rdo";
            tabieaResize_rdo.Size = new Size(63, 19);
            tabieaResize_rdo.TabIndex = 8;
            tabieaResize_rdo.TabStop = true;
            tabieaResize_rdo.Text = "ReSize";
            tabieaResize_rdo.UseVisualStyleBackColor = true;
            tabieaResize_rdo.CheckedChanged += tabiea_rdo_CheckedChanged;
            // 
            // tabieaImport_btn
            // 
            tabieaImport_btn.Location = new Point(27, 19);
            tabieaImport_btn.Name = "tabieaImport_btn";
            tabieaImport_btn.Size = new Size(75, 23);
            tabieaImport_btn.TabIndex = 6;
            tabieaImport_btn.Text = "導入圖片";
            tabieaImport_btn.UseVisualStyleBackColor = true;
            tabieaImport_btn.Click += import_Click;
            // 
            // tabieaTrans_btn
            // 
            tabieaTrans_btn.Location = new Point(18, 250);
            tabieaTrans_btn.Name = "tabieaTrans_btn";
            tabieaTrans_btn.Size = new Size(75, 23);
            tabieaTrans_btn.TabIndex = 5;
            tabieaTrans_btn.Text = "Translate";
            tabieaTrans_btn.UseVisualStyleBackColor = true;
            tabieaTrans_btn.Click += tabieaTrans_btn_Click;
            // 
            // tabieaRotate270_rdo
            // 
            tabieaRotate270_rdo.AutoSize = true;
            tabieaRotate270_rdo.Location = new Point(20, 175);
            tabieaRotate270_rdo.Name = "tabieaRotate270_rdo";
            tabieaRotate270_rdo.Size = new Size(75, 19);
            tabieaRotate270_rdo.TabIndex = 4;
            tabieaRotate270_rdo.TabStop = true;
            tabieaRotate270_rdo.Text = "旋轉270°";
            tabieaRotate270_rdo.UseVisualStyleBackColor = true;
            tabieaRotate270_rdo.CheckedChanged += tabiea_rdo_CheckedChanged;
            // 
            // tabieaRotate180_rdo
            // 
            tabieaRotate180_rdo.AutoSize = true;
            tabieaRotate180_rdo.Location = new Point(20, 150);
            tabieaRotate180_rdo.Name = "tabieaRotate180_rdo";
            tabieaRotate180_rdo.Size = new Size(75, 19);
            tabieaRotate180_rdo.TabIndex = 3;
            tabieaRotate180_rdo.TabStop = true;
            tabieaRotate180_rdo.Text = "旋轉180°";
            tabieaRotate180_rdo.UseVisualStyleBackColor = true;
            tabieaRotate180_rdo.CheckedChanged += tabiea_rdo_CheckedChanged;
            // 
            // tabieaRotate90_rdo
            // 
            tabieaRotate90_rdo.AutoSize = true;
            tabieaRotate90_rdo.Location = new Point(20, 125);
            tabieaRotate90_rdo.Name = "tabieaRotate90_rdo";
            tabieaRotate90_rdo.Size = new Size(68, 19);
            tabieaRotate90_rdo.TabIndex = 2;
            tabieaRotate90_rdo.TabStop = true;
            tabieaRotate90_rdo.Text = "旋轉90°";
            tabieaRotate90_rdo.UseVisualStyleBackColor = true;
            tabieaRotate90_rdo.CheckedChanged += tabiea_rdo_CheckedChanged;
            // 
            // tabieaLeftRight_rdo
            // 
            tabieaLeftRight_rdo.AutoSize = true;
            tabieaLeftRight_rdo.Location = new Point(20, 100);
            tabieaLeftRight_rdo.Name = "tabieaLeftRight_rdo";
            tabieaLeftRight_rdo.Size = new Size(73, 19);
            tabieaLeftRight_rdo.TabIndex = 1;
            tabieaLeftRight_rdo.TabStop = true;
            tabieaLeftRight_rdo.Text = "左右組合";
            tabieaLeftRight_rdo.UseVisualStyleBackColor = true;
            tabieaLeftRight_rdo.CheckedChanged += tabiea_rdo_CheckedChanged;
            // 
            // tabieaUpDown_rdo
            // 
            tabieaUpDown_rdo.AutoSize = true;
            tabieaUpDown_rdo.Location = new Point(20, 75);
            tabieaUpDown_rdo.Name = "tabieaUpDown_rdo";
            tabieaUpDown_rdo.Size = new Size(73, 19);
            tabieaUpDown_rdo.TabIndex = 0;
            tabieaUpDown_rdo.TabStop = true;
            tabieaUpDown_rdo.Text = "上下組合";
            tabieaUpDown_rdo.UseVisualStyleBackColor = true;
            tabieaUpDown_rdo.CheckedChanged += tabiea_rdo_CheckedChanged;
            // 
            // tabimgeFrame
            // 
            tabimgeFrame.Controls.Add(tabiefBack_btn);
            tabimgeFrame.Controls.Add(tabiefLineColor_lbl);
            tabimgeFrame.Controls.Add(tabiefBack_pic);
            tabimgeFrame.Controls.Add(tabiefMassNum_nud);
            tabimgeFrame.Controls.Add(tabiefMass_chk);
            tabimgeFrame.Controls.Add(tabiefOtherColor_lbl);
            tabimgeFrame.Controls.Add(tabiefOtherLoc_lbl);
            tabimgeFrame.Controls.Add(tabiefOtherColor_btn);
            tabimgeFrame.Controls.Add(tabiefOtherClear_btn);
            tabimgeFrame.Controls.Add(tabiefOtherAdd_btn);
            tabimgeFrame.Controls.Add(tabiefOther_lst);
            tabimgeFrame.Controls.Add(tabiefOtherLoc_hsc);
            tabimgeFrame.Controls.Add(tabiefOther_cmb);
            tabimgeFrame.Controls.Add(tabiefOther_chk);
            tabimgeFrame.Controls.Add(tabiefNine_chk);
            tabimgeFrame.Controls.Add(tabiefOutside_chk);
            tabimgeFrame.Controls.Add(tabiefCenter_chk);
            tabimgeFrame.Controls.Add(tabiefLineColor_btn);
            tabimgeFrame.Controls.Add(tabiefBack_lbl);
            tabimgeFrame.ImeMode = ImeMode.On;
            tabimgeFrame.Location = new Point(235, 6);
            tabimgeFrame.Name = "tabimgeFrame";
            tabimgeFrame.Size = new Size(175, 99);
            tabimgeFrame.TabIndex = 10;
            tabimgeFrame.TabStop = false;
            tabimgeFrame.Text = "Frame&&CrossLine&&Border";
            // 
            // tabiefBack_btn
            // 
            tabiefBack_btn.BackColor = Color.Black;
            tabiefBack_btn.ForeColor = Color.Black;
            tabiefBack_btn.Location = new Point(72, 16);
            tabiefBack_btn.Name = "tabiefBack_btn";
            tabiefBack_btn.Size = new Size(24, 24);
            tabiefBack_btn.TabIndex = 9;
            tabiefBack_btn.UseVisualStyleBackColor = false;
            tabiefBack_btn.Click += SetButtonBackgroundColor;
            // 
            // tabiefLineColor_lbl
            // 
            tabiefLineColor_lbl.AutoSize = true;
            tabiefLineColor_lbl.Location = new Point(319, 70);
            tabiefLineColor_lbl.Name = "tabiefLineColor_lbl";
            tabiefLineColor_lbl.Size = new Size(55, 30);
            tabiefLineColor_lbl.TabIndex = 32;
            tabiefLineColor_lbl.Text = "\n框線顏色";
            // 
            // tabiefBack_pic
            // 
            tabiefBack_pic.BackgroundImage = Properties.Resources.Img;
            tabiefBack_pic.BackgroundImageLayout = ImageLayout.Zoom;
            tabiefBack_pic.BorderStyle = BorderStyle.FixedSingle;
            tabiefBack_pic.Location = new Point(177, 16);
            tabiefBack_pic.Name = "tabiefBack_pic";
            tabiefBack_pic.Size = new Size(48, 48);
            tabiefBack_pic.TabIndex = 31;
            tabiefBack_pic.TabStop = false;
            tabiefBack_pic.Click += import_Click;
            // 
            // tabiefMassNum_nud
            // 
            tabiefMassNum_nud.Location = new Point(408, 32);
            tabiefMassNum_nud.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            tabiefMassNum_nud.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            tabiefMassNum_nud.Name = "tabiefMassNum_nud";
            tabiefMassNum_nud.Size = new Size(45, 23);
            tabiefMassNum_nud.TabIndex = 30;
            tabiefMassNum_nud.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // tabiefMass_chk
            // 
            tabiefMass_chk.AutoSize = true;
            tabiefMass_chk.Location = new Point(312, 18);
            tabiefMass_chk.Name = "tabiefMass_chk";
            tabiefMass_chk.Size = new Size(191, 34);
            tabiefMass_chk.TabIndex = 29;
            tabiefMass_chk.Text = "大量建圖\r\n(從0開始，每                  階生成)";
            tabiefMass_chk.UseVisualStyleBackColor = true;
            // 
            // tabiefOtherColor_lbl
            // 
            tabiefOtherColor_lbl.AutoSize = true;
            tabiefOtherColor_lbl.Location = new Point(320, 115);
            tabiefOtherColor_lbl.Name = "tabiefOtherColor_lbl";
            tabiefOtherColor_lbl.Size = new Size(43, 15);
            tabiefOtherColor_lbl.TabIndex = 28;
            tabiefOtherColor_lbl.Text = "線顏色";
            // 
            // tabiefOtherLoc_lbl
            // 
            tabiefOtherLoc_lbl.AutoSize = true;
            tabiefOtherLoc_lbl.Location = new Point(319, 143);
            tabiefOtherLoc_lbl.Name = "tabiefOtherLoc_lbl";
            tabiefOtherLoc_lbl.Size = new Size(14, 15);
            tabiefOtherLoc_lbl.TabIndex = 27;
            tabiefOtherLoc_lbl.Text = "1";
            // 
            // tabiefOtherColor_btn
            // 
            tabiefOtherColor_btn.BackColor = Color.White;
            tabiefOtherColor_btn.ForeColor = Color.White;
            tabiefOtherColor_btn.Location = new Point(370, 111);
            tabiefOtherColor_btn.Name = "tabiefOtherColor_btn";
            tabiefOtherColor_btn.Size = new Size(24, 24);
            tabiefOtherColor_btn.TabIndex = 26;
            tabiefOtherColor_btn.UseVisualStyleBackColor = false;
            tabiefOtherColor_btn.Click += SetButtonBackgroundColor;
            // 
            // tabiefOtherClear_btn
            // 
            tabiefOtherClear_btn.Location = new Point(319, 197);
            tabiefOtherClear_btn.Name = "tabiefOtherClear_btn";
            tabiefOtherClear_btn.Size = new Size(75, 23);
            tabiefOtherClear_btn.TabIndex = 25;
            tabiefOtherClear_btn.Text = "Clear";
            tabiefOtherClear_btn.UseVisualStyleBackColor = true;
            tabiefOtherClear_btn.Click += gbxfBtnaddclear_Click;
            // 
            // tabiefOtherAdd_btn
            // 
            tabiefOtherAdd_btn.Location = new Point(319, 166);
            tabiefOtherAdd_btn.Name = "tabiefOtherAdd_btn";
            tabiefOtherAdd_btn.Size = new Size(75, 23);
            tabiefOtherAdd_btn.TabIndex = 24;
            tabiefOtherAdd_btn.Text = "Add";
            tabiefOtherAdd_btn.UseVisualStyleBackColor = true;
            tabiefOtherAdd_btn.Click += gbxfBtnaddclear_Click;
            // 
            // tabiefOther_lst
            // 
            tabiefOther_lst.FormattingEnabled = true;
            tabiefOther_lst.ItemHeight = 15;
            tabiefOther_lst.Location = new Point(11, 166);
            tabiefOther_lst.Name = "tabiefOther_lst";
            tabiefOther_lst.ScrollAlwaysVisible = true;
            tabiefOther_lst.Size = new Size(291, 139);
            tabiefOther_lst.TabIndex = 23;
            // 
            // tabiefOtherLoc_hsc
            // 
            tabiefOtherLoc_hsc.LargeChange = 1;
            tabiefOtherLoc_hsc.Location = new Point(11, 139);
            tabiefOtherLoc_hsc.Minimum = 1;
            tabiefOtherLoc_hsc.Name = "tabiefOtherLoc_hsc";
            tabiefOtherLoc_hsc.Size = new Size(291, 19);
            tabiefOtherLoc_hsc.TabIndex = 19;
            tabiefOtherLoc_hsc.Value = 1;
            tabiefOtherLoc_hsc.ValueChanged += gbxfHsblocation_ValueChanged;
            // 
            // tabiefOther_cmb
            // 
            tabiefOther_cmb.DropDownStyle = ComboBoxStyle.DropDownList;
            tabiefOther_cmb.FormattingEnabled = true;
            tabiefOther_cmb.Items.AddRange(new object[] { "HLine", "VLine" });
            tabiefOther_cmb.Location = new Point(106, 106);
            tabiefOther_cmb.Name = "tabiefOther_cmb";
            tabiefOther_cmb.Size = new Size(190, 23);
            tabiefOther_cmb.TabIndex = 18;
            tabiefOther_cmb.SelectedIndexChanged += tabiefOther_cmb_SelectedIndexChanged;
            // 
            // tabiefOther_chk
            // 
            tabiefOther_chk.AutoSize = true;
            tabiefOther_chk.Location = new Point(15, 110);
            tabiefOther_chk.Name = "tabiefOther_chk";
            tabiefOther_chk.Size = new Size(85, 19);
            tabiefOther_chk.TabIndex = 17;
            tabiefOther_chk.Text = "其他(H&&V)";
            tabiefOther_chk.UseVisualStyleBackColor = true;
            tabiefOther_chk.CheckStateChanged += Framechkother_CheckStateChanged;
            // 
            // tabiefNine_chk
            // 
            tabiefNine_chk.AutoSize = true;
            tabiefNine_chk.Location = new Point(176, 81);
            tabiefNine_chk.Name = "tabiefNine_chk";
            tabiefNine_chk.Size = new Size(74, 19);
            tabiefNine_chk.TabIndex = 16;
            tabiefNine_chk.Text = "九點對位";
            tabiefNine_chk.UseVisualStyleBackColor = true;
            // 
            // tabiefOutside_chk
            // 
            tabiefOutside_chk.AutoSize = true;
            tabiefOutside_chk.Location = new Point(105, 81);
            tabiefOutside_chk.Name = "tabiefOutside_chk";
            tabiefOutside_chk.Size = new Size(62, 19);
            tabiefOutside_chk.TabIndex = 15;
            tabiefOutside_chk.Text = "外框線";
            tabiefOutside_chk.UseVisualStyleBackColor = true;
            // 
            // tabiefCenter_chk
            // 
            tabiefCenter_chk.AutoSize = true;
            tabiefCenter_chk.Location = new Point(15, 81);
            tabiefCenter_chk.Name = "tabiefCenter_chk";
            tabiefCenter_chk.Size = new Size(74, 19);
            tabiefCenter_chk.TabIndex = 14;
            tabiefCenter_chk.Text = "中心對位";
            tabiefCenter_chk.UseVisualStyleBackColor = true;
            // 
            // tabiefLineColor_btn
            // 
            tabiefLineColor_btn.BackColor = Color.White;
            tabiefLineColor_btn.ForeColor = Color.White;
            tabiefLineColor_btn.Location = new Point(377, 81);
            tabiefLineColor_btn.Name = "tabiefLineColor_btn";
            tabiefLineColor_btn.Size = new Size(24, 24);
            tabiefLineColor_btn.TabIndex = 12;
            tabiefLineColor_btn.UseVisualStyleBackColor = false;
            tabiefLineColor_btn.Click += SetButtonBackgroundColor;
            // 
            // tabiefBack_lbl
            // 
            tabiefBack_lbl.AutoSize = true;
            tabiefBack_lbl.Location = new Point(14, 22);
            tabiefBack_lbl.Name = "tabiefBack_lbl";
            tabiefBack_lbl.Size = new Size(151, 15);
            tabiefBack_lbl.TabIndex = 11;
            tabiefBack_lbl.Text = "背景顏色                導入截圖";
            // 
            // tabimgeGradient
            // 
            tabimgeGradient.Controls.Add(tabigColorBar_rdo);
            tabimgeGradient.Controls.Add(tabigOther_pnl);
            tabimgeGradient.Controls.Add(tabigOther_btn);
            tabimgeGradient.Controls.Add(tabigBaseColorCustom_btn);
            tabimgeGradient.Controls.Add(tabigBaseColorB_btn);
            tabimgeGradient.Controls.Add(tabigBaseColorG_btn);
            tabimgeGradient.Controls.Add(tabigBaseColorR_btn);
            tabimgeGradient.Controls.Add(tabigBaseColorW_btn);
            tabimgeGradient.Controls.Add(tabigBaseColor_lbl);
            tabimgeGradient.Controls.Add(tabigLastLevel_cmb);
            tabimgeGradient.Controls.Add(tabigFistLevel_cmb);
            tabimgeGradient.Controls.Add(tabigLastLevel_lbl);
            tabimgeGradient.Controls.Add(tabigFistLevel_lbl);
            tabimgeGradient.Controls.Add(tabigDivid_cmb);
            tabimgeGradient.Controls.Add(tabigStep_cmb);
            tabimgeGradient.Controls.Add(tabigDivid_lbl);
            tabimgeGradient.Controls.Add(tabigStep_lbl);
            tabimgeGradient.Controls.Add(tabigHWay_rdo);
            tabimgeGradient.Controls.Add(tabigVWay_rdo);
            tabimgeGradient.Location = new Point(40, 19);
            tabimgeGradient.Name = "tabimgeGradient";
            tabimgeGradient.Size = new Size(132, 88);
            tabimgeGradient.TabIndex = 10;
            tabimgeGradient.TabStop = false;
            tabimgeGradient.Text = "Gradient";
            tabimgeGradient.Visible = false;
            // 
            // tabigColorBar_rdo
            // 
            tabigColorBar_rdo.AutoSize = true;
            tabigColorBar_rdo.Location = new Point(272, 20);
            tabigColorBar_rdo.Name = "tabigColorBar_rdo";
            tabigColorBar_rdo.Size = new Size(98, 19);
            tabigColorBar_rdo.TabIndex = 18;
            tabigColorBar_rdo.TabStop = true;
            tabigColorBar_rdo.Text = "ColorBar模式";
            tabigColorBar_rdo.UseVisualStyleBackColor = true;
            // 
            // tabigOther_pnl
            // 
            tabigOther_pnl.Controls.Add(tabigOtherSplit_lst);
            tabigOther_pnl.Controls.Add(tabigOtherColorC_lbl);
            tabigOther_pnl.Controls.Add(tabigOtherColorC_btn);
            tabigOther_pnl.Controls.Add(tabigOtherColorM_btn);
            tabigOther_pnl.Controls.Add(tabigOtherColorA_btn);
            tabigOther_pnl.Controls.Add(tabigOtherColorY_btn);
            tabigOther_pnl.Controls.Add(tabigOtherColorD_btn);
            tabigOther_pnl.Controls.Add(tabigOtherColorB_btn);
            tabigOther_pnl.Controls.Add(tabigOtherColorG_btn);
            tabigOther_pnl.Controls.Add(tabigOtherColorR_btn);
            tabigOther_pnl.Controls.Add(tabigOtherColorW_btn);
            tabigOther_pnl.Controls.Add(tabigOtherSplit_lbl);
            tabigOther_pnl.Controls.Add(tabigOtherSplit_nud);
            tabigOther_pnl.Controls.Add(tabigOtherHsplit_rdo);
            tabigOther_pnl.Controls.Add(tabigOtherVSplit_rdo);
            tabigOther_pnl.Enabled = false;
            tabigOther_pnl.Location = new Point(27, 133);
            tabigOther_pnl.Name = "tabigOther_pnl";
            tabigOther_pnl.Size = new Size(455, 173);
            tabigOther_pnl.TabIndex = 17;
            // 
            // tabigOtherSplit_lst
            // 
            tabigOtherSplit_lst.Cursor = Cursors.Cross;
            tabigOtherSplit_lst.FormattingEnabled = true;
            tabigOtherSplit_lst.ItemHeight = 15;
            tabigOtherSplit_lst.Items.AddRange(new object[] { "1", "2", "3", "4", "5", "6", "7" });
            tabigOtherSplit_lst.Location = new Point(39, 33);
            tabigOtherSplit_lst.Name = "tabigOtherSplit_lst";
            tabigOtherSplit_lst.SelectionMode = SelectionMode.MultiExtended;
            tabigOtherSplit_lst.Size = new Size(218, 109);
            tabigOtherSplit_lst.TabIndex = 22;
            // 
            // tabigOtherColorC_lbl
            // 
            tabigOtherColorC_lbl.AutoSize = true;
            tabigOtherColorC_lbl.Location = new Point(308, 105);
            tabigOtherColorC_lbl.Name = "tabigOtherColorC_lbl";
            tabigOtherColorC_lbl.Size = new Size(55, 15);
            tabigOtherColorC_lbl.TabIndex = 21;
            tabigOtherColorC_lbl.Text = "選取顏色";
            // 
            // tabigOtherColorC_btn
            // 
            tabigOtherColorC_btn.BackgroundImage = Properties.Resources.custom;
            tabigOtherColorC_btn.Location = new Point(363, 100);
            tabigOtherColorC_btn.Name = "tabigOtherColorC_btn";
            tabigOtherColorC_btn.Size = new Size(24, 24);
            tabigOtherColorC_btn.TabIndex = 18;
            tabigOtherColorC_btn.UseVisualStyleBackColor = false;
            // 
            // tabigOtherColorM_btn
            // 
            tabigOtherColorM_btn.BackColor = Color.Magenta;
            tabigOtherColorM_btn.ForeColor = Color.Black;
            tabigOtherColorM_btn.Location = new Point(393, 70);
            tabigOtherColorM_btn.Name = "tabigOtherColorM_btn";
            tabigOtherColorM_btn.Size = new Size(24, 24);
            tabigOtherColorM_btn.TabIndex = 20;
            tabigOtherColorM_btn.Text = "M";
            tabigOtherColorM_btn.UseVisualStyleBackColor = false;
            // 
            // tabigOtherColorA_btn
            // 
            tabigOtherColorA_btn.BackColor = Color.Aqua;
            tabigOtherColorA_btn.ForeColor = Color.Black;
            tabigOtherColorA_btn.Location = new Point(363, 70);
            tabigOtherColorA_btn.Name = "tabigOtherColorA_btn";
            tabigOtherColorA_btn.Size = new Size(24, 24);
            tabigOtherColorA_btn.TabIndex = 19;
            tabigOtherColorA_btn.Text = "A";
            tabigOtherColorA_btn.UseVisualStyleBackColor = false;
            // 
            // tabigOtherColorY_btn
            // 
            tabigOtherColorY_btn.BackColor = Color.Yellow;
            tabigOtherColorY_btn.ForeColor = Color.Black;
            tabigOtherColorY_btn.Location = new Point(327, 69);
            tabigOtherColorY_btn.Name = "tabigOtherColorY_btn";
            tabigOtherColorY_btn.Size = new Size(24, 24);
            tabigOtherColorY_btn.TabIndex = 18;
            tabigOtherColorY_btn.Text = "Y";
            tabigOtherColorY_btn.UseVisualStyleBackColor = false;
            // 
            // tabigOtherColorD_btn
            // 
            tabigOtherColorD_btn.BackColor = Color.Black;
            tabigOtherColorD_btn.ForeColor = Color.White;
            tabigOtherColorD_btn.Location = new Point(297, 69);
            tabigOtherColorD_btn.Name = "tabigOtherColorD_btn";
            tabigOtherColorD_btn.Size = new Size(24, 24);
            tabigOtherColorD_btn.TabIndex = 17;
            tabigOtherColorD_btn.Text = "D";
            tabigOtherColorD_btn.UseVisualStyleBackColor = false;
            // 
            // tabigOtherColorB_btn
            // 
            tabigOtherColorB_btn.BackColor = Color.Blue;
            tabigOtherColorB_btn.ForeColor = Color.White;
            tabigOtherColorB_btn.Location = new Point(393, 40);
            tabigOtherColorB_btn.Name = "tabigOtherColorB_btn";
            tabigOtherColorB_btn.Size = new Size(24, 24);
            tabigOtherColorB_btn.TabIndex = 15;
            tabigOtherColorB_btn.Text = "B";
            tabigOtherColorB_btn.UseVisualStyleBackColor = false;
            // 
            // tabigOtherColorG_btn
            // 
            tabigOtherColorG_btn.BackColor = Color.Lime;
            tabigOtherColorG_btn.ForeColor = Color.White;
            tabigOtherColorG_btn.Location = new Point(363, 40);
            tabigOtherColorG_btn.Name = "tabigOtherColorG_btn";
            tabigOtherColorG_btn.Size = new Size(24, 24);
            tabigOtherColorG_btn.TabIndex = 14;
            tabigOtherColorG_btn.Text = "G";
            tabigOtherColorG_btn.UseVisualStyleBackColor = false;
            // 
            // tabigOtherColorR_btn
            // 
            tabigOtherColorR_btn.BackColor = Color.Red;
            tabigOtherColorR_btn.ForeColor = Color.White;
            tabigOtherColorR_btn.Location = new Point(326, 39);
            tabigOtherColorR_btn.Name = "tabigOtherColorR_btn";
            tabigOtherColorR_btn.Size = new Size(24, 24);
            tabigOtherColorR_btn.TabIndex = 13;
            tabigOtherColorR_btn.Text = "R";
            tabigOtherColorR_btn.UseVisualStyleBackColor = false;
            // 
            // tabigOtherColorW_btn
            // 
            tabigOtherColorW_btn.BackColor = Color.White;
            tabigOtherColorW_btn.Location = new Point(297, 39);
            tabigOtherColorW_btn.Name = "tabigOtherColorW_btn";
            tabigOtherColorW_btn.Size = new Size(24, 24);
            tabigOtherColorW_btn.TabIndex = 12;
            tabigOtherColorW_btn.Text = "W";
            tabigOtherColorW_btn.UseVisualStyleBackColor = false;
            // 
            // tabigOtherSplit_lbl
            // 
            tabigOtherSplit_lbl.AutoSize = true;
            tabigOtherSplit_lbl.Location = new Point(245, 9);
            tabigOtherSplit_lbl.Name = "tabigOtherSplit_lbl";
            tabigOtherSplit_lbl.Size = new Size(49, 15);
            tabigOtherSplit_lbl.TabIndex = 3;
            tabigOtherSplit_lbl.Text = "Bar格數";
            // 
            // tabigOtherSplit_nud
            // 
            tabigOtherSplit_nud.Location = new Point(297, 7);
            tabigOtherSplit_nud.Name = "tabigOtherSplit_nud";
            tabigOtherSplit_nud.Size = new Size(120, 23);
            tabigOtherSplit_nud.TabIndex = 2;
            tabigOtherSplit_nud.Value = new decimal(new int[] { 8, 0, 0, 0 });
            // 
            // tabigOtherHsplit_rdo
            // 
            tabigOtherHsplit_rdo.AutoSize = true;
            tabigOtherHsplit_rdo.Location = new Point(108, 6);
            tabigOtherHsplit_rdo.Name = "tabigOtherHsplit_rdo";
            tabigOtherHsplit_rdo.Size = new Size(58, 19);
            tabigOtherHsplit_rdo.TabIndex = 1;
            tabigOtherHsplit_rdo.TabStop = true;
            tabigOtherHsplit_rdo.Text = "H分割";
            tabigOtherHsplit_rdo.UseVisualStyleBackColor = true;
            // 
            // tabigOtherVSplit_rdo
            // 
            tabigOtherVSplit_rdo.AutoSize = true;
            tabigOtherVSplit_rdo.Location = new Point(11, 8);
            tabigOtherVSplit_rdo.Name = "tabigOtherVSplit_rdo";
            tabigOtherVSplit_rdo.Size = new Size(57, 19);
            tabigOtherVSplit_rdo.TabIndex = 0;
            tabigOtherVSplit_rdo.TabStop = true;
            tabigOtherVSplit_rdo.Text = "V分割";
            tabigOtherVSplit_rdo.UseVisualStyleBackColor = true;
            // 
            // tabigOther_btn
            // 
            tabigOther_btn.Location = new Point(333, 102);
            tabigOther_btn.Name = "tabigOther_btn";
            tabigOther_btn.Size = new Size(75, 23);
            tabigOther_btn.TabIndex = 16;
            tabigOther_btn.Text = "其他顏色";
            tabigOther_btn.UseVisualStyleBackColor = true;
            tabigOther_btn.Click += tabigOther_btn_Click;
            // 
            // tabigBaseColorCustom_btn
            // 
            tabigBaseColorCustom_btn.BackgroundImage = Properties.Resources.colorbar;
            tabigBaseColorCustom_btn.Location = new Point(240, 100);
            tabigBaseColorCustom_btn.Name = "tabigBaseColorCustom_btn";
            tabigBaseColorCustom_btn.Size = new Size(24, 24);
            tabigBaseColorCustom_btn.TabIndex = 15;
            tabigBaseColorCustom_btn.UseVisualStyleBackColor = false;
            tabigBaseColorCustom_btn.Click += GradientColor_Click;
            // 
            // tabigBaseColorB_btn
            // 
            tabigBaseColorB_btn.BackColor = Color.Blue;
            tabigBaseColorB_btn.ForeColor = Color.White;
            tabigBaseColorB_btn.Location = new Point(210, 100);
            tabigBaseColorB_btn.Name = "tabigBaseColorB_btn";
            tabigBaseColorB_btn.Size = new Size(24, 24);
            tabigBaseColorB_btn.TabIndex = 14;
            tabigBaseColorB_btn.Text = "B";
            tabigBaseColorB_btn.UseVisualStyleBackColor = false;
            tabigBaseColorB_btn.Click += GradientColor_Click;
            // 
            // tabigBaseColorG_btn
            // 
            tabigBaseColorG_btn.BackColor = Color.Lime;
            tabigBaseColorG_btn.ForeColor = Color.White;
            tabigBaseColorG_btn.Location = new Point(180, 100);
            tabigBaseColorG_btn.Name = "tabigBaseColorG_btn";
            tabigBaseColorG_btn.Size = new Size(24, 24);
            tabigBaseColorG_btn.TabIndex = 13;
            tabigBaseColorG_btn.Text = "G";
            tabigBaseColorG_btn.UseVisualStyleBackColor = false;
            tabigBaseColorG_btn.Click += GradientColor_Click;
            // 
            // tabigBaseColorR_btn
            // 
            tabigBaseColorR_btn.BackColor = Color.Red;
            tabigBaseColorR_btn.ForeColor = Color.White;
            tabigBaseColorR_btn.Location = new Point(150, 100);
            tabigBaseColorR_btn.Name = "tabigBaseColorR_btn";
            tabigBaseColorR_btn.Size = new Size(24, 24);
            tabigBaseColorR_btn.TabIndex = 12;
            tabigBaseColorR_btn.Text = "R";
            tabigBaseColorR_btn.UseVisualStyleBackColor = false;
            tabigBaseColorR_btn.Click += GradientColor_Click;
            // 
            // tabigBaseColorW_btn
            // 
            tabigBaseColorW_btn.BackColor = Color.White;
            tabigBaseColorW_btn.Location = new Point(120, 100);
            tabigBaseColorW_btn.Name = "tabigBaseColorW_btn";
            tabigBaseColorW_btn.Size = new Size(24, 24);
            tabigBaseColorW_btn.TabIndex = 11;
            tabigBaseColorW_btn.Text = "W";
            tabigBaseColorW_btn.UseVisualStyleBackColor = false;
            tabigBaseColorW_btn.Click += GradientColor_Click;
            // 
            // tabigBaseColor_lbl
            // 
            tabigBaseColor_lbl.AutoSize = true;
            tabigBaseColor_lbl.BorderStyle = BorderStyle.FixedSingle;
            tabigBaseColor_lbl.Font = new Font("Microsoft JhengHei UI", 16F);
            tabigBaseColor_lbl.Location = new Point(12, 100);
            tabigBaseColor_lbl.Name = "tabigBaseColor_lbl";
            tabigBaseColor_lbl.Size = new Size(102, 30);
            tabigBaseColor_lbl.TabIndex = 10;
            tabigBaseColor_lbl.Text = "基底色彩";
            // 
            // tabigLastLevel_cmb
            // 
            tabigLastLevel_cmb.FormattingEnabled = true;
            tabigLastLevel_cmb.Items.AddRange(new object[] { "0", "31", "63", "95", "127", "159", "191", "223", "255" });
            tabigLastLevel_cmb.Location = new Point(406, 73);
            tabigLastLevel_cmb.Name = "tabigLastLevel_cmb";
            tabigLastLevel_cmb.Size = new Size(121, 23);
            tabigLastLevel_cmb.TabIndex = 9;
            // 
            // tabigFistLevel_cmb
            // 
            tabigFistLevel_cmb.FormattingEnabled = true;
            tabigFistLevel_cmb.Items.AddRange(new object[] { "0", "31", "63", "95", "127", "159", "191", "223", "255" });
            tabigFistLevel_cmb.Location = new Point(406, 43);
            tabigFistLevel_cmb.Name = "tabigFistLevel_cmb";
            tabigFistLevel_cmb.Size = new Size(121, 23);
            tabigFistLevel_cmb.TabIndex = 8;
            // 
            // tabigLastLevel_lbl
            // 
            tabigLastLevel_lbl.AutoSize = true;
            tabigLastLevel_lbl.Location = new Point(321, 77);
            tabigLastLevel_lbl.Name = "tabigLastLevel_lbl";
            tabigLastLevel_lbl.Size = new Size(58, 15);
            tabigLastLevel_lbl.TabIndex = 7;
            tabigLastLevel_lbl.Text = "LastLevel";
            // 
            // tabigFistLevel_lbl
            // 
            tabigFistLevel_lbl.AutoSize = true;
            tabigFistLevel_lbl.Location = new Point(313, 48);
            tabigFistLevel_lbl.Name = "tabigFistLevel_lbl";
            tabigFistLevel_lbl.Size = new Size(54, 15);
            tabigFistLevel_lbl.TabIndex = 6;
            tabigFistLevel_lbl.Text = "FistLevel";
            // 
            // tabigDivid_cmb
            // 
            tabigDivid_cmb.FormattingEnabled = true;
            tabigDivid_cmb.Items.AddRange(new object[] { "1", "2", "4" });
            tabigDivid_cmb.Location = new Point(138, 69);
            tabigDivid_cmb.Name = "tabigDivid_cmb";
            tabigDivid_cmb.Size = new Size(121, 23);
            tabigDivid_cmb.TabIndex = 5;
            // 
            // tabigStep_cmb
            // 
            tabigStep_cmb.FormattingEnabled = true;
            tabigStep_cmb.Items.AddRange(new object[] { "16", "32", "64", "128", "256" });
            tabigStep_cmb.Location = new Point(8, 69);
            tabigStep_cmb.Name = "tabigStep_cmb";
            tabigStep_cmb.Size = new Size(121, 23);
            tabigStep_cmb.TabIndex = 4;
            // 
            // tabigDivid_lbl
            // 
            tabigDivid_lbl.AutoSize = true;
            tabigDivid_lbl.Location = new Point(138, 47);
            tabigDivid_lbl.Name = "tabigDivid_lbl";
            tabigDivid_lbl.Size = new Size(67, 15);
            tabigDivid_lbl.TabIndex = 3;
            tabigDivid_lbl.Text = "畫面幾等分";
            // 
            // tabigStep_lbl
            // 
            tabigStep_lbl.AutoSize = true;
            tabigStep_lbl.Location = new Point(20, 51);
            tabigStep_lbl.Name = "tabigStep_lbl";
            tabigStep_lbl.Size = new Size(67, 15);
            tabigStep_lbl.TabIndex = 2;
            tabigStep_lbl.Text = "漸層分幾階";
            // 
            // tabigHWay_rdo
            // 
            tabigHWay_rdo.AutoSize = true;
            tabigHWay_rdo.Location = new Point(135, 20);
            tabigHWay_rdo.Name = "tabigHWay_rdo";
            tabigHWay_rdo.Size = new Size(111, 19);
            tabigHWay_rdo.TabIndex = 1;
            tabigHWay_rdo.TabStop = true;
            tabigHWay_rdo.Text = "Horizontal 漸層";
            tabigHWay_rdo.UseVisualStyleBackColor = true;
            // 
            // tabigVWay_rdo
            // 
            tabigVWay_rdo.AutoSize = true;
            tabigVWay_rdo.Location = new Point(10, 20);
            tabigVWay_rdo.Name = "tabigVWay_rdo";
            tabigVWay_rdo.Size = new Size(94, 19);
            tabigVWay_rdo.TabIndex = 0;
            tabigVWay_rdo.TabStop = true;
            tabigVWay_rdo.Text = "Vertical 漸層";
            tabigVWay_rdo.UseVisualStyleBackColor = true;
            // 
            // tabimgeChess
            // 
            tabimgeChess.Controls.Add(tabiecBaseColorCustom_lbl);
            tabimgeChess.Controls.Add(tabiecGray_lbl);
            tabimgeChess.Controls.Add(tabiecBaseColorCustom_btn);
            tabimgeChess.Controls.Add(tabiecGray_hsc);
            tabimgeChess.Controls.Add(tabiecBaseColorM_btn);
            tabimgeChess.Controls.Add(tabiecVNum_nud);
            tabimgeChess.Controls.Add(tabiecBaseColorA_btn);
            tabimgeChess.Controls.Add(tabiecHNum_nud);
            tabimgeChess.Controls.Add(tabiecBaseColorY_btn);
            tabimgeChess.Controls.Add(tabiecBaseColor_lbl);
            tabimgeChess.Controls.Add(tabiecVNum_lbl);
            tabimgeChess.Controls.Add(tabiecBaseColorB_btn);
            tabimgeChess.Controls.Add(tabiecHNum_lbl);
            tabimgeChess.Controls.Add(tabiecBaseColorG_btn);
            tabimgeChess.Controls.Add(tabiecBaseColorW_btn);
            tabimgeChess.Controls.Add(tabiecBaseColorR_btn);
            tabimgeChess.Location = new Point(26, 143);
            tabimgeChess.Name = "tabimgeChess";
            tabimgeChess.Size = new Size(174, 81);
            tabimgeChess.TabIndex = 0;
            tabimgeChess.TabStop = false;
            tabimgeChess.Text = "Chess";
            // 
            // tabiecBaseColorCustom_lbl
            // 
            tabiecBaseColorCustom_lbl.AutoSize = true;
            tabiecBaseColorCustom_lbl.Font = new Font("Microsoft JhengHei UI", 12F);
            tabiecBaseColorCustom_lbl.Location = new Point(245, 108);
            tabiecBaseColorCustom_lbl.Name = "tabiecBaseColorCustom_lbl";
            tabiecBaseColorCustom_lbl.Size = new Size(73, 20);
            tabiecBaseColorCustom_lbl.TabIndex = 31;
            tabiecBaseColorCustom_lbl.Text = "自訂顏色";
            // 
            // tabiecGray_lbl
            // 
            tabiecGray_lbl.AutoSize = true;
            tabiecGray_lbl.BorderStyle = BorderStyle.FixedSingle;
            tabiecGray_lbl.Font = new Font("Microsoft JhengHei UI", 20F);
            tabiecGray_lbl.ImageAlign = ContentAlignment.MiddleRight;
            tabiecGray_lbl.Location = new Point(43, 130);
            tabiecGray_lbl.Name = "tabiecGray_lbl";
            tabiecGray_lbl.Size = new Size(82, 37);
            tabiecGray_lbl.TabIndex = 38;
            tabiecGray_lbl.Text = "Gray:";
            // 
            // tabiecBaseColorCustom_btn
            // 
            tabiecBaseColorCustom_btn.BackColor = Color.Gold;
            tabiecBaseColorCustom_btn.BackgroundImage = Properties.Resources.custom;
            tabiecBaseColorCustom_btn.Location = new Point(333, 105);
            tabiecBaseColorCustom_btn.Name = "tabiecBaseColorCustom_btn";
            tabiecBaseColorCustom_btn.Size = new Size(24, 24);
            tabiecBaseColorCustom_btn.TabIndex = 27;
            tabiecBaseColorCustom_btn.UseVisualStyleBackColor = false;
            tabiecBaseColorCustom_btn.Click += GetButtonBackgroundColor;
            // 
            // tabiecGray_hsc
            // 
            tabiecGray_hsc.LargeChange = 1;
            tabiecGray_hsc.Location = new Point(43, 171);
            tabiecGray_hsc.Maximum = 255;
            tabiecGray_hsc.Name = "tabiecGray_hsc";
            tabiecGray_hsc.Size = new Size(349, 19);
            tabiecGray_hsc.TabIndex = 37;
            tabiecGray_hsc.ValueChanged += hScrollBar1_ValueChanged;
            // 
            // tabiecBaseColorM_btn
            // 
            tabiecBaseColorM_btn.BackColor = Color.Magenta;
            tabiecBaseColorM_btn.ForeColor = Color.Black;
            tabiecBaseColorM_btn.Location = new Point(333, 75);
            tabiecBaseColorM_btn.Name = "tabiecBaseColorM_btn";
            tabiecBaseColorM_btn.Size = new Size(24, 24);
            tabiecBaseColorM_btn.TabIndex = 30;
            tabiecBaseColorM_btn.Text = "M";
            tabiecBaseColorM_btn.UseVisualStyleBackColor = false;
            tabiecBaseColorM_btn.Click += GetButtonBackgroundColor;
            // 
            // tabiecVNum_nud
            // 
            tabiecVNum_nud.Location = new Point(43, 100);
            tabiecVNum_nud.Name = "tabiecVNum_nud";
            tabiecVNum_nud.Size = new Size(120, 23);
            tabiecVNum_nud.TabIndex = 36;
            tabiecVNum_nud.Value = new decimal(new int[] { 32, 0, 0, 0 });
            // 
            // tabiecBaseColorA_btn
            // 
            tabiecBaseColorA_btn.BackColor = Color.Aqua;
            tabiecBaseColorA_btn.ForeColor = Color.Black;
            tabiecBaseColorA_btn.Location = new Point(303, 75);
            tabiecBaseColorA_btn.Name = "tabiecBaseColorA_btn";
            tabiecBaseColorA_btn.Size = new Size(24, 24);
            tabiecBaseColorA_btn.TabIndex = 29;
            tabiecBaseColorA_btn.Text = "A";
            tabiecBaseColorA_btn.UseVisualStyleBackColor = false;
            tabiecBaseColorA_btn.Click += GetButtonBackgroundColor;
            // 
            // tabiecHNum_nud
            // 
            tabiecHNum_nud.Location = new Point(43, 50);
            tabiecHNum_nud.Name = "tabiecHNum_nud";
            tabiecHNum_nud.Size = new Size(120, 23);
            tabiecHNum_nud.TabIndex = 35;
            tabiecHNum_nud.Value = new decimal(new int[] { 32, 0, 0, 0 });
            // 
            // tabiecBaseColorY_btn
            // 
            tabiecBaseColorY_btn.BackColor = Color.Yellow;
            tabiecBaseColorY_btn.ForeColor = Color.Black;
            tabiecBaseColorY_btn.Location = new Point(273, 75);
            tabiecBaseColorY_btn.Name = "tabiecBaseColorY_btn";
            tabiecBaseColorY_btn.Size = new Size(24, 24);
            tabiecBaseColorY_btn.TabIndex = 28;
            tabiecBaseColorY_btn.Text = "Y";
            tabiecBaseColorY_btn.UseVisualStyleBackColor = false;
            tabiecBaseColorY_btn.Click += GetButtonBackgroundColor;
            // 
            // tabiecBaseColor_lbl
            // 
            tabiecBaseColor_lbl.AutoSize = true;
            tabiecBaseColor_lbl.Font = new Font("Microsoft JhengHei UI", 12F);
            tabiecBaseColor_lbl.Location = new Point(235, 24);
            tabiecBaseColor_lbl.Name = "tabiecBaseColor_lbl";
            tabiecBaseColor_lbl.Size = new Size(73, 20);
            tabiecBaseColor_lbl.TabIndex = 34;
            tabiecBaseColor_lbl.Text = "基底顏色";
            // 
            // tabiecVNum_lbl
            // 
            tabiecVNum_lbl.AutoSize = true;
            tabiecVNum_lbl.Location = new Point(43, 80);
            tabiecVNum_lbl.Name = "tabiecVNum_lbl";
            tabiecVNum_lbl.Size = new Size(55, 15);
            tabiecVNum_lbl.TabIndex = 33;
            tabiecVNum_lbl.Text = "垂直幾格";
            // 
            // tabiecBaseColorB_btn
            // 
            tabiecBaseColorB_btn.BackColor = Color.Blue;
            tabiecBaseColorB_btn.ForeColor = Color.White;
            tabiecBaseColorB_btn.Location = new Point(333, 45);
            tabiecBaseColorB_btn.Name = "tabiecBaseColorB_btn";
            tabiecBaseColorB_btn.Size = new Size(24, 24);
            tabiecBaseColorB_btn.TabIndex = 25;
            tabiecBaseColorB_btn.Text = "B";
            tabiecBaseColorB_btn.UseVisualStyleBackColor = false;
            tabiecBaseColorB_btn.Click += GetButtonBackgroundColor;
            // 
            // tabiecHNum_lbl
            // 
            tabiecHNum_lbl.AutoSize = true;
            tabiecHNum_lbl.Location = new Point(43, 30);
            tabiecHNum_lbl.Name = "tabiecHNum_lbl";
            tabiecHNum_lbl.Size = new Size(55, 15);
            tabiecHNum_lbl.TabIndex = 32;
            tabiecHNum_lbl.Text = "水平幾格";
            // 
            // tabiecBaseColorG_btn
            // 
            tabiecBaseColorG_btn.BackColor = Color.Lime;
            tabiecBaseColorG_btn.ForeColor = Color.White;
            tabiecBaseColorG_btn.Location = new Point(303, 45);
            tabiecBaseColorG_btn.Name = "tabiecBaseColorG_btn";
            tabiecBaseColorG_btn.Size = new Size(24, 24);
            tabiecBaseColorG_btn.TabIndex = 24;
            tabiecBaseColorG_btn.Text = "G";
            tabiecBaseColorG_btn.UseVisualStyleBackColor = false;
            tabiecBaseColorG_btn.Click += GetButtonBackgroundColor;
            // 
            // tabiecBaseColorW_btn
            // 
            tabiecBaseColorW_btn.BackColor = Color.White;
            tabiecBaseColorW_btn.Location = new Point(243, 45);
            tabiecBaseColorW_btn.Name = "tabiecBaseColorW_btn";
            tabiecBaseColorW_btn.Size = new Size(24, 24);
            tabiecBaseColorW_btn.TabIndex = 22;
            tabiecBaseColorW_btn.Text = "W";
            tabiecBaseColorW_btn.UseVisualStyleBackColor = false;
            tabiecBaseColorW_btn.Click += GetButtonBackgroundColor;
            // 
            // tabiecBaseColorR_btn
            // 
            tabiecBaseColorR_btn.BackColor = Color.Red;
            tabiecBaseColorR_btn.ForeColor = Color.White;
            tabiecBaseColorR_btn.Location = new Point(273, 45);
            tabiecBaseColorR_btn.Name = "tabiecBaseColorR_btn";
            tabiecBaseColorR_btn.Size = new Size(24, 24);
            tabiecBaseColorR_btn.TabIndex = 23;
            tabiecBaseColorR_btn.Text = "R";
            tabiecBaseColorR_btn.UseVisualStyleBackColor = false;
            tabiecBaseColorR_btn.Click += GetButtonBackgroundColor;
            // 
            // tabimgeWindow
            // 
            tabimgeWindow.Controls.Add(tabiwWin_grp);
            tabimgeWindow.Controls.Add(tabiwBack_grp);
            tabimgeWindow.Controls.Add(tabiwCustom_pnl);
            tabimgeWindow.Controls.Add(tabiwLineColor_lbl);
            tabimgeWindow.Controls.Add(tabiwLineColor_btn);
            tabimgeWindow.Controls.Add(tabiwLineCross_btn);
            tabimgeWindow.Controls.Add(tabiwLineOutside_btn);
            tabimgeWindow.Controls.Add(tabiwWinLoc_grp);
            tabimgeWindow.Controls.Add(tabiwWinSize_grp);
            tabimgeWindow.Location = new Point(460, 19);
            tabimgeWindow.Name = "tabimgeWindow";
            tabimgeWindow.Size = new Size(150, 75);
            tabimgeWindow.TabIndex = 0;
            tabimgeWindow.TabStop = false;
            tabimgeWindow.Text = "Window";
            // 
            // tabiwWin_grp
            // 
            tabiwWin_grp.Controls.Add(tabiwWin_pic);
            tabiwWin_grp.Controls.Add(tabiwWinColor_rdo);
            tabiwWin_grp.Controls.Add(tabiwWinCustom_rdo);
            tabiwWin_grp.Controls.Add(tabiwWinImg_rdo);
            tabiwWin_grp.Location = new Point(134, 16);
            tabiwWin_grp.Name = "tabiwWin_grp";
            tabiwWin_grp.Size = new Size(114, 78);
            tabiwWin_grp.TabIndex = 18;
            tabiwWin_grp.TabStop = false;
            tabiwWin_grp.Text = "方塊顏色";
            // 
            // tabiwWin_pic
            // 
            tabiwWin_pic.BackgroundImage = Properties.Resources.Img;
            tabiwWin_pic.BackgroundImageLayout = ImageLayout.Zoom;
            tabiwWin_pic.BorderStyle = BorderStyle.FixedSingle;
            tabiwWin_pic.Location = new Point(3, 21);
            tabiwWin_pic.Name = "tabiwWin_pic";
            tabiwWin_pic.Size = new Size(40, 40);
            tabiwWin_pic.SizeMode = PictureBoxSizeMode.Zoom;
            tabiwWin_pic.TabIndex = 8;
            tabiwWin_pic.TabStop = false;
            tabiwWin_pic.Click += windowPictureBox_Click;
            // 
            // tabiwWinColor_rdo
            // 
            tabiwWinColor_rdo.AutoSize = true;
            tabiwWinColor_rdo.Location = new Point(50, 14);
            tabiwWinColor_rdo.Name = "tabiwWinColor_rdo";
            tabiwWinColor_rdo.Size = new Size(49, 19);
            tabiwWinColor_rdo.TabIndex = 5;
            tabiwWinColor_rdo.TabStop = true;
            tabiwWinColor_rdo.Tag = "";
            tabiwWinColor_rdo.Text = "純色";
            tabiwWinColor_rdo.UseVisualStyleBackColor = true;
            tabiwWinColor_rdo.Click += windowPictureBox_Click;
            // 
            // tabiwWinCustom_rdo
            // 
            tabiwWinCustom_rdo.AutoSize = true;
            tabiwWinCustom_rdo.Enabled = false;
            tabiwWinCustom_rdo.Location = new Point(50, 51);
            tabiwWinCustom_rdo.Name = "tabiwWinCustom_rdo";
            tabiwWinCustom_rdo.Size = new Size(61, 19);
            tabiwWinCustom_rdo.TabIndex = 7;
            tabiwWinCustom_rdo.TabStop = true;
            tabiwWinCustom_rdo.Tag = "";
            tabiwWinCustom_rdo.Text = "自製圖";
            tabiwWinCustom_rdo.UseVisualStyleBackColor = true;
            // 
            // tabiwWinImg_rdo
            // 
            tabiwWinImg_rdo.AutoSize = true;
            tabiwWinImg_rdo.Location = new Point(50, 33);
            tabiwWinImg_rdo.Name = "tabiwWinImg_rdo";
            tabiwWinImg_rdo.Size = new Size(49, 19);
            tabiwWinImg_rdo.TabIndex = 6;
            tabiwWinImg_rdo.TabStop = true;
            tabiwWinImg_rdo.Tag = "";
            tabiwWinImg_rdo.Text = "截圖";
            tabiwWinImg_rdo.UseVisualStyleBackColor = true;
            tabiwWinImg_rdo.Click += windowPictureBox_Click;
            // 
            // tabiwBack_grp
            // 
            tabiwBack_grp.Controls.Add(tabiwBack_pic);
            tabiwBack_grp.Controls.Add(tabiwBackColor_rdo);
            tabiwBack_grp.Controls.Add(tabiwBackCustom_rdo);
            tabiwBack_grp.Controls.Add(tabiwBackImg_rdo);
            tabiwBack_grp.Location = new Point(10, 16);
            tabiwBack_grp.Name = "tabiwBack_grp";
            tabiwBack_grp.Size = new Size(118, 79);
            tabiwBack_grp.TabIndex = 17;
            tabiwBack_grp.TabStop = false;
            tabiwBack_grp.Text = "背景顏色";
            // 
            // tabiwBack_pic
            // 
            tabiwBack_pic.BackgroundImage = Properties.Resources.Img;
            tabiwBack_pic.BackgroundImageLayout = ImageLayout.Zoom;
            tabiwBack_pic.BorderStyle = BorderStyle.FixedSingle;
            tabiwBack_pic.InitialImage = null;
            tabiwBack_pic.Location = new Point(5, 22);
            tabiwBack_pic.Name = "tabiwBack_pic";
            tabiwBack_pic.Size = new Size(40, 40);
            tabiwBack_pic.SizeMode = PictureBoxSizeMode.Zoom;
            tabiwBack_pic.TabIndex = 5;
            tabiwBack_pic.TabStop = false;
            tabiwBack_pic.Click += windowPictureBox_Click;
            // 
            // tabiwBackColor_rdo
            // 
            tabiwBackColor_rdo.AutoSize = true;
            tabiwBackColor_rdo.Location = new Point(54, 16);
            tabiwBackColor_rdo.Name = "tabiwBackColor_rdo";
            tabiwBackColor_rdo.Size = new Size(49, 19);
            tabiwBackColor_rdo.TabIndex = 2;
            tabiwBackColor_rdo.TabStop = true;
            tabiwBackColor_rdo.Tag = "";
            tabiwBackColor_rdo.Text = "純色";
            tabiwBackColor_rdo.UseVisualStyleBackColor = true;
            tabiwBackColor_rdo.Click += windowPictureBox_Click;
            // 
            // tabiwBackCustom_rdo
            // 
            tabiwBackCustom_rdo.AutoSize = true;
            tabiwBackCustom_rdo.Enabled = false;
            tabiwBackCustom_rdo.Location = new Point(54, 53);
            tabiwBackCustom_rdo.Name = "tabiwBackCustom_rdo";
            tabiwBackCustom_rdo.Size = new Size(61, 19);
            tabiwBackCustom_rdo.TabIndex = 4;
            tabiwBackCustom_rdo.TabStop = true;
            tabiwBackCustom_rdo.Tag = "";
            tabiwBackCustom_rdo.Text = "自製圖";
            tabiwBackCustom_rdo.UseVisualStyleBackColor = true;
            // 
            // tabiwBackImg_rdo
            // 
            tabiwBackImg_rdo.AutoSize = true;
            tabiwBackImg_rdo.Location = new Point(54, 35);
            tabiwBackImg_rdo.Name = "tabiwBackImg_rdo";
            tabiwBackImg_rdo.Size = new Size(49, 19);
            tabiwBackImg_rdo.TabIndex = 3;
            tabiwBackImg_rdo.TabStop = true;
            tabiwBackImg_rdo.Tag = "";
            tabiwBackImg_rdo.Text = "截圖";
            tabiwBackImg_rdo.UseVisualStyleBackColor = true;
            tabiwBackImg_rdo.Click += windowPictureBox_Click;
            // 
            // tabiwCustom_pnl
            // 
            tabiwCustom_pnl.Controls.Add(tabiwCustom_lbl);
            tabiwCustom_pnl.Controls.Add(tabiwCustom_cmb);
            tabiwCustom_pnl.Location = new Point(282, 21);
            tabiwCustom_pnl.Name = "tabiwCustom_pnl";
            tabiwCustom_pnl.Size = new Size(287, 292);
            tabiwCustom_pnl.TabIndex = 14;
            tabiwCustom_pnl.Visible = false;
            // 
            // tabiwCustom_lbl
            // 
            tabiwCustom_lbl.AutoSize = true;
            tabiwCustom_lbl.Location = new Point(38, 11);
            tabiwCustom_lbl.Name = "tabiwCustom_lbl";
            tabiwCustom_lbl.Size = new Size(43, 15);
            tabiwCustom_lbl.TabIndex = 1;
            tabiwCustom_lbl.Text = "自製圖";
            // 
            // tabiwCustom_cmb
            // 
            tabiwCustom_cmb.FormattingEnabled = true;
            tabiwCustom_cmb.Location = new Point(114, 6);
            tabiwCustom_cmb.Name = "tabiwCustom_cmb";
            tabiwCustom_cmb.Size = new Size(121, 23);
            tabiwCustom_cmb.TabIndex = 0;
            // 
            // tabiwLineColor_lbl
            // 
            tabiwLineColor_lbl.AutoSize = true;
            tabiwLineColor_lbl.Location = new Point(185, 268);
            tabiwLineColor_lbl.Name = "tabiwLineColor_lbl";
            tabiwLineColor_lbl.Size = new Size(43, 15);
            tabiwLineColor_lbl.TabIndex = 13;
            tabiwLineColor_lbl.Text = "線顏色";
            // 
            // tabiwLineColor_btn
            // 
            tabiwLineColor_btn.Location = new Point(234, 265);
            tabiwLineColor_btn.Name = "tabiwLineColor_btn";
            tabiwLineColor_btn.Size = new Size(24, 24);
            tabiwLineColor_btn.TabIndex = 12;
            tabiwLineColor_btn.UseVisualStyleBackColor = true;
            tabiwLineColor_btn.Click += SetButtonBackgroundColor;
            // 
            // tabiwLineCross_btn
            // 
            tabiwLineCross_btn.AutoSize = true;
            tabiwLineCross_btn.Location = new Point(188, 240);
            tabiwLineCross_btn.Name = "tabiwLineCross_btn";
            tabiwLineCross_btn.Size = new Size(50, 19);
            tabiwLineCross_btn.TabIndex = 11;
            tabiwLineCross_btn.Text = "十字";
            tabiwLineCross_btn.UseVisualStyleBackColor = true;
            // 
            // tabiwLineOutside_btn
            // 
            tabiwLineOutside_btn.AutoSize = true;
            tabiwLineOutside_btn.Location = new Point(188, 216);
            tabiwLineOutside_btn.Name = "tabiwLineOutside_btn";
            tabiwLineOutside_btn.Size = new Size(50, 19);
            tabiwLineOutside_btn.TabIndex = 10;
            tabiwLineOutside_btn.Text = "外框";
            tabiwLineOutside_btn.UseVisualStyleBackColor = true;
            // 
            // tabiwWinLoc_grp
            // 
            tabiwWinLoc_grp.Controls.Add(tabiwWinLocPixcelY_nud);
            tabiwWinLoc_grp.Controls.Add(tabiwWinLocTwo_rdo);
            tabiwWinLoc_grp.Controls.Add(tabiwWinLocPixcelX_nud);
            tabiwWinLoc_grp.Controls.Add(tabiwWinLocCenter_rdo);
            tabiwWinLoc_grp.Controls.Add(tabiwWinLocPixcel_rdo);
            tabiwWinLoc_grp.Location = new Point(6, 206);
            tabiwWinLoc_grp.Name = "tabiwWinLoc_grp";
            tabiwWinLoc_grp.Size = new Size(171, 98);
            tabiwWinLoc_grp.TabIndex = 9;
            tabiwWinLoc_grp.TabStop = false;
            tabiwWinLoc_grp.Text = "視窗位置";
            // 
            // tabiwWinLocPixcelY_nud
            // 
            tabiwWinLocPixcelY_nud.Location = new Point(106, 67);
            tabiwWinLocPixcelY_nud.Name = "tabiwWinLocPixcelY_nud";
            tabiwWinLocPixcelY_nud.Size = new Size(59, 23);
            tabiwWinLocPixcelY_nud.TabIndex = 7;
            // 
            // tabiwWinLocTwo_rdo
            // 
            tabiwWinLocTwo_rdo.AutoSize = true;
            tabiwWinLocTwo_rdo.Location = new Point(80, 16);
            tabiwWinLocTwo_rdo.Name = "tabiwWinLocTwo_rdo";
            tabiwWinLocTwo_rdo.Size = new Size(49, 19);
            tabiwWinLocTwo_rdo.TabIndex = 1;
            tabiwWinLocTwo_rdo.TabStop = true;
            tabiwWinLocTwo_rdo.Text = "兩組";
            tabiwWinLocTwo_rdo.UseVisualStyleBackColor = true;
            // 
            // tabiwWinLocPixcelX_nud
            // 
            tabiwWinLocPixcelX_nud.Location = new Point(106, 37);
            tabiwWinLocPixcelX_nud.Name = "tabiwWinLocPixcelX_nud";
            tabiwWinLocPixcelX_nud.Size = new Size(59, 23);
            tabiwWinLocPixcelX_nud.TabIndex = 6;
            // 
            // tabiwWinLocCenter_rdo
            // 
            tabiwWinLocCenter_rdo.AutoSize = true;
            tabiwWinLocCenter_rdo.Location = new Point(10, 16);
            tabiwWinLocCenter_rdo.Name = "tabiwWinLocCenter_rdo";
            tabiwWinLocCenter_rdo.Size = new Size(49, 19);
            tabiwWinLocCenter_rdo.TabIndex = 0;
            tabiwWinLocCenter_rdo.TabStop = true;
            tabiwWinLocCenter_rdo.Text = "置中";
            tabiwWinLocCenter_rdo.UseVisualStyleBackColor = true;
            // 
            // tabiwWinLocPixcel_rdo
            // 
            tabiwWinLocPixcel_rdo.AutoSize = true;
            tabiwWinLocPixcel_rdo.Location = new Point(10, 38);
            tabiwWinLocPixcel_rdo.Name = "tabiwWinLocPixcel_rdo";
            tabiwWinLocPixcel_rdo.Size = new Size(93, 49);
            tabiwWinLocPixcel_rdo.TabIndex = 5;
            tabiwWinLocPixcel_rdo.TabStop = true;
            tabiwWinLocPixcel_rdo.Text = "                   X:\r\n指定Pixel  \r\n                   Y:";
            tabiwWinLocPixcel_rdo.UseVisualStyleBackColor = true;
            // 
            // tabiwWinSize_grp
            // 
            tabiwWinSize_grp.Controls.Add(tabiwWinSizePixelH_nud);
            tabiwWinSize_grp.Controls.Add(tabiwWinSizePixelW_nud);
            tabiwWinSize_grp.Controls.Add(tabiwWinSizePixel_rdo);
            tabiwWinSize_grp.Controls.Add(tabiwWinSizePercent_nud);
            tabiwWinSize_grp.Controls.Add(tabiwWinSizePercent_rdo);
            tabiwWinSize_grp.Location = new Point(6, 104);
            tabiwWinSize_grp.Name = "tabiwWinSize_grp";
            tabiwWinSize_grp.Size = new Size(171, 101);
            tabiwWinSize_grp.TabIndex = 8;
            tabiwWinSize_grp.TabStop = false;
            tabiwWinSize_grp.Text = "視窗大小";
            // 
            // tabiwWinSizePixelH_nud
            // 
            tabiwWinSizePixelH_nud.Location = new Point(105, 75);
            tabiwWinSizePixelH_nud.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            tabiwWinSizePixelH_nud.Name = "tabiwWinSizePixelH_nud";
            tabiwWinSizePixelH_nud.Size = new Size(60, 23);
            tabiwWinSizePixelH_nud.TabIndex = 4;
            tabiwWinSizePixelH_nud.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // tabiwWinSizePixelW_nud
            // 
            tabiwWinSizePixelW_nud.Location = new Point(104, 46);
            tabiwWinSizePixelW_nud.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            tabiwWinSizePixelW_nud.Name = "tabiwWinSizePixelW_nud";
            tabiwWinSizePixelW_nud.Size = new Size(61, 23);
            tabiwWinSizePixelW_nud.TabIndex = 3;
            tabiwWinSizePixelW_nud.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // tabiwWinSizePixel_rdo
            // 
            tabiwWinSizePixel_rdo.AutoSize = true;
            tabiwWinSizePixel_rdo.Location = new Point(10, 47);
            tabiwWinSizePixel_rdo.Name = "tabiwWinSizePixel_rdo";
            tabiwWinSizePixel_rdo.Size = new Size(94, 49);
            tabiwWinSizePixel_rdo.TabIndex = 2;
            tabiwWinSizePixel_rdo.TabStop = true;
            tabiwWinSizePixel_rdo.Text = "                  寬:\r\n指定Pixel  \r\n                  高:";
            tabiwWinSizePixel_rdo.UseVisualStyleBackColor = true;
            // 
            // tabiwWinSizePercent_nud
            // 
            tabiwWinSizePercent_nud.Location = new Point(90, 16);
            tabiwWinSizePercent_nud.Name = "tabiwWinSizePercent_nud";
            tabiwWinSizePercent_nud.Size = new Size(36, 23);
            tabiwWinSizePercent_nud.TabIndex = 1;
            // 
            // tabiwWinSizePercent_rdo
            // 
            tabiwWinSizePercent_rdo.AutoSize = true;
            tabiwWinSizePercent_rdo.Location = new Point(10, 16);
            tabiwWinSizePercent_rdo.Name = "tabiwWinSizePercent_rdo";
            tabiwWinSizePercent_rdo.Size = new Size(80, 19);
            tabiwWinSizePercent_rdo.TabIndex = 0;
            tabiwWinSizePercent_rdo.TabStop = true;
            tabiwWinSizePercent_rdo.Text = "百分比(%)";
            tabiwWinSizePercent_rdo.UseVisualStyleBackColor = true;
            // 
            // tabimgeMass
            // 
            tabimgeMass.Location = new Point(235, 143);
            tabimgeMass.Name = "tabimgeMass";
            tabimgeMass.Size = new Size(156, 72);
            tabimgeMass.TabIndex = 11;
            tabimgeMass.TabStop = false;
            tabimgeMass.Text = "MassImage";
            // 
            // tabimgeFuncList
            // 
            tabimgeFuncList.DropDownStyle = ComboBoxStyle.DropDownList;
            tabimgeFuncList.FormattingEnabled = true;
            tabimgeFuncList.Items.AddRange(new object[] { "Frame&CrossLine&Border", "Gradient&ColorBar", "Chess", "Window", "Mask(FlickerPattern)", "ImageAdjust", "MassImage" });
            tabimgeFuncList.Location = new Point(3, 3);
            tabimgeFuncList.Name = "tabimgeFuncList";
            tabimgeFuncList.Size = new Size(291, 23);
            tabimgeFuncList.TabIndex = 5;
            tabimgeFuncList.SelectedIndexChanged += cmbFunclist_SelectedIndexChanged;
            // 
            // tabDirList
            // 
            tabDirList.Controls.Add(tabdlPatternList_lvw);
            tabDirList.Location = new Point(4, 24);
            tabDirList.Name = "tabDirList";
            tabDirList.Padding = new Padding(3);
            tabDirList.Size = new Size(660, 484);
            tabDirList.TabIndex = 1;
            tabDirList.Text = "DirList";
            tabDirList.UseVisualStyleBackColor = true;
            // 
            // tabdlPatternList_lvw
            // 
            tabdlPatternList_lvw.Dock = DockStyle.Fill;
            tabdlPatternList_lvw.Location = new Point(3, 3);
            tabdlPatternList_lvw.Name = "tabdlPatternList_lvw";
            tabdlPatternList_lvw.Size = new Size(654, 478);
            tabdlPatternList_lvw.TabIndex = 11;
            tabdlPatternList_lvw.UseCompatibleStateImageBehavior = false;
            tabdlPatternList_lvw.SelectedIndexChanged += listView_SelectedIndexChanged;
            // 
            // tabImgList
            // 
            tabImgList.Controls.Add(tabilPatternList_lst);
            tabImgList.Location = new Point(4, 24);
            tabImgList.Name = "tabImgList";
            tabImgList.Padding = new Padding(3);
            tabImgList.Size = new Size(660, 484);
            tabImgList.TabIndex = 2;
            tabImgList.Text = "ImgList";
            tabImgList.UseVisualStyleBackColor = true;
            // 
            // tabilPatternList_lst
            // 
            tabilPatternList_lst.Dock = DockStyle.Fill;
            tabilPatternList_lst.FormattingEnabled = true;
            tabilPatternList_lst.ItemHeight = 15;
            tabilPatternList_lst.Items.AddRange(new object[] { "White", "Black", "Red", "Green", "Blue" });
            tabilPatternList_lst.Location = new Point(3, 3);
            tabilPatternList_lst.Name = "tabilPatternList_lst";
            tabilPatternList_lst.Size = new Size(654, 478);
            tabilPatternList_lst.TabIndex = 0;
            // 
            // Showimage_flp
            // 
            Showimage_flp.AutoSize = true;
            Showimage_flp.Controls.Add(showimgSave_btn);
            Showimage_flp.Controls.Add(showimgGenerate_btn);
            Showimage_flp.Controls.Add(showimgPicture_pic);
            Showimage_flp.Controls.Add(showimgSize_btn);
            Showimage_flp.Dock = DockStyle.Right;
            Showimage_flp.FlowDirection = FlowDirection.BottomUp;
            Showimage_flp.Location = new Point(693, 27);
            Showimage_flp.Margin = new Padding(0);
            Showimage_flp.Name = "Showimage_flp";
            Showimage_flp.Size = new Size(306, 512);
            Showimage_flp.TabIndex = 8;
            Showimage_flp.WrapContents = false;
            // 
            // showimgSave_btn
            // 
            showimgSave_btn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            showimgSave_btn.Location = new Point(228, 486);
            showimgSave_btn.Name = "showimgSave_btn";
            showimgSave_btn.Size = new Size(75, 23);
            showimgSave_btn.TabIndex = 0;
            showimgSave_btn.Text = "Save";
            showimgSave_btn.UseVisualStyleBackColor = true;
            showimgSave_btn.Click += Save_Click;
            // 
            // showimgGenerate_btn
            // 
            showimgGenerate_btn.Location = new Point(3, 457);
            showimgGenerate_btn.Name = "showimgGenerate_btn";
            showimgGenerate_btn.Size = new Size(112, 23);
            showimgGenerate_btn.TabIndex = 1;
            showimgGenerate_btn.Text = "Generate";
            showimgGenerate_btn.UseVisualStyleBackColor = true;
            showimgGenerate_btn.Click += Generate_Click;
            // 
            // showimgPicture_pic
            // 
            showimgPicture_pic.AllowDrop = true;
            showimgPicture_pic.BackColor = SystemColors.InactiveCaption;
            showimgPicture_pic.BackgroundImageLayout = ImageLayout.Center;
            showimgPicture_pic.Location = new Point(3, 283);
            showimgPicture_pic.MinimumSize = new Size(200, 0);
            showimgPicture_pic.Name = "showimgPicture_pic";
            showimgPicture_pic.Size = new Size(300, 168);
            showimgPicture_pic.TabIndex = 7;
            showimgPicture_pic.TabStop = false;
            showimgPicture_pic.DragDrop += pictureBox_DragDrop;
            showimgPicture_pic.DragEnter += pictureBox_DragEnter;
            showimgPicture_pic.DoubleClick += FullScreen;
            // 
            // showimgSize_btn
            // 
            showimgSize_btn.AutoSize = true;
            showimgSize_btn.Location = new Point(3, 265);
            showimgSize_btn.Name = "showimgSize_btn";
            showimgSize_btn.Size = new Size(0, 15);
            showimgSize_btn.TabIndex = 8;
            // 
            // Statusstrip
            // 
            Statusstrip.Items.AddRange(new ToolStripItem[] { ssrStatus_lbl, ssrProgressbar_prg });
            Statusstrip.LayoutStyle = ToolStripLayoutStyle.Flow;
            Statusstrip.Location = new Point(0, 539);
            Statusstrip.MinimumSize = new Size(300, 0);
            Statusstrip.Name = "Statusstrip";
            Statusstrip.Size = new Size(999, 20);
            Statusstrip.TabIndex = 14;
            Statusstrip.Text = "statusStrip1";
            // 
            // ssrStatus_lbl
            // 
            ssrStatus_lbl.Name = "ssrStatus_lbl";
            ssrStatus_lbl.Size = new Size(41, 15);
            ssrStatus_lbl.Text = "Status";
            // 
            // ssrProgressbar_prg
            // 
            ssrProgressbar_prg.Name = "ssrProgressbar_prg";
            ssrProgressbar_prg.RightToLeft = RightToLeft.No;
            ssrProgressbar_prg.Size = new Size(200, 16);
            ssrProgressbar_prg.Visible = false;
            // 
            // Mainwindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(999, 559);
            Controls.Add(Showimage_flp);
            Controls.Add(tabControl);
            Controls.Add(Statusstrip);
            Controls.Add(Menustrip);
            MainMenuStrip = Menustrip;
            MinimumSize = new Size(888, 555);
            Name = "Mainwindow";
            Text = "PatternMagic";
            Menustrip.ResumeLayout(false);
            Menustrip.PerformLayout();
            tabControl.ResumeLayout(false);
            tabImgEditor.ResumeLayout(false);
            tabimgePanel.ResumeLayout(false);
            tabimgeMask.ResumeLayout(false);
            tabimgeMask.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)tabiemHNum_nud).EndInit();
            ((System.ComponentModel.ISupportInitialize)tabiemWNum_nud).EndInit();
            tabimgeAdjust.ResumeLayout(false);
            tabimgeAdjust.PerformLayout();
            tabieaPanel_pnl.ResumeLayout(false);
            tabieaLeftRight_pnl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)tabieaLeftRightL_pic).EndInit();
            ((System.ComponentModel.ISupportInitialize)tabieaLeftRightR_pic).EndInit();
            tabieaUpDown_pnl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)tabieaUpDownU_pic).EndInit();
            ((System.ComponentModel.ISupportInitialize)tabieaUpDownD_pic).EndInit();
            tabimgeFrame.ResumeLayout(false);
            tabimgeFrame.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)tabiefBack_pic).EndInit();
            ((System.ComponentModel.ISupportInitialize)tabiefMassNum_nud).EndInit();
            tabimgeGradient.ResumeLayout(false);
            tabimgeGradient.PerformLayout();
            tabigOther_pnl.ResumeLayout(false);
            tabigOther_pnl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)tabigOtherSplit_nud).EndInit();
            tabimgeChess.ResumeLayout(false);
            tabimgeChess.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)tabiecVNum_nud).EndInit();
            ((System.ComponentModel.ISupportInitialize)tabiecHNum_nud).EndInit();
            tabimgeWindow.ResumeLayout(false);
            tabimgeWindow.PerformLayout();
            tabiwWin_grp.ResumeLayout(false);
            tabiwWin_grp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)tabiwWin_pic).EndInit();
            tabiwBack_grp.ResumeLayout(false);
            tabiwBack_grp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)tabiwBack_pic).EndInit();
            tabiwCustom_pnl.ResumeLayout(false);
            tabiwCustom_pnl.PerformLayout();
            tabiwWinLoc_grp.ResumeLayout(false);
            tabiwWinLoc_grp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)tabiwWinLocPixcelY_nud).EndInit();
            ((System.ComponentModel.ISupportInitialize)tabiwWinLocPixcelX_nud).EndInit();
            tabiwWinSize_grp.ResumeLayout(false);
            tabiwWinSize_grp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)tabiwWinSizePixelH_nud).EndInit();
            ((System.ComponentModel.ISupportInitialize)tabiwWinSizePixelW_nud).EndInit();
            ((System.ComponentModel.ISupportInitialize)tabiwWinSizePercent_nud).EndInit();
            tabDirList.ResumeLayout(false);
            tabImgList.ResumeLayout(false);
            Showimage_flp.ResumeLayout(false);
            Showimage_flp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)showimgPicture_pic).EndInit();
            Statusstrip.ResumeLayout(false);
            Statusstrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private MenuStrip Menustrip;
        private ToolStripMenuItem mnsImport_btn;
        private ToolStripMenuItem mnsFullscreen_btn;
        private ToolStripComboBox mnsScreenlist_cmb;
        private ToolStripTextBox mnsH_txt;
        private ToolStripMenuItem mnsX_lbl;
        private ToolStripTextBox mnsW_txt;
        private PictureBox showimgPicture_pic;
        private TabControl tabControl;
        private TabPage tabImgEditor;
        private ComboBox tabimgeFuncList;
        private TabPage tabDirList;
        private Button tabiefBack_btn;
        private FlowLayoutPanel Showimage_flp;
        private Button showimgSave_btn;
        private Button showimgGenerate_btn;
        private GroupBox tabimgeFrame;
        private GroupBox tabimgeGradient;
        private GroupBox tabimgeChess;
        private GroupBox tabimgeWindow;
        private GroupBox tabimgeMask;
        private GroupBox tabimgeAdjust;
        private Label tabiefBack_lbl;
        private Button tabiefLineColor_btn;
        private ListView tabdlPatternList_lvw;
        private Label showimgSize_btn;
        private Panel tabimgePanel;
        private ComboBox tabiefOther_cmb;
        private CheckBox tabiefOther_chk;
        private CheckBox tabiefNine_chk;
        private CheckBox tabiefOutside_chk;
        private CheckBox tabiefCenter_chk;
        private HScrollBar tabiefOtherLoc_hsc;
        private ListBox tabiefOther_lst;
        private Button tabiefOtherColor_btn;
        private Button tabiefOtherClear_btn;
        private Button tabiefOtherAdd_btn;
        private Label tabiefOtherLoc_lbl;
        private Label tabiefOtherColor_lbl;
        private RadioButton tabigHWay_rdo;
        private RadioButton tabigVWay_rdo;
        private Panel tabigOther_pnl;
        private RadioButton tabigOtherHsplit_rdo;
        private RadioButton tabigOtherVSplit_rdo;
        private Button tabigOther_btn;
        private Button tabigBaseColorCustom_btn;
        private Button tabigBaseColorB_btn;
        private Button tabigBaseColorG_btn;
        private Button tabigBaseColorR_btn;
        private Button tabigBaseColorW_btn;
        private Label tabigBaseColor_lbl;
        private ComboBox tabigLastLevel_cmb;
        private ComboBox tabigFistLevel_cmb;
        private Label tabigLastLevel_lbl;
        private Label tabigFistLevel_lbl;
        private ComboBox tabigDivid_cmb;
        private ComboBox tabigStep_cmb;
        private Label tabigDivid_lbl;
        private Label tabigStep_lbl;
        private Button tabigOtherColorM_btn;
        private Button tabigOtherColorA_btn;
        private Button tabigOtherColorY_btn;
        private Button tabigOtherColorD_btn;
        private Button tabigOtherColorB_btn;
        private Button tabigOtherColorG_btn;
        private Button tabigOtherColorR_btn;
        private Button tabigOtherColorW_btn;
        private Label tabigOtherSplit_lbl;
        private NumericUpDown tabigOtherSplit_nud;
        private ListBox tabigOtherSplit_lst;
        private Label tabigOtherColorC_lbl;
        private Button tabigOtherColorC_btn;
        private Label tabiecGray_lbl;
        private HScrollBar tabiecGray_hsc;
        private NumericUpDown tabiecVNum_nud;
        private NumericUpDown tabiecHNum_nud;
        private Label tabiecBaseColor_lbl;
        private Label tabiecVNum_lbl;
        private Label tabiecHNum_lbl;
        private RadioButton tabigColorBar_rdo;
        private Label tabiecBaseColorCustom_lbl;
        private Button tabiecBaseColorCustom_btn;
        private Button tabiecBaseColorM_btn;
        private Button tabiecBaseColorA_btn;
        private Button tabiecBaseColorY_btn;
        private Button tabiecBaseColorB_btn;
        private Button tabiecBaseColorG_btn;
        private Button tabiecBaseColorW_btn;
        private Button tabiecBaseColorR_btn;
        private RadioButton tabieaLeftRight_rdo;
        private RadioButton tabieaUpDown_rdo;
        private Label tabiwWin_lbl;
        private RadioButton tabieaRotate180_rdo;
        private RadioButton tabieaRotate90_rdo;
        private RadioButton tabieaRotate270_rdo;
        private StatusStrip Statusstrip;
        private ToolStripStatusLabel ssrStatus_lbl;
        private ToolStripProgressBar ssrProgressbar_prg;
        private Button tabieaTrans_btn;
        private CheckedListBox tabieaPatternList_chl;
        private Button tabieaImport_btn;
        private NumericUpDown tabiemHNum_nud;
        private NumericUpDown tabiemWNum_nud;
        private Label tabiemHNum_lbl;
        private Label tabiemWNum_lbl;
        private RadioButton tabiemSubPixel_rdo;
        private RadioButton tabiemPixel_rdo;
        private Panel tabiemPanel_pnl;
        private GroupBox tabiwWinLoc_grp;
        private GroupBox tabiwWinSize_grp;
        private RadioButton tabiwWinSizePixel_rdo;
        private NumericUpDown tabiwWinSizePercent_nud;
        private RadioButton tabiwWinSizePercent_rdo;
        private RadioButton tabiwWinCustom_rdo;
        private RadioButton tabiwWinImg_rdo;
        private RadioButton tabiwWinColor_rdo;
        private RadioButton tabiwBackCustom_rdo;
        private RadioButton tabiwBackImg_rdo;
        private RadioButton tabiwBackColor_rdo;
        private RadioButton tabiwWinLocCenter_rdo;
        private NumericUpDown tabiwWinSizePixelH_nud;
        private NumericUpDown tabiwWinSizePixelW_nud;
        private NumericUpDown tabiwWinLocPixcelY_nud;
        private RadioButton tabiwWinLocTwo_rdo;
        private NumericUpDown tabiwWinLocPixcelX_nud;
        private RadioButton tabiwWinLocPixcel_rdo;
        private Label tabiwLineColor_lbl;
        private Button tabiwLineColor_btn;
        private CheckBox tabiwLineCross_btn;
        private CheckBox tabiwLineOutside_btn;
        private Panel tabiwCustom_pnl;
        private Label tabiwCustom_lbl;
        private ComboBox tabiwCustom_cmb;
        private GroupBox tabimgeMass;
        private CheckBox tabiefMass_chk;
        private NumericUpDown tabiefMassNum_nud;
        private ToolStripTextBox mnsPattername_txt;
        private TabPage tabImgList;
        private ListBox tabilPatternList_lst;
        private RadioButton tabieaResize_rdo;
        private RadioButton tabieaVFlip_rdo;
        private RadioButton tabieaHFlip_rdo;
        private Panel tabiwWin_pnl;
        private PictureBox tabiwWin_pic;
        private PictureBox tabiwBack_pic;
        private GroupBox tabiwBack_grp;
        private GroupBox tabiwWin_grp;
        private Label tabiefLineColor_lbl;
        private PictureBox tabiefBack_pic;
        private Button tabieaClear_btn;
        private Panel tabieaPanel_pnl;
        private Panel tabieaLeftRight_pnl;
        private Panel tabieaUpDown_pnl;
        private PictureBox tabieaUpDownD_pic;
        private PictureBox tabieaUpDownU_pic;
        private PictureBox tabieaLeftRightR_pic;
        private PictureBox tabieaLeftRightL_pic;
        private Button button1;
    }
}
