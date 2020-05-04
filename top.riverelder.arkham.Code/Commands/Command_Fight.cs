﻿using System;
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

    /// <summary>
    /// 战斗 攻击 <目标> [武器]
    /// 战斗 闪避
    /// 战斗 反击
    /// 战斗 逃跑
    /// 战斗 战技 [指令]
    /// 
    /// 战斗 放弃
    /// 战斗跳过
    /// </summary>
    class Command_Fight : DiceCmdEntry {

        public string Usage => "战斗 <攻击|闪避|跳过|放弃|射击> [目标] [武器名]";

        private string GiveUp(DMEnv env, Investigator inv) {
            if (inv.Fights.Count == 0) {
                return "未找到打斗事件";
            }
            FightEvent fight = inv.Fights.Dequeue();

            if (!env.Sce.TryGetInvestigator(fight.SourceName, out Investigator source)) {
                return $"未找到打斗来源：{fight.SourceName}";
            }

            return CalculateDamage(env, source, inv, fight.WeaponName);
        }

        private string Skip(DMEnv env, Investigator inv) {
            if (inv.Fights.Count > 0) {
                FightEvent fight = inv.Fights.Dequeue();
                env.Save();
                return $"已经跳过了来自{fight.SourceName}的打斗";
            }
            return "没有需要跳过打斗";
        }

        string Dodge(DMEnv env, Investigator inv) {
            if (inv.Fights.Count == 0) {
                return "未找到打斗事件";
            }
            FightEvent fight = inv.Fights.Dequeue();

            if (!env.Sce.TryGetInvestigator(fight.SourceName, out Investigator source)) {
                return $"未找到打斗来源：{fight.SourceName}";
            }

            if (!inv.Values.TryGet("闪避", out Value dodge)) {
                if (!inv.Values.TryGet("敏捷", out Value dex)) {
                    return $"未找到{inv.Name}的闪避或敏捷属性";
                }
                dodge = new Value(dex.Val / 2);
            }

            CheckResult result = dodge.Check();
            if (result.succeed && result.type <= fight.ResultType) {
                env.Save();
                return $"{inv.Name}躲开了{source.Name}的攻击({result.ActualTypeString})！";
            } else {
                string r = $"{inv.Name}闪避失败\n" + CalculateDamage(env, source, inv, fight.WeaponName);
                env.Save();
                return r;
            }
        }

        public static string Attack(DMEnv env, Investigator source, Investigator target, string weaponName) {

            Item w = null;
            if (weaponName != null) {
                if (!source.Inventory.TryGetValue(weaponName, out w)) {
                    return $"未找到{source.Name}武器：{weaponName}";
                }
            } else {
                w = new Item("身体");
            }

            if (!source.Values.TryGet(w.SkillName, out Value skill) && !Global.DefaultValues.TryGet(w.SkillName, out skill)) {
                return $"未找到的{w.SkillName}技能及其默认值";
            }
            CheckResult result = skill.Check();

            if (!result.succeed) {
                return $"{source.Name}对{target.Name}的攻击失败";
            }

            int mulfunctionCheckResult = Dice.Roll(100);
            if (mulfunctionCheckResult > w.Mulfunction) {
                return $"{source.Name}的{w.Name}{(w.Type == "射击" ? "炸膛" : "坏掉")}了！({mulfunctionCheckResult} > {w.Mulfunction})";
            }

            switch (w.Type) {
                case "肉搏": return CommitFight(env, source, target, weaponName, result.result, result.type);
                case "投掷": return CommitFight(env, source, target, weaponName, result.result, result.type);
                case "射击": return CalculateDamage(env, source, target, weaponName);
            }
            
            return $"未知的武器类型：{w.Type}，只能是：肉搏、投掷、射击";
        }

        public static string FightBack(DMEnv env, Investigator target, string weaponName) {

            Item selfWeapon = null;
            if (weaponName != null) {
                if (!target.Inventory.TryGetValue(weaponName, out selfWeapon)) {
                    return $"未找到{target.Name}的{weaponName}";
                }
            } else {
                selfWeapon = new Item("身体");
            }

            if (target.Fights.Count == 0) {
                return "未找到打斗事件";
            }
            FightEvent fight = target.Fights.Dequeue();

            if (!env.Sce.TryGetInvestigator(fight.SourceName, out Investigator source)) {
                return $"未找到打斗来源：{fight.SourceName}";
            }

            Item oppositeWeapon = null;
            if (fight.WeaponName != null) {
                if (!source.Inventory.TryGetValue(fight.WeaponName, out oppositeWeapon)) {
                    return $"未找到{source.Name}的{fight.WeaponName}";
                }
            } else {
                oppositeWeapon = new Item("身体");
            }

            if (oppositeWeapon.Type != "肉搏") {
                return oppositeWeapon + "不是肉搏武器，仅肉搏可反击！";
            }

            if (!target.Values.TryGet(oppositeWeapon.SkillName, out Value skill)) {
                return $"未找到{target.Name}的{oppositeWeapon.SkillName}";
            }

            CheckResult result = skill.Check();
            string r = "出现异常";
            if (result.succeed && result.type < fight.ResultType) {
                int mulfunctionCheckResult = Dice.Roll(100);
                if (mulfunctionCheckResult > selfWeapon.Mulfunction) {
                    return $"{target.Name}的{selfWeapon.Name}{(selfWeapon.Type == "射击" ? "炸膛" : "坏掉")}了！({mulfunctionCheckResult} > {selfWeapon.Mulfunction})";
                }
                r = $"{target.Name}反击成功{source.Name}({result.ActualTypeString})！\n" + CalculateDamage(env, target, source, weaponName);
            } else {
                r = $"{target.Name}反击失败\n" + CalculateDamage(env, source, target, fight.WeaponName);
            }
            return r;
        }

        static string CommitFight(DMEnv env, Investigator source, Investigator target, string weaponName, int points, int resultType) {
            string wName = string.IsNullOrEmpty(weaponName) ? "肉体" : weaponName;
            target.Fights.Enqueue(new FightEvent(source.Name, target.Name, weaponName, points, resultType));
            env.Save();
            return $"{source.Name}使用{wName}对{target.Name}发起了攻击({CheckResult.TypeStrings[resultType]}{points})";
        }

        static string CalculateDamage(DMEnv env, Investigator source, Investigator target, string weaponName) {
            Item w = null;
            if (weaponName != null) {
                if (!source.Inventory.TryGetValue(weaponName, out w)) {
                    return $"未找到{source.Name}武器：{weaponName}";
                }
            } else {
                w = new Item("身体");
            }

            if (w.CurrentLoad <= 0) {
                return "弹药不足，请装弹";
            }

            StringBuilder sb = new StringBuilder();
            int r = Dice.RollWith(w.Damage, source.DamageBonus);
            int cost = Math.Min(w.Cost, w.CurrentLoad);
            w.CurrentLoad -= cost;
            sb.AppendLine($"{source.Name}对{target.Name}造成伤害{r}，弹药消耗{cost}，弹药剩余{w.CurrentLoad}/{w.Capacity}");
            if (target.Values.TryGet("护甲", out Value protect)) {
                r = Math.Max(0, r - protect.Val);
                sb.AppendLine("护甲阻挡部分伤害，最终伤害：" + r);
            }
            if (r > 0) {
                if (!target.Values.TryGet("体力", out Value th)) {
                    return sb.Append("而对方没有体力").ToString();
                }
                int prev = th.Val;
                th.Sub(r);
                sb.Append($"{target.Name}的体力：{(target.Is("NPC") ? "???" : Convert.ToString(prev))} - {r} => {(target.Is("NPC") ? "???" : Convert.ToString(th.Val))}");
                if (r >= th.Max / 2 && th.Val > 0) {
                    if (!target.Values.TryWidelyGet("意志", out Value san)) {
                        san = new Value(50);
                    }
                    CheckResult cr = san.Check();
                    sb.AppendLine().Append("失血过半，检定意志：");
                    sb.Append($"结果：{cr.result}");
                    if (cr.succeed) {
                        sb.Append("成功挺住");
                    } else {
                        sb.Append($"{target.Name}昏厥");
                    }
                }
            }
            env.Save();
            return sb.ToString();
        }

        public static readonly string NPCTag = "NPC";
        public static readonly string UnkonwnValue = "???";

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.Register("战斗")
            .Handles(Extensions.ExistSelfInv())
            .Then(
                Literal<DMEnv>("攻击").Then(
                    String<DMEnv>("目标")
                    .Handles(Extensions.ExistInv())
                    .Executes((env, args, dict) => Attack(env, env.Inv, args.GetInv("目标"), null))
                    .Then(
                        String<DMEnv>("武器名").Executes((env, args, dict) => Attack(env, env.Inv, args.GetInv("目标"), args.GetStr("武器名")))
                    )
                )
            ).Then(
                Literal<DMEnv>("闪避").Executes((env, args, dict) => Dodge(env, env.Inv))
            ).Then(
                Literal<DMEnv>("反击").Executes((env, args, dict) => FightBack(env, env.Inv, null))
                .Then(
                    String<DMEnv>("武器名").Executes((env, args, dict) => FightBack(env, env.Inv, args.GetStr("武器名")))
                )
            //).Then(
            //    Literal<DMEnv>("战技").Executes((env, args, dict) => DodgeFight(env, env.Inv))
            //).Then(
            //    Literal<DMEnv>("逃跑").Executes((env, args, dict) => DodgeFight(env, env.Inv))
            ).Then(
                Literal<DMEnv>("跳过").Executes((env, args, dict) => Skip(env, env.Inv))
            ).Then(
                Literal<DMEnv>("放弃").Executes((env, args, dict) => GiveUp(env, env.Inv))
            );

            dispatcher.SetAlias("攻击", "战斗 攻击");
            dispatcher.SetAlias("闪避", "战斗 闪避");
            dispatcher.SetAlias("反击", "战斗 反击");


            dispatcher.SetAlias("fg", "战斗");
            dispatcher.SetAlias("at", "战斗 攻击");
            dispatcher.SetAlias("dd", "战斗 闪避");
            dispatcher.SetAlias("fb", "战斗 反击");
        }
    }
}
