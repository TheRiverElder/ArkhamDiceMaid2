using System.Collections.Generic;
using System.Text;

using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;
using static top.riverelder.RiverCommand.PresetNodes;

namespace top.riverelder.arkham.Code.Commands {
    class Command_Display : DiceCmdEntry {

        public string Usage => "显示 <属性|信息|物品|战斗>";
        

        public static string DisplayInfo(Investigator inv) {
            return $"姓名：{inv.Name}，描述：{inv.Desc}，体格：{inv.Build}，DB：{inv.DamageBonus}";
        }

        public static string DisplayInventory(Investigator inv, string itemName) {
            if (inv.Inventory.Count == 0) {
                return $"{inv.Name}没有物品";
            }
            if (!string.IsNullOrEmpty(itemName)) {
                if (inv.Inventory.TryGetValue(itemName, out Item it)) {
                    StringBuilder b = new StringBuilder().AppendLine($"{inv.Name}的 {itemName}：");
                    b
                        .Append("技能名：").AppendLine(it.SkillName)
                        .Append("类型：").AppendLine(it.Type)
                        .Append("伤害：").AppendLine(it.Damage)
                        .Append("贯穿：").AppendLine(it.Impale ? "是" : "否")
                        .Append("连发数：").AppendLine(it.MaxCount.ToString())
                        .Append("弹匣：").AppendLine(it.Capacity.ToString())
                        .Append("故障值：").AppendLine(it.Mulfunction.ToString())
                        .Append("弹药：").AppendLine(it.CurrentLoad.ToString())
                        .Append("消耗：").Append(it.Cost.ToString());
                    return b.ToString();
                } else {
                    return $"{inv.Name}没有{itemName}";
                }
            }
            StringBuilder sb = new StringBuilder().Append($"{inv.Name}的物品：");
            foreach (Item item in inv.Inventory.Values) {
                sb.AppendLine().Append(item.Name);
            }
            return sb.ToString();
        }

        public static string DisplayValue(Investigator inv, string valueName) {
            if (!string.IsNullOrEmpty(valueName)) {
                if (inv.Values.TryWidelyGet(valueName, out Value value)) {
                    StringBuilder b = new StringBuilder()
                        .Append($"{inv.Name}的{valueName}：")
                        .Append(value.Val).Append('/')
                        .Append(value.HardVal).Append('/')
                        .Append(value.ExtremeVal);
                    if (value.Max > 0) {
                        b.Append('(').Append(value.Max).Append(')');
                    }
                    return b.ToString();
                } else {
                    return $"未找到{inv.Name}的{valueName}";
                }
            } else {
                StringBuilder b = new StringBuilder().AppendLine($"{inv.Name}的数值：");
                foreach (string name in inv.Values.Names) {
                    b.Append(name).Append(':').Append(inv.Values[name]).Append(' ');
                }
                return b.ToString();
            }
        }

        public static string DisplayFightEvents(Investigator inv) {
            if (inv.Fights.Count == 0) {
                return $"{inv.Name}没有战斗事件";
            }
            StringBuilder sb = new StringBuilder().Append($"{inv.Name}的战斗事件：");
            foreach (FightEvent fight in inv.Fights) {
                sb.AppendLine().Append($"来自{fight.SourceName}使用{fight.WeaponName ?? "肉体"}的攻击");
            }
            return sb.ToString();
        }

        public static string DisplaySpells(Investigator inv) {
            if (inv.Spells.Count == 0) {
                return inv.Name + "不会任何法术";
            }
            StringBuilder sb = new StringBuilder().Append($"{inv.Name}的法术：");
            foreach (string sn in inv.Spells) {
                sb.AppendLine().Append(sn);
            }
            return sb.ToString();
        }

        public static string DisplaySpell(Scenario sce, Investigator inv, string spellName) {
            if (!sce.Spells.TryGetValue(spellName, out Spell spell)) {
                return "不存在法术：" + spellName;
            } else if (!inv.Spells.Contains(spellName)) {
                return inv.Name + "还未学会" + spellName;
            }
            StringBuilder sb = new StringBuilder().Append($"{spellName}消耗：");
            foreach (var e in spell.Cost) {
                sb.AppendLine().Append(e.Key).Append('：').Append(e.Value);
            }
            return sb.ToString();
        }

        public static string DisplayTags(Investigator inv) {
            return inv.Name + (inv.Tags.Count == 0 ? "没有标签" : "的标签：" + string.Join("、", inv.Tags));
        }

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.Register("显示")
            .Handles(Extensions.ExistSelfInv())
            .Then(
                Literal<DMEnv>("信息").Executes((env, args, dict) => DisplayInfo(env.Inv))
            ).Then(
                Literal<DMEnv>("物品")
                .Executes((env, args, dict) => DisplayInventory(env.Inv, null))
                .Then(
                    String<DMEnv>("物品名").Executes((env, args, dict) => DisplayInventory(env.Inv, args.GetStr("物品名")))
                )
            ).Then(
                Literal<DMEnv>("数值")
                .Executes((env, args, dict) => DisplayValue(env.Inv, null))
                .Then(
                    String<DMEnv>("数值名").Executes((env, args, dict) => DisplayValue(env.Inv, args.GetStr("数值名")))
                )
            ).Then(
                Literal<DMEnv>("战斗").Executes((env, args, dict) => DisplayFightEvents(env.Inv))
            ).Then(
                Literal<DMEnv>("法术")
                .Executes((env, args, dict) => DisplaySpells(env.Inv))
                .Then(
                    String<DMEnv>("法术名").Executes((env, args, dict) => DisplaySpell(env.Sce, env.Inv, args.GetStr("法术名")))
                )
            ).Then(
                Literal<DMEnv>("标签")
                .Executes((env, args, dict) => DisplayTags(env.Inv))
            );
        }
    }
}
