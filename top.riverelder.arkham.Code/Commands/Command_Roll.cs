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
                .Then(
                    Extensions.Dice<DMEnv>("骰子").Executes((env, args, dict) => Roll(args.GetDice("骰子")))
                );
        }

        public static string Roll(Dice dice) {
            return $"{dice.ToString()} = {dice.Roll()}";
        }
    }
}
