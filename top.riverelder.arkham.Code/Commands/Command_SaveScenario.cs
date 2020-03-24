using System.Collections.Generic;
using top.riverelder.arkham.Code.Bot;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;

namespace top.riverelder.arkham.Code.Commands
{
    class Command_SaveScenario : ICommand
    {
        public string Name => "存团";

        public ArgumentValidater Validater { get; } = ArgumentValidater.Empty
            .SetListArgCountMax(1)
            .AddListArg(ArgumentValidater.Any);

        public string Usage => "存团 [团名]";

        public string Execute(string[] listArgs, IDictionary<string, string> dictArgs, string originalString, CmdEnv env)
        {
            string name = listArgs.Length > 0 ? listArgs[0] : env.ScenarioName;
            if (Global.Scenarios.TryGetValue(name, out Scenario scenario))
            {
                SaveUtil.Save(scenario);
                return "保存完毕";
            }
            return $"未找到团：{name}";
        }
    }
}
