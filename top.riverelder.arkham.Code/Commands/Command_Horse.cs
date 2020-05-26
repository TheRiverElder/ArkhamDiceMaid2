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
    public class Command_Horse : DiceCmdEntry {

        public static double BonusMin = 1.2;
        public static double BonusMax = 2.5;

        public static string Rule = new StringBuilder()
            .AppendLine("    🏇 赛 马 规 则 🏇").AppendLine()
            .AppendLine("1. 每匹🐎都有自己的隐藏潜力值，调查员可以通过自己的账户给对应的🐎下注，可以下注多只；")
            .AppendLine("2. 每个调查员，在一回合中只能杀同一匹🐎一次，无论成败，武器可以自选或者肉搏；")
            .AppendLine("3. 若杀🐎的调查员检定成功，且等级对抗赢了目标🐎，则造成伤害；")
            .AppendLine("4. 若一匹🐎的体力归零，则死亡，下注该马的调查员一无所获，而杀🐎者将获得该马身上的所有筹码；")
            .AppendLine("5. 当一批🐎（可能有多匹）冲过终点后，比赛结束，这些🐎视为胜利；")
            .AppendLine($"6. 每匹胜利的🐎，将会为它的下注者带来{BonusMin}到{BonusMax}倍于本金的奖金（包括本金）。")
            .ToString();

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.Register("赛马")
            .Then(
                Literal<DMEnv>("开始")
                .Then(
                    Int<DMEnv>("数量").Executes((env, args, dict) => Start(env, args.GetInt("数量")))
                )
            ).Then(
                Literal<DMEnv>("终止").Executes((env, args, dict) => { env.Sce.Horses.Clear(); return "赛马已终止！"; })
            ).Then(
                Literal<DMEnv>("步进").Executes((env, args, dict) => Step(env))
            ).Then(
                Literal<DMEnv>("显示").Executes((env, args, dict) => DisplayHorses(env.Sce.Horses))
            ).Then(
                Literal<DMEnv>("规则").Executes((env, args, dict) => Rule)
            ).Then(
                Literal<DMEnv>("下注")
                .Then(
                    Int<DMEnv>("马匹序号")
                    .Then(
                        Int<DMEnv>("金额")
                        .Executes((env, args, dict) => Bet(env, env.Inv, args.GetInt("马匹序号"), args.GetInt("金额")))
                    )
                )
            ).Then(
                Literal<DMEnv>("杀马")
                .Then(
                    Int<DMEnv>("马匹序号")
                    .Executes((env, args, dict) => Kill(env, env.Inv, args.GetInt("马匹序号"), null))
                    .Then(
                        String<DMEnv>("武器名")
                        .Executes((env, args, dict) => Kill(env, env.Inv, args.GetInt("马匹序号"), args.GetStr("武器名")))
                    )
                )
            );

            dispatcher.SetAlias("杀马", "赛马 杀马");
            dispatcher.SetAlias("下注", "赛马 下注");
            dispatcher.SetAlias("步进", "赛马 步进");
        }
        

        public static void Start(DMEnv env, int amount) {
            var horses = env.Sce.Horses;
            if (horses.Count > 0) {
                env.Next = "🏇已经开始！";
                return;
            }
            if (amount < 2 || amount > 10) {
                env.Next = "🐎匹不得少于2只且不得大于10只！";
                return;
            }

            for (int i = 0; i < amount; i++) {
                Horse horse = new Horse();
                horses.Add(horse);
            }
            env.Save();
            env.Next = DisplayHorses(horses);
        }

        public static void Step(DMEnv env) {
            var horses = env.Sce.Horses;
            var scenario = env.Sce;
            if (horses.Count == 0) {
                env.Next = "🏇还未开始！";
                return;
            }
            HashSet<int> winners = new HashSet<int>();
            for (int i = 0; i < horses.Count; i++) {
                if (horses[i].Step()) {
                    winners.Add(i);
                }
            }
            env.Append(DisplayHorses(horses));
            if (winners.Count > 0) {
                env.LineAppend("赢家：");
                Dictionary<string, int> profits = new Dictionary<string, int>();
                foreach (int index in winners) {
                    env.Append(Indices.ElementAt(index));
                    double bonus = BonusMin + Horse.Rand.NextDouble() * (BonusMax - BonusMin);
                    foreach (var e in horses[index].Bets) {
                        profits[e.Key] = (int)(e.Value * bonus) + (profits.ContainsKey(e.Key) ? profits[e.Key] : 0);
                    }
                }
                env.AppendLine("号🐎。");
                foreach (var e in profits) {
                    if (scenario.TryGetInvestigator(e.Key, out Investigator inv)) {
                        if (!inv.Values.TryWidelyGet("账户", out Value account)) {
                            account = new Value(0);
                            inv.Values.Put("账户", account);
                        }
                        env.AppendLine(inv.Change("账户", e.Value));
                    } else {
                        env.AppendLine($"未找到【{e.Key}】，很遗憾，他的奖金全没了");
                    }
                }
                horses.Clear();
                env.Append("🏇已结束！");
            }
            env.Save();
        }

        public static void Bet(DMEnv env, Investigator inv, int index, int amount) {
            var horses = env.Sce.Horses;
            if (horses.Count == 0) {
                env.Next = "🏇还未开始！";
                return;
            }
            if (amount < 0) {
                env.Next = "必须输入账户且必须大于零！";
                return;
            } else if (index <= 0 || index > horses.Count) {
                env.Next = $"找不到{index}号🐎";
                return;
            }
            if (!inv.Values.TryGet("账户", out Value account)) {
                env.Next = $"{inv.Name}没有账户";
                return;
            } else if (account.Val < amount) {
                env.Next = $"{inv.Name}的账户只有{account.Val}不足{amount}";
                return;
            }
            account.Add(-amount);
            Horse horse = horses[index - 1];
            if (horse.Bets.ContainsKey(inv.Name)) {
                horse.Bets[inv.Name] += amount;
            } else {
                horse.Bets[inv.Name] = amount;
            }
            env.Save();
            env.Next = $"{inv.Name}下注成功，对象：{index}号🐎，金额：{amount}，总金额：{horse.Bets[inv.Name]}";
        }

        public static void Kill(DMEnv env, Investigator source, int index, string weaponName) {
            List<Horse> horses = env.Sce.Horses;
            if (horses.Count == 0) {
                env.Next = "🏇已经结束！";
                return;
            }
            if (index <= 0 || index > horses.Count) {
                env.Next = $"找不到{index}号🐎";
                return;
            }

            Horse horse = horses[index - 1];
            string horseName = Indices[index - 1] + "号🐎";

            if (horse.Sources.Contains(source.Name)) {
                env.Next = $"{source.Name}本轮已经杀过此🐎了！";
                return;
            }

            Item w = source.GetItem(weaponName);

            horse.Sources.Add(source.Name);

            if (!source.Check(w.SkillName, out CheckResult sr, out string str)) {
                env.Save();
                env.Append(str);
                return;
            }
            env.AppendLine(str);
            if (!sr.succeed) {
                env.Save();
                env.Append("杀🐎失败，该回合不能再杀此🐎");
                return;
            }
            // 检定🐎的闪避
            CheckResult hr = new Value(horse.Ponential).Check();
            env.AppendLine($"{horseName} => {hr.points}，逃离{hr.TypeString}");
            if (hr.succeed && hr.type <= sr.type) {
                env.Save();
                env.Next = $"{source.Name}没有打中飞速移动中的{horseName}";
                return;
            }
            // 计算伤害
            int r = Dice.RollWith(w.Damage, source.DamageBonus);
            env.Append($"造成伤害：{r}");
            if (r > 0) {
                int prev = horse.Health;
                horse.Health = Math.Min(Math.Max(0, prev - r), Horse.MaxHealth);
                env.LineAppend($"{horseName}的体力：{prev} - {r} => {horse.Health}");
                if (horse.Health <= 0) {
                    int totalBets = 0;
                    foreach (int bet in horse.Bets.Values) {
                        totalBets += bet;
                    }
                    horse.Bets.Clear();
                    env.AppendLine($"{source.Name}获得{horseName}身上的所有筹码({totalBets})");
                    env.Append(source.Change("账户", totalBets));
                }
            }
            env.Save();
        }

        public static string Indices = "①②③④⑤⑥⑦⑧⑨⑩";
        public static string DisplayHorses(List<Horse> horses) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < horses.Count; i++) {
                sb.Append(Indices.ElementAt(i)).Append("号：").Append(horses[i].Display());
                if (i < horses.Count - 1) {
                    sb.AppendLine();
                }
            }
            return sb.ToString();
        }
    }
}
