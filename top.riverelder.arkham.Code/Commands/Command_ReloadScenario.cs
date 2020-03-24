using System;
using System.Collections.Generic;
using top.riverelder.arkham.Code.Bot;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;

namespace top.riverelder.arkham.Code.Commands
{
    class Command_ReloadScenario : ICommand
    {
        public string Name => "读团";

        public ArgumentValidater Validater { get; } = ArgumentValidater.Empty
            .SetListArgCountMin(0)
            .SetListArgCountMax(1)
            .AddListArg(ArgumentValidater.Any);

        public string Usage => "读团 [团名]";

        public string Execute(string[] listArgs, IDictionary<string, string> dictArgs, string originalString, CmdEnv env)
        {
            string name = listArgs.Length > 0 ? listArgs[0] : env.ScenarioName;
            try
            {
                if (SaveUtil.TryLoad(name, out Scenario scenario))
                {
                    Global.Scenarios[name] = scenario;
                    Global.Groups[env.Group] = scenario.Name;
                    return $"读团完毕：{name}";
                }
            }
            catch (Exception e)
            {
                return "读团失败，原因：" + e.Message;
            }
            return $"读团失败：{name}";
        }
    }
}
