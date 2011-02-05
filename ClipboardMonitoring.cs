using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ChiiTrans
{
    class ClipboardMonitoring
    {
        public static bool Enabled
        {
            get
            {
                return Global.options.clipboardMonitoring;
            }
            set
            {
                Global.options.clipboardMonitoring = value;
                UpdateEnabled();
            }
        }

        private static bool _enabled = false;
        private static string oldText = null;

        public static void UpdateEnabled()
        {
            if (Enabled && !_enabled)
                Application.Idle += MonitorProc;
            else if (!Enabled && _enabled)
                Application.Idle -= MonitorProc;
            _enabled = Enabled;
            Form1.thisForm.UpdateClipboardMonitoringButton();
        }

        private static void MonitorProc(object sender, EventArgs e)
        {
            if (!Enabled)
            {
                return;
            }
            try
            {
                if (Clipboard.ContainsText())
                {
                    string clip = Clipboard.GetText();
                    int maxLength = Global.options.maxSourceLength * 2;
                    if (clip.Length > maxLength)
                        clip = clip.Substring(0, maxLength);
                    if (clip != oldText)
                    {
                        if (!Global.options.clipboardMonitoringJapanese || Translation.hasJapanese(clip))
                        {
                            Translation.Translate(clip, null, true);
                        }
                        oldText = clip;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in ClipboardMonitoring: " + ex.Message);
                Application.Idle -= MonitorProc;
                _enabled = false;
            }
        }

    }
}
