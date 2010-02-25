using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Globalization;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ChiiTrans
{
    public partial class Form1 : Form
    {
        public static Form1 thisForm;
        private FormOptions formOptions;
        private FormAddReplacement formAddReplacement;

        public Form1()
        {
            InitializeComponent();
            thisForm = this;
            BackColor = Color.FromArgb(0, 0, 1);
            UpdateButtonOnOffDelegate = new UpdateButtonOnOffType(UpdateButtonOnOff);
            Global.Init();
            Global.runScript = RunScript;
            WindowPosition.Deserialize(this, Global.windowPosition.MainFormPosition);
            buttonOnOff.Image = imageList1.Images["loading"];
            browser.Top = 28;
            browser.Height = ClientSize.Height - browser.Top;
            browser.ObjectForScripting = Global.script;
            browser.Url = new Uri("file://" + Path.Combine(Application.StartupPath, "html\\base.html"));
            ApplyOptions();
            Global.agth.LoadAppProfiles();
            UseCommandLineArgs();
            if (Global.agth.TryConnectPipe())
                turnOn();
            else
                turnOff();
            buttonOnOff.Enabled = true;
        }

        private void UseCommandLineArgs()
        {
            string[] args = Environment.GetCommandLineArgs();
            string keys = "";
            string app = null;
            foreach (string arg in args)
            {
                if (arg == args[0])
                    continue;
                if (arg.Length > 0)
                {
                    if (arg[0] == '/')
                    {
                        keys += arg + ' ';
                    }
                    else
                    {
                        if (app == null)
                            app = arg;
                    }
                }
            }
            if (app != null)
            {
                try
                {
                    Global.RunGame(app, keys);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static void Debug(string s)
        {
            if (thisForm.InvokeRequired)
            {
                ShowDebugDelegate tmp = new ShowDebugDelegate(thisForm.ShowDebug);
                thisForm.Invoke(tmp, s);
            }
            else
            {
                thisForm.ShowDebug(s);
            }
        }

        private delegate void ShowDebugDelegate(string s);
        private void ShowDebug(string s)
        {
            textBoxDebug.Visible = true;
            textBoxDebug.Text = s;
        }

        private void ApplyOptions()
        {
            this.TopMost = Global.isTopMost();
        }

        private void toggleOnOff()
        {
            if (Global.agth.is_on)
            {
                turnOff();
            }
            else
            {
                turnOn();
            }
        }

        private void turnOn()
        {
            if (!Global.agth.TurnOn())
                MessageBox.Show("AGTH or another instance of this program is running.\r\nClose AGTH and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void turnOff()
        {
            Global.agth.TurnOff();
        }

        private delegate void UpdateButtonOnOffType(bool isOn);
        private UpdateButtonOnOffType UpdateButtonOnOffDelegate;
        public void UpdateButtonOnOff(bool isOn)
        {
            if (InvokeRequired)
            {
                Invoke(UpdateButtonOnOffDelegate, isOn);
            }
            else
            {
                if (isOn)
                {
                    buttonOnOff.Image = imageList1.Images["on"];
                    buttonOnOff.Text = "Turn off (T)";
                }
                else
                {
                    buttonOnOff.Image = imageList1.Images["off"];
                    buttonOnOff.Text = "Turn on (T)";
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            toggleOnOff();
        }

        public delegate object RunScriptDelegate(string name, object[] args);

        public object RunScript(string name, object[] args)
        {
            try
            {
                if (InvokeRequired)
                {
                    RunScriptDelegate tmp = new RunScriptDelegate(RunScript);
                    return Invoke(tmp, name, args);
                }
                else
                {
                    return browser.Document.InvokeScript(name, args);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (formOptions == null)
                formOptions = new FormOptions();
            this.TopMost = false;
            if (formOptions.ShowDialog() == DialogResult.OK)
            {
                formOptions.SaveOptions();
                this.ApplyOptions();
                Global.script.UpdateBrowserSettings();
            }
            else
            {
                this.TopMost = Global.isTopMost();
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Global.RunScript("ClearContent");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Translation.Translate(Translation.lastGoodBuffer, null);
        }

        private void textBoxDebug_Click(object sender, EventArgs e)
        {
            textBoxDebug.SelectAll();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Global.fullscreen)
                Global.FullscreenOff();
            Global.windowPosition.MainFormPosition = WindowPosition.Serialize(this);
            if (FormMonitor.isCreated())
            {
                Global.windowPosition.MonitorFormPosition = WindowPosition.Serialize(FormMonitor.instance);
            }
            if (formOptions != null)
            {
                Global.windowPosition.OptionsFormPosition = WindowPosition.Serialize(formOptions);
            }
            Global.windowPosition.Save();
            Hide();
            if (FormMonitor.isCreated())
            {
                FormMonitor.instance.Hide();
            }
            if (Global.script.transparentMode)
            {
                FormBottomLayer.instance.Hide();
            }
            if (Global.options.useCache)
                Global.cache.Save();
            Global.options.SaveOptions();
            Global.agth.Disconnect();
            Global.agth.SaveAppProfiles();
            if (Atlas.status == AtlasInitStatus.SUCCESS)
            {
                Atlas.Deinitialize();
            }
            if (Mecab.status == MecabInitStatus.SUCCESS)
            {
                Mecab.Deinitialize();
            }
            /*foreach (Translation trans in Translation.current)
            {
                trans.Abort();
            }*/
        }

        private string GetSelectedText()
        {
            object result = Global.RunScript("GetSelectedText");
            string selText = "";
            try
            {
                selText = (string)result;
                if (selText == null)
                    selText = "";
            }
            catch (Exception)
            { }
            return selText;
        }
        
        private void buttonAddReplacement_Click(object sender, EventArgs e)
        {
            string selText = GetSelectedText();
            if (formAddReplacement == null)
                formAddReplacement = new FormAddReplacement();
            string oldNewText = "";
            foreach (Replacement rep in Global.options.replacements)
            {
                if (rep.oldText == selText)
                {
                    oldNewText = rep.newText;
                    break;
                }
            }
            formAddReplacement.UpdateControls(selText, oldNewText);
            TopMost = false;
            if (formAddReplacement.ShowDialog() == DialogResult.OK)
            {
                string oldText, newText;
                formAddReplacement.GetControlValues(out oldText, out newText);
                Global.options.replacements.Add(new Replacement(oldText, newText));
                Global.options.SaveReplacements();
            }
            TopMost = Global.isTopMost();
        }

        bool duplicateWorkaround = false;
        Keys oldKey;
        private void browser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == oldKey && duplicateWorkaround)
            {
                duplicateWorkaround = false;
            }
            else
            {
                oldKey = e.KeyCode;
                if (!e.Control && !e.Alt)
                {
                    if (e.KeyCode == Keys.Insert)
                        buttonAddReplacement_Click(sender, null);
                    else if (e.KeyCode == Keys.T)
                        button2_Click(sender, null);
                    else if (e.KeyCode == Keys.R)
                        button3_Click(sender, null);
                    else if (e.KeyCode == Keys.Delete)
                        button2_Click_1(sender, null);
                    else if (e.KeyCode == Keys.Escape)
                        TransparentModeOff();
                    else if (e.KeyCode == Keys.Space)
                        buttonTranslateSelected_Click(sender, null);
                    else if (e.KeyCode == Keys.Add && Global.script.transparentMode)
                        ChangeBottomLayerOpacity(1);
                    else if (e.KeyCode == Keys.Subtract && Global.script.transparentMode)
                        ChangeBottomLayerOpacity(-1);
                    else if (e.KeyCode == Keys.M)
                        buttonMonitor_Click(sender, null);
                    else if (e.KeyCode == Keys.G)
                        buttonRun_Click(sender, null);
                    else if (e.KeyCode == Keys.O)
                        button1_Click(sender, null);
                }
                else if (e.Control && e.KeyCode == Keys.V || e.Shift && e.KeyCode == Keys.Insert)
                {
                    TranslateFromClipboard();
                }
                duplicateWorkaround = true;
            }

        }

        private void buttonTransparent_Click(object sender, EventArgs e)
        {
            if (Global.script.transparentMode)
                TransparentModeOff();
            else
                TransparentModeOn();
        }

        string oldFormText;
        int oldY;
        private void TransparentModeOn()
        {
            toolStrip1.Hide();
            oldY = browser.Top;
            browser.Top = 0;
            browser.Height += oldY;
            TransparencyKey = Color.FromArgb(0, 0, 1);
            oldFormText = Text;
            Text = "Press Escape to restore the window";
            Global.RunScript("TransparentModeOn");
            ChangeBottomLayerOpacity(0);
            FormBottomLayer.instance.UpdatePos();
            FormBottomLayer.instance.BackColor = Global.options.colors["back_tmode"].color;
            FormBottomLayer.instance.Show();
            TopMost = true;
            Global.script.transparentMode = true;
        }

        private void TransparentModeOff()
        {
            if (Global.script.transparentMode)
            {
                Global.script.transparentMode = false;
                Global.RunScript("TransparentModeOff");
                Text = oldFormText;
                FormBorderStyle = FormBorderStyle.Sizable;
                TransparencyKey = Color.Empty;
                browser.Height -= oldY;
                browser.Top = oldY;
                toolStrip1.Show();
                FormBottomLayer.instance.Hide();
                TopMost = Global.isTopMost();
            }
        }

        private void buttonMonitor_Click(object sender, EventArgs e)
        {
            if (FormMonitor.instance.Visible)
            {
                FormMonitor.instance.Hide();
            }
            else
            {
                FormMonitor.instance.Show();
            }
        }

        private void buttonTranslateSelected_Click(object sender, EventArgs e)
        {
            string sel = GetSelectedText();
            if (sel != "")
                Translation.Translate(sel, null);
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            TopMost = false;
            if (Global.agth.appProfiles["profiles"].dict.Count == 0)
            {
                if (openFileDialogRun.ShowDialog() == DialogResult.OK)
                {
                    FormRun.instance.SetExeName(openFileDialogRun.FileName);
                    if (FormRun.instance.ShowDialog() == DialogResult.OK)
                    {
                        FormRun.instance.Run();
                    }
                }
            }
            else
            {
                if (FormRun.instance.ShowDialog() == DialogResult.OK)
                {
                    FormRun.instance.Run();
                }
            }
            TopMost = Global.isTopMost();
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] data = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (data.Length > 0)
                {
                    FormRun.instance.SetExeName(data[0]);
                    if (FormRun.instance.ShowDialog() == DialogResult.OK)
                    {
                        FormRun.instance.Run();
                    }
                }
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Form1_LocationChanged(object sender, EventArgs e)
        {
            if (Global.script != null && Global.script.transparentMode)
            {
                if (!suspendBottomLayerUpdates)
                    FormBottomLayer.instance.UpdatePos();
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (Global.script != null && Global.script.transparentMode)
            {
                browser.Size = ClientSize;
                if (!suspendBottomLayerUpdates)
                    FormBottomLayer.instance.UpdatePos();
            }
        }

        private void ChangeBottomLayerOpacity(int delta)
        {
            int value = Global.options.bottomLayerOpacity;
            value += delta * 10;
            if (value < 0) value = 0;
            if (value > 100) value = 100;
            Global.options.bottomLayerOpacity = value;
            FormBottomLayer.instance.Opacity = (double)value / 100;
        }

        bool suspendBottomLayerUpdates = false;
        private void Form1_Activated(object sender, EventArgs e)
        {
            if (!suspendBottomLayerUpdates && Global.script.transparentMode)
            {
                suspendBottomLayerUpdates = true;
                //SuspendLayout();
                //browser.Hide();
                int oldWidth = Width;
                int oldHeight = Height;
                FormBorderStyle = FormBorderStyle.Sizable;
                int border = (Width - oldWidth) / 2;
                Left -= border;
                Top -= (Height - oldHeight - border);
                //browser.Show();
                //ResumeLayout(false);
                TopMost = true;
                FormBottomLayer.instance.UpdatePos();
                suspendBottomLayerUpdates = false;
            }
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            if (!suspendBottomLayerUpdates && Global.script.transparentMode)
            {
                suspendBottomLayerUpdates = true;
                //SuspendLayout();
                //browser.Hide();
                int oldWidth = Width;
                int oldHeight = Height;
                FormBorderStyle = FormBorderStyle.None;
                int border = (oldWidth - Width) / 2;
                Left += border;
                Top += (oldHeight - Height - border);
                //browser.Show();
                //ResumeLayout(false);
                TopMost = true;
                FormBottomLayer.instance.UpdatePos();
                suspendBottomLayerUpdates = false;
                /*if (Global.fullscreen && Global.gameWindow != IntPtr.Zero)
                {
                    PInvokeFunc.SetWindowPos(Global.gameWindow, FormBottomLayer.instance.Handle, 0, 0, 0, 0, 19);
                }*/
                //TopMost = true;
                //FormBottomLayer.instance.UpdatePos();
            }
        }

        private void buttonFullscreen_Click(object sender, EventArgs e)
        {
            Global.ToggleFullscreen();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            const int WM_KEYDOWN = 0x100;
            const int WM_SYSKEYDOWN = 0x104;

            if (msg.Msg == WM_KEYDOWN || msg.Msg == WM_SYSKEYDOWN)
            {
                if (keyData == (Keys.Alt | Keys.F))
                {
                    buttonFullscreen_Click(this, null);
                    return true;
                }
                else if (keyData == (Keys.Alt | Keys.Z))
                {
                    buttonTransparent_Click(this, null);
                    return true;
                }
                else if (keyData == (Keys.Alt | Keys.R))
                {
                    buttonTranslateFull_Click(this, null);
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void TranslateFromClipboard()
        {
            if (Clipboard.ContainsText(TextDataFormat.UnicodeText))
            {
                string buffer = Clipboard.GetText();
                Translation.Translate(buffer, null);
            }
        }

        private void buttonPaste_Click(object sender, EventArgs e)
        {
            TranslateFromClipboard();
        }

        private void toolStripHomePage_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Go to ChiiTrans' home page?\r\nhttp://sites.google.com/site/chiitranslator/", "ChiiTrans v." + Application.ProductVersion, MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                Process.Start("http://sites.google.com/site/chiitranslator/");
            }
        }

        private void buttonClearCache_Click(object sender, EventArgs e)
        {
            Global.cache.Clear();
            Global.cache.Save();
        }

        private void buttonTranslateFull_Click(object sender, EventArgs e)
        {
            Options options = Global.options.Clone();
            options.displayOriginal = true;
            foreach (TranslatorRecord rec in options.translators)
            {
                rec.inUse = true;
            }
            Translation.Translate(Translation.lastGoodBuffer, options);
        }

    }
}
