using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Model;
using top.riverelder.RiverCommand.ParamParsers;
using top.riverelder.RiverCommand.Utils;

namespace top.riverelder.arkham.Code.Utils {
    public class ValueParser : ParamParser {

        public override string Tip => "数值";

        protected override bool Parse(StringReader reader, out object result) {
            if (!reader.HasNext || !char.IsDigit(reader.Peek())) {
                result = null;
                return false;
            }
            int start = reader.Cursor;
            while (reader.HasNext && char.IsDigit(reader.Peek())) {
                reader.Skip();
            }
            if (!int.TryParse(reader.Slice(start), out int val)) {
                result = null;
                return false;
            }
            if (!reader.HasNext || reader.Peek() != '/') {
                result = new Value(val);
                return true;
            }
            reader.Skip();
            if (!reader.HasNext || !char.IsDigit(reader.Peek())) {
                result = new Value(val);
                return true;
            }
            start = reader.Cursor;
            while (reader.HasNext && char.IsDigit(reader.Peek())) {
                reader.Skip();
            }
            if (!int.TryParse(reader.Slice(start), out int max)) {
                result = null;
                return false;
            }
            result = new Value(val, max);
            return true;
        }
    }
}
