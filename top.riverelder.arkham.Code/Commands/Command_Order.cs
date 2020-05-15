using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;

namespace top.riverelder.arkham.Code.Commands {
    class Command_Order : DiceCmdEntry {

        public string Usage => "排序 <数值名> {调查员名}";

        public static string OrderByValue(Scenario scenario, string[] invNames, string valueName) {
            IList<string> notFoundNames = new List<string>();
            IList<string> notFoundValues = new List<string>();
            IDictionary<Investigator, int> map = new Dictionary<Investigator, int>();

            foreach (string name in invNames) {
                Match m = Regex.Match(name, @"[+-]\d+$");
                int fix = 0;
                string invName = name;
                if (m.Success) {
                    invName = name.Substring(0, name.Length - m.Value.Length);
                    fix = int.Parse(m.Value);
                }
                if (scenario.TryGetInvestigator(invName, out Investigator inv)) {
                    if (inv.Values.TryGet(valueName, out Value value)) {
                        map[inv] = value.Val + fix;
                    } else {
                        notFoundValues.Add(invName);
                    }
                } else {
                    notFoundNames.Add(invName);
                }
            }

            StringBuilder sb = new StringBuilder();

            if (map.Count > 0) {
                List<Investigator> list = new List<Investigator>(map.Keys);
                list.Sort((a, b) => map[b] - map[a]);
                for (int i = 0; i < list.Count; i++) {
                    if (i > 0) {
                        sb.Append(" > ");
                    }
                    Investigator inv = list[i];
                    sb.Append(inv.Name).Append('(').Append(inv.Is("HIDE_VALUE") ? "???" : Convert.ToString(map[inv])).Append(')');
                }
            }
            if (notFoundNames.Count > 0) {
                sb.AppendLine().Append("未找到调查员：").Append(string.Join("、", notFoundNames));
            }
            if (notFoundValues.Count > 0) {
                sb.AppendLine().Append($"未找到带{valueName}调查员：").Append(string.Join("、", notFoundValues));
            }
            return sb.ToString();
        }
        

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.Register("排序")
            .Then(
                PresetNodes.String<DMEnv>("数值名").Rest(
                    PresetNodes.String<DMEnv>("调查员名")
                    .Handles(Extensions.ConvertObjectArrayToStringArray())
                    .Executes((env, args, dict) => OrderByValue(env.Sce, args.Get<string[]>("调查员名"), args.GetStr("数值名")))
                )
            );

            dispatcher.SetAlias("战列", "排序 敏捷");
        }
    }
}
