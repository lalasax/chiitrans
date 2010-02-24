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
    public partial class FormBottomLayer : Form
    {
        private static FormBottomLayer _instance;
        public static FormBottomLayer instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FormBottomLayer();
                }
                return _instance;
            }
        }

        public FormBottomLayer()
        {
            InitializeComponent();
        }

        public void UpdatePos()
        {
            int border = (Form1.thisForm.Width - Form1.thisForm.ClientSize.Width) / 2;
            PInvokeFunc.SetWindowPos(Handle, Form1.thisForm.Handle, Form1.thisForm.Left + border, Form1.thisForm.Top + (Form1.thisForm.Height - border - Form1.thisForm.ClientSize.Height), Form1.thisForm.ClientSize.Width, Form1.thisForm.ClientSize.Height, 16);
            WindowState = Form1.thisForm.WindowState;
        }

        private void FormBottomLayer_Activated(object sender, EventArgs e)
        {
            Form1.thisForm.Activate();
        }

        private void FormBottomLayer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;
        }
    }
}
