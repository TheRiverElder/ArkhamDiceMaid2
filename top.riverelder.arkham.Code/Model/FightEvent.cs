using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Utils;

namespace top.riverelder.arkham.Code.Model {
    public class FightEvent {
        public string SourceName { get; set; }
        public string TargetName { get; set; }
        public string WeaponName { get; set; }
        public int Points { get; set; }
        public int ResultType { get; set; }

        public FightEvent(string sourceName, string targetName, string weaponName, int points, int resultType) {
            SourceName = sourceName;
            TargetName = targetName;
            WeaponName = weaponName;
            Points = points;
            ResultType = resultType;
        }

        public override string ToString() {
            return $"来自{SourceName}使用{WeaponName ?? "身体"}的攻击({Points}，{CheckResult.TypeStrings[ResultType]})";
        }
    }


    public class FightEvent1 {

        // 战斗顺序
        public List<string> Order = new List<string>();

        // 声明延迟行动的调查员
        public HashSet<string> Delayed = new HashSet<string>();

        public int Index = 0;

        public string GetNextName() {
            return Order[Index];
        }

        public string TurnToNext() {
            if (++Index > Order.Count) {
                Index = 0;
                Delayed.Clear();
            }
            return Index < Order.Count ? Order[Index] : null;
        }

        public string GetWhoCanActHint() {
            StringBuilder builder = new StringBuilder();
            if (Index >= Order.Count) {
                builder.Append("本轮已经结束，");
                if (Delayed.Count > 0) {
                    builder.AppendLine("仅申明延迟者可行动：").Append(string.Join("，", Delayed)); 
                } else {
                    builder.Append("无人申明延迟，请开始新的一轮");
                }
            } else {
                builder.Append("下一个行动者：").Append(Order[Index]);
                if (Delayed.Count > 0) {
                    builder.AppendLine("，或申明延迟者：").Append(string.Join("，", Delayed));
                } else {
                    builder.Append("，无人申明延迟");
                }
            }
            return builder.ToString();
        }

        public bool CanAct(string name, out string err) {
            if (!Order.Contains(name)) {
                err = name + "没有参加战斗";
                return false;
            } else if (Delayed.Contains(name)) {
                err = null;
                return true;
            } else if (GetNextName() == name) {
                err = null;
                return true;
            } else {
                err = $"未轮到{name}行动，" + GetWhoCanActHint();
                return false;
            }
        }
    }
}
