using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace ChiiTrans
{
    public partial class FormAgthKeySearch : Form
    {
        private static FormAgthKeySearch _instance = null;
        public static FormAgthKeySearch instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FormAgthKeySearch();
                }
                return _instance;
            }
        }
        
        public FormAgthKeySearch()
        {
            InitializeComponent();
        }

        public string ShowSearch(string exeFile)
        {
            textBoxTitle.Text = "";
            gridResults.Rows.Clear();
            labelResults.Text = "Search results";

            if (System.IO.File.Exists(exeFile))
            {
                try
                {
                    var info = System.Diagnostics.FileVersionInfo.GetVersionInfo(exeFile);
                    textBoxTitle.Text = info.ProductName;
                    if (string.IsNullOrEmpty(info.ProductName))
                        textBoxTitle.Text = info.CompanyName;
                }
                catch (Exception) {}
            }

            textBoxTitle.Focus();

            if (ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (gridResults.Rows.Count > 0)
                {
                    if (gridResults.SelectedRows.Count > 0)
                        return gridResults.SelectedRows[0].Cells[1].Value.ToString();
                    else
                        return gridResults.Rows[0].Cells[1].Value.ToString();
                }
                else
                    return null;
            }
            else
            {
                return null;
            }
        }
        
        private void buttonSearch_Click(object sender, EventArgs e)
        {
            string title = textBoxTitle.Text.Trim();
            if (title.Length == 0) return;

            Cursor.Current = Cursors.WaitCursor;
            try
            {
                gridResults.Rows.Clear();
                var request = Translation.CreateHTTPRequest("http://agthdb.bakastyle.com/games/search?q=" + Translation.UrlEncode(title));
                request.Accept = "application/json";
                string res = Translation.ReadAnswer(request);
                JsArray js = (JsArray)Json.Parse(res);
                for (var i = 0; i < js.length; ++i)
                {
                    var info = js[i];
                    gridResults.Rows.Add(info["game"]["title"].ToString(), info["game"]["agth"].ToString());
                }
                labelResults.Text = "Search results for \"" + title + "\"";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void textBoxTitle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonSearch_Click(sender, null);
                e.Handled = true;
            }
        }

        private void gridResults_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
