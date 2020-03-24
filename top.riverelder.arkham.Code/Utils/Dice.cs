using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code.Utils
{
    public class Dice
    {
        private IList<DiceItem> items = new List<DiceItem>();

        public static int Roll(string s)
        {
            Dice dice = Of(s);
            return dice.Roll();
        }

        public bool IsEmpty => items.Count == 0;

        public int Roll()
        {
            int result = 0;
            foreach (DiceItem item in items)
            {
                result += item.Gen();
            }
            return result;
        }
        
        private readonly static Regex empty = new Regex(@"^\s+$");
        private readonly static Regex sep = new Regex(@"^[-+]");
        private readonly static Regex reg = new Regex(@"^\s*([+-])?\s*((\d+)\s*[dD])?\s*(\d+)");
        private readonly static Dictionary<string, Dice> cache = new Dictionary<string, Dice>();

        public static bool TryParse(string s, out Dice dice, out int length)
        {
            if (cache.ContainsKey(s))
            {
                dice = cache[s];
                length = s.Length;
                return true;
            }
            dice = new Dice();
            string str = s;
            Match match = sep.Match(str);
            int sign = 1;
            if (match.Success)
            {
                sign = "-".Equals(match.Value) ? -1 : 1;
                str = str.Substring(match.Value.Length);
            }
            match = reg.Match(str);
            while (!string.IsNullOrEmpty(str) && !empty.IsMatch(str) && match.Success)
            {
                str = str.Substring(match.Value.Length);
                int innerSign = 1;
                if (match.Groups[1].Success)
                {
                    innerSign = "-".Equals(match.Groups[1].Value) ? -1 : 1;
                }
                int value = int.Parse(match.Groups[4].Value);
                if (match.Groups[3].Success)
                {
                    int times = int.Parse(match.Groups[3].Value);
                    dice.items.Add(DiceItem.Random(sign * innerSign, value, times));
                }
                else
                {
                    dice.items.Add(DiceItem.Constant(sign * innerSign, value));
                }
                match = sep.Match(str);
                if (!match.Success)
                {
                    break;
                }
                sign = "-".Equals(match.Value) ? -1 : 1;
                str = str.Substring(match.Value.Length);
                match = reg.Match(str);
            }

            if (dice.IsEmpty)
            {
                dice = null;
                length = 0;
                return false;
            }
            cache[s] = dice;
            length = s.Length - str.Length;
            return true;
        }

        public static Dice Of(string raw)
        {
            if (TryParse(raw, out Dice dice, out int length))
            {
                return dice;
            }
            return new Dice();
        }
    }

    class DiceItem
    {
        Random random = new Random();

        int sign;
        int value;
        int times;
        bool constant;

        public DiceItem(int sign, int value, int times, bool constant)
        {
            this.sign = sign;
            this.value = value;
            this.times = times;
            this.constant = constant;
        }

        public static DiceItem Constant(int sign, int value) => new DiceItem(sign, value, 1, true);
        public static DiceItem Random(int sign, int value, int times) => new DiceItem(sign, value, times, false);

        public int Gen()
        {
            if (constant)
            {
                return sign * value;
            }
            int sum = 0;
            for (int i = 0; i < times; i++)
            {
                sum += random.Next(value) + 1;
            }
            return sign * sum;
        }
    }
}