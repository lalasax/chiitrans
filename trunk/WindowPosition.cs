using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace ChiiTrans
{
    class WindowPosition
    {
        public string MainFormPosition;
        public string FullscreenPosition;
        public string MonitorFormPosition;
        public string OptionsFormPosition;

        private string cfgFile;

        public WindowPosition()
        {
            cfgFile = Path.Combine(Global.cfgdir, "window.txt");
        }

        public void Load()
        {
            MainFormPosition = null;
            FullscreenPosition = null;
            MonitorFormPosition = null;
            OptionsFormPosition = null;
            try
            {
                JsObject data = Json.Parse(File.ReadAllText(cfgFile));
                MainFormPosition = data.str["main"];
                FullscreenPosition = data.str["fullscreen"];
                MonitorFormPosition = data.str["monitor"];
                OptionsFormPosition = data.str["options"];
            }
            catch(Exception)
            {
            }
        }

        public void Save()
        {
            try
            {
                JsObject data = new JsObject();
                data.str["main"] = MainFormPosition;
                data.str["fullscreen"] = FullscreenPosition;
                data.str["monitor"] = MonitorFormPosition;
                data.str["options"] = OptionsFormPosition;
                File.WriteAllText(cfgFile, data.Serialize());
            }
            catch (Exception)
            { }
        }

        public static string Serialize(Form form)
        {
            if (form.WindowState == FormWindowState.Normal)
            {
                return form.Left.ToString() + "|" +
                    form.Top.ToString() + "|" +
                    form.Width.ToString() + "|" +
                    form.Height.ToString() + "|" +
                    form.WindowState.ToString();
            }
            else
            {
                return form.RestoreBounds.Left.ToString() + "|" +
                    form.RestoreBounds.Top.ToString() + "|" +
                    form.RestoreBounds.Width.ToString() + "|" +
                    form.RestoreBounds.Height.ToString() + "|" +
                    form.WindowState.ToString();
            }
        }

        public static void Deserialize(Form formIn, string thisWindowGeometry)
        {
            Deserialize(formIn, thisWindowGeometry, false);
        }

        public static void Deserialize(Form formIn, string thisWindowGeometry, bool force)
        {
            try
            {
                if (string.IsNullOrEmpty(thisWindowGeometry) == true)
                {
                    return;
                }
                string[] numbers = thisWindowGeometry.Split('|');
                string windowString = numbers[4];

                Point windowPoint = new Point(int.Parse(numbers[0]),
                    int.Parse(numbers[1]));
                Size windowSize = new Size(int.Parse(numbers[2]),
                    int.Parse(numbers[3]));

                bool locOkay = GeometryIsBizarreLocation(windowPoint, windowSize);
                bool sizeOkay = GeometryIsBizarreSize(windowSize);

                if (force || (locOkay == true && sizeOkay == true))
                {
                    formIn.Location = windowPoint;
                    formIn.Size = windowSize;
                    formIn.StartPosition = FormStartPosition.Manual;
                }
                else if (sizeOkay == true)
                {
                    formIn.Size = windowSize;
                }

                if (windowString == "Maximized")
                    formIn.WindowState = FormWindowState.Maximized;
                else
                    formIn.WindowState = FormWindowState.Normal;
            }
            catch (Exception)
            {
            }
        }

        private static bool GeometryIsBizarreLocation(Point loc, Size size)
        {
            bool locOkay;
            if (loc.X < 0 || loc.Y < 0)
            {
                locOkay = false;
            }
            else if (loc.X + size.Width > Screen.PrimaryScreen.WorkingArea.Width)
            {
                locOkay = false;
            }
            else if (loc.Y + size.Height > Screen.PrimaryScreen.WorkingArea.Height)
            {
                locOkay = false;
            }
            else
            {
                locOkay = true;
            }
            return locOkay;
        }

        private static bool GeometryIsBizarreSize(Size size)
        {
            return (size.Height <= Screen.PrimaryScreen.WorkingArea.Height &&
                size.Width <= Screen.PrimaryScreen.WorkingArea.Width);
        }

        public static void Normalize(Form form)
        {
            int w = Screen.PrimaryScreen.WorkingArea.Width;
            int h = Screen.PrimaryScreen.WorkingArea.Height;
            if (form.Width > w)
                form.Width = w;
            if (form.Height > h)
                form.Height = h;
            if (form.Left < 0)
                form.Left = 0;
            if (form.Top < 0)
                form.Top = 0;
            if (form.Left + form.Width > w)
                form.Left = w - form.Width;
            if (form.Top + form.Height > h)
                form.Top = h - form.Height;
        }
    }
}
