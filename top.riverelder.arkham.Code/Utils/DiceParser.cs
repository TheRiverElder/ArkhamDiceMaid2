using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Model;
using top.riverelder.RiverCommand;
using top.riverelder.RiverCommand.ParamParsers;
using top.riverelder.RiverCommand.Utils;

namespace top.riverelder.arkham.Code.Utils {
    public class DiceParser : ParamParser<DMEnv> {

        public override string Tip => "骰子";

        protected override bool Parse(CmdDispatcher<DMEnv> dispatcher, DMEnv env, StringReader reader, out object result) {
            if (Dice.TryParse(reader, out Dice dice)) {
                result = dice;
                return true;
            } else {
                result = null;
                return false;
            }
        }
    }
}
