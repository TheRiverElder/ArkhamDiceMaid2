using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Bot;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;

namespace top.riverelder.arkham.Code.Commands {
    class Command_CreateScenario : ICommand {
        public string Name => "开团";

        public string Usage => "开团 <团名>";

        public ArgumentValidater Validater { get; } = ArgumentValidater.Empty
            .SetListArgCountMin(1)
            .SetListArgCountMax(2)
            .AddListArg(ArgumentValidater.Any)
            .AddListArg("强制");

        public string Execute(string[] listArgs, IDictionary<string, string> dictArgs, string originalString, CmdEnv env) {
            string name = listArgs[0];
            bool force = listArgs.Length > 1;

            if (string.Equals(name, env.ScenarioName) && !force)  {
                return "已经存在该团，若有要强行覆盖请添加‘强制’参数";
            }
            Scenario s = new Scenario(name);
            Global.Scenarios[name] = s;
            Global.Groups[env.Group] = s.Name;
            SaveUtil.Save(s);
            return $"【{name}】开团啦！";
        }
    }
}
