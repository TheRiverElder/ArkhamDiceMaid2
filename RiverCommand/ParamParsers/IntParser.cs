using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using top.riverelder.RiverCommand.Parsing;
using top.riverelder.RiverCommand.Utils;

namespace top.riverelder.RiverCommand.ParamParsers {
    public class IntParser<TEnv> : ParamParser<TEnv> {

        //public NumberType Type;
        

        //public string Tip => Type.Str;
        public override string Tip => "整数";

        public override string[] Certain => null;

        protected override bool Parse(CmdDispatcher<TEnv> dispatcher, TEnv env, StringReader reader, out object result) {
            if (!reader.HasNext) {
                result = 0;
                return false;
            }
            int start = reader.Cursor;
            if (reader.Peek() == '+' || reader.Peek() == '-') {
                reader.Skip();
            }

            while (reader.HasNext && char.IsDigit(reader.Peek())) {
                reader.Skip();
            }

            string s = reader.Data.Substring(start, reader.Cursor - start);
            if (int.TryParse(s, out int rst)) {
                result = rst;
                return true;
            }
            result = 0;
            return false;
        }
    }
}
