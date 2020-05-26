using RiverCommand;
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
    public class Command_If : DiceCmdEntry {
        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.Register("如果")
            .Then(
                PresetNodes.Cmd<DMEnv>("条件")
                .Then(
                    PresetNodes.Cmd<DMEnv>("真值指令")
                    .Executes((env, args, dict) => If(args.GetCmd("条件"), args.GetCmd("真值指令"), null))
                    .Then(
                        PresetNodes.Cmd<DMEnv>("假值指令")
                        .Executes((env, args, dict) => If(args.GetCmd("条件"), args.GetCmd("真值指令"), args.GetCmd("假值指令")))
                    )
                )
            );

            dispatcher.SetAlias("若", "如果");
            dispatcher.SetAlias("if", "如果");
        }

        public static object If(
            CommandResult<DMEnv> cond,
            CommandResult<DMEnv> trueStmt,
            CommandResult<DMEnv> falseStmt
        ) {
            object c = cond.Execute();
            cond.Env.Line();
            if (c is bool && (bool)c) {
                return trueStmt.Execute();
            } else if (falseStmt != null) {
                return falseStmt.Execute();
            }
            return null;
        }
    }
}
