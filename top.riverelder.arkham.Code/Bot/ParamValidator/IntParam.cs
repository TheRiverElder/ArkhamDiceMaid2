using System;
using System.Text.RegularExpressions;

namespace top.riverelder.arkham.Code.Bot.ParamValidator
{
    public class IntParam : IParam
    {
        public static readonly Regex Reg = new Regex(@"^(\d+)");
        public bool Validate(string raw, out object result, out int length, out string err)
        {
            Match m = Reg.Match(raw);
            if (!m.Success)
            {
                result = null;
                length = 0;
                err = "参数不是整数";
                return false;
            }

            string s = m.Groups[1].Value;
            if (int.TryParse(s, out int rst))
            {
                result = rst;
                length = s.Length;
                err = null;
                return true;
            }
            result = 0;
            length = 0;
            err = "整数解析错误";
            return false;
        }
    }
}
