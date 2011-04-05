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

        public static bool isCreated()
        {
            return _instance != null;
        }

        private Font LabelFont;
        private Label label1;
        private Label labelPage;
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

            labelPage = new Label();
            labelPage.Width = 35;
            labelPage.ForeColor = Color.DarkBlue;
            labelPage.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            panel.ColumnStyles[0] = new ColumnStyle(SizeType.Absolute, 35);
            MoveAway();
        }

        private void ShowTooltipInternal(int cur_tt_index, string title, string text)
        {
            LabelFont = new Font(Global.options.tooltipFont.FontFamily.Name, Global.options.tooltipFont.Size * 0.8f, FontStyle.Regular);
            label1.Font = new Font("MS Mincho", Global.options.tooltipFont.Size * 1.1f, FontStyle.Regular);
            panel.ColumnStyles[0].Width = Global.options.tooltipFont.Size * 3f;
            label1.Text = title;
            string[] meaning = text.Split('$');
            panel.Controls.Clear();
            panel.Controls.Add(label1, 0, 0);
            panel.SetColumnSpan(label1, 2);
            panel1.Controls.Remove(labelPage);
            int row = 1;

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
                }
            }

            if (_key.Length >= 2)
            {
                panel1.Controls.Add(labelPage);
                label1.Margin = new Padding(0, 0, 35, 5);
                label1.MaximumSize = new System.Drawing.Size(panel.MaximumSize.Width - 35, panel.MaximumSize.Height);
                var left = panel1.Width - 40;
                labelPage.Left = left;
                labelPage.Top = 5;
                labelPage.Text = string.Format("{0}/{1}", cur_tt_index + 1, _key.Length);
                labelPage.TextAlign = ContentAlignment.MiddleRight;
                labelPage.BringToFront();
            }
            else
            {
                label1.Margin = new Padding(0, 0, 0, 5);
                label1.MaximumSize = labelMaxSize;
            }
        }

        private string[] _key;
        private string[] _reading;
        private string[] _text;
        private Dictionary<string, int> tt_index = new Dictionary<string,int>();
        private string cur_tt_key;
        
        public void ShowTooltip(string key, string reading, string text)
        {
            cur_tt_key = key;
            _key = key.Split('#');
            _reading = reading.Split('#');
            _text = text.Split('#');
            if (!tt_index.ContainsKey(key))
                tt_index.Add(key, 0);
            UpdateTooltip();
            UpdateTooltipPos();
        }

        private void UpdateTooltipPos()
        {
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
            _Show();
        }

        private void UpdateTooltip()
        {
            int cur_tt_index = tt_index[cur_tt_key];
            var key = _key[cur_tt_index];
            var reading = _reading[cur_tt_index];
            var text = _text[cur_tt_index];
            ShowTooltipInternal(cur_tt_index,
                string.IsNullOrWhiteSpace(reading) || reading == key || reading == "-" || Translation.KatakanaToHiragana(key) == reading ? key : string.Format("{0} ({1})", _key[cur_tt_index], _reading[cur_tt_index]),
                _text[cur_tt_index]);
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

        internal void TooltipPage(int delta)
        {
            if (tt_index.ContainsKey(cur_tt_key))
            {
                int cur_tt_index = tt_index[cur_tt_key] + delta;
                if (cur_tt_index < 0) cur_tt_index = 0;
                if (cur_tt_index >= _key.Length) cur_tt_index = _key.Length - 1;
                if (tt_index[cur_tt_key] != cur_tt_index)
                {
                    tt_index[cur_tt_key] = cur_tt_index;
                    //this.SuspendLayout();
                    Hide();
                    UpdateTooltip();
                    UpdateTooltipPos();
                    //this.ResumeLayout(true);
                }
            }
        }
    }
}
