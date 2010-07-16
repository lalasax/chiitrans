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
    public partial class FormTooltip : Form
    {
        private static FormTooltip _instance;
        public static FormTooltip instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FormTooltip();
                return _instance;
            }
        }

        private Font LabelFont;
        private Label label1;
        private Size labelMaxSize;
        protected override bool ShowWithoutActivation
        {
            get
            {
                return true;
            }
        }
        
        public FormTooltip()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.None;
            //panel.Left = 1;
            //panel.Top = 1;

            panel.MaximumSize = new Size(MaximumSize.Width - 12, MaximumSize.Height);
            labelMaxSize = new Size(panel.MaximumSize.Width, panel.MaximumSize.Height);
            label1 = new Label();
            label1.AutoSize = true;
            label1.MaximumSize = labelMaxSize;
            label1.Margin = new Padding(0, 0, 0, 5);
            panel.ColumnStyles[0] = new ColumnStyle(SizeType.Absolute, 35);
            MoveAway();
        }

        public void ShowTooltip(string title, string text)
        {
            //SuspendLayout();
            //panel.SuspendLayout();
            LabelFont = new Font("Arial", Global.options.font.Size * 0.8f, FontStyle.Regular);
            label1.Font = new Font("MS Mincho", Global.options.font.Size * 1.1f, FontStyle.Regular);
            panel.ColumnStyles[0].Width = Global.options.font.Size * 3f;
            label1.Text = title;
            string[] meaning = text.Split('$');
            panel.Controls.Clear();
            panel.Controls.Add(label1, 0, 0);
            panel.SetColumnSpan(label1, 2);
            int row = 1;
            //bool haveList = false;
            for (int i = 0; i < meaning.Length; ++i)
            {
                if (meaning[i] == "" || meaning[i] == "-")
                    continue;
                if (i == 0)
                {
                    Label label = new Label();
                    label.AutoSize = true;
                    label.MaximumSize = labelMaxSize;
                    label.Font = LabelFont;
                    label.Text = meaning[i].Trim();
                    panel.Controls.Add(label, 0, row++);
                    panel.SetColumnSpan(label, 2);
                }
                else
                {
                    Label labelNum = new Label();
                    labelNum.AutoSize = true;
                    labelNum.Font = LabelFont;
                    labelNum.Text = row.ToString() + ".";
                    labelNum.Dock = DockStyle.Right;
                    labelNum.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                    panel.Controls.Add(labelNum, 0, row);
                    labelNum.Margin = new Padding(0);
                    labelNum.Padding = new Padding(0);
                    Label label = new Label();
                    label.AutoSize = true;
                    label.MaximumSize = new Size(labelMaxSize.Width - (int)panel.ColumnStyles[0].Width - 1, labelMaxSize.Height);
                    label.Font = LabelFont;
                    label.Text = meaning[i].Trim();
                    label.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                    panel.Controls.Add(label, 1, row++);
                    label.Margin = new Padding(0);
                    label.Padding = new Padding(0);
                    //haveList = true;
                }
            }
            /*if (haveList)
            {
                panel.SetColumn(label1, 1);
                panel.SetColumnSpan(label1, 1);
            }*/
            //panel.ResumeLayout(true);
            //ResumeLayout();

            Point newpos = new Point(Cursor.Position.X + 15, Cursor.Position.Y + 15);
            Rectangle workingArea = Screen.GetWorkingArea(Form1.thisForm);
            int screenWidth = workingArea.Width + workingArea.Left;
            int screenHeight = workingArea.Height + workingArea.Top;
            if (newpos.X + Width > screenWidth)
                newpos.X = screenWidth - Width - 15;
            if (newpos.Y + Height > screenHeight)
                newpos.Y = Cursor.Position.Y - 15 - Height;
            if (newpos.X < workingArea.Left)
                newpos.X = workingArea.Left;
            if (newpos.Y < workingArea.Top)
                newpos.Y = workingArea.Top;
            Location = newpos;

            //Form1.thisForm.suspendBottomLayerUpdates = true;
            //TopLevel = true;
            //
            /*bool form1top = Form1.thisForm.TopMost;
            if (form1top)
            {
                //Form1.thisForm.TopMost = false;
                TopMost = true;
            }*/
            _Show();
            //Form1.thisForm.suspendBottomLayerUpdates = false;
        }

        public void _Show()
        {
            PInvokeFunc.ShowWindow(this.Handle, PInvokeFunc.SW_SHOWNOACTIVATE);
            PInvokeFunc.SetWindowPos(this.Handle, new IntPtr(-1), 0, 0, 0, 0, 19);
        }
        
        private void MoveAway()
        {
            this.Location = new Point(99999, 99999);
        }

        private void panel_MouseLeave(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
