using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;

namespace top.riverelder.arkham.Code.Commands {
    public class Command_Heal : DiceCmdEntry {

        public string Usage => "治疗 <技能名> <目标> <回血量>";
        
        public static string Heal(DMEnv env, Investigator inv, Investigator target, string valueName, Dice dice) {
            Scenario scenario = env.Sce;
            if (!target.Values.TryWidelyGet("体力", out Value health)) {
                return $"未找到{target.Name}的体力";
            }
            Value skill = inv.Values[valueName];
            StringBuilder sb = new StringBuilder();
            sb.Append($"{inv.Name}检定{valueName}({skill.Val})：");
            int prev = health.Val;
            CheckResult cr = skill.Check();
            sb.Append(cr.TypeString);
            if (cr.succeed) {
                int r = dice.Roll();
                sb.AppendLine().Append($"{target.Name}的体力：{prev} + {r}({dice.ToString()}) => {health.Add(r)}");
                SaveUtil.Save(scenario);
            }
            return sb.ToString();
        }

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.Register("治疗")
            .Then(
                PresetNodes.String<DMEnv>("技能名")
                .Handles(Extensions.ExistSelfValue())
                .Then(
                    PresetNodes.String<DMEnv>("目标")
                    .Handles(Extensions.ExistInv())
                    .Executes((env, args, dict) => Heal(env, env.Inv, args.GetInv("目标"), args.GetStr("技能名"), Dice.Of("1d3")))
                    .Then(
                        Extensions.Dice("增量")
                        .Executes((env, args, dict) => Heal(env, env.Inv, args.GetInv("目标"), args.GetStr("技能名"), args.GetDice("增量")))
                    )
                )
            );

            dispatcher.SetAlias("医学", "治疗 医学");
            dispatcher.SetAlias("急救", "治疗 急救");
        }
    }
}
