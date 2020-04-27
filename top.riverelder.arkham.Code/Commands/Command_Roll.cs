using System.Collections.Generic;
using System.Text;

using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;

namespace top.riverelder.arkham.Code.Commands {

    /// <summary>
    /// 简单地投掷一个骰子，骰子需要符合骰子表达式
    /// </summary>
    public class Command_Roll : DiceCmdEntry {

        public string Usage => "投掷 <骰子>";

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.Register("投掷")
            .Executes((env, args, dict) => Roll(env, Dice.Of("1d100")))
            .Then(
                Extensions.Dice("骰子").Executes((env, args, dict) => Roll(env, args.GetDice("骰子")))
            ).Rest(
                PresetNodes.String<DMEnv>("选项组")
                .Handles(Extensions.ConvertObjectArrayToStringArray())
                .Executes((env, args, dict) => Choose(args.Get<string[]>("选项组")))
            );

            dispatcher.SetAlias("r", "投掷");
        }

        public static string Roll(DMEnv env, Dice dice) {
            if (env.TryGetInv(out Scenario sce, out Investigator inv)) {
                return $"{dice.ToString()} => {dice.RollWith(inv.DamageBonus)}";
            } else {
                return $"{dice.ToString()} => {dice.Roll()}";
            }
        }

        public static string Choose(string[] results) {
            if (results.Length > 0) {
                int index = Dice.Roll(results.Length) - 1;
                if (index >= 0 && index < results.Length) {
                    return results[index];
                } else {
                    return "骰娘出错了๐·°(৹˃̵﹏˂̵৹)°·๐";
                }
            }
            return "没有选项哦";
        }
    }
}
