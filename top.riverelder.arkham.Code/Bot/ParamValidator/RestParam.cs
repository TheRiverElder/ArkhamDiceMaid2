using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code.Bot.ParamValidator
{
    public class RestParam : IParam
    {
        public bool Validate(string raw, out object result, out int length, out string err)
        {
            result = raw.TrimStart();
            length = raw.Length;
            err = null;
            return true;
        }
    }
}
