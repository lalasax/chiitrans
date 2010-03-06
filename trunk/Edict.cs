using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading;

namespace ChiiTrans
{
    class EdictEntry
    {
        public string key;
        public string reading;
        public string[] meaning;
        public int priority;

        public EdictEntry(string key, string reading, string[] meaning, int priority)
        {
            this.key = key;
            this.reading = reading;
            this.meaning = meaning;
            this.priority = priority;
        }

        public static int Comparer(EdictEntry a, EdictEntry b)
        {
            int res = a.key.CompareTo(b.key);
            if (res == 0)
                return b.priority.CompareTo(a.priority);
            else
                return res;
        }

        public static int ByReading(EdictEntry a, EdictEntry b)
        {
            int res = a.reading.CompareTo(b.reading);
            if (res == 0)
                return b.priority.CompareTo(a.priority);
            else
                return res;
        }
    }
    
    class Edict
    {
        private static Edict _instance;
        private static bool initializing = false;
        public static Edict instance
        {
            get
            {
                if (initializing)
                {
                    while (initializing)
                        Thread.Sleep(1);
                }
                if (_instance == null)
                {
                    initializing = true;
                    _instance = new Edict();
                    initializing = false;
                }
                return _instance;
            }
        }
        public static bool Created()
        {
            return _instance != null;
        }
        public readonly bool Ready;

        private readonly EdictEntry[] dict, rdict;
        private EdictEntry[] user;

        //private static Regex r2 = new Regex(@"\s*(?:\(\D.*?\)\s*)*(.*)");

        public static string CleanMeaning(string meaning, out int priority)
        {
            /*Match m = r2.Match(meaning);
            if (m.Success)
            {
                return m.Groups[1].Value;
            }
            else
                return "";*/
            int i = 0;
            string num = "";
            priority = 0;
            while (i < meaning.Length)
            {
                if (char.IsWhiteSpace(meaning[i]))
                {
                    ++i;
                    continue;
                }
                if (meaning[i] == '(')
                {
                    if (i + 1 < meaning.Length)
                    {
                        int pos = meaning.IndexOf(')', i + 1);
                        if (pos >= 0)
                        {
                            if (meaning[i + 1] >= '0' && meaning[i + 1] <= '9')
                            {
                                num = meaning.Substring(i, pos + 1 - i) + " ";
                            }
                            if (meaning[i + 1] == 'P' && meaning[i + 2] == ')')
                            {
                                priority += 2;
                            }
                            if (meaning[i + 1] == 'u' && meaning[i + 2] == 'k' && meaning[i + 3] == ')')
                            {
                                priority += 1;
                            }
                            i = pos + 1;
                            continue;
                        }
                    }
                }
                break;
            }
            return num + meaning.Substring(i);
        }

        private EdictEntry[] LoadDict(string[] ss)
        {
            try
            {
                EdictEntry[] res = new EdictEntry[ss.Length - 1];
                for (int i = 1; i < ss.Length; ++i)
                {
                    string s = ss[i];
                    string[] part = s.Split('/');
                    string head = part[0];
                    int p0 = head.IndexOf('[');
                    int p1 = -1;
                    if (p0 >= 0)
                        p1 = head.IndexOf(']', p0);
                    string key, reading;
                    if (p0 >= 0 && p1 >= 0)
                    {
                        key = head.Substring(0, p0).TrimEnd();
                        reading = head.Substring(p0 + 1, p1 - (p0 + 1));
                    }
                    else
                    {
                        key = head.Trim();
                        reading = key;
                    }
                    List<string> meaning = new List<string>(part.Length - 1);
                    bool pri1 = false;
                    bool pri2 = false;
                    for (int j = 1; j < part.Length; ++j)
                    {
                        int pp;
                        string val = CleanMeaning(part[j], out pp);
                        if (pp == 1)
                            pri1 = true;
                        if (pp == 2)
                            pri2 = true;
                        if (pp == 3)
                        {
                            pri1 = true;
                            pri2 = true;
                        }
                        if (val != "")
                            meaning.Add(val);
                    }
                    int priority = (pri1 ? 1 : 0) + (pri2 ? 2 : 0);
                    if (priority > 3)
                        MessageBox.Show("Debug: priority > 3");
                    res[i - 1] = new EdictEntry(key, reading, meaning.ToArray(), priority);
                }
                Array.Sort(res, EdictEntry.Comparer);
                return res;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        public Edict()
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                Ready = true;
                dict = LoadDict(LoadDictText("edict", Encoding.GetEncoding("EUC-JP")));
                if (dict == null)
                {
                    Ready = false;
                    return;
                }
                rdict = (EdictEntry[])dict.Clone();
                Array.Sort(rdict, EdictEntry.ByReading);
                ReloadUserDictionary();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        public void ReloadUserDictionary(string text)
        {
            user = LoadDict(text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
        }

        public void ReloadUserDictionary()
        {
            user = LoadDict(LoadDictText("user.txt", Encoding.UTF8));
        }

        private string GetRealFilename(string filename)
        {
            return Path.Combine(Path.Combine(Application.StartupPath, "edict"), filename);
        }
        
        public string[] LoadDictText(string filename, Encoding encoding)
        {
            return File.ReadAllLines(GetRealFilename(filename), encoding);
        }

        public string[] LoadDictText(string filename)
        {
            return LoadDictText(filename, Encoding.UTF8);
        }

        public void SaveDictText(string filename, string text, Encoding encoding)
        {
            File.WriteAllText(GetRealFilename(filename), text, encoding);
        }

        public void SaveDictText(string filename, string text)
        {
            SaveDictText(filename, text, Encoding.UTF8);
        }

        private EdictEntry SearchByReading(string key, bool checkPriority)
        {
            int id = BinarySearchByReading(rdict, key);
            if (id < rdict.Length && key == rdict[id].reading && (!checkPriority || (key.Length >= 3 && rdict[id].priority >= 1) || rdict[id].priority >= 3))
                return rdict[id];
            else
                return null;
        }
        
        private EdictEntry SearchInt(string key, bool second, int minHira)
        {
            if (!Ready)
                return null;
            for (int i = key.Length; i > 0 && i > key.Length - 3; --i)
            {
                int id;
                if (user != null)
                {
                    EdictEntry res = null;
                    id = BinarySearch(user, key.Substring(0, i));
                    if (id < user.Length && Like(key, user[id].key))
                        res = user[id];
                    if (res != null)
                    {
                        if (!second && res.meaning.Length > 0)
                        {
                            char ch = res.meaning[0][0];
                            if (ch == '=')
                                return SearchByReading(key, false);
                            else if (char.GetUnicodeCategory(ch) == System.Globalization.UnicodeCategory.OtherLetter)
                                return SearchInt(res.meaning[0], true, minHira);
                            else
                                return res;
                        }
                        else
                        {
                            return res;
                        }
                    }
                }
                id = BinarySearch(dict, key.Substring(0, i));
                if (id < dict.Length && Like(key, dict[id].key))
                    return dict[id];
            }
            if (!second && key.ToCharArray().All(Translation.isKatakana))
                return SearchInt(Translation.KatakanaToHiragana(key), true, minHira);
            if (key.Length >= minHira && key.ToCharArray().All(Translation.isHiragana))
                return SearchByReading(key, true);
            return null;
        }

        public EdictEntry Search(string key, int minHira)
        {
            EdictEntry res = SearchInt(key, false, minHira);
            if (res != null && res.meaning.Length == 0)
            {
                return null;
            }
            else
            {
                return res;
            }
        }

        private int BinarySearch(EdictEntry[] dict, string key)
        {
            int l = 0;
            int r = dict.Length;
            while (l < r)
            {
                int mid = (l + r) / 2;
                int res = dict[mid].key.CompareTo(key);
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

        private int BinarySearchByReading(EdictEntry[] dict, string key)
        {
            int l = 0;
            int r = dict.Length;
            while (l < r)
            {
                int mid = (l + r) / 2;
                int res = dict[mid].reading.CompareTo(key);
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
        
        private bool Like(string key, string entry)
        {
            if (key.Length >= 2 && key == entry)
                return true;
            /*if (Math.Abs(key.Length - entry.Length) > 2)
                return false;*/
            for (int j = key.Length; j < entry.Length; ++j)
            {
                if (Translation.isKanji(entry[j]))
                    return false;
            }
            int matches = 0;
            bool hasKanji = false;
            for (int i = 0; i < key.Length; ++i)
            {
                char ch = key[i];
                char ch2 = i < entry.Length ? entry[i] : '\0';
                if (ch == ch2)
                {
                    ++matches;
                    if (Translation.isKanji(ch))
                        hasKanji = true;
                }
                else
                {
                    if (Translation.isKanji(ch))
                        return false;
                    for (int j = i; j < entry.Length; ++j)
                    {
                        if (Translation.isKanji(entry[j]))
                            return false;
                    }
                    break;
                }
            }
            return (hasKanji || matches >= 3) && (entry.Length - matches <= 2);
        }

        private void DictSearchAddItem(HashSet<EdictEntry> added, List<string> res, EdictEntry entry)
        {
            if (!added.Add(entry))
                return;
            res.Add(entry.key);
            res.Add(Translation.formatReading(entry.key, entry.reading, Global.options.furiganaRomaji));
            res.Add(Translation.formatMeaning(entry.meaning));
        }
        
        private void DictSearchAddDict(HashSet<EdictEntry> added, List<string> res, EdictEntry[] dict, string key)
        {
            int id = BinarySearch(dict, key);
            while (id < dict.Length && Like(key, dict[id].key))
            {
                DictSearchAddItem(added, res, dict[id++]);
            }
        }

        private void DictSearchAddDictByReading(HashSet<EdictEntry> added, List<string> res, EdictEntry[] dict, string key)
        {
            int id = BinarySearchByReading(dict, key);
            while (id < dict.Length && key == dict[id].reading)
            {
                DictSearchAddItem(added, res, dict[id++]);
            }
        }
        
        public string[] DictionarySearch(string key)
        {
            if (!Ready)
                return null;
            List<string> res = new List<string>();
            HashSet<EdictEntry> added = new HashSet<EdictEntry>();
            if (user != null)
            {
                DictSearchAddDict(added, res, user, key);
            }
            DictSearchAddDict(added, res, dict, key);
            key = Translation.KatakanaToHiragana(key);
            if (key.ToCharArray().All(Translation.isHiragana))
            {
                DictSearchAddDictByReading(added, res, rdict, key);
            }
            return res.ToArray();
        }
    }
}
