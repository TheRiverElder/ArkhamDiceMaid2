using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;
using top.riverelder.RiverCommand.Parsing;

namespace top.riverelder.arkham.Code.Commands {
    public class Command_Heal : DiceCmdEntry {

        public string Usage => "治疗 <技能名> <目标> <回血量>";
        
        public static bool Heal(DMEnv env, Investigator inv, Investigator target, string valueName, string healValueName, Dice dice) {
            Scenario scenario = env.Sce;
            if (!inv.Check(valueName, out CheckResult cr, out string str)) {
                env.Next = str;
                return false;
            }
            env.Append(str);
            if (cr.succeed) {
                int r = dice.Roll();
                env.LineAppend(target.Change(healValueName, r));
                SaveUtil.Save(scenario);
            }
            return cr.succeed;
        }

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.Register("治疗")
            .Then(
                PresetNodes.String<DMEnv>("技能名")
                .Handles(Extensions.ExistSelfValue)
                .Then(
                    PresetNodes.String<DMEnv>("目标")
                    .Handles(Extensions.ExistInv)
                    //.Executes((env, args, dict) => Heal(env, env.Inv, args.GetInv("目标"), args.GetStr("技能名"), "体力", Dice.Of("1d3")))
                    .Then(
                        Extensions.Dice("增量")
                        .Executes((env, args, dict) => Heal(env, env.Inv, args.GetInv("目标"), args.GetStr("技能名"), "体力", args.GetDice("增量")))
                        .Then(
                            PresetNodes.String<DMEnv>("回复数值名")
                            .Executes((env, args, dict) => Heal(env, env.Inv, args.GetInv("目标"), args.GetStr("技能名"), args.GetStr("回复数值名"), args.GetDice("增量")))
                        )
                    )
                )
            );

            dispatcher.SetAlias("医学", "治疗 医学");
            dispatcher.SetAlias("急救", "治疗 急救");
        }
    }
}
