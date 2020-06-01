using System.Collections.Generic;
using System.Text;

using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;
using top.riverelder.RiverCommand.Parsing;
using static top.riverelder.RiverCommand.PresetNodes;

namespace top.riverelder.arkham.Code.Commands {
    class Command_Display : DiceCmdEntry {

        public string Usage => "显示 <属性|信息|物品|战斗>";
        

        public static void DisplayInfo(DMEnv env, Investigator inv) {
            env.Next = $"姓名：{inv.Name}，描述：{inv.Desc}，体格：{inv.Build}，DB：{inv.DamageBonus}";
        }

        public static void DisplayInventory(DMEnv env, Investigator inv, string itemName) {
            if (inv.Inventory.Count == 0) {
                env.Next = $"{inv.Name}没有物品";
            }
            if (!string.IsNullOrEmpty(itemName)) {
                if (inv.Inventory.TryGetValue(itemName, out Item it)) {
                    env.AppendLine($"{inv.Name}的 {itemName}：")
                        .Append("技能名：").AppendLine(it.SkillName)
                        .Append("类型：").AppendLine(it.Type)
                        .Append("伤害：").AppendLine(it.Damage)
                        .Append("贯穿：").AppendLine(it.Impale ? "是" : "否")
                        .Append("连发数：").AppendLine(it.MaxCount.ToString())
                        .Append("弹匣：").AppendLine(it.Capacity.ToString())
                        .Append("故障值：").AppendLine(it.Mulfunction.ToString())
                        .Append("弹药：").AppendLine(it.CurrentLoad.ToString())
                        .Append("消耗：").Append(it.Cost.ToString());
                } else {
                    env.Next = $"{inv.Name}没有{itemName}";
                }
                return;
            }
            foreach (Item item in inv.Inventory.Values) {
                env.LineAppend(item.Name);
            }
        }

        public static void DisplayValue(DMEnv env, Investigator inv, string valueName) {
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
                } else {
                    env.Next = $"未找到{inv.Name}的{valueName}";
                }
                return;
            } else {
                env.AppendLine($"{inv.Name}的数值：");
                foreach (string name in inv.Values.Names) {
                    env.Append(name).Append(':').Append(inv.Values[name]).Append(' ');
                }
            }
        }

        public static void DisplayFightEvents(DMEnv env, Investigator inv) {
            if (inv.Fights.Count == 0) {
                env.Next = $"{inv.Name}没有战斗事件";
                return;
            }
            env.Append($"{inv.Name}的战斗事件：");
            foreach (FightEvent fight in inv.Fights) {
                env.Line().Append($"来自{fight.SourceName}使用{fight.WeaponName ?? "身体"}的攻击");
            }
        }

        public static void DisplaySpells(DMEnv env, Investigator inv) {
            if (inv.Spells.Count == 0) {
                env.Next = inv.Name + "不会任何法术";
            }
            env.Append($"{inv.Name}的法术：");
            foreach (string sn in inv.Spells) {
                env.Line().Append(sn);
            }
        }

        public static void DisplaySpell(DMEnv env, Scenario sce, Investigator inv, string spellName) {
            if (!sce.Spells.TryGetValue(spellName, out Spell spell)) {
                env.Next = "不存在法术：" + spellName;
            } else if (!inv.Spells.Contains(spellName)) {
                env.Next = inv.Name + "还未学会" + spellName;
            }
            env.Append($"{spellName}消耗：");
            foreach (var e in spell.Cost) {
                env.Line().Append(e.Key).Append('：').Append(e.Value);
            }
        }

        public static void DisplayTags(DMEnv env, Investigator inv) {
            env.Next = inv.Name + (inv.Tags.Count == 0 ? "没有标签" : "的标签：" + string.Join("、", inv.Tags));
        }

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.Register("显示")
            .Then(
                Literal<DMEnv>("信息").Executes((env, args, dict) => DisplayInfo(env, env.Inv))
            ).Then(
                Literal<DMEnv>("物品")
                .Executes((env, args, dict) => DisplayInventory(env, env.Inv, null))
                .Then(
                    String<DMEnv>("物品名").Executes((env, args, dict) => DisplayInventory(env, env.Inv, args.GetStr("物品名")))
                )
            ).Then(
                Literal<DMEnv>("数值")
                .Executes((env, args, dict) => DisplayValue(env, env.Inv, null))
                .Then(
                    String<DMEnv>("数值名").Executes((env, args, dict) => DisplayValue(env, env.Inv, args.GetStr("数值名")))
                )
            ).Then(
                Literal<DMEnv>("战斗").Executes((env, args, dict) => DisplayFightEvents(env, env.Inv))
            ).Then(
                Literal<DMEnv>("法术")
                .Executes((env, args, dict) => DisplaySpells(env, env.Inv))
                .Then(
                    String<DMEnv>("法术名").Executes((env, args, dict) => DisplaySpell(env, env.Sce, env.Inv, args.GetStr("法术名")))
                )
            ).Then(
                Literal<DMEnv>("标签")
                .Executes((env, args, dict) => DisplayTags(env, env.Inv))
            );

            dispatcher.SetAlias("ds", "显示");
            dispatcher.SetAlias("dn", "显示 数值");
            dispatcher.SetAlias("di", "显示 信息");
            dispatcher.SetAlias("df", "显示 战斗");
        }
    }
}
