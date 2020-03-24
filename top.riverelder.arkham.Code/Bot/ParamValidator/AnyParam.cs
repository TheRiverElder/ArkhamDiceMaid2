using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code.Bot.ParamValidator
{
    public class AnyParam : IParam
    {
        public static readonly Regex Reg = new Regex(@"^(\S+)");
        public bool Validate(string raw, out object result, out int length, out string err)
        {
            Match m = Reg.Match(raw);
            if (m.Success)
            {
                string s = m.Groups[1].Value;
                result = s;
                length = s.Length;
                err = null;
                return true;
            }
            result = null;
            length = 0;
            err = "参数为空";
            return false;
        }
    }
}
