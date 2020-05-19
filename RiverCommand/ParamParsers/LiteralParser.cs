using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.RiverCommand.Utils;

namespace top.riverelder.RiverCommand.ParamParsers {
    public class LiteralParser<TEnv> : ParamParser<TEnv> {

        public string Literal { get; }

        public override string Tip => Literal;

        public override string[] Certain => new string[] { Literal };

        public LiteralParser(string literal) {
            Literal = literal;
        }

        protected override bool Parse(CmdDispatcher<TEnv> dispatcher, TEnv env, StringReader reader, out object result) {
            if (reader.Read(Literal)) {
                result = Literal;
                return true;
            } else {
                result = null;
                return false;
            }
        }
    }
}
