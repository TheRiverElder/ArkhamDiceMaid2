using RiverCommand;
using System.Collections.Generic;

using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;
using top.riverelder.RiverCommand.Parsing;

namespace top.riverelder.arkham.Code.Commands {
    class Command_Control : DiceCmdEntry {

        public string Usage => "控制 <卡名>";

        public static string Control(DMEnv env, long selfId, Scenario sce, Investigator inv) {
            sce.Control(selfId, inv.Name);
            SaveUtil.Save(sce);
            return env.Next = $"早啊，{inv.Name}！";
        }

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.Register("控制")
            .Then(
                PresetNodes.Literal<DMEnv>("关联")
                .Executes((env, args, dict) => Relate(env))
            )
            .Then(
                PresetNodes.String<DMEnv>("卡名")
                .Handles(Extensions.ExistInv)
                .Executes((env, args, dict) => Control(env, env.SelfId, env.Sce, args.GetInv("卡名")))
                .Then(
                    PresetNodes.Cmd<DMEnv>("行动").Executes((env, args, dict) => ControlAndAct(env, args.GetInv("卡名"), args.GetCmd("行动")))
                )
            );

            dispatcher.SetAlias("ct", "控制");
            dispatcher.SetAlias("关联到此", "控制 关联");
        }

        public static object ControlAndAct(DMEnv env, Investigator inv, CommandResult<DMEnv> action) {
            Scenario sce = env.Sce;
            if (!sce.PlayerNames.TryGetValue(env.SelfId, out string selfName)) {
                selfName = null;
            }
            sce.Control(env.SelfId, inv.Name);
            env.ClearCache();
            action.Env = env;
            object res = action.Execute();
            if (selfName != null) {
                sce.Control(env.SelfId, selfName);
            }
            env.Save();
            return res;
        }

        public static bool Relate(DMEnv env) {
            if (env.GroupId <= 10000) {
                env.Next = "泥确定是在群里执行这条指令的？";
                return false;
            }
            Global.Users[env.SelfId] = env.GroupId;
            SaveUtil.SaveGlobal();
            env.Next = "已将泥关联到该羣！现在可以在小窗操作我了哦~";
            return true;
        }
    }
}
