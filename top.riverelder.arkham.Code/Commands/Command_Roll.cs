using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;
using top.riverelder.RiverCommand.Parsing;

namespace top.riverelder.arkham.Code.Commands {

    /// <summary>
    /// 简单地投掷一个骰子，骰子需要符合骰子表达式
    /// </summary>
    public class Command_Roll : DiceCmdEntry {

        public static CommandNode<DMEnv> MainAction;

        public string Usage => "投掷 <骰子>";

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.Register("投掷")
            .Executes((env, args, dict) => Roll(env, Dice.Of("1d100")))
            .Then(
                MainAction = Extensions.Dice("骰子").Executes((env, args, dict) => Roll(env, args.GetDice("骰子")))
            ).Rest(
                PresetNodes.String<DMEnv>("选项组")
                .Handles(PreProcesses.ConvertObjectArrayToStringArray)
                .Executes((env, args, dict) => Choose(env, args.Get<string[]>("选项组")))
            );

            dispatcher.SetAlias("r", "投掷");
        }

        public static int Roll(DMEnv env, Dice dice) {
            string invName = "你";
            int result;
            if (env.TryGetInv(out Scenario sce, out Investigator inv)) {
                result = dice.RollWith(inv.DamageBonus);
                invName = inv.Name;
            } else {
                result = dice.Roll();
            }
            if (!Global.TranslatorTone) {
                env.Append($"{dice.ToString()} => {result}");
                return result;
            }
            string diceStr = dice.ToString();
            env.Append(WithTranslatorTone(diceStr, Convert.ToString(result), invName));
            return result;
        }

        public static string WithTranslatorTone(string dice, string result, string name) {
            StringBuilder pb = new StringBuilder().Append(Dice.Roll(Starts));
            int rand = Dice.Roll(2);
            for (int i = 0; i < rand; i++) {
                pb.Append(OddsAndEnds());
            }
            pb.Append(Dice.Roll(Rolls));
            rand = Dice.Roll(2);
            for (int i = 0; i < rand; i++) {
                pb.Append(OddsAndEnds());
            }
            pb.Append(Dice.Roll(Results));
            rand = Dice.Roll(2);
            for (int i = 0; i < rand; i++) {
                pb.Append(OddsAndEnds());
            }
            return string.Format(pb.ToString(), dice, result, name);
        }

        private static string[] Starts = new string[] {
            "哦，我的上帝啊，",
            "猜猜看，老伙计，",
            "有什么问题吗，我的朋友，",
            "{2}别闹，",
            "嘿，{2}，你听说了吗？",
            "瞧瞧我发现了什么？大家快来看呐！还有你{2}，",
            "也许，我是说也许，其实一个医生也没什么可笑的，连续五次救人失败的话，对吧{2}？",
            "",
        };
        private static string[] Results = new string[] {
            "但还是得到了一个{1}。",
            "可是它却给了我们一个{1}！",
            "虽然{1}不能是相当令人满意的，但是生活还是要继续不是吗？",
            "俗话说塞凡的父亲失去了马，可能不是不幸运的，正如这个{1}你刚刚投的一样。",
            "可能你期待的是一个1，但是{1}也是个不错的选择不是么？",
            "好的，现在，让我们揭晓一下答案吧！是{1}，我说是{1}！真是不可思议！是{1}！",
        };
        private static string[] Rolls = new string[] {
            "让我们看看{2}用{0}投出了了什么东西？",
            "谁能想到这是一个{0}的骰子投出来的呢？",
            "谁又能想到这个{0}是{2}的杰作呢？",
            "如果，我是说如果，这个{0}的骰子能够更加懂得礼节的话，",
            "当然，骰子是听不懂{2}的话的，哪怕是{0}的骰子也一样。",
            "虽然这只是一个{0}的骰子，事实上它的确是一个{0}的骰子，",
            "我打赌，这个{0}会如你所愿的，会的，",
            "有时候，一个微小的骰子也能释放无穷的乐趣，不是吗？例如这个{0}的骰子，",
            "作为{0}骰，尊严会被拾起在落下的那个瞬间，",
        };



        private static string OddsAndEnds() {
            string pattern = Dice.Roll(Patterns);
            StringBuilder builder = new StringBuilder();
            Match m = Regex.Match(pattern, @"\[([^\[\]]+)\]");
            int prev = 0;
            while (m.Success) {
                string key = m.Groups[1].Value;
                string replacement = Tags.TryGetValue(key, out var list) ? Dice.Roll(list) : key;
                builder.Append(pattern, prev, m.Index - prev).Append(replacement);
                prev = m.Index + m.Length;
                m = m.NextMatch();
            }
            builder.Append(pattern, prev, pattern.Length - prev);
            return builder.ToString();
        }

        private static string[] Patterns = new string[] {
            "我简直想用[名字]的[物品]塞进你的[目标]。",
            "说实话，伙计，你的[目标]就像[名字]家的那[物品]一样。",
            "虽然你的手气不像[名字]那么地糟糕，但是泥吧[物品]放在[目标]里地时候着实地让我想起了[名字]的[物品]。",
            "这谁又能想到的，对吗，[名字]？",
            "如此一来，我就只好祝福你了，我的朋友，希望你在[名字]家度过一个愉快的晚上。",
        };

        private static Dictionary<string, string[]> Tags = new Dictionary<string, string[]> {
            ["目标"] = new string[] {
                "鼻孔",
                "屁股",
                "脸颊",
                "脑门",
                "鼻子",
            },
            ["名字"] = new string[] {
                "玛莎",
                "特斯吉洛的二姑妈",
                "泰温兰尼斯特",
                "你",
                "我",
                "KP",
                "双儿",
                "JS",
                "岚岚",
                "曦月",
                "老安",
                "邻居的黑猫",
            },
            ["物品"] = new string[] {
                "钥匙链",
                "棒球棒",
                "狼牙棒",
                "拳头",
                "发了霉的橙子",
                "楼上这只愚蠢的土拨鼠",
                "会飞的蟑螂",
                "花栗鼠",
                "老八吃剩的玩意儿",
            },
        };


        public static string Choose(DMEnv env, string[] results) {
            if (results.Length > 0) {
                int index = Dice.Roll(results.Length);
                if (index >= 0 && index < results.Length) {
                    return env.Next = results[index];
                } else {
                    return env.Next = "骰娘出错了๐·°(৹˃̵﹏˂̵৹)°·๐";
                }
            }
            return env.Next = "没有选项哦";
        }
    }
}
