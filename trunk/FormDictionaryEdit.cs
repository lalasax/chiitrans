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
    public partial class FormDictionaryEdit : Form
    {
        private static FormDictionaryEdit _instance = null;
        public static FormDictionaryEdit instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FormDictionaryEdit();
                return _instance;
            }
        }

        private EdictEntry[] LastSearchResults;

        public FormDictionaryEdit()
        {
            InitializeComponent();
        }

        public DialogResult MyShow(EdictEntry entry)
        {
            SetTextBoxes(entry);
            textBoxWord.Text = entry.key;
            listBox1.Items.Clear();
            var res = ShowDialog();
            entry.key = textBoxWord.Text;
            entry.reading = textBoxReading.Text;
            entry.meaning = textBoxMeaning.Text.Split('/');
            return res;
        }

        private void SetTextBoxes(EdictEntry entry)
        {
            textBoxReading.Text = entry.reading;
            textBoxMeaning.Text = ((entry.pos != null && entry.pos.Length > 0) ? "(" + string.Join(", ", entry.pos) + ") " : "") + string.Join("/", entry.meaning);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!Edict.instance.Ready)
                return;
            string word = textBoxWord.Text.Trim();
            if (word != "")
            {
                LastSearchResults = Edict.instance.DictionarySearchEntries(word);
                listBox1.Items.Clear();
                foreach (var entry in LastSearchResults)
                {
                    listBox1.Items.Add(string.Format("{0} [{1}] /{2}{3}", entry.key, entry.reading, (entry.pos != null && entry.pos.Length > 0) ? "(" + string.Join(", ", entry.pos) + ") " : "", string.Join("/", entry.meaning)));
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LastSearchResults != null && listBox1.SelectedIndex >= 0)
                SetTextBoxes(LastSearchResults[listBox1.SelectedIndex]);
        }
    }
}
