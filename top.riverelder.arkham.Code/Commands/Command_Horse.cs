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
    class Command_Horse : ICommand {
        public string Name => "赛马";

        public string Usage => "赛马 <开始|终止|步进|显示|下注|杀马> [数量|序号] [金额|武器]";

        public ArgumentValidater Validater { get; } = ArgumentValidater.Empty
            .SetListArgCountMin(1)
            .SetListArgCountMax(3)
            .AddListArg("开始|终止|步进|显示|下注|杀马")
            .AddListArg(ArgumentValidater.Number);

        public string Execute(string[] listArgs, IDictionary<string, string> dictArgs, string originalString, CmdEnv env) {
            if (!EnvValidator.ExistInv(env, out Investigator inv, out string err)) {
                return err;
            }

            Scenario scenario = env.Scenario;
            List<Horse> horses = scenario.Horses;

            string opt = listArgs[0];

            string ret = "未知错误";
            switch (opt) {
                case "开始": {
                        if (listArgs.Length < 2 || !int.TryParse(listArgs[1], out int num)) {
                            return "请输入🐎匹数量！";
                        }
                        ret = Start(horses, num);
                    }
                    break;
                case "终止": {
                        horses.Clear();
                        ret = "🏇已终止！所有下注已被吞！";
                    } break;
                case "步进": ret = Step(horses, scenario); break;
                case "显示": ret = DisplayHorses(horses); break;
                case "下注": {
                        if (listArgs.Length < 2 || !int.TryParse(listArgs[1], out int index)) {
                            return "请输入🐎匹序号！（不是名字！）";
                        }
                        if (listArgs.Length < 3 || !int.TryParse(listArgs[2], out int amount)) {
                            return "请输入下注金额！";
                        }
                        ret = Bet(horses, inv, index, amount);
                    }
                    break;
                case "杀马": {
                        if (listArgs.Length < 2 || !int.TryParse(listArgs[1], out int index)) {
                            return "请输入🐎匹序号！（不是名字！）";
                        }
                        string weaponName = listArgs.Length < 3 ? null : listArgs[2];
                        ret = Kill(horses, inv, index, weaponName);
                    }
                    break;
                default: return "未知错误";
            }
            SaveUtil.Save(scenario);
            return ret;
        }

        string Start(List<Horse> horses, int amount) {
            if (horses.Count > 0) {
                return "🏇已经开始！";
            }
            if (amount < 2 || amount > 10) {
                return "🐎匹不得少于2只且不得大于10只！";
            }

            for (int i = 0; i < amount; i++) {
                Horse horse = new Horse();
                horses.Add(horse);
            }
            return DisplayHorses(horses);
        }

        string Step(List<Horse> horses, Scenario scenario) {
            if (horses.Count == 0) {
                return "🏇还未开始！";
            }
            HashSet<int> winners = new HashSet<int>();
            for (int i = 0; i < horses.Count; i++) {
                if (horses[i].Step()) {
                    winners.Add(i);
                }
            }
            string scene = DisplayHorses(horses);
            if (winners.Count > 0) {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(scene).Append("赢家：");
                Dictionary<string, int> profits = new Dictionary<string, int>();
                foreach (int index in winners) {
                    sb.Append(indices.ElementAt(index));
                    double bonus = 1.2 + Horse.Rand.NextDouble() * 0.3;
                    foreach (var e in horses[index].Bets) {
                        profits[e.Key] = (int)(e.Value * bonus) + (profits.ContainsKey(e.Key) ? profits[e.Key] : 0);
                    }
                }
                sb.AppendLine("号。");
                foreach (var e in profits) {
                    if (scenario.TryGetInvestigator(e.Key, out Investigator inv)) {
                        if (!inv.Values.TryWidelyGet("账户", out Value account)) {
                            account = new Value(0);
                            inv.Values.Put("账户", account);
                        }
                        int prev = account.Val;
                        account.Add(e.Value);
                        sb.AppendLine($"{inv.Name}的账户：{prev} + {e.Value} => {account.Val}");
                    } else {
                        sb.AppendLine($"未找到{e.Key}");
                    }
                }
                horses.Clear();
                return sb.Append("🏇已结束！").ToString();
            }
            return scene;
        }

        string Bet(List<Horse> horses, Investigator inv, int index, int amount) {
            if (amount < 0) {
                return "必须输入账户且必须大于零！";
            } else if (index <= 0 || index > horses.Count) {
                return $"找不到{index}号🐎";
            }
            if (!inv.Values.TryGet("账户", out Value account)) {
                return $"{inv.Name}没有账户";
            } else if (account.Val < amount) {
                return $"{inv.Name}的账户只有{account.Val}不足{amount}";
            }
            account.Add(-amount);
            Horse horse = horses[index - 1];
            if (horse.Bets.ContainsKey(inv.Name)) {
                horse.Bets[inv.Name] += amount;
            } else {
                horse.Bets[inv.Name] = amount;
            }
            return $"{inv.Name}下注成功，对象：{index}号🐎，金额：{amount}，总金额：{horse.Bets[inv.Name]}";
        }

        string Kill(List<Horse> horses, Investigator source, int index, string weaponName) {
            if (horses.Count == 0) {
                return "🏇已经结束！";
            }
            if (index <= 0 || index > horses.Count) {
                return $"找不到{index}号🐎";
            }

            Horse horse = horses[index - 1];

            if (horse.Sources.Contains(source.Name)) {
                return $"{source.Name}本轮已经杀过🐎了！";
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
                    SkillName = "斗殴",
                    SkillValue = 25,
                    Damage = "1D3+DB",
                    Impale = false,
                    MaxCount = 1,
                    Capacity = 1,
                    Mulfunction = 100,
                    CurrentLoad = 1,
                };
            }
            horse.Sources.Add(source.Name);

            StringBuilder sb = new StringBuilder();
            // 检定🐎的闪避
            if (Horse.Rand.Next(100) <= horse.Ponential) {
                return $"{source.Name}没有打中飞速移动中的{index}号🐎";
            }
            // 计算伤害
            string damage = Regex.Replace(w.Damage, @"DB", source.DamageBonus, RegexOptions.IgnoreCase);
            int r = Dice.Roll(damage);
            sb.Append($"造成伤害：{r}");
            if (r > 0) {
                int prev = horse.Health;
                horse.Health = Math.Min(Math.Max(0, prev - r), Horse.MaxHealth);
                sb.AppendLine().Append($"{index}号🐎的体力：{prev} - {r} => {horse.Health}");
            }
            return sb.ToString();
        }

        public static string indices = "①②③④⑤⑥⑦⑧⑨⑩";
        string DisplayHorses(List<Horse> horses) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < horses.Count; i++) {
                sb.Append(indices.ElementAt(i)).Append("号：").Append(horses[i].Display());
                if (i < horses.Count - 1) {
                    sb.AppendLine();
                }
            }
            return sb.ToString();
        }
    }
}
