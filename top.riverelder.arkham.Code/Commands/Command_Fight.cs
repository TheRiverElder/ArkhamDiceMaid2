using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;
using top.riverelder.RiverCommand.Parsing;
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

        private static void GiveUp(DMEnv env, Investigator inv) {
            if (inv.Fights.Count == 0) {
                env.Next = "未找到打斗事件";
            }
            FightEvent fight = inv.Fights.Dequeue();

            if (!env.Sce.TryGetInvestigator(fight.SourceName, out Investigator source)) {
                env.Next = $"未找到打斗来源：{fight.SourceName}";
            }

            CalculateDamage(env, source, inv, fight.WeaponName);
        }

        private static void Skip(DMEnv env, Investigator inv) {
            if (inv.Fights.Count > 0) {
                FightEvent fight = inv.Fights.Dequeue();
                env.Save();
                env.Next = $"已经跳过了来自{fight.SourceName}的打斗";
            }
            env.Next = "没有需要跳过打斗";
        }

        private static bool Dodge(DMEnv env, Investigator inv) {
            
            FightEvent fight = inv.PeekNextFight(env.Sce, out Investigator source, out Item oppositeWeapon);

            if (oppositeWeapon.Type == "射击") {
                env.Next = oppositeWeapon + "是射击武器，仅肉搏或投掷武器可闪避！";
                return false;
            }

            if (!inv.Check("闪避", out CheckResult result, out string str)) {
                env.Next = str;
                return false;
            }
            inv.Fights.Dequeue();

            env.AppendLine(str);
            env.Save();
            if (result.succeed && result.type <= fight.ResultType) {
                env.Append($"躲开了{fight}！");
                return true;
            } else {
                env.AppendLine($"受到了{fight}");
                CalculateDamage(env, source, inv, fight.WeaponName);
                return false;
            }
        }

        private static bool Attack(DMEnv env, Investigator source, Investigator target, string weaponName) {

            Item w = source.GetItem(weaponName);
            
            if (!source.Check(w.SkillName, out CheckResult result, out string str)) {
                return false;
            }
            env.AppendLine(str);
            if (!result.succeed) {
                env.Append("攻击失败");
                return false;
            }

            int mulfunctionCheckResult = Dice.Roll(100);
            if (mulfunctionCheckResult > w.Mulfunction) {
                env.Append($"{source.Name}的{w.Name}{(w.Type == "射击" ? "炸膛" : "坏掉")}了！({mulfunctionCheckResult} > {w.Mulfunction})");
            }

            switch (w.Type) {
                case "肉搏": CommitFight(env, source, target, weaponName, result.points, result.type); return true;
                case "投掷": CommitFight(env, source, target, weaponName, result.points, result.type); return true;
                case "射击": CalculateDamage(env, source, target, weaponName); return true;
                default: env.Append($"未知的武器类型：{w.Type}，只能是：肉搏、投掷、射击"); break;
            }

            return false;
        }

        private static bool FightBack(DMEnv env, Investigator target, string weaponName) {

            Item selfWeapon = target.GetItem(weaponName);
            FightEvent fight = target.PeekNextFight(env.Sce, out Investigator source, out Item oppositeWeapon);

            if (oppositeWeapon.Type != "肉搏") {
                env.Next = oppositeWeapon + "不是肉搏武器，仅肉搏可反击！";
                return false;
            }

            if (!target.Check(oppositeWeapon.SkillName, out CheckResult result, out string str)) {
                env.Next = str;
                return false;
            }

            target.Fights.Dequeue();
            
            env.AppendLine(str);
            if (result.succeed && result.type < fight.ResultType) {
                int mulfunctionCheckResult = Dice.Roll(100);
                if (mulfunctionCheckResult > selfWeapon.Mulfunction) {
                    env.Append($"{target.Name}的{selfWeapon.Name}{(selfWeapon.Type == "射击" ? "炸膛" : "坏掉")}了！({mulfunctionCheckResult} > {selfWeapon.Mulfunction})");
                    return false;
                }
                env.Append($"反击了{fight}！");
                CalculateDamage(env, target, source, weaponName);
            } else {
                env.Append($"受到了{fight}！");
                CalculateDamage(env, source, target, fight.WeaponName);
            }
            return true;
        }

        private static void CommitFight(DMEnv env, Investigator source, Investigator target, string weaponName, int points, int resultType) {
            FightEvent fight = new FightEvent(source.Name, target.Name, weaponName, points, resultType);
            target.Fights.Enqueue(fight);
            env.Save();
            env.Next = target.Name + "即将受到" + fight;
        }

        private static void CalculateDamage(DMEnv env, Investigator source, Investigator target, string weaponName) {
            Item w = null;
            if (weaponName != null) {
                if (!source.Inventory.TryGetValue(weaponName, out w)) {
                    env.Next = $"未找到{source.Name}武器：{weaponName}";
                }
            } else {
                w = new Item("身体");
            }

            if (w.CurrentLoad <= 0) {
                env.Next = "弹药不足，请装弹";
            }
            
            // 计算伤害值
            int r = Dice.RollWith(w.Damage, source.DamageBonus);
            int cost = Math.Min(w.Cost, w.CurrentLoad);
            w.CurrentLoad -= cost;
            env.Append($"{source.Name}对{target.Name}造成伤害{r}");
            if (w.Cost == 0) {
                env.Append($"，弹药消耗{cost}，弹药剩余{w.CurrentLoad}/{w.Capacity}");
            }
            env.Line();
            // 计算护甲格挡
            if (target.Values.TryGet("护甲", out Value protect) && protect.Val > 0) {
                r = Math.Max(0, r - protect.Val);
                env.AppendLine("护甲阻挡部分伤害，最终实际伤害：" + r);
            }
            // 施加伤害
            if (r > 0) {
                if (!target.Values.TryGet("体力", out Value th)) {
                    env.Append("而对方没有体力").ToString();
                    return;
                }
                // 真正减少体力
                int prev = th.Val;
                env.Append(target.Change("体力", -r));
                if (prev < r) { // 检定是否可急救
                    env.Line().Append("受伤过重，无法治疗");
                } else if (r >= (int)(th.Max / 2.0) && th.Val > 0) { // 检定昏迷
                    env.Line().Append("一次失血过半，");
                    if (!target.Values.TryWidelyGet("意志", out Value san)) {
                        san = new Value(50);
                    }
                    if (!target.Check("意志", out CheckResult cr, out string str)) {
                        env.Append(str);
                        return;
                    }
                    env.Append(str).Append("，");
                    if (cr.succeed) {
                        env.Append($"{target.Name}成功挺住");
                    } else {
                        env.Append($"{target.Name}昏厥");
                    }
                }
            }
            env.Save();
        }

        public static readonly string HIDE_VALUETag = "HIDE_VALUE";
        public static readonly string UnkonwnValue = "???";

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.Register("战斗")
            .Then(
                Literal<DMEnv>("攻击").Then(
                    String<DMEnv>("目标")
                    .Handles(Extensions.ExistInv)
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
