namespace Thunder
{
    partial class pictureWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmsShowLoc = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.cmsSlideshow = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsSlideshow1 = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsSlideshow3 = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsSlideshow5 = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsSlideshow10 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.cmsSlideshowText = new System.Windows.Forms.ToolStripTextBox();
            this.cmsSlideshowTextButton = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsSlideshowStop = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cmsAdjust = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsAdjustRotate90 = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsAdjustRotate180 = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsAdjustRotate270 = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsAdjustHFlip = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsAdjustVFlip = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsScreenlist = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cmsClose = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.picwinPicture_pic = new System.Windows.Forms.PictureBox();
            this.timerSlideshow = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picwinPicture_pic)).BeginInit();
            this.SuspendLayout();
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmsShowLoc,
            this.toolStripSeparator3,
            this.cmsSlideshow,
            this.cmsSlideshowStop,
            this.toolStripSeparator1,
            this.cmsAdjust,
            this.cmsScreenlist,
            this.toolStripSeparator2,
            this.cmsClose});
            this.contextMenuStrip.Name = "contextMenuStrip1";
            this.contextMenuStrip.Size = new System.Drawing.Size(182, 159);
            this.contextMenuStrip.Opened += new System.EventHandler(this.contextMenuStrip_Opened);
            // 
            // cmsShowLoc
            // 
            this.cmsShowLoc.Name = "cmsShowLoc";
            this.cmsShowLoc.Size = new System.Drawing.Size(181, 22);
            this.cmsShowLoc.Text = "顯示座標";
            this.cmsShowLoc.Click += new System.EventHandler(this.cmsShowLoc_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(178, 6);
            // 
            // cmsSlideshow
            // 
            this.cmsSlideshow.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmsSlideshow1,
            this.cmsSlideshow3,
            this.cmsSlideshow5,
            this.cmsSlideshow10,
            this.toolStripSeparator4,
            this.cmsSlideshowText,
            this.cmsSlideshowTextButton});
            this.cmsSlideshow.Name = "cmsSlideshow";
            this.cmsSlideshow.Size = new System.Drawing.Size(181, 22);
            this.cmsSlideshow.Text = "幻燈片";
            // 
            // cmsSlideshow1
            // 
            this.cmsSlideshow1.Name = "cmsSlideshow1";
            this.cmsSlideshow1.Size = new System.Drawing.Size(204, 22);
            this.cmsSlideshow1.Text = "每1秒";
            this.cmsSlideshow1.Click += new System.EventHandler(this.Slideshow);
            // 
            // cmsSlideshow3
            // 
            this.cmsSlideshow3.Name = "cmsSlideshow3";
            this.cmsSlideshow3.Size = new System.Drawing.Size(204, 22);
            this.cmsSlideshow3.Text = "每3秒";
            this.cmsSlideshow3.Click += new System.EventHandler(this.Slideshow);
            // 
            // cmsSlideshow5
            // 
            this.cmsSlideshow5.Name = "cmsSlideshow5";
            this.cmsSlideshow5.Size = new System.Drawing.Size(204, 22);
            this.cmsSlideshow5.Text = "每5秒";
            this.cmsSlideshow5.Click += new System.EventHandler(this.Slideshow);
            // 
            // cmsSlideshow10
            // 
            this.cmsSlideshow10.Name = "cmsSlideshow10";
            this.cmsSlideshow10.Size = new System.Drawing.Size(204, 22);
            this.cmsSlideshow10.Text = "每10秒";
            this.cmsSlideshow10.Click += new System.EventHandler(this.Slideshow);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(201, 6);
            // 
            // cmsSlideshowText
            // 
            this.cmsSlideshowText.Font = new System.Drawing.Font("Microsoft JhengHei UI", 9F);
            this.cmsSlideshowText.Name = "cmsSlideshowText";
            this.cmsSlideshowText.Size = new System.Drawing.Size(100, 23);
            // 
            // cmsSlideshowTextButton
            // 
            this.cmsSlideshowTextButton.Name = "cmsSlideshowTextButton";
            this.cmsSlideshowTextButton.Size = new System.Drawing.Size(204, 22);
            this.cmsSlideshowTextButton.Text = "上面輸入幾\"毫\"秒後按我";
            this.cmsSlideshowTextButton.Click += new System.EventHandler(this.Slideshow);
            // 
            // cmsSlideshowStop
            // 
            this.cmsSlideshowStop.Name = "cmsSlideshowStop";
            this.cmsSlideshowStop.Size = new System.Drawing.Size(181, 22);
            this.cmsSlideshowStop.Text = "停止幻燈片";
            this.cmsSlideshowStop.Click += new System.EventHandler(this.Slideshow);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(178, 6);
            // 
            // cmsAdjust
            // 
            this.cmsAdjust.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmsAdjustRotate90,
            this.cmsAdjustRotate180,
            this.cmsAdjustRotate270,
            this.cmsAdjustHFlip,
            this.cmsAdjustVFlip});
            this.cmsAdjust.Name = "cmsAdjust";
            this.cmsAdjust.Size = new System.Drawing.Size(181, 22);
            this.cmsAdjust.Text = "旋轉/翻轉";
            // 
            // cmsAdjustRotate90
            // 
            this.cmsAdjustRotate90.Name = "cmsAdjustRotate90";
            this.cmsAdjustRotate90.Size = new System.Drawing.Size(122, 22);
            this.cmsAdjustRotate90.Text = "旋轉90";
            this.cmsAdjustRotate90.Click += new System.EventHandler(this.cmsAdjust_Click);
            // 
            // cmsAdjustRotate180
            // 
            this.cmsAdjustRotate180.Name = "cmsAdjustRotate180";
            this.cmsAdjustRotate180.Size = new System.Drawing.Size(122, 22);
            this.cmsAdjustRotate180.Text = "旋轉180";
            this.cmsAdjustRotate180.Click += new System.EventHandler(this.cmsAdjust_Click);
            // 
            // cmsAdjustRotate270
            // 
            this.cmsAdjustRotate270.Name = "cmsAdjustRotate270";
            this.cmsAdjustRotate270.Size = new System.Drawing.Size(122, 22);
            this.cmsAdjustRotate270.Text = "旋轉270";
            this.cmsAdjustRotate270.Click += new System.EventHandler(this.cmsAdjust_Click);
            // 
            // cmsAdjustHFlip
            // 
            this.cmsAdjustHFlip.Name = "cmsAdjustHFlip";
            this.cmsAdjustHFlip.Size = new System.Drawing.Size(122, 22);
            this.cmsAdjustHFlip.Text = "水平翻轉";
            this.cmsAdjustHFlip.Click += new System.EventHandler(this.cmsAdjust_Click);
            // 
            // cmsAdjustVFlip
            // 
            this.cmsAdjustVFlip.Name = "cmsAdjustVFlip";
            this.cmsAdjustVFlip.Size = new System.Drawing.Size(122, 22);
            this.cmsAdjustVFlip.Text = "垂直翻轉";
            this.cmsAdjustVFlip.Click += new System.EventHandler(this.cmsAdjust_Click);
            // 
            // cmsScreenlist
            // 
            this.cmsScreenlist.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmsScreenlist.Name = "cmsScreenlist";
            this.cmsScreenlist.Size = new System.Drawing.Size(121, 23);
            this.cmsScreenlist.SelectedIndexChanged += new System.EventHandler(this.cmsScreenlist_cmb_SelectedIndexChanged);
            this.cmsScreenlist.Click += new System.EventHandler(this.Screenlist);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(178, 6);
            // 
            // cmsClose
            // 
            this.cmsClose.Name = "cmsClose";
            this.cmsClose.Size = new System.Drawing.Size(181, 22);
            this.cmsClose.Text = "Close";
            this.cmsClose.Click += new System.EventHandler(this.cmsToBeContinued_Click);
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 5000;
            this.toolTip.InitialDelay = 300;
            this.toolTip.ReshowDelay = 1000;
            this.toolTip.ShowAlways = true;
            // 
            // picwinPicture_pic
            // 
            this.picwinPicture_pic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picwinPicture_pic.Location = new System.Drawing.Point(0, 0);
            this.picwinPicture_pic.Margin = new System.Windows.Forms.Padding(0);
            this.picwinPicture_pic.Name = "picwinPicture_pic";
            this.picwinPicture_pic.Size = new System.Drawing.Size(800, 450);
            this.picwinPicture_pic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picwinPicture_pic.TabIndex = 1;
            this.picwinPicture_pic.TabStop = false;
            this.picwinPicture_pic.DoubleClick += new System.EventHandler(this.pictureWindow_DoubleClick);
            // 
            // timerSlideshow
            // 
            this.timerSlideshow.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // pictureWindow
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Desktop;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.ContextMenuStrip = this.contextMenuStrip;
            this.Controls.Add(this.picwinPicture_pic);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.KeyPreview = true;
            this.Name = "pictureWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "PatternShow";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.pictureWindow_FormClosed);
            this.Load += new System.EventHandler(this.pictureWindow_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.pictureWindow_KeyDown);
            this.contextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picwinPicture_pic)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem cmsShowLoc;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem cmsAdjust;
        private System.Windows.Forms.ToolStripMenuItem cmsAdjustRotate90;
        private System.Windows.Forms.ToolStripMenuItem cmsAdjustRotate180;
        private System.Windows.Forms.ToolStripMenuItem cmsAdjustRotate270;
        private System.Windows.Forms.ToolStripMenuItem cmsAdjustHFlip;
        private System.Windows.Forms.ToolStripMenuItem cmsAdjustVFlip;
        private System.Windows.Forms.ToolStripMenuItem cmsClose;
        private System.Windows.Forms.ToolStripComboBox cmsScreenlist;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.PictureBox picwinPicture_pic;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem cmsSlideshow;
        private System.Windows.Forms.ToolStripMenuItem cmsSlideshow5;
        private System.Windows.Forms.ToolStripMenuItem cmsSlideshow10;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripTextBox cmsSlideshowText;
        private System.Windows.Forms.ToolStripMenuItem cmsSlideshowTextButton;
        private System.Windows.Forms.ToolStripMenuItem cmsSlideshowStop;
        private System.Windows.Forms.Timer timerSlideshow;
        private System.Windows.Forms.ToolStripMenuItem cmsSlideshow1;
        private System.Windows.Forms.ToolStripMenuItem cmsSlideshow3;
    }
}