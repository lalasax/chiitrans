using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Windows.Forms;

namespace ChiiTrans
{
    class Inflect
    {
        private static Inflect _instance;
        private static bool initializing = false;
        public static Inflect instance
        {
            get
            {
                while (initializing)
                    Thread.Sleep(1);
                if (_instance == null)
                    _instance = new Inflect();
                return _instance;
            }
        }
        public bool Ready = false;

        private Inflect()
        {
            try
            {
                initializing = true;
                //long old = DateTime.Now.Ticks;
                InitializeUsingCache();
                //Form1.Debug(((double)(DateTime.Now.Ticks - old) / 10000000).ToString());
            }
            finally
            {
                initializing = false;
            }
        }

        private class Helper
        {
            public string[] orig;
            public string[] forms;
        }

        public class Record
        {
            public string form;
            public string pos;
            public string orig;

            public Record(string form, string pos, string orig)
            {
                this.form = form;
                this.pos = pos;
                this.orig = orig;
            }

            public static int CompareByForm(Record a, Record b)
            {
                int res = a.form.CompareTo(b.form);
                if (res == 0)
                    return a.pos.CompareTo(b.pos);
                else
                    return res;
            }
        }

        public List<Record> conj;
        
        private void Initialize()
        {
            try
            {
                string fn = Path.Combine(Path.Combine(Application.StartupPath, "edict"), "conj.txt");
                string[] s = File.ReadAllLines(fn);
                Dictionary<string, Helper> d = new Dictionary<string, Helper>();
                conj = new List<Record>();
                for (int i = 0; i < s.Length; i += 3)
                {
                    string pos = s[i];
                    Helper helper = new Helper();
                    if (s[i + 1] == "" || s[i + 1] == ",")
                        helper.orig = new string[] { "" };
                    else
                        helper.orig = s[i + 1].Split(',');
                    helper.forms = s[i + 2].Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
                    if (d.ContainsKey(pos))
                    {
                        pos += " ";
                    }
                    d.Add(pos, helper);
                }
                foreach (var dd in d)
                {
                    HashSet<string> been = new HashSet<string>();
                    addConj(dd.Key, null, dd.Key, d, "", 0);
                }
                conj.Sort(Record.CompareByForm);
                List<Record> res = new List<Record>();
                res.Add(conj[0]);
                for (int i = 1; i < conj.Count; ++i)
                {
                    if (conj[i].form == conj[i - 1].form && conj[i].pos == conj[i - 1].pos && conj[i].orig == conj[i - 1].orig)
                        continue;
                    res.Add(conj[i]);
                }
                conj = res;

                /*StringBuilder dbg = new StringBuilder();
                foreach (var k in conj)
                {
                    if (k.form == "")
                        dbg.Append(string.Format("{0}:{1}:{2}\r\n", k.form, k.pos, k.orig));
                }
                Form1.Debug(dbg.ToString());*/
                //Form1.Debug(conj.Count.ToString());

                Ready = true;
            }
            catch (Exception)
            {
                Ready = false;
            }
        }

        private void InitializeUsingCache()
        {
            string file_name = Path.Combine(Path.Combine(Application.StartupPath, "edict"), "conj.txt");
            string cache_name = Path.Combine(Path.Combine(Application.StartupPath, "edict"), "conj.cache.txt");
            try
            {
                if (File.GetLastWriteTime(file_name) >= File.GetLastWriteTime(cache_name))
                    throw new Exception();
                var sr = new StreamReader(new FileStream(cache_name, FileMode.Open, FileAccess.Read), Encoding.UTF8);
                try
                {
                    int n = int.Parse(sr.ReadLine());
                    conj = new List<Record>(n);
                    for (int i = 0; i < n; ++i)
                    {
                        string form = sr.ReadLine();
                        string pos = sr.ReadLine();
                        string orig = sr.ReadLine();
                        conj.Add(new Record(form, pos, orig));
                    }
                }
                finally
                {
                    sr.Close();
                }
            }
            catch (Exception)
            {
                Initialize();
                if (Ready)
                {
                    try
                    {
                        SaveToCache(cache_name);
                    }
                    catch (Exception)
                    { }
                }
            }
        }

        private void SaveToCache(string cache_name)
        {
            StreamWriter sw = new StreamWriter(new FileStream(cache_name, FileMode.Create, FileAccess.Write), Encoding.UTF8);
            try
            {
                sw.WriteLine(conj.Count);
                foreach (Record rec in conj)
                {
                    sw.WriteLine(rec.form);
                    sw.WriteLine(rec.pos);
                    sw.WriteLine(rec.orig);
                }
            }
            finally
            {
                sw.Close();
            }
        }

        private void addConj(string pos, string nya_orig, string item, Dictionary<string, Helper> d, string stem, int deep)
        {
            if (stem.Length > 5)
                return;
            Helper helper = d[item];
            foreach (string orig in helper.orig)
            {
                foreach (string form in helper.forms)
                {
                    if (form.EndsWith(")"))
                    {
                        int x = form.IndexOf('(');
                        string link = form.Substring(x + 1, form.Length - x - 2);
                        string st = form.Substring(0, x);
                        conj.Add(new Record(stem + st, pos.TrimEnd(), nya_orig != null ? nya_orig : orig));
                        st = st.Substring(0, st.Length - d[link].orig[0].Length);
                        if (deep < 3)
                            addConj(pos, nya_orig != null ? nya_orig : orig, link, d, stem + st, deep + 1);
                    }
                    else
                    {
                        conj.Add(new Record(stem + (form == "0" ? "" : form), pos.TrimEnd(), nya_orig != null ? nya_orig : orig));
                    }
                }
            }
        }

        private int BinarySearch(List<Record> list, string key)
        {
            int l = 0;
            int r = list.Count;
            while (l < r)
            {
                int mid = (l + r) / 2;
                int res = list[mid].form.CompareTo(key);
                if (res < 0)
                {
                    l = mid + 1;
                }
                else
                {
                    r = mid;
                }
            }
            return l;
        }
        
        public int Search(string form)
        {
            return BinarySearch(conj, form);
        }
    }
}
