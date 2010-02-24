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
    public partial class FormAddReplacement : Form
    {
        public FormAddReplacement()
        {
            InitializeComponent();
        }

        public void UpdateControls(string oldText, string newText)
        {
            textBoxOldText.Text = oldText;
            textBoxNewText.Text = newText;
        }

        public void GetControlValues(out string oldText, out string newText)
        {
            oldText = textBoxOldText.Text;
            newText = textBoxNewText.Text;
        }

        private void FormAddReplacement_Shown(object sender, EventArgs e)
        {
            if (textBoxOldText.Text == "")
                textBoxOldText.Focus();
            else
                textBoxNewText.Focus();
        }

        private void textBoxOldText_TextChanged(object sender, EventArgs e)
        {
            button3.Enabled = textBoxOldText.Text != "";
        }
    }
}
