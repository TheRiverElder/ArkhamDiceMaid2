using System.Collections.Generic;
using System.Text;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.arkham.Code.Model;
using top.riverelder.RiverCommand;

namespace top.riverelder.arkham.Code.Commands {

    class Command_Check : DiceCmdEntry {

        public string Usage => "检定 <数值名> [普通|困难|极难|对抗|奖励|惩罚] [对手名] [对抗数值名]";

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
                PresetNodes.String<DMEnv>("数值名")
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
                        .Executes((env, args, dict) => CheckAgainst(env.Inv, args.GetStr("数值名"), args.GetInv("对手名"), args.GetStr("数值名")))
                        .Then(
                            PresetNodes.String<DMEnv>("对抗数值名")
                            .Executes((env, args, dict) => CheckAgainst(env.Inv, args.GetStr("数值名"), args.GetInv("对手名"), args.GetStr("对抗数值名")))
                        )
                    )
                ).Then(
                    PresetNodes.String<DMEnv>("目标")
                    .Executes((env, args, dict) => SimpleCheckTo(env.Inv, args.GetStr("数值名"), args.GetStr("目标")))
                )
            );
        }

        public string RandomResult(string[] results) {
            int index = Dice.Roll("1d" + results.Length) - 1;
            return results[index];
        }

        public string SimpleCheckTo(Investigator inv, string valueName, string target) {
            CheckResult result = inv.Values[valueName].Check();

            return new StringBuilder()
                .AppendLine($"{inv.Name}的{valueName}：")
                .AppendLine($"({result.target}/{result.value}) => {result.result}，{result.ActualTypeString}")
                .Append(result.succeed ? $"{inv.Name}把{target}给{valueName}了！" : "")
                .ToString();
        }

        public string SimpleCheck(Investigator inv, string valueName, int hardness) {
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
                .Append($"({result.target}/{result.value}) => {result.result}，{result.ActualTypeString}")
                .ToString();
        }

        public string TwiceCheck(Investigator inv, string valueName, bool isBonus, int hardness) {
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
                    .Append($"({smaller.value}) => {smaller.result}({bigger.result})，{smaller.ActualTypeString}")
                    .ToString();
            } else {
                return new StringBuilder()
                    .AppendLine($"{inv.Name}的惩罚{valueName}：")
                    .Append($"({bigger.value}) => {bigger.result}({smaller.result})，{bigger.ActualTypeString}")
                    .ToString();
            }
        }

        public string CheckAgainst(Investigator inv, string valueName, Investigator target, string againstValueName) {
            CheckResult selfResult = inv.Values[valueName].Check();
            if (!target.Values.TryWidelyGet(againstValueName, out Value againstValue)) {
                return $"{target.Name}没有{againstValueName}";
            }
            CheckResult targetResult = againstValue.Check();
            
            return new StringBuilder()
                .AppendLine($"{inv.Name}的{valueName}：({selfResult.value}) => {selfResult.result}")
                .AppendLine($"{target.Name}的{againstValueName}：({targetResult.value}) => {targetResult.result}")
                .Append(inv.Name).Append('：').Append(selfResult.result <= targetResult.result ? "胜利" : "失败")
                .ToString();
        }

    }
}
