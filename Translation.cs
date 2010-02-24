using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Net;
using System.IO;
using System.Web;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Globalization;

namespace ChiiTrans
{
    class Translation
    {
        public static HashSet<Translation> current = new HashSet<Translation>();
        private static int transId = 0;
        public static string lastGoodBuffer = "";
        
        private string source;
        private string sourceNew;
        private string sourceFixed;
        public int id { get; private set; }
        public static string[] Translators = { "Translit (MeCab)", "Atlas", "Translit (Google)", "OCN", "Babylon", "Google", "SysTran", "Excite" };
        List<TranslationTask> tasks;
        private int tasksToComplete;
        private Options options;
        
        public Translation(int id, string source, Options _options)
        {
            options = _options;

            //long before = DateTime.Now.Ticks;
            this.id = id;
            this.source = source;
            this.sourceFixed = makeReplacements(source);
            if (options.excludeSpeakers)
                this.sourceNew = excludeSpeaker(sourceFixed);
            else
                this.sourceNew = sourceFixed;
            sourceNew = makeFinalAdjustments(sourceNew);
            //long passed = DateTime.Now.Ticks - before;
            //Form1.Debug(passed.ToString());
            List<string> usedTranslators = new List<string>();
            foreach (TranslatorRecord rec in options.translators)
            {
                if (rec.inUse)
                {
                    string trans = Translators[rec.id];
                    if (trans == "Atlas" && !Atlas.Ready())
                        continue;
                    if (trans == "Translit (MeCab)" && !Mecab.Ready())
                        continue;
                    usedTranslators.Add(Translators[rec.id]);
                }
            }
            List<object> args = new List<object>();
            args.Add(id);
            bool parseWords = options.displayOriginal && (options.wordParseMethod == Options.PARSE_MECAB && Mecab.Ready() || options.wordParseMethod == Options.PARSE_WWWJDIC);
            bool reserveLineHeight = parseWords && options.displayReadings;
            args.Add(reserveLineHeight);
            args.Add(options.furiganaRomaji);
            if (options.displayOriginal)
            {
                args.Add(source);
                if (options.displayFixed && source != sourceFixed)
                    args.Add(sourceFixed);
                else
                    args.Add("");
            }
            else
            {
                args.Add("");
                args.Add("");
            }
            foreach (string name in usedTranslators)
            {
                args.Add(name);
            }
            Global.RunScript2("AddTranslationBlock", args.ToArray());
            tasks = new List<TranslationTask>();
            if (parseWords)
                tasksToComplete = usedTranslators.Count + 1;
            else
                tasksToComplete = usedTranslators.Count;
            current.Add(this);
            foreach (string s in usedTranslators)
            {
                if (s == "Translit (MeCab)")
                    tasks.Add(new TranslationTask(this, s, this.GetType().GetMethod("TranslateMecabTranslit"), false));
                else if (s == "Translit (Google)")
                    tasks.Add(new TranslationTask(this, s, this.GetType().GetMethod("TranslateTranslit"), false));
                else
                    tasks.Add(new TranslationTask(this, s, this.GetType().GetMethod("Translate" + s), false));
            }
            if (parseWords)
            {
                if (options.wordParseMethod == Options.PARSE_MECAB)
                    tasks.Add(new TranslationTask(this, "Mecab", this.GetType().GetMethod("MecabLookup"), true));
                else
                    tasks.Add(new TranslationTask(this, "JDic", this.GetType().GetMethod("JDicLookup"), true));
            }
        }
        
        private string makeReplacements(string source)
        {
            string result = source;
            foreach (int ch in result)
            {
                if (ch >= 0xFF61 && ch <= 0xFF9F)
                {
                    // half-width kana
                    result = HalfWidth.Convert(result);
                    break;
                }
            }
            //full-width numbers
            StringBuilder sb = new StringBuilder();
            foreach (int ch in result)
            {
                if (ch >= 0xFF10 && ch <= 0xFF19)
                {
                    sb.Append((char)('0' + (ch - 0xFF10)));
                }
                else
                {
                    sb.Append((char)ch);
                }
            }
            result = sb.ToString();
            string sempai = null;
            string sensei = null;
            foreach (Replacement rep in options.replacements)
            {
                //suffixes hack
                if (rep.oldText == "先輩")
                    sempai = rep.newText;
                else if (rep.oldText == "先生")
                    sensei = rep.newText;
                else
                    result = replaceNormal(result, rep.oldText, rep.newText);
            }
            if (options.replaceSuffixes)
                result = replaceSuffixes(result);
            if (sempai != null)
                result = replaceNormal(result, "先輩", sempai);
            if (sensei != null)
                result = replaceNormal(result, "先生", sensei);
            return result;
        }

        public static bool isKatakana(char ch)
        {
            return (ch >= '\u30A0' && ch <= '\u30FF');
        }

        public static bool isHiragana(char ch)
        {
            return (ch >= '\u3040' && ch <= '\u309F');
        }

        public static bool isKanji(char ch)
        {
            return char.GetUnicodeCategory(ch) == UnicodeCategory.OtherLetter && !isKatakana(ch) && !isHiragana(ch);
        }

        public static string KatakanaToHiragana(string s)
        {
            StringBuilder res = new StringBuilder();
            foreach (char ch in s)
            {
                if (isKatakana(ch) && ch != '・')
                    res.Append((char)(ch - 0x60));
                else
                    res.Append(ch);
            }
            return res.ToString();
        }

        public static string HiraganaToRomaji(string s)
        {
            return HiraganaConvertor.instance.Convert(s);
        }
        
        private string replaceNormal(string source, string oldText, string newText)
        {
            if (oldText.Length == 0)
                return source;
            if (newText.Length > 0)
            {
                newText = newText.Replace(' ', '　');
                UnicodeCategory cat = char.GetUnicodeCategory(newText[newText.Length - 1]);
                if (cat == UnicodeCategory.UppercaseLetter || cat == UnicodeCategory.LowercaseLetter)
                    newText += "　";
                bool allKatakana = true;
                foreach (char ch in oldText)
                {
                    if (!isKatakana(ch))
                    {
                        allKatakana = false;
                    }
                }
                if (allKatakana)
                {
                    return Regex.Replace(source, "(?<![\\u30A1-\\u30FA])" + Regex.Escape(oldText) + "(?![\\u30A1-\\u30FA])", newText);
                }
            }
            return Regex.Replace(source, oldText, newText);
        }

        private string suffixReplace(string source, string to_find, string replacement)
        {
            Regex suf = new Regex("([A-Za-z])　" + to_find);
            return suf.Replace(source, match => match.Groups[1].Value + replacement + "　");
        }
        
        private string replaceSuffixes(string source)
        {
            string x = source;
            x = suffixReplace(x, "さん", "-san");
            x = suffixReplace(x, "くん", "-kun");
            x = suffixReplace(x, "ちゃん", "-chan");
            x = suffixReplace(x, "ちん", "-chin");
            x = suffixReplace(x, "せんぱい", "-sempai");
            x = suffixReplace(x, "センパイ", "-sempai");
            x = suffixReplace(x, "先輩", "-sempai");
            x = suffixReplace(x, "先生", "-sensei");
            x = suffixReplace(x, "っち", "cchi");
            x = suffixReplace(x, "様", "-sama");
            x = suffixReplace(x, "氏", "-shi");
            x = suffixReplace(x, "君", "-kun");
            x = suffixReplace(x, "殿", "-dono");
            return x;
        }

        private string excludeSpeaker(string source)
        {
            try
            {
                Match match = Regex.Match(source, options.excludeSpeakersPattern);
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
            }
            catch (Exception) { }
            return source;
        }

        private string makeFinalAdjustments(string source)
        {
            if (source.Length == 0)
                return source;
            source = Regex.Replace(source, "[っー]+(?!\\w)", "");
            source = source.Replace('『', '「')
                           .Replace('』', '」')
                           .Replace("…", "・・・")
                           .Replace("‥", "・・");
            source = Regex.Replace(source, "・+", "$0 ").Replace('・', '.');
            bool allKatakana = true;
            foreach (char ch in source)
            {
                if (char.GetUnicodeCategory(ch) == UnicodeCategory.OtherLetter && isHiragana(ch))
                {
                    allKatakana = false;
                }
            }
            if (allKatakana)
                source = KatakanaToHiragana(source);
            return source;
        }

        public void Abort()
        {
            foreach (TranslationTask task in tasks)
            {
                task.Abort();
            }
        }

        public void CompleteTask()
        {
            Interlocked.Add(ref tasksToComplete, -1);
            if (tasksToComplete <= 0)
                current.Remove(this);
        }

        private HttpWebRequest CreateHTTPRequest(string url)
        {
            HttpWebRequest result = (HttpWebRequest)WebRequest.Create(url);
            result.Proxy = null;
            result.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729)";
            result.Timeout = 10000;
            return result;
        }

        private string ReadAnswer(HttpWebRequest req)
        {
            return ReadAnswer(req, Encoding.UTF8);
        }

        private string ReadAnswer(HttpWebRequest req, Encoding encoding)
        {
            WebResponse res = req.GetResponse();
            return new StreamReader(req.GetResponse().GetResponseStream(), encoding).ReadToEnd();
        }

        private void WritePost(HttpWebRequest req, string data)
        {
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = data.Length;
            Stream ss = req.GetRequestStream();
            StreamWriter sw = new StreamWriter(ss);
            sw.Write(data);
            sw.Close();
            ss.Close();
        }
        
        private string UrlEncode(string data)
        {
            return HttpUtility.UrlEncode(data, Encoding.UTF8);
        }

        private string FindSubString(string source, string beginMark, string endMark)
        {
            int x = source.IndexOf(beginMark);
            if (x < 0)
                return null;
            x += beginMark.Length;
            int y = source.IndexOf(endMark, x);
            if (y < 0)
                return null;
            return source.Substring(x, y - x);
        }
        
        public string TranslateBabylon()
        {
            string url = "http://translation.babylon.com/post_post.php";
            string src = sourceNew.Replace('「', '\"').Replace('」', '\"');
            string query = "mytextarea1=" + UrlEncode(src) + "&lps=8&lpt=0";
            HttpWebRequest req = CreateHTTPRequest(url);
            WritePost(req, query);
            string res = ReadAnswer(req);
            if (res.ToLower().IndexOf(">internal server error<") >= 0)
                throw new Exception();
            return res;
        }

        public string TranslateTranslit()
        {
            string url = "http://translate.google.com/translate_a/t";
            string query = "client=t&text=" + UrlEncode(sourceFixed.Replace('　', ' ')) + "&sl=ja&tl=ja";
            HttpWebRequest req = CreateHTTPRequest(url + "?" + query);
            JsObject js = Json.Parse(ReadAnswer(req));
            string result = js["sentences"]["0"]["translit"].ToString();
            result = Regex.Replace(result, "~tsu ([A-Za-z])", match => match.Groups[1].Value + match.Groups[1].Value);
            result = result.Replace("~tsu", "");
            return result;
        }

        public string TranslateGoogle()
        {
            string url = "http://ajax.googleapis.com/ajax/services/language/translate";
            string srclang, destlang;
            if (options.translateToOtherLanguage)
            {
                destlang = options.translateLanguage;
                bool allLatin = true;
                foreach (char ch in sourceNew)
                {
                    if (char.IsLetter(ch) && !(ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z'))
                    {
                        allLatin = false;
                        break;
                    }
                }
                if (allLatin)
                {
                    srclang = "en";
                }
                else
                {
                    srclang = "ja";
                }
            }
            else
            {
                srclang = "ja";
                destlang = "en";
            }
            string query = "v=1.0&q=" + UrlEncode(sourceNew) + "&langpair=" + srclang + "%7C" + destlang;
            HttpWebRequest req = CreateHTTPRequest(url + "?" + query);
            //req.Referer = "http://127.0.0.1/";
            JsObject js = Json.Parse(ReadAnswer(req));
            /*StringBuilder sb = new StringBuilder();
            for (int i = 0; i < ((JsArray)js["sentences"]).length; ++i)
            {
                sb.Append(js["sentences"][i]["trans"].ToString());
            }
            return sb.ToString();*/
            return js["responseData"].str["translatedText"];
            //return js.Serialize();
        }

        public string TranslateOCN()
        {
            string url = "http://cgi01.ocn.ne.jp/cgi-bin/translation/index.cgi";
            string src = sourceNew.Replace('-', '‐');
            string query = "langpair=jaen&sourceText=" + UrlEncode(src);
            HttpWebRequest req = CreateHTTPRequest(url);
            WritePost(req, query);
            string result = ReadAnswer(req, Encoding.GetEncoding(932));
            Regex re = new Regex("NAME=\"responseText\".*?\\>(.*?)\\<\\/TEXTAREA\\>", RegexOptions.Singleline);
            Match m = re.Match(result);
            if (m.Success)
                return m.Groups[1].Value;
            else
                throw new Exception();
        }

        /*
        public string TranslateHonyaku()
        {
            string url = "http://honyaku.yahoo.co.jp/transtext";
            string query = "both=TH&eid=CR-JE&text=" + UrlEncode(sourceNew);
            HttpWebRequest req = CreateHTTPRequest(url);
            WritePost(req, query);
            string result = ReadAnswer(req);
            Regex re = new Regex("id=\"trn_textText\".*?\\>(.*?)\\<\\/textarea\\>", RegexOptions.Singleline);
            Match m = re.Match(result);
            if (m.Success)
                return m.Groups[1].Value;
            else
                throw new Exception();
        }
        */

        /*
        public string TranslateBabelFish()
        {
            string url = "http://babelfish.yahoo.com/translate_txt";
            //ei=UTF-8&doit=done&fr=bf-res&intl=1&tt=urltext&lp=ja_en&btnTrTxt=Translate&trtext=
            string query = "ei=UTF-8&fr=bf-badge&lp=ja_en&trtext=" + UrlEncode(sourceNew);
            HttpWebRequest req = CreateHTTPRequest(url);
            WritePost(req, query);
            string result = FindSubString(ReadAnswer(req), "<div id=\"result\"><div style=\"padding:0.6em;\">", "</div>");
            return result;
        }
         */

        public string TranslateSysTran()
        {
            string url = "http://www.systranet.com/tt?lp=ja_en&service=translate";
            HttpWebRequest req = CreateHTTPRequest(url);
            string src_s = sourceNew.Replace('\n', ' ');
            byte[] src = Encoding.UTF8.GetBytes(src_s);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = src.Length;
            Stream s = req.GetRequestStream();
            s.Write(src, 0, src.Length);
            s.Close();
            string result = ReadAnswer(req);
            int x = result.IndexOf("body=");
            if (x >= 0)
            {
                result = HttpUtility.UrlDecode(result.Substring(x + 5).Trim());
            }
            return result;
        }

        public string TranslateExcite()
        {
            string url = "http://www.excite.co.jp/world/english/";
            string query = "wb_lp=JAEN&after=start=+%96%7C+%96%F3+&before=" + HttpUtility.UrlEncode(sourceNew, Encoding.GetEncoding(932));
            HttpWebRequest req = CreateHTTPRequest(url);
            WritePost(req, query);
            string result = ReadAnswer(req, Encoding.GetEncoding(932));
            Regex re = new Regex("name=\"after\".*?\\>(.*?)\\<\\/textarea\\>", RegexOptions.Singleline);
            Match m = re.Match(result);
            if (m.Success)
                return m.Groups[1].Value;
            else
                throw new Exception();
        }

        public string TranslateAtlas()
        {
            return TranslateAtlas(sourceNew);
        }

        private string TranslateAtlas(string src)
        {
            src = Regex.Replace(src, "(?<!\\w)あ(?!\\w)", "ああ");
            src = Regex.Replace(src, "(?<!\\w)え(?!\\w)", "eh");
            StringBuilder res = new StringBuilder();
            StringBuilder buf = new StringBuilder();
            int i;
            for (i = 0; i < src.Length; ++i)
            {
                char ch = src[i];
                if (char.IsPunctuation(ch) && ch != '-' && ch != ',' && ch != '、' && ch != ';' && ch != '〜' && ch != ':' && ch != '：')
                {
                    if (buf.Length > 0)
                    {
                        bool is_stop = (ch == '.' || ch == '?' || ch == '!' || ch == '。' || ch == '！' || ch == '？');
                        if (is_stop)
                            buf.Append(ch);
                        string tran = Atlas.Translate(buf.ToString());
                        if (tran == null)
                            throw new Exception();
                        res.Append(tran);
                        if (!is_stop)
                            res.Append(ch);
                        res.Append(' ');
                        buf = new StringBuilder();
                    }
                    else
                    {
                        res.Append(ch);
                    }
                }
                else
                    buf.Append(ch);
            }
            if (buf.Length > 0)
            {
                string tran = Atlas.Translate(buf.ToString());
                if (tran == null)
                    throw new Exception();
                res.Append(tran);
            }
            return res.ToString().Trim().Replace('。', '.').Replace('！', '!').Replace('？', '?');
        }

        public string SecondTranslate(string source, string lang)
        {
            if (lang == "ru" && Global.options.usePromt)
            {
                string url = "http://m.translate.ru/translator/result/";
                string query = "text=" + UrlEncode(source) + "&dirCode=er";
                HttpWebRequest req = CreateHTTPRequest(url + "?" + query);
                string result = ReadAnswer(req);
                Regex re = new Regex("class=\"tres\"\\>(.*?)\\<\\/div\\>", RegexOptions.Singleline);
                Match m = re.Match(result);
                if (m.Success)
                    return m.Groups[1].Value;
                else
                    throw new Exception();
            }
            else
            {
                string url = "http://ajax.googleapis.com/ajax/services/language/translate";
                string query = "v=1.0&q=" + UrlEncode(source) + "&langpair=en%7C" + lang;
                HttpWebRequest req = CreateHTTPRequest(url + "?" + query);
                JsObject js = Json.Parse(ReadAnswer(req));
                return js["responseData"].str["translatedText"];
            }
        }

        private class JDicLookupSentenceRecord
        {
            public bool word;
            public string text;

            public JDicLookupSentenceRecord(bool word, string text)
            {
                this.word = word;
                this.text = text;
            }
        }
        
        private List<JDicLookupSentenceRecord> parseSentence(string s)
        {
            List<JDicLookupSentenceRecord> a = new List<JDicLookupSentenceRecord>();
            while (s.Length > 0)
            {
                if (s.Length >= 5 && s.Substring(0, 5).ToUpper() == "<FONT")
                {
                    int x = s.IndexOf(">");
                    int y = s.ToUpper().IndexOf("</FONT>");
                    string inner = s.Substring(x + 1, y - (x + 1));
                    a.Add(new JDicLookupSentenceRecord(true, inner));
                    s = s.Substring(y + 7);
                }
                else
                {
                    int x = s.ToUpper().IndexOf("<FONT");
                    string ss;
                    if (x < 0)
                    {
                        ss = s;
                        s = "";
                    }
                    else
                    {
                        ss = s.Substring(0, x);
                        s = s.Substring(x);
                    }
                    a.Add(new JDicLookupSentenceRecord(false, ss));
                }
            }
            return a;
        }

        private class JDicLookupDefinitionRecord
        {
            public string key;
            public string reading;
            public string meaning;
        }
        
        private List<JDicLookupDefinitionRecord> parseDefinitions(string text)
        {
            int i = 0;
            List<JDicLookupDefinitionRecord> res = new List<JDicLookupDefinitionRecord>();
            while (i < text.Length)
            {
                int x = text.IndexOf("<li>", i);
                if (x < 0)
                    break;
                int y = text.IndexOf("</li>", x);
                if (y < 0)
                    break;
                string s = text.Substring(x + 5, y - (x + 5));
                res.Add(parseDefinition(s));
                i = y + 4;
            }
            return res;
        }

        private JDicLookupDefinitionRecord parseDefinition(string s)
        {
            JDicLookupDefinitionRecord res = new JDicLookupDefinitionRecord();
            if (s.Length >= 8 && s.Substring(0, 8) == "Possible")
            {
                int br = s.IndexOf("<br>");
                if (br >= 0)
                {
                    s = s.Substring(br + 4);
                }
            }
            int beg = s.IndexOf(" ");
            if (beg < 0)
            {
                res.key = s;
                res.meaning = "";
                res.reading = "";
            }
            else
            {
                res.key = s.Substring(0, beg);
                s = s.Substring(beg + 1);
                beg = s.IndexOf('【');
                if (beg < 0)
                {
                    res.reading = res.key;
                    res.meaning = s;
                }
                else
                {
                    int end = s.IndexOf('】', beg);
                    res.reading = s.Substring(beg + 1, end - (beg + 1)).Trim();
                    res.meaning = s.Substring(end + 1).Trim();
                }
            }
            return res;
        }
        
        private readonly string[] JDicCodes =
            { "AV", "BU", "CA", "CC", "CO", "ED", "EP", "ES", "EV", "FM", "FO", "GE", "KD", "LG", "LS", "LW1/2", "MA",
                "NA", "PL", "PP", "RH", "RW", "SP", "ST", "WI1/2"};

        private readonly Regex htmlKiller = new Regex(@"<.+?>.*?</.+?>");

        private string JDicParse(string s1, string s2)
        {
            List<JDicLookupSentenceRecord> parts = parseSentence(s1);
            List<JDicLookupDefinitionRecord> defs = parseDefinitions(s2);

            List<string> res = new List<string>();
            int def_ctr = 0;
            foreach (JDicLookupSentenceRecord part in parts)
            {
                if (!part.word)
                    continue;
                JDicLookupDefinitionRecord cur = defs[def_ctr++];
                res.Add(part.text);
                res.Add(formatReading(part.text, cur.reading == "" ? "" : cur.reading.Split(new string[] {"; "}, StringSplitOptions.None)[0]));
                res.Add(cur.key);
                res.Add(formatReading(cur.key, cur.reading));
                List<string> mm = new List<string>();
                foreach (string m in cur.meaning.Split(new string[] { "; " }, StringSplitOptions.None))
                {
                    string newm = Edict.CleanMeaning(m);
                    newm = htmlKiller.Replace(newm, "").Trim();
                    if (newm != "" && Array.IndexOf(JDicCodes, newm) < 0)
                        mm.Add(newm);
                }
                res.Add(formatMeaning(mm.ToArray()));
            }
            //Form1.Debug(string.Join("\r", res.ToArray()));
            return string.Join("\r", res.ToArray());
        }
        
        public void JDicLookup()
        {
            try
            {
                if (options.useCache)
                {
                    string cached = Global.cache.Find(source, "WWWJDIC", options.furiganaRomaji ? "r" : "h");
                    if (cached != null)
                    {
                        Global.RunScript("UpdateWords", id, cached, TranslationTask.FROM_CACHE);
                        return;
                    }
                }

                string url = options.JDicServer;
                string query = "9MIG" + HttpUtility.UrlEncode(source);
                HttpWebRequest req = CreateHTTPRequest(url + "?" + query);
                string result = ReadAnswer(req, Encoding.GetEncoding(20932));
                string beginMark = "<font size=\"-3\">&nbsp;</font><br>\n<br>\n";
                string endMark = "<br>\n<p>";
                int start = result.IndexOf(beginMark);
                if (start < 0)
                    return;
                start += beginMark.Length;
                int end = result.IndexOf(endMark, start);
                string ss1 = "";
                string ss2 = "";
                while (true)
                {
                    int fin1 = result.IndexOf("<br>", start);
                    if (fin1 > end)
                        break;
                    string s1 = result.Substring(start, fin1 - start);
                    if (s1.ToUpper().IndexOf("<FONT") < 0)
                    {
                        ss1 += s1;
                        start = fin1 + 5;
                    }
                    else
                    {
                        int x = result.IndexOf("<ul>", start);
                        if (x < 0)
                            break;
                        int y = result.IndexOf("</ul>", x + 4);
                        if (y < 0)
                            break;
                        string s2 = result.Substring(x + 4, y - (x + 4));
                        ss1 += s1;
                        ss2 += s2;
                        start = y + 5;
                    }
                }
                //Form1.Debug(ss1 + "\r\n" + ss2);
                //Form1.Debug(result);
                if (ss1 != "" && ss2 != "")
                {
                    string res = JDicParse(ss1, ss2);
                    Global.RunScript("UpdateWords", id, res, TranslationTask.COMPLETED);
                    if (options.useCache)
                    {
                        Global.cache.Store(source, "WWWJDIC", options.furiganaRomaji ? "r" : "h", res);
                    }
                }
            }
            catch (Exception) 
            {
            }
        }

        public string TranslateMecabTranslit()
        {
            string src = sourceFixed.Replace('　', ' ');
            string data = Mecab.Translate(src);
            data = data.Replace("\r", "");
            string[] ss = data.Split('\n');
            StringBuilder res = new StringBuilder();
            foreach (string s in ss)
            {
                if (s == "EOS")
                    break;
                string[] dd = s.Split(new char[] { '\t' }, 2);
                string key = dd[0];
                if (key == "")
                    key = " ";
                dd = dd[1].Split(',');
                bool haveLettersOrDigits = false;
                foreach (char ch in key)
                {
                    if (char.IsLetterOrDigit(ch))
                    {
                        haveLettersOrDigits = true;
                        break;
                    }
                }
                string chunk;
                if (dd.Length >= 9)
                    chunk = dd[8];
                else
                    chunk = key;
                chunk = KatakanaToHiragana(chunk);
                if (haveLettersOrDigits && chunk.Length > 0 && "ぁぃぅぇぉゃゅょ゜".IndexOf(chunk[0]) < 0)
                    res.Append(' ');
                res.Append(chunk);
            }
            return HiraganaToRomaji(res.ToString());
        }

        private class MecabLookupRecord
        {
            public string key;
            public string reading;

            public MecabLookupRecord(string key, string reading)
            {
                this.key = key;
                this.reading = reading;
            }
        }

        private int countScore(string s, out EdictEntry e)
        {
            if (s == "")
            {
                e = null;
                return -1000;
            }
            bool onlyABC = true;
            foreach (char ch in s)
            {
                if (char.IsWhiteSpace(ch))
                    continue;
                UnicodeCategory cat = char.GetUnicodeCategory(ch);
                if (cat != UnicodeCategory.UppercaseLetter && cat != UnicodeCategory.LowercaseLetter)
                {
                    onlyABC = false;
                    break;
                }
            }
            if (onlyABC)
            {
                s = s.Trim();
                e = new EdictEntry("", s, new string[] { s });
                return s.Length * 1000;
            }
            else
            {
                e = Edict.instance.Search(s);
                if (e == null)
                    return -1;
                else
                {
                    int res = 0;
                    for (int i = 0; i < s.Length; ++i)
                    {
                        if (i >= e.key.Length)
                            break;
                        if (s[i] == e.key[i])
                            ++res;
                        else
                            break;
                    }
                    return res;
                }
            }
        }

        private string formatMeaning(string[] meaning)
        {
            string tmp = string.Join("; ", meaning);
            string tr = Regex.Replace(tmp, @"(?:^|; )\(\d+\)", "<li>");
            if (tr != tmp)
            {
                tr = "<ol>" + tr + "</ol>";
            }
            return tr;
        }

        private string formatReading(string key, string reading)
        {
            bool hasKana = false;
            bool hasKanji = false;
            foreach (char ch in key)
            {
                if (char.GetUnicodeCategory(ch) == UnicodeCategory.OtherLetter)
                {
                    hasKana = true;
                    if (isKanji(ch))
                    {
                        hasKanji = true;
                        break;
                    }
                }
            }
            if (!hasKana)
                return "";
            if (options.furiganaRomaji)
            {
                reading = HiraganaConvertor.instance.Convert(KatakanaToHiragana(reading));
                if (reading.Length > 0 && char.IsUpper(reading[0]))
                    reading = char.ToLower(reading[0]) + reading.Substring(1);
            }
            else
            {
                if (!hasKanji)
                    return "";
                reading = KatakanaToHiragana(reading);
            }
            return reading;
        }

        private bool isWord(string s)
        {
            bool hasLetters = false;
            foreach (char ch in s)
            {
                if (char.IsLetter(ch))
                {
                    hasLetters = true;
                    if (isKanji(ch))
                        return true;
                }
            }
            return hasLetters && s.Length >= 3;
        }
        
        private string MecabLookupTranslateWords(List<MecabLookupRecord> list)
        {
            List<string> res = new List<string>();
            int i = 0;
            while (i < list.Count)
            {
                string s = list[i].key;
                string s2;
                if (i + 1 < list.Count)
                {
                    s2 = s + list[i + 1].key;
                }
                else
                {
                    s2 = "";
                }
                s = makeReplacements(s);
                s2 = makeReplacements(s2);
                EdictEntry e, e2;
                int score = countScore(s, out e);
                int score2 = countScore(s2, out e2);
                if (score2 > score)
                {
                    string key = list[i].key + list[i + 1].key;
                    res.Add(key);
                    if (e2 != null && e2.key == "")
                    {
                        res.Add(e2.reading);
                        res.Add("");
                        res.Add("");
                        res.Add(formatMeaning(e2.meaning));
                    }
                    else
                    {
                        if (e2 != null)
                        {
                            if (e2.key == key)
                                res.Add(formatReading(e2.key, e2.reading));
                            else
                                res.Add(formatReading(key, list[i].reading + list[i + 1].reading));
                            res.Add(e2.key);
                            res.Add(formatReading(e2.key, e2.reading));
                            res.Add(formatMeaning(e2.meaning));
                        }
                        else
                        {
                            res.Add(formatReading(key, list[i].reading + list[i + 1].reading));
                            res.Add("");
                            res.Add("");
                            res.Add(isWord(key) ? "-" : "");
                        }
                    }
                    i += 2;
                }
                else
                {
                    res.Add(list[i].key);
                    if (e != null && e.key == "")
                    {
                        res.Add(e.reading);
                        res.Add("");
                        res.Add("");
                        res.Add(formatMeaning(e.meaning));
                    }
                    else
                    {
                        res.Add(formatReading(list[i].key, list[i].reading));
                        if (e != null)
                        {
                            res.Add(e.key);
                            res.Add(formatReading(e.key, e.reading));
                            res.Add(formatMeaning(e.meaning));
                        }
                        else
                        {
                            res.Add("");
                            res.Add("");
                            res.Add(isWord(list[i].key) ? "-" : "");
                        }
                    }
                    ++i;
                }
            }
            //Form1.Debug(string.Join("\r", res.ToArray()));
            return string.Join("\r", res.ToArray());
        }
        
        public void MecabLookup()
        {
            try
            {
                if (options.useCache)
                {
                    string cached = Global.cache.Find(source, "Mecab", options.furiganaRomaji ? "r" : "h");
                    if (cached != null)
                    {
                        Global.RunScript("UpdateWords", id, cached, TranslationTask.FROM_CACHE);
                        return;
                    }
                }
                if (!Mecab.Ready())
                    throw new Exception();
                string data = Mecab.Translate(source);
                if (data == null)
                    throw new Exception();
                data = data.Replace("\r", "");
                string[] ss = data.Split('\n');
                List<MecabLookupRecord> res = new List<MecabLookupRecord>();
                foreach (string s in ss)
                {
                    if (s == "EOS")
                        break;
                    string[] dd = s.Split(new char[] { '\t' }, 2);
                    if (dd[0] == "")
                    {
                        if (dd[1][0] == '\t')
                        {
                            dd[0] = "\t";
                            dd[1] = dd[1].Substring(1);
                        }
                        else
                        {
                            dd[0] = "\n";
                        }
                    }
                    string key = dd[0];
                    string reading = "";
                    dd = dd[1].Split(',');
                    if (dd.Length >= 8)
                    {
                        reading = KatakanaToHiragana(dd[7]);
                    }
                    res.Add(new MecabLookupRecord(key, reading));
                }
                string result = MecabLookupTranslateWords(res);
                Global.RunScript("UpdateWords", id, result, TranslationTask.COMPLETED);
                if (options.useCache)
                {
                    Global.cache.Store(source, "Mecab", options.furiganaRomaji ? "r" : "h", result);
                }
            }
            catch (Exception) 
            {
            }
        }

        private string TranslateAtlasInline(string src)
        {
            string result;
            try
            {
                string name = "Atlas";
                if (Global.options.useCache)
                {
                    string res = Global.cache.Find(src, name, getLanguageForCache());
                    if (res != null)
                    {
                        return res;
                    }
                }
                result = TranslateAtlas(src);
                if (Global.options.translateToOtherLanguage)
                {
                    result = SecondTranslate(result, Global.options.translateLanguage);
                }
                if (Global.options.useCache)
                {
                    Global.cache.Store(src, name, getLanguageForCache(), result);
                }
            }
            catch (Exception)
            {
                result = "";
            }
            return result;
        }

        private string getLanguageForCache()
        {
            if (options.translateToOtherLanguage)
            {
                if (options.usePromt && options.translateLanguage == "ru")
                    return "ru_promt";
                else
                    return options.translateLanguage;
            }
            else
                return "";
        }
        
        public string findCache(string taskName)
        {
            return Global.cache.Find(sourceNew, taskName, getLanguageForCache());
        }

        public void storeCache(string taskName, string result)
        {
            Global.cache.Store(sourceNew, taskName, getLanguageForCache(), result);
        }

        public static void Translate(string raw_source, Options options)
        {
            if (options == null)
                options = Global.options;
            if (raw_source == "")
                return;
            if (raw_source.Length > options.maxSourceLength * 2)
                raw_source = raw_source.Substring(0, options.maxSourceLength * 2);
            string source = GetSource(raw_source, options);
            if (source != null && source != "")
            {
                AddTranslationBlock(source, options);
            }
        }

        private static string GetSource(string raw_text, Options options)
        {
            string result = raw_text;
            if (result.ToCharArray().All(ch => !char.IsLetter(ch)))
                return null;
            lastGoodBuffer = raw_text;
            if (options.checkDouble)
                result = CheckDouble(result);
            if (options.checkRepeatingPhrases)
            {
                List<string> rep;
                int n;
                rep = Djon.GetRepeatingPhrases(result, 10, out n);
                if (n > 1)
                {
                    result = string.Join("。", rep.ToArray());
                }
            }
            if (result.Length > options.maxSourceLength)
                result = result.Substring(0, options.maxSourceLength);
            result = result.Trim();
            return result;
        }

        private static void AddTranslationBlock(string source, Options options)
        {
            if (current.Count < 10)
                new Translation(transId++, source, options);
        }

        private static string CheckDouble(string text)
        {
            int i = 0;
            int num = text.Length; // supposed number of repeating chars
            HashSet<int> lens = new HashSet<int>();
            while (i < text.Length)
            {
                char ch = text[i];
                if (char.IsWhiteSpace(ch))
                {
                    i += 1;
                    continue;
                }
                int j = 1;
                while (i + j < text.Length && text[i + j] == ch)
                    ++j;
                if (j == 1)
                    return text; //not double
                if (j < num)
                    num = j;
                lens.Add(j);
                i += num;
            }
            foreach (int len in lens)
            {
                if (len % num != 0)
                    return text; //not double, lol
            }
            StringBuilder res = new StringBuilder();
            i = 0;
            while (i < text.Length)
            {
                char ch = text[i];
                res.Append(ch);
                if (char.IsWhiteSpace(ch))
                {
                    i += 1;
                    int di = 1;
                    while (i < text.Length && char.IsWhiteSpace(text[i]) && di < num)
                    {
                        ++i;
                        ++di;
                    }
                }
                else
                    i += num;
            }
            return res.ToString(); //double.. or triple, or quadruple...
        }
    }
}

// こんにちは、世界！
