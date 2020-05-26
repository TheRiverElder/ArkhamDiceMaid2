using System.Collections.Generic;
using System.Text;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.arkham.Code.Model;
using top.riverelder.RiverCommand;
using top.riverelder.RiverCommand.Parsing;

namespace top.riverelder.arkham.Code.Commands {

    class Command_Check : DiceCmdEntry {

        public string Usage => "检定 <数值名> [普通|困难|极难|对抗|奖励|惩罚] [对手名] [对抗数值名]";

        public static CommandNode<DMEnv> MainAction = null;

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            IDictionary<object, object> hardnessMap = new Dictionary<object, object> {
                ["普通"] = CheckResult.NormalSuccess,
                ["困难"] = CheckResult.HardSuccess,
                ["极难"] = CheckResult.ExtremeSuccess,
            };
            IDictionary<object, object> twiceMap = new Dictionary<object, object> {
                ["奖励"] = true,
                ["惩罚"] = false,
            };
            dispatcher.Register("检定").Then(
                MainAction = PresetNodes.String<DMEnv>("数值名")
                .Handles(Extensions.ExistSelfValue)
                .Executes((DMEnv env, Args args, Args dict) => SimpleCheck(env, env.Inv, args.GetStr("数值名"), CheckResult.NormalSuccess))
                .Then(
                    PresetNodes.Or<DMEnv>("难度", "普通", "困难", "极难")
                    .Handles(PreProcesses.MapArg<DMEnv>(hardnessMap))
                    .Executes((DMEnv env, Args args, Args dict) => SimpleCheck(env, env.Inv, args.GetStr("数值名"), args.GetInt("难度")))
                    .Then(
                        PresetNodes.Or<DMEnv>("奖惩", "奖励", "惩罚")
                        .Handles(PreProcesses.MapArg<DMEnv>(twiceMap))
                        .Executes((DMEnv env, Args args, Args dict) => TwiceCheck(env, env.Inv, args.GetStr("数值名"), args.GetBool("奖惩"), args.GetInt("难度")))
                    )
                ).Then(
                    PresetNodes.Or<DMEnv>("奖惩", "奖励", "惩罚")
                    .Handles(PreProcesses.MapArg<DMEnv>(twiceMap))
                    .Executes((DMEnv env, Args args, Args dict) => TwiceCheck(env, env.Inv, args.GetStr("数值名"), args.GetBool("奖惩"), CheckResult.NormalSuccess))
                ).Then(
                    PresetNodes.Literal<DMEnv>("对抗").Then(
                        PresetNodes.String<DMEnv>("对手名")
                        .Handles(Extensions.ExistInv)
                        .Executes((DMEnv env, Args args, Args dict) => CheckAgainst(env, env.Inv, args.GetStr("数值名"), args.GetInv("对手名"), args.GetStr("数值名"), false))
                        .Then(
                            PresetNodes.String<DMEnv>("对抗数值名")
                            .Executes((DMEnv env, Args args, Args dict) => CheckAgainst(env, env.Inv, args.GetStr("数值名"), args.GetInv("对手名"), args.GetStr("对抗数值名"), false))
                        )
                    )
                ).Then(
                    PresetNodes.Literal<DMEnv>("等级对抗").Then(
                        PresetNodes.String<DMEnv>("对手名")
                        .Handles(Extensions.ExistInv)
                        .Executes((DMEnv env, Args args, Args dict) => CheckAgainst(env, env.Inv, args.GetStr("数值名"), args.GetInv("对手名"), args.GetStr("数值名"), true))
                        .Then(
                            PresetNodes.String<DMEnv>("对抗数值名")
                            .Executes((DMEnv env, Args args, Args dict) => CheckAgainst(env, env.Inv, args.GetStr("数值名"), args.GetInv("对手名"), args.GetStr("对抗数值名"), true))
                        )
                    )
                ).Then(
                    PresetNodes.String<DMEnv>("目标")
                    .Executes((DMEnv env, Args args, Args dict) => SimpleCheckTo(env, env.Inv, args.GetStr("数值名"), args.GetStr("目标")))
                )
            );

            dispatcher.SetAlias("ch", "检定");
        }

        public static bool SimpleCheck(DMEnv env, Investigator inv, string valueName, int hardness) {
            CheckResult result = inv.Values[valueName].Check(hardness);
            string hardnessStr;
            switch (hardness) {
                case CheckResult.ExtremeSuccess: hardnessStr = "极难"; break;
                case CheckResult.HardSuccess: hardnessStr = "困难"; break;
                case CheckResult.NormalSuccess: hardnessStr = "普通"; break;
                default: hardnessStr = "普通"; break;
            }

            env.AppendLine($"{inv.Name}的{hardnessStr}{valueName}：")
                .Append(GenResultStr(inv, result, true));
            return result.succeed;
        }

        public static bool SimpleCheckTo(DMEnv env, Investigator inv, string valueName, string target) {
            if (!inv.Check(valueName, out CheckResult result, out string str)) {
                env.Append(str);
                return false;
            }
            env.Append(str + (result.succeed ? $"，{inv.Name}把{target}给{valueName}了！" : ""));
            return result.succeed;
        }

        public static bool TwiceCheck(DMEnv env, Investigator inv, string valueName, bool isBonus, int hardness) {
            Value value = inv.Values[valueName];
            CheckResult bigger = value.Check(hardness);
            CheckResult smaller = value.Check(hardness);
            if (bigger.points < smaller.points) {
                CheckResult tmp = bigger;
                bigger = smaller;
                smaller = tmp;
            }
            if (isBonus) {
                env.AppendLine($"{inv.Name}的奖励{valueName}：")
                    .Append(GenResultStr(inv, smaller, false) + $"({bigger.points})，" + smaller.ActualTypeString);
                return smaller.succeed;
            } else {
                env.AppendLine($"{inv.Name}的惩罚{valueName}：")
                    .Append(GenResultStr(inv, bigger, false) + $"({smaller.points})，" + bigger.ActualTypeString);
                return bigger.succeed;
            }
        }

        public static bool CheckAgainst(DMEnv env, Investigator inv, string valueName, Investigator target, string againstValueName, bool byLevel) {
            CheckResult selfResult = inv.Values[valueName].Check();
            if (!target.Values.TryWidelyGet(againstValueName, out Value againstValue)) {
                env.Append($"{target.Name}没有{againstValueName}");
                return false;
            }
            CheckResult targetResult = againstValue.Check();
            string resultStr = null;
            bool r = false;
            if (byLevel) {
                if (!selfResult.succeed) {
                    resultStr = "完败";
                    r = false;
                } else if (selfResult.type == targetResult.type) {
                    resultStr = "平分秋色";
                    r = true;
                } else {
                    resultStr = (r = selfResult.type < targetResult.type) ? "完胜" : "完败";
                }
            } else {
                if (!selfResult.succeed) {
                    resultStr = "失败";
                    r = false;
                } else if (selfResult.points == targetResult.points) {
                    resultStr = "平局";
                    r = true;
                } else {
                    resultStr = (r = selfResult.points < targetResult.points) ? "胜利" : "失败";
                }
            }

            env
                .AppendLine($"{inv.Name}的{valueName}：" + GenResultStr(inv, selfResult, false))
                .AppendLine($"{target.Name}的{againstValueName}：" + GenResultStr(target, targetResult, false))
                .Append(inv.Name).Append("：").Append(resultStr);
            return r;
        }
        
        public static string GenResultStr(Investigator inv, CheckResult result, bool showTypeString) {
            return 
                (inv.Is("HIDE_VALUE") ? "(???)" : $"({result.target}/{result.value})") +
                $" => {result.points}" +
                (showTypeString ? $"，{result.ActualTypeString}" : "");
        }

    }
}
