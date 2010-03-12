using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Reflection;

namespace ChiiTrans
{
    public class TranslatorRecord
    {
        public int id;
        public bool inUse;

        public TranslatorRecord(int id, bool inUse)
        {
            this.id = id;
            this.inUse = inUse;
        }
    }

    public class Replacement
    {
        public string oldText { get; set; }
        public string newText { get; set; }

        public Replacement(string oldText, string newText)
        {
            this.oldText = oldText;
            this.newText = newText;
        }

        public Replacement()
        {
            this.oldText = "";
            this.newText = "";
        }
    }

    public class ReplacementList : List<Replacement>
    {
        public static int SortByOldText(Replacement a, Replacement b)
        {
            return a.oldText.CompareTo(b.oldText);
        }

        public static int SortByNewText(Replacement a, Replacement b)
        {
            return a.newText.CompareTo(b.newText);
        }
    }

    public class ColorRecord
    {
        public string name;
        public Color color;

        public ColorRecord(string name, Color color)
        {
            this.name = name;
            this.color = color;
        }
    }

    public class Options
    {
        public const string VersionStamp = "ChiiTrans v.1.0";
        public const int PARSE_NONE = 0;
        public const int PARSE_BUILTIN = 1;
        public const int PARSE_WWWJDIC = 2;
        
        public List<TranslatorRecord> translators;
        public Dictionary<string, ColorRecord> colors;
        public ReplacementList replacements;

        public int wordParseMethod;
        public string JDicServer;
        public bool alwaysOnTop;
        public bool checkDouble;
        public bool replaceSuffixes;
        public bool excludeSpeakers;
        public string excludeSpeakersPattern;
        public int messageDelay;
        public int maxSourceLength;
        public Font font;
        public bool translateToOtherLanguage;
        public string translateLanguage;
        public bool useCache;
        public bool checkRepeatingPhrases;
        public bool displayOriginal;
        public int bottomLayerOpacity;
        public bool displayFixed;
        public bool displayReadings;
        public bool appendBottom;
        public bool dropShadow;
        public bool usePromt;
        public bool furiganaRomaji;
        public int maxBlocks;
        public bool largeMargins;
        public string hivemindServer;
        public bool toolbarVisible;

        private Type OptionsType;
        
        public Options()
        {
            OptionsType = GetType();
        }

        public Options Clone()
        {
            Options res = (Options)this.MemberwiseClone();
            res.translators = new List<TranslatorRecord>();
            foreach (TranslatorRecord rec in translators)
            {
                res.translators.Add(new TranslatorRecord(rec.id, rec.inUse));
            }
            res.replacements = new ReplacementList();
            foreach (Replacement rep in replacements)
            {
                res.replacements.Add(new Replacement(rep.oldText, rep.newText));
            }
            res.colors = new Dictionary<string, ColorRecord>();
            foreach (KeyValuePair<string, ColorRecord> kvp in colors)
            {
                res.colors.Add(kvp.Key, new ColorRecord(kvp.Value.name, kvp.Value.color));
            }
            res.font = (Font)font.Clone();
            return res;
        }

        public void SetDefaultColors()
        {
            colors = new Dictionary<string, ColorRecord>();
            colors["text"] = new ColorRecord("Text", Color.Black);
            colors["back"] = new ColorRecord("Background", Color.FromArgb(0xF8, 0xF8, 0xF0));
            colors["trans_name"] = new ColorRecord("Translator names", Color.FromArgb(0x99, 0x99, 0x99));
            colors["fixed"] = new ColorRecord("Fixed text", Color.Gray);
            colors["highlight1"] = new ColorRecord("Words highlight 1", Color.FromArgb(0xD8, 0xFF, 0xD8));
            colors["highlight2"] = new ColorRecord("Words highlight 2", Color.FromArgb(0xD8, 0xD8, 0xFF));
            colors["hr"] = new ColorRecord("Horizontal lines", Color.FromArgb(0xDD, 0xDD, 0xDD));
            colors["text_tmode"] = new ColorRecord("Text (transparent mode)", Color.White);
            colors["back_tmode"] = new ColorRecord("Background (transparent mode)", Color.Black);
            colors["trans_name_tmode"] = new ColorRecord("Translator names (transparent mode)", Color.Gray);
            colors["fixed_tmode"] = new ColorRecord("Fixed text (transparent mode)", Color.Gray);
        }

        public void SetDefault()
        {
            translators = new List<TranslatorRecord>();
            for (int i = 0; i < Translation.Translators.Length; ++i)
                translators.Add(new TranslatorRecord(i, i != 8)); //hivemind off by default

            SetDefaultColors();

            wordParseMethod = PARSE_BUILTIN;
            JDicServer = "http://www.csse.monash.edu.au/~jwb/cgi-bin/wwwjdic.cgi";
            alwaysOnTop = false;
            checkDouble = true;
            checkRepeatingPhrases = true;
            replaceSuffixes = true;
            excludeSpeakers = false;
            excludeSpeakersPattern = "^.+?「(.*?)」$";
            messageDelay = 300;
            maxSourceLength = 250;
            translateLanguage = "ru";
            translateToOtherLanguage = false;
            useCache = true;
            font = new Font("Arial", 12);
            displayOriginal = true;
            displayFixed = false;
            bottomLayerOpacity = 50;
            displayReadings = true;
            appendBottom = true;
            dropShadow = false;
            usePromt = false;
            furiganaRomaji = false;
            maxBlocks = 50;
            largeMargins = false;
            hivemindServer = "http://chii.sorakake.ru/";
            toolbarVisible = true;

            replacements = new ReplacementList();
        }

        public void Save()
        {
            SaveOptions();
            SaveReplacements();
        }

        public void Load()
        {
            try
            {
                LoadOptions(Path.Combine(Global.cfgdir, "options.txt"));
            }
            catch (Exception) { }
            try
            {
                LoadReplacements(Path.Combine(Global.cfgdir, "replacements.txt"));
            }
            catch (Exception)
            {
                replacements = new ReplacementList();
            }
        }

        private void loadOpt(JsObject data, string option)
        {
            if (!data.dict.ContainsKey(option))
                return;
            FieldInfo opt = OptionsType.GetField(option);
            Type optType = opt.FieldType;
            if (optType == typeof(string))
                opt.SetValue(this, data.str[option]);
            else if (optType == typeof(int))
                opt.SetValue(this, data.num[option]);
            else if (optType == typeof(bool))
                opt.SetValue(this, data.num[option] != 0);
        }

        private void saveOpt(JsObject data, string option)
        {
            FieldInfo opt = OptionsType.GetField(option);
            Type optType = opt.FieldType;
            if (optType == typeof(string))
                data.str[option] = (string)opt.GetValue(this);
            else if (optType == typeof(int))
                data.num[option] = (int)opt.GetValue(this);
            else if (optType == typeof(bool))
                data.num[option] = (bool)opt.GetValue(this) ? 1 : 0;
        }
        
        public void LoadOptions(string filename)
        {
            SetDefault();
            JsObject data = Json.Parse(File.ReadAllText(filename));
            
            JsArray tr = (JsArray)data["translators"];
            if (tr.length == Translation.Translators.Length)
            {
                translators = new List<TranslatorRecord>();
                for (int i = 0; i < tr.length; ++i)
                {
                    translators.Add(new TranslatorRecord(tr[i].num["id"], tr[i].num["inUse"] != 0));
                }
            }

            if (data.dict.ContainsKey("colors"))
            {
                foreach (string key in data["colors"].dict.Keys)
                {
                    if (colors.ContainsKey(key))
                        colors[key].color = Color.FromArgb(data["colors"].num[key]);
                }
            }

            loadOpt(data, "wordParseMethod");
            loadOpt(data, "JDicServer");
            if (JDicServer.EndsWith("?9U"))
                JDicServer = JDicServer.Substring(0, JDicServer.Length - 3);
            loadOpt(data, "alwaysOnTop");
            loadOpt(data, "checkDouble");
            loadOpt(data, "checkRepeatingPhrases");
            loadOpt(data, "replaceSuffixes");
            loadOpt(data, "excludeSpeakers");
            loadOpt(data, "excludeSpeakersPattern");
            loadOpt(data, "messageDelay");
            loadOpt(data, "maxSourceLength");
            try
            {
                font = (Font)(new FontConverter().ConvertFromString(data.str["font"]));
            }
            catch (Exception)
            {
            }
            loadOpt(data, "translateToOtherLanguage");
            loadOpt(data, "translateLanguage");
            loadOpt(data, "useCache");
            loadOpt(data, "displayOriginal");
            loadOpt(data, "displayFixed");
            loadOpt(data, "bottomLayerOpacity");
            loadOpt(data, "displayReadings");
            loadOpt(data, "appendBottom");
            loadOpt(data, "dropShadow");
            loadOpt(data, "usePromt");
            loadOpt(data, "furiganaRomaji");
            loadOpt(data, "maxBlocks");
            loadOpt(data, "largeMargins");
            loadOpt(data, "hivemindServer");
            loadOpt(data, "toolbarVisible");
        }

        public void SaveOptions(string filename)
        {
            JsObject data = new JsObject();
            JsArray arr = new JsArray();
            for (int i = 0; i < translators.Count; ++i)
            {
                arr.Add(Json.Parse("{\"id\":" + translators[i].id + ", \"inUse\":" + (translators[i].inUse ? "1" : "0") + "}"));
            }
            data["translators"] = arr;

            foreach (KeyValuePair<string, ColorRecord> kvp in colors)
            {
                data["colors"].num[kvp.Key] = kvp.Value.color.ToArgb();
            }

            saveOpt(data, "wordParseMethod");
            saveOpt(data, "JDicServer");
            saveOpt(data, "alwaysOnTop");
            saveOpt(data, "checkDouble");
            saveOpt(data, "checkRepeatingPhrases");
            saveOpt(data, "replaceSuffixes");
            saveOpt(data, "excludeSpeakers");
            saveOpt(data, "excludeSpeakersPattern");
            saveOpt(data, "messageDelay");
            saveOpt(data, "maxSourceLength");
            data.str["font"] = new FontConverter().ConvertToString(font);
            saveOpt(data, "translateToOtherLanguage");
            saveOpt(data, "translateLanguage");
            saveOpt(data, "useCache");
            saveOpt(data, "displayOriginal");
            saveOpt(data, "displayFixed");
            saveOpt(data, "bottomLayerOpacity");
            saveOpt(data, "displayReadings");
            saveOpt(data, "appendBottom");
            saveOpt(data, "dropShadow");
            saveOpt(data, "usePromt");
            saveOpt(data, "furiganaRomaji");
            saveOpt(data, "maxBlocks");
            saveOpt(data, "largeMargins");
            saveOpt(data, "hivemindServer");
            saveOpt(data, "toolbarVisible");

            File.WriteAllText(filename, data.Serialize());
        }

        public void LoadReplacements(string filename)
        {
            replacements = new ReplacementList();

            string[] lines = File.ReadAllLines(filename);
            int start = 0;
            if (lines.Length > 0 && lines[0] == VersionStamp)
            {
                start = 1;
            }
            for (int i = start; i < lines.Length; i += 2)
            {
                replacements.Add(new Replacement(lines[i], lines[i + 1]));
            }
        }

        public void SaveReplacements()
        {
            try
            {
                SaveReplacements(Path.Combine(Global.cfgdir, "replacements.txt"));
            }
            catch (Exception)
            {
            }
        }

        public void SaveOptions()
        {
            try
            {
                SaveOptions(Path.Combine(Global.cfgdir, "options.txt"));
            }
            catch (Exception)
            {
            }
        }
        
        public void SaveReplacements(string filename)
        {
            RemoveDuplicateReplacements();
            using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(VersionStamp);
                for (int i = 0; i < replacements.Count; ++i)
                {
                    sw.WriteLine(replacements[i].oldText);
                    sw.WriteLine(replacements[i].newText);
                }
            }
        }

        public void RemoveDuplicateReplacements()
        {
            ReplacementList res = new ReplacementList();
            HashSet<string> existing = new HashSet<string>();
            foreach (Replacement rep in replacements.Reverse<Replacement>())
            {
                if (existing.Add(rep.oldText))
                    res.Add(rep);
            }
            res.Reverse();
            replacements = res;
        }
    }
}
