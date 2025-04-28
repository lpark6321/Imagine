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
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cmsAdjust = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsAdjustRotate90 = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsAdjustRotate180 = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsAdjustRotate270 = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsAdjustHFlip = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsAdjustVFlip = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsScreenlist = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cmsToBeContinued = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.picwinPicture_pic = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picwinPicture_pic)).BeginInit();
            this.SuspendLayout();
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmsShowLoc,
            this.toolStripSeparator1,
            this.cmsAdjust,
            this.cmsScreenlist,
            this.toolStripSeparator2,
            this.cmsToBeContinued});
            this.contextMenuStrip.Name = "contextMenuStrip1";
            this.contextMenuStrip.Size = new System.Drawing.Size(182, 109);
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
            this.cmsAdjustRotate90.Size = new System.Drawing.Size(180, 22);
            this.cmsAdjustRotate90.Text = "旋轉90";
            this.cmsAdjustRotate90.Click += new System.EventHandler(this.cmsAdjust_Click);
            // 
            // cmsAdjustRotate180
            // 
            this.cmsAdjustRotate180.Name = "cmsAdjustRotate180";
            this.cmsAdjustRotate180.Size = new System.Drawing.Size(180, 22);
            this.cmsAdjustRotate180.Text = "旋轉180";
            this.cmsAdjustRotate180.Click += new System.EventHandler(this.cmsAdjust_Click);
            // 
            // cmsAdjustRotate270
            // 
            this.cmsAdjustRotate270.Name = "cmsAdjustRotate270";
            this.cmsAdjustRotate270.Size = new System.Drawing.Size(180, 22);
            this.cmsAdjustRotate270.Text = "旋轉270";
            this.cmsAdjustRotate270.Click += new System.EventHandler(this.cmsAdjust_Click);
            // 
            // cmsAdjustHFlip
            // 
            this.cmsAdjustHFlip.Name = "cmsAdjustHFlip";
            this.cmsAdjustHFlip.Size = new System.Drawing.Size(180, 22);
            this.cmsAdjustHFlip.Text = "水平翻轉";
            this.cmsAdjustHFlip.Click += new System.EventHandler(this.cmsAdjust_Click);
            // 
            // cmsAdjustVFlip
            // 
            this.cmsAdjustVFlip.Name = "cmsAdjustVFlip";
            this.cmsAdjustVFlip.Size = new System.Drawing.Size(180, 22);
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
            // cmsToBeContinued
            // 
            this.cmsToBeContinued.Name = "cmsToBeContinued";
            this.cmsToBeContinued.Size = new System.Drawing.Size(181, 22);
            this.cmsToBeContinued.Text = "Close";
            this.cmsToBeContinued.Click += new System.EventHandler(this.cmsToBeContinued_Click);
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
            this.picwinPicture_pic.Name = "picwinPicture_pic";
            this.picwinPicture_pic.Size = new System.Drawing.Size(800, 450);
            this.picwinPicture_pic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picwinPicture_pic.TabIndex = 1;
            this.picwinPicture_pic.TabStop = false;
            this.picwinPicture_pic.DoubleClick += new System.EventHandler(this.pictureWindow_DoubleClick);
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
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "pictureWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Form2";
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
        private System.Windows.Forms.ToolStripMenuItem cmsToBeContinued;
        private System.Windows.Forms.ToolStripComboBox cmsScreenlist;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.PictureBox picwinPicture_pic;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}