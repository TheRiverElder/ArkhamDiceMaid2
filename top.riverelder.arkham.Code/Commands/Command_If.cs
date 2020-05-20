using RiverCommand;
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

        public static string If(
            CompiledCommand<DMEnv> cond, 
            CompiledCommand<DMEnv> trueStmt, 
            CompiledCommand<DMEnv> falseStmt
        ) {
            object c = cond.Execute(out string reply);
            if (c is bool && (bool)c) {
                trueStmt.Execute(out reply);
            } else if (falseStmt != null) {
                falseStmt.Execute(out reply);
            }
            return reply;
        }
    }
}
