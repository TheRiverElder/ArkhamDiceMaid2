using System.Collections.Generic;
using System.Text;
using top.riverelder.arkham.Code.Bot;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;

namespace top.riverelder.arkham.Code.Commands
{
    class Command_CreateInv : ICommand
    {
        public string Name => "车卡";

        public ArgumentValidater Validater { get; } = ArgumentValidater.Empty
                .SetListArgCountMin(1)
                .SetListArgCountMax(2)
                .SetRestDictArg(ArgumentValidater.Value);

        public string Usage => "车卡 <姓名> [描述]; {属性名:属性值}";
        

        public string Execute(string[] listArgs, IDictionary<string, string> dictArgs, string originalString, CmdEnv env) {
            if (!env.ScenarioExist) {
                return "还未开团";
            }

            string name = listArgs[0];
            string description = listArgs.Length > 1 ? listArgs[1] : "";

            Investigator inv = new Investigator(name, description);

            inv.Values.Fill(Global.DefaultValues);

            foreach (string key in dictArgs.Keys) {
                Value value = Value.Of(dictArgs[key]);
                inv.Values.Put(key, value);
            }
            inv.Calc(out string err);

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("名字：{0}", name).AppendLine();
            builder.AppendFormat("描述：{0}", description).AppendLine();

            IList<string> keys = new List<string>(inv.Values.Names);
            for (int i = 0; i < keys.Count; i++) {
                if (i % 3 == 0) {
                    builder.AppendLine();
                }
                Value v = inv.Values[keys[i]];
                builder.AppendFormat(" {0}:{1}; ", keys[i], v.Val);
            }
            env.Scenario.PutInvestigator(inv);
            env.Scenario.Control(env.User, inv.Name);
            SaveUtil.Save(env.Scenario);
            return builder.ToString();
        }
    }
}
