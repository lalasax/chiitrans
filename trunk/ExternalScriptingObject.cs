using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Web;
using System.Threading;

namespace ChiiTrans
{
    [ComVisible(true)]
    public class ExternalScriptingObject
    {
        public bool transparentMode = false;
        private static bool welcomed = false;

        public void UpdateBrowserSettings()
        {
            Global.RunScript("ApplySettings",
                Global.options.font.FontFamily.Name,
                Global.options.font.SizeInPoints,
                Global.options.appendBottom,
                Global.options.dropShadow,
                Global.options.maxBlocks,
                Global.options.largeMargins,
                Global.options.marginSize
            );
            List<object> obj = new List<object>();
            foreach (KeyValuePair<string, ColorRecord> kvp in Global.options.colors)
            {
                obj.Add(kvp.Key);
                string clr = string.Format("#{0:X6}", kvp.Value.color.ToArgb() & 0xFFFFFF);
                obj.Add(clr);
            }
            Global.RunScript2("ApplyColors", obj.ToArray());
            if (transparentMode)
            {
                Global.RunScript("TransparentModeOn");
                FormBottomLayer.instance.BackColor = Global.options.colors["back_tmode"].color;
            }
            else
            {
                Global.RunScript("TransparentModeOff");
            }
            if (!welcomed)
            {
                welcomed = true;
                string appversion = System.Windows.Forms.Application.ProductVersion;
                appversion = string.Join(".", appversion.Split('.').Take(2).ToArray());
                Global.RunScript("Welcome", "ChiiTrans v." + appversion);
            }
        }

        public void HivemindFastSubmit(string block_id, string block_name, string id, string src, string trans)
        {
            string url;
            url = Global.options.hivemindServer;
            url += (url.EndsWith("/") ? "" : "/");
            StringBuilder q = new StringBuilder();
            if (string.IsNullOrEmpty(id) || id == "0")
            {
                q.Append("src=" + Translation.UrlEncode(src));
            }
            else
            {
                q.Append("src_id=" + Translation.UrlEncode(id));
            }
            q.Append("&trans=" + Translation.UrlEncode(trans));
            HivemindSubmit.RequestData req = new HivemindSubmit.RequestData();
            req.query = q.ToString();
            req.block_id = block_id;
            req.block_name = block_name;
            req.translation = trans;
            Thread reqThread = new Thread(HivemindSubmit.instance.reqThreadProc);
            reqThread.IsBackground = true;
            reqThread.Start(req);
        }

        public void HivemindEdit(string block_id, string block_name, string id, string src)
        {
            string url;
            url = Global.options.hivemindServer;
            url += (url.EndsWith("/") ? "" : "/");
            if (string.IsNullOrEmpty(id) || id == "0")
            {
                //url += "index.php?q=" + Translation.UrlEncode(src);
                HivemindSubmit.instance.UpdateData(block_id, block_name, src);
                HivemindSubmit.instance.Show();
                HivemindSubmit.instance.Activate();
            }
            else
            {
                url += "index.php?p=view&id=" + id;
                Process.Start(url);
            }
        }

        public void LockHotkeys()
        {
            Global.lockHotkeys = true;
        }

        public void UnlockHotkeys()
        {
            Global.lockHotkeys = false;
        }

        public void ShowTooltip(string key, string reading, string text)
        {
            FormTooltip.instance.ShowTooltip(key, reading, text);
        }

        public void TooltipPage(int delta)
        {
            FormTooltip.instance.TooltipPage(delta);
        }

        public void HideTooltip()
        {
            FormTooltip.instance.Hide();
        }

        public void OnStartClick()
        {
            Form1.thisForm.buttonRun_Click(null, null);
        }
    }
}
