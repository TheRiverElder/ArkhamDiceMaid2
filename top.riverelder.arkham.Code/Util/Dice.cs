using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code
{
    public class Dice
    {
        public readonly int count;
        public readonly int min;
        public readonly int max;
        public readonly int fix;

        private Random random = new Random();


        public static int Roll(string s)
        {
            Dice dice = Parse(s);
            return dice.Roll();
        }


        public int Roll()
        {
            int result = 0;
            for (int i = 0; i < count; i++)
            {
                result += random.Next(min, max + 1); ;
            }
            result += fix;
            return result;
        }

        public override string ToString()
        {
            string s = min == 1 ?
                $"{count}d{max}" :
                $"{count}d{min}~{max}";
            if (fix != 0)
            {
                s += fix;
            }
            return s;
        }


        public Dice(int count, int min, int max, int fix)
        {
            this.count = count;
            this.min = min;
            this.max = max;
            this.fix = fix;
        }

        private readonly static string reg = @"(\d+)d((\d+)~)?(\d+)([+-]\d+)?";
        private readonly static Dictionary<string, Dice> cache = new Dictionary<string, Dice>();

        public static Dice Parse(string s)
        {
            s = s.Trim();
            if (cache.ContainsKey(s))
            {
                return cache[s];
            }
            Match match = Regex.Match(s, reg);
            if (!match.Success)
            {
                return null;
            }

            if (!int.TryParse(match.Groups[1].Value, out int count))
            {
                count = 1;
            }
            if (!int.TryParse(match.Groups[3].Value, out int min))
            {
                min = 1;
            }
            if (!int.TryParse(match.Groups[4].Value, out int max))
            {
                max = 100;
            }
            if (!int.TryParse(match.Groups[5].Value, out int fix))
            {
                fix = 0;
            }

            Dice dice = new Dice(count, min, max, fix);
            cache[s] = dice;
            return dice;
        }
    }
}
