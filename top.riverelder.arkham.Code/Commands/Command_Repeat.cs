using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;
using top.riverelder.RiverCommand.Parsing;

namespace top.riverelder.arkham.Code.Commands {
    /// <summary>
    /// 将一个指令重复n遍
    /// </summary>
    public class Command_Repeat : DiceCmdEntry {
        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.Register("重复").Then(
                PresetNodes.Int<DMEnv>("次数").Then(
                    PresetNodes.Cmd<DMEnv>("命令").Executes((env, args, dict) => Repeat(env, args.GetCmd("命令"), args.GetInt("次数")))
                )
            );
        }

        public static void Repeat(DMEnv env, CommandResult<DMEnv> cmd, int times) {
            for (int i = 0; i < times; i++) {
                if (i != 0) {
                    env.Line();
                }
                cmd.Execute();
            }
        }
    }
}
