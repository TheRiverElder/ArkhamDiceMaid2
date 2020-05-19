using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.RiverCommand.ParamParsers;
using top.riverelder.RiverCommand.Utils;

namespace top.riverelder.RiverCommand.ParamParsers {
    public class BoolParser<TEnv> : ParamParser<TEnv> {

        public string TrueValue = "true";
        public string FalseValue = "false";

        public BoolParser(string trueValue, string falseValue) {
            TrueValue = trueValue;
            FalseValue = falseValue;
            Tip = $"{TrueValue}|{FalseValue}";
        }

        public override string[] Certain => new string[] { TrueValue, FalseValue };

        public override string Tip { get; }

        protected override bool Parse(CmdDispatcher<TEnv> dispatcher, TEnv env, StringReader reader, out object result) {
            if (reader.Read(TrueValue)) {
                result = true;
                return true;
            } else if (reader.Read(FalseValue)) {
                result = false;
                return true;
            } else {
                result = false;
                return false;
            }
        }
    }
}
