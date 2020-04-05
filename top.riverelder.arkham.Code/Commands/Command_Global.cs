using System.Collections.Generic;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;

namespace top.riverelder.arkham.Code.Commands {
    class Command_Global : DiceCmdEntry {

        public string Usage => "全局 <配置|调试> <载入|保存|开启|关闭>";

        public override void OnRegister(CmdDispatcher<DiceMaidEnv> dispatcher) {
            IDictionary<object, object> oao = new Dictionary<object, object> {
                ["开"] = true,
                ["关"] = false,
            };
            dispatcher.Register("全局").Then(
                PresetNodes.Literal<DiceMaidEnv>("配置").Then(
                    PresetNodes.Literal<DiceMaidEnv>("载入")
                    .Executes((env, args, dict) => {
                        SaveUtil.LoadGlobal();
                        return "载入完毕";
                    })
                ).Then(
                    PresetNodes.Literal<DiceMaidEnv>("保存")
                    .Executes((env, args, dict) => {
                        SaveUtil.SaveGlobal();
                        return "保存完毕";
                    })
                )
            ).Then(
                PresetNodes.Literal<DiceMaidEnv>("调试").Then(
                    PresetNodes.Or<DiceMaidEnv>("开关", "开", "关")
                    .Handles(PreProcesses.Mapper<DiceMaidEnv>(oao))
                    .Executes((env, args, dict) => {
                        Global.Debug = args.GetBool("开关");
                        return "调试模式：" + (Global.Debug ? "开" : "关");
                    })
                )
            );
        }

        


    }
}
