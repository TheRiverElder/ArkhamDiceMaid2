using System;
using System.Collections.Generic;
using System.Text;

using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;
using top.riverelder.RiverCommand.Parsing;

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
            .Then(
                Extensions.Dice("成功骰子").Then(
                    Extensions.Dice("失败骰子")
                    .Executes((env, args, dict) => SanCheck(env, env.Sce, env.Inv, args.GetDice("成功骰子"), args.GetDice("失败骰子")))
                )
            );
            dispatcher.SetAlias("sc", "SC");
        }

        public static bool SanCheck(DMEnv env, Scenario sce, Investigator inv, Dice success, Dice failure) {
            if (!inv.Values.TryWidelyGet("理智", out Value value)) {
                env.Append(inv.Name + "没有理智！");
                return false;
            }
            if (!inv.Check("理智", out CheckResult result, out string str)) {
                env.Append(str);
                return false;
            }
            env.Append(str);

            int v = result.succeed ? success.Roll() : failure.Roll();
            if (v != 0) {
                env.LineAppend(Desc[new Random().Next(Desc.Length)]);
                env.LineAppend(inv.Change("理智", -v));
                if (v >= 5) {
                    env.LineAppend(Command_Status.DrawMadness(inv));
                }
            }
            SaveUtil.Save(sce);

            return result.succeed;
        }
    }
}
