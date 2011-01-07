using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace ChiiTrans
{
    public partial class FormUpdate : Form
    {
        public static FormUpdate instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FormUpdate();
                return _instance;
            }
        }
        private static FormUpdate _instance;
        
        public FormUpdate()
        {
            InitializeComponent();
        }

        delegate void UpdLabel();

        private void CheckSiteVersion()
        {
            string result;
            try
            {
                WebClient webClient = new WebClient();
                webClient.Proxy = null;
                string s = webClient.DownloadString("http://ftp.monash.edu.au/pub/nihongo/edicthdr.txt");
                var ss = s.Split('/');
                result = ss[ss.Length - 2].Trim() + ", " + ss[ss.Length - 1].Trim();
            }
            catch (Exception)
            {
                result = "(update error)";
            }
            if (InvokeRequired)
            {
                Invoke(new UpdLabel(delegate() { labelRemote.Text = string.Format("Most recent EDICT version: {0}.", result); }));
            }
        }

        private void FormUpdate_Shown(object sender, EventArgs e)
        {
            string localVersion;
            if (!Edict.instance.Ready)
                localVersion = "(not available)";
            else
                localVersion = Edict.instance.VersionInfo();
            labelLocal.Text = string.Format("Local EDICT file: {0}.", localVersion);
            var checkThread = new Thread(new ThreadStart(CheckSiteVersion));
            checkThread.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var updateThread = new Thread(new ThreadStart(UpdateEDICT));
            updateThread.Start();
        }

        private void UpdateEDICT()
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://ftp.monash.edu.au/pub/nihongo/edict.gz");
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                Stream responseStream = response.GetResponseStream();
                var gzip = new GZipStream(responseStream, CompressionMode.Decompress);
                var fs = new FileStream(Edict.instance.GetRealFilename("edict.new"), FileMode.Create, FileAccess.Write);
                CopyStream(gzip, fs);
                fs.Close();
                File.Replace(Edict.instance.GetRealFilename("edict.new"), Edict.instance.GetRealFilename("edict"), Edict.instance.GetRealFilename("edict.bak"), true);
                Edict.instance.Initialize();
                MessageBox.Show("The EDICT file was updated successfully.", "Update EDICT", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                try
                {
                    File.Delete(Edict.instance.GetRealFilename("edict.new"));
                }
                catch (Exception) { }
                MessageBox.Show("The EDICT file was not updated.\r\n" + ex.Message, "Update EDICT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[500000];
            while (true)
            {
                int read = input.Read(buffer, 0, buffer.Length);
                if (read <= 0) return;
                output.Write(buffer, 0, read);
            }
        }
    }
}
