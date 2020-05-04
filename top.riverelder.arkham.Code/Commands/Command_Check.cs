using System.Collections.Generic;
using System.Text;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.arkham.Code.Model;
using top.riverelder.RiverCommand;

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
                .Handles(Extensions.ExistSelfValue())
                .Executes((env, args, dict) => SimpleCheck(env.Inv, args.GetStr("数值名"), CheckResult.NormalSuccess))
                .Then(
                    PresetNodes.Or<DMEnv>("难度", "普通", "困难", "极难")
                    .Handles(PreProcesses.Mapper<DMEnv>(hardnessMap))
                    .Executes((env, args, dict) => SimpleCheck(env.Inv, args.GetStr("数值名"), args.GetInt("难度")))
                    .Then(
                        PresetNodes.Or<DMEnv>("奖惩", "奖励", "惩罚")
                        .Handles(PreProcesses.Mapper<DMEnv>(twiceMap))
                        .Executes((env, args, dict) => TwiceCheck(env.Inv, args.GetStr("数值名"), args.GetBool("奖惩"), args.GetInt("难度")))
                    )
                ).Then(
                    PresetNodes.Or<DMEnv>("奖惩", "奖励", "惩罚")
                    .Handles(PreProcesses.Mapper<DMEnv>(twiceMap))
                    .Executes((env, args, dict) => TwiceCheck(env.Inv, args.GetStr("数值名"), args.GetBool("奖惩"), CheckResult.NormalSuccess))
                ).Then(
                    PresetNodes.Literal<DMEnv>("对抗").Then(
                        PresetNodes.String<DMEnv>("对手名")
                        .Handles(Extensions.ExistInv())
                        .Executes((env, args, dict) => CheckAgainst(env.Inv, args.GetStr("数值名"), args.GetInv("对手名"), args.GetStr("数值名"), false))
                        .Then(
                            PresetNodes.String<DMEnv>("对抗数值名")
                            .Executes((env, args, dict) => CheckAgainst(env.Inv, args.GetStr("数值名"), args.GetInv("对手名"), args.GetStr("对抗数值名"), false))
                        )
                    )
                ).Then(
                    PresetNodes.Literal<DMEnv>("等级对抗").Then(
                        PresetNodes.String<DMEnv>("对手名")
                        .Handles(Extensions.ExistInv())
                        .Executes((env, args, dict) => CheckAgainst(env.Inv, args.GetStr("数值名"), args.GetInv("对手名"), args.GetStr("数值名"), true))
                        .Then(
                            PresetNodes.String<DMEnv>("对抗数值名")
                            .Executes((env, args, dict) => CheckAgainst(env.Inv, args.GetStr("数值名"), args.GetInv("对手名"), args.GetStr("对抗数值名"), true))
                        )
                    )
                ).Then(
                    PresetNodes.String<DMEnv>("目标")
                    .Executes((env, args, dict) => SimpleCheckTo(env.Inv, args.GetStr("数值名"), args.GetStr("目标")))
                )
            );

            dispatcher.SetAlias("ch", "检定");
        }

        public static string SimpleCheckTo(Investigator inv, string valueName, string target) {
            CheckResult result = inv.Values[valueName].Check();

            return new StringBuilder()
                .AppendLine($"{inv.Name}的{valueName}：")
                .AppendLine(GenResultStr(inv, result, true))
                .Append(result.succeed ? $"{inv.Name}把{target}给{valueName}了！" : "")
                .ToString();
        }

        public static string SimpleCheck(Investigator inv, string valueName, int hardness) {
            CheckResult result = inv.Values[valueName].Check(hardness);
            string hardnessStr;
            switch (hardness) {
                case CheckResult.ExtremeSuccess: hardnessStr = "极难"; break;
                case CheckResult.HardSuccess: hardnessStr = "困难"; break;
                case CheckResult.NormalSuccess: hardnessStr = "普通"; break;
                default: hardnessStr = "普通"; break;
            }

            return new StringBuilder()
                .AppendLine($"{inv.Name}的{hardnessStr}{valueName}：")
                .Append(GenResultStr(inv, result, true))
                .ToString();
        }

        public static string TwiceCheck(Investigator inv, string valueName, bool isBonus, int hardness) {
            Value value = inv.Values[valueName];
            CheckResult bigger = value.Check(hardness);
            CheckResult smaller = value.Check(hardness);
            if (bigger.result < smaller.result) {
                CheckResult tmp = bigger;
                bigger = smaller;
                smaller = tmp;
            }
            if (isBonus) {
                return new StringBuilder()
                    .AppendLine($"{inv.Name}的奖励{valueName}：")
                    .Append(GenResultStr(inv, smaller, false) + $"({bigger.result})，" + smaller.ActualTypeString)
                    .ToString();
            } else {
                return new StringBuilder()
                    .AppendLine($"{inv.Name}的惩罚{valueName}：")
                    .Append(GenResultStr(inv, bigger, false) + $"({smaller.result})，" + bigger.ActualTypeString)
                    .ToString();
            }
        }

        public static string CheckAgainst(Investigator inv, string valueName, Investigator target, string againstValueName, bool byLevel) {
            CheckResult selfResult = inv.Values[valueName].Check();
            if (!target.Values.TryWidelyGet(againstValueName, out Value againstValue)) {
                return $"{target.Name}没有{againstValueName}";
            }
            CheckResult targetResult = againstValue.Check();
            string resultStr = null;
            if (byLevel) {
                if (!selfResult.succeed) {
                    resultStr = "完败";
                } else if (selfResult.type == targetResult.type) {
                    resultStr = "平分秋色";
                } else {
                    resultStr = selfResult.type < targetResult.type ? "完胜" : "完败";
                }
            } else {
                if (!selfResult.succeed) {
                    resultStr = "失败";
                } else if (selfResult.result == targetResult.result) {
                    resultStr = "平局";
                } else {
                    resultStr = selfResult.result < targetResult.result ? "胜利" : "失败";
                }
            }
            
            return new StringBuilder()
                .AppendLine($"{inv.Name}的{valueName}：" + GenResultStr(inv, selfResult, false))
                .AppendLine($"{target.Name}的{againstValueName}：" + GenResultStr(target, targetResult, false))
                .Append(inv.Name).Append('：').Append(resultStr)
                .ToString();
        }

        public static string GenResultStr(Investigator inv, CheckResult result, bool showResultTypeStr) {
            return (inv.Is("NPC") ? "(???)" : $"({result.target}/{result.value})") +
                $" => {result.result}" + 
                (showResultTypeStr ? "，" + result.ActualTypeString : "");
        }

    }
}
