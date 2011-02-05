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
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuitemTranslate = new System.Windows.Forms.ToolStripMenuItem();
            this.menuitemTranslateAll = new System.Windows.Forms.ToolStripMenuItem();
            this.searchInDictionaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuitemNewReplacement = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.menuitemCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.menuitemPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.menuitemOnOff = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemFullscreen = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemTransparent = new System.Windows.Forms.ToolStripMenuItem();
            this.showToolbarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemClipboardMonitoring = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.menuitemOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.menuitemMore = new System.Windows.Forms.ToolStripMenuItem();
            this.menuitemRun = new System.Windows.Forms.ToolStripMenuItem();
            this.menuitemClear = new System.Windows.Forms.ToolStripMenuItem();
            this.menuitemKataHira = new System.Windows.Forms.ToolStripMenuItem();
            this.menuitemClearCache = new System.Windows.Forms.ToolStripMenuItem();
            this.menuitemMonitor = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadUserDictionaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showFormBordersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addACustomDictionaryEntryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textBoxDebug = new System.Windows.Forms.TextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.buttonOnOff = new System.Windows.Forms.ToolStripButton();
            this.buttonRun = new System.Windows.Forms.ToolStripButton();
            this.buttonOptions = new System.Windows.Forms.ToolStripButton();
            this.buttonMonitor = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonClear = new System.Windows.Forms.ToolStripButton();
            this.buttonTranslateSelected = new System.Windows.Forms.ToolStripButton();
            this.buttonTranslateFull = new System.Windows.Forms.ToolStripButton();
            this.buttonPaste = new System.Windows.Forms.ToolStripButton();
            this.buttonClipboardMonitoring = new System.Windows.Forms.ToolStripButton();
            this.buttonKataHira = new System.Windows.Forms.ToolStripButton();
            this.buttonDictionary = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonAddReplacement = new System.Windows.Forms.ToolStripButton();
            this.buttonUserDict = new System.Windows.Forms.ToolStripButton();
            this.buttonClearCache = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonFullscreen = new System.Windows.Forms.ToolStripButton();
            this.buttonTransparent = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonUpdateEDICT = new System.Windows.Forms.ToolStripButton();
            this.toolStripHomePage = new System.Windows.Forms.ToolStripButton();
            this.openFileDialogRun = new System.Windows.Forms.OpenFileDialog();
            this.contextMenu.SuspendLayout();
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
            this.browser.ContextMenuStrip = this.contextMenu;
            this.browser.IsWebBrowserContextMenuEnabled = false;
            this.browser.Location = new System.Drawing.Point(0, 28);
            this.browser.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.browser.MinimumSize = new System.Drawing.Size(20, 20);
            this.browser.Name = "browser";
            this.browser.Size = new System.Drawing.Size(782, 528);
            this.browser.TabIndex = 4;
            this.browser.Url = new System.Uri("", System.UriKind.Relative);
            this.browser.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.browser_PreviewKeyDown);
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuitemTranslate,
            this.menuitemTranslateAll,
            this.searchInDictionaryToolStripMenuItem,
            this.menuitemNewReplacement,
            this.toolStripSeparator5,
            this.menuitemCopy,
            this.menuitemPaste,
            this.toolStripSeparator6,
            this.menuitemOnOff,
            this.menuItemFullscreen,
            this.menuItemTransparent,
            this.showToolbarToolStripMenuItem,
            this.menuItemClipboardMonitoring,
            this.toolStripSeparator8,
            this.menuitemOptions,
            this.menuitemMore});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(279, 356);
            this.contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenu_Opening);
            // 
            // menuitemTranslate
            // 
            this.menuitemTranslate.Name = "menuitemTranslate";
            this.menuitemTranslate.ShortcutKeyDisplayString = "Space";
            this.menuitemTranslate.Size = new System.Drawing.Size(278, 24);
            this.menuitemTranslate.Text = "Translate";
            this.menuitemTranslate.Click += new System.EventHandler(this.buttonTranslateSelected_Click);
            // 
            // menuitemTranslateAll
            // 
            this.menuitemTranslateAll.Name = "menuitemTranslateAll";
            this.menuitemTranslateAll.ShortcutKeyDisplayString = "A";
            this.menuitemTranslateAll.Size = new System.Drawing.Size(278, 24);
            this.menuitemTranslateAll.Text = "Translate with all";
            this.menuitemTranslateAll.Click += new System.EventHandler(this.buttonTranslateFull_Click);
            // 
            // searchInDictionaryToolStripMenuItem
            // 
            this.searchInDictionaryToolStripMenuItem.Name = "searchInDictionaryToolStripMenuItem";
            this.searchInDictionaryToolStripMenuItem.ShortcutKeyDisplayString = "D";
            this.searchInDictionaryToolStripMenuItem.Size = new System.Drawing.Size(278, 24);
            this.searchInDictionaryToolStripMenuItem.Text = "Search in dictionary";
            this.searchInDictionaryToolStripMenuItem.Click += new System.EventHandler(this.buttonDictionary_Click);
            // 
            // menuitemNewReplacement
            // 
            this.menuitemNewReplacement.Name = "menuitemNewReplacement";
            this.menuitemNewReplacement.ShortcutKeyDisplayString = "Insert";
            this.menuitemNewReplacement.Size = new System.Drawing.Size(278, 24);
            this.menuitemNewReplacement.Text = "Add new replacement...";
            this.menuitemNewReplacement.Click += new System.EventHandler(this.buttonAddReplacement_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(275, 6);
            // 
            // menuitemCopy
            // 
            this.menuitemCopy.Name = "menuitemCopy";
            this.menuitemCopy.ShortcutKeyDisplayString = "Ctrl-C";
            this.menuitemCopy.Size = new System.Drawing.Size(278, 24);
            this.menuitemCopy.Text = "Copy";
            this.menuitemCopy.Click += new System.EventHandler(this.menuitemCopy_Click);
            // 
            // menuitemPaste
            // 
            this.menuitemPaste.Name = "menuitemPaste";
            this.menuitemPaste.ShortcutKeyDisplayString = "Ctrl-V";
            this.menuitemPaste.Size = new System.Drawing.Size(278, 24);
            this.menuitemPaste.Text = "Paste";
            this.menuitemPaste.Click += new System.EventHandler(this.buttonPaste_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(275, 6);
            // 
            // menuitemOnOff
            // 
            this.menuitemOnOff.Name = "menuitemOnOff";
            this.menuitemOnOff.ShortcutKeyDisplayString = "T";
            this.menuitemOnOff.Size = new System.Drawing.Size(278, 24);
            this.menuitemOnOff.Text = "Active";
            this.menuitemOnOff.Click += new System.EventHandler(this.button2_Click);
            // 
            // menuItemFullscreen
            // 
            this.menuItemFullscreen.Name = "menuItemFullscreen";
            this.menuItemFullscreen.ShortcutKeyDisplayString = "Alt-F";
            this.menuItemFullscreen.Size = new System.Drawing.Size(278, 24);
            this.menuItemFullscreen.Text = "Fullscreen";
            this.menuItemFullscreen.Click += new System.EventHandler(this.buttonFullscreen_Click);
            // 
            // menuItemTransparent
            // 
            this.menuItemTransparent.Name = "menuItemTransparent";
            this.menuItemTransparent.ShortcutKeyDisplayString = "Alt-Z";
            this.menuItemTransparent.Size = new System.Drawing.Size(278, 24);
            this.menuItemTransparent.Text = "Transparent mode";
            this.menuItemTransparent.Click += new System.EventHandler(this.buttonTransparent_Click);
            // 
            // showToolbarToolStripMenuItem
            // 
            this.showToolbarToolStripMenuItem.Name = "showToolbarToolStripMenuItem";
            this.showToolbarToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F10;
            this.showToolbarToolStripMenuItem.Size = new System.Drawing.Size(278, 24);
            this.showToolbarToolStripMenuItem.Text = "Show toolbar";
            this.showToolbarToolStripMenuItem.Click += new System.EventHandler(this.showToolbarToolStripMenuItem_Click);
            // 
            // menuItemClipboardMonitoring
            // 
            this.menuItemClipboardMonitoring.Name = "menuItemClipboardMonitoring";
            this.menuItemClipboardMonitoring.ShortcutKeyDisplayString = "C";
            this.menuItemClipboardMonitoring.Size = new System.Drawing.Size(278, 24);
            this.menuItemClipboardMonitoring.Text = "Clipboard monitoring";
            this.menuItemClipboardMonitoring.Click += new System.EventHandler(this.buttonClipboardMonitoring_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(275, 6);
            // 
            // menuitemOptions
            // 
            this.menuitemOptions.Name = "menuitemOptions";
            this.menuitemOptions.ShortcutKeyDisplayString = "O";
            this.menuitemOptions.Size = new System.Drawing.Size(278, 24);
            this.menuitemOptions.Text = "Options...";
            this.menuitemOptions.Click += new System.EventHandler(this.button1_Click);
            // 
            // menuitemMore
            // 
            this.menuitemMore.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuitemRun,
            this.menuitemClear,
            this.menuitemKataHira,
            this.menuitemClearCache,
            this.menuitemMonitor,
            this.reloadUserDictionaryToolStripMenuItem,
            this.showFormBordersToolStripMenuItem,
            this.addACustomDictionaryEntryToolStripMenuItem});
            this.menuitemMore.Name = "menuitemMore";
            this.menuitemMore.Size = new System.Drawing.Size(278, 24);
            this.menuitemMore.Text = "More";
            // 
            // menuitemRun
            // 
            this.menuitemRun.Name = "menuitemRun";
            this.menuitemRun.ShortcutKeyDisplayString = "G";
            this.menuitemRun.Size = new System.Drawing.Size(305, 24);
            this.menuitemRun.Text = "Run game to translate";
            this.menuitemRun.Click += new System.EventHandler(this.buttonRun_Click);
            // 
            // menuitemClear
            // 
            this.menuitemClear.Name = "menuitemClear";
            this.menuitemClear.ShortcutKeyDisplayString = "Delete";
            this.menuitemClear.Size = new System.Drawing.Size(305, 24);
            this.menuitemClear.Text = "Clear window";
            this.menuitemClear.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // menuitemKataHira
            // 
            this.menuitemKataHira.Name = "menuitemKataHira";
            this.menuitemKataHira.ShortcutKeyDisplayString = "K";
            this.menuitemKataHira.Size = new System.Drawing.Size(305, 24);
            this.menuitemKataHira.Text = "Katakana to hiragana";
            this.menuitemKataHira.Click += new System.EventHandler(this.buttonKataHira_Click);
            // 
            // menuitemClearCache
            // 
            this.menuitemClearCache.Name = "menuitemClearCache";
            this.menuitemClearCache.Size = new System.Drawing.Size(305, 24);
            this.menuitemClearCache.Text = "Clear cache";
            this.menuitemClearCache.Click += new System.EventHandler(this.buttonClearCache_Click);
            // 
            // menuitemMonitor
            // 
            this.menuitemMonitor.Name = "menuitemMonitor";
            this.menuitemMonitor.ShortcutKeyDisplayString = "M";
            this.menuitemMonitor.Size = new System.Drawing.Size(305, 24);
            this.menuitemMonitor.Text = "Select threads to monitor...";
            this.menuitemMonitor.Click += new System.EventHandler(this.buttonMonitor_Click);
            // 
            // reloadUserDictionaryToolStripMenuItem
            // 
            this.reloadUserDictionaryToolStripMenuItem.Name = "reloadUserDictionaryToolStripMenuItem";
            this.reloadUserDictionaryToolStripMenuItem.Size = new System.Drawing.Size(305, 24);
            this.reloadUserDictionaryToolStripMenuItem.Text = "Reload user dictionary";
            this.reloadUserDictionaryToolStripMenuItem.Click += new System.EventHandler(this.reloadUserDictionaryToolStripMenuItem_Click);
            // 
            // showFormBordersToolStripMenuItem
            // 
            this.showFormBordersToolStripMenuItem.Name = "showFormBordersToolStripMenuItem";
            this.showFormBordersToolStripMenuItem.ShortcutKeyDisplayString = "B";
            this.showFormBordersToolStripMenuItem.Size = new System.Drawing.Size(305, 24);
            this.showFormBordersToolStripMenuItem.Text = "Show form borders";
            this.showFormBordersToolStripMenuItem.Click += new System.EventHandler(this.showFormBordersToolStripMenuItem_Click);
            // 
            // addACustomDictionaryEntryToolStripMenuItem
            // 
            this.addACustomDictionaryEntryToolStripMenuItem.Name = "addACustomDictionaryEntryToolStripMenuItem";
            this.addACustomDictionaryEntryToolStripMenuItem.ShortcutKeyDisplayString = "U";
            this.addACustomDictionaryEntryToolStripMenuItem.Size = new System.Drawing.Size(305, 24);
            this.addACustomDictionaryEntryToolStripMenuItem.Text = "Add a custom dictionary entry...";
            this.addACustomDictionaryEntryToolStripMenuItem.Click += new System.EventHandler(this.buttonUserDict_Click);
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
            this.buttonMonitor,
            this.toolStripSeparator2,
            this.buttonClear,
            this.buttonTranslateSelected,
            this.buttonTranslateFull,
            this.buttonPaste,
            this.buttonClipboardMonitoring,
            this.buttonKataHira,
            this.buttonDictionary,
            this.toolStripSeparator4,
            this.buttonAddReplacement,
            this.buttonUserDict,
            this.buttonClearCache,
            this.toolStripSeparator1,
            this.buttonFullscreen,
            this.buttonTransparent,
            this.toolStripSeparator3,
            this.buttonUpdateEDICT,
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
            // buttonTranslateFull
            // 
            this.buttonTranslateFull.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonTranslateFull.Image = ((System.Drawing.Image)(resources.GetObject("buttonTranslateFull.Image")));
            this.buttonTranslateFull.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonTranslateFull.Name = "buttonTranslateFull";
            this.buttonTranslateFull.Size = new System.Drawing.Size(26, 26);
            this.buttonTranslateFull.Text = "Translate using all translators (A)";
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
            // buttonClipboardMonitoring
            // 
            this.buttonClipboardMonitoring.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonClipboardMonitoring.Image = ((System.Drawing.Image)(resources.GetObject("buttonClipboardMonitoring.Image")));
            this.buttonClipboardMonitoring.ImageTransparentColor = System.Drawing.Color.White;
            this.buttonClipboardMonitoring.Name = "buttonClipboardMonitoring";
            this.buttonClipboardMonitoring.Size = new System.Drawing.Size(26, 26);
            this.buttonClipboardMonitoring.Text = "Enable clipboard monitoring (C)";
            this.buttonClipboardMonitoring.Click += new System.EventHandler(this.buttonClipboardMonitoring_Click);
            // 
            // buttonKataHira
            // 
            this.buttonKataHira.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonKataHira.Image = ((System.Drawing.Image)(resources.GetObject("buttonKataHira.Image")));
            this.buttonKataHira.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonKataHira.Name = "buttonKataHira";
            this.buttonKataHira.Size = new System.Drawing.Size(26, 26);
            this.buttonKataHira.Text = "Convert all katakana to hiragana and translate (K)";
            this.buttonKataHira.Click += new System.EventHandler(this.buttonKataHira_Click);
            // 
            // buttonDictionary
            // 
            this.buttonDictionary.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonDictionary.Image = ((System.Drawing.Image)(resources.GetObject("buttonDictionary.Image")));
            this.buttonDictionary.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonDictionary.Name = "buttonDictionary";
            this.buttonDictionary.Size = new System.Drawing.Size(26, 26);
            this.buttonDictionary.Text = "Search selected text in dictionary (D)";
            this.buttonDictionary.Click += new System.EventHandler(this.buttonDictionary_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 29);
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
            // buttonUserDict
            // 
            this.buttonUserDict.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonUserDict.Image = ((System.Drawing.Image)(resources.GetObject("buttonUserDict.Image")));
            this.buttonUserDict.ImageTransparentColor = System.Drawing.Color.FromArgb(((int)(((byte)(163)))), ((int)(((byte)(73)))), ((int)(((byte)(164)))));
            this.buttonUserDict.Name = "buttonUserDict";
            this.buttonUserDict.Size = new System.Drawing.Size(26, 26);
            this.buttonUserDict.Text = "Add a custom dictionary entry (U)";
            this.buttonUserDict.Click += new System.EventHandler(this.buttonUserDict_Click);
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
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 29);
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
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 29);
            // 
            // buttonUpdateEDICT
            // 
            this.buttonUpdateEDICT.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonUpdateEDICT.Image = ((System.Drawing.Image)(resources.GetObject("buttonUpdateEDICT.Image")));
            this.buttonUpdateEDICT.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonUpdateEDICT.Name = "buttonUpdateEDICT";
            this.buttonUpdateEDICT.Size = new System.Drawing.Size(26, 26);
            this.buttonUpdateEDICT.Text = "Update EDICT dictionary file";
            this.buttonUpdateEDICT.Click += new System.EventHandler(this.buttonUpdateEDICT_Click);
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
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.LocationChanged += new System.EventHandler(this.Form1_LocationChanged);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.contextMenu.ResumeLayout(false);
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
        private System.Windows.Forms.ToolStripButton buttonKataHira;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem menuitemTranslate;
        private System.Windows.Forms.ToolStripMenuItem menuitemTranslateAll;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem menuitemCopy;
        private System.Windows.Forms.ToolStripMenuItem menuitemPaste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem menuitemNewReplacement;
        private System.Windows.Forms.ToolStripMenuItem menuItemFullscreen;
        private System.Windows.Forms.ToolStripMenuItem menuItemTransparent;
        private System.Windows.Forms.ToolStripMenuItem menuitemMore;
        private System.Windows.Forms.ToolStripMenuItem menuitemRun;
        private System.Windows.Forms.ToolStripMenuItem menuitemClear;
        private System.Windows.Forms.ToolStripMenuItem menuitemKataHira;
        private System.Windows.Forms.ToolStripMenuItem menuitemClearCache;
        private System.Windows.Forms.ToolStripMenuItem menuitemMonitor;
        private System.Windows.Forms.ToolStripMenuItem menuitemOnOff;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem menuitemOptions;
        private System.Windows.Forms.ToolStripMenuItem reloadUserDictionaryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showToolbarToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton buttonDictionary;
        private System.Windows.Forms.ToolStripMenuItem searchInDictionaryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showFormBordersToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton buttonUpdateEDICT;
        private System.Windows.Forms.ToolStripMenuItem addACustomDictionaryEntryToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton buttonUserDict;
        private System.Windows.Forms.ToolStripButton buttonClipboardMonitoring;
        private System.Windows.Forms.ToolStripMenuItem menuItemClipboardMonitoring;
    }
}

