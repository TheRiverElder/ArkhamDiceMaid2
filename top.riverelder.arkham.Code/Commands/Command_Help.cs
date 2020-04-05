using System.Collections.Generic;
using System.Text;

using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;

namespace top.riverelder.arkham.Code.Commands {
    class Command_Help : DiceCmdEntry {
        
        public string Usage => "帮助 [命令名称]";

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.Register("帮助")
            .Executes((env, args, dict) => PrintAllHelp())
            .Then(
                PresetNodes.String<DMEnv>("指令名")
                .Executes((env, args, dict) => PrintHelp(args.GetStr("指令名")))
            );
        }

        public static string PrintAllHelp() {
            StringBuilder builder = new StringBuilder().AppendLine("可用的命令集：");
            foreach (CommandNode<DMEnv> node in Global.Dispatcher.Commands) {
                builder.AppendLine();
                foreach (string s in node.GetHelp()) {
                    builder.AppendLine().Append(Global.Prefix).Append(s);
                }
            }
            return builder.ToString();
        }

        public static string PrintHelp(string head) {
            if (Global.Dispatcher.CommandMap.TryGetValue(head, out CommandNode<DMEnv> node)) {
                return string.Join("\n", node.GetHelp());
            }
            return $"未找到指令：{head}";
        }
    }
}
