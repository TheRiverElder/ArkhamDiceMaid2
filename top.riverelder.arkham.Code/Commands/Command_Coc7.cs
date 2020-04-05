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
        public override void OnRegister(CmdDispatcher<DiceMaidEnv> dispatcher) {
            dispatcher.Register("coc7").Then(
                PresetNodes.Int<DiceMaidEnv>("数量").Executes((env, args, dict) => DrawProperties(args.GetInt("数量")))
            ).Executes((env, args, dict) => DrawProperties(5));
        }

        public static string[] properties = new string[] { "力量", "体质", "体型", "教育", "智力", "意志", "外貌" };

        public static string DrawProperties(int size) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < size; i++) {
                if (i > 0) {
                    sb.AppendLine();
                }
                for (int j = 0; j < properties.Length; j++) {
                    if (j > 0) {
                        sb.Append(' ');
                    }
                    sb.Append(properties[j]).Append('：').Append(Dice.Roll("3d6") * 5);
                }
            }
            return sb.ToString();
        }
    }
}
