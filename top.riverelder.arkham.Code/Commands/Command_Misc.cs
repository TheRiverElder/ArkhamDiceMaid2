using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;
using static top.riverelder.RiverCommand.PresetNodes;

namespace top.riverelder.arkham.Code.Commands {
    public class Command_Misc : DiceCmdEntry {

        public static string LuckyOneOfDay(Scenario sce) {
            Investigator luckOne = null;
            int luckMax = 0;

            long seed = DateTime.Today.Ticks;
            string tlsn = GetTodayLuckySkillName(seed);
            foreach (Investigator inv in sce.investigators.Values) {
                int luck = CalcLuck(inv, seed, tlsn);
                if (luckOne == null || luck > luckMax) {
                    luckOne = inv;
                    luckMax = luck;
                }
            }
            if (luckOne == null) {
                return "今天没有幸运儿";
            } else {
                return "天的幸运儿是：\n" + luckOne.Name + "，" + luckOne.Desc;
            }
        }

        public static string[] Params = new string[] { "幸运", "敏捷", "教育", "体型", "体力", "智力", "外貌" };

        public static string GetTodayLuckySkillName(long seed) {
            var c = new List<string>(Global.DefaultValues.Names);
            return c[(int)(seed % c.Count)];
        }

        public static int CalcLuck(Investigator inv, long seed, string tlsn) {
            int con = 0;
            foreach (string key in Params) {
                if (inv.Values.TryGet(key, out Value value)) {
                    con ^= value.Val;
                }
            }
            if (!string.IsNullOrEmpty(tlsn) && inv.Values.TryGet(tlsn, out Value lv)) {
                con ^= lv.Val;
            }
            return Math.Abs((int)(seed % con));
        }

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.Register("杂项").Then(
                Literal<DMEnv>("今日幸运儿")
                .Handles(Extensions.ExistSce())
                .Executes((env, args ,dict) => LuckyOneOfDay(env.Sce))
            );

            dispatcher.SetAlias("今日幸运儿", "杂项 今日幸运儿");
        }
    }
}
