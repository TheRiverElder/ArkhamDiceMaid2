using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code.Bot.ParamValidator
{
    public class DecParam
    {
        public static readonly Regex Reg = new Regex(@"^(\d*\.\d+)");
        public bool Validate(string raw, out object result, out int length, out string err)
        {
            Match m = Reg.Match(raw);
            if (!m.Success)
            {
                result = null;
                length = 0;
                err = "参数不是浮点数";
                return false;
            }

            string s = m.Groups[1].Value;
            if (float.TryParse(s, out float rst))
            {
                result = rst;
                length = s.Length;
                err = null;
                return true;
            }
            result = 0;
            length = 0;
            err = "浮点数解析错误";
            return false;
        }
    }
}
