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

        public static string Attack(DMEnv env, Investigator source, Investigator target, string weaponName) {

            Item w = null;
            if (weaponName != null) {
                if (!source.Inventory.TryGet(weaponName, out w)) {
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

            switch (w.Type) {
                case "肉搏": return CommitFight(env, source, target, weaponName);
                case "投掷": return CommitFight(env, source, target, weaponName);
                case "射击": return CalculateDamage(env, source, target, weaponName);
            }
            
            return $"未知的武器类型：{w.Type}，只能是：肉搏、投掷、射击";
        }

        static string CommitFight(DMEnv env, Investigator source, Investigator target, string weaponName) {
            string wName = string.IsNullOrEmpty(weaponName) ? "肉体" : weaponName;
            target.Fights.Enqueue(new FightEvent(source.Name, target.Name, weaponName));
            env.Save();
            return $"{source.Name}使用{wName}对{target.Name}发起了攻击";
        }

        static string CalculateDamage(DMEnv env, Investigator source, Investigator target, string weaponName) {
            Item w = null;
            if (weaponName != null) {
                if (!source.Inventory.TryGet(weaponName, out w)) {
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
            sb.AppendLine($"造成伤害{r}，弹药消耗{cost}，弹药剩余{w.CurrentLoad}/{w.Capacity}");
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
                    .Executes((env, args, dict) => Attack(env, env.Inv, args.GetInv("目标"), null))
                    .Then(
                        String<DMEnv>("武器名").Executes((env, args, dict) => Attack(env, env.Inv, args.GetInv("目标"), args.GetStr("武器名")))
                    )
                )
            ).Then(
                Literal<DMEnv>("闪避").Executes((env, args, dict) => Dodge(env, env.Inv))
            //).Then(
            //    Literal<DMEnv>("反击").Executes((env, args, dict) => DodgeFight(env, env.Inv))
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
        }
    }
}
