using System.Collections.Generic;
using System.Text;
using top.riverelder.arkham.Code.Bot;
using top.riverelder.arkham.Code.Model;

namespace top.riverelder.arkham.Code.Commands {
    class Command_Help : ICommand {

        public string Name => "帮助";

        public ArgumentValidater Validater { get; } = ArgumentValidater.Empty
            .SetListArgCountMax(1);

        public string Usage => "帮助 [命令名称]";

        public string Execute(string[] listArgs, IDictionary<string, string> dictArgs, string originalString, CmdEnv env) {

            if (listArgs.Length > 0) {
                string head = listArgs[0];
                if (Global.Dispatcher.Heads.Contains(head)) {
                    return Global.Dispatcher[head].Usage;
                }
                return $"未找到指令：{head}";
            }

            StringBuilder builder = new StringBuilder().AppendLine("可用的命令集：");
            foreach (ICommand command in Global.Dispatcher.Commands) {
                builder.AppendLine().Append(Global.Prefix).Append(command.Usage);
            }

            return builder.ToString();
        }
    }
}
