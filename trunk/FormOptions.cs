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
    public partial class FormOptions : Form
    {
        private Font font;
        private Font tooltipFont;
        private Options options;
        private List<ColorRecord> colors;
        private bool userDictInit;
        private bool userDictChanged;

        public FormOptions()
        {
            InitializeComponent();

            for (int i = 0; i < tabControl1.TabCount; ++i)
                tabControl1.TabPages[i].BackColor = System.Drawing.SystemColors.Control;
            WindowPosition.Deserialize(this, Global.windowPosition.OptionsFormPosition);
        }

        private static Image ImageFromColor(Color color)
        {
            Bitmap bmp = new Bitmap(20, 20);
            Graphics gr = Graphics.FromImage(bmp);
            Pen p = Pens.Black;
            Brush br = new SolidBrush(color);
            Rectangle all = new Rectangle(0, 0, bmp.Width - 1, bmp.Height - 1);
            gr.FillRectangle(br, all);
            gr.DrawRectangle(p, all);
            return bmp;
        }

        private void UpdateColors()
        {
            gridColors.Rows.Clear();
            colors = new List<ColorRecord>();
            foreach (ColorRecord rec in options.colors.Values)
            {
                colors.Add(rec);
                gridColors.Rows.Add(rec.name, ImageFromColor(rec.color));
            }
        }
        
        private void UpdateControls()
        {
            listBoxTranslators.Items.Clear();
            foreach (TranslatorRecord rec in options.translators)
            {
                listBoxTranslators.Items.Add(Translation.Translators[rec.id], rec.inUse);
            }

            UpdateColors();

            switch (options.wordParseMethod)
            {
                case Options.PARSE_NONE:
                    radioNone.Checked = true;
                    break;
                case Options.PARSE_BUILTIN:
                    radioEdict.Checked = true;
                    break;
                case Options.PARSE_WWWJDIC:
                    radioJdic.Checked = true;
                    break;
            }
            comboBoxJDic.Text = options.JDicServer;
            checkBoxAlwaysOnTop.Checked = options.alwaysOnTop;
            checkBoxCheckDouble.Checked = options.checkDouble;
            checkBoxCheckPhrases.Checked = options.checkRepeatingPhrases;
            checkBoxRepeatingPhrasesAdv.Checked = options.checkRepeatingPhrasesAdv;
            checkBoxSuffixes.Checked = options.replaceSuffixes;
            checkBoxExcludeSpeakers.Checked = options.excludeSpeakers;
            textBoxPattern.Text = options.excludeSpeakersPattern;
            numericClipboardPollInterval.Value = options.messageDelay;
            numericMaxSourceLength.Value = options.maxSourceLength;
            this.font = options.font;
            fontDialog1.Font = font;
            textBoxFont.Text = (string)(new FontConverter().ConvertToString(font));
            this.tooltipFont = options.tooltipFont;
            textBoxTooltipFont.Text = (string)(new FontConverter().ConvertToString(tooltipFont));
            checkBoxTranslateOther.Checked = options.translateToOtherLanguage;
            textBoxLanguage.Text = options.translateLanguage;
            checkBoxCache.Checked = options.useCache;
            checkBoxDisplayOriginal.Checked = options.displayOriginal;
            checkBoxDisplayFixed.Checked = options.displayFixed;
            trackBarOpacity.Value = options.bottomLayerOpacity;
            checkBoxDisplayReadings.Checked = options.displayReadings;
            checkBoxAppendBottom.Checked = options.appendBottom;
            checkBoxShadow.Checked = options.dropShadow;
            checkBoxPromt.Checked = options.usePromt;
            if (options.furiganaRomaji)
                radioFuriganaRomaji.Checked = true;
            else
                radioFuriganaHiragana.Checked = true;
            numMaxBlocks.Value = options.maxBlocks;
            checkBoxLargeMargin.Checked = options.largeMargins;
            numMarginSize.Value = options.marginSize;
            comboBoxHivemind.Text = options.hivemindServer;

            bindingSource1.DataSource = options.replacements;
            bindingSource1.AllowNew = true;
            dataGridView1.Columns[0].DataPropertyName = "oldText";
            dataGridView1.Columns[1].DataPropertyName = "newText";
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            int itemindex = listBoxTranslators.SelectedIndex;
            if (itemindex <= 0)
                return;
            object old = listBoxTranslators.Items[itemindex - 1];
            bool old_check = listBoxTranslators.GetItemChecked(itemindex - 1);
            listBoxTranslators.Items[itemindex - 1] = listBoxTranslators.Items[itemindex];
            listBoxTranslators.SetItemChecked(itemindex - 1, listBoxTranslators.GetItemChecked(itemindex));
            listBoxTranslators.Items[itemindex] = old;
            listBoxTranslators.SetItemChecked(itemindex, old_check);
            listBoxTranslators.SelectedIndex = itemindex - 1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int itemindex = listBoxTranslators.SelectedIndex;
            if (itemindex < 0 || itemindex == listBoxTranslators.Items.Count - 1)
                return;
            object old = listBoxTranslators.Items[itemindex + 1];
            bool old_check = listBoxTranslators.GetItemChecked(itemindex + 1);
            listBoxTranslators.Items[itemindex + 1] = listBoxTranslators.Items[itemindex];
            listBoxTranslators.SetItemChecked(itemindex + 1, listBoxTranslators.GetItemChecked(itemindex));
            listBoxTranslators.Items[itemindex] = old;
            listBoxTranslators.SetItemChecked(itemindex, old_check);
            listBoxTranslators.SelectedIndex = itemindex + 1;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                font = fontDialog1.Font;
                textBoxFont.Text = (string)(new FontConverter().ConvertToString(font));
            }
        }

        private void checkBoxExcludeSpeakers_CheckedChanged(object sender, EventArgs e)
        {
            textBoxPattern.Enabled = checkBoxExcludeSpeakers.Checked;
        }

        public void SaveOptions()
        {
            for (int i = 0; i < listBoxTranslators.Items.Count; ++i)
            {
                bool inUse = listBoxTranslators.GetItemChecked(i);
                int id = Array.IndexOf(Translation.Translators, listBoxTranslators.Items[i]);
                Global.options.translators[i] = new TranslatorRecord(id, inUse);
            }

            foreach (ColorRecord rec in Global.options.colors.Values)
            {
                foreach (ColorRecord rec2 in colors)
                {
                    if (rec.name == rec2.name)
                        rec.color = rec2.color;
                }
            }

            if (radioEdict.Checked)
                Global.options.wordParseMethod = Options.PARSE_BUILTIN;
            else if (radioJdic.Checked)
                Global.options.wordParseMethod = Options.PARSE_WWWJDIC;
            else
                Global.options.wordParseMethod = Options.PARSE_NONE;
            Global.options.JDicServer = comboBoxJDic.Text;
            Global.options.alwaysOnTop = checkBoxAlwaysOnTop.Checked;
            Global.options.checkDouble = checkBoxCheckDouble.Checked;
            Global.options.checkRepeatingPhrases = checkBoxCheckPhrases.Checked;
            Global.options.checkRepeatingPhrasesAdv = checkBoxRepeatingPhrasesAdv.Checked;
            Global.options.replaceSuffixes = checkBoxSuffixes.Checked;
            Global.options.excludeSpeakers = checkBoxExcludeSpeakers.Checked;
            Global.options.excludeSpeakersPattern = textBoxPattern.Text;
            Global.options.messageDelay = (int)numericClipboardPollInterval.Value;
            Global.options.maxSourceLength = (int)numericMaxSourceLength.Value;
            Global.options.font = this.font;
            Global.options.tooltipFont = this.tooltipFont;
            Global.options.replacements = options.replacements;
            Global.options.translateToOtherLanguage = checkBoxTranslateOther.Checked;
            Global.options.translateLanguage = textBoxLanguage.Text;
            Global.options.useCache = checkBoxCache.Checked;
            Global.options.displayOriginal = checkBoxDisplayOriginal.Checked;
            Global.options.displayFixed = checkBoxDisplayFixed.Checked;
            Global.options.bottomLayerOpacity = trackBarOpacity.Value;
            Global.options.displayReadings = checkBoxDisplayReadings.Checked;
            Global.options.appendBottom = checkBoxAppendBottom.Checked;
            Global.options.dropShadow = checkBoxShadow.Checked;
            Global.options.usePromt = checkBoxPromt.Checked;
            Global.options.furiganaRomaji = radioFuriganaRomaji.Checked;
            Global.options.maxBlocks = (int)numMaxBlocks.Value;
            Global.options.largeMargins = checkBoxLargeMargin.Checked;
            Global.options.marginSize = (int)numMarginSize.Value;
            Global.options.hivemindServer = comboBoxHivemind.Text;

            try
            {
                Global.options.Save();
            }
            catch (Exception) 
            {
            }

            if (userDictChanged)
            {
                try
                {
                    string userDict = GenUserDictText();
                    Edict.instance.ReloadUserDictionary(userDict);
                    Edict.instance.SaveDictText("user.txt", userDict);
                }
                catch (Exception)
                {
                    MessageBox.Show("Cannot save user dictionary!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            options = new Options();
            options.SetDefault();
            this.UpdateControls();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete all items?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                bindingSource1.Clear();
            }
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    options.LoadReplacements(openFileDialog1.FileName);
                    bindingSource1.DataSource = options.replacements;
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed to load replacements!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Options tmp = new Options();
                    tmp.LoadReplacements(openFileDialog1.FileName);
                    options.replacements.AddRange(tmp.replacements);
                    bindingSource1.ResetBindings(false);
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed to add replacements!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    options.SaveReplacements(saveFileDialog1.FileName);
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed to save replacements!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void MoveRow(int old_index, int new_index)
        {
            if (new_index < 0 || new_index >= options.replacements.Count)
                return;
            Replacement tmp = options.replacements[old_index];
            options.replacements[old_index] = options.replacements[new_index];
            options.replacements[new_index] = tmp;
            bindingSource1.ResetItem(old_index);
            bindingSource1.ResetItem(new_index);
        }

        private void moveUpButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                List<int> x = new List<int>();
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    x.Add(row.Index);
                }
                x.Sort();
                foreach (int index in x)
                {
                    MoveRow(index, index - 1);
                }
                bindingSource1.MovePrevious();
                foreach (int index in x)
                {
                    if (index - 1 >= 0)
                        dataGridView1.Rows[index - 1].Selected = true;
                }
            }
            else
            {
                MoveRow(dataGridView1.CurrentRow.Index, dataGridView1.CurrentRow.Index - 1);
                bindingSource1.MovePrevious();
            }
        }

        private void moveDownButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                List<int> x = new List<int>();
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    x.Add(row.Index);
                }
                var xx = x.OrderByDescending(a => a);
                foreach (int index in xx)
                {
                    MoveRow(index, index + 1);
                }
                bindingSource1.MoveNext();
                foreach (int index in xx)
                {
                    if (index + 1 < bindingSource1.List.Count)
                        dataGridView1.Rows[index + 1].Selected = true;
                }
            }
            else
            {
                MoveRow(dataGridView1.CurrentRow.Index, dataGridView1.CurrentRow.Index + 1);
                bindingSource1.MoveNext();
            }
        }

        private void sortByOld_Click(object sender, EventArgs e)
        {
            options.replacements.Sort(ReplacementListComparers.SortByOldText);
            bindingSource1.ResetBindings(false);
        }

        private void sortByNew_Click(object sender, EventArgs e)
        {
            options.replacements.Sort(ReplacementListComparers.SortByNewText);
            bindingSource1.ResetBindings(false);
        }

        private void removeItemsButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 1)
            {
                if (MessageBox.Show("Delete " + dataGridView1.SelectedRows.Count.ToString() + " rows?", "Confirm action", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    {
                        if (!row.IsNewRow)
                            dataGridView1.Rows.Remove(row);
                    }
                }
            }
            else
                dataGridView1.Rows.Remove(dataGridView1.CurrentRow);
        }

        private void checkBoxTranslateOther_CheckedChanged(object sender, EventArgs e)
        {
            textBoxLanguage.Enabled = checkBoxTranslateOther.Checked;
        }

        private void FormOptions_Shown(object sender, EventArgs e)
        {
            options = Global.options.Clone();
            UpdateControls();
            comboBoxJDic.Select(0, 0);
            comboBoxHivemind.Select(0, 0);
            WindowPosition.Normalize(this);
            userDictInit = false;
            userDictChanged = false;
        }

        private void buttonInsert_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                bindingSource1.Insert(dataGridView1.CurrentRow.Index, new Replacement());
                bindingSource1.MovePrevious();
            }
        }

        private void gridColors_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (gridColors.SelectedRows.Count == 0)
                return;
            int num = gridColors.SelectedRows[0].Index;
            colorDialog1.Color = colors[num].color;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                colors[num].color = colorDialog1.Color;
                gridColors.Rows[num].Cells[1].Value = ImageFromColor(colors[num].color);
            }
        }

        private void buttonResetColors_Click(object sender, EventArgs e)
        {
            options.SetDefaultColors();
            UpdateColors();
        }

        private void tabPage5_Enter(object sender, EventArgs e)
        {
            if (!userDictInit)
            {
                try
                {
                    InitGridUserDict(Edict.instance.user);
                }
                catch (Exception) { }
                userDictInit = true;
                userDictChanged = false;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                tooltipFont = fontDialog1.Font;
                textBoxTooltipFont.Text = (string)(new FontConverter().ConvertToString(tooltipFont));
            }
        }

        private void gridUserDict_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            userDictChanged = true;
        }

        private void InitGridUserDict(EdictEntry[] data)
        {
            gridUserDict.Rows.Clear();
            foreach (EdictEntry entry in data)
            {
                gridUserDict.Rows.Add(entry.key, entry.reading, ((entry.pos != null && entry.pos.Length > 0) ? "(" + string.Join(", ", entry.pos) + ") " : "") + string.Join("/", entry.meaning));
            }
        }

        private string GenUserDictText()
        {
            var res = new StringBuilder();
            res.AppendLine("# User dictionary (eDict format)");
            foreach (DataGridViewRow row in gridUserDict.Rows)
            {
                if (row.Cells[0].Value.ToString().Trim().Length > 0)
                    res.AppendLine(string.Format("{0} [{1}]/{2}", row.Cells[0].Value, row.Cells[1].Value, row.Cells[2].Value));
            }
            return res.ToString();
        }

        private void gridUserDict_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                var row = gridUserDict.Rows[e.RowIndex];
                EdictEntry entry = new EdictEntry(row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString(), row.Cells[2].Value.ToString().Split('/'), null, 0);
                if (FormDictionaryEdit.instance.MyShow(entry) == System.Windows.Forms.DialogResult.OK)
                {
                    row.SetValues(entry.key, entry.reading, string.Join("/", entry.meaning));
                }
            }
        }

        private void gridUserDict_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (gridUserDict.SelectedRows.Count > 0)
                {
                    if (MessageBox.Show("Delete " + gridUserDict.SelectedRows.Count + " rows?", "Delete", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        foreach (DataGridViewRow row in gridUserDict.SelectedRows)
                        {
                            gridUserDict.Rows.Remove(row);
                        }
                    }
                    e.Handled = true;
                }
            }
        }
    }
}
