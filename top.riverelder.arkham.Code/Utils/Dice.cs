using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using top.riverelder.RiverCommand.Utils;

namespace top.riverelder.arkham.Code.Utils {
    public class Dice {
        private IList<DiceItem> items = new List<DiceItem>();

        public static int Roll(string s) => RollWith(s, null);

        public static int RollWith(string s, string db) {
            Dice dice = Of(s);
            return dice.RollWith(db);
        }

        public bool IsEmpty => items.Count == 0;

        public int Roll() => RollWith(null);

        public int RollWith(string db) {
            int result = 0;
            foreach (DiceItem item in items) {
                result += item.Gen(db);
            }
            return result;
        }
        
        private readonly static Dictionary<string, Dice> cache = new Dictionary<string, Dice>();

        public static bool TryParse(StringReader reader, out Dice dice) {
            dice = new Dice();
            bool needSign = false;
            while (reader.HasNext && !char.IsWhiteSpace(reader.Peek())) {
                if (TryParseItem(reader, out DiceItem item, needSign)) {
                    dice.items.Add(item);
                } else {
                    return false;
                }
                needSign = true;
            }
            return true;
        }

        private static bool TryParseItem(StringReader reader, out DiceItem item, bool needSign) {
            int signCount = 0;
            int sign = 1;
            while (reader.HasNext) {
                char ch = reader.Peek();
                if (ch == '+') {
                    reader.Skip();
                    signCount++;
                } else if (ch == '-') {
                    sign *= -1;
                    reader.Skip();
                    signCount++;
                } else {
                    break;
                }
            }
            if ((needSign && signCount == 0) || !reader.HasNext) {
                item = null;
                return false;
            }
            int tStart = reader.Cursor;
            while (reader.HasNext && char.IsDigit(reader.Peek())) {
                reader.Skip();
            }
            int times = tStart >= reader.Cursor ? 1 : int.Parse(reader.Data.Substring(tStart, reader.Cursor - tStart));
            if (!reader.HasNext || (reader.Peek() != 'D' && reader.Peek() != 'd')) {
                item = DiceItem.Constant(sign, times);
                return true;
            }
            reader.Skip();
            if (!reader.HasNext) {
                item = null;
                return false;
            }
            if (reader.Peek() == 'B' || reader.Peek() == 'b') {
                reader.Skip();
                item = DiceItem.DamageBonus(sign, times);
                return true;
            } else if (char.IsDigit(reader.Peek())) {
                int vStart = reader.Cursor;
                reader.Skip();
                while (reader.HasNext && char.IsDigit(reader.Peek())) {
                    reader.Skip();
                }
                int value = vStart >= reader.Cursor ? 1 : int.Parse(reader.Data.Substring(vStart, reader.Cursor - vStart));
                item = DiceItem.Random(sign, value, times);
                return true;
            } else {
                item = null;
                return false;
            }
        }

        public static Dice Of(string raw) {
            if (cache.TryGetValue(raw, out Dice dice)) {
                return dice;
            }
            if (TryParse(new StringReader(raw), out dice)) {
                return dice;
            }
            return new Dice();
        }

        public override string ToString() {
            if (items.Count == 0) {
                return "0";
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < items.Count; i++) {
                DiceItem item = items[i];
                if (i > 0 || item.sign < 0) {
                    sb.Append(item.sign < 0 ? '-' : '+');
                }
                switch (item.type) {
                    case DiceItem.TypeConstant: sb.Append(item.value); break;
                    case DiceItem.TypeRandom: sb.Append(item.times).Append('d').Append(item.value); break;
                    case DiceItem.TypeDamageBonus: sb.Append(item.times).Append("DB"); break;
                }
            }
            return sb.ToString();
        }
    }

    class DiceItem {

        static int seed = (int)DateTime.Now.Ticks;

        public const int TypeConstant = 0;
        public const int TypeRandom = 1;
        public const int TypeDamageBonus = 2;

        public int sign;
        public int value;
        public int times;
        public int type;

        public DiceItem(int sign, int value, int times, int type) {
            this.sign = sign;
            this.value = value;
            this.times = times;
            this.type = type;
        }

        public static DiceItem Constant(int sign, int value) => new DiceItem(sign, value, 1, TypeConstant);
        public static DiceItem Random(int sign, int value, int times) => new DiceItem(sign, value, times, TypeRandom);
        public static DiceItem DamageBonus(int sign, int times) => new DiceItem(sign, 0, times, TypeDamageBonus);

        public int Gen(string db) {
            if (type == TypeConstant) {
                return sign * value;
            } else if (type == TypeDamageBonus) {
                return sign * times * (string.IsNullOrEmpty(db) ? 0 : Dice.Roll(db));
            }
            Random random = new Random(seed++);
            int sum = 0;
            for (int i = 0; i < times; i++) {
                sum += random.Next(value) + 1;
            }
            return sign * sum;
        }
    }
}