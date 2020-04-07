using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;

namespace top.riverelder.arkham.Code.Commands {
    public class Command_Coc7 : DiceCmdEntry {
        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.Register("coc7").Then(
                PresetNodes.Int<DMEnv>("数量").Executes((env, args, dict) => DrawProperties(args.GetInt("数量")))
            ).Executes((env, args, dict) => DrawProperties(5));

            dispatcher.SetAlias("COC7", "coc7");
            dispatcher.SetAlias("COC", "coc7");
            dispatcher.SetAlias("coc", "coc7");
        }

        public static string[] properties = new string[] { "力量", "体质", "体型", "教育", "智力", "意志", "外貌" };

        public static string DrawProperties(int size) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < size; i++) {
                if (i > 0) {
                    sb.AppendLine();
                }
                int rest = 0;
                for (int j = 0; j < properties.Length; j++) {
                    int val = Dice.Roll("3d6") * 5;
                    sb.Append(properties[j]).Append(':').Append(val).Append(' ');
                    rest += val;
                }
                int luck = Dice.Roll("3d6") * 5;
                sb.Append("幸运:").Append(luck).AppendLine();
                sb.Append($"带幸运：{rest + luck}，不带幸运：{rest}");
            }
            return sb.ToString();
        }
    }
}
