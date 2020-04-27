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
            dispatcher.RegesterCustom(
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
            dispatcher.RegesterCustom(
                PresetNodes.String<DMEnv>("数值名")
                .Handles(Extensions.ExistSelfValue())
                .Executes((env, args, dict) => {
                    Investigator inv = env.Inv;
                    string name = args.GetStr("数值名");
                    Value value = inv.Values[name];
                    CheckResult result = value.Check();
                    return $"{inv.Name}的{name}：{value.Val} => {result.result}，{result.ActualTypeString}";
                })
            );
        }
    }
}
