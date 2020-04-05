using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using top.riverelder.RiverCommand.Utils;

namespace top.riverelder.RiverCommand.ParamParsers {
    public class IntParser : ParamParser {

        //public NumberType Type;
        

        //public string Tip => Type.Str;
        public override string Tip => "整数";

        public override string[] Certain => null;

        protected override bool Parse(StringReader reader, out object result) {
            int start = reader.Cursor;
            if (!reader.HasNext) {
                result = 0;
                return false;
            }
            if (reader.Peek() == '+' || reader.Peek() == '-') {
                reader.Read();
            }

            while (reader.HasNext && char.IsDigit(reader.Peek())) {
                reader.Skip();
            }

            string s = reader.Data.Substring(start, reader.Cursor);
            if (int.TryParse(s, out int rst)) {
                result = rst;
                return true;
            }
            result = 0;
            return false;
        }
    }
}
