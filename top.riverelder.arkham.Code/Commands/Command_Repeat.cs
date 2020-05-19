using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;

namespace top.riverelder.arkham.Code.Commands {
    /// <summary>
    /// 将一个指令重复n遍
    /// </summary>
    public class Command_Repeat : DiceCmdEntry {
        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.Register("重复").Then(
                PresetNodes.Int<DMEnv>("次数").Then(
                    PresetNodes.Rest<DMEnv>("命令").Executes((env, args, dict) => Repeat(env, args.GetStr("命令"), args.GetInt("次数")))
                )
            );
        }

        public static string Repeat(DMEnv env, string rawCmd, int times) {
            StringBuilder sb = new StringBuilder();
            string reply;
            for (int i = 0; i < times; i++) {
                if (i != 0) {
                    sb.AppendLine();
                }
                Global.Dispatcher.Execute(rawCmd, env, out object ret, out reply);
                sb.Append(reply);
            }
            return sb.ToString();
        }
    }
}
