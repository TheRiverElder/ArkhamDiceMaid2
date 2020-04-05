using System.Collections.Generic;

using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;

namespace top.riverelder.arkham.Code.Commands {
    class Command_Control : DiceCmdEntry {

        public string Usage => "控制 <卡名>";

        public static string Control(long selfId, Scenario sce, Investigator inv) {
            sce.Control(selfId, inv.Name);
            SaveUtil.Save(sce);
            return $"早啊，{inv.Name}！";
        }

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.Register("控制").Then(
                PresetNodes.String<DMEnv>("卡名")
                .Handles(Extensions.ExistInv())
                .Executes((env, args, dict) => Control(env.SelfId, env.Sce, args.GetInv("卡名")))
            );
        }
    }
}
