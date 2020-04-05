﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.RiverCommand.Utils;

namespace top.riverelder.RiverCommand.ParamParsers {
    public class OrParser : ParamParser {

        public string[] ValidValues;

        public OrParser(string[] validValues) {
            ValidValues = validValues;
            Tip = string.Join("，", ValidValues);
        }
        
        public override string Tip { get; }

        public override string[] Certain => ValidValues;

        protected override bool Parse(StringReader reader, out object result) {
            int start = reader.Cursor;
            foreach (string s in ValidValues) {
                if (Match(reader, s)) {
                    result = s;
                    return true;
                }
                reader.Cursor = start;
            }
            result = null;
            return false;
        }

        private bool Match(StringReader reader, string s) {
            for (int i = 0; i < s.Length; i++) {
                if (!reader.HasNext || reader.Read() != s[i]) {
                    return false;
                }
            }
            return true;
        }
    }
}