using System.Collections.Generic;
using top.riverelder.arkham.Code.Bot;
using top.riverelder.arkham.Code.Utils;

namespace top.riverelder.arkham.Code.Commands
{
    class Command_Global : ICommand
    {
        public string Name => "配置";

        public ArgumentValidater Validater { get; } = ArgumentValidater.Empty
            .SetListArgCount(1)
            .AddListArg("载入|保存");

        public string Usage => "配置 <载入|保存>";

        public string Execute(string[] listArgs, IDictionary<string, string> dictArgs, string originalString, CmdEnv env)
        {
            string opt = listArgs[0];
            if ("载入".Equals(opt))
            {
                SaveUtil.LoadGlobal();
                return "配置载入完毕";
            }
            else
            {
                SaveUtil.SaveGlobal();
                return "配置保存完毕";
            }
        }
    }
}
