using System.Collections.Generic;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;

namespace top.riverelder.arkham.Code.Commands {

    /// <summary>
    /// 进行全局选项的操作，包括
    /// 配置的保存于载入
    /// 调试模式的开关
    /// </summary>
    class Command_Global : DiceCmdEntry {

        public string Usage => "全局 <配置|调试> <载入|保存|开启|关闭>";

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            IDictionary<object, object> oao = new Dictionary<object, object> {
                ["开"] = true,
                ["关"] = false,
            };
            dispatcher.Register("全局").Then(
                PresetNodes.Literal<DMEnv>("配置").Then(
                    PresetNodes.Literal<DMEnv>("载入")
                    .Executes((env, args, dict) => {
                        SaveUtil.LoadGlobal();
                        return "载入完毕";
                    })
                ).Then(
                    PresetNodes.Literal<DMEnv>("保存")
                    .Executes((env, args, dict) => {
                        SaveUtil.SaveGlobal();
                        return "保存完毕";
                    })
                )
            ).Then(
                PresetNodes.Literal<DMEnv>("调试").Then(
                    PresetNodes.Or<DMEnv>("开关", "开", "关")
                    .Handles(PreProcesses.Mapper<DMEnv>(oao))
                    .Executes((env, args, dict) => {
                        Global.Debug = args.GetBool("开关");
                        return "调试模式：" + (Global.Debug ? "开" : "关");
                    })
                )
            );

            dispatcher.SetAlias("配置", "全局 配置");
            dispatcher.SetAlias("调试", "全局 调试");
        }

    }
}
