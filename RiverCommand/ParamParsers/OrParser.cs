using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.RiverCommand.Parsing;
using top.riverelder.RiverCommand.Utils;

namespace top.riverelder.RiverCommand.ParamParsers {
    public class OrParser<TEnv> : ParamParser<TEnv> {

        public string[] ValidValues;

        public OrParser(string[] validValues) {
            ValidValues = validValues;
            Tip = string.Join("|", ValidValues);
        }
        
        public override string Tip { get; }

        public override string[] Certain => ValidValues;

        protected override bool Parse(CmdDispatcher<TEnv> dispatcher, TEnv env, StringReader reader, out object result) {
            int start = reader.Cursor;
            foreach (string s in ValidValues) {
                if (reader.Read(s)) {
                    result = s;
                    return true;
                }
                reader.Cursor = start;
            }
            result = null;
            return false;
        }
    }
}
