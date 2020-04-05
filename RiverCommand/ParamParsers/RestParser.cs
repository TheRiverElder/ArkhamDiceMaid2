﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.RiverCommand.Utils;

namespace top.riverelder.RiverCommand.ParamParsers {
    class RestParser : ParamParser {

        public override string Tip => "剩余字符串";

        protected override bool Parse(StringReader reader, out object result) {
            if (reader.HasNext) {
                result = reader.ReadToEnd();
                return true;
            }
            result = null;
            return false;
        }
    }
}
