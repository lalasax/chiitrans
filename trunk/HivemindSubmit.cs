using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Threading;
using System.Net;

namespace ChiiTrans
{
    public partial class HivemindSubmit : Form
    {
        private static HivemindSubmit _instance;
        public static HivemindSubmit instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new HivemindSubmit();
                }
                return _instance;
            }
        }
        public static bool isCreated()
        {
            return _instance != null;
        }

        private string block_id;
        private string block_name;
        
        public HivemindSubmit()
        {
            InitializeComponent();

            WindowPosition.Deserialize(this, Global.windowPosition.HivemindSubmitFormPosition);
        }

        private void HivemindSubmit_Shown(object sender, EventArgs e)
        {
            textBoxTranslation.Focus();
        }

        public void UpdateData(string block_id, string block_name, string src)
        {
            this.block_id = block_id;
            this.block_name = block_name;

            textBoxSource.Text = src;
            textBoxTranslation.Text = "";
            textBoxComment.Text = "";
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (textBoxSource.Text == "")
            {
                MessageBox.Show("Source text must not be empty!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (textBoxTranslation.Text == "")
            {
                MessageBox.Show("Enter translation text!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            StringBuilder q = new StringBuilder();
            q.Append("src=" + Translation.UrlEncode(textBoxSource.Text));
            if (textBoxUser.Text != "")
            {
                q.Append("&user=" + Translation.UrlEncode(textBoxUser.Text));
                MD5 md5 = MD5.Create();
                byte[] pass_bytes = Encoding.UTF8.GetBytes(textBoxPassword.Text);
                byte[] hash = md5.ComputeHash(pass_bytes);
                StringBuilder hash_str = new StringBuilder();
                foreach (byte b in hash)
                {
                    hash_str.Append(b.ToString("x2"));
                }
                q.Append("&hash=" + hash_str.ToString());
            }
            q.Append("&trans=" + Translation.UrlEncode(textBoxTranslation.Text));
            q.Append("&comment=" + Translation.UrlEncode(textBoxComment.Text));
            RequestData req = new RequestData();
            req.query = q.ToString();
            req.block_id = block_id;
            req.block_name = block_name;
            req.translation = textBoxTranslation.Text;
            Thread reqThread = new Thread(reqThreadProc);
            reqThread.IsBackground = true;
            reqThread.Start(req);
            Hide();
        }

        class RequestData
        {
            public string query;
            public string block_id;
            public string block_name;
            public string translation;
        }

        void reqThreadProc(Object data)
        {
            RequestData reqdata = (RequestData)data;
            try
            {
                Global.RunScript("UpdateHivemind", reqdata.block_id, reqdata.block_name, "...", 0);
                string url = Global.options.hivemindServer;
                if (!url.EndsWith("/"))
                    url += "/";
                HttpWebRequest req = Translation.CreateHTTPRequest(url + "submit.php");
                Translation.WritePost(req, reqdata.query);
                string res = Translation.ReadAnswer(req);
                if (res.StartsWith("OK"))
                {
                    string id = res.Split(',')[1];
                    Global.RunScript("UpdateHivemind", reqdata.block_id, reqdata.block_name, id + "," + reqdata.translation, 1);
                }
                else
                {
                    Global.RunScript("UpdateHivemind", reqdata.block_id, reqdata.block_name, res, -1);
                }
            }
            catch (Exception ex)
            {
                Global.RunScript("UpdateHivemind", reqdata.block_id, reqdata.block_name, ex.Message, -1);
            }
        }

        private void HivemindSubmit_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void HivemindSubmit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Hide();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
