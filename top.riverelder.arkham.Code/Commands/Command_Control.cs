//using System.Collections.Generic;

//using top.riverelder.arkham.Code.Model;
//using top.riverelder.arkham.Code.Utils;

//namespace top.riverelder.arkham.Code.Commands {
//    class Command_Control : DiceCmdEntry {
//        public string Name => "控制";

//        public string Usage => "控制 <卡名>";

//        public string Execute(string[] listArgs, IDictionary<string, string> dictArgs, string originalString, CmdEnv env) {
//            string name = listArgs[0];

//            if (!env.ScenarioExist) {
//                return "还未开团";
//            }
//            if (!env.Scenario.ExistInvestigator(name)) {
//                return $"不存在调查员：{name}";
//            }

//            env.Scenario.Control(env.User, name);
//            SaveUtil.Save(env.Scenario);
//            return $"现在你是：{name}";
//        }
//    }
//}
