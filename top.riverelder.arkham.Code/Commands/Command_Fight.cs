using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;
using static top.riverelder.RiverCommand.PresetNodes;

namespace top.riverelder.arkham.Code.Commands {
    class Command_Fight : DiceCmdEntry {

        public string Usage => "战斗 <攻击|闪避|跳过|放弃|射击> [目标] [武器名]";

        private string GiveUpFight(DMEnv env, Investigator inv) {
            if (inv.Fights.Count == 0) {
                return "未找到打斗事件";
            }
            FightEvent fight = inv.Fights.Dequeue();

            if (!env.Sce.TryGetInvestigator(fight.SourceName, out Investigator source)) {
                return $"未找到打斗来源：{fight.SourceName}";
            }

            return CalculateDamage(env, source, inv, fight.WeaponName);
        }

        private string SkipFight(DMEnv env, Investigator inv) {
            if (inv.Fights.Count > 0) {
                FightEvent fight = inv.Fights.Dequeue();
                env.Save();
                return $"已经跳过了来自{fight.SourceName}的打斗";
            }
            return "没有需要跳过打斗";
        }

        string DodgeFight(DMEnv env, Investigator inv) {
            if (inv.Fights.Count == 0) {
                return "未找到打斗事件";
            }
            FightEvent fight = inv.Fights.Dequeue();

            if (!env.Sce.TryGetInvestigator(fight.SourceName, out Investigator source)) {
                return $"未找到打斗来源：{fight.SourceName}";
            }

            if (!inv.Values.TryGet("敏捷", out Value dex)) {
                return $"未找到{inv.Name}的敏捷属性";
            }
            Value dodge = new Value(dex.Val / 2);

            CheckResult result = dodge.Check();
            if (result.succeed) {
                env.Save();
                return $"{inv.Name}躲开了{source.Name}的攻击";
            } else {
                string r = $"{inv.Name}闪避失败\n" + CalculateDamage(env, source, inv, fight.WeaponName);
                env.Save();
                return r;
            }
        }

        public static string CommitFight(DMEnv env, Investigator source, Investigator target, string weaponName) {

            WeaponInfo w;
            if (weaponName != null) {
                if (!source.Inventory.TryGet(weaponName, out Item item)) {
                    return $"未找到{source.Name}武器：{weaponName}";
                }
                if (!item.IsWeapon) {
                    return $"{source.Name}的{weaponName}不是武器";
                }
                w = item.Weapon;
            } else {
                w = new WeaponInfo {
                    SkillName = "斗殴",
                    SkillValue = 25,
                    Damage = "1D3+DB",
                    Impale = false,
                    MaxCount = 1,
                    Capacity = 1,
                    Mulfunction = 100,
                    CurrentLoad = 1,
                    Cost = 1,
                };
            }

            if (!source.Values.TryGet(w.SkillName, out Value skill)) {
                skill = new Value(w.SkillValue);
            }
            CheckResult result = skill.Check();

            if (!result.succeed) {
                return $"{source.Name}对{target.Name}的攻击失败";
            }

            string wName = string.IsNullOrEmpty(weaponName) ? "肉体" : weaponName;
            target.Fights.Enqueue(new FightEvent(source.Name, target.Name, weaponName));
            env.Save();
            return $"{source.Name}使用{wName}对{target.Name}发起了攻击";
        }

        string CalculateDamage(DMEnv env, Investigator source, Investigator target, string weaponName) {

            WeaponInfo w;
            if (weaponName != null) {
                if (!source.Inventory.TryGet(weaponName, out Item item)) {
                    return $"未找到{source.Name}武器：{weaponName}";
                }
                if (!item.IsWeapon) {
                    return $"{source.Name}的{weaponName}不是武器";
                }
                w = item.Weapon;
            } else {
                w = new WeaponInfo {
                    SkillName = "斗殴",
                    SkillValue = 25,
                    Damage = "1D3+DB",
                    Impale = false,
                    MaxCount = 1,
                    Capacity = 1,
                    Mulfunction = 100,
                    CurrentLoad = 1,
                    Cost = 0,
                };
            }

            if (w.CurrentLoad <= 0) {
                return "弹药不足，请装弹";
            }

            StringBuilder sb = new StringBuilder();
            string damage = Regex.Replace(w.Damage, @"DB", source.DamageBonus, RegexOptions.IgnoreCase);
            int r = Dice.Roll(damage);
            int cost = Math.Min(w.Cost, w.CurrentLoad);
            w.CurrentLoad -= cost;
            sb.AppendLine($"伤害{r}，弹药{cost}，剩余{w.CurrentLoad}/{w.Capacity}");
            if (r > 0) {
                if (!target.Values.TryGet("体力", out Value th)) {
                    return sb.Append("而对方没有体力").ToString();
                }
                int prev = th.Val;
                sb.Append($"{target.Name}的体力：{prev} - {r} => {th.Sub(r).Val}");
                if (r >= th.Max / 2 && th.Val > 0) {
                    if (!target.Values.TryWidelyGet("意志", out Value san)) {
                        san = new Value(50);
                    }
                    CheckResult cr = san.Check();
                    sb.AppendLine().Append($"失血过半，检定意志({san.Val})：");
                    sb.Append($"结果：{cr.result}");
                    if (cr.succeed) {
                        sb.Append($"成功挺住");
                    } else {
                        sb.Append($"{target.Name}昏厥");
                    }
                }
            }
            env.Save();
            return sb.ToString();
        }

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.Register("战斗")
            .Handles(Extensions.ExistSelfInv())
            .Then(
                Literal<DMEnv>("攻击").Then(
                    String<DMEnv>("目标")
                    .Handles(Extensions.ExistInv())
                    .Executes((env, args, dict) => CommitFight(env, env.Inv, args.GetInv("目标"), null))
                    .Then(
                        String<DMEnv>("武器名").Executes((env, args, dict) => CommitFight(env, env.Inv, args.GetInv("目标"), args.GetStr("武器名")))
                    )
                )
            ).Then(
                Literal<DMEnv>("射击").Then(
                    String<DMEnv>("目标")
                    .Handles(Extensions.ExistInv())
                    .Then(
                        String<DMEnv>("武器名").Executes((env, args, dict) => CalculateDamage(env, env.Inv, args.GetInv("目标"), args.GetStr("武器名")))
                    )
                )
            ).Then(
                Literal<DMEnv>("闪避").Executes((env, args, dict) => DodgeFight(env, env.Inv))
            ).Then(
                Literal<DMEnv>("跳过").Executes((env, args, dict) => SkipFight(env, env.Inv))
            ).Then(
                Literal<DMEnv>("放弃").Executes((env, args, dict) => GiveUpFight(env, env.Inv))
            );

            dispatcher.SetAlias("攻击", "战斗 攻击");
            dispatcher.SetAlias("闪避", "战斗 闪避");
            dispatcher.SetAlias("射击", "战斗 射击");
        }
    }
}
