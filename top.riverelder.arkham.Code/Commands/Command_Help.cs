using System.Collections.Generic;
using System.Text;

using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;

namespace top.riverelder.arkham.Code.Commands {
    class Command_Help : DiceCmdEntry {
        
        public string Usage => "帮助 [命令名称]";

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.Register("帮助")
            .Executes((env, args, dict) => PrintAllHelp())
            .Then(
                PresetNodes.Literal<DMEnv>("模板")
                .Executes((env, args, dict) => "可用的模板：\n" + string.Join("、", Temepletes.Keys))
                .Then(
                    PresetNodes.String<DMEnv>("模板名")
                    .Executes((env, args, dict) => Temepletes.TryGetValue(args.GetStr("模板名"), out string t) ? t : $"未找到{args.GetStr("模板名")}的模板")
                )
            ).Then(
                PresetNodes.String<DMEnv>("指令名")
                .Executes((env, args, dict) => PrintHelp(args.GetStr("指令名")))
            );

            dispatcher.SetAlias("模板", "帮助 模板");
        }

        public static string PrintAllHelp() {
            StringBuilder builder = new StringBuilder().AppendLine("可用的命令集：");
            foreach (CommandNode<DMEnv> node in Global.Dispatcher.Commands) {
                builder.AppendLine("----------------");
                builder.AppendLine(string.Join("\n", node.GetHelp()));
            }
            return builder.ToString();
        }

        public static string PrintHelp(string head) {
            if (Global.Dispatcher.CommandMap.TryGetValue(head, out CommandNode<DMEnv> node)) {
                return string.Join("\n", node.GetHelp());
            }
            return $"未找到指令：{head}";
        }

        public static Dictionary<string, string> Temepletes = new Dictionary<string, string> {
            ["人物卡"] = new StringBuilder()
            .Append("/车卡 艾利克斯 只剩半条命的反抗军成员")
            .AppendLine().Append("力量：50")
            .AppendLine().Append("体质：50")
            .AppendLine().Append("体型：50")
            .AppendLine().Append("敏捷：50")
            .AppendLine().Append("外貌：50")
            .AppendLine().Append("智力：50")
            .AppendLine().Append("意志：50")
            .AppendLine().Append("教育：50")
            .AppendLine().Append("体力：10/10")
            .AppendLine().Append("理智：50/99")
            .AppendLine().Append("幸运：50/99")
            .AppendLine().Append("魔法：10/10")
            .ToString(),

            ["物品"] = new StringBuilder()
            .Append("/物品 创造 棒球棒")
            .AppendLine().Append("技能名：斗殴")
            .AppendLine().Append("类型：肉搏")
            .AppendLine().Append("伤害：1d8+db")
            .AppendLine().Append("穿刺：否")
            .AppendLine().Append("连发数：1")
            .AppendLine().Append("弹匣：1")
            .AppendLine().Append("故障值：100")
            .AppendLine().Append("弹药：1")
            .AppendLine().Append("消耗：0")
            .ToString(),
        };
    }
}
