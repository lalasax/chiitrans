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
    public partial class FormRun : Form
    {
        private static FormRun _instance;
        public static FormRun instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FormRun();
                }
                return _instance;
            }
        }

        private string exeName = null;
        
        public FormRun()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Form1.thisForm.openFileDialogRun.ShowDialog() == DialogResult.OK)
            {
                comboBoxExe.Text = Form1.thisForm.openFileDialogRun.FileName;
            }
        }

        public void SetExeName(string exeName)
        {
            this.exeName = exeName;
        }

        public void Run()
        {
            try
            {
                string args = "";
                if (checkBoxFixLocale.Checked)
                    args += "/L ";
                if (textBoxKey.Text != "")
                {
                    string s = textBoxKey.Text;
                    string[] keys = s.Split();
                    foreach (string key in keys)
                    {
                        if (key.Length >= 2 && key[0] == '/')
                        {
                            char ch = char.ToUpper(key[1]);
                            if (ch == 'H' || ch == 'V' || ch == 'X' || ch == 'F' || ch == 'P' || ch == 'K')
                                args += key + " ";
                        }
                    }
                }
                Global.RunGame(comboBoxExe.Text, args);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void UpdateKeys(string keys)
        {
            string[] keys_arr = keys.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> res = new List<string>();
            bool fix_locale = false;
            foreach (string key in keys_arr)
            {
                if (key.ToUpper() != "/L")
                    res.Add(key);
                else
                    fix_locale = true;
            }
            textBoxKey.Text = string.Join(" ", res.ToArray());
            checkBoxFixLocale.Checked = fix_locale;
        }

        private void FormRun_Shown(object sender, EventArgs e)
        {
            comboBoxExe.Items.Clear();
            comboBoxExe.Items.AddRange(Global.agth.appProfiles["profiles"].Keys.ToArray());
            if (exeName == null)
            {
                comboBoxExe.Text = Global.agth.appProfiles.str["last_run"];
            }
            else
            {
                comboBoxExe.Text = exeName;
                exeName = null;
            }
        }

        private void comboBoxExe_TextChanged(object sender, EventArgs e)
        {
            string text = comboBoxExe.Text;
            if (Global.agth.appProfiles["profiles"].ContainsKey(text))
            {
                UpdateKeys(Global.agth.appProfiles["profiles"][text].str["keys"]);
            }
        }
    }
}
