using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace ChiiTrans
{
    public partial class FormSelectApp : Form
    {
        private static FormSelectApp _instance;
        public static FormSelectApp instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FormSelectApp();
                }
                return _instance;
            }
        }

        public FormSelectApp()
        {
            InitializeComponent();
        }

        public void UpdateWindowsList(IntPtr[] all)
        {
            listBox1.Items.Clear();
            foreach (IntPtr hwnd in all)
            {
                string s = PInvokeFunc.GetWindowText(hwnd);
                listBox1.Items.Add(new AppRecord(s, hwnd));
            }
        }

        public IntPtr GetSelectedWindow()
        {
            if (listBox1.SelectedItem != null)
            {
                return ((AppRecord)listBox1.SelectedItem).hwnd;
            }
            else
            {
                return IntPtr.Zero;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            buttonOk.Enabled = listBox1.SelectedIndex >= 0;
        }
    }

    class AppRecord
    {
        public string title;
        public IntPtr hwnd;

        public AppRecord(string title, IntPtr hwnd)
        {
            this.title = title;
            this.hwnd = hwnd;
        }

        public override string ToString()
        {
            return title;
        }
    }
}
