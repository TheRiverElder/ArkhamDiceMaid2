using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code.Bot
{
    public class CompiledCommand
    {
        public readonly ICommand command;
        public readonly string[] listArgs;
        public readonly Dictionary<string, string> dictArgs;
        public readonly string originalString;
        public readonly long senderToken;


        public string Execute(CmdEnv env)
        {
            return command.Execute(listArgs, dictArgs, originalString, env);
        }

        public CompiledCommand(ICommand command, string[] listArgs, Dictionary<string, string> dictArgs, string originalString)
        {
            this.command = command;
            this.listArgs = listArgs;
            this.dictArgs = dictArgs;
            this.originalString = originalString;
            this.senderToken = senderToken;
        }
    }
}
