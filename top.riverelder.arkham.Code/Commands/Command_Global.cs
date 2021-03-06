﻿using System.Collections.Generic;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;
using top.riverelder.RiverCommand.Parsing;

namespace top.riverelder.arkham.Code.Commands {

    /// <summary>
    /// 进行全局选项的操作，包括
    /// 配置的保存于载入
    /// 调试模式的开关
    /// </summary>
    class Command_Global : DiceCmdEntry {

        public string Usage => "全局 <配置|调试> <载入|保存|开启|关闭>";

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.Register("全局").Then(
                PresetNodes.Literal<DMEnv>("配置").Then(
                    PresetNodes.Literal<DMEnv>("载入")
                    .Executes((env, args, dict) => ("载入" + (SaveUtil.LoadGlobal() ? "成功" : "失败")))
                ).Then(
                    PresetNodes.Literal<DMEnv>("保存")
                    .Executes((env, args, dict) => ("保存" + (SaveUtil.SaveGlobal() ? "成功" : "失败")))
                )
            ).Then(
                PresetNodes.Literal<DMEnv>("调试").Then(
                    PresetNodes.Bool<DMEnv>("开关", "开", "关")
                    .Executes((env, args, dict) => {
                        Global.Debug = args.GetBool("开关");
                        env.Next = "调试模式：" + (Global.Debug ? "开" : "关");
                    })
                )
            ).Then(
                PresetNodes.Literal<DMEnv>("回复").Then(
                    PresetNodes.Bool<DMEnv>("开关", "开", "关")
                    .Executes((env, args, dict) => {
                        Global.DoAt = args.GetBool("开关");
                        SaveUtil.SaveGlobal();
                        env.Next = "回复：" + (Global.DoAt ? "开" : "关");
                    })
                )
            ).Then(
                PresetNodes.Literal<DMEnv>("自动载入").Then(
                    PresetNodes.Bool<DMEnv>("开关", "开", "关")
                    .Executes((env, args, dict) => {
                        Global.AutoLoad = args.GetBool("开关");
                        SaveUtil.SaveGlobal();
                        env.Next = "自动载入：" + (Global.AutoLoad ? "开" : "关");
                    })
                )
            ).Then(
                PresetNodes.Literal<DMEnv>("翻译腔").Then(
                    PresetNodes.Bool<DMEnv>("开关", "开", "关")
                    .Executes((env, args, dict) => {
                        Global.TranslatorTone = args.GetBool("开关");
                        //SaveUtil.SaveGlobal();
                        env.Next = "翻译腔：" + (Global.TranslatorTone ? "开" : "关");
                    })
                )
            ).Then(
                PresetNodes.Literal<DMEnv>("睡眠").Then(
                    PresetNodes.Bool<DMEnv>("开关", "开", "关")
                    .Executes((env, args, dict) => {
                        Global.Sleep = args.GetBool("开关");
                        SaveUtil.SaveGlobal();
                        env.Next = (Global.Sleep ? "那我去睡觉觉咯~w" : "爷来啦！");
                    })
                )
            ).Then(
                PresetNodes.Literal<DMEnv>("灌铅")
                .Then(
                    PresetNodes.Int<DMEnv>("数值")
                    .Executes((env, args, dict) => {
                        env.Next = !Global.AllowedLead ? "禁止给老娘灌铅！喵~o( =▼ω▼= )m！" : "当前铅量：" + (Global.SetLead(args.GetInt("数值")));
                        SaveUtil.SaveGlobal();
                    })
                ).Then(
                    PresetNodes.Literal<DMEnv>("重置")
                    .Executes((env, args, dict) => {
                        Global.SetLead(50);
                        env.Next = "铅量已重置为" + Global.Lead;
                        SaveUtil.SaveGlobal();
                    })
                )
            );

            dispatcher.SetAlias("配置", "全局 配置");
            dispatcher.SetAlias("调试", "全局 调试");
            dispatcher.SetAlias("回复", "全局 回复");
            dispatcher.SetAlias("自动载入", "全局 自动载入");
            dispatcher.SetAlias("闭嘴", "全局 睡眠 开");
            dispatcher.SetAlias("说话", "全局 睡眠 关");
            dispatcher.SetAlias("翻译腔", "全局 翻译腔");
            dispatcher.SetAlias("灌铅", "全局 灌铅");
        }

    }
}
