using System;
using System.Collections.Generic;
using System.Text;
using top.riverelder.arkham.Code.Bot;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;

namespace top.riverelder.arkham.Code.Commands
{
    public class Command_SanCheck : ICommand
    {
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

        public string Name => "SC";

        public ArgumentValidater Validater { get; } = ArgumentValidater.Empty
                .SetListArgCount(2)
                .AddListArg(ArgumentValidater.Dice)
                .AddListArg(ArgumentValidater.Dice);

        public string Usage => "SC <成功骰子或数字> <失败骰子或数字>";

        public string Execute(string[] listArgs, IDictionary<string, string> dictArgs, string raw, CmdEnv env)
        {
            if (!EnvValidator.ExistInv(env, out Investigator inv, out string err)) {
                return err;
            }
            
            // 检查是否存在属性
            if (!inv.Values.TryGet("理智", out Value value))
            {
                return "未找到属性：理智";
            }
            CheckResult result = CheckUtil.Check(value.Val);

            bool isDice = false;
            string s = result.Succeed ? listArgs[0] : listArgs[1];
            if (!int.TryParse(s, out int v))
            {
                isDice = true;
                v = Dice.Roll(s);
            }
            value.Sub(v);

            StringBuilder builder = new StringBuilder();
            string typeStr = result.Succeed ? "成功" : "失败";
            builder.AppendLine($"{inv.Name}的San={value.Val}，检定结果：");
            builder.AppendLine($"{result.dice} = {result.result}, 判定为 {typeStr}");
            if (v > 0)
            {
                builder.AppendLine(Desc[new Random().Next(Desc.Length)]);
            }
            string deltaExp = (isDice ? s + " = " : "") + v;
            builder.Append($"{inv.Name} San -{deltaExp}");
            SaveUtil.Save(env.Scenario);

            return builder.ToString();
        }
    }
}
