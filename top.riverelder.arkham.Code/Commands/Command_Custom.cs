using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;

namespace top.riverelder.arkham.Code.Commands {
    public class Command_Custom : DiceCmdEntry {

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.RegisterCustom(
                Extensions.Dice("骰子")
                .Executes((env, args, dict) => {
                    Dice dice = args.GetDice("骰子");
                    if (env.TryGetInv(out Scenario sce, out Investigator inv)) {
                        return $"{dice.ToString()} => {dice.RollWith(inv.DamageBonus)}";
                    } else {
                        return $"{dice.ToString()} => {dice.Roll()}";
                    }
                })
            );
            dispatcher.RegisterCustom(Command_Check.MainAction);
        }
    }
}
