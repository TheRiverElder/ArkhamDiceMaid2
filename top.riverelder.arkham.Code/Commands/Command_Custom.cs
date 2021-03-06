﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;
using top.riverelder.RiverCommand.Parsing;

namespace top.riverelder.arkham.Code.Commands {
    public class Command_Custom : DiceCmdEntry {

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.RegisterCustom(Command_Roll.MainAction);
            dispatcher.RegisterCustom(Command_Check.MainAction);
            dispatcher.RegisterCustom(
                PresetNodes.String<DMEnv>("卡名")
                .Handles(Extensions.ExistInv)
                .Then(
                    PresetNodes.Cmd<DMEnv>("行动")
                    .Executes((env, args, dict) => Command_Control.ControlAndAct(env, args.GetInv("卡名"), args.GetCmd("行动")))
                )
            );
        }
    }
}
