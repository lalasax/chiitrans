using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

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
                Global.options.largeMargins
            );
            List<object> obj = new List<object>();
            foreach (KeyValuePair<string, ColorRecord> kvp in Global.options.colors)
            {
                obj.Add(kvp.Key);
                string clr = string.Format("#{0:X6}", kvp.Value.color.ToArgb() & 0xFFFFFF);
                /*while (clr.Length < 6)
                    clr = "0" + clr;
                obj.Add("#" + clr);*/
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

        public void HivemindClick(string id, string src)
        {
            string url;
            if (string.IsNullOrEmpty(id) || id == "0")
            {
                url = new Uri(new Uri(Global.options.hivemindServer), "index.php?q=" + Translation.UrlEncode(src)).ToString();
            }
            else
            {
                url = new Uri(new Uri(Global.options.hivemindServer), "index.php?p=view&id=" + id).ToString();
            }
            Process.Start(url);
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
