using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Bot;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;

namespace top.riverelder.arkham.Code.Commands {
    class Command_Fight : ICommand {
        public string Name => "战斗";

        public string Usage => "战斗 <攻击|闪避|跳过|放弃> [目标] [武器名]";

        public ArgumentValidater Validater { get; } = ArgumentValidater.Empty
            .SetListArgCountMin(1)
            .SetListArgCountMax(3)
            .AddListArg("攻击|闪避|跳过|放弃")
            .AddListArg(ArgumentValidater.Any)
            .AddListArg(ArgumentValidater.Any);

        public string Execute(string[] listArgs, IDictionary<string, string> dictArgs, string originalString, CmdEnv env) {
            string opt = listArgs[0];
            string targetName = listArgs.Length > 1 ? listArgs[1] : null;
            string weaponName = listArgs.Length > 2 ? listArgs[2] : null;


            if (!EnvValidator.ExistInv(env, out Investigator inv, out string err)) {
                return err;
            }

            string ret = "未知错误";
            switch (opt) {
                case "攻击": {
                        if (string.IsNullOrEmpty(targetName)) {
                            return "攻击请指定目标";
                        }
                        ret = CommitFight(env, inv, targetName, weaponName);
                    } break;
                case "闪避": ret = DodgeFight(env, inv); break;
                case "跳过": ret = SkipFight(env, inv); break;
                case "放弃": ret = GiveUpFight(env, inv); break;
            }
            SaveUtil.Save(env.Scenario);
            return ret;
        }

        private string GiveUpFight(CmdEnv env, Investigator inv) {
            if (inv.Fights.Count == 0) {
                return "未找到打斗事件";
            }
            FightEvent fight = inv.Fights.Dequeue();

            if (!env.Scenario.TryGetInvestigator(fight.SourceName, out Investigator source)) {
                return $"未找到打斗来源：{fight.SourceName}";
            }

            return CalculateDamage(env, source, inv, fight.WeaponName);
        }

        private string SkipFight(CmdEnv env, Investigator inv) {
            if (inv.Fights.Count > 0) {
                FightEvent fight = inv.Fights.Dequeue();
                return $"已经跳过了来自{fight.SourceName}的打斗";
            }
            return "没有需要跳过打斗";
        }

        string DodgeFight(CmdEnv env, Investigator inv) {
            if (inv.Fights.Count == 0) {
                return "未找到打斗事件";
            }
            FightEvent fight = inv.Fights.Dequeue();

            if (!env.Scenario.TryGetInvestigator(fight.SourceName, out Investigator source)) {
                return $"未找到打斗来源：{fight.SourceName}";
            }

            if (!inv.Values.TryGet("敏捷", out Value dex)) {
                return $"未找到{inv.Name}的敏捷属性";
            }
            Value dodge = new Value("闪避", dex.Val / 2);

            CheckResult result =dodge.Check();
            if (result.succeed) {
                return $"{inv.Name}躲开了{source.Name}的攻击";
            } else {
                return $"{inv.Name}闪避失败\n" + CalculateDamage(env, source, inv, fight.WeaponName);
            }
        }

        string CommitFight(CmdEnv env, Investigator source, string targetName, string weaponName) {

            if (!env.Scenario.TryGetInvestigator(targetName, out Investigator target)) {
                return $"未找到目标：{targetName}";
            }

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
                    Skill = new Value("斗殴", 25),
                    Damage = "1D3+DB",
                    Impale = false,
                    MaxCount = 1,
                    Capacity = 1,
                    Mulfunction = 100,
                    CurrentLoad = 1,
                };
            }

            if (!source.Values.TryGet(w.Skill.Name, out Value skill)) {
                skill = w.Skill;
            }
            CheckResult result = skill.Check();

            if (!result.succeed) {
                return $"{source.Name}对{target.Name}的攻击失败";
            }

            string wName = string.IsNullOrEmpty(weaponName) ? "肉体" : weaponName;
            target.Fights.Enqueue(new FightEvent(source.Name, targetName, weaponName));
            return $"{source.Name}使用{wName}对{targetName}发起了攻击";
        }

        string CalculateDamage(CmdEnv env, Investigator source, Investigator target, string weaponName) {

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
                    Skill = new Value("斗殴", 25),
                    Damage = "1D3+DB",
                    Impale = false,
                    MaxCount = 1,
                    Capacity = 1,
                    Mulfunction = 100,
                    CurrentLoad = 1,
                };
            }
            
            StringBuilder sb = new StringBuilder();
            string damage = Regex.Replace(w.Damage, @"DB", source.DamageBonus, RegexOptions.IgnoreCase);
            int r = Dice.Roll(damage);
            sb.AppendLine($"造成伤害：{r}");
            if (r > 0) {
                if (!target.Values.TryGet("体力", out Value th)) {
                    return sb.Append("而对方没有体力").ToString();
                }
                int prev = th.Val;
                sb.Append($"{target.Name}的体力：{prev} - {r} => {th.Sub(r).Val}");
            }
            return sb.ToString();
        }
    }
}
