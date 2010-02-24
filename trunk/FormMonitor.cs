using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ChiiTrans
{
    public partial class FormMonitor : Form
    {
        private static FormMonitor _instance;
        public static FormMonitor instance {
            get
            {
                if (_instance == null)
                {
                    _instance = new FormMonitor();
                }
                return _instance;
            }
        }
        public static bool isCreated()
        {
            return _instance != null;
        }

        private string oldCurrentThread;
        
        public FormMonitor()
        {
            InitializeComponent();
            
            UpdateThreadListDelegate = new UpdateThreadListType(UpdateThreadList);
            AddThreadBlockDelegate = new AddThreadBlockType(AddThreadBlock);

            WindowPosition.Deserialize(this, Global.windowPosition.MonitorFormPosition);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private static string GetSortKey(string s)
        {
            string[] ss = s.Split(new char[] {' '}, 2);
            if (ss.Length == 1)
                return ss[0];
            else
                return ss[1] + ' ' + ss[0];
        }
        
        private delegate void UpdateThreadListType();
        private UpdateThreadListType UpdateThreadListDelegate;
        public void UpdateThreadList()
        {
            if (InvokeRequired)
            {
                Invoke(UpdateThreadListDelegate);
            }
            else
            {
                listBoxThreads.Items.Clear();
                foreach (KeyValuePair<string, BlockList> pair in Global.agth.blocks.OrderBy(a => GetSortKey(a.Key)))
                {
                    int id = listBoxThreads.Items.Add(pair.Key, pair.Value.IsMonitored);
                    if (pair.Key == oldCurrentThread)
                        listBoxThreads.SelectedIndex = id;
                }
            }
        }

        private delegate void AddThreadBlockType(string threadName, string text);
        private AddThreadBlockType AddThreadBlockDelegate;
        public void AddThreadBlock(string threadName, string text)
        {
            if (oldCurrentThread == threadName)
            {
                if (InvokeRequired)
                {
                    Invoke(AddThreadBlockDelegate, threadName, text);
                }
                else
                {
                    textBoxContents.AppendText(text + Environment.NewLine + Environment.NewLine);
                }
            }
        }

        private void listBoxThreads_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            string name = listBoxThreads.Items[e.Index].ToString();
            if (Global.agth.blocks.ContainsKey(name))
            {
                Global.agth.manualMonitoring = true;
                Global.agth.blocks[name].IsMonitored = e.NewValue == CheckState.Checked;
            }
        }

        private void listBoxThreads_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxThreads.SelectedItem.ToString() != oldCurrentThread)
            {
                oldCurrentThread = listBoxThreads.SelectedItem.ToString();
                UpdateThreadContents();
            }
        }

        private void UpdateThreadContents()
        {
            string name = listBoxThreads.SelectedItem.ToString();
            if (Global.agth.blocks.ContainsKey(name))
            {
                textBoxContents.Text = string.Join(Environment.NewLine + Environment.NewLine, Global.agth.blocks[name].ToArray());
                textBoxContents.AppendText(Environment.NewLine + Environment.NewLine);
            }
            else
            {
                textBoxContents.Text = "";
            }
        }

        private void FormMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void FormMonitor_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible == true)
            {
                if (Global.agth.isConnected)
                {
                    UpdateThreadList();
                    if (listBoxThreads.SelectedIndex >= 0)
                        UpdateThreadContents();
                }
            }
        }

        private void FormMonitor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape || e.KeyCode == Keys.M)
                Hide();
        }
    }
}
