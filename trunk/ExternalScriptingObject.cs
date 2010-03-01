using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

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
    }
}
