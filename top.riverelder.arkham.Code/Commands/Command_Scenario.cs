using System;
using System.Collections.Generic;

using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;

namespace top.riverelder.arkham.Code.Commands {
    public class Command_Scenario : DiceCmdEntry {

        public string Usage => "读团 [团名]";

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.Register("团子").Then(
                PresetNodes.Literal<DMEnv>("保存")
                .Handles(Extensions.ExistSce())
                .Executes((env, args, dict) => SaveScenario(env.Sce))
                .Then(
                    PresetNodes.String<DMEnv>("团名")
                    .Executes((env, args, dict) => SaveScenario(args.GetStr("团名")))
                )
            ).Then(
                PresetNodes.Literal<DMEnv>("读取")
                .Handles(Extensions.ExistSce())
                .Executes((env, args, dict) => LoadScenario(env.GroupId))
                .Then(
                    PresetNodes.String<DMEnv>("团名")
                    .Executes((env, args, dict) => LoadScenario(env.GroupId, args.GetStr("团名")))
                )
            );
        }

        public static string LoadScenario(long groupId) {
            if (!Global.Groups.TryGetValue(groupId, out string sceName)) {
                return "该群还未有团存在";
            }
            return LoadScenario(groupId, sceName);
        }

        public static string LoadScenario(long groupId, string sceName) {
            try {
                if (SaveUtil.TryLoad(sceName, out Scenario scenario)) {
                    Global.Scenarios[sceName] = scenario;
                    Global.Groups[groupId] = scenario.Name;
                    return $"读团完毕：{sceName}";
                }
            } catch (Exception e) {
                return "读团失败，原因：" + e.Message;
            }
            return $"读团失败：{sceName}";
        }

        public static string SaveScenario(Scenario sce) {
            SaveUtil.Save(sce);
            return "保存完毕：" + sce.Name;
        }

        public static string SaveScenario(string sceName) {
            if (Global.Scenarios.TryGetValue(sceName, out Scenario scenario)) {
                SaveUtil.Save(scenario);
                return "保存完毕：" + sceName;
            }
            return $"未找到团：{sceName}";
        }
    }
}
