using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Utils;

namespace top.riverelder.arkham.Code.Bot.ParamValidator
{
    public class DiceParam : IParam
    {
        public bool Validate(string raw, out object result, out int length, out string err)
        {
            if (Dice.TryParse(raw, out Dice dice, out length))
            {
                err = null;
                result = dice;
                return true;
            }
            result = null;
            err = "不是骰子表达式";
            return false;
        }
    }
}
