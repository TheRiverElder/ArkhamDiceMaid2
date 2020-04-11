using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Utils;

namespace top.riverelder.arkham.Code.Model {
    public class Spell {

        public string Name { get; }

        public Dictionary<string, string> Cost { get; }

        public Spell(string name, Dictionary<string, string> cost) {
            Name = name;
            Cost = cost;
        }

        public bool Use(Investigator user, out string reply) {
            Dictionary<string, int> actualCost = new Dictionary<string, int>();
            foreach (var e in Cost) {
                int c = Dice.RollWith(e.Value, user.DamageBonus);
                if (!user.Values.TryWidelyGet(e.Key, out Value value)) {
                    reply = user.Name + "不存在数值：" + e.Key;
                    return false;
                } else if (value.Val < c) {
                    reply = $"{user.Name}的{e.Key}少于消耗数值：{value.Val} < {c}";
                    return false;
                }
                actualCost[e.Key] = c;
            }
            StringBuilder builder = new StringBuilder();
            builder.Append("消耗");
            foreach (var e in actualCost) {
                if (!user.Values.TryWidelyGet(e.Key, out Value value)) {
                    reply = user.Name + "不存在数值：" + e.Key;
                    return false;
                }
                int prev = value.Val;
                value.Sub(e.Value);
                builder.AppendLine().Append(e.Key).Append('：').Append(prev).Append(" - ").Append(e.Value).Append(" => ").Append(value.Val);
            }
            reply = builder.ToString();
            return true;
        }
    }
}
