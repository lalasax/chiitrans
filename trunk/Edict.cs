using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace ChiiTrans
{
    class EdictEntry
    {
        public string key;
        public string reading;
        public string[] meaning;

        public EdictEntry(string key, string reading, string[] meaning)
        {
            this.key = key;
            this.reading = reading;
            this.meaning = meaning;
        }

        public static int Comparer(EdictEntry a, EdictEntry b)
        {
            return a.key.CompareTo(b.key);
        }
    }
    
    class Edict
    {
        private static Edict _instance;
        public static Edict instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Edict();
                return _instance;
            }
        }
        public static bool Created()
        {
            return _instance != null;
        }
        public readonly bool Ready;

        EdictEntry[] dict;

        private Regex r;
        private static Regex r2 = new Regex(@"\s*(?:\(\D.*?\)\s*)*(.*)");

        public static string CleanMeaning(string meaning)
        {
            Match m = r2.Match(meaning);
            if (m.Success)
            {
                return m.Groups[1].Value;
            }
            else
                return "";
        }
        
        public Edict()
        {
            Ready = true;
            r = new Regex(@"(.+?)\s+\[(.+?)\]");

            try
            {
                string[] ss = File.ReadAllLines(Path.Combine(Application.StartupPath, "edict\\edict"), Encoding.GetEncoding("EUC-JP"));
                dict = new EdictEntry[ss.Length - 1];
                for (int i = 1; i < ss.Length; ++i)
                {
                    string s = ss[i];
                    string[] part = s.Split('/');
                    Match m = r.Match(part[0]);
                    string key, reading;
                    if (m.Success)
                    {
                        key = m.Groups[1].Value;
                        reading = m.Groups[2].Value;
                    }
                    else
                    {
                        key = part[0].Trim();
                        reading = key;
                    }
                    List<string> meaning = new List<string>();
                    for (int j = 1; j < part.Length; ++j)
                    {
                        string val = CleanMeaning(part[j]);
                        if (val != "")
                            meaning.Add(val);
                    }
                    dict[i - 1] = new EdictEntry(key, reading, meaning.ToArray());
                }
                Array.Sort(dict, EdictEntry.Comparer);
            }
            catch (Exception)
            {
                Ready = false;
            }
        }

        public EdictEntry Search(string key)
        {
            if (!Ready)
                return null;
            for (int i = key.Length; i > 0; --i)
            {
                int id = BinarySearch(key.Substring(0, i));
                if (id < dict.Length && Like(key, dict[id].key))
                    return dict[id];
                if (id - 1 >= 0 && Like(key, dict[id - 1].key))
                    return dict[id - 1];
            }
            return null;
        }

        private int BinarySearch(string key)
        {
            int l = 0;
            int r = dict.Length;
            while (l < r)
            {
                int mid = (l + r) / 2;
                int res = dict[mid].key.CompareTo(key);
                if (res == 0)
                    return mid;
                else if (res < 0)
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
            if (key.Length >= 3 && key == entry)
                return true;
            if (Math.Abs(key.Length - entry.Length) > 1)
                return false;
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
            return hasKanji || (matches >= 3);
        }
    }
}
