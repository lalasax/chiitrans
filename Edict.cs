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
        public string[] pos;
        public int priority;
        private int id;
        private static int id_ctr = 0;

        public EdictEntry()
        {
        }
        
        public EdictEntry(string key, string reading)
        {
            this.key = key;
            this.reading = reading;
            this.pos = new string[] { };
            this.id = id_ctr++;
        }
        
        public EdictEntry(string key, string reading, string[] meaning, string[] pos, int priority)
        {
            this.key = key;
            this.reading = reading;
            this.meaning = meaning;
            this.priority = priority;
            this.pos = pos;
            this.id = id_ctr++;
        }

        public void AddPOS(string _pos)
        {
            foreach (string p in pos)
                if (p == _pos)
                    return;
            Array.Resize(ref pos, pos.Length + 1);
            pos[pos.Length - 1] = _pos;
        }

        public bool isPOS(string _pos)
        {
            return Array.IndexOf(pos, _pos) >= 0;
        }

        public static int Comparer(EdictEntry a, EdictEntry b)
        {
            int res = a.key.CompareTo(b.key);
            if (res == 0)
                res = b.priority.CompareTo(a.priority);
            if (res == 0)
                res = a.id.CompareTo(b.id);
            return res;
        }

        public class ByReading: IComparer<EdictEntry>
        {
            public int Compare(EdictEntry a, EdictEntry b)
            {
                int res = a.reading.CompareTo(b.reading);
                if (res == 0)
                    return b.priority.CompareTo(a.priority);
                else
                    return res;
            }
        }

        public void LoadFrom(StreamReader sr)
        {
            key = sr.ReadLine();
            id = int.Parse(sr.ReadLine());
            priority = int.Parse(sr.ReadLine());
            pos = sr.ReadLine().Split(',');
            reading = sr.ReadLine();
            meaning = sr.ReadLine().Split('/');
        }

        public void SaveTo(StreamWriter sw)
        {
            sw.WriteLine(key);
            sw.WriteLine(id.ToString());
            sw.WriteLine(priority.ToString());
            sw.WriteLine(string.Join(",", pos));
            sw.WriteLine(reading);
            sw.WriteLine(string.Join("/", meaning));
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
                while (initializing)
                    Thread.Sleep(1);
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
        public bool Ready;

        public EdictEntry[] dict, rdict;
        public EdictEntry[] user;

        private static string GetAnnotation(ref string meaning)
        {
            if (meaning.Length == 0 || meaning[0] != '(')
                return null;
            int b2 = meaning.IndexOf(')', 1);
            if (b2 < 0)
                return null;
            string res = meaning.Substring(1, b2 - 1);
            ++b2;
            while (b2 < meaning.Length && char.IsWhiteSpace(meaning[b2]))
                ++b2;
            if (b2 >= meaning.Length)
                meaning = "";
            else
                meaning = meaning.Substring(b2);
            return res;
        }

        private static string GetAnnotationLast(ref string meaning)
        {
            int len = meaning.Length;
            if (len == 0 || meaning[len - 1] != ')')
                return null;
            int b2 = meaning.LastIndexOf('(', len - 2);
            if (b2 < 0)
                return null;
            string res = meaning.Substring(b2 + 1, len - b2 - 2);
            --b2;
            while (b2 >= 0 && char.IsWhiteSpace(meaning[b2]))
                --b2;
            if (b2 < 0)
                meaning = "";
            else
                meaning = meaning.Substring(0, b2 + 1);
            return res;
        }

        private static readonly string[] pos_list = { "adj-i", "adj-na", "adj-no", "adj-pn", "adj-f", "adj-t",
            "adv", "adv-to", "aux", "aux-v", "conj", "ctr", "exp", "id", "int", "n", "n-adv", "n-t", "pn", "prt", "pref",
            "suf", "v1", "vi", "vs", "vs-i", "vs-s", "vk", "vt", "vz", "adj-ta", "copula" }; // ,"v5*"

        private static void ParseMeaning(string meaning, EdictEntry entry)
        {
            if (meaning.EndsWith("(P)/"))
            {
                entry.priority = 2;
                meaning = meaning.Substring(0, meaning.Length - 4);
            }
            bool uk = false;
            string[] res = meaning.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < res.Length; ++i)
            {
                string rr = res[i].Trim();
                string buf = "";
                string tmp = GetAnnotation(ref rr);
                while (tmp != null)
                {
                    bool isPOS = false;
                    foreach (string pos in tmp.Split(','))
                    {
                        if (pos.StartsWith("v5") || Array.IndexOf(pos_list, pos) >= 0)
                        {
                            entry.AddPOS(pos);
                            isPOS = true;
                        }
                    }
                    if (!isPOS)
                    {
                        if (tmp == "uk")
                        {
                            uk = true;
                        }
                        else
                        {
                            if (tmp.Length > 7 || (tmp.Length >= 1 && tmp[0] >= '0' && tmp[0] <= '9'))
                                buf += "(" + tmp + ") ";
                            if (tmp.Length > 7)
                                break;
                        }
                    }
                    tmp = GetAnnotation(ref rr);
                }
                res[i] = buf + rr;
            }
            if (uk)
                entry.priority += 1;
            entry.meaning = res;
            /*if (entry.pos.Length > 0 && entry.meaning.Length > 0)
            {
                entry.meaning[entry.meaning.Length - 1] += " [" + string.Join(", ", entry.pos) + "]";
            }*/
        }

        public static string CleanMeaning(string meaning)
        {
            meaning = meaning.Trim();
            while (true)
            {
                string tmp = GetAnnotation(ref meaning);
                if (tmp == null)
                    break;
                if (tmp.Length > 7 && tmp.IndexOf(' ') >= 0)
                {
                    meaning = "(" + tmp + ") " + meaning;
                    break;
                }
            }
            if (meaning.EndsWith("(P)"))
                GetAnnotationLast(ref meaning);
            return meaning;
        }

        private EdictEntry[] LoadDict(string[] ss)
        {
            try
            {
                EdictEntry[] res = new EdictEntry[ss.Length - 1];
                for (int i = 1; i < ss.Length; ++i)
                {
                    string s = ss[i];
                    string[] part = s.Split(new char[] { '/' }, 2);
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
                    EdictEntry entry = new EdictEntry(key, reading);
                    if (part.Length >= 2)
                        ParseMeaning(part[1], entry);
                    else
                        ParseMeaning("", entry);
                    res[i - 1] = entry;
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
            Initialize();
        }

        public void Initialize()
        {
            try
            {
                Ready = true;
                dict = LoadDictUsingCache("edict", Encoding.GetEncoding("EUC-JP"), out rdict);
                if (dict == null)
                {
                    Ready = false;
                    return;
                }
                ReloadUserDictionary();
            }
            catch (Exception)
            {
                Ready = false;
            }        
        }

        public EdictEntry[] LoadDictUsingCache(string name, Encoding encoding, out EdictEntry[] rdict)
        {
            try
            {
                string file_name = GetRealFilename(name);
                string cache_name = file_name + ".cache.txt";
                if (File.GetLastWriteTime(file_name) >= File.GetLastWriteTime(cache_name))
                    throw new Exception();
                var sr = new StreamReader(new FileStream(cache_name, FileMode.Open, FileAccess.Read), Encoding.UTF8);
                try
                {
                    int n = int.Parse(sr.ReadLine());
                    EdictEntry[] res = new EdictEntry[n];
                    for (int i = 0; i < n; ++i)
                    {
                        res[i] = new EdictEntry();
                        res[i].LoadFrom(sr);
                    }
                    rdict = new EdictEntry[n];
                    int ctr = 0;
                    foreach (string s in sr.ReadLine().Split(','))
                    {
                        rdict[ctr++] = res[int.Parse(s)];
                    }
                    if (ctr != n)
                    {
                        //MessageBox.Show("Debug: size of dict and rdict do not match");
                        throw new Exception();
                    }
                    return res;
                }
                finally
                {
                    sr.Close();
                }
            }
            catch (Exception)
            {
                EdictEntry[] res = LoadDict(LoadDictText(name, encoding));
                rdict = (EdictEntry[])res.Clone();
                int[] nums = new int[res.Length];
                for (int i = 0; i < nums.Length; ++i)
                {
                    nums[i] = i;
                }
                Array.Sort(rdict, nums, new EdictEntry.ByReading());
                try
                {
                    SaveDictToCache(res, nums, name);
                }
                catch (Exception)
                { }
                return res;
            }
        }

        public void SaveDictToCache(EdictEntry[] dict, int[] rdictpos, string name)
        {
            name = GetRealFilename(name) + ".cache.txt";
            StreamWriter sw = new StreamWriter(new FileStream(name, FileMode.Create, FileAccess.Write), Encoding.UTF8);
            try
            {
                sw.WriteLine(dict.Length);
                foreach (EdictEntry entry in dict)
                    entry.SaveTo(sw);
                bool first = true;
                foreach (int pos in rdictpos)
                {
                    if (first)
                    {
                        sw.Write(pos);
                        first = false;
                    }
                    else
                    {
                        sw.Write(',');
                        sw.Write(pos);
                    }
                }
                sw.WriteLine();
            }
            finally
            {
                sw.Close();
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

        public string GetRealFilename(string filename)
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
            if (entry.meaning.Length == 0)
                return;
            res.Add(entry.key);
            res.Add(string.Join(", ", entry.pos));
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
            EdictEntry[] infl = Inflect.FindInflectedAll(key);
            foreach (EdictEntry entry in infl)
            {
                DictSearchAddItem(added, res, entry);
            }
            return res.ToArray();
        }

        private EdictEntry SearchExact(string key, string pos, bool second, bool byReading)
        {
            if (!Ready)
                return null;
            if (key.Length == 0 || key.Length == 1 && !Translation.isKanji(key[0]))
                return null;
            int x;
            if (!byReading)
            {
                if (user != null)
                {
                    x = BinarySearch(user, key);
                    while (x < user.Length && user[x].key == key)
                    {
                        EdictEntry entry = user[x];
                        if (!second && entry.meaning.Length > 0)
                        {
                            char ch = entry.meaning[0][0];
                            if (ch == '=')
                            {
                                return SearchExact(key, pos, true, true);
                            }
                            else if (char.GetUnicodeCategory(ch) == System.Globalization.UnicodeCategory.OtherLetter)
                            {
                                key = entry.meaning[0];
                                return SearchExact(key, pos, true, false);
                            }
                            else
                                return entry;
                        }
                        else
                        {
                            return entry;
                        }
                    }
                }
                x = BinarySearch(dict, key);
                while (x < dict.Length && dict[x].key == key)
                {
                    EdictEntry entry = dict[x];
                    if (pos == null || entry.isPOS(pos))
                        return entry;
                    ++x;
                }
            }
            if (key.ToCharArray().All(Translation.isKatakana))
                key = Translation.KatakanaToHiragana(key);
            if (key.ToCharArray().All(Translation.isHiragana))
            {
                x = BinarySearchByReading(rdict, key);
                string reading = Translation.KatakanaToHiragana(rdict[x].reading);
                while (x < rdict.Length && reading == key)
                {
                    EdictEntry entry = rdict[x];
                    if (pos == null || entry.isPOS(pos))
                    {
                        if (second || key.Length > 3 || key.Length > 2 && entry.priority >= 1 || entry.priority >= 3)
                            return entry;
                    }
                    ++x;
                    if (x < rdict.Length)
                        reading = Translation.KatakanaToHiragana(rdict[x].reading);
                }
            }
            return null;
        }

        public EdictEntry SearchExact(string key, string pos)
        {
            return SearchExact(key, pos, false, false);
        }

        public string VersionInfo()
        {
            try
            {
                string fn = GetRealFilename("edict");
                var fs = new FileStream(fn, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var sr = new StreamReader(fs, Encoding.GetEncoding("EUC-JP"));
                string s = sr.ReadLine();
                sr.Close();
                fs.Close();
                var ss = s.Split('/');
                return ss[ss.Length - 2].Trim() + ", " + (dict.Length + 1) + " entries";
            }
            catch (Exception)
            {
                return "(not available)";
            }
        }
    }
}
