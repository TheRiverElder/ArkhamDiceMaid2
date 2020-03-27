using System.Collections.Generic;
using System.Text;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Bot;

namespace top.riverelder.arkham.Code.Commands {

    class Command_Check : ICommand {

        public string Name => "检定";

        public ArgumentValidater Validater { get; } = ArgumentValidater.Empty
                .SetListArgCountMin(1)
                .SetListArgCountMax(4)
                .AddListArg(null)
                .AddListArg("普通|困难|极难|对抗|奖励|惩罚");

        public string Usage => "检定 <数值名> [普通|困难|极难|对抗|奖励|惩罚] [对手名] [对抗数值名]";

        public string Execute(string[] listArgs, IDictionary<string, string> dictArgs, string originalString, CmdEnv env) {
            string valueName = listArgs[0];
            string hardness = listArgs.Length > 1 ? listArgs[1] : "普通";
            string opposite = listArgs.Length > 2 ? listArgs[2] : null;
            string againstValueName = listArgs.Length > 3 ? listArgs[3] : null;

            if (!EnvValidator.ExistValue(env, valueName, out Value value, out string err)) {
                return err;
            }

            Scenario scenario = env.Scenario;
            Investigator inv = env.Investigator;

            StringBuilder builder = new StringBuilder();
            
            if ("对抗".Equals(hardness)) {
                if (string.IsNullOrEmpty(opposite)) {
                    return $"对抗需输入对手名";
                }
                if (!scenario.TryGetInvestigator(opposite, out Investigator target)) {
                    return $"未找到对手：{opposite}";
                } else {
                    if (string.IsNullOrEmpty(againstValueName)) {
                        againstValueName = valueName;
                    }
                    if (!target.Values.TryGet(againstValueName, out Value againstValue)) {
                        return $"未找到{opposite}的对抗属性：{againstValueName}";
                    } else {
                        builder.AppendLine($"{inv.Name}对抗{opposite}的{againstValueName}({againstValue.Val})");
                        int val = againstValue.Val;
                        if (val < 50) {
                            hardness = "普通";
                        } else if (val < 90) {
                            hardness = "困难";
                        } else {
                            hardness = "极难";
                        }
                    }
                }
            }

            CheckResult result = null;

            if ("奖励" == hardness || "惩罚" == hardness) {
                result = value.Check(CheckResult.NormalSuccess);
                CheckResult result2 = value.Check(CheckResult.NormalSuccess);
                CheckResult smaller = result.result <= result2.result ? result : result2;
                CheckResult larger = result.result >= result2.result ? result : result2;
                if ("奖励" == hardness) {
                    result = smaller;
                    builder.AppendLine($"奖励取小：{smaller.result} <= {larger.result}");
                } else {
                    result = larger;
                    builder.AppendLine($"惩罚取大：{larger.result} >= {smaller.result}");
                }
            } else {
                switch (hardness) {
                    case "普通": result = value.Check(CheckResult.NormalSuccess); break;
                    case "困难": result = value.Check(CheckResult.HardSuccess); break;
                    case "极难": result = value.Check(CheckResult.ExtremeSuccess); break;
                    default : result = value.Check(CheckResult.NormalSuccess); break;
                }
            }

            builder.AppendLine($"{inv.Name}检定{hardness}{valueName}({result.target}/{result.value})：");
            builder.Append($"{result.dice} = {result.result}, 判定为 {result.ActualTypeString}");

            return builder.ToString();
        }
    }
}
