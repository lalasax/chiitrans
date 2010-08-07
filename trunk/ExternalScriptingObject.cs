using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Web;

namespace ChiiTrans
{
    [ComVisible(true)]
    public class ExternalScriptingObject
    {
        public bool transparentMode = false;

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
        }

        public void HivemindClick(string block_id, string block_name, string id, string src)
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

        public void ShowTooltip(string title, string text)
        {
            FormTooltip.instance.ShowTooltip(title, text);
        }

        public void HideTooltip()
        {
            FormTooltip.instance.Hide();
        }
    }
}
