using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.RiverCommand.Utils;

namespace top.riverelder.RiverCommand.ParamParsers {
    public class LiteralParser : ParamParser {

        public string Literal { get; }

        public override string Tip => Literal;

        public override string[] Certain => new string[] { Literal };

        public LiteralParser(string literal) {
            Literal = literal;
        }

        protected override bool Parse(StringReader reader, out object result) {
            int index = 0;
            while (index < Literal.Length) {
                if (!reader.HasNext || reader.Peek() != Literal[index]) {
                    result = null;
                    return false;
                }
                reader.Skip();
                index++;
            }
            result = Literal;
            return true;
        }
    }
}
