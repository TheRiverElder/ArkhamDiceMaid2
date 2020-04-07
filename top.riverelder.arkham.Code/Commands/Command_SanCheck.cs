using System;
using System.Collections.Generic;
using System.Text;

using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;

namespace top.riverelder.arkham.Code.Commands {
    public class Command_SanCheck : DiceCmdEntry {
        public static string[] Desc = new string[] {
            "一股寒意袭来，脑中出现了无数不知如何描述的几何图形",
            "宛如刚从深海浮上水面的人，感到一阵恍惚",
            "脊背一凉",
            "由于一种未知的原因，仿佛一种黑暗从腹中冲上胸腔，直顶大脑",
            "眩晕夹杂着些许耳鸣，踉跄了一下",
            "一阵天旋地转，回过神来才发现自己差点倒地",
            "眼前的事物变得扭曲而且怪异，无法分辨色彩与形状",
            "不可名状的未知名讳犹如火车长笛般驶入大脑",
            "是谁在耳边低语？是同伴还是恶魔的诱唆？这都已经无从分辨",
        };

        public string Usage => "SC <成功骰子> <失败骰子>";

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.Register("SC")
            .Handles(Extensions.ExistSelfInv())
            .Then(
                Extensions.Dice("成功骰子").Then(
                    Extensions.Dice("失败骰子")
                    .Executes((env, args, dict) => SanCheck(env.Sce, env.Inv, args.GetDice("成功骰子"), args.GetDice("失败骰子")))
                ).Then(
                    PresetNodes.Literal<DMEnv>("/").Then(
                        Extensions.Dice("失败骰子")
                        .Executes((env, args, dict) => SanCheck(env.Sce, env.Inv, args.GetDice("成功骰子"), args.GetDice("失败骰子")))
                    )
                )
            );
            dispatcher.SetAlias("sc", "SC");
        }

        public static string SanCheck(Scenario sce, Investigator inv, Dice success, Dice failure) {
            if (!inv.Values.TryWidelyGet("理智", out Value value)) {
                return inv.Name + "没有理智！";
            }

            StringBuilder builder = new StringBuilder();
            CheckResult result = value.Check();
            string typeStr = result.succeed ? "成功" : "失败";
            builder.Append($"{inv.Name}的SC({value.Val}) => {result.result}：{typeStr}");

            int v = result.succeed ? success.Roll() : failure.Roll();
            if (v > 0) {
                builder.AppendLine(Desc[new Random().Next(Desc.Length)]);
            }
            int prev = value.Val;
            value.Sub(v);
            builder.Append($"{inv.Name}的理智：{prev} - {v} => {value.Val}");
            SaveUtil.Save(sce);

            return builder.ToString();
        }
    }
}
