using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ChiiTrans
{
    struct Pair<T1, T2>
    {
        public T1 first;
        public T2 second;
        public Pair(T1 _first, T2 _second)
        {
            first = _first;
            second = _second;
        }
    }
    
    class Json
    {
        
        public static JsObject Parse(string str)
        {
            var tokens = Tokenize(str);
            var res = ParseObject(tokens, 0);
            if (res.second != tokens.Count)
                throw new JsonParseException();
            return res.first;
        }

        static Pair<JsObject, int> ParseObject(List<Token> tokens, int pos)
        {
            if (pos >= tokens.Count)
                throw new JsonParseException();
            var token = tokens[pos];
            switch (token.type)
            {
                case TokenType.L_CURLY:
                    return ParseDict(tokens, pos + 1);
                case TokenType.L_SQUARE:
                    return ParseArray(tokens, pos + 1);
                case TokenType.STRING:
                    return new Pair<JsObject, int>(new JsAtom(token.value), pos + 1);
                default:
                    throw new JsonParseException();
            }
        }

        static Pair<JsObject, int> ParseDict(List<Token> tokens, int pos)
        {
            var res = new JsObject();
            bool first = true;
            while (pos < tokens.Count)
            {
                if (tokens[pos].type == TokenType.R_CURLY)
                    return new Pair<JsObject,int>(res, pos + 1);
                if (first)
                {
                    first = false;
                }
                else
                {
                    /*if (tokens[pos].type != TokenType.COMMA)
                        throw new JsonParseException();
                    pos += 1;*/
                    if (tokens[pos].type == TokenType.COMMA)
                        pos += 1;
                }
                if (pos >= tokens.Count || tokens[pos].type != TokenType.STRING)
                    throw new JsonParseException();
                string key = tokens[pos].value;
                pos += 1;
                if (pos >= tokens.Count || tokens[pos].type != TokenType.COLON)
                    throw new JsonParseException();
                pos += 1;
                var cur = ParseObject(tokens, pos);
                pos = cur.second;
                res[key] = cur.first;
            }
            throw new JsonParseException();
        }

        static Pair<JsObject, int> ParseArray(List<Token> tokens, int pos)
        {
            var res = new JsArray();
            bool first = true;
            while (pos < tokens.Count)
            {
                if (tokens[pos].type == TokenType.R_SQUARE)
                    return new Pair<JsObject, int>(res, pos + 1);
                if (first)
                {
                    first = false;
                }
                else
                {
                    /*if (tokens[pos].type != TokenType.COMMA)
                        throw new JsonParseException();
                    pos += 1;*/
                    if (tokens[pos].type == TokenType.COMMA)
                        pos += 1;
                }
                while (pos < tokens.Count && tokens[pos].type == TokenType.COMMA)
                {
                    var cur2 = new JsObject();
                    res.Add(cur2);
                    ++pos;
                }
                if (pos >= tokens.Count)
                    break;
                var cur = ParseObject(tokens, pos);
                pos = cur.second;
                res.Add(cur.first);
            }
            throw new JsonParseException();
        }

        static List<Token> Tokenize(string str)
        {
            var tokens = new List<Token>();
            StringReader sr = new StringReader(str);
            int ch;
            StringBuilder buf = new StringBuilder();
            while ((ch = sr.Read()) >= 0)
            {
                Token? tt = null;
                bool literal = false;
                switch (ch)
                {
                    case ' ':
                    case '\t':
                    case '\n':
                    case '\r':
                        break;
                    case '{':
                        tt = new Token(TokenType.L_CURLY);
                        break;
                    case '}':
                        tt = new Token(TokenType.R_CURLY);
                        break;
                    case '[':
                        tt = new Token(TokenType.L_SQUARE);
                        break;
                    case ']':
                        tt = new Token(TokenType.R_SQUARE);
                        break;
                    case ',':
                        tt = new Token(TokenType.COMMA);
                        break;
                    case ':':
                    case '=': // omg omg
                        tt = new Token(TokenType.COLON);
                        break;
                    case '"':
                    case '\'':
                        tt = new Token(TokenType.STRING, ReadString(sr, (char)ch));
                        break;
                    default:
                        buf.Append((char)ch);
                        literal = true;
                        break;
                }
                if (!literal)
                {
                    if (buf.Length > 0)
                    {
                        string s = buf.ToString();
                        if (s == "null")
                            tokens.Add(new Token(TokenType.STRING, null));
                        else
                            tokens.Add(new Token(TokenType.STRING, buf.ToString()));
                        buf = new StringBuilder();
                    }
                    if (tt != null)
                        tokens.Add(tt.Value);
                }
            }
            return tokens;
        }

        static string ReadString(StringReader sr, char quote)
        {
            int ch;
            StringBuilder buf = new StringBuilder();
            bool ok = false;
            while ((ch = sr.Read()) >= 0)
            {
                if (ch == '\\')
                {
                    ch = sr.Read();
                    if (ch < 0)
                        break;
                    switch (ch)
                    {
                        case 'n':
                            ch = '\n';
                            break;
                        case 'r':
                            ch = '\r';
                            break;
                        case 'u':
                        case 'U':
                            string s = "";
                            for (int i = 0; i < 4; ++i)
                            {
                                int tmp = sr.Read();
                                if (tmp < 0)
                                    throw new JsonException("Unclosed string");
                                s += (char)tmp;
                            }
                            ch = Convert.ToInt32(s, 16);
                            break;
                    }
                }
                else
                {
                    if (ch == quote)
                    {
                        ok = true;
                        break;
                    }
                }
                buf.Append((char)ch);
            }
            if (!ok) throw new JsonException("Unclosed string");
            return buf.ToString();
        }

        public static string EscapeString(string str)
        {
            StringReader sr = new StringReader(str);
            StringBuilder sb = new StringBuilder();

            sb.Append("\"");
            while (sr.Peek() >= 0)
            {
                char ch = (char)sr.Read();
                if (ch == '\\') sb.Append("\\\\");
                else if (ch == '"') sb.Append("\\\"");
                else if (ch == '\n') sb.Append("\\n");
                else if (ch == '\r') sb.Append("\\r");
                else sb.Append(ch);
            }
            sb.Append("\"");

            return sb.ToString();
        }

    }

    class JsObject : IEnumerable<JsObject>
    {
        public Dictionary<string, JsObject> dict = new Dictionary<string, JsObject>();
        
        public JsObject this[object key]
        {
            get {
                try
                {
                    return dict[key.ToString()];
                }
                catch (KeyNotFoundException)
                {
                    JsObject res = new JsObject();
                    dict.Add(key.ToString(), res);
                    return res;
                }
            }
            set { dict[key.ToString()] = value; }
        }

        public StringCollection str {get { return new StringCollection(this); } }

        public class StringCollection : IEnumerable<string>
        {
            JsObject obj;
            
            public StringCollection(JsObject _obj)
            {
                obj = _obj;
            }

            public string this[object key]
            {
                get { return obj[key].ToString(); }
                set { obj[key] = new JsAtom(value); }
            }

            public IEnumerator<string> GetEnumerator()
            {
                foreach (var x in obj)
                    yield return x.ToString();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        public IntCollection num {get {return new IntCollection(this); }}

        public class IntCollection : IEnumerable<int>
        {
            JsObject obj;
            
            public IntCollection(JsObject _obj)
            {
                obj = _obj;
            }

            public int this[object key]
            {
                get { return int.Parse(obj[key].ToString()); }
                set { obj[key] = new JsAtom(value.ToString()); }
            }

            public IEnumerator<int> GetEnumerator()
            {
                foreach (var x in obj)
                    yield return int.Parse(x.ToString());
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }

        public virtual bool Empty()
        {
            return dict.Count == 0;
        }
        
        public virtual void Remove(object key)
        {
            dict.Remove(key.ToString());
        }
        
        public bool ContainsKey(object key)
        {
            return dict.ContainsKey(key.ToString());
        }
        
        public virtual string Serialize()
        {
            StringBuilder s = new StringBuilder();
            s.Append("{");
            bool first = true;
            foreach (var x in dict)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    s.Append(", ");
                }
                s.Append(Json.EscapeString(x.Key) + ": " + x.Value.Serialize());
            }
            s.Append("}");
            return s.ToString();
        }

        public override string ToString()
        {
            if (this.Empty()) return "";
            return this.Serialize();
        }

        public virtual IEnumerator<JsObject> GetEnumerator()
        {
            return dict.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public Dictionary<string, JsObject>.KeyCollection Keys
        {
            get { return dict.Keys; }
        }

        public Dictionary<string, JsObject>.ValueCollection Values
        {
            get { return dict.Values; }
        }
    }

    class JsAtom : JsObject
    {
        string value;
        public JsAtom(string _value)
        {
            value = _value;
        }
        public override string Serialize()
        {
            if (value == null)
                return "null";
            else
                return Json.EscapeString(value);
        }
        public override string ToString()
        {
            return value;
        }
        public override bool Empty()
        {
            return value == "";
        }
    }
    
    class JsArray : JsObject
    {
        public int length = 0;

        public int Add(JsObject obj)
        {
            int pos = length;
            this[pos] = obj;
            length += 1;
            return pos;
        }

        public override void Remove(object key)
        {
            string skey = key.ToString();
            dict.Remove(skey);
            if (skey == (length - 1).ToString())
            {
                length -= 1;
                while (length > 0 && !ContainsKey(length - 1))
                    length -= 1;
            }
        }

        public override string Serialize()
        {
            StringBuilder s = new StringBuilder();
            s.Append("[");
            bool first = true;
            foreach (var x in this)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    s.Append(", ");
                }
                s.Append(x.Serialize());
            }
            s.Append("]");
            return s.ToString();
        }

        public override string ToString()
        {
            return this.Serialize();
        }

        public override bool Empty()
        {
            return length == 0;
        }

        public override IEnumerator<JsObject> GetEnumerator()
        {
            return new Enumerator(this);
        }

        class Enumerator : IEnumerator<JsObject>
        {
            JsArray a;
            int pos = -1;

            public Enumerator(JsArray _a)
            {
                a = _a;
            }

            public bool MoveNext()
            {
                ++pos;
                while (pos < a.length && !a.ContainsKey(pos))
                    ++pos;
                return pos < a.length;
            }

            public void Reset()
            {
                pos = -1;
            }

            public void Dispose()
            {
            }

            Object IEnumerator.Current
            {
                get { return a[pos]; }
            }
            
            public JsObject Current
            {
                get { return a[pos]; }
            }
        }

        public IEnumerable<int> Indexes
        {
            get { return new IndexEnumerator(this); }
        }

        class IndexEnumerator : IEnumerable<int>, IEnumerator<int>
        {
            JsArray a;
            int pos = -1;

            public IndexEnumerator(JsArray _a)
            {
                a = _a;
            }

            public IEnumerator<int> GetEnumerator()
            {
                return this;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this;
            }

            public bool MoveNext()
            {
                ++pos;
                while (pos < a.length && !a.ContainsKey(pos))
                    ++pos;
                return pos < a.length;
            }

            public void Reset()
            {
                pos = -1;
            }

            public int Current
            {
                get { return pos; }
            }

            public void Dispose() { }

            object IEnumerator.Current
            {
                get { return pos; }
            }
        }
    }

    enum TokenType
    {
        L_CURLY,
        R_CURLY,
        L_SQUARE,
        R_SQUARE,
        COLON,
        COMMA,
        STRING
    }

    struct Token
    {
        public TokenType type;
        public string value;

        public Token(TokenType _type) : this(_type, "") { }
        public Token(TokenType _type, string _value)
        {
            type = _type;
            value = _value;
        }
    }

    class JsonException : Exception
    {
        public JsonException(string msg) : base(msg) { }
    }

    class JsonParseException : JsonException
    {
        public JsonParseException() : base("Parse error") { }
    }
}
