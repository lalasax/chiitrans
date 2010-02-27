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
        public bool priority;

        public EdictEntry(string key, string reading, string[] meaning, bool priority)
        {
            this.key = key;
            this.reading = reading;
            this.meaning = meaning;
            this.priority = priority;
        }

        public static int Comparer(EdictEntry a, EdictEntry b)
        {
            return a.key.CompareTo(b.key);
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

        private readonly EdictEntry[] dict, user, rdict;

        //private static Regex r2 = new Regex(@"\s*(?:\(\D.*?\)\s*)*(.*)");

        public static string CleanMeaning(string meaning, out bool priority)
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
            priority = false;
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
                                priority = true;
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

        private EdictEntry[] LoadDict(string filename, Encoding encoding)
        {
            try
            {
                string[] ss = File.ReadAllLines(filename, encoding);
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
                    bool priority = false;
                    for (int j = 1; j < part.Length; ++j)
                    {
                        bool pp;
                        string val = CleanMeaning(part[j], out pp);
                        priority = priority || pp;
                        if (val != "")
                            meaning.Add(val);
                    }
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
            Ready = true;
            dict = LoadDict(Path.Combine(Application.StartupPath, "edict\\edict"), Encoding.GetEncoding("EUC-JP"));
            if (dict == null)
            {
                Ready = false;
                return;
            }
            rdict = (EdictEntry[])dict.Clone();
            Array.Sort(rdict, EdictEntry.ByReading);
            user = LoadDict(Path.Combine(Application.StartupPath, "edict\\user.txt"), Encoding.UTF8);
        }

        private EdictEntry SearchByReading(string key)
        {
            int id = BinarySearchByReading(rdict, key);
            if (id < rdict.Length && key == rdict[id].reading)
                return rdict[id];
            else
                return null;
        }
        
        private EdictEntry SearchInt(string key, bool second)
        {
            if (!Ready)
                return null;
            for (int i = key.Length; i > 0 && i > key.Length - 3; --i)
            {
                int id;
                if (user != null)
                {
                    id = BinarySearch(user, key.Substring(0, i));
                    if (id < user.Length && Like(key, user[id].key))
                        return user[id];
                    if (id - 1 >= 0 && Like(key, user[id - 1].key))
                        return user[id - 1];
                }
                id = BinarySearch(dict, key.Substring(0, i));
                if (id < dict.Length && Like(key, dict[id].key))
                    return dict[id];
                if (id - 1 >= 0 && Like(key, dict[id - 1].key))
                    return dict[id - 1];
            }
            if (!second && key.ToCharArray().All(Translation.isKatakana))
                return SearchInt(Translation.KatakanaToHiragana(key), true);
            if (key.Length >= 3 && key.ToCharArray().All(Translation.isHiragana))
                return SearchByReading(key);
            return null;
        }

        public EdictEntry Search(string key)
        {
            EdictEntry res = SearchInt(key, false);
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

        private int BinarySearchByReading(EdictEntry[] dict, string key)
        {
            int l = 0;
            int r = dict.Length;
            while (l < r)
            {
                int mid = (l + r) / 2;
                int res = dict[mid].reading.CompareTo(key);
                if (res == 0 && dict[mid].priority)
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
            return (hasKanji || matches >= 3) && (Math.Max(key.Length, entry.Length) - matches <= 2);
        }
    }
}
