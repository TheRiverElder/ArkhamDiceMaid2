//using System.Collections.Generic;

//using top.riverelder.arkham.Code.Utils;

//namespace top.riverelder.arkham.Code.Commands
//{
//    class Command_SetPrefix : DiceCmdEntry {
//        public string Name => "我信仰";

//        public ArgumentValidater Validater { get; } = ArgumentValidater.Empty
//            .SetListArgCount(1);

//        public string Usage => "我信仰 <新前缀>";

//        public string Execute(string[] listArgs, IDictionary<string, string> dictArgs, string originalString, CmdEnv env)
//        {
//            if ("=".Equals(listArgs[0]))
//            {
//                return "前缀不能是'='";
//            }
//            Global.Prefix = listArgs[0];
//            SaveUtil.SaveGlobal();
//            return "因尔之信仰，\n尔等凡人欲召神之奴仆，\n需唤神之名讳：\n" + Global.Prefix;
//        }
//    }
//}
