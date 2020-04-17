using Native.Sdk.Cqp;
using Native.Sdk.Cqp.Enum;
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

        public static string Clap = CQApi.CQCode_Face(CQFace.鼓掌).ToSendString();
        public static string Face = CQApi.CQCode_Face(CQFace.发呆).ToSendString();
        public static string Templete = new StringBuilder()
            .AppendLine("让我们用热烈掌声欢迎今天的幸运儿！")
            .AppendLine("TA凭借着{0}的幸运值，成为了今日之🐖！他就是")
            .AppendLine(Repeat(Clap, 8))
            .AppendLine("|        " + Face + "        |")
            .AppendLine("|        🥇        |")
            .AppendLine(Clap + "{1}" + Clap)
            .AppendLine(Clap + "{2}" + Clap)
            .AppendLine(Repeat(Clap, 8))
            .Append("让我们再次把热烈的掌声送给他")
            .ToString();

        public static string LuckyOneOfDay(Scenario sce) {
            Investigator luckOne = null;
            int luckMax = 0;

            long seed = DateTime.Today.Ticks;
            int p = (int)seed;
            string tlsn = GetTodayLuckySkillName(seed);
            foreach (Investigator inv in sce.Investigators.Values) {
                int luck = CalcLuck(inv, p, tlsn);
                if (luckOne == null || luck > luckMax) {
                    luckOne = inv;
                    luckMax = luck;
                }
            }
            if (luckOne == null) {
                return "今天没有幸运儿";
            } else {
                return string.Format(Templete, luckMax, luckOne.Name, luckOne.Desc);
            }
        }

        public static string[] Params = new string[] { "幸运", "敏捷", "教育", "体型", "体力", "智力", "外貌" };

        public static string GetTodayLuckySkillName(long seed) {
            var c = new List<string>(Global.DefaultValues.Names);
            return c.Count > 0 ? c[(int)(seed % c.Count)] : null;
        }

        public static int CalcLuck(Investigator inv, int p, string tlsn) {
            int con = 0;
            foreach (string key in Params) {
                if (inv.Values.TryGet(key, out Value value)) {
                    con += (int)(p * Math.Min(1.0f, value.Val / (float)(value.Max > 0 ? value.Max : 100)));
                    if (con % 2 == 1) {
                        con ^= value.Val;
                    } else {
                        con = con * 3 + 1;
                    }
                }
            }
            if (!string.IsNullOrEmpty(tlsn) && inv.Values.TryGet(tlsn, out Value lv)) {
                con ^= lv.Val;
            }
            return (Math.Abs(con ^ p) / 100);
        }

        public static string Repeat(string s, int times) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < times; i++) {
                sb.Append(s);
            }
            return sb.ToString();
        }

        private static int Seed = (int)DateTime.Now.Ticks;
        

        public static string SendClaps(Dice times) {
            if (times == null) {
                times = Dice.Of("1d5");
            }
            return Repeat(Clap, times.Roll());
        }

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.Register("杂项").Then(
                Literal<DMEnv>("今日幸运儿")
                .Handles(Extensions.ExistSce())
                .Executes((env, args ,dict) => LuckyOneOfDay(env.Sce))
            ).Then(
                Literal<DMEnv>("鼓掌")
                .Executes((env, args, dict) => SendClaps(null))
                .Then(
                    Extensions.Dice("次数").Executes((env, args, dict) => SendClaps(args.GetDice("次数")))
                )
            );

            dispatcher.SetAlias("今日幸运儿", "杂项 今日幸运儿");
            dispatcher.SetAlias("鼓掌", "杂项 鼓掌");
        }
    }
}
