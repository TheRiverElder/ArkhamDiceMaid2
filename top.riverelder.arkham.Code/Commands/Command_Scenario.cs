using System;
using System.Collections.Generic;

using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;
using top.riverelder.RiverCommand.Parsing;

namespace top.riverelder.arkham.Code.Commands {
    public class Command_Scenario : DiceCmdEntry {

        public string Usage => "读团 [团名]";

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.Register("存档").Then(
                PresetNodes.Literal<DMEnv>("保存")
                
                .Executes((env, args, dict) => SaveScenario(env, env.Sce))
                .Then(
                    PresetNodes.String<DMEnv>("团名")
                    .Executes((env, args, dict) => SaveScenario(env, args.GetStr("团名")))
                )
            ).Then(
                PresetNodes.Literal<DMEnv>("读取")
                .Executes((env, args, dict) => LoadScenario(env, env.GroupId))
                .Then(
                    PresetNodes.String<DMEnv>("团名")
                    .Executes((env, args, dict) => LoadScenario(env, env.GroupId, args.GetStr("团名")))
                )
            ).Then(
                PresetNodes.Literal<DMEnv>("开始")
                .Then(
                    PresetNodes.String<DMEnv>("团名")
                    .Executes((env, args, dict) => StartScenario(env, env.GroupId, env.SelfId, args.GetStr("团名")))
                    //.Then(
                    //    PresetNodes.Literal<DMEnv>("强制")
                    //    .Executes((env, args, dict) => StartScenario(env.GroupId, args.GetStr("团名")))
                    //)
                )
            );

            dispatcher.SetAlias("读团", "存档 读取");
            dispatcher.SetAlias("存团", "存档 保存");
            dispatcher.SetAlias("开团", "存档 开始");
        }

        public static bool LoadScenario(DMEnv env, long groupId) {
            if (!Global.Groups.TryGetValue(groupId, out string sceName)) {
                env.Append("该群还未有存档存在");
                return false;
            }
            return LoadScenario(env, groupId, sceName);
        }

        public static bool LoadScenario(DMEnv env, long groupId, string sceName) {
            try {
                if (SaveUtil.TryLoad(sceName, out Scenario scenario)) {
                    Global.Scenarios[sceName] = scenario;
                    Global.Groups[groupId] = scenario.Name;
                    env.Append($"读团完毕：{sceName}");
                    return true;
                }
            } catch (Exception e) {
                env.Append("读团失败，原因：" + e.Message);
                return false;
            }
            env.Append($"读团失败：{sceName}");
            return false;
        }

        public static bool SaveScenario(DMEnv env, Scenario sce) {
            SaveUtil.Save(sce);
            env.Append("保存完毕：" + sce.Name);
            return false;
        }

        public static bool SaveScenario(DMEnv env, string sceName) {
            if (Global.Scenarios.TryGetValue(sceName, out Scenario scenario)) {
                SaveUtil.Save(scenario);
                env.Append("保存完毕：" + sceName);
                return true;
            }
            env.Append($"未找到团：{sceName}");
            return false;
        }

        public static bool StartScenario(DMEnv env, long groupId, long selfId, string sceName) {
            Scenario s = new Scenario(sceName);
            Global.Scenarios[sceName] = s;
            Global.Groups[groupId] = s.Name;
            SaveUtil.Save(s);
            env.Append($"【{sceName}】开团啦！");
            return true;
        }
    }
}
