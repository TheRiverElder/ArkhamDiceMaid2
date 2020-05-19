using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;

namespace top.riverelder.arkham.Code.Commands {
    public class Command_If : DiceCmdEntry {
        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.Register("如果")
            .Then(
                PresetNodes.Cmd<DMEnv>("条件")
                .Then(
                    PresetNodes.Cmd<DMEnv>("真值指令")
                    .Then(
                        PresetNodes.Cmd<DMEnv>("假值指令")
                    )
                )
            );
        }
    }
}
