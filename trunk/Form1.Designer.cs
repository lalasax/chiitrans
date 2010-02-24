namespace ChiiTrans
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.browser = new System.Windows.Forms.WebBrowser();
            this.textBoxDebug = new System.Windows.Forms.TextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.buttonOnOff = new System.Windows.Forms.ToolStripButton();
            this.buttonRun = new System.Windows.Forms.ToolStripButton();
            this.buttonOptions = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonClear = new System.Windows.Forms.ToolStripButton();
            this.buttonTranslateSelected = new System.Windows.Forms.ToolStripButton();
            this.buttonRepeat = new System.Windows.Forms.ToolStripButton();
            this.buttonTranslateFull = new System.Windows.Forms.ToolStripButton();
            this.buttonPaste = new System.Windows.Forms.ToolStripButton();
            this.buttonAddReplacement = new System.Windows.Forms.ToolStripButton();
            this.buttonClearCache = new System.Windows.Forms.ToolStripButton();
            this.buttonMonitor = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonTransparent = new System.Windows.Forms.ToolStripButton();
            this.buttonFullscreen = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripHomePage = new System.Windows.Forms.ToolStripButton();
            this.openFileDialogRun = new System.Windows.Forms.OpenFileDialog();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "on");
            this.imageList1.Images.SetKeyName(1, "off");
            this.imageList1.Images.SetKeyName(2, "loading");
            // 
            // browser
            // 
            this.browser.AllowWebBrowserDrop = false;
            this.browser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.browser.Location = new System.Drawing.Point(0, 28);
            this.browser.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.browser.MinimumSize = new System.Drawing.Size(20, 20);
            this.browser.Name = "browser";
            this.browser.Size = new System.Drawing.Size(782, 528);
            this.browser.TabIndex = 4;
            this.browser.Url = new System.Uri("", System.UriKind.Relative);
            this.browser.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.browser_PreviewKeyDown);
            // 
            // textBoxDebug
            // 
            this.textBoxDebug.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDebug.Location = new System.Drawing.Point(551, 411);
            this.textBoxDebug.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBoxDebug.Multiline = true;
            this.textBoxDebug.Name = "textBoxDebug";
            this.textBoxDebug.Size = new System.Drawing.Size(201, 133);
            this.textBoxDebug.TabIndex = 7;
            this.textBoxDebug.TabStop = false;
            this.textBoxDebug.Visible = false;
            this.textBoxDebug.Click += new System.EventHandler(this.textBoxDebug_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(22, 22);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonOnOff,
            this.buttonRun,
            this.buttonOptions,
            this.toolStripSeparator2,
            this.buttonClear,
            this.buttonTranslateSelected,
            this.buttonRepeat,
            this.buttonTranslateFull,
            this.buttonPaste,
            this.buttonAddReplacement,
            this.buttonClearCache,
            this.buttonMonitor,
            this.toolStripSeparator1,
            this.buttonTransparent,
            this.buttonFullscreen,
            this.toolStripSeparator3,
            this.toolStripHomePage});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(782, 29);
            this.toolStrip1.TabIndex = 9;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // buttonOnOff
            // 
            this.buttonOnOff.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonOnOff.Enabled = false;
            this.buttonOnOff.Image = ((System.Drawing.Image)(resources.GetObject("buttonOnOff.Image")));
            this.buttonOnOff.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonOnOff.Name = "buttonOnOff";
            this.buttonOnOff.Size = new System.Drawing.Size(26, 26);
            this.buttonOnOff.Text = "Loading...";
            this.buttonOnOff.Click += new System.EventHandler(this.button2_Click);
            // 
            // buttonRun
            // 
            this.buttonRun.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonRun.Image = ((System.Drawing.Image)(resources.GetObject("buttonRun.Image")));
            this.buttonRun.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Size = new System.Drawing.Size(26, 26);
            this.buttonRun.Text = "Run game to translate (G)";
            this.buttonRun.Click += new System.EventHandler(this.buttonRun_Click);
            // 
            // buttonOptions
            // 
            this.buttonOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonOptions.Image = ((System.Drawing.Image)(resources.GetObject("buttonOptions.Image")));
            this.buttonOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonOptions.Name = "buttonOptions";
            this.buttonOptions.Size = new System.Drawing.Size(26, 26);
            this.buttonOptions.Text = "Options (O)";
            this.buttonOptions.Click += new System.EventHandler(this.button1_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 29);
            // 
            // buttonClear
            // 
            this.buttonClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonClear.Image = ((System.Drawing.Image)(resources.GetObject("buttonClear.Image")));
            this.buttonClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(26, 26);
            this.buttonClear.Text = "Clear window (Delete)";
            this.buttonClear.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // buttonTranslateSelected
            // 
            this.buttonTranslateSelected.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonTranslateSelected.Image = ((System.Drawing.Image)(resources.GetObject("buttonTranslateSelected.Image")));
            this.buttonTranslateSelected.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonTranslateSelected.Name = "buttonTranslateSelected";
            this.buttonTranslateSelected.Size = new System.Drawing.Size(26, 26);
            this.buttonTranslateSelected.Text = "Translate selected text (Space)";
            this.buttonTranslateSelected.Click += new System.EventHandler(this.buttonTranslateSelected_Click);
            // 
            // buttonRepeat
            // 
            this.buttonRepeat.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonRepeat.Image = ((System.Drawing.Image)(resources.GetObject("buttonRepeat.Image")));
            this.buttonRepeat.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonRepeat.Name = "buttonRepeat";
            this.buttonRepeat.Size = new System.Drawing.Size(26, 26);
            this.buttonRepeat.Text = "Repeat last translation (R)";
            this.buttonRepeat.Click += new System.EventHandler(this.button3_Click);
            // 
            // buttonTranslateFull
            // 
            this.buttonTranslateFull.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonTranslateFull.Image = ((System.Drawing.Image)(resources.GetObject("buttonTranslateFull.Image")));
            this.buttonTranslateFull.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonTranslateFull.Name = "buttonTranslateFull";
            this.buttonTranslateFull.Size = new System.Drawing.Size(26, 26);
            this.buttonTranslateFull.Text = "Repeat last translation, using all translators (Alt-R)";
            this.buttonTranslateFull.Click += new System.EventHandler(this.buttonTranslateFull_Click);
            // 
            // buttonPaste
            // 
            this.buttonPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonPaste.Image = ((System.Drawing.Image)(resources.GetObject("buttonPaste.Image")));
            this.buttonPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonPaste.Name = "buttonPaste";
            this.buttonPaste.Size = new System.Drawing.Size(26, 26);
            this.buttonPaste.Text = "Translate text from clipboard (Ctrl-V)";
            this.buttonPaste.Click += new System.EventHandler(this.buttonPaste_Click);
            // 
            // buttonAddReplacement
            // 
            this.buttonAddReplacement.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonAddReplacement.Image = ((System.Drawing.Image)(resources.GetObject("buttonAddReplacement.Image")));
            this.buttonAddReplacement.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonAddReplacement.Name = "buttonAddReplacement";
            this.buttonAddReplacement.Size = new System.Drawing.Size(26, 26);
            this.buttonAddReplacement.Text = "Add a text replacement (Insert)";
            this.buttonAddReplacement.Click += new System.EventHandler(this.buttonAddReplacement_Click);
            // 
            // buttonClearCache
            // 
            this.buttonClearCache.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonClearCache.Image = ((System.Drawing.Image)(resources.GetObject("buttonClearCache.Image")));
            this.buttonClearCache.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonClearCache.Name = "buttonClearCache";
            this.buttonClearCache.Size = new System.Drawing.Size(26, 26);
            this.buttonClearCache.Text = "Clear translations cache";
            this.buttonClearCache.Click += new System.EventHandler(this.buttonClearCache_Click);
            // 
            // buttonMonitor
            // 
            this.buttonMonitor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonMonitor.Image = ((System.Drawing.Image)(resources.GetObject("buttonMonitor.Image")));
            this.buttonMonitor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonMonitor.Name = "buttonMonitor";
            this.buttonMonitor.Size = new System.Drawing.Size(26, 26);
            this.buttonMonitor.Text = "Select monitored threads (M)";
            this.buttonMonitor.Click += new System.EventHandler(this.buttonMonitor_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 29);
            // 
            // buttonTransparent
            // 
            this.buttonTransparent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonTransparent.Image = ((System.Drawing.Image)(resources.GetObject("buttonTransparent.Image")));
            this.buttonTransparent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonTransparent.Name = "buttonTransparent";
            this.buttonTransparent.Size = new System.Drawing.Size(26, 26);
            this.buttonTransparent.Text = "Transparent mode (Alt-&Z)";
            this.buttonTransparent.Click += new System.EventHandler(this.buttonTransparent_Click);
            // 
            // buttonFullscreen
            // 
            this.buttonFullscreen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonFullscreen.Image = ((System.Drawing.Image)(resources.GetObject("buttonFullscreen.Image")));
            this.buttonFullscreen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonFullscreen.Name = "buttonFullscreen";
            this.buttonFullscreen.Size = new System.Drawing.Size(26, 26);
            this.buttonFullscreen.Text = "Toggle fullscreen (Alt-&F)";
            this.buttonFullscreen.Click += new System.EventHandler(this.buttonFullscreen_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 29);
            // 
            // toolStripHomePage
            // 
            this.toolStripHomePage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripHomePage.Image = ((System.Drawing.Image)(resources.GetObject("toolStripHomePage.Image")));
            this.toolStripHomePage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripHomePage.Name = "toolStripHomePage";
            this.toolStripHomePage.Size = new System.Drawing.Size(26, 26);
            this.toolStripHomePage.Text = "Go to ChiiTrans\' home page";
            this.toolStripHomePage.Click += new System.EventHandler(this.toolStripHomePage_Click);
            // 
            // openFileDialogRun
            // 
            this.openFileDialogRun.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
            this.openFileDialogRun.FilterIndex = 0;
            this.openFileDialogRun.Title = "Select a game to run";
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(782, 555);
            this.Controls.Add(this.textBoxDebug);
            this.Controls.Add(this.browser);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "ChiiTrans - Automatic translation tool for Japanese games";
            this.Deactivate += new System.EventHandler(this.Form1_Deactivate);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.Activated += new System.EventHandler(this.Form1_Activated);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.LocationChanged += new System.EventHandler(this.Form1_LocationChanged);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.WebBrowser browser;
        private System.Windows.Forms.TextBox textBoxDebug;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton buttonOnOff;
        private System.Windows.Forms.ToolStripButton buttonRepeat;
        private System.Windows.Forms.ToolStripButton buttonClear;
        private System.Windows.Forms.ToolStripButton buttonOptions;
        private System.Windows.Forms.ToolStripButton buttonAddReplacement;
        private System.Windows.Forms.ToolStripButton buttonTransparent;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton buttonMonitor;
        private System.Windows.Forms.ToolStripButton buttonTranslateSelected;
        private System.Windows.Forms.ToolStripButton buttonRun;
        public System.Windows.Forms.OpenFileDialog openFileDialogRun;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton buttonFullscreen;
        private System.Windows.Forms.ToolStripButton buttonPaste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripHomePage;
        private System.Windows.Forms.ToolStripButton buttonTranslateFull;
        private System.Windows.Forms.ToolStripButton buttonClearCache;
    }
}

