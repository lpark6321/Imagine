using System;
using System.Drawing;
using System.Windows.Forms;

namespace Thunder
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
            this.components = new System.ComponentModel.Container();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.mnsImport_btn = new System.Windows.Forms.ToolStripMenuItem();
            this.mnsFullscreen_btn = new System.Windows.Forms.ToolStripMenuItem();
            this.mnsH_txt = new System.Windows.Forms.ToolStripTextBox();
            this.mnsX_lbl = new System.Windows.Forms.ToolStripMenuItem();
            this.mnsW_txt = new System.Windows.Forms.ToolStripTextBox();
            this.mnsScreenlist_cmb = new System.Windows.Forms.ToolStripComboBox();
            this.mnsPattername_txt = new System.Windows.Forms.ToolStripTextBox();
            this.mnsGenerate_btn = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabImgEditor = new System.Windows.Forms.TabPage();
            this.tabimgePanel = new System.Windows.Forms.Panel();
            this.tabimgeGradient = new System.Windows.Forms.GroupBox();
            this.tabigOther_chk = new System.Windows.Forms.CheckBox();
            this.tabigOther_grp = new System.Windows.Forms.GroupBox();
            this.tabigOtherSplitLoc_lbl = new System.Windows.Forms.Label();
            this.tabigOtherVSplit_rdo = new System.Windows.Forms.RadioButton();
            this.tabigOtherSplit_pnl = new System.Windows.Forms.Panel();
            this.tabigOtherHSplit_rdo = new System.Windows.Forms.RadioButton();
            this.tabigFourColor_btn = new System.Windows.Forms.Button();
            this.tabigOtherSplit_nud = new System.Windows.Forms.NumericUpDown();
            this.tabigOtherColor_lbl = new System.Windows.Forms.Label();
            this.tabigOtherSplit_lbl = new System.Windows.Forms.Label();
            this.tabigOtherColorC_btn = new System.Windows.Forms.Button();
            this.tabigOtherColorW_btn = new System.Windows.Forms.Button();
            this.tabigOtherColorM_btn = new System.Windows.Forms.Button();
            this.tabigOtherColorR_btn = new System.Windows.Forms.Button();
            this.tabigOtherColorA_btn = new System.Windows.Forms.Button();
            this.tabigOtherColorG_btn = new System.Windows.Forms.Button();
            this.tabigOtherColorY_btn = new System.Windows.Forms.Button();
            this.tabigOtherColorB_btn = new System.Windows.Forms.Button();
            this.tabigOtherColorD_btn = new System.Windows.Forms.Button();
            this.tabigStep_lbl = new System.Windows.Forms.Label();
            this.tabigStep_cmb = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabigBaseColorCustom_btn = new System.Windows.Forms.Button();
            this.tabigBaseColor_lbl = new System.Windows.Forms.Label();
            this.tabigBaseColorB_btn = new System.Windows.Forms.Button();
            this.tabigBaseColorW_btn = new System.Windows.Forms.Button();
            this.tabigBaseColorG_btn = new System.Windows.Forms.Button();
            this.tabigBaseColorR_btn = new System.Windows.Forms.Button();
            this.tabigVWay_rdo = new System.Windows.Forms.RadioButton();
            this.tabigDivid_cmb = new System.Windows.Forms.ComboBox();
            this.tabigColorBar_rdo = new System.Windows.Forms.RadioButton();
            this.tabigFirstLevel_lbl = new System.Windows.Forms.Label();
            this.tabigHWay_rdo = new System.Windows.Forms.RadioButton();
            this.tabigFirstLevel_cmb = new System.Windows.Forms.ComboBox();
            this.tabigLastLevel_cmb = new System.Windows.Forms.ComboBox();
            this.tabimgeWindow = new System.Windows.Forms.GroupBox();
            this.tabiwCustom_grp = new System.Windows.Forms.GroupBox();
            this.tabiwCustomGrad_grp = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.tabiwCGradOtherSplit_pnl = new System.Windows.Forms.Panel();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.button1 = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.tabiwCGrad0 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.button11 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.button12 = new System.Windows.Forms.Button();
            this.button13 = new System.Windows.Forms.Button();
            this.button14 = new System.Windows.Forms.Button();
            this.button15 = new System.Windows.Forms.Button();
            this.tabiwCGradVWay_rdo = new System.Windows.Forms.RadioButton();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.tabiwCGrad = new System.Windows.Forms.RadioButton();
            this.tabiwCGradHWay_rdo = new System.Windows.Forms.RadioButton();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.comboBox4 = new System.Windows.Forms.ComboBox();
            this.tabiwCustom_cmb = new System.Windows.Forms.ComboBox();
            this.tabiwCustomMask_grp = new System.Windows.Forms.GroupBox();
            this.tabiwCMaskPixelGray_lbl = new System.Windows.Forms.Label();
            this.tabiwCMaskSubPixel_rdo = new System.Windows.Forms.RadioButton();
            this.tabiwCMaskPixel_rdo = new System.Windows.Forms.RadioButton();
            this.tabiwCMaskPixelGray_vsc = new System.Windows.Forms.VScrollBar();
            this.tabiwCMaskPixelColor_lbl = new System.Windows.Forms.Label();
            this.tabiwCMaskPixelLoc_lbl = new System.Windows.Forms.Label();
            this.tabiwCMaskPixelColor_btn = new System.Windows.Forms.Button();
            this.tabiwCMaskPixelClear_btn = new System.Windows.Forms.Button();
            this.tabiwCMaskPixelPanel_pnl = new System.Windows.Forms.Panel();
            this.tabiwCMaskHNum_nud = new System.Windows.Forms.NumericUpDown();
            this.tabiwCMaskWNum_nud = new System.Windows.Forms.NumericUpDown();
            this.tabiwCMaskHNum_lbl = new System.Windows.Forms.Label();
            this.tabiwCMaskWNum_lbl = new System.Windows.Forms.Label();
            this.tabiwWin_grp = new System.Windows.Forms.GroupBox();
            this.tabiwWin_pic = new System.Windows.Forms.PictureBox();
            this.tabiwWinColor_rdo = new System.Windows.Forms.RadioButton();
            this.tabiwWinCustom_rdo = new System.Windows.Forms.RadioButton();
            this.tabiwWinImg_rdo = new System.Windows.Forms.RadioButton();
            this.tabiwBack_grp = new System.Windows.Forms.GroupBox();
            this.tabiwBack_pic = new System.Windows.Forms.PictureBox();
            this.tabiwBackColor_rdo = new System.Windows.Forms.RadioButton();
            this.tabiwBackCustom_rdo = new System.Windows.Forms.RadioButton();
            this.tabiwBackImg_rdo = new System.Windows.Forms.RadioButton();
            this.tabiwLineColor_lbl = new System.Windows.Forms.Label();
            this.tabiwLineColor_btn = new System.Windows.Forms.Button();
            this.tabiwLineCross_btn = new System.Windows.Forms.CheckBox();
            this.tabiwLineOutside_btn = new System.Windows.Forms.CheckBox();
            this.tabiwWinLoc_grp = new System.Windows.Forms.GroupBox();
            this.tabiwWinLocPixcelY_nud = new System.Windows.Forms.NumericUpDown();
            this.tabiwWinLocTwo_rdo = new System.Windows.Forms.RadioButton();
            this.tabiwWinLocPixcelX_nud = new System.Windows.Forms.NumericUpDown();
            this.tabiwWinLocCenter_rdo = new System.Windows.Forms.RadioButton();
            this.tabiwWinLocPixcel_rdo = new System.Windows.Forms.RadioButton();
            this.tabiwWinSize_grp = new System.Windows.Forms.GroupBox();
            this.tabiwWinSizePixelH_nud = new System.Windows.Forms.NumericUpDown();
            this.tabiwWinSizePixelW_nud = new System.Windows.Forms.NumericUpDown();
            this.tabiwWinSizePixel_rdo = new System.Windows.Forms.RadioButton();
            this.tabiwWinSizePercent_nud = new System.Windows.Forms.NumericUpDown();
            this.tabiwWinSizePercent_rdo = new System.Windows.Forms.RadioButton();
            this.tabimgeMask = new System.Windows.Forms.GroupBox();
            this.tabiemPixelGray_lbl = new System.Windows.Forms.Label();
            this.tabiemPixelGray_vsc = new System.Windows.Forms.VScrollBar();
            this.tabiemPixelColor_lbl = new System.Windows.Forms.Label();
            this.tabiemPixelLoc_lbl = new System.Windows.Forms.Label();
            this.tabiemPixelColor_btn = new System.Windows.Forms.Button();
            this.tabiemPixelClear_btn = new System.Windows.Forms.Button();
            this.tabiemPixelPanel_pnl = new System.Windows.Forms.Panel();
            this.tabiemHNum_nud = new System.Windows.Forms.NumericUpDown();
            this.tabiemWNum_nud = new System.Windows.Forms.NumericUpDown();
            this.tabiemHNum_lbl = new System.Windows.Forms.Label();
            this.tabiemWNum_lbl = new System.Windows.Forms.Label();
            this.tabiemSubPixel_rdo = new System.Windows.Forms.RadioButton();
            this.tabiemPixel_rdo = new System.Windows.Forms.RadioButton();
            this.tabimgeChess = new System.Windows.Forms.GroupBox();
            this.tabiecBaseColorCustom_lbl = new System.Windows.Forms.Label();
            this.tabiecGray_lbl = new System.Windows.Forms.Label();
            this.tabiecBaseColorCustom_btn = new System.Windows.Forms.Button();
            this.tabiecGray_hsc = new System.Windows.Forms.HScrollBar();
            this.tabiecBaseColorM_btn = new System.Windows.Forms.Button();
            this.tabiecVNum_nud = new System.Windows.Forms.NumericUpDown();
            this.tabiecBaseColorA_btn = new System.Windows.Forms.Button();
            this.tabiecHNum_nud = new System.Windows.Forms.NumericUpDown();
            this.tabiecBaseColorY_btn = new System.Windows.Forms.Button();
            this.tabiecBaseColor_lbl = new System.Windows.Forms.Label();
            this.tabiecVNum_lbl = new System.Windows.Forms.Label();
            this.tabiecBaseColorB_btn = new System.Windows.Forms.Button();
            this.tabiecHNum_lbl = new System.Windows.Forms.Label();
            this.tabiecBaseColorG_btn = new System.Windows.Forms.Button();
            this.tabiecBaseColorW_btn = new System.Windows.Forms.Button();
            this.tabiecBaseColorR_btn = new System.Windows.Forms.Button();
            this.tabimgeFrame = new System.Windows.Forms.GroupBox();
            this.tabiefOther_pnl = new System.Windows.Forms.Panel();
            this.tabiefOther_cmb = new System.Windows.Forms.ComboBox();
            this.tabiefOtherLoc_hsc = new System.Windows.Forms.HScrollBar();
            this.tabiefOther_lst = new System.Windows.Forms.ListBox();
            this.tabiefOtherAdd_btn = new System.Windows.Forms.Button();
            this.tabiefOtherClear_btn = new System.Windows.Forms.Button();
            this.tabiefOtherColor_btn = new System.Windows.Forms.Button();
            this.tabiefOtherColor_lbl = new System.Windows.Forms.Label();
            this.tabiefOtherLoc_lbl = new System.Windows.Forms.Label();
            this.tabiefBack_btn = new System.Windows.Forms.Button();
            this.tabiefLineColor_lbl = new System.Windows.Forms.Label();
            this.tabiefBack_pic = new System.Windows.Forms.PictureBox();
            this.tabiefMassNum_nud = new System.Windows.Forms.NumericUpDown();
            this.tabiefMass_chk = new System.Windows.Forms.CheckBox();
            this.tabiefOther_chk = new System.Windows.Forms.CheckBox();
            this.tabiefNine_chk = new System.Windows.Forms.CheckBox();
            this.tabiefOutside_chk = new System.Windows.Forms.CheckBox();
            this.tabiefCenter_chk = new System.Windows.Forms.CheckBox();
            this.tabiefLineColor_btn = new System.Windows.Forms.Button();
            this.tabiefBack_lbl = new System.Windows.Forms.Label();
            this.tabimgeAdjust = new System.Windows.Forms.GroupBox();
            this.tabieaPanel_pnl = new System.Windows.Forms.Panel();
            this.tabieaLeftRight_pnl = new System.Windows.Forms.Panel();
            this.tabieaLeftRightR_pic = new System.Windows.Forms.PictureBox();
            this.tabieaLeftRightL_pic = new System.Windows.Forms.PictureBox();
            this.tabieaUpDown_pnl = new System.Windows.Forms.Panel();
            this.tabieaUpDownD_pic = new System.Windows.Forms.PictureBox();
            this.tabieaUpDownU_pic = new System.Windows.Forms.PictureBox();
            this.tabieaPatternList_chl = new System.Windows.Forms.CheckedListBox();
            this.tabieaClear_btn = new System.Windows.Forms.Button();
            this.tabieaVFlip_rdo = new System.Windows.Forms.RadioButton();
            this.tabieaHFlip_rdo = new System.Windows.Forms.RadioButton();
            this.tabieaResize_rdo = new System.Windows.Forms.RadioButton();
            this.tabieaImport_btn = new System.Windows.Forms.Button();
            this.tabieaTrans_btn = new System.Windows.Forms.Button();
            this.tabieaRotate270_rdo = new System.Windows.Forms.RadioButton();
            this.tabieaRotate180_rdo = new System.Windows.Forms.RadioButton();
            this.tabieaRotate90_rdo = new System.Windows.Forms.RadioButton();
            this.tabieaLeftRight_rdo = new System.Windows.Forms.RadioButton();
            this.tabieaUpDown_rdo = new System.Windows.Forms.RadioButton();
            this.tabimgeMass = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabimgeFuncList = new System.Windows.Forms.ComboBox();
            this.tabDirList = new System.Windows.Forms.TabPage();
            this.tabdlPatternList_lvw = new System.Windows.Forms.ListView();
            this.tabImgList = new System.Windows.Forms.TabPage();
            this.tabilPatternList_lst = new System.Windows.Forms.ListBox();
            this.Showimage_flp = new System.Windows.Forms.FlowLayoutPanel();
            this.showimgSave_btn = new System.Windows.Forms.Button();
            this.showimgGenerate_btn = new System.Windows.Forms.Button();
            this.showimgPicture_pic = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmsShowLoc = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cmsRotate = new System.Windows.Forms.ToolStripMenuItem();
            this.旋轉ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.旋轉180ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.旋轉270ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.水平翻轉ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.垂直翻轉ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsToBeContinued = new System.Windows.Forms.ToolStripMenuItem();
            this.cms = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.showimgSize_btn = new System.Windows.Forms.Label();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.ssrStatus_lbl = new System.Windows.Forms.ToolStripStatusLabel();
            this.ssrProgressbar_prg = new System.Windows.Forms.ToolStripProgressBar();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.menuStrip.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabImgEditor.SuspendLayout();
            this.tabimgePanel.SuspendLayout();
            this.tabimgeGradient.SuspendLayout();
            this.tabigOther_grp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabigOtherSplit_nud)).BeginInit();
            this.panel1.SuspendLayout();
            this.tabimgeWindow.SuspendLayout();
            this.tabiwCustom_grp.SuspendLayout();
            this.tabiwCustomGrad_grp.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.panel3.SuspendLayout();
            this.tabiwCustomMask_grp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabiwCMaskHNum_nud)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabiwCMaskWNum_nud)).BeginInit();
            this.tabiwWin_grp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabiwWin_pic)).BeginInit();
            this.tabiwBack_grp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabiwBack_pic)).BeginInit();
            this.tabiwWinLoc_grp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabiwWinLocPixcelY_nud)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabiwWinLocPixcelX_nud)).BeginInit();
            this.tabiwWinSize_grp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabiwWinSizePixelH_nud)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabiwWinSizePixelW_nud)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabiwWinSizePercent_nud)).BeginInit();
            this.tabimgeMask.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabiemHNum_nud)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabiemWNum_nud)).BeginInit();
            this.tabimgeChess.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabiecVNum_nud)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabiecHNum_nud)).BeginInit();
            this.tabimgeFrame.SuspendLayout();
            this.tabiefOther_pnl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabiefBack_pic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabiefMassNum_nud)).BeginInit();
            this.tabimgeAdjust.SuspendLayout();
            this.tabieaPanel_pnl.SuspendLayout();
            this.tabieaLeftRight_pnl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabieaLeftRightR_pic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabieaLeftRightL_pic)).BeginInit();
            this.tabieaUpDown_pnl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabieaUpDownD_pic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabieaUpDownU_pic)).BeginInit();
            this.tabimgeMass.SuspendLayout();
            this.tabDirList.SuspendLayout();
            this.tabImgList.SuspendLayout();
            this.Showimage_flp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.showimgPicture_pic)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnsImport_btn,
            this.mnsFullscreen_btn,
            this.mnsH_txt,
            this.mnsX_lbl,
            this.mnsW_txt,
            this.mnsScreenlist_cmb,
            this.mnsPattername_txt,
            this.mnsGenerate_btn});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip.Size = new System.Drawing.Size(1392, 27);
            this.menuStrip.TabIndex = 12;
            this.menuStrip.Text = "menuStrip";
            // 
            // mnsImport_btn
            // 
            this.mnsImport_btn.Name = "mnsImport_btn";
            this.mnsImport_btn.Size = new System.Drawing.Size(57, 23);
            this.mnsImport_btn.Text = "Import";
            this.mnsImport_btn.Click += new System.EventHandler(this.import_Click);
            // 
            // mnsFullscreen_btn
            // 
            this.mnsFullscreen_btn.Name = "mnsFullscreen_btn";
            this.mnsFullscreen_btn.Size = new System.Drawing.Size(76, 23);
            this.mnsFullscreen_btn.Text = "FullScreen";
            this.mnsFullscreen_btn.Click += new System.EventHandler(this.FullScreen);
            // 
            // mnsH_txt
            // 
            this.mnsH_txt.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.mnsH_txt.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F);
            this.mnsH_txt.Name = "mnsH_txt";
            this.mnsH_txt.Size = new System.Drawing.Size(86, 23);
            this.mnsH_txt.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // mnsX_lbl
            // 
            this.mnsX_lbl.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.mnsX_lbl.Enabled = false;
            this.mnsX_lbl.Name = "mnsX_lbl";
            this.mnsX_lbl.ShowShortcutKeys = false;
            this.mnsX_lbl.Size = new System.Drawing.Size(25, 23);
            this.mnsX_lbl.Text = "x";
            // 
            // mnsW_txt
            // 
            this.mnsW_txt.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.mnsW_txt.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F);
            this.mnsW_txt.Name = "mnsW_txt";
            this.mnsW_txt.Size = new System.Drawing.Size(86, 23);
            this.mnsW_txt.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // mnsScreenlist_cmb
            // 
            this.mnsScreenlist_cmb.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.mnsScreenlist_cmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mnsScreenlist_cmb.Name = "mnsScreenlist_cmb";
            this.mnsScreenlist_cmb.Size = new System.Drawing.Size(104, 23);
            this.mnsScreenlist_cmb.SelectedIndexChanged += new System.EventHandler(this.menuToolStripCmb_SelectedIndexChanged);
            this.mnsScreenlist_cmb.Click += new System.EventHandler(this.Screenlist);
            // 
            // mnsPattername_txt
            // 
            this.mnsPattername_txt.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.mnsPattername_txt.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F);
            this.mnsPattername_txt.Name = "mnsPattername_txt";
            this.mnsPattername_txt.Size = new System.Drawing.Size(129, 23);
            this.mnsPattername_txt.Text = "PatternName";
            // 
            // mnsGenerate_btn
            // 
            this.mnsGenerate_btn.Name = "mnsGenerate_btn";
            this.mnsGenerate_btn.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Space)));
            this.mnsGenerate_btn.Size = new System.Drawing.Size(71, 23);
            this.mnsGenerate_btn.Text = "Generate";
            this.mnsGenerate_btn.Click += new System.EventHandler(this.Generate_Click);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabImgEditor);
            this.tabControl.Controls.Add(this.tabDirList);
            this.tabControl.Controls.Add(this.tabImgList);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Left;
            this.tabControl.Location = new System.Drawing.Point(0, 27);
            this.tabControl.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl.Multiline = true;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(807, 516);
            this.tabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl.TabIndex = 6;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tabImgEditor
            // 
            this.tabImgEditor.Controls.Add(this.tabimgePanel);
            this.tabImgEditor.Controls.Add(this.tabimgeFuncList);
            this.tabImgEditor.Location = new System.Drawing.Point(4, 22);
            this.tabImgEditor.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabImgEditor.Name = "tabImgEditor";
            this.tabImgEditor.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabImgEditor.Size = new System.Drawing.Size(799, 490);
            this.tabImgEditor.TabIndex = 0;
            this.tabImgEditor.Text = "ImgEditor";
            this.tabImgEditor.UseVisualStyleBackColor = true;
            // 
            // tabimgePanel
            // 
            this.tabimgePanel.Controls.Add(this.tabimgeGradient);
            this.tabimgePanel.Controls.Add(this.tabimgeWindow);
            this.tabimgePanel.Controls.Add(this.tabimgeMask);
            this.tabimgePanel.Controls.Add(this.tabimgeChess);
            this.tabimgePanel.Controls.Add(this.tabimgeFrame);
            this.tabimgePanel.Controls.Add(this.tabimgeAdjust);
            this.tabimgePanel.Controls.Add(this.tabimgeMass);
            this.tabimgePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabimgePanel.Location = new System.Drawing.Point(3, 26);
            this.tabimgePanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabimgePanel.Name = "tabimgePanel";
            this.tabimgePanel.Size = new System.Drawing.Size(793, 462);
            this.tabimgePanel.TabIndex = 11;
            // 
            // tabimgeGradient
            // 
            this.tabimgeGradient.Controls.Add(this.tabigOther_chk);
            this.tabimgeGradient.Controls.Add(this.tabigOther_grp);
            this.tabimgeGradient.Controls.Add(this.tabigStep_lbl);
            this.tabimgeGradient.Controls.Add(this.tabigStep_cmb);
            this.tabimgeGradient.Controls.Add(this.panel1);
            this.tabimgeGradient.Controls.Add(this.tabigVWay_rdo);
            this.tabimgeGradient.Controls.Add(this.tabigDivid_cmb);
            this.tabimgeGradient.Controls.Add(this.tabigColorBar_rdo);
            this.tabimgeGradient.Controls.Add(this.tabigFirstLevel_lbl);
            this.tabimgeGradient.Controls.Add(this.tabigHWay_rdo);
            this.tabimgeGradient.Controls.Add(this.tabigFirstLevel_cmb);
            this.tabimgeGradient.Controls.Add(this.tabigLastLevel_cmb);
            this.tabimgeGradient.Location = new System.Drawing.Point(276, 282);
            this.tabimgeGradient.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabimgeGradient.Name = "tabimgeGradient";
            this.tabimgeGradient.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabimgeGradient.Size = new System.Drawing.Size(207, 140);
            this.tabimgeGradient.TabIndex = 10;
            this.tabimgeGradient.TabStop = false;
            this.tabimgeGradient.Text = "Gradient";
            this.tabimgeGradient.Visible = false;
            // 
            // tabigOther_chk
            // 
            this.tabigOther_chk.AutoSize = true;
            this.tabigOther_chk.Location = new System.Drawing.Point(313, 100);
            this.tabigOther_chk.Name = "tabigOther_chk";
            this.tabigOther_chk.Size = new System.Drawing.Size(72, 16);
            this.tabigOther_chk.TabIndex = 24;
            this.tabigOther_chk.Text = "其他顏色";
            this.tabigOther_chk.UseVisualStyleBackColor = true;
            this.tabigOther_chk.CheckedChanged += new System.EventHandler(this.otherColor_CheckedChanged);
            // 
            // tabigOther_grp
            // 
            this.tabigOther_grp.Controls.Add(this.tabigOtherSplitLoc_lbl);
            this.tabigOther_grp.Controls.Add(this.tabigOtherVSplit_rdo);
            this.tabigOther_grp.Controls.Add(this.tabigOtherSplit_pnl);
            this.tabigOther_grp.Controls.Add(this.tabigOtherHSplit_rdo);
            this.tabigOther_grp.Controls.Add(this.tabigFourColor_btn);
            this.tabigOther_grp.Controls.Add(this.tabigOtherSplit_nud);
            this.tabigOther_grp.Controls.Add(this.tabigOtherColor_lbl);
            this.tabigOther_grp.Controls.Add(this.tabigOtherSplit_lbl);
            this.tabigOther_grp.Controls.Add(this.tabigOtherColorC_btn);
            this.tabigOther_grp.Controls.Add(this.tabigOtherColorW_btn);
            this.tabigOther_grp.Controls.Add(this.tabigOtherColorM_btn);
            this.tabigOther_grp.Controls.Add(this.tabigOtherColorR_btn);
            this.tabigOther_grp.Controls.Add(this.tabigOtherColorA_btn);
            this.tabigOther_grp.Controls.Add(this.tabigOtherColorG_btn);
            this.tabigOther_grp.Controls.Add(this.tabigOtherColorY_btn);
            this.tabigOther_grp.Controls.Add(this.tabigOtherColorB_btn);
            this.tabigOther_grp.Controls.Add(this.tabigOtherColorD_btn);
            this.tabigOther_grp.Enabled = false;
            this.tabigOther_grp.Location = new System.Drawing.Point(10, 129);
            this.tabigOther_grp.Name = "tabigOther_grp";
            this.tabigOther_grp.Size = new System.Drawing.Size(397, 199);
            this.tabigOther_grp.TabIndex = 23;
            this.tabigOther_grp.TabStop = false;
            // 
            // tabigOtherSplitLoc_lbl
            // 
            this.tabigOtherSplitLoc_lbl.AutoSize = true;
            this.tabigOtherSplitLoc_lbl.Location = new System.Drawing.Point(215, 160);
            this.tabigOtherSplitLoc_lbl.Name = "tabigOtherSplitLoc_lbl";
            this.tabigOtherSplitLoc_lbl.Size = new System.Drawing.Size(24, 12);
            this.tabigOtherSplitLoc_lbl.TabIndex = 23;
            this.tabigOtherSplitLoc_lbl.Text = "--,--";
            // 
            // tabigOtherVSplit_rdo
            // 
            this.tabigOtherVSplit_rdo.AutoSize = true;
            this.tabigOtherVSplit_rdo.Checked = true;
            this.tabigOtherVSplit_rdo.Location = new System.Drawing.Point(7, 18);
            this.tabigOtherVSplit_rdo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabigOtherVSplit_rdo.Name = "tabigOtherVSplit_rdo";
            this.tabigOtherVSplit_rdo.Size = new System.Drawing.Size(55, 16);
            this.tabigOtherVSplit_rdo.TabIndex = 0;
            this.tabigOtherVSplit_rdo.TabStop = true;
            this.tabigOtherVSplit_rdo.Text = "V分割";
            this.tabigOtherVSplit_rdo.UseVisualStyleBackColor = true;
            this.tabigOtherVSplit_rdo.Click += new System.EventHandler(this.SplitPanelCreat);
            // 
            // tabigOtherSplit_pnl
            // 
            this.tabigOtherSplit_pnl.Location = new System.Drawing.Point(8, 40);
            this.tabigOtherSplit_pnl.Name = "tabigOtherSplit_pnl";
            this.tabigOtherSplit_pnl.Size = new System.Drawing.Size(200, 140);
            this.tabigOtherSplit_pnl.TabIndex = 22;
            // 
            // tabigOtherHSplit_rdo
            // 
            this.tabigOtherHSplit_rdo.AutoSize = true;
            this.tabigOtherHSplit_rdo.Location = new System.Drawing.Point(91, 17);
            this.tabigOtherHSplit_rdo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabigOtherHSplit_rdo.Name = "tabigOtherHSplit_rdo";
            this.tabigOtherHSplit_rdo.Size = new System.Drawing.Size(55, 16);
            this.tabigOtherHSplit_rdo.TabIndex = 1;
            this.tabigOtherHSplit_rdo.Text = "H分割";
            this.tabigOtherHSplit_rdo.UseVisualStyleBackColor = true;
            this.tabigOtherHSplit_rdo.Click += new System.EventHandler(this.SplitPanelCreat);
            // 
            // tabigFourColor_btn
            // 
            this.tabigFourColor_btn.Location = new System.Drawing.Point(253, 123);
            this.tabigFourColor_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabigFourColor_btn.Name = "tabigFourColor_btn";
            this.tabigFourColor_btn.Size = new System.Drawing.Size(61, 21);
            this.tabigFourColor_btn.TabIndex = 16;
            this.tabigFourColor_btn.Text = "常規四色";
            this.tabigFourColor_btn.UseVisualStyleBackColor = true;
            this.tabigFourColor_btn.Click += new System.EventHandler(this.ColorBar_btn_Click);
            // 
            // tabigOtherSplit_nud
            // 
            this.tabigOtherSplit_nud.Location = new System.Drawing.Point(259, 15);
            this.tabigOtherSplit_nud.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabigOtherSplit_nud.Name = "tabigOtherSplit_nud";
            this.tabigOtherSplit_nud.Size = new System.Drawing.Size(103, 22);
            this.tabigOtherSplit_nud.TabIndex = 2;
            this.tabigOtherSplit_nud.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.tabigOtherSplit_nud.ValueChanged += new System.EventHandler(this.SplitPanelCreat);
            // 
            // tabigOtherColor_lbl
            // 
            this.tabigOtherColor_lbl.AutoSize = true;
            this.tabigOtherColor_lbl.Location = new System.Drawing.Point(255, 98);
            this.tabigOtherColor_lbl.Name = "tabigOtherColor_lbl";
            this.tabigOtherColor_lbl.Size = new System.Drawing.Size(53, 12);
            this.tabigOtherColor_lbl.TabIndex = 21;
            this.tabigOtherColor_lbl.Text = "選取顏色";
            // 
            // tabigOtherSplit_lbl
            // 
            this.tabigOtherSplit_lbl.AutoSize = true;
            this.tabigOtherSplit_lbl.Location = new System.Drawing.Point(208, 19);
            this.tabigOtherSplit_lbl.Name = "tabigOtherSplit_lbl";
            this.tabigOtherSplit_lbl.Size = new System.Drawing.Size(46, 12);
            this.tabigOtherSplit_lbl.TabIndex = 3;
            this.tabigOtherSplit_lbl.Text = "Bar格數";
            // 
            // tabigOtherColorC_btn
            // 
            this.tabigOtherColorC_btn.BackgroundImage = global::Thunder.Properties.Resources.custom;
            this.tabigOtherColorC_btn.Location = new System.Drawing.Point(309, 92);
            this.tabigOtherColorC_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabigOtherColorC_btn.Name = "tabigOtherColorC_btn";
            this.tabigOtherColorC_btn.Size = new System.Drawing.Size(24, 24);
            this.tabigOtherColorC_btn.TabIndex = 18;
            this.tabigOtherColorC_btn.UseVisualStyleBackColor = false;
            this.tabigOtherColorC_btn.Click += new System.EventHandler(this.GradientColor_Click);
            // 
            // tabigOtherColorW_btn
            // 
            this.tabigOtherColorW_btn.BackColor = System.Drawing.Color.White;
            this.tabigOtherColorW_btn.Location = new System.Drawing.Point(253, 43);
            this.tabigOtherColorW_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabigOtherColorW_btn.Name = "tabigOtherColorW_btn";
            this.tabigOtherColorW_btn.Size = new System.Drawing.Size(24, 24);
            this.tabigOtherColorW_btn.TabIndex = 12;
            this.tabigOtherColorW_btn.Text = "W";
            this.tabigOtherColorW_btn.UseVisualStyleBackColor = false;
            this.tabigOtherColorW_btn.Click += new System.EventHandler(this.GradientColor_Click);
            // 
            // tabigOtherColorM_btn
            // 
            this.tabigOtherColorM_btn.BackColor = System.Drawing.Color.Magenta;
            this.tabigOtherColorM_btn.ForeColor = System.Drawing.Color.Black;
            this.tabigOtherColorM_btn.Location = new System.Drawing.Point(335, 68);
            this.tabigOtherColorM_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabigOtherColorM_btn.Name = "tabigOtherColorM_btn";
            this.tabigOtherColorM_btn.Size = new System.Drawing.Size(24, 24);
            this.tabigOtherColorM_btn.TabIndex = 20;
            this.tabigOtherColorM_btn.Text = "M";
            this.tabigOtherColorM_btn.UseVisualStyleBackColor = false;
            this.tabigOtherColorM_btn.Click += new System.EventHandler(this.GradientColor_Click);
            // 
            // tabigOtherColorR_btn
            // 
            this.tabigOtherColorR_btn.BackColor = System.Drawing.Color.Red;
            this.tabigOtherColorR_btn.ForeColor = System.Drawing.Color.White;
            this.tabigOtherColorR_btn.Location = new System.Drawing.Point(280, 43);
            this.tabigOtherColorR_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabigOtherColorR_btn.Name = "tabigOtherColorR_btn";
            this.tabigOtherColorR_btn.Size = new System.Drawing.Size(24, 24);
            this.tabigOtherColorR_btn.TabIndex = 13;
            this.tabigOtherColorR_btn.Text = "R";
            this.tabigOtherColorR_btn.UseVisualStyleBackColor = false;
            this.tabigOtherColorR_btn.Click += new System.EventHandler(this.GradientColor_Click);
            // 
            // tabigOtherColorA_btn
            // 
            this.tabigOtherColorA_btn.BackColor = System.Drawing.Color.Aqua;
            this.tabigOtherColorA_btn.ForeColor = System.Drawing.Color.Black;
            this.tabigOtherColorA_btn.Location = new System.Drawing.Point(309, 68);
            this.tabigOtherColorA_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabigOtherColorA_btn.Name = "tabigOtherColorA_btn";
            this.tabigOtherColorA_btn.Size = new System.Drawing.Size(24, 24);
            this.tabigOtherColorA_btn.TabIndex = 19;
            this.tabigOtherColorA_btn.Text = "A";
            this.tabigOtherColorA_btn.UseVisualStyleBackColor = false;
            this.tabigOtherColorA_btn.Click += new System.EventHandler(this.GradientColor_Click);
            // 
            // tabigOtherColorG_btn
            // 
            this.tabigOtherColorG_btn.BackColor = System.Drawing.Color.Lime;
            this.tabigOtherColorG_btn.ForeColor = System.Drawing.Color.White;
            this.tabigOtherColorG_btn.Location = new System.Drawing.Point(309, 44);
            this.tabigOtherColorG_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabigOtherColorG_btn.Name = "tabigOtherColorG_btn";
            this.tabigOtherColorG_btn.Size = new System.Drawing.Size(24, 24);
            this.tabigOtherColorG_btn.TabIndex = 14;
            this.tabigOtherColorG_btn.Text = "G";
            this.tabigOtherColorG_btn.UseVisualStyleBackColor = false;
            this.tabigOtherColorG_btn.Click += new System.EventHandler(this.GradientColor_Click);
            // 
            // tabigOtherColorY_btn
            // 
            this.tabigOtherColorY_btn.BackColor = System.Drawing.Color.Yellow;
            this.tabigOtherColorY_btn.ForeColor = System.Drawing.Color.Black;
            this.tabigOtherColorY_btn.Location = new System.Drawing.Point(280, 67);
            this.tabigOtherColorY_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabigOtherColorY_btn.Name = "tabigOtherColorY_btn";
            this.tabigOtherColorY_btn.Size = new System.Drawing.Size(24, 24);
            this.tabigOtherColorY_btn.TabIndex = 18;
            this.tabigOtherColorY_btn.Text = "Y";
            this.tabigOtherColorY_btn.UseVisualStyleBackColor = false;
            this.tabigOtherColorY_btn.Click += new System.EventHandler(this.GradientColor_Click);
            // 
            // tabigOtherColorB_btn
            // 
            this.tabigOtherColorB_btn.BackColor = System.Drawing.Color.Blue;
            this.tabigOtherColorB_btn.ForeColor = System.Drawing.Color.White;
            this.tabigOtherColorB_btn.Location = new System.Drawing.Point(335, 44);
            this.tabigOtherColorB_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabigOtherColorB_btn.Name = "tabigOtherColorB_btn";
            this.tabigOtherColorB_btn.Size = new System.Drawing.Size(24, 24);
            this.tabigOtherColorB_btn.TabIndex = 15;
            this.tabigOtherColorB_btn.Text = "B";
            this.tabigOtherColorB_btn.UseVisualStyleBackColor = false;
            this.tabigOtherColorB_btn.Click += new System.EventHandler(this.GradientColor_Click);
            // 
            // tabigOtherColorD_btn
            // 
            this.tabigOtherColorD_btn.BackColor = System.Drawing.Color.Black;
            this.tabigOtherColorD_btn.ForeColor = System.Drawing.Color.White;
            this.tabigOtherColorD_btn.Location = new System.Drawing.Point(253, 67);
            this.tabigOtherColorD_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabigOtherColorD_btn.Name = "tabigOtherColorD_btn";
            this.tabigOtherColorD_btn.Size = new System.Drawing.Size(24, 24);
            this.tabigOtherColorD_btn.TabIndex = 17;
            this.tabigOtherColorD_btn.Text = "D";
            this.tabigOtherColorD_btn.UseVisualStyleBackColor = false;
            this.tabigOtherColorD_btn.Click += new System.EventHandler(this.GradientColor_Click);
            // 
            // tabigStep_lbl
            // 
            this.tabigStep_lbl.AutoSize = true;
            this.tabigStep_lbl.Location = new System.Drawing.Point(12, 40);
            this.tabigStep_lbl.Name = "tabigStep_lbl";
            this.tabigStep_lbl.Size = new System.Drawing.Size(149, 12);
            this.tabigStep_lbl.TabIndex = 20;
            this.tabigStep_lbl.Text = "漸層分幾階        畫面幾等分";
            // 
            // tabigStep_cmb
            // 
            this.tabigStep_cmb.FormattingEnabled = true;
            this.tabigStep_cmb.Items.AddRange(new object[] {
            "16",
            "32",
            "64",
            "128",
            "256"});
            this.tabigStep_cmb.Location = new System.Drawing.Point(14, 55);
            this.tabigStep_cmb.Name = "tabigStep_cmb";
            this.tabigStep_cmb.Size = new System.Drawing.Size(99, 20);
            this.tabigStep_cmb.TabIndex = 21;
            this.tabigStep_cmb.Text = "256";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tabigBaseColorCustom_btn);
            this.panel1.Controls.Add(this.tabigBaseColor_lbl);
            this.panel1.Controls.Add(this.tabigBaseColorB_btn);
            this.panel1.Controls.Add(this.tabigBaseColorW_btn);
            this.panel1.Controls.Add(this.tabigBaseColorG_btn);
            this.panel1.Controls.Add(this.tabigBaseColorR_btn);
            this.panel1.Location = new System.Drawing.Point(7, 86);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(285, 45);
            this.panel1.TabIndex = 22;
            // 
            // tabigBaseColorCustom_btn
            // 
            this.tabigBaseColorCustom_btn.BackgroundImage = global::Thunder.Properties.Resources.custom;
            this.tabigBaseColorCustom_btn.Location = new System.Drawing.Point(234, 14);
            this.tabigBaseColorCustom_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabigBaseColorCustom_btn.Name = "tabigBaseColorCustom_btn";
            this.tabigBaseColorCustom_btn.Size = new System.Drawing.Size(24, 24);
            this.tabigBaseColorCustom_btn.TabIndex = 15;
            this.tabigBaseColorCustom_btn.UseVisualStyleBackColor = false;
            this.tabigBaseColorCustom_btn.Click += new System.EventHandler(this.GradientColor_Click);
            // 
            // tabigBaseColor_lbl
            // 
            this.tabigBaseColor_lbl.AutoSize = true;
            this.tabigBaseColor_lbl.BackColor = System.Drawing.Color.White;
            this.tabigBaseColor_lbl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabigBaseColor_lbl.Font = new System.Drawing.Font("Microsoft JhengHei UI", 16F);
            this.tabigBaseColor_lbl.Location = new System.Drawing.Point(14, 7);
            this.tabigBaseColor_lbl.Name = "tabigBaseColor_lbl";
            this.tabigBaseColor_lbl.Size = new System.Drawing.Size(102, 30);
            this.tabigBaseColor_lbl.TabIndex = 10;
            this.tabigBaseColor_lbl.Text = "基底色彩";
            // 
            // tabigBaseColorB_btn
            // 
            this.tabigBaseColorB_btn.BackColor = System.Drawing.Color.Blue;
            this.tabigBaseColorB_btn.ForeColor = System.Drawing.Color.White;
            this.tabigBaseColorB_btn.Location = new System.Drawing.Point(208, 14);
            this.tabigBaseColorB_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabigBaseColorB_btn.Name = "tabigBaseColorB_btn";
            this.tabigBaseColorB_btn.Size = new System.Drawing.Size(24, 24);
            this.tabigBaseColorB_btn.TabIndex = 14;
            this.tabigBaseColorB_btn.Text = "B";
            this.tabigBaseColorB_btn.UseVisualStyleBackColor = false;
            this.tabigBaseColorB_btn.Click += new System.EventHandler(this.GradientColor_Click);
            // 
            // tabigBaseColorW_btn
            // 
            this.tabigBaseColorW_btn.BackColor = System.Drawing.Color.White;
            this.tabigBaseColorW_btn.Location = new System.Drawing.Point(131, 14);
            this.tabigBaseColorW_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabigBaseColorW_btn.Name = "tabigBaseColorW_btn";
            this.tabigBaseColorW_btn.Size = new System.Drawing.Size(24, 24);
            this.tabigBaseColorW_btn.TabIndex = 11;
            this.tabigBaseColorW_btn.Text = "W";
            this.tabigBaseColorW_btn.UseVisualStyleBackColor = false;
            this.tabigBaseColorW_btn.Click += new System.EventHandler(this.GradientColor_Click);
            // 
            // tabigBaseColorG_btn
            // 
            this.tabigBaseColorG_btn.BackColor = System.Drawing.Color.Lime;
            this.tabigBaseColorG_btn.ForeColor = System.Drawing.Color.White;
            this.tabigBaseColorG_btn.Location = new System.Drawing.Point(182, 14);
            this.tabigBaseColorG_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabigBaseColorG_btn.Name = "tabigBaseColorG_btn";
            this.tabigBaseColorG_btn.Size = new System.Drawing.Size(24, 24);
            this.tabigBaseColorG_btn.TabIndex = 13;
            this.tabigBaseColorG_btn.Text = "G";
            this.tabigBaseColorG_btn.UseVisualStyleBackColor = false;
            this.tabigBaseColorG_btn.Click += new System.EventHandler(this.GradientColor_Click);
            // 
            // tabigBaseColorR_btn
            // 
            this.tabigBaseColorR_btn.BackColor = System.Drawing.Color.Red;
            this.tabigBaseColorR_btn.ForeColor = System.Drawing.Color.White;
            this.tabigBaseColorR_btn.Location = new System.Drawing.Point(157, 14);
            this.tabigBaseColorR_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabigBaseColorR_btn.Name = "tabigBaseColorR_btn";
            this.tabigBaseColorR_btn.Size = new System.Drawing.Size(24, 24);
            this.tabigBaseColorR_btn.TabIndex = 12;
            this.tabigBaseColorR_btn.Text = "R";
            this.tabigBaseColorR_btn.UseVisualStyleBackColor = false;
            this.tabigBaseColorR_btn.Click += new System.EventHandler(this.GradientColor_Click);
            // 
            // tabigVWay_rdo
            // 
            this.tabigVWay_rdo.AutoSize = true;
            this.tabigVWay_rdo.Checked = true;
            this.tabigVWay_rdo.Location = new System.Drawing.Point(9, 16);
            this.tabigVWay_rdo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabigVWay_rdo.Name = "tabigVWay_rdo";
            this.tabigVWay_rdo.Size = new System.Drawing.Size(86, 16);
            this.tabigVWay_rdo.TabIndex = 0;
            this.tabigVWay_rdo.TabStop = true;
            this.tabigVWay_rdo.Text = "Vertical 漸層";
            this.tabigVWay_rdo.UseVisualStyleBackColor = true;
            // 
            // tabigDivid_cmb
            // 
            this.tabigDivid_cmb.FormattingEnabled = true;
            this.tabigDivid_cmb.Items.AddRange(new object[] {
            "1",
            "2",
            "4"});
            this.tabigDivid_cmb.Location = new System.Drawing.Point(119, 58);
            this.tabigDivid_cmb.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabigDivid_cmb.Name = "tabigDivid_cmb";
            this.tabigDivid_cmb.Size = new System.Drawing.Size(104, 20);
            this.tabigDivid_cmb.TabIndex = 5;
            this.tabigDivid_cmb.Text = "1";
            // 
            // tabigColorBar_rdo
            // 
            this.tabigColorBar_rdo.AutoSize = true;
            this.tabigColorBar_rdo.Location = new System.Drawing.Point(233, 16);
            this.tabigColorBar_rdo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabigColorBar_rdo.Name = "tabigColorBar_rdo";
            this.tabigColorBar_rdo.Size = new System.Drawing.Size(91, 16);
            this.tabigColorBar_rdo.TabIndex = 18;
            this.tabigColorBar_rdo.TabStop = true;
            this.tabigColorBar_rdo.Text = "ColorBar模式";
            this.tabigColorBar_rdo.UseVisualStyleBackColor = true;
            this.tabigColorBar_rdo.CheckedChanged += new System.EventHandler(this.otherColor_CheckedChanged);
            // 
            // tabigFirstLevel_lbl
            // 
            this.tabigFirstLevel_lbl.AutoSize = true;
            this.tabigFirstLevel_lbl.Location = new System.Drawing.Point(269, 41);
            this.tabigFirstLevel_lbl.Name = "tabigFirstLevel_lbl";
            this.tabigFirstLevel_lbl.Size = new System.Drawing.Size(51, 36);
            this.tabigFirstLevel_lbl.TabIndex = 6;
            this.tabigFirstLevel_lbl.Text = "FirstLevel\r\n\r\nLastLevel";
            // 
            // tabigHWay_rdo
            // 
            this.tabigHWay_rdo.AutoSize = true;
            this.tabigHWay_rdo.Location = new System.Drawing.Point(116, 16);
            this.tabigHWay_rdo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabigHWay_rdo.Name = "tabigHWay_rdo";
            this.tabigHWay_rdo.Size = new System.Drawing.Size(99, 16);
            this.tabigHWay_rdo.TabIndex = 1;
            this.tabigHWay_rdo.TabStop = true;
            this.tabigHWay_rdo.Text = "Horizontal 漸層";
            this.tabigHWay_rdo.UseVisualStyleBackColor = true;
            // 
            // tabigFirstLevel_cmb
            // 
            this.tabigFirstLevel_cmb.FormattingEnabled = true;
            this.tabigFirstLevel_cmb.Items.AddRange(new object[] {
            "0",
            "31",
            "63",
            "95",
            "127",
            "159",
            "191",
            "223",
            "255"});
            this.tabigFirstLevel_cmb.Location = new System.Drawing.Point(349, 37);
            this.tabigFirstLevel_cmb.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabigFirstLevel_cmb.Name = "tabigFirstLevel_cmb";
            this.tabigFirstLevel_cmb.Size = new System.Drawing.Size(104, 20);
            this.tabigFirstLevel_cmb.TabIndex = 8;
            this.tabigFirstLevel_cmb.Text = "0";
            // 
            // tabigLastLevel_cmb
            // 
            this.tabigLastLevel_cmb.FormattingEnabled = true;
            this.tabigLastLevel_cmb.Items.AddRange(new object[] {
            "0",
            "31",
            "63",
            "95",
            "127",
            "159",
            "191",
            "223",
            "255"});
            this.tabigLastLevel_cmb.Location = new System.Drawing.Point(349, 61);
            this.tabigLastLevel_cmb.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabigLastLevel_cmb.Name = "tabigLastLevel_cmb";
            this.tabigLastLevel_cmb.Size = new System.Drawing.Size(104, 20);
            this.tabigLastLevel_cmb.TabIndex = 9;
            this.tabigLastLevel_cmb.Text = "255";
            // 
            // tabimgeWindow
            // 
            this.tabimgeWindow.Controls.Add(this.tabiwCustom_grp);
            this.tabimgeWindow.Controls.Add(this.tabiwWin_grp);
            this.tabimgeWindow.Controls.Add(this.tabiwBack_grp);
            this.tabimgeWindow.Controls.Add(this.tabiwLineColor_lbl);
            this.tabimgeWindow.Controls.Add(this.tabiwLineColor_btn);
            this.tabimgeWindow.Controls.Add(this.tabiwLineCross_btn);
            this.tabimgeWindow.Controls.Add(this.tabiwLineOutside_btn);
            this.tabimgeWindow.Controls.Add(this.tabiwWinLoc_grp);
            this.tabimgeWindow.Controls.Add(this.tabiwWinSize_grp);
            this.tabimgeWindow.Location = new System.Drawing.Point(539, 250);
            this.tabimgeWindow.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabimgeWindow.Name = "tabimgeWindow";
            this.tabimgeWindow.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabimgeWindow.Size = new System.Drawing.Size(222, 90);
            this.tabimgeWindow.TabIndex = 0;
            this.tabimgeWindow.TabStop = false;
            this.tabimgeWindow.Text = "Window";
            // 
            // tabiwCustom_grp
            // 
            this.tabiwCustom_grp.Controls.Add(this.tabiwCustomGrad_grp);
            this.tabiwCustom_grp.Controls.Add(this.tabiwCustom_cmb);
            this.tabiwCustom_grp.Controls.Add(this.tabiwCustomMask_grp);
            this.tabiwCustom_grp.Location = new System.Drawing.Point(219, 11);
            this.tabiwCustom_grp.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwCustom_grp.Name = "tabiwCustom_grp";
            this.tabiwCustom_grp.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwCustom_grp.Size = new System.Drawing.Size(424, 315);
            this.tabiwCustom_grp.TabIndex = 19;
            this.tabiwCustom_grp.TabStop = false;
            this.tabiwCustom_grp.Text = "自製圖";
            // 
            // tabiwCustomGrad_grp
            // 
            this.tabiwCustomGrad_grp.Controls.Add(this.label4);
            this.tabiwCustomGrad_grp.Controls.Add(this.groupBox1);
            this.tabiwCustomGrad_grp.Controls.Add(this.tabiwCGrad0);
            this.tabiwCustomGrad_grp.Controls.Add(this.comboBox1);
            this.tabiwCustomGrad_grp.Controls.Add(this.panel3);
            this.tabiwCustomGrad_grp.Controls.Add(this.tabiwCGradVWay_rdo);
            this.tabiwCustomGrad_grp.Controls.Add(this.comboBox2);
            this.tabiwCustomGrad_grp.Controls.Add(this.tabiwCGrad);
            this.tabiwCustomGrad_grp.Controls.Add(this.tabiwCGradHWay_rdo);
            this.tabiwCustomGrad_grp.Controls.Add(this.comboBox3);
            this.tabiwCustomGrad_grp.Controls.Add(this.comboBox4);
            this.tabiwCustomGrad_grp.Location = new System.Drawing.Point(9, 28);
            this.tabiwCustomGrad_grp.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwCustomGrad_grp.Name = "tabiwCustomGrad_grp";
            this.tabiwCustomGrad_grp.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwCustomGrad_grp.Size = new System.Drawing.Size(405, 310);
            this.tabiwCustomGrad_grp.TabIndex = 20;
            this.tabiwCustomGrad_grp.TabStop = false;
            this.tabiwCustomGrad_grp.Text = "Gradient";
            this.tabiwCustomGrad_grp.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(188, 35);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 36);
            this.label4.TabIndex = 37;
            this.label4.Text = "FirstLevel\r\n\r\nLastLevel";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Controls.Add(this.tabiwCGradOtherSplit_pnl);
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.numericUpDown1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Controls.Add(this.button5);
            this.groupBox1.Controls.Add(this.button6);
            this.groupBox1.Controls.Add(this.button7);
            this.groupBox1.Controls.Add(this.button8);
            this.groupBox1.Controls.Add(this.button9);
            this.groupBox1.Controls.Add(this.button10);
            this.groupBox1.Location = new System.Drawing.Point(10, 110);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(368, 189);
            this.groupBox1.TabIndex = 36;
            this.groupBox1.TabStop = false;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(7, 18);
            this.radioButton1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(55, 16);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "V分割";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // tabiwCGradOtherSplit_pnl
            // 
            this.tabiwCGradOtherSplit_pnl.Location = new System.Drawing.Point(8, 40);
            this.tabiwCGradOtherSplit_pnl.Name = "tabiwCGradOtherSplit_pnl";
            this.tabiwCGradOtherSplit_pnl.Size = new System.Drawing.Size(200, 140);
            this.tabiwCGradOtherSplit_pnl.TabIndex = 22;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(91, 17);
            this.radioButton2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(55, 16);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.Text = "H分割";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(253, 123);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(61, 21);
            this.button1.TabIndex = 16;
            this.button1.Text = "常規四色";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(259, 15);
            this.numericUpDown1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(103, 22);
            this.numericUpDown1.TabIndex = 2;
            this.numericUpDown1.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(255, 98);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 21;
            this.label2.Text = "選取顏色";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(208, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "Bar格數";
            // 
            // button2
            // 
            this.button2.BackgroundImage = global::Thunder.Properties.Resources.custom;
            this.button2.Location = new System.Drawing.Point(309, 92);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(24, 24);
            this.button2.TabIndex = 18;
            this.button2.UseVisualStyleBackColor = false;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.White;
            this.button3.Location = new System.Drawing.Point(253, 43);
            this.button3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(24, 24);
            this.button3.TabIndex = 12;
            this.button3.Text = "W";
            this.button3.UseVisualStyleBackColor = false;
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.Magenta;
            this.button4.ForeColor = System.Drawing.Color.Black;
            this.button4.Location = new System.Drawing.Point(335, 68);
            this.button4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(24, 24);
            this.button4.TabIndex = 20;
            this.button4.Text = "M";
            this.button4.UseVisualStyleBackColor = false;
            // 
            // button5
            // 
            this.button5.BackColor = System.Drawing.Color.Red;
            this.button5.ForeColor = System.Drawing.Color.White;
            this.button5.Location = new System.Drawing.Point(280, 43);
            this.button5.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(24, 24);
            this.button5.TabIndex = 13;
            this.button5.Text = "R";
            this.button5.UseVisualStyleBackColor = false;
            // 
            // button6
            // 
            this.button6.BackColor = System.Drawing.Color.Aqua;
            this.button6.ForeColor = System.Drawing.Color.Black;
            this.button6.Location = new System.Drawing.Point(309, 68);
            this.button6.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(24, 24);
            this.button6.TabIndex = 19;
            this.button6.Text = "A";
            this.button6.UseVisualStyleBackColor = false;
            // 
            // button7
            // 
            this.button7.BackColor = System.Drawing.Color.Lime;
            this.button7.ForeColor = System.Drawing.Color.White;
            this.button7.Location = new System.Drawing.Point(309, 44);
            this.button7.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(24, 24);
            this.button7.TabIndex = 14;
            this.button7.Text = "G";
            this.button7.UseVisualStyleBackColor = false;
            // 
            // button8
            // 
            this.button8.BackColor = System.Drawing.Color.Yellow;
            this.button8.ForeColor = System.Drawing.Color.Black;
            this.button8.Location = new System.Drawing.Point(280, 67);
            this.button8.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(24, 24);
            this.button8.TabIndex = 18;
            this.button8.Text = "Y";
            this.button8.UseVisualStyleBackColor = false;
            // 
            // button9
            // 
            this.button9.BackColor = System.Drawing.Color.Blue;
            this.button9.ForeColor = System.Drawing.Color.White;
            this.button9.Location = new System.Drawing.Point(335, 44);
            this.button9.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(24, 24);
            this.button9.TabIndex = 15;
            this.button9.Text = "B";
            this.button9.UseVisualStyleBackColor = false;
            // 
            // button10
            // 
            this.button10.BackColor = System.Drawing.Color.Black;
            this.button10.ForeColor = System.Drawing.Color.White;
            this.button10.Location = new System.Drawing.Point(253, 67);
            this.button10.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(24, 24);
            this.button10.TabIndex = 17;
            this.button10.Text = "D";
            this.button10.UseVisualStyleBackColor = false;
            // 
            // tabiwCGrad0
            // 
            this.tabiwCGrad0.AutoSize = true;
            this.tabiwCGrad0.Location = new System.Drawing.Point(22, 35);
            this.tabiwCGrad0.Name = "tabiwCGrad0";
            this.tabiwCGrad0.Size = new System.Drawing.Size(149, 12);
            this.tabiwCGrad0.TabIndex = 33;
            this.tabiwCGrad0.Text = "漸層分幾階        畫面幾等分";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "16",
            "32",
            "64",
            "128",
            "256"});
            this.comboBox1.Location = new System.Drawing.Point(25, 53);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(49, 20);
            this.comboBox1.TabIndex = 34;
            this.comboBox1.Text = "256";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.button11);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Controls.Add(this.button12);
            this.panel3.Controls.Add(this.button13);
            this.panel3.Controls.Add(this.button14);
            this.panel3.Controls.Add(this.button15);
            this.panel3.Location = new System.Drawing.Point(21, 75);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(285, 45);
            this.panel3.TabIndex = 35;
            // 
            // button11
            // 
            this.button11.BackgroundImage = global::Thunder.Properties.Resources.custom;
            this.button11.Location = new System.Drawing.Point(234, 14);
            this.button11.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(24, 24);
            this.button11.TabIndex = 15;
            this.button11.UseVisualStyleBackColor = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Font = new System.Drawing.Font("Microsoft JhengHei UI", 16F);
            this.label5.Location = new System.Drawing.Point(14, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(102, 30);
            this.label5.TabIndex = 10;
            this.label5.Text = "基底色彩";
            // 
            // button12
            // 
            this.button12.BackColor = System.Drawing.Color.Blue;
            this.button12.ForeColor = System.Drawing.Color.White;
            this.button12.Location = new System.Drawing.Point(208, 14);
            this.button12.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(24, 24);
            this.button12.TabIndex = 14;
            this.button12.Text = "B";
            this.button12.UseVisualStyleBackColor = false;
            // 
            // button13
            // 
            this.button13.BackColor = System.Drawing.Color.White;
            this.button13.Location = new System.Drawing.Point(131, 14);
            this.button13.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(24, 24);
            this.button13.TabIndex = 11;
            this.button13.Text = "W";
            this.button13.UseVisualStyleBackColor = false;
            // 
            // button14
            // 
            this.button14.BackColor = System.Drawing.Color.Lime;
            this.button14.ForeColor = System.Drawing.Color.White;
            this.button14.Location = new System.Drawing.Point(182, 14);
            this.button14.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(24, 24);
            this.button14.TabIndex = 13;
            this.button14.Text = "G";
            this.button14.UseVisualStyleBackColor = false;
            // 
            // button15
            // 
            this.button15.BackColor = System.Drawing.Color.Red;
            this.button15.ForeColor = System.Drawing.Color.White;
            this.button15.Location = new System.Drawing.Point(157, 14);
            this.button15.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(24, 24);
            this.button15.TabIndex = 12;
            this.button15.Text = "R";
            this.button15.UseVisualStyleBackColor = false;
            // 
            // tabiwCGradVWay_rdo
            // 
            this.tabiwCGradVWay_rdo.AutoSize = true;
            this.tabiwCGradVWay_rdo.Checked = true;
            this.tabiwCGradVWay_rdo.Location = new System.Drawing.Point(13, 12);
            this.tabiwCGradVWay_rdo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwCGradVWay_rdo.Name = "tabiwCGradVWay_rdo";
            this.tabiwCGradVWay_rdo.Size = new System.Drawing.Size(86, 16);
            this.tabiwCGradVWay_rdo.TabIndex = 24;
            this.tabiwCGradVWay_rdo.TabStop = true;
            this.tabiwCGradVWay_rdo.Text = "Vertical 漸層";
            this.tabiwCGradVWay_rdo.UseVisualStyleBackColor = true;
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "1",
            "2",
            "4"});
            this.comboBox2.Location = new System.Drawing.Point(89, 54);
            this.comboBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(39, 20);
            this.comboBox2.TabIndex = 27;
            this.comboBox2.Text = "1";
            // 
            // tabiwCGrad
            // 
            this.tabiwCGrad.AutoSize = true;
            this.tabiwCGrad.Location = new System.Drawing.Point(196, 13);
            this.tabiwCGrad.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwCGrad.Name = "tabiwCGrad";
            this.tabiwCGrad.Size = new System.Drawing.Size(91, 16);
            this.tabiwCGrad.TabIndex = 32;
            this.tabiwCGrad.TabStop = true;
            this.tabiwCGrad.Text = "ColorBar模式";
            this.tabiwCGrad.UseVisualStyleBackColor = true;
            // 
            // tabiwCGradHWay_rdo
            // 
            this.tabiwCGradHWay_rdo.AutoSize = true;
            this.tabiwCGradHWay_rdo.Location = new System.Drawing.Point(86, 15);
            this.tabiwCGradHWay_rdo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwCGradHWay_rdo.Name = "tabiwCGradHWay_rdo";
            this.tabiwCGradHWay_rdo.Size = new System.Drawing.Size(99, 16);
            this.tabiwCGradHWay_rdo.TabIndex = 25;
            this.tabiwCGradHWay_rdo.TabStop = true;
            this.tabiwCGradHWay_rdo.Text = "Horizontal 漸層";
            this.tabiwCGradHWay_rdo.UseVisualStyleBackColor = true;
            // 
            // comboBox3
            // 
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Items.AddRange(new object[] {
            "0",
            "31",
            "63",
            "95",
            "127",
            "159",
            "191",
            "223",
            "255"});
            this.comboBox3.Location = new System.Drawing.Point(244, 32);
            this.comboBox3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(104, 20);
            this.comboBox3.TabIndex = 30;
            this.comboBox3.Text = "0";
            // 
            // comboBox4
            // 
            this.comboBox4.FormattingEnabled = true;
            this.comboBox4.Items.AddRange(new object[] {
            "0",
            "31",
            "63",
            "95",
            "127",
            "159",
            "191",
            "223",
            "255"});
            this.comboBox4.Location = new System.Drawing.Point(245, 52);
            this.comboBox4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.comboBox4.Name = "comboBox4";
            this.comboBox4.Size = new System.Drawing.Size(104, 20);
            this.comboBox4.TabIndex = 31;
            this.comboBox4.Text = "255";
            // 
            // tabiwCustom_cmb
            // 
            this.tabiwCustom_cmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tabiwCustom_cmb.FormattingEnabled = true;
            this.tabiwCustom_cmb.Items.AddRange(new object[] {
            "Mask",
            "Gradient"});
            this.tabiwCustom_cmb.Location = new System.Drawing.Point(172, 8);
            this.tabiwCustom_cmb.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwCustom_cmb.Name = "tabiwCustom_cmb";
            this.tabiwCustom_cmb.Size = new System.Drawing.Size(104, 20);
            this.tabiwCustom_cmb.TabIndex = 0;
            this.tabiwCustom_cmb.SelectedIndexChanged += new System.EventHandler(this.tabiwCustom_cmb_SelectedIndexChanged);
            // 
            // tabiwCustomMask_grp
            // 
            this.tabiwCustomMask_grp.Controls.Add(this.tabiwCMaskPixelGray_lbl);
            this.tabiwCustomMask_grp.Controls.Add(this.tabiwCMaskSubPixel_rdo);
            this.tabiwCustomMask_grp.Controls.Add(this.tabiwCMaskPixel_rdo);
            this.tabiwCustomMask_grp.Controls.Add(this.tabiwCMaskPixelGray_vsc);
            this.tabiwCustomMask_grp.Controls.Add(this.tabiwCMaskPixelColor_lbl);
            this.tabiwCustomMask_grp.Controls.Add(this.tabiwCMaskPixelLoc_lbl);
            this.tabiwCustomMask_grp.Controls.Add(this.tabiwCMaskPixelColor_btn);
            this.tabiwCustomMask_grp.Controls.Add(this.tabiwCMaskPixelClear_btn);
            this.tabiwCustomMask_grp.Controls.Add(this.tabiwCMaskPixelPanel_pnl);
            this.tabiwCustomMask_grp.Controls.Add(this.tabiwCMaskHNum_nud);
            this.tabiwCustomMask_grp.Controls.Add(this.tabiwCMaskWNum_nud);
            this.tabiwCustomMask_grp.Controls.Add(this.tabiwCMaskHNum_lbl);
            this.tabiwCustomMask_grp.Controls.Add(this.tabiwCMaskWNum_lbl);
            this.tabiwCustomMask_grp.Location = new System.Drawing.Point(11, 31);
            this.tabiwCustomMask_grp.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwCustomMask_grp.Name = "tabiwCustomMask_grp";
            this.tabiwCustomMask_grp.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwCustomMask_grp.Size = new System.Drawing.Size(257, 264);
            this.tabiwCustomMask_grp.TabIndex = 2;
            this.tabiwCustomMask_grp.TabStop = false;
            this.tabiwCustomMask_grp.Text = "Mask";
            this.tabiwCustomMask_grp.Visible = false;
            // 
            // tabiwCMaskPixelGray_lbl
            // 
            this.tabiwCMaskPixelGray_lbl.AutoSize = true;
            this.tabiwCMaskPixelGray_lbl.Location = new System.Drawing.Point(226, 32);
            this.tabiwCMaskPixelGray_lbl.Name = "tabiwCMaskPixelGray_lbl";
            this.tabiwCMaskPixelGray_lbl.Size = new System.Drawing.Size(52, 12);
            this.tabiwCMaskPixelGray_lbl.TabIndex = 27;
            this.tabiwCMaskPixelGray_lbl.Text = "Gray: 255";
            this.tabiwCMaskPixelGray_lbl.Visible = false;
            // 
            // tabiwCMaskSubPixel_rdo
            // 
            this.tabiwCMaskSubPixel_rdo.AutoSize = true;
            this.tabiwCMaskSubPixel_rdo.Location = new System.Drawing.Point(85, 12);
            this.tabiwCMaskSubPixel_rdo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwCMaskSubPixel_rdo.Name = "tabiwCMaskSubPixel_rdo";
            this.tabiwCMaskSubPixel_rdo.Size = new System.Drawing.Size(92, 16);
            this.tabiwCMaskSubPixel_rdo.TabIndex = 26;
            this.tabiwCMaskSubPixel_rdo.Text = "Sub Pixel Base";
            this.tabiwCMaskSubPixel_rdo.UseVisualStyleBackColor = true;
            this.tabiwCMaskSubPixel_rdo.CheckedChanged += new System.EventHandler(this.Pixel_rdo_CheckedChanged);
            // 
            // tabiwCMaskPixel_rdo
            // 
            this.tabiwCMaskPixel_rdo.AutoSize = true;
            this.tabiwCMaskPixel_rdo.Checked = true;
            this.tabiwCMaskPixel_rdo.Location = new System.Drawing.Point(11, 13);
            this.tabiwCMaskPixel_rdo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwCMaskPixel_rdo.Name = "tabiwCMaskPixel_rdo";
            this.tabiwCMaskPixel_rdo.Size = new System.Drawing.Size(71, 16);
            this.tabiwCMaskPixel_rdo.TabIndex = 25;
            this.tabiwCMaskPixel_rdo.TabStop = true;
            this.tabiwCMaskPixel_rdo.Text = "Pixel Base";
            this.tabiwCMaskPixel_rdo.UseVisualStyleBackColor = true;
            this.tabiwCMaskPixel_rdo.CheckedChanged += new System.EventHandler(this.Pixel_rdo_CheckedChanged);
            // 
            // tabiwCMaskPixelGray_vsc
            // 
            this.tabiwCMaskPixelGray_vsc.LargeChange = 1;
            this.tabiwCMaskPixelGray_vsc.Location = new System.Drawing.Point(226, 51);
            this.tabiwCMaskPixelGray_vsc.Maximum = 255;
            this.tabiwCMaskPixelGray_vsc.Name = "tabiwCMaskPixelGray_vsc";
            this.tabiwCMaskPixelGray_vsc.Size = new System.Drawing.Size(20, 192);
            this.tabiwCMaskPixelGray_vsc.TabIndex = 24;
            this.tabiwCMaskPixelGray_vsc.ValueChanged += new System.EventHandler(this.PixelGray_vsc_ValueChanged);
            // 
            // tabiwCMaskPixelColor_lbl
            // 
            this.tabiwCMaskPixelColor_lbl.AutoSize = true;
            this.tabiwCMaskPixelColor_lbl.Location = new System.Drawing.Point(73, 247);
            this.tabiwCMaskPixelColor_lbl.Name = "tabiwCMaskPixelColor_lbl";
            this.tabiwCMaskPixelColor_lbl.Size = new System.Drawing.Size(53, 12);
            this.tabiwCMaskPixelColor_lbl.TabIndex = 23;
            this.tabiwCMaskPixelColor_lbl.Text = "選擇顏色";
            // 
            // tabiwCMaskPixelLoc_lbl
            // 
            this.tabiwCMaskPixelLoc_lbl.AutoSize = true;
            this.tabiwCMaskPixelLoc_lbl.Location = new System.Drawing.Point(13, 246);
            this.tabiwCMaskPixelLoc_lbl.Name = "tabiwCMaskPixelLoc_lbl";
            this.tabiwCMaskPixelLoc_lbl.Size = new System.Drawing.Size(24, 12);
            this.tabiwCMaskPixelLoc_lbl.TabIndex = 22;
            this.tabiwCMaskPixelLoc_lbl.Text = "--,--";
            // 
            // tabiwCMaskPixelColor_btn
            // 
            this.tabiwCMaskPixelColor_btn.Location = new System.Drawing.Point(125, 243);
            this.tabiwCMaskPixelColor_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwCMaskPixelColor_btn.Name = "tabiwCMaskPixelColor_btn";
            this.tabiwCMaskPixelColor_btn.Size = new System.Drawing.Size(21, 19);
            this.tabiwCMaskPixelColor_btn.TabIndex = 21;
            this.tabiwCMaskPixelColor_btn.UseVisualStyleBackColor = true;
            this.tabiwCMaskPixelColor_btn.Click += new System.EventHandler(this.SetButtonBackgroundColor);
            // 
            // tabiwCMaskPixelClear_btn
            // 
            this.tabiwCMaskPixelClear_btn.Location = new System.Drawing.Point(175, 243);
            this.tabiwCMaskPixelClear_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwCMaskPixelClear_btn.Name = "tabiwCMaskPixelClear_btn";
            this.tabiwCMaskPixelClear_btn.Size = new System.Drawing.Size(42, 18);
            this.tabiwCMaskPixelClear_btn.TabIndex = 20;
            this.tabiwCMaskPixelClear_btn.Text = "Clear";
            this.tabiwCMaskPixelClear_btn.UseVisualStyleBackColor = true;
            this.tabiwCMaskPixelClear_btn.Click += new System.EventHandler(this.MaskPanelCreat);
            // 
            // tabiwCMaskPixelPanel_pnl
            // 
            this.tabiwCMaskPixelPanel_pnl.BackgroundImage = global::Thunder.Properties.Resources.transparent;
            this.tabiwCMaskPixelPanel_pnl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabiwCMaskPixelPanel_pnl.Cursor = System.Windows.Forms.Cursors.Cross;
            this.tabiwCMaskPixelPanel_pnl.Location = new System.Drawing.Point(11, 51);
            this.tabiwCMaskPixelPanel_pnl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwCMaskPixelPanel_pnl.Name = "tabiwCMaskPixelPanel_pnl";
            this.tabiwCMaskPixelPanel_pnl.Size = new System.Drawing.Size(206, 192);
            this.tabiwCMaskPixelPanel_pnl.TabIndex = 19;
            // 
            // tabiwCMaskHNum_nud
            // 
            this.tabiwCMaskHNum_nud.Location = new System.Drawing.Point(144, 32);
            this.tabiwCMaskHNum_nud.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwCMaskHNum_nud.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.tabiwCMaskHNum_nud.Name = "tabiwCMaskHNum_nud";
            this.tabiwCMaskHNum_nud.Size = new System.Drawing.Size(33, 22);
            this.tabiwCMaskHNum_nud.TabIndex = 18;
            this.tabiwCMaskHNum_nud.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.tabiwCMaskHNum_nud.ValueChanged += new System.EventHandler(this.MaskPanelCreat);
            // 
            // tabiwCMaskWNum_nud
            // 
            this.tabiwCMaskWNum_nud.Location = new System.Drawing.Point(52, 32);
            this.tabiwCMaskWNum_nud.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwCMaskWNum_nud.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.tabiwCMaskWNum_nud.Name = "tabiwCMaskWNum_nud";
            this.tabiwCMaskWNum_nud.Size = new System.Drawing.Size(34, 22);
            this.tabiwCMaskWNum_nud.TabIndex = 17;
            this.tabiwCMaskWNum_nud.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.tabiwCMaskWNum_nud.ValueChanged += new System.EventHandler(this.MaskPanelCreat);
            // 
            // tabiwCMaskHNum_lbl
            // 
            this.tabiwCMaskHNum_lbl.AutoSize = true;
            this.tabiwCMaskHNum_lbl.Location = new System.Drawing.Point(104, 34);
            this.tabiwCMaskHNum_lbl.Name = "tabiwCMaskHNum_lbl";
            this.tabiwCMaskHNum_lbl.Size = new System.Drawing.Size(36, 12);
            this.tabiwCMaskHNum_lbl.TabIndex = 16;
            this.tabiwCMaskHNum_lbl.Text = "Height";
            // 
            // tabiwCMaskWNum_lbl
            // 
            this.tabiwCMaskWNum_lbl.AutoSize = true;
            this.tabiwCMaskWNum_lbl.Location = new System.Drawing.Point(13, 34);
            this.tabiwCMaskWNum_lbl.Name = "tabiwCMaskWNum_lbl";
            this.tabiwCMaskWNum_lbl.Size = new System.Drawing.Size(34, 12);
            this.tabiwCMaskWNum_lbl.TabIndex = 15;
            this.tabiwCMaskWNum_lbl.Text = "Width";
            // 
            // tabiwWin_grp
            // 
            this.tabiwWin_grp.Controls.Add(this.tabiwWin_pic);
            this.tabiwWin_grp.Controls.Add(this.tabiwWinColor_rdo);
            this.tabiwWin_grp.Controls.Add(this.tabiwWinCustom_rdo);
            this.tabiwWin_grp.Controls.Add(this.tabiwWinImg_rdo);
            this.tabiwWin_grp.Location = new System.Drawing.Point(115, 13);
            this.tabiwWin_grp.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwWin_grp.Name = "tabiwWin_grp";
            this.tabiwWin_grp.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwWin_grp.Size = new System.Drawing.Size(98, 62);
            this.tabiwWin_grp.TabIndex = 18;
            this.tabiwWin_grp.TabStop = false;
            this.tabiwWin_grp.Text = "方塊顏色";
            // 
            // tabiwWin_pic
            // 
            this.tabiwWin_pic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tabiwWin_pic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabiwWin_pic.Image = global::Thunder.Properties.Resources.Img;
            this.tabiwWin_pic.Location = new System.Drawing.Point(3, 17);
            this.tabiwWin_pic.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwWin_pic.Name = "tabiwWin_pic";
            this.tabiwWin_pic.Size = new System.Drawing.Size(35, 32);
            this.tabiwWin_pic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.tabiwWin_pic.TabIndex = 8;
            this.tabiwWin_pic.TabStop = false;
            this.tabiwWin_pic.Click += new System.EventHandler(this.windowPictureBox_Click);
            // 
            // tabiwWinColor_rdo
            // 
            this.tabiwWinColor_rdo.AutoSize = true;
            this.tabiwWinColor_rdo.Checked = true;
            this.tabiwWinColor_rdo.Location = new System.Drawing.Point(43, 11);
            this.tabiwWinColor_rdo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwWinColor_rdo.Name = "tabiwWinColor_rdo";
            this.tabiwWinColor_rdo.Size = new System.Drawing.Size(47, 16);
            this.tabiwWinColor_rdo.TabIndex = 5;
            this.tabiwWinColor_rdo.TabStop = true;
            this.tabiwWinColor_rdo.Tag = "";
            this.tabiwWinColor_rdo.Text = "純色";
            this.tabiwWinColor_rdo.UseVisualStyleBackColor = true;
            this.tabiwWinColor_rdo.Click += new System.EventHandler(this.windowPictureBox_Click);
            // 
            // tabiwWinCustom_rdo
            // 
            this.tabiwWinCustom_rdo.AutoSize = true;
            this.tabiwWinCustom_rdo.Enabled = false;
            this.tabiwWinCustom_rdo.Location = new System.Drawing.Point(43, 41);
            this.tabiwWinCustom_rdo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwWinCustom_rdo.Name = "tabiwWinCustom_rdo";
            this.tabiwWinCustom_rdo.Size = new System.Drawing.Size(59, 16);
            this.tabiwWinCustom_rdo.TabIndex = 7;
            this.tabiwWinCustom_rdo.TabStop = true;
            this.tabiwWinCustom_rdo.Tag = "";
            this.tabiwWinCustom_rdo.Text = "自製圖";
            this.tabiwWinCustom_rdo.UseVisualStyleBackColor = true;
            this.tabiwWinCustom_rdo.Click += new System.EventHandler(this.windowPictureBox_Click);
            // 
            // tabiwWinImg_rdo
            // 
            this.tabiwWinImg_rdo.AutoSize = true;
            this.tabiwWinImg_rdo.Location = new System.Drawing.Point(43, 26);
            this.tabiwWinImg_rdo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwWinImg_rdo.Name = "tabiwWinImg_rdo";
            this.tabiwWinImg_rdo.Size = new System.Drawing.Size(47, 16);
            this.tabiwWinImg_rdo.TabIndex = 6;
            this.tabiwWinImg_rdo.TabStop = true;
            this.tabiwWinImg_rdo.Tag = "";
            this.tabiwWinImg_rdo.Text = "截圖";
            this.tabiwWinImg_rdo.UseVisualStyleBackColor = true;
            this.tabiwWinImg_rdo.Click += new System.EventHandler(this.windowPictureBox_Click);
            // 
            // tabiwBack_grp
            // 
            this.tabiwBack_grp.Controls.Add(this.tabiwBack_pic);
            this.tabiwBack_grp.Controls.Add(this.tabiwBackColor_rdo);
            this.tabiwBack_grp.Controls.Add(this.tabiwBackCustom_rdo);
            this.tabiwBack_grp.Controls.Add(this.tabiwBackImg_rdo);
            this.tabiwBack_grp.Location = new System.Drawing.Point(9, 13);
            this.tabiwBack_grp.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwBack_grp.Name = "tabiwBack_grp";
            this.tabiwBack_grp.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwBack_grp.Size = new System.Drawing.Size(101, 63);
            this.tabiwBack_grp.TabIndex = 17;
            this.tabiwBack_grp.TabStop = false;
            this.tabiwBack_grp.Text = "背景顏色";
            // 
            // tabiwBack_pic
            // 
            this.tabiwBack_pic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tabiwBack_pic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabiwBack_pic.Image = global::Thunder.Properties.Resources.Img;
            this.tabiwBack_pic.InitialImage = null;
            this.tabiwBack_pic.Location = new System.Drawing.Point(4, 18);
            this.tabiwBack_pic.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwBack_pic.Name = "tabiwBack_pic";
            this.tabiwBack_pic.Size = new System.Drawing.Size(35, 32);
            this.tabiwBack_pic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.tabiwBack_pic.TabIndex = 5;
            this.tabiwBack_pic.TabStop = false;
            this.tabiwBack_pic.Click += new System.EventHandler(this.windowPictureBox_Click);
            // 
            // tabiwBackColor_rdo
            // 
            this.tabiwBackColor_rdo.AutoSize = true;
            this.tabiwBackColor_rdo.Checked = true;
            this.tabiwBackColor_rdo.Location = new System.Drawing.Point(46, 13);
            this.tabiwBackColor_rdo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwBackColor_rdo.Name = "tabiwBackColor_rdo";
            this.tabiwBackColor_rdo.Size = new System.Drawing.Size(47, 16);
            this.tabiwBackColor_rdo.TabIndex = 2;
            this.tabiwBackColor_rdo.TabStop = true;
            this.tabiwBackColor_rdo.Tag = "";
            this.tabiwBackColor_rdo.Text = "純色";
            this.tabiwBackColor_rdo.UseVisualStyleBackColor = true;
            this.tabiwBackColor_rdo.Click += new System.EventHandler(this.windowPictureBox_Click);
            // 
            // tabiwBackCustom_rdo
            // 
            this.tabiwBackCustom_rdo.AutoSize = true;
            this.tabiwBackCustom_rdo.Enabled = false;
            this.tabiwBackCustom_rdo.Location = new System.Drawing.Point(46, 42);
            this.tabiwBackCustom_rdo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwBackCustom_rdo.Name = "tabiwBackCustom_rdo";
            this.tabiwBackCustom_rdo.Size = new System.Drawing.Size(59, 16);
            this.tabiwBackCustom_rdo.TabIndex = 4;
            this.tabiwBackCustom_rdo.TabStop = true;
            this.tabiwBackCustom_rdo.Tag = "";
            this.tabiwBackCustom_rdo.Text = "自製圖";
            this.tabiwBackCustom_rdo.UseVisualStyleBackColor = true;
            this.tabiwBackCustom_rdo.Click += new System.EventHandler(this.windowPictureBox_Click);
            // 
            // tabiwBackImg_rdo
            // 
            this.tabiwBackImg_rdo.AutoSize = true;
            this.tabiwBackImg_rdo.Location = new System.Drawing.Point(46, 28);
            this.tabiwBackImg_rdo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwBackImg_rdo.Name = "tabiwBackImg_rdo";
            this.tabiwBackImg_rdo.Size = new System.Drawing.Size(47, 16);
            this.tabiwBackImg_rdo.TabIndex = 3;
            this.tabiwBackImg_rdo.TabStop = true;
            this.tabiwBackImg_rdo.Tag = "";
            this.tabiwBackImg_rdo.Text = "截圖";
            this.tabiwBackImg_rdo.UseVisualStyleBackColor = true;
            this.tabiwBackImg_rdo.Click += new System.EventHandler(this.windowPictureBox_Click);
            // 
            // tabiwLineColor_lbl
            // 
            this.tabiwLineColor_lbl.AutoSize = true;
            this.tabiwLineColor_lbl.Location = new System.Drawing.Point(159, 214);
            this.tabiwLineColor_lbl.Name = "tabiwLineColor_lbl";
            this.tabiwLineColor_lbl.Size = new System.Drawing.Size(41, 12);
            this.tabiwLineColor_lbl.TabIndex = 13;
            this.tabiwLineColor_lbl.Text = "線顏色";
            // 
            // tabiwLineColor_btn
            // 
            this.tabiwLineColor_btn.BackColor = System.Drawing.Color.White;
            this.tabiwLineColor_btn.Location = new System.Drawing.Point(201, 212);
            this.tabiwLineColor_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwLineColor_btn.Name = "tabiwLineColor_btn";
            this.tabiwLineColor_btn.Size = new System.Drawing.Size(21, 19);
            this.tabiwLineColor_btn.TabIndex = 12;
            this.tabiwLineColor_btn.UseVisualStyleBackColor = false;
            this.tabiwLineColor_btn.Click += new System.EventHandler(this.SetButtonBackgroundColor);
            // 
            // tabiwLineCross_btn
            // 
            this.tabiwLineCross_btn.AutoSize = true;
            this.tabiwLineCross_btn.Location = new System.Drawing.Point(161, 192);
            this.tabiwLineCross_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwLineCross_btn.Name = "tabiwLineCross_btn";
            this.tabiwLineCross_btn.Size = new System.Drawing.Size(48, 16);
            this.tabiwLineCross_btn.TabIndex = 11;
            this.tabiwLineCross_btn.Text = "十字";
            this.tabiwLineCross_btn.UseVisualStyleBackColor = true;
            // 
            // tabiwLineOutside_btn
            // 
            this.tabiwLineOutside_btn.AutoSize = true;
            this.tabiwLineOutside_btn.Location = new System.Drawing.Point(161, 173);
            this.tabiwLineOutside_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwLineOutside_btn.Name = "tabiwLineOutside_btn";
            this.tabiwLineOutside_btn.Size = new System.Drawing.Size(48, 16);
            this.tabiwLineOutside_btn.TabIndex = 10;
            this.tabiwLineOutside_btn.Text = "外框";
            this.tabiwLineOutside_btn.UseVisualStyleBackColor = true;
            // 
            // tabiwWinLoc_grp
            // 
            this.tabiwWinLoc_grp.Controls.Add(this.tabiwWinLocPixcelY_nud);
            this.tabiwWinLoc_grp.Controls.Add(this.tabiwWinLocTwo_rdo);
            this.tabiwWinLoc_grp.Controls.Add(this.tabiwWinLocPixcelX_nud);
            this.tabiwWinLoc_grp.Controls.Add(this.tabiwWinLocCenter_rdo);
            this.tabiwWinLoc_grp.Controls.Add(this.tabiwWinLocPixcel_rdo);
            this.tabiwWinLoc_grp.Location = new System.Drawing.Point(5, 165);
            this.tabiwWinLoc_grp.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwWinLoc_grp.Name = "tabiwWinLoc_grp";
            this.tabiwWinLoc_grp.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwWinLoc_grp.Size = new System.Drawing.Size(147, 78);
            this.tabiwWinLoc_grp.TabIndex = 9;
            this.tabiwWinLoc_grp.TabStop = false;
            this.tabiwWinLoc_grp.Text = "視窗位置";
            // 
            // tabiwWinLocPixcelY_nud
            // 
            this.tabiwWinLocPixcelY_nud.Location = new System.Drawing.Point(91, 54);
            this.tabiwWinLocPixcelY_nud.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwWinLocPixcelY_nud.Name = "tabiwWinLocPixcelY_nud";
            this.tabiwWinLocPixcelY_nud.Size = new System.Drawing.Size(51, 22);
            this.tabiwWinLocPixcelY_nud.TabIndex = 7;
            // 
            // tabiwWinLocTwo_rdo
            // 
            this.tabiwWinLocTwo_rdo.AutoSize = true;
            this.tabiwWinLocTwo_rdo.Location = new System.Drawing.Point(69, 13);
            this.tabiwWinLocTwo_rdo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwWinLocTwo_rdo.Name = "tabiwWinLocTwo_rdo";
            this.tabiwWinLocTwo_rdo.Size = new System.Drawing.Size(47, 16);
            this.tabiwWinLocTwo_rdo.TabIndex = 1;
            this.tabiwWinLocTwo_rdo.Text = "兩組";
            this.tabiwWinLocTwo_rdo.UseVisualStyleBackColor = true;
            // 
            // tabiwWinLocPixcelX_nud
            // 
            this.tabiwWinLocPixcelX_nud.Location = new System.Drawing.Point(91, 30);
            this.tabiwWinLocPixcelX_nud.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwWinLocPixcelX_nud.Name = "tabiwWinLocPixcelX_nud";
            this.tabiwWinLocPixcelX_nud.Size = new System.Drawing.Size(51, 22);
            this.tabiwWinLocPixcelX_nud.TabIndex = 6;
            // 
            // tabiwWinLocCenter_rdo
            // 
            this.tabiwWinLocCenter_rdo.AutoSize = true;
            this.tabiwWinLocCenter_rdo.Checked = true;
            this.tabiwWinLocCenter_rdo.Location = new System.Drawing.Point(9, 13);
            this.tabiwWinLocCenter_rdo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwWinLocCenter_rdo.Name = "tabiwWinLocCenter_rdo";
            this.tabiwWinLocCenter_rdo.Size = new System.Drawing.Size(47, 16);
            this.tabiwWinLocCenter_rdo.TabIndex = 0;
            this.tabiwWinLocCenter_rdo.TabStop = true;
            this.tabiwWinLocCenter_rdo.Text = "置中";
            this.tabiwWinLocCenter_rdo.UseVisualStyleBackColor = true;
            // 
            // tabiwWinLocPixcel_rdo
            // 
            this.tabiwWinLocPixcel_rdo.AutoSize = true;
            this.tabiwWinLocPixcel_rdo.Location = new System.Drawing.Point(9, 30);
            this.tabiwWinLocPixcel_rdo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwWinLocPixcel_rdo.Name = "tabiwWinLocPixcel_rdo";
            this.tabiwWinLocPixcel_rdo.Size = new System.Drawing.Size(91, 40);
            this.tabiwWinLocPixcel_rdo.TabIndex = 5;
            this.tabiwWinLocPixcel_rdo.Text = "                   X:\r\n指定Pixel  \r\n                   Y:";
            this.tabiwWinLocPixcel_rdo.UseVisualStyleBackColor = true;
            // 
            // tabiwWinSize_grp
            // 
            this.tabiwWinSize_grp.Controls.Add(this.tabiwWinSizePixelH_nud);
            this.tabiwWinSize_grp.Controls.Add(this.tabiwWinSizePixelW_nud);
            this.tabiwWinSize_grp.Controls.Add(this.tabiwWinSizePixel_rdo);
            this.tabiwWinSize_grp.Controls.Add(this.tabiwWinSizePercent_nud);
            this.tabiwWinSize_grp.Controls.Add(this.tabiwWinSizePercent_rdo);
            this.tabiwWinSize_grp.Location = new System.Drawing.Point(5, 83);
            this.tabiwWinSize_grp.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwWinSize_grp.Name = "tabiwWinSize_grp";
            this.tabiwWinSize_grp.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwWinSize_grp.Size = new System.Drawing.Size(147, 81);
            this.tabiwWinSize_grp.TabIndex = 8;
            this.tabiwWinSize_grp.TabStop = false;
            this.tabiwWinSize_grp.Text = "視窗大小";
            // 
            // tabiwWinSizePixelH_nud
            // 
            this.tabiwWinSizePixelH_nud.Location = new System.Drawing.Point(90, 60);
            this.tabiwWinSizePixelH_nud.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwWinSizePixelH_nud.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.tabiwWinSizePixelH_nud.Name = "tabiwWinSizePixelH_nud";
            this.tabiwWinSizePixelH_nud.Size = new System.Drawing.Size(51, 22);
            this.tabiwWinSizePixelH_nud.TabIndex = 4;
            this.tabiwWinSizePixelH_nud.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // tabiwWinSizePixelW_nud
            // 
            this.tabiwWinSizePixelW_nud.Location = new System.Drawing.Point(89, 37);
            this.tabiwWinSizePixelW_nud.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwWinSizePixelW_nud.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.tabiwWinSizePixelW_nud.Name = "tabiwWinSizePixelW_nud";
            this.tabiwWinSizePixelW_nud.Size = new System.Drawing.Size(52, 22);
            this.tabiwWinSizePixelW_nud.TabIndex = 3;
            this.tabiwWinSizePixelW_nud.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // tabiwWinSizePixel_rdo
            // 
            this.tabiwWinSizePixel_rdo.AutoSize = true;
            this.tabiwWinSizePixel_rdo.Location = new System.Drawing.Point(9, 38);
            this.tabiwWinSizePixel_rdo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwWinSizePixel_rdo.Name = "tabiwWinSizePixel_rdo";
            this.tabiwWinSizePixel_rdo.Size = new System.Drawing.Size(92, 40);
            this.tabiwWinSizePixel_rdo.TabIndex = 2;
            this.tabiwWinSizePixel_rdo.Text = "                  寬:\r\n指定Pixel  \r\n                  高:";
            this.tabiwWinSizePixel_rdo.UseVisualStyleBackColor = true;
            // 
            // tabiwWinSizePercent_nud
            // 
            this.tabiwWinSizePercent_nud.Location = new System.Drawing.Point(77, 13);
            this.tabiwWinSizePercent_nud.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwWinSizePercent_nud.Name = "tabiwWinSizePercent_nud";
            this.tabiwWinSizePercent_nud.Size = new System.Drawing.Size(31, 22);
            this.tabiwWinSizePercent_nud.TabIndex = 1;
            this.tabiwWinSizePercent_nud.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // tabiwWinSizePercent_rdo
            // 
            this.tabiwWinSizePercent_rdo.AutoSize = true;
            this.tabiwWinSizePercent_rdo.Checked = true;
            this.tabiwWinSizePercent_rdo.Location = new System.Drawing.Point(9, 13);
            this.tabiwWinSizePercent_rdo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiwWinSizePercent_rdo.Name = "tabiwWinSizePercent_rdo";
            this.tabiwWinSizePercent_rdo.Size = new System.Drawing.Size(76, 16);
            this.tabiwWinSizePercent_rdo.TabIndex = 0;
            this.tabiwWinSizePercent_rdo.TabStop = true;
            this.tabiwWinSizePercent_rdo.Text = "百分比(%)";
            this.tabiwWinSizePercent_rdo.UseVisualStyleBackColor = true;
            // 
            // tabimgeMask
            // 
            this.tabimgeMask.Controls.Add(this.tabiemPixelGray_lbl);
            this.tabimgeMask.Controls.Add(this.tabiemPixelGray_vsc);
            this.tabimgeMask.Controls.Add(this.tabiemPixelColor_lbl);
            this.tabimgeMask.Controls.Add(this.tabiemPixelLoc_lbl);
            this.tabimgeMask.Controls.Add(this.tabiemPixelColor_btn);
            this.tabimgeMask.Controls.Add(this.tabiemPixelClear_btn);
            this.tabimgeMask.Controls.Add(this.tabiemPixelPanel_pnl);
            this.tabimgeMask.Controls.Add(this.tabiemHNum_nud);
            this.tabimgeMask.Controls.Add(this.tabiemWNum_nud);
            this.tabimgeMask.Controls.Add(this.tabiemHNum_lbl);
            this.tabimgeMask.Controls.Add(this.tabiemWNum_lbl);
            this.tabimgeMask.Controls.Add(this.tabiemSubPixel_rdo);
            this.tabimgeMask.Controls.Add(this.tabiemPixel_rdo);
            this.tabimgeMask.Location = new System.Drawing.Point(23, 269);
            this.tabimgeMask.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabimgeMask.Name = "tabimgeMask";
            this.tabimgeMask.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabimgeMask.Size = new System.Drawing.Size(149, 170);
            this.tabimgeMask.TabIndex = 0;
            this.tabimgeMask.TabStop = false;
            this.tabimgeMask.Text = "Mask(FlickerPattern)";
            // 
            // tabiemPixelGray_lbl
            // 
            this.tabiemPixelGray_lbl.AutoSize = true;
            this.tabiemPixelGray_lbl.Location = new System.Drawing.Point(236, 49);
            this.tabiemPixelGray_lbl.Name = "tabiemPixelGray_lbl";
            this.tabiemPixelGray_lbl.Size = new System.Drawing.Size(52, 12);
            this.tabiemPixelGray_lbl.TabIndex = 15;
            this.tabiemPixelGray_lbl.Text = "Gray: 255";
            this.tabiemPixelGray_lbl.Visible = false;
            // 
            // tabiemPixelGray_vsc
            // 
            this.tabiemPixelGray_vsc.LargeChange = 1;
            this.tabiemPixelGray_vsc.Location = new System.Drawing.Point(239, 68);
            this.tabiemPixelGray_vsc.Maximum = 255;
            this.tabiemPixelGray_vsc.Name = "tabiemPixelGray_vsc";
            this.tabiemPixelGray_vsc.Size = new System.Drawing.Size(20, 192);
            this.tabiemPixelGray_vsc.TabIndex = 14;
            this.tabiemPixelGray_vsc.Visible = false;
            this.tabiemPixelGray_vsc.ValueChanged += new System.EventHandler(this.PixelGray_vsc_ValueChanged);
            // 
            // tabiemPixelColor_lbl
            // 
            this.tabiemPixelColor_lbl.AutoSize = true;
            this.tabiemPixelColor_lbl.Location = new System.Drawing.Point(86, 266);
            this.tabiemPixelColor_lbl.Name = "tabiemPixelColor_lbl";
            this.tabiemPixelColor_lbl.Size = new System.Drawing.Size(53, 12);
            this.tabiemPixelColor_lbl.TabIndex = 13;
            this.tabiemPixelColor_lbl.Text = "選擇顏色";
            // 
            // tabiemPixelLoc_lbl
            // 
            this.tabiemPixelLoc_lbl.AutoSize = true;
            this.tabiemPixelLoc_lbl.Location = new System.Drawing.Point(24, 264);
            this.tabiemPixelLoc_lbl.Name = "tabiemPixelLoc_lbl";
            this.tabiemPixelLoc_lbl.Size = new System.Drawing.Size(24, 12);
            this.tabiemPixelLoc_lbl.TabIndex = 12;
            this.tabiemPixelLoc_lbl.Text = "--,--";
            // 
            // tabiemPixelColor_btn
            // 
            this.tabiemPixelColor_btn.BackColor = System.Drawing.Color.White;
            this.tabiemPixelColor_btn.Location = new System.Drawing.Point(138, 262);
            this.tabiemPixelColor_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiemPixelColor_btn.Name = "tabiemPixelColor_btn";
            this.tabiemPixelColor_btn.Size = new System.Drawing.Size(21, 19);
            this.tabiemPixelColor_btn.TabIndex = 11;
            this.tabiemPixelColor_btn.UseVisualStyleBackColor = false;
            this.tabiemPixelColor_btn.Click += new System.EventHandler(this.SetButtonBackgroundColor);
            // 
            // tabiemPixelClear_btn
            // 
            this.tabiemPixelClear_btn.Location = new System.Drawing.Point(188, 262);
            this.tabiemPixelClear_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiemPixelClear_btn.Name = "tabiemPixelClear_btn";
            this.tabiemPixelClear_btn.Size = new System.Drawing.Size(42, 18);
            this.tabiemPixelClear_btn.TabIndex = 10;
            this.tabiemPixelClear_btn.Text = "Clear";
            this.tabiemPixelClear_btn.UseVisualStyleBackColor = true;
            this.tabiemPixelClear_btn.Click += new System.EventHandler(this.MaskPanelCreat);
            // 
            // tabiemPixelPanel_pnl
            // 
            this.tabiemPixelPanel_pnl.BackgroundImage = global::Thunder.Properties.Resources.transparent;
            this.tabiemPixelPanel_pnl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabiemPixelPanel_pnl.Cursor = System.Windows.Forms.Cursors.Cross;
            this.tabiemPixelPanel_pnl.Location = new System.Drawing.Point(24, 68);
            this.tabiemPixelPanel_pnl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiemPixelPanel_pnl.Name = "tabiemPixelPanel_pnl";
            this.tabiemPixelPanel_pnl.Size = new System.Drawing.Size(206, 192);
            this.tabiemPixelPanel_pnl.TabIndex = 9;
            // 
            // tabiemHNum_nud
            // 
            this.tabiemHNum_nud.Location = new System.Drawing.Point(157, 46);
            this.tabiemHNum_nud.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiemHNum_nud.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.tabiemHNum_nud.Name = "tabiemHNum_nud";
            this.tabiemHNum_nud.Size = new System.Drawing.Size(33, 22);
            this.tabiemHNum_nud.TabIndex = 5;
            this.tabiemHNum_nud.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.tabiemHNum_nud.Click += new System.EventHandler(this.MaskPanelCreat);
            // 
            // tabiemWNum_nud
            // 
            this.tabiemWNum_nud.Location = new System.Drawing.Point(65, 46);
            this.tabiemWNum_nud.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiemWNum_nud.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.tabiemWNum_nud.Name = "tabiemWNum_nud";
            this.tabiemWNum_nud.Size = new System.Drawing.Size(34, 22);
            this.tabiemWNum_nud.TabIndex = 4;
            this.tabiemWNum_nud.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.tabiemWNum_nud.Click += new System.EventHandler(this.MaskPanelCreat);
            // 
            // tabiemHNum_lbl
            // 
            this.tabiemHNum_lbl.AutoSize = true;
            this.tabiemHNum_lbl.Location = new System.Drawing.Point(117, 51);
            this.tabiemHNum_lbl.Name = "tabiemHNum_lbl";
            this.tabiemHNum_lbl.Size = new System.Drawing.Size(36, 12);
            this.tabiemHNum_lbl.TabIndex = 3;
            this.tabiemHNum_lbl.Text = "Height";
            // 
            // tabiemWNum_lbl
            // 
            this.tabiemWNum_lbl.AutoSize = true;
            this.tabiemWNum_lbl.Location = new System.Drawing.Point(26, 51);
            this.tabiemWNum_lbl.Name = "tabiemWNum_lbl";
            this.tabiemWNum_lbl.Size = new System.Drawing.Size(34, 12);
            this.tabiemWNum_lbl.TabIndex = 2;
            this.tabiemWNum_lbl.Text = "Width";
            // 
            // tabiemSubPixel_rdo
            // 
            this.tabiemSubPixel_rdo.AutoSize = true;
            this.tabiemSubPixel_rdo.Location = new System.Drawing.Point(82, 19);
            this.tabiemSubPixel_rdo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiemSubPixel_rdo.Name = "tabiemSubPixel_rdo";
            this.tabiemSubPixel_rdo.Size = new System.Drawing.Size(92, 16);
            this.tabiemSubPixel_rdo.TabIndex = 1;
            this.tabiemSubPixel_rdo.Text = "Sub Pixel Base";
            this.tabiemSubPixel_rdo.UseVisualStyleBackColor = true;
            this.tabiemSubPixel_rdo.CheckedChanged += new System.EventHandler(this.Pixel_rdo_CheckedChanged);
            // 
            // tabiemPixel_rdo
            // 
            this.tabiemPixel_rdo.AutoSize = true;
            this.tabiemPixel_rdo.Checked = true;
            this.tabiemPixel_rdo.Location = new System.Drawing.Point(9, 20);
            this.tabiemPixel_rdo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiemPixel_rdo.Name = "tabiemPixel_rdo";
            this.tabiemPixel_rdo.Size = new System.Drawing.Size(71, 16);
            this.tabiemPixel_rdo.TabIndex = 0;
            this.tabiemPixel_rdo.TabStop = true;
            this.tabiemPixel_rdo.Text = "Pixel Base";
            this.tabiemPixel_rdo.UseVisualStyleBackColor = true;
            this.tabiemPixel_rdo.CheckedChanged += new System.EventHandler(this.Pixel_rdo_CheckedChanged);
            // 
            // tabimgeChess
            // 
            this.tabimgeChess.Controls.Add(this.tabiecBaseColorCustom_lbl);
            this.tabimgeChess.Controls.Add(this.tabiecGray_lbl);
            this.tabimgeChess.Controls.Add(this.tabiecBaseColorCustom_btn);
            this.tabimgeChess.Controls.Add(this.tabiecGray_hsc);
            this.tabimgeChess.Controls.Add(this.tabiecBaseColorM_btn);
            this.tabimgeChess.Controls.Add(this.tabiecVNum_nud);
            this.tabimgeChess.Controls.Add(this.tabiecBaseColorA_btn);
            this.tabimgeChess.Controls.Add(this.tabiecHNum_nud);
            this.tabimgeChess.Controls.Add(this.tabiecBaseColorY_btn);
            this.tabimgeChess.Controls.Add(this.tabiecBaseColor_lbl);
            this.tabimgeChess.Controls.Add(this.tabiecVNum_lbl);
            this.tabimgeChess.Controls.Add(this.tabiecBaseColorB_btn);
            this.tabimgeChess.Controls.Add(this.tabiecHNum_lbl);
            this.tabimgeChess.Controls.Add(this.tabiecBaseColorG_btn);
            this.tabimgeChess.Controls.Add(this.tabiecBaseColorW_btn);
            this.tabimgeChess.Controls.Add(this.tabiecBaseColorR_btn);
            this.tabimgeChess.Location = new System.Drawing.Point(518, 18);
            this.tabimgeChess.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabimgeChess.Name = "tabimgeChess";
            this.tabimgeChess.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabimgeChess.Size = new System.Drawing.Size(215, 160);
            this.tabimgeChess.TabIndex = 0;
            this.tabimgeChess.TabStop = false;
            this.tabimgeChess.Text = "Chess";
            // 
            // tabiecBaseColorCustom_lbl
            // 
            this.tabiecBaseColorCustom_lbl.AutoSize = true;
            this.tabiecBaseColorCustom_lbl.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.tabiecBaseColorCustom_lbl.Location = new System.Drawing.Point(206, 88);
            this.tabiecBaseColorCustom_lbl.Name = "tabiecBaseColorCustom_lbl";
            this.tabiecBaseColorCustom_lbl.Size = new System.Drawing.Size(73, 20);
            this.tabiecBaseColorCustom_lbl.TabIndex = 31;
            this.tabiecBaseColorCustom_lbl.Text = "自訂顏色";
            // 
            // tabiecGray_lbl
            // 
            this.tabiecGray_lbl.AutoSize = true;
            this.tabiecGray_lbl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabiecGray_lbl.Font = new System.Drawing.Font("Microsoft JhengHei UI", 20F);
            this.tabiecGray_lbl.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.tabiecGray_lbl.Location = new System.Drawing.Point(37, 104);
            this.tabiecGray_lbl.Name = "tabiecGray_lbl";
            this.tabiecGray_lbl.Size = new System.Drawing.Size(130, 37);
            this.tabiecGray_lbl.TabIndex = 38;
            this.tabiecGray_lbl.Text = "Gray:255";
            // 
            // tabiecBaseColorCustom_btn
            // 
            this.tabiecBaseColorCustom_btn.BackColor = System.Drawing.Color.Gold;
            this.tabiecBaseColorCustom_btn.BackgroundImage = global::Thunder.Properties.Resources.custom;
            this.tabiecBaseColorCustom_btn.Location = new System.Drawing.Point(285, 84);
            this.tabiecBaseColorCustom_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiecBaseColorCustom_btn.Name = "tabiecBaseColorCustom_btn";
            this.tabiecBaseColorCustom_btn.Size = new System.Drawing.Size(24, 24);
            this.tabiecBaseColorCustom_btn.TabIndex = 27;
            this.tabiecBaseColorCustom_btn.UseVisualStyleBackColor = false;
            this.tabiecBaseColorCustom_btn.Click += new System.EventHandler(this.GradientColor_Click);
            // 
            // tabiecGray_hsc
            // 
            this.tabiecGray_hsc.LargeChange = 1;
            this.tabiecGray_hsc.Location = new System.Drawing.Point(37, 144);
            this.tabiecGray_hsc.Maximum = 255;
            this.tabiecGray_hsc.Name = "tabiecGray_hsc";
            this.tabiecGray_hsc.Size = new System.Drawing.Size(299, 19);
            this.tabiecGray_hsc.TabIndex = 37;
            this.tabiecGray_hsc.Value = 255;
            this.tabiecGray_hsc.ValueChanged += new System.EventHandler(this.hScrollBar1_ValueChanged);
            // 
            // tabiecBaseColorM_btn
            // 
            this.tabiecBaseColorM_btn.BackColor = System.Drawing.Color.Magenta;
            this.tabiecBaseColorM_btn.ForeColor = System.Drawing.Color.Black;
            this.tabiecBaseColorM_btn.Location = new System.Drawing.Point(285, 60);
            this.tabiecBaseColorM_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiecBaseColorM_btn.Name = "tabiecBaseColorM_btn";
            this.tabiecBaseColorM_btn.Size = new System.Drawing.Size(24, 24);
            this.tabiecBaseColorM_btn.TabIndex = 30;
            this.tabiecBaseColorM_btn.Text = "M";
            this.tabiecBaseColorM_btn.UseVisualStyleBackColor = false;
            this.tabiecBaseColorM_btn.Click += new System.EventHandler(this.GradientColor_Click);
            // 
            // tabiecVNum_nud
            // 
            this.tabiecVNum_nud.Location = new System.Drawing.Point(37, 80);
            this.tabiecVNum_nud.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiecVNum_nud.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.tabiecVNum_nud.Name = "tabiecVNum_nud";
            this.tabiecVNum_nud.Size = new System.Drawing.Size(103, 22);
            this.tabiecVNum_nud.TabIndex = 36;
            this.tabiecVNum_nud.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            // 
            // tabiecBaseColorA_btn
            // 
            this.tabiecBaseColorA_btn.BackColor = System.Drawing.Color.Aqua;
            this.tabiecBaseColorA_btn.ForeColor = System.Drawing.Color.Black;
            this.tabiecBaseColorA_btn.Location = new System.Drawing.Point(260, 60);
            this.tabiecBaseColorA_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiecBaseColorA_btn.Name = "tabiecBaseColorA_btn";
            this.tabiecBaseColorA_btn.Size = new System.Drawing.Size(24, 24);
            this.tabiecBaseColorA_btn.TabIndex = 29;
            this.tabiecBaseColorA_btn.Text = "A";
            this.tabiecBaseColorA_btn.UseVisualStyleBackColor = false;
            this.tabiecBaseColorA_btn.Click += new System.EventHandler(this.GradientColor_Click);
            // 
            // tabiecHNum_nud
            // 
            this.tabiecHNum_nud.Location = new System.Drawing.Point(37, 40);
            this.tabiecHNum_nud.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiecHNum_nud.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.tabiecHNum_nud.Name = "tabiecHNum_nud";
            this.tabiecHNum_nud.Size = new System.Drawing.Size(103, 22);
            this.tabiecHNum_nud.TabIndex = 35;
            this.tabiecHNum_nud.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            // 
            // tabiecBaseColorY_btn
            // 
            this.tabiecBaseColorY_btn.BackColor = System.Drawing.Color.Yellow;
            this.tabiecBaseColorY_btn.ForeColor = System.Drawing.Color.Black;
            this.tabiecBaseColorY_btn.Location = new System.Drawing.Point(234, 60);
            this.tabiecBaseColorY_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiecBaseColorY_btn.Name = "tabiecBaseColorY_btn";
            this.tabiecBaseColorY_btn.Size = new System.Drawing.Size(24, 24);
            this.tabiecBaseColorY_btn.TabIndex = 28;
            this.tabiecBaseColorY_btn.Text = "Y";
            this.tabiecBaseColorY_btn.UseVisualStyleBackColor = false;
            this.tabiecBaseColorY_btn.Click += new System.EventHandler(this.GradientColor_Click);
            // 
            // tabiecBaseColor_lbl
            // 
            this.tabiecBaseColor_lbl.AutoSize = true;
            this.tabiecBaseColor_lbl.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F);
            this.tabiecBaseColor_lbl.Location = new System.Drawing.Point(201, 19);
            this.tabiecBaseColor_lbl.Name = "tabiecBaseColor_lbl";
            this.tabiecBaseColor_lbl.Size = new System.Drawing.Size(73, 20);
            this.tabiecBaseColor_lbl.TabIndex = 34;
            this.tabiecBaseColor_lbl.Text = "基底顏色";
            // 
            // tabiecVNum_lbl
            // 
            this.tabiecVNum_lbl.AutoSize = true;
            this.tabiecVNum_lbl.Location = new System.Drawing.Point(37, 64);
            this.tabiecVNum_lbl.Name = "tabiecVNum_lbl";
            this.tabiecVNum_lbl.Size = new System.Drawing.Size(53, 12);
            this.tabiecVNum_lbl.TabIndex = 33;
            this.tabiecVNum_lbl.Text = "垂直幾格";
            // 
            // tabiecBaseColorB_btn
            // 
            this.tabiecBaseColorB_btn.BackColor = System.Drawing.Color.Blue;
            this.tabiecBaseColorB_btn.ForeColor = System.Drawing.Color.White;
            this.tabiecBaseColorB_btn.Location = new System.Drawing.Point(285, 36);
            this.tabiecBaseColorB_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiecBaseColorB_btn.Name = "tabiecBaseColorB_btn";
            this.tabiecBaseColorB_btn.Size = new System.Drawing.Size(24, 24);
            this.tabiecBaseColorB_btn.TabIndex = 25;
            this.tabiecBaseColorB_btn.Text = "B";
            this.tabiecBaseColorB_btn.UseVisualStyleBackColor = false;
            this.tabiecBaseColorB_btn.Click += new System.EventHandler(this.GradientColor_Click);
            // 
            // tabiecHNum_lbl
            // 
            this.tabiecHNum_lbl.AutoSize = true;
            this.tabiecHNum_lbl.Location = new System.Drawing.Point(37, 24);
            this.tabiecHNum_lbl.Name = "tabiecHNum_lbl";
            this.tabiecHNum_lbl.Size = new System.Drawing.Size(53, 12);
            this.tabiecHNum_lbl.TabIndex = 32;
            this.tabiecHNum_lbl.Text = "水平幾格";
            // 
            // tabiecBaseColorG_btn
            // 
            this.tabiecBaseColorG_btn.BackColor = System.Drawing.Color.Lime;
            this.tabiecBaseColorG_btn.ForeColor = System.Drawing.Color.White;
            this.tabiecBaseColorG_btn.Location = new System.Drawing.Point(260, 36);
            this.tabiecBaseColorG_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiecBaseColorG_btn.Name = "tabiecBaseColorG_btn";
            this.tabiecBaseColorG_btn.Size = new System.Drawing.Size(24, 24);
            this.tabiecBaseColorG_btn.TabIndex = 24;
            this.tabiecBaseColorG_btn.Text = "G";
            this.tabiecBaseColorG_btn.UseVisualStyleBackColor = false;
            this.tabiecBaseColorG_btn.Click += new System.EventHandler(this.GradientColor_Click);
            // 
            // tabiecBaseColorW_btn
            // 
            this.tabiecBaseColorW_btn.BackColor = System.Drawing.Color.White;
            this.tabiecBaseColorW_btn.Location = new System.Drawing.Point(208, 36);
            this.tabiecBaseColorW_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiecBaseColorW_btn.Name = "tabiecBaseColorW_btn";
            this.tabiecBaseColorW_btn.Size = new System.Drawing.Size(24, 24);
            this.tabiecBaseColorW_btn.TabIndex = 22;
            this.tabiecBaseColorW_btn.Text = "W";
            this.tabiecBaseColorW_btn.UseVisualStyleBackColor = false;
            this.tabiecBaseColorW_btn.Click += new System.EventHandler(this.GradientColor_Click);
            // 
            // tabiecBaseColorR_btn
            // 
            this.tabiecBaseColorR_btn.BackColor = System.Drawing.Color.Red;
            this.tabiecBaseColorR_btn.ForeColor = System.Drawing.Color.White;
            this.tabiecBaseColorR_btn.Location = new System.Drawing.Point(234, 36);
            this.tabiecBaseColorR_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiecBaseColorR_btn.Name = "tabiecBaseColorR_btn";
            this.tabiecBaseColorR_btn.Size = new System.Drawing.Size(24, 24);
            this.tabiecBaseColorR_btn.TabIndex = 23;
            this.tabiecBaseColorR_btn.Text = "R";
            this.tabiecBaseColorR_btn.UseVisualStyleBackColor = false;
            this.tabiecBaseColorR_btn.Click += new System.EventHandler(this.GradientColor_Click);
            // 
            // tabimgeFrame
            // 
            this.tabimgeFrame.Controls.Add(this.tabiefOther_pnl);
            this.tabimgeFrame.Controls.Add(this.tabiefBack_btn);
            this.tabimgeFrame.Controls.Add(this.tabiefLineColor_lbl);
            this.tabimgeFrame.Controls.Add(this.tabiefBack_pic);
            this.tabimgeFrame.Controls.Add(this.tabiefMassNum_nud);
            this.tabimgeFrame.Controls.Add(this.tabiefMass_chk);
            this.tabimgeFrame.Controls.Add(this.tabiefOther_chk);
            this.tabimgeFrame.Controls.Add(this.tabiefNine_chk);
            this.tabimgeFrame.Controls.Add(this.tabiefOutside_chk);
            this.tabimgeFrame.Controls.Add(this.tabiefCenter_chk);
            this.tabimgeFrame.Controls.Add(this.tabiefLineColor_btn);
            this.tabimgeFrame.Controls.Add(this.tabiefBack_lbl);
            this.tabimgeFrame.ImeMode = System.Windows.Forms.ImeMode.On;
            this.tabimgeFrame.Location = new System.Drawing.Point(33, 111);
            this.tabimgeFrame.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabimgeFrame.Name = "tabimgeFrame";
            this.tabimgeFrame.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabimgeFrame.Size = new System.Drawing.Size(129, 76);
            this.tabimgeFrame.TabIndex = 10;
            this.tabimgeFrame.TabStop = false;
            this.tabimgeFrame.Text = "Frame&&CrossLine&&Border";
            // 
            // tabiefOther_pnl
            // 
            this.tabiefOther_pnl.Controls.Add(this.tabiefOther_cmb);
            this.tabiefOther_pnl.Controls.Add(this.tabiefOtherLoc_hsc);
            this.tabiefOther_pnl.Controls.Add(this.tabiefOther_lst);
            this.tabiefOther_pnl.Controls.Add(this.tabiefOtherAdd_btn);
            this.tabiefOther_pnl.Controls.Add(this.tabiefOtherClear_btn);
            this.tabiefOther_pnl.Controls.Add(this.tabiefOtherColor_btn);
            this.tabiefOther_pnl.Controls.Add(this.tabiefOtherColor_lbl);
            this.tabiefOther_pnl.Controls.Add(this.tabiefOtherLoc_lbl);
            this.tabiefOther_pnl.Enabled = false;
            this.tabiefOther_pnl.Location = new System.Drawing.Point(15, 109);
            this.tabiefOther_pnl.Name = "tabiefOther_pnl";
            this.tabiefOther_pnl.Size = new System.Drawing.Size(426, 175);
            this.tabiefOther_pnl.TabIndex = 33;
            // 
            // tabiefOther_cmb
            // 
            this.tabiefOther_cmb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tabiefOther_cmb.FormattingEnabled = true;
            this.tabiefOther_cmb.Items.AddRange(new object[] {
            "HLine",
            "VLine"});
            this.tabiefOther_cmb.Location = new System.Drawing.Point(16, 6);
            this.tabiefOther_cmb.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiefOther_cmb.Name = "tabiefOther_cmb";
            this.tabiefOther_cmb.Size = new System.Drawing.Size(163, 20);
            this.tabiefOther_cmb.TabIndex = 18;
            this.tabiefOther_cmb.SelectedIndexChanged += new System.EventHandler(this.tabiefOther_cmb_SelectedIndexChanged);
            // 
            // tabiefOtherLoc_hsc
            // 
            this.tabiefOtherLoc_hsc.LargeChange = 1;
            this.tabiefOtherLoc_hsc.Location = new System.Drawing.Point(17, 30);
            this.tabiefOtherLoc_hsc.Minimum = 1;
            this.tabiefOtherLoc_hsc.Name = "tabiefOtherLoc_hsc";
            this.tabiefOtherLoc_hsc.Size = new System.Drawing.Size(249, 19);
            this.tabiefOtherLoc_hsc.TabIndex = 19;
            this.tabiefOtherLoc_hsc.Value = 1;
            this.tabiefOtherLoc_hsc.ValueChanged += new System.EventHandler(this.gbxfHsblocation_ValueChanged);
            // 
            // tabiefOther_lst
            // 
            this.tabiefOther_lst.FormattingEnabled = true;
            this.tabiefOther_lst.ItemHeight = 12;
            this.tabiefOther_lst.Location = new System.Drawing.Point(17, 54);
            this.tabiefOther_lst.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiefOther_lst.Name = "tabiefOther_lst";
            this.tabiefOther_lst.ScrollAlwaysVisible = true;
            this.tabiefOther_lst.Size = new System.Drawing.Size(250, 112);
            this.tabiefOther_lst.TabIndex = 23;
            // 
            // tabiefOtherAdd_btn
            // 
            this.tabiefOtherAdd_btn.Location = new System.Drawing.Point(281, 54);
            this.tabiefOtherAdd_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiefOtherAdd_btn.Name = "tabiefOtherAdd_btn";
            this.tabiefOtherAdd_btn.Size = new System.Drawing.Size(64, 18);
            this.tabiefOtherAdd_btn.TabIndex = 24;
            this.tabiefOtherAdd_btn.Text = "Add";
            this.tabiefOtherAdd_btn.UseVisualStyleBackColor = true;
            this.tabiefOtherAdd_btn.Click += new System.EventHandler(this.gbxfBtnaddclear_Click);
            // 
            // tabiefOtherClear_btn
            // 
            this.tabiefOtherClear_btn.Location = new System.Drawing.Point(281, 79);
            this.tabiefOtherClear_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiefOtherClear_btn.Name = "tabiefOtherClear_btn";
            this.tabiefOtherClear_btn.Size = new System.Drawing.Size(64, 18);
            this.tabiefOtherClear_btn.TabIndex = 25;
            this.tabiefOtherClear_btn.Text = "Clear";
            this.tabiefOtherClear_btn.UseVisualStyleBackColor = true;
            this.tabiefOtherClear_btn.Click += new System.EventHandler(this.gbxfBtnaddclear_Click);
            // 
            // tabiefOtherColor_btn
            // 
            this.tabiefOtherColor_btn.BackColor = System.Drawing.Color.White;
            this.tabiefOtherColor_btn.ForeColor = System.Drawing.Color.White;
            this.tabiefOtherColor_btn.Location = new System.Drawing.Point(242, 7);
            this.tabiefOtherColor_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiefOtherColor_btn.Name = "tabiefOtherColor_btn";
            this.tabiefOtherColor_btn.Size = new System.Drawing.Size(21, 19);
            this.tabiefOtherColor_btn.TabIndex = 26;
            this.tabiefOtherColor_btn.UseVisualStyleBackColor = false;
            this.tabiefOtherColor_btn.Click += new System.EventHandler(this.SetButtonBackgroundColor);
            // 
            // tabiefOtherColor_lbl
            // 
            this.tabiefOtherColor_lbl.AutoSize = true;
            this.tabiefOtherColor_lbl.Location = new System.Drawing.Point(199, 10);
            this.tabiefOtherColor_lbl.Name = "tabiefOtherColor_lbl";
            this.tabiefOtherColor_lbl.Size = new System.Drawing.Size(41, 12);
            this.tabiefOtherColor_lbl.TabIndex = 28;
            this.tabiefOtherColor_lbl.Text = "線顏色";
            // 
            // tabiefOtherLoc_lbl
            // 
            this.tabiefOtherLoc_lbl.AutoSize = true;
            this.tabiefOtherLoc_lbl.Location = new System.Drawing.Point(281, 35);
            this.tabiefOtherLoc_lbl.Name = "tabiefOtherLoc_lbl";
            this.tabiefOtherLoc_lbl.Size = new System.Drawing.Size(11, 12);
            this.tabiefOtherLoc_lbl.TabIndex = 27;
            this.tabiefOtherLoc_lbl.Text = "0";
            // 
            // tabiefBack_btn
            // 
            this.tabiefBack_btn.BackColor = System.Drawing.Color.Black;
            this.tabiefBack_btn.Location = new System.Drawing.Point(69, 15);
            this.tabiefBack_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiefBack_btn.Name = "tabiefBack_btn";
            this.tabiefBack_btn.Size = new System.Drawing.Size(21, 19);
            this.tabiefBack_btn.TabIndex = 9;
            this.tabiefBack_btn.UseVisualStyleBackColor = false;
            this.tabiefBack_btn.Click += new System.EventHandler(this.SetButtonBackgroundColor);
            // 
            // tabiefLineColor_lbl
            // 
            this.tabiefLineColor_lbl.AutoSize = true;
            this.tabiefLineColor_lbl.Location = new System.Drawing.Point(273, 56);
            this.tabiefLineColor_lbl.Name = "tabiefLineColor_lbl";
            this.tabiefLineColor_lbl.Size = new System.Drawing.Size(53, 24);
            this.tabiefLineColor_lbl.TabIndex = 32;
            this.tabiefLineColor_lbl.Text = "\n框線顏色";
            // 
            // tabiefBack_pic
            // 
            this.tabiefBack_pic.BackgroundImage = global::Thunder.Properties.Resources.Img;
            this.tabiefBack_pic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tabiefBack_pic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabiefBack_pic.Location = new System.Drawing.Point(165, 13);
            this.tabiefBack_pic.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiefBack_pic.Name = "tabiefBack_pic";
            this.tabiefBack_pic.Size = new System.Drawing.Size(41, 39);
            this.tabiefBack_pic.TabIndex = 31;
            this.tabiefBack_pic.TabStop = false;
            this.tabiefBack_pic.Click += new System.EventHandler(this.import_Click);
            // 
            // tabiefMassNum_nud
            // 
            this.tabiefMassNum_nud.Location = new System.Drawing.Point(350, 26);
            this.tabiefMassNum_nud.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiefMassNum_nud.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.tabiefMassNum_nud.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.tabiefMassNum_nud.Name = "tabiefMassNum_nud";
            this.tabiefMassNum_nud.Size = new System.Drawing.Size(39, 22);
            this.tabiefMassNum_nud.TabIndex = 30;
            this.tabiefMassNum_nud.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // tabiefMass_chk
            // 
            this.tabiefMass_chk.AutoSize = true;
            this.tabiefMass_chk.Location = new System.Drawing.Point(254, 14);
            this.tabiefMass_chk.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiefMass_chk.Name = "tabiefMass_chk";
            this.tabiefMass_chk.Size = new System.Drawing.Size(188, 28);
            this.tabiefMass_chk.TabIndex = 29;
            this.tabiefMass_chk.Text = "大量建圖\r\n(從0開始，每                  階生成)";
            this.tabiefMass_chk.UseVisualStyleBackColor = true;
            // 
            // tabiefOther_chk
            // 
            this.tabiefOther_chk.AutoSize = true;
            this.tabiefOther_chk.Location = new System.Drawing.Point(13, 88);
            this.tabiefOther_chk.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiefOther_chk.Name = "tabiefOther_chk";
            this.tabiefOther_chk.Size = new System.Drawing.Size(81, 16);
            this.tabiefOther_chk.TabIndex = 17;
            this.tabiefOther_chk.Text = "其他(H&&V)";
            this.tabiefOther_chk.UseVisualStyleBackColor = true;
            this.tabiefOther_chk.CheckStateChanged += new System.EventHandler(this.Framechkother_CheckStateChanged);
            // 
            // tabiefNine_chk
            // 
            this.tabiefNine_chk.AutoSize = true;
            this.tabiefNine_chk.Location = new System.Drawing.Point(151, 65);
            this.tabiefNine_chk.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiefNine_chk.Name = "tabiefNine_chk";
            this.tabiefNine_chk.Size = new System.Drawing.Size(72, 16);
            this.tabiefNine_chk.TabIndex = 16;
            this.tabiefNine_chk.Text = "九點對位";
            this.tabiefNine_chk.UseVisualStyleBackColor = true;
            // 
            // tabiefOutside_chk
            // 
            this.tabiefOutside_chk.AutoSize = true;
            this.tabiefOutside_chk.Location = new System.Drawing.Point(90, 65);
            this.tabiefOutside_chk.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiefOutside_chk.Name = "tabiefOutside_chk";
            this.tabiefOutside_chk.Size = new System.Drawing.Size(60, 16);
            this.tabiefOutside_chk.TabIndex = 15;
            this.tabiefOutside_chk.Text = "外框線";
            this.tabiefOutside_chk.UseVisualStyleBackColor = true;
            // 
            // tabiefCenter_chk
            // 
            this.tabiefCenter_chk.AutoSize = true;
            this.tabiefCenter_chk.Location = new System.Drawing.Point(13, 65);
            this.tabiefCenter_chk.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiefCenter_chk.Name = "tabiefCenter_chk";
            this.tabiefCenter_chk.Size = new System.Drawing.Size(72, 16);
            this.tabiefCenter_chk.TabIndex = 14;
            this.tabiefCenter_chk.Text = "中心對位";
            this.tabiefCenter_chk.UseVisualStyleBackColor = true;
            // 
            // tabiefLineColor_btn
            // 
            this.tabiefLineColor_btn.BackColor = System.Drawing.Color.White;
            this.tabiefLineColor_btn.ForeColor = System.Drawing.Color.White;
            this.tabiefLineColor_btn.Location = new System.Drawing.Point(325, 65);
            this.tabiefLineColor_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabiefLineColor_btn.Name = "tabiefLineColor_btn";
            this.tabiefLineColor_btn.Size = new System.Drawing.Size(21, 19);
            this.tabiefLineColor_btn.TabIndex = 12;
            this.tabiefLineColor_btn.UseVisualStyleBackColor = false;
            this.tabiefLineColor_btn.Click += new System.EventHandler(this.SetButtonBackgroundColor);
            // 
            // tabiefBack_lbl
            // 
            this.tabiefBack_lbl.AutoSize = true;
            this.tabiefBack_lbl.Location = new System.Drawing.Point(10, 19);
            this.tabiefBack_lbl.Name = "tabiefBack_lbl";
            this.tabiefBack_lbl.Size = new System.Drawing.Size(149, 12);
            this.tabiefBack_lbl.TabIndex = 11;
            this.tabiefBack_lbl.Text = "背景顏色                導入截圖";
            // 
            // tabimgeAdjust
            // 
            this.tabimgeAdjust.Controls.Add(this.tabieaPanel_pnl);
            this.tabimgeAdjust.Controls.Add(this.tabieaClear_btn);
            this.tabimgeAdjust.Controls.Add(this.tabieaVFlip_rdo);
            this.tabimgeAdjust.Controls.Add(this.tabieaHFlip_rdo);
            this.tabimgeAdjust.Controls.Add(this.tabieaResize_rdo);
            this.tabimgeAdjust.Controls.Add(this.tabieaImport_btn);
            this.tabimgeAdjust.Controls.Add(this.tabieaTrans_btn);
            this.tabimgeAdjust.Controls.Add(this.tabieaRotate270_rdo);
            this.tabimgeAdjust.Controls.Add(this.tabieaRotate180_rdo);
            this.tabimgeAdjust.Controls.Add(this.tabieaRotate90_rdo);
            this.tabimgeAdjust.Controls.Add(this.tabieaLeftRight_rdo);
            this.tabimgeAdjust.Controls.Add(this.tabieaUpDown_rdo);
            this.tabimgeAdjust.Location = new System.Drawing.Point(338, 40);
            this.tabimgeAdjust.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabimgeAdjust.Name = "tabimgeAdjust";
            this.tabimgeAdjust.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabimgeAdjust.Size = new System.Drawing.Size(145, 62);
            this.tabimgeAdjust.TabIndex = 0;
            this.tabimgeAdjust.TabStop = false;
            this.tabimgeAdjust.Text = "  ImgAdjus";
            // 
            // tabieaPanel_pnl
            // 
            this.tabieaPanel_pnl.Controls.Add(this.tabieaLeftRight_pnl);
            this.tabieaPanel_pnl.Controls.Add(this.tabieaUpDown_pnl);
            this.tabieaPanel_pnl.Controls.Add(this.tabieaPatternList_chl);
            this.tabieaPanel_pnl.Location = new System.Drawing.Point(188, 17);
            this.tabieaPanel_pnl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabieaPanel_pnl.Name = "tabieaPanel_pnl";
            this.tabieaPanel_pnl.Size = new System.Drawing.Size(274, 256);
            this.tabieaPanel_pnl.TabIndex = 12;
            // 
            // tabieaLeftRight_pnl
            // 
            this.tabieaLeftRight_pnl.Controls.Add(this.tabieaLeftRightR_pic);
            this.tabieaLeftRight_pnl.Controls.Add(this.tabieaLeftRightL_pic);
            this.tabieaLeftRight_pnl.Location = new System.Drawing.Point(157, 11);
            this.tabieaLeftRight_pnl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabieaLeftRight_pnl.Name = "tabieaLeftRight_pnl";
            this.tabieaLeftRight_pnl.Size = new System.Drawing.Size(83, 98);
            this.tabieaLeftRight_pnl.TabIndex = 9;
            // 
            // tabieaLeftRightR_pic
            // 
            this.tabieaLeftRightR_pic.BackgroundImage = global::Thunder.Properties.Resources.Img;
            this.tabieaLeftRightR_pic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tabieaLeftRightR_pic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabieaLeftRightR_pic.Dock = System.Windows.Forms.DockStyle.Right;
            this.tabieaLeftRightR_pic.Location = new System.Drawing.Point(-54, 0);
            this.tabieaLeftRightR_pic.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabieaLeftRightR_pic.Name = "tabieaLeftRightR_pic";
            this.tabieaLeftRightR_pic.Size = new System.Drawing.Size(137, 98);
            this.tabieaLeftRightR_pic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.tabieaLeftRightR_pic.TabIndex = 1;
            this.tabieaLeftRightR_pic.TabStop = false;
            this.tabieaLeftRightR_pic.Click += new System.EventHandler(this.tabiea_pic_Click);
            // 
            // tabieaLeftRightL_pic
            // 
            this.tabieaLeftRightL_pic.BackgroundImage = global::Thunder.Properties.Resources.Img;
            this.tabieaLeftRightL_pic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tabieaLeftRightL_pic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabieaLeftRightL_pic.Dock = System.Windows.Forms.DockStyle.Left;
            this.tabieaLeftRightL_pic.Location = new System.Drawing.Point(0, 0);
            this.tabieaLeftRightL_pic.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabieaLeftRightL_pic.Name = "tabieaLeftRightL_pic";
            this.tabieaLeftRightL_pic.Size = new System.Drawing.Size(137, 98);
            this.tabieaLeftRightL_pic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.tabieaLeftRightL_pic.TabIndex = 0;
            this.tabieaLeftRightL_pic.TabStop = false;
            this.tabieaLeftRightL_pic.Click += new System.EventHandler(this.tabiea_pic_Click);
            // 
            // tabieaUpDown_pnl
            // 
            this.tabieaUpDown_pnl.Controls.Add(this.tabieaUpDownD_pic);
            this.tabieaUpDown_pnl.Controls.Add(this.tabieaUpDownU_pic);
            this.tabieaUpDown_pnl.Location = new System.Drawing.Point(16, 15);
            this.tabieaUpDown_pnl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabieaUpDown_pnl.Name = "tabieaUpDown_pnl";
            this.tabieaUpDown_pnl.Size = new System.Drawing.Size(111, 93);
            this.tabieaUpDown_pnl.TabIndex = 8;
            // 
            // tabieaUpDownD_pic
            // 
            this.tabieaUpDownD_pic.BackgroundImage = global::Thunder.Properties.Resources.Img;
            this.tabieaUpDownD_pic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tabieaUpDownD_pic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabieaUpDownD_pic.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabieaUpDownD_pic.Location = new System.Drawing.Point(0, -35);
            this.tabieaUpDownD_pic.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabieaUpDownD_pic.Name = "tabieaUpDownD_pic";
            this.tabieaUpDownD_pic.Size = new System.Drawing.Size(111, 128);
            this.tabieaUpDownD_pic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.tabieaUpDownD_pic.TabIndex = 1;
            this.tabieaUpDownD_pic.TabStop = false;
            this.tabieaUpDownD_pic.Click += new System.EventHandler(this.tabiea_pic_Click);
            // 
            // tabieaUpDownU_pic
            // 
            this.tabieaUpDownU_pic.BackgroundImage = global::Thunder.Properties.Resources.Img;
            this.tabieaUpDownU_pic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tabieaUpDownU_pic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabieaUpDownU_pic.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabieaUpDownU_pic.Location = new System.Drawing.Point(0, 0);
            this.tabieaUpDownU_pic.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabieaUpDownU_pic.Name = "tabieaUpDownU_pic";
            this.tabieaUpDownU_pic.Size = new System.Drawing.Size(111, 128);
            this.tabieaUpDownU_pic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.tabieaUpDownU_pic.TabIndex = 0;
            this.tabieaUpDownU_pic.TabStop = false;
            this.tabieaUpDownU_pic.Click += new System.EventHandler(this.tabiea_pic_Click);
            // 
            // tabieaPatternList_chl
            // 
            this.tabieaPatternList_chl.CheckOnClick = true;
            this.tabieaPatternList_chl.FormattingEnabled = true;
            this.tabieaPatternList_chl.HorizontalScrollbar = true;
            this.tabieaPatternList_chl.Location = new System.Drawing.Point(16, 137);
            this.tabieaPatternList_chl.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabieaPatternList_chl.Name = "tabieaPatternList_chl";
            this.tabieaPatternList_chl.Size = new System.Drawing.Size(250, 72);
            this.tabieaPatternList_chl.TabIndex = 7;
            this.tabieaPatternList_chl.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.tabieaPatternList_chL_ItemCheck);
            // 
            // tabieaClear_btn
            // 
            this.tabieaClear_btn.Location = new System.Drawing.Point(99, 15);
            this.tabieaClear_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabieaClear_btn.Name = "tabieaClear_btn";
            this.tabieaClear_btn.Size = new System.Drawing.Size(64, 18);
            this.tabieaClear_btn.TabIndex = 11;
            this.tabieaClear_btn.Text = "Clear";
            this.tabieaClear_btn.UseVisualStyleBackColor = true;
            this.tabieaClear_btn.Click += new System.EventHandler(this.tabieaClear_btn_Click);
            // 
            // tabieaVFlip_rdo
            // 
            this.tabieaVFlip_rdo.AutoSize = true;
            this.tabieaVFlip_rdo.Location = new System.Drawing.Point(18, 180);
            this.tabieaVFlip_rdo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabieaVFlip_rdo.Name = "tabieaVFlip_rdo";
            this.tabieaVFlip_rdo.Size = new System.Drawing.Size(71, 16);
            this.tabieaVFlip_rdo.TabIndex = 10;
            this.tabieaVFlip_rdo.Text = "垂直翻轉";
            this.tabieaVFlip_rdo.UseVisualStyleBackColor = true;
            this.tabieaVFlip_rdo.CheckedChanged += new System.EventHandler(this.tabiea_rdo_CheckedChanged);
            // 
            // tabieaHFlip_rdo
            // 
            this.tabieaHFlip_rdo.AutoSize = true;
            this.tabieaHFlip_rdo.Location = new System.Drawing.Point(17, 160);
            this.tabieaHFlip_rdo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabieaHFlip_rdo.Name = "tabieaHFlip_rdo";
            this.tabieaHFlip_rdo.Size = new System.Drawing.Size(71, 16);
            this.tabieaHFlip_rdo.TabIndex = 9;
            this.tabieaHFlip_rdo.Text = "水平翻轉";
            this.tabieaHFlip_rdo.UseVisualStyleBackColor = true;
            this.tabieaHFlip_rdo.CheckedChanged += new System.EventHandler(this.tabiea_rdo_CheckedChanged);
            // 
            // tabieaResize_rdo
            // 
            this.tabieaResize_rdo.AutoSize = true;
            this.tabieaResize_rdo.Location = new System.Drawing.Point(17, 40);
            this.tabieaResize_rdo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabieaResize_rdo.Name = "tabieaResize_rdo";
            this.tabieaResize_rdo.Size = new System.Drawing.Size(55, 16);
            this.tabieaResize_rdo.TabIndex = 8;
            this.tabieaResize_rdo.Text = "ReSize";
            this.tabieaResize_rdo.UseVisualStyleBackColor = true;
            this.tabieaResize_rdo.CheckedChanged += new System.EventHandler(this.tabiea_rdo_CheckedChanged);
            // 
            // tabieaImport_btn
            // 
            this.tabieaImport_btn.Location = new System.Drawing.Point(23, 15);
            this.tabieaImport_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabieaImport_btn.Name = "tabieaImport_btn";
            this.tabieaImport_btn.Size = new System.Drawing.Size(64, 18);
            this.tabieaImport_btn.TabIndex = 6;
            this.tabieaImport_btn.Text = "導入圖片";
            this.tabieaImport_btn.UseVisualStyleBackColor = true;
            this.tabieaImport_btn.Click += new System.EventHandler(this.import_Click);
            // 
            // tabieaTrans_btn
            // 
            this.tabieaTrans_btn.Location = new System.Drawing.Point(15, 200);
            this.tabieaTrans_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabieaTrans_btn.Name = "tabieaTrans_btn";
            this.tabieaTrans_btn.Size = new System.Drawing.Size(64, 18);
            this.tabieaTrans_btn.TabIndex = 5;
            this.tabieaTrans_btn.Text = "Translate";
            this.tabieaTrans_btn.UseVisualStyleBackColor = true;
            this.tabieaTrans_btn.Click += new System.EventHandler(this.tabieaTrans_btn_Click);
            // 
            // tabieaRotate270_rdo
            // 
            this.tabieaRotate270_rdo.AutoSize = true;
            this.tabieaRotate270_rdo.Location = new System.Drawing.Point(17, 140);
            this.tabieaRotate270_rdo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabieaRotate270_rdo.Name = "tabieaRotate270_rdo";
            this.tabieaRotate270_rdo.Size = new System.Drawing.Size(68, 16);
            this.tabieaRotate270_rdo.TabIndex = 4;
            this.tabieaRotate270_rdo.Text = "旋轉270°";
            this.tabieaRotate270_rdo.UseVisualStyleBackColor = true;
            this.tabieaRotate270_rdo.CheckedChanged += new System.EventHandler(this.tabiea_rdo_CheckedChanged);
            // 
            // tabieaRotate180_rdo
            // 
            this.tabieaRotate180_rdo.AutoSize = true;
            this.tabieaRotate180_rdo.Location = new System.Drawing.Point(17, 120);
            this.tabieaRotate180_rdo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabieaRotate180_rdo.Name = "tabieaRotate180_rdo";
            this.tabieaRotate180_rdo.Size = new System.Drawing.Size(68, 16);
            this.tabieaRotate180_rdo.TabIndex = 3;
            this.tabieaRotate180_rdo.Text = "旋轉180°";
            this.tabieaRotate180_rdo.UseVisualStyleBackColor = true;
            this.tabieaRotate180_rdo.CheckedChanged += new System.EventHandler(this.tabiea_rdo_CheckedChanged);
            // 
            // tabieaRotate90_rdo
            // 
            this.tabieaRotate90_rdo.AutoSize = true;
            this.tabieaRotate90_rdo.Location = new System.Drawing.Point(17, 100);
            this.tabieaRotate90_rdo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabieaRotate90_rdo.Name = "tabieaRotate90_rdo";
            this.tabieaRotate90_rdo.Size = new System.Drawing.Size(62, 16);
            this.tabieaRotate90_rdo.TabIndex = 2;
            this.tabieaRotate90_rdo.Text = "旋轉90°";
            this.tabieaRotate90_rdo.UseVisualStyleBackColor = true;
            this.tabieaRotate90_rdo.CheckedChanged += new System.EventHandler(this.tabiea_rdo_CheckedChanged);
            // 
            // tabieaLeftRight_rdo
            // 
            this.tabieaLeftRight_rdo.AutoSize = true;
            this.tabieaLeftRight_rdo.Location = new System.Drawing.Point(17, 80);
            this.tabieaLeftRight_rdo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabieaLeftRight_rdo.Name = "tabieaLeftRight_rdo";
            this.tabieaLeftRight_rdo.Size = new System.Drawing.Size(71, 16);
            this.tabieaLeftRight_rdo.TabIndex = 1;
            this.tabieaLeftRight_rdo.Text = "左右組合";
            this.tabieaLeftRight_rdo.UseVisualStyleBackColor = true;
            this.tabieaLeftRight_rdo.CheckedChanged += new System.EventHandler(this.tabiea_rdo_CheckedChanged);
            // 
            // tabieaUpDown_rdo
            // 
            this.tabieaUpDown_rdo.AutoSize = true;
            this.tabieaUpDown_rdo.Location = new System.Drawing.Point(17, 60);
            this.tabieaUpDown_rdo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabieaUpDown_rdo.Name = "tabieaUpDown_rdo";
            this.tabieaUpDown_rdo.Size = new System.Drawing.Size(71, 16);
            this.tabieaUpDown_rdo.TabIndex = 0;
            this.tabieaUpDown_rdo.Text = "上下組合";
            this.tabieaUpDown_rdo.UseVisualStyleBackColor = true;
            this.tabieaUpDown_rdo.CheckedChanged += new System.EventHandler(this.tabiea_rdo_CheckedChanged);
            // 
            // tabimgeMass
            // 
            this.tabimgeMass.Controls.Add(this.label1);
            this.tabimgeMass.Location = new System.Drawing.Point(221, 42);
            this.tabimgeMass.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabimgeMass.Name = "tabimgeMass";
            this.tabimgeMass.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabimgeMass.Size = new System.Drawing.Size(93, 79);
            this.tabimgeMass.TabIndex = 11;
            this.tabimgeMass.TabStop = false;
            this.tabimgeMass.Text = "MassImage";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "ToBeContinued";
            // 
            // tabimgeFuncList
            // 
            this.tabimgeFuncList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tabimgeFuncList.FormattingEnabled = true;
            this.tabimgeFuncList.Items.AddRange(new object[] {
            "Frame&CrossLine&Border",
            "Gradient&ColorBar",
            "Chess",
            "Window",
            "Mask(FlickerPattern)",
            "ImageAdjust",
            "MassImage"});
            this.tabimgeFuncList.Location = new System.Drawing.Point(3, 2);
            this.tabimgeFuncList.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabimgeFuncList.Name = "tabimgeFuncList";
            this.tabimgeFuncList.Size = new System.Drawing.Size(250, 20);
            this.tabimgeFuncList.TabIndex = 5;
            this.tabimgeFuncList.SelectedIndexChanged += new System.EventHandler(this.cmbFunclist_SelectedIndexChanged);
            // 
            // tabDirList
            // 
            this.tabDirList.Controls.Add(this.tabdlPatternList_lvw);
            this.tabDirList.Location = new System.Drawing.Point(4, 22);
            this.tabDirList.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabDirList.Name = "tabDirList";
            this.tabDirList.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabDirList.Size = new System.Drawing.Size(799, 490);
            this.tabDirList.TabIndex = 1;
            this.tabDirList.Text = "DirList";
            this.tabDirList.UseVisualStyleBackColor = true;
            // 
            // tabdlPatternList_lvw
            // 
            this.tabdlPatternList_lvw.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabdlPatternList_lvw.HideSelection = false;
            this.tabdlPatternList_lvw.Location = new System.Drawing.Point(3, 2);
            this.tabdlPatternList_lvw.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabdlPatternList_lvw.Name = "tabdlPatternList_lvw";
            this.tabdlPatternList_lvw.Size = new System.Drawing.Size(793, 486);
            this.tabdlPatternList_lvw.TabIndex = 11;
            this.tabdlPatternList_lvw.UseCompatibleStateImageBehavior = false;
            this.tabdlPatternList_lvw.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
            // 
            // tabImgList
            // 
            this.tabImgList.Controls.Add(this.tabilPatternList_lst);
            this.tabImgList.Location = new System.Drawing.Point(4, 22);
            this.tabImgList.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabImgList.Name = "tabImgList";
            this.tabImgList.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabImgList.Size = new System.Drawing.Size(799, 490);
            this.tabImgList.TabIndex = 2;
            this.tabImgList.Text = "ImgList";
            this.tabImgList.UseVisualStyleBackColor = true;
            // 
            // tabilPatternList_lst
            // 
            this.tabilPatternList_lst.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabilPatternList_lst.FormattingEnabled = true;
            this.tabilPatternList_lst.ItemHeight = 12;
            this.tabilPatternList_lst.Items.AddRange(new object[] {
            "White",
            "Black",
            "Red",
            "Green",
            "Blue"});
            this.tabilPatternList_lst.Location = new System.Drawing.Point(3, 2);
            this.tabilPatternList_lst.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabilPatternList_lst.Name = "tabilPatternList_lst";
            this.tabilPatternList_lst.Size = new System.Drawing.Size(793, 486);
            this.tabilPatternList_lst.TabIndex = 0;
            // 
            // Showimage_flp
            // 
            this.Showimage_flp.AutoSize = true;
            this.Showimage_flp.Controls.Add(this.showimgSave_btn);
            this.Showimage_flp.Controls.Add(this.showimgGenerate_btn);
            this.Showimage_flp.Controls.Add(this.showimgPicture_pic);
            this.Showimage_flp.Controls.Add(this.showimgSize_btn);
            this.Showimage_flp.Dock = System.Windows.Forms.DockStyle.Right;
            this.Showimage_flp.FlowDirection = System.Windows.Forms.FlowDirection.BottomUp;
            this.Showimage_flp.Location = new System.Drawing.Point(1129, 27);
            this.Showimage_flp.Margin = new System.Windows.Forms.Padding(0);
            this.Showimage_flp.Name = "Showimage_flp";
            this.Showimage_flp.Size = new System.Drawing.Size(263, 516);
            this.Showimage_flp.TabIndex = 8;
            this.Showimage_flp.WrapContents = false;
            // 
            // showimgSave_btn
            // 
            this.showimgSave_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.showimgSave_btn.Location = new System.Drawing.Point(196, 496);
            this.showimgSave_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.showimgSave_btn.Name = "showimgSave_btn";
            this.showimgSave_btn.Size = new System.Drawing.Size(64, 18);
            this.showimgSave_btn.TabIndex = 0;
            this.showimgSave_btn.Text = "Save";
            this.showimgSave_btn.UseVisualStyleBackColor = true;
            this.showimgSave_btn.Click += new System.EventHandler(this.Save_Click);
            // 
            // showimgGenerate_btn
            // 
            this.showimgGenerate_btn.Location = new System.Drawing.Point(3, 474);
            this.showimgGenerate_btn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.showimgGenerate_btn.Name = "showimgGenerate_btn";
            this.showimgGenerate_btn.Size = new System.Drawing.Size(96, 18);
            this.showimgGenerate_btn.TabIndex = 1;
            this.showimgGenerate_btn.Text = "Generate";
            this.showimgGenerate_btn.UseVisualStyleBackColor = true;
            this.showimgGenerate_btn.Click += new System.EventHandler(this.Generate_Click);
            // 
            // showimgPicture_pic
            // 
            this.showimgPicture_pic.AllowDrop = true;
            this.showimgPicture_pic.BackColor = System.Drawing.Color.Black;
            this.showimgPicture_pic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.showimgPicture_pic.ContextMenuStrip = this.contextMenuStrip;
            this.showimgPicture_pic.Location = new System.Drawing.Point(3, 336);
            this.showimgPicture_pic.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.showimgPicture_pic.MinimumSize = new System.Drawing.Size(171, 0);
            this.showimgPicture_pic.Name = "showimgPicture_pic";
            this.showimgPicture_pic.Size = new System.Drawing.Size(257, 134);
            this.showimgPicture_pic.TabIndex = 7;
            this.showimgPicture_pic.TabStop = false;
            this.showimgPicture_pic.DragDrop += new System.Windows.Forms.DragEventHandler(this.pictureBox_DragDrop);
            this.showimgPicture_pic.DragEnter += new System.Windows.Forms.DragEventHandler(this.pictureBox_DragEnter);
            this.showimgPicture_pic.DoubleClick += new System.EventHandler(this.FullScreen);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmsShowLoc,
            this.toolStripSeparator1,
            this.cmsRotate,
            this.cmsToBeContinued,
            this.cms,
            this.toolStripComboBox1});
            this.contextMenuStrip.Name = "contextMenuStrip1";
            this.contextMenuStrip.Size = new System.Drawing.Size(182, 121);
            this.contextMenuStrip.Opened += new System.EventHandler(this.contextMenuStrip_Opened);
            // 
            // cmsShowLoc
            // 
            this.cmsShowLoc.Name = "cmsShowLoc";
            this.cmsShowLoc.Size = new System.Drawing.Size(181, 22);
            this.cmsShowLoc.Text = "顯示座標";
            this.cmsShowLoc.Click += new System.EventHandler(this.cmsShowLoc_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(178, 6);
            // 
            // cmsRotate
            // 
            this.cmsRotate.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.旋轉ToolStripMenuItem,
            this.旋轉180ToolStripMenuItem,
            this.旋轉270ToolStripMenuItem,
            this.水平翻轉ToolStripMenuItem,
            this.垂直翻轉ToolStripMenuItem});
            this.cmsRotate.Name = "cmsRotate";
            this.cmsRotate.Size = new System.Drawing.Size(181, 22);
            this.cmsRotate.Text = "Adjust";
            // 
            // 旋轉ToolStripMenuItem
            // 
            this.旋轉ToolStripMenuItem.Name = "旋轉ToolStripMenuItem";
            this.旋轉ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.旋轉ToolStripMenuItem.Text = "旋轉90";
            // 
            // 旋轉180ToolStripMenuItem
            // 
            this.旋轉180ToolStripMenuItem.Name = "旋轉180ToolStripMenuItem";
            this.旋轉180ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.旋轉180ToolStripMenuItem.Text = "旋轉180";
            // 
            // 旋轉270ToolStripMenuItem
            // 
            this.旋轉270ToolStripMenuItem.Name = "旋轉270ToolStripMenuItem";
            this.旋轉270ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.旋轉270ToolStripMenuItem.Text = "旋轉270";
            // 
            // 水平翻轉ToolStripMenuItem
            // 
            this.水平翻轉ToolStripMenuItem.Name = "水平翻轉ToolStripMenuItem";
            this.水平翻轉ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.水平翻轉ToolStripMenuItem.Text = "水平翻轉";
            // 
            // 垂直翻轉ToolStripMenuItem
            // 
            this.垂直翻轉ToolStripMenuItem.Name = "垂直翻轉ToolStripMenuItem";
            this.垂直翻轉ToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.垂直翻轉ToolStripMenuItem.Text = "垂直翻轉";
            // 
            // cmsToBeContinued
            // 
            this.cmsToBeContinued.Name = "cmsToBeContinued";
            this.cmsToBeContinued.Size = new System.Drawing.Size(181, 22);
            this.cmsToBeContinued.Text = "ToBeContinued";
            // 
            // cms
            // 
            this.cms.AutoToolTip = true;
            this.cms.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.cms.Enabled = false;
            this.cms.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F);
            this.cms.Name = "cms";
            this.cms.ReadOnly = true;
            this.cms.Size = new System.Drawing.Size(100, 16);
            this.cms.Text = "ToBeContinued";
            this.cms.ToolTipText = "{e}";
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(121, 23);
            this.toolStripComboBox1.Text = "ToBeContinued";
            // 
            // showimgSize_btn
            // 
            this.showimgSize_btn.AutoSize = true;
            this.showimgSize_btn.Location = new System.Drawing.Point(3, 322);
            this.showimgSize_btn.Name = "showimgSize_btn";
            this.showimgSize_btn.Size = new System.Drawing.Size(0, 12);
            this.showimgSize_btn.TabIndex = 8;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ssrStatus_lbl,
            this.ssrProgressbar_prg});
            this.statusStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.statusStrip.Location = new System.Drawing.Point(0, 543);
            this.statusStrip.MinimumSize = new System.Drawing.Size(257, 0);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 12, 0);
            this.statusStrip.Size = new System.Drawing.Size(1392, 20);
            this.statusStrip.TabIndex = 14;
            this.statusStrip.Text = "statusStrip1";
            // 
            // ssrStatus_lbl
            // 
            this.ssrStatus_lbl.Name = "ssrStatus_lbl";
            this.ssrStatus_lbl.Size = new System.Drawing.Size(41, 15);
            this.ssrStatus_lbl.Text = "Status";
            // 
            // ssrProgressbar_prg
            // 
            this.ssrProgressbar_prg.Name = "ssrProgressbar_prg";
            this.ssrProgressbar_prg.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ssrProgressbar_prg.Size = new System.Drawing.Size(171, 13);
            this.ssrProgressbar_prg.Visible = false;
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 5000;
            this.toolTip.InitialDelay = 500;
            this.toolTip.ReshowDelay = 1000;
            this.toolTip.ShowAlways = true;
            // 
            // Mainwindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1392, 563);
            this.Controls.Add(this.Showimage_flp);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(763, 452);
            this.Name = "Mainwindow";
            this.Text = "PatternMagic";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabImgEditor.ResumeLayout(false);
            this.tabimgePanel.ResumeLayout(false);
            this.tabimgeGradient.ResumeLayout(false);
            this.tabimgeGradient.PerformLayout();
            this.tabigOther_grp.ResumeLayout(false);
            this.tabigOther_grp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabigOtherSplit_nud)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabimgeWindow.ResumeLayout(false);
            this.tabimgeWindow.PerformLayout();
            this.tabiwCustom_grp.ResumeLayout(false);
            this.tabiwCustomGrad_grp.ResumeLayout(false);
            this.tabiwCustomGrad_grp.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.tabiwCustomMask_grp.ResumeLayout(false);
            this.tabiwCustomMask_grp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabiwCMaskHNum_nud)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabiwCMaskWNum_nud)).EndInit();
            this.tabiwWin_grp.ResumeLayout(false);
            this.tabiwWin_grp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabiwWin_pic)).EndInit();
            this.tabiwBack_grp.ResumeLayout(false);
            this.tabiwBack_grp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabiwBack_pic)).EndInit();
            this.tabiwWinLoc_grp.ResumeLayout(false);
            this.tabiwWinLoc_grp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabiwWinLocPixcelY_nud)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabiwWinLocPixcelX_nud)).EndInit();
            this.tabiwWinSize_grp.ResumeLayout(false);
            this.tabiwWinSize_grp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabiwWinSizePixelH_nud)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabiwWinSizePixelW_nud)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabiwWinSizePercent_nud)).EndInit();
            this.tabimgeMask.ResumeLayout(false);
            this.tabimgeMask.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabiemHNum_nud)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabiemWNum_nud)).EndInit();
            this.tabimgeChess.ResumeLayout(false);
            this.tabimgeChess.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabiecVNum_nud)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabiecHNum_nud)).EndInit();
            this.tabimgeFrame.ResumeLayout(false);
            this.tabimgeFrame.PerformLayout();
            this.tabiefOther_pnl.ResumeLayout(false);
            this.tabiefOther_pnl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tabiefBack_pic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabiefMassNum_nud)).EndInit();
            this.tabimgeAdjust.ResumeLayout(false);
            this.tabimgeAdjust.PerformLayout();
            this.tabieaPanel_pnl.ResumeLayout(false);
            this.tabieaLeftRight_pnl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tabieaLeftRightR_pic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabieaLeftRightL_pic)).EndInit();
            this.tabieaUpDown_pnl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tabieaUpDownD_pic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabieaUpDownU_pic)).EndInit();
            this.tabimgeMass.ResumeLayout(false);
            this.tabimgeMass.PerformLayout();
            this.tabDirList.ResumeLayout(false);
            this.tabImgList.ResumeLayout(false);
            this.Showimage_flp.ResumeLayout(false);
            this.Showimage_flp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.showimgPicture_pic)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.contextMenuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private MenuStrip menuStrip;
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
        private RadioButton tabigOtherHSplit_rdo;
        private RadioButton tabigOtherVSplit_rdo;
        private Button tabigFourColor_btn;
        private Button tabigBaseColorCustom_btn;
        private Button tabigBaseColorB_btn;
        private Button tabigBaseColorG_btn;
        private Button tabigBaseColorR_btn;
        private Button tabigBaseColorW_btn;
        private Label tabigBaseColor_lbl;
        private ComboBox tabigLastLevel_cmb;
        private ComboBox tabigFirstLevel_cmb;
        private Label tabigFirstLevel_lbl;
        private ComboBox tabigDivid_cmb;
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
        private Label tabigOtherColor_lbl;
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
        private RadioButton tabieaRotate180_rdo;
        private RadioButton tabieaRotate90_rdo;
        private RadioButton tabieaRotate270_rdo;
        private StatusStrip statusStrip;
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
        private Panel tabiemPixelPanel_pnl;
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
        private Button tabiemPixelClear_btn;
        private Button tabiemPixelColor_btn;
        private GroupBox tabiwCustomGrad_grp;
        private GroupBox tabiwCustom_grp;
        private GroupBox tabiwCustomMask_grp;
        private VScrollBar tabiwCMaskPixelGray_vsc;
        private Label tabiwCMaskPixelColor_lbl;
        private Label tabiwCMaskPixelLoc_lbl;
        private Button tabiwCMaskPixelColor_btn;
        private Button tabiwCMaskPixelClear_btn;
        private Panel tabiwCMaskPixelPanel_pnl;
        private NumericUpDown tabiwCMaskHNum_nud;
        private NumericUpDown tabiwCMaskWNum_nud;
        private Label tabiwCMaskHNum_lbl;
        private Label tabiwCMaskWNum_lbl;
        private VScrollBar tabiemPixelGray_vsc;
        private Label tabiemPixelColor_lbl;
        private Label tabiemPixelLoc_lbl;
        private ContextMenuStrip contextMenuStrip;
        private RadioButton tabiwCMaskSubPixel_rdo;
        private RadioButton tabiwCMaskPixel_rdo;
        private Label tabiemPixelGray_lbl;
        private Label tabiwCMaskPixelGray_lbl;
        private ToolStripMenuItem cmsShowLoc;
        private ToolStripMenuItem cmsRotate;
        private ToolStripMenuItem cmsToBeContinued;
        private ToolTip toolTip;
        private ToolStripTextBox cms;
        private ToolStripSeparator toolStripSeparator1;
        private Panel tabiefOther_pnl;
        private Label label1;
        private ToolStripMenuItem 旋轉ToolStripMenuItem;
        private ToolStripMenuItem 旋轉180ToolStripMenuItem;
        private ToolStripMenuItem 旋轉270ToolStripMenuItem;
        private ToolStripMenuItem 水平翻轉ToolStripMenuItem;
        private ToolStripMenuItem 垂直翻轉ToolStripMenuItem;
        private ToolStripComboBox toolStripComboBox1;
        private Label tabigStep_lbl;
        private ComboBox tabigStep_cmb;
        private Panel panel1;
        private Panel tabigOtherSplit_pnl;
        private GroupBox tabigOther_grp;
        private GroupBox groupBox1;
        private RadioButton radioButton1;
        private Panel tabiwCGradOtherSplit_pnl;
        private RadioButton radioButton2;
        private Button button1;
        private NumericUpDown numericUpDown1;
        private Label label2;
        private Label label3;
        private Button button2;
        private Button button3;
        private Button button4;
        private Button button5;
        private Button button6;
        private Button button7;
        private Button button8;
        private Button button9;
        private Button button10;
        private Label tabiwCGrad0;
        private ComboBox comboBox1;
        private Panel panel3;
        private Button button11;
        private Label label5;
        private Button button12;
        private Button button13;
        private Button button14;
        private Button button15;
        private RadioButton tabiwCGradVWay_rdo;
        private ComboBox comboBox2;
        private RadioButton tabiwCGrad;
        private RadioButton tabiwCGradHWay_rdo;
        private ComboBox comboBox3;
        private ComboBox comboBox4;
        private Label label4;
        private Label tabigOtherSplitLoc_lbl;
        private ToolStripMenuItem mnsGenerate_btn;
        private CheckBox tabigOther_chk;
    }
}
