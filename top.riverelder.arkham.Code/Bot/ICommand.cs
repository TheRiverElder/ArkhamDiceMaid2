using System.Collections.Generic;
using top.riverelder.arkham.Code.Model;

namespace top.riverelder.arkham.Code.Bot
{
    public interface ICommand
    {
        string Name { get; }
        string Usage { get; }
        ArgumentValidater Validater { get; }

        string Execute(string[] listArgs, IDictionary<string, string> dictArgs, string originalString, CmdEnv env);
    }
}
