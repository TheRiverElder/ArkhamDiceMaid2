using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using top.riverelder.RiverCommand.Parsing;
using top.riverelder.RiverCommand.Utils;

namespace top.riverelder.RiverCommand.ParamParsers {
    public class StringParser<TEnv> : ParamParser<TEnv> {

        public static readonly char EscapeChar = '\\';

        public static readonly IDictionary<char, char> pairs = new Dictionary<char, char> {
            ['"'] = '"',
            ['“'] = '”',
            ['\''] = '\'',
            ['‘'] = '’',
        };
        

        public override string Tip => "字符串";

        public override string[] Certain => null;

        protected override bool Parse(CmdDispatcher<TEnv> dispatcher, TEnv env, StringReader reader, out object result) {
            if (!reader.HasNext) {
                result = null;
                return false;
            }

            StringBuilder sb = new StringBuilder();
            
            if (!pairs.TryGetValue(reader.Peek(), out char end)) {
                string res = reader.Read(IsValidNakeChar);
                if (string.IsNullOrEmpty(res)) {
                    result = null;
                    return false;
                } else {
                    result = res;
                    return true;
                }
            }
            reader.Skip();
            bool escape = false;

            while (reader.HasNext) {
                char ch = reader.Read();
                if (ch == end) {
                    if (escape) {
                        sb.Append(ch);
                        escape = false;
                    } else {
                        break;
                    }
                } else if (ch == EscapeChar) {
                    escape = true;
                } else if (escape) {
                    sb.Append(EscapeChar).Append(ch);
                    escape = false;
                } else {
                    sb.Append(ch);
                }
            }

            result = sb.ToString();
            return true;
        }
        
        public static bool IsValidNakeChar(char ch) {
            return ArgUtil.IsNameChar(ch) || ch == '_' || ch == '-';
        }
    }
}
