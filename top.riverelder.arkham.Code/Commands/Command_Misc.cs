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
            .AppendLine(Clap + "    {1}    " + Clap)
            .AppendLine(Clap + "    {2}    " + Clap)
            .AppendLine(Repeat(Clap, 8))
            .Append("让我们再次把热烈的掌声送给他")
            .ToString();

        public static string ListLuck(Scenario sce) {
            Dictionary<string, int> lucks = new Dictionary<string, int>();

            long seed = DateTime.Today.Ticks;
            int p = (int)seed;
            string tlsn = GetTodayLuckySkillName(seed);
            foreach (Investigator inv in sce.Investigators.Values) {
                if (!inv.Is("HIDE_VALUE")) {
                    lucks[inv.Name] = CalcLuck(inv, p, tlsn);
                }
            }
            List<string> list = new List<string>(lucks.Keys);
            list.Sort((a, b) => lucks[b] - lucks[a]);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++) {
                if (i > 0) {
                    sb.AppendLine();
                }
                string indexStr = Convert.ToString(i + 1) + ".  ";
                switch (i + 1) {
                    case 1: indexStr = "🥇"; break;
                    case 2: indexStr = "🥈"; break;
                    case 3: indexStr = "🥉"; break;
                }
                sb.Append(indexStr).Append(list[i]).Append('(').Append(lucks[list[i]]).Append(')');
            }
            return sb.ToString();
        }

        public static string LuckyOneOfDay(Scenario sce) {
            Investigator luckOne = null;
            int luckMax = 0;

            long seed = DateTime.Today.Ticks;
            int p = (int)seed;
            string tlsn = GetTodayLuckySkillName(seed);
            foreach (Investigator inv in sce.Investigators.Values) {
                if (inv.Is("HIDE_VALUE")) {
                    continue;
                }
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
                    con += (int)(p * Math.Min(1.0f, value.Val / (value.Max > 0f ? value.Max : 100f)));
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
            return Math.Abs((int)((con ^ p) / (float)p * 10));
        }

        public static string Repeat(string s, int times) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < times; i++) {
                sb.Append(s);
            }
            return sb.ToString();
        }

        public static string SendClaps(Dice times) {
            if (times == null) {
                times = Dice.Of("1d5");
            }
            return Repeat(Clap, times.Roll());
        }

        private static int Seed = (int)DateTime.Now.Ticks;


        public static Dictionary<string, string> NameLetterSet = new Dictionary<string, string> {
            ["武侠"] = 
            "男默言肖枫子杰尹灵奇女幽妍梦屏蔚蓝樱朵姬夜旋璎珞颜离如烟清沙婉儿" +
            "慕莫奈南宫契权寰宇安佐旭郝连水渊仙人气质的白夕若冷萧逸皓月司徒雨" +
            "瞳奕世贵形依娜筱羽馨煜婷弯久皇旒漫妮千爱火怜影凤舞幻风夏洋景杨柯" +
            "寻越冉承苍神薰光上空瑞稀裴翼商易伊楚凌漪楼焰麟翔冰北堂赫烨航百里" +
            "淇自殷尧云沂耀卓常柏卫圣龙孤鹤沉瀚海岭零毓禹秦希宋吟阎岚霍菁方璇" +
            "燕勒绯臻莱玥拉班布岳寒霄泽弈硕锦衡归听雪段洛霆遥落盛关葵紫竹雅绿" +
            "小宝玄秋星经凯惠乐家谷正志蔺睿好匡胜东郭俊弼郗华鄂和昶滑英闵鸿蒋" +
            "勇喻元驹鞠宏伟禄资车良策马寇博容韩宜修启罗天阳佟游林向牟嘉熙浩邈" +
            "勾君扶弘业文辛学乌粱朗玉堵豪纵鱼达聪夔晖那成周加祺然魏包顾苏爽邱" +
            "发红淼泰初慎真雷永张麻焦翰杭炎彬从仲孙兴德戚语茂",
            ["外译中"] = 
            "布普德特格克弗夫兹斯什吉奇思赫姆尔伍伊茨巴芭帕达塔加卡瓦娃法扎萨" +
            "沙贾查撒哈马玛纳娜拉夸亚察阿艾拜派代戴泰盖凯赛蔡海迈奈莱赖怀厄伯" +
            "珀泽瑟舍哲彻默勒沃耶策埃贝佩维费塞谢杰切黑梅内雷韦惠奎比皮迪蒂基" +
            "菲齐西锡希米尼利莉里丽威乌杜图古库武富朱苏舒丘胡穆努卢鲁尤楚奥博" +
            "波多托戈科福佐索肖乔霍莫诺洛罗约鲍保道陶高考豪毛瑙劳姚允久休纽柳" +
            "留班潘丹坦甘坎范赞桑香詹钱汉曼南檑兰万扬因恩敦査本彭登根肯文芬增" +
            "森中琴亨门伦温昆延岑宾平丁廷金津辛欣钦明林英青昂邦庞唐汤冈康冯方" +
            "藏琼杭芒农朗旺匡仓翁蓬东栋通贡孔丰宗松雄洪蒙隆龙荣聪邓滕",
        };
        
        public static string MakeName(string dict, int len) {
            dict = dict ?? "武侠";
            if (len <= 0) {
                return "名字长度必须是正整数！";
            }
            if (!NameLetterSet.TryGetValue(dict, out string set)) {
                return "未找到字集：" + dict;
            }
            List<char> list = new List<char>();
            Random random = new Random(Seed++);
            for (int i = 0; i < len; i++) {
                list.Add(set[random.Next(set.Length)]);
            }
            return string.Join("", list);
        }

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.Register("杂项").Then(
                Literal<DMEnv>("今日幸运儿")
                .Executes((env, args, dict) => LuckyOneOfDay(env.Sce))
            ).Then(
                Literal<DMEnv>("幸运儿")
                .Executes((env, args, dict) => ListLuck(env.Sce))
            ).Then(
                Literal<DMEnv>("鼓掌")
                .Executes((env, args, dict) => SendClaps(null))
                .Then(
                    Extensions.Dice("次数").Executes((env, args, dict) => SendClaps(args.GetDice("次数")))
                )
            ).Then(
                Literal<DMEnv>("取名")
                .Executes((env, args, dict) => MakeName("武侠", 3))
                .Then(
                    Int<DMEnv>("长度")
                    .Executes((env, args, dict) => MakeName("武侠", args.GetInt("长度")))
                    .Then(
                        String<DMEnv>("字集")
                        .Executes((env, args, dict) => MakeName(args.GetStr("字集"), args.GetInt("长度")))
                    )
                ).Then(
                    Literal<DMEnv>("字集").Executes((env, args, dict) => string.Join("，", NameLetterSet.Keys))
                )
            ).Then(
                Literal<DMEnv>("说")
                .Then(
                    String<DMEnv>("内容")
                    .Executes((env, args, dict) => env.Inv.Name + "：" + args.GetStr("内容"))
                )
            );

            dispatcher.SetAlias("今日幸运儿", "杂项 今日幸运儿");
            dispatcher.SetAlias("幸运儿", "杂项 幸运儿");
            dispatcher.SetAlias("鼓掌", "杂项 鼓掌");
            dispatcher.SetAlias("取名", "杂项 取名");
            dispatcher.SetAlias("说", "杂项 说");

            dispatcher.SetAlias("sy", "杂项 说");
        }
    }
}
