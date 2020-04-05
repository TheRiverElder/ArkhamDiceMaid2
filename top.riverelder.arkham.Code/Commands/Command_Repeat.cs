using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;

namespace top.riverelder.arkham.Code.Commands {
    public class Command_Repeat : DiceCmdEntry {
        public override void OnRegister(CmdDispatcher<DiceMaidEnv> dispatcher) {
            dispatcher.Register("重复").Then(
                PresetNodes.Int<DiceMaidEnv>("次数").Then(
                    PresetNodes.Rest<DiceMaidEnv>("命令").Executes((env, args, dict) => Repeat(env, args.GetStr("命令"), args.GetInt("次数")))
                )
            );
        }

        public static string Repeat(DiceMaidEnv env, string rawCmd, int times) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < times; i++) {
                if (i != 0) {
                    sb.AppendLine();
                }
                Global.Dispatcher.Dispatch(rawCmd, env, out string reply);
                sb.Append(reply);
            }
            return sb.ToString();
        }
    }
}
