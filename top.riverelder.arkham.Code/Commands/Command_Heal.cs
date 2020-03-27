using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Bot;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;

namespace top.riverelder.arkham.Code.Commands {
    public class Command_Heal : ICommand {
        public string Name => "治疗";

        public string Usage => "治疗 <技能名> <目标> <回血量>";

        public ArgumentValidater Validater { get; } = ArgumentValidater.Empty
            .SetListArgCount(3)
            .AddListArg(ArgumentValidater.Any)
            .AddListArg(ArgumentValidater.Any)
            .AddListArg(ArgumentValidater.Dice);

        public string Execute(string[] listArgs, IDictionary<string, string> dictArgs, string originalString, CmdEnv env) {
            if (!EnvValidator.ExistInv(env, out Investigator inv, out string err)) {
                return err;
            }

            string skillName = listArgs[0];
            string targetName = listArgs[1];
            string dice = listArgs[2];
            //string hardness = listArgs.Length > 2 ? listArgs[2] : "普通";

            Scenario scenario = env.Scenario;
            if (!scenario.TryGetInvestigator(targetName, out Investigator target)) {
                return "目标不存在：" + targetName;
            }
            if (!target.Values.TryWidelyGet("体力", out Value health)) {
                return $"未找到{target.Name}的体力";
            }
            if (!inv.Values.TryWidelyGet(skillName, out Value skill)) {
                return $"未找到{inv.Name}的{skillName}";
            }
            StringBuilder sb = new StringBuilder();
            sb.Append($"{inv.Name}检定{skill.Name}({skill.Val})：");
            int prev = health.Val;
            CheckResult cr = skill.Check();
            sb.Append(cr.TypeString);
            if (cr.succeed) {
                int r = Dice.Roll(dice);
                sb.AppendLine().Append($"{target.Name}的体力：{prev} + {r} => {health.Add(r)}");
            }
            return sb.ToString();
        }
    }
}
