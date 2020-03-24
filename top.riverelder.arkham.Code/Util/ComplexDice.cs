using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code.Util
{
    public class ComplexDice
    {
        private IList<Dice> dices;

        public ComplexDice(IList<Dice> dices)
        {
            this.dices = dices;
        }

        private static readonly string reg = @"(\d+[dD]\d+)";
        private static readonly IDictionary<string, ComplexDice> cache = new Dictionary<string, ComplexDice>();

        public static ComplexDice Of(string expr)
        {
            expr = expr.Trim();
            if (cache.TryGetValue(expr, out ComplexDice cd))
            {
                return cd;
            }

        }
    }
}
