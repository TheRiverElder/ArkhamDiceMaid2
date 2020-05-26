using RiverCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.RiverCommand.Parsing;
using top.riverelder.RiverCommand.Utils;

namespace top.riverelder.RiverCommand.ParamParsers {
    public class CmdParser<TEnv> : ParamParser<TEnv> {

        protected override bool Parse(CmdDispatcher<TEnv> dispatcher, TEnv env, StringReader reader, out object result) {
            bool hasParen = false;
            if (!reader.HasNext) {
                result = null;
                return false;
            }
            if (Config.OpenParen.Contains(reader.Peek())) {
                reader.Skip();
                hasParen = true;
            }
            dispatcher.Dispatch(reader, env, out ICmdResult res);
            if (hasParen && !(reader.HasNext && Config.CloseParen.Contains(reader.Read()))) {
                result = null;
                return false;
            }

            result = res;
            return true;
        }
    }
}
