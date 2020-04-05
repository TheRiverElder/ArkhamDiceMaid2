//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;

//using top.riverelder.arkham.Code.Model;

//namespace top.riverelder.arkham.Code.Commands {
//    class Command_Order : DiceCmdEntry {

//        public string Name => "排序";

//        public string Usage => "排序 <数值名> {调查员名}";

//        public ArgumentValidater Validater => ArgumentValidater.Empty
//            .SetListArgCountMin(3)
//            .SetListArgCountMax(ArgumentValidater.Unlimited);

//        public static string OrderByValue(Scenario scenario, string[] invNames, string valueName) {
//            IList<string> notFoundNames = new List<string>();
//            IList<string> notFoundValues = new List<string>();
//            IDictionary<string, int> map = new Dictionary<string, int>();

//            foreach (string name in invNames) {
//                Match m = Regex.Match(name, @"[+-]\d+$");
//                int fix = 0;
//                string invName = name;
//                if (m.Success) {
//                    invName = name.Substring(0, name.Length - m.Value.Length);
//                    fix = int.Parse(m.Value);
//                }
//                if (scenario.TryGetInvestigator(invName, out Investigator inv)) {
//                    if (inv.Values.TryGet(valueName, out Value value)) {
//                        map[invName] = value.Val + fix;
//                    } else {
//                        notFoundValues.Add(invName);
//                    }
//                } else {
//                    notFoundNames.Add(invName);
//                }
//            }

//            StringBuilder sb = new StringBuilder();

//            if (map.Count > 0) {
//                List<string> list = new List<string>(map.Keys);
//                list.Sort((a, b) => map[b] - map[a]);
                
//                sb.Append(string.Join(" > ", list.ToArray()));
//            }
//            if (notFoundNames.Count > 0) {
//                sb.AppendLine().Append("未找到调查员：").Append(string.Join("、", notFoundNames));
//            }
//            if (notFoundValues.Count > 0) {
//                sb.AppendLine().Append($"未找到带{valueName}调查员：").Append(string.Join("、", notFoundValues));
//            }
//            return sb.ToString();
//        }

//        public string Execute(string[] listArgs, IDictionary<string, string> dictArgs, string originalString, CmdEnv env) {
//            if (!env.ScenarioExist) {
//                return "该羣还未开团";
//            }

//            string valueName = listArgs[0];
//            string[] invNames = new string[listArgs.Length - 1];
//            Array.Copy(listArgs, 1, invNames, 0, invNames.Length);
//            return OrderByValue(env.Scenario, invNames, valueName);
//        }
//    }
//}
