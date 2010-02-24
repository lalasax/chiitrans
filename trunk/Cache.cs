using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace ChiiTrans
{
    public struct CacheKey
    {
        public string translator;
        public string language;

        public CacheKey(string translator, string language)
        {
            this.translator = translator;
            this.language = language;
        }
    }

    public class CacheRecord : Dictionary<CacheKey, string>
    {
    }
    
    public class Cache
    {
        Dictionary<string, CacheRecord> dict;
        string cacheFile;
        
        public Cache()
        {
            dict = new Dictionary<string, CacheRecord>();
            cacheFile = Path.Combine(Global.cfgdir, "cache.txt");
        }

        public void Load()
        {
            try
            {
                JsObject data = Json.Parse(File.ReadAllText(cacheFile));
                dict = new Dictionary<string, CacheRecord>();
                foreach (KeyValuePair<string, JsObject> item in data.dict)
                {
                    CacheRecord rec = new CacheRecord();
                    JsArray entryList = (JsArray)item.Value;
                    foreach (JsObject i2 in entryList)
                    {
                        JsArray entry = (JsArray)i2;
                        rec.Add(new CacheKey(i2.str[0], i2.str[1]), i2.str[2]);
                    }
                    dict.Add(item.Key, rec);
                }
            }
            catch (Exception)
            {
                //Form1.Debug(e.Message);
            }
        }

        public void Save()
        {
            try
            {
                StringBuilder res = new StringBuilder();
                res.Append("{");
                bool first = true;
                foreach (KeyValuePair<string, CacheRecord> rec in dict)
                {
                    if (first)
                        first = false;
                    else
                        res.Append(", ");
                    res.Append(Json.EscapeString(rec.Key));
                    res.Append(": [");
                    bool first2 = true;
                    foreach (KeyValuePair<CacheKey, string> rec2 in rec.Value)
                    {
                        if (first2)
                            first2 = false;
                        else
                            res.Append(", ");
                        res.Append(string.Format("[{0}, {1}, {2}]",
                            Json.EscapeString(rec2.Key.translator),
                            Json.EscapeString(rec2.Key.language),
                            Json.EscapeString(rec2.Value)));
                    }
                    res.Append("]");
                }
                res.Append("}");
                File.WriteAllText(cacheFile, res.ToString());
            }
            catch (Exception)
            {
                //Form1.Debug(e.Message);
            }
        }

        public string Find(string source, string translator, string language)
        {
            CacheRecord rec;
            if (dict.TryGetValue(source, out rec))
            {
                string result;
                if (rec.TryGetValue(new CacheKey(translator, language), out result))
                    return result;
            }
            return null;
        }

        public void Store(string source, string translator, string language, string result)
        {
            CacheRecord rec;
            if (!dict.TryGetValue(source, out rec))
            {
                rec = new CacheRecord();
                dict.Add(source, rec);
            }
            rec[new CacheKey(translator, language)] = result;
        }

        public void Clear()
        {
            dict.Clear();
        }
    }
}
