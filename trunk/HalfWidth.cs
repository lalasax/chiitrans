using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChiiTrans
{
    class HalfWidth
    {
        private static char getChar1(char x)
        {
            const string s = "カガキギクグケゲコゴサザシジスズセゼソゾタダチヂツヅテデトドハバヒビフブヘベホボウヴヽヾ";
            for (int i = 0; i < s.Length; i += 2)
            {
                if (s[i] == x) return s[i + 1];
            }
            return '?';
        }

        private static char getChar2(char x)
        {
            const string s = "ハパヒピフプヘペホポ";
            for (int i = 0; i < s.Length; i += 2)
            {
                if (s[i] == x) return s[i + 1];
            }
            return '?';
        }

        public static string Convert(string s)
        {
            Encoding enc = Encoding.GetEncoding(50220);
            byte[] tmp = enc.GetBytes(s);
            s = enc.GetString(tmp);

            StringBuilder res = new StringBuilder();
            for (int i = 0; i < s.Length; ++i)
            {
                if (i + 1 < s.Length)
                {
                    if (s[i + 1] == '゛')
                    {
                        res.Append(getChar1(s[i]));
                        ++i;
                        continue;
                    }
                    else if (s[i + 1] == 'ﾟ')
                    {
                        res.Append(getChar2(s[i]));
                        ++i;
                        continue;
                    }
                }
                res.Append(s[i]);
            }
            return res.ToString();
        }
    }
}
