﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;
using top.riverelder.RiverCommand.ParamParsers;

namespace top.riverelder.arkham.Code.Commands {
    public class Command_Coc7 : DiceCmdEntry {
        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            DictMapper mapper = new DictMapper()
                .Then("年龄", new IntParser())
                .SkipRest();

            dispatcher.Register("coc7").Then(
                PresetNodes.Int<DMEnv>("数量")
                .MapDict(mapper)
                .Executes((env, args, dict) => DrawProperties(args.GetInt("数量"), dict.Get("年龄", -1)))
            )
            .MapDict(mapper)
            .Executes((env, args, dict) => DrawProperties(5, dict.Get("年龄", -1)));

            dispatcher.SetAlias("COC7", "coc7");
            dispatcher.SetAlias("COC", "coc7");
            dispatcher.SetAlias("coc", "coc7");
        }

        public static Dictionary<string, Dice> properties = new Dictionary<string, Dice> {
            ["力量"] = Dice.Of("3d6"),
            ["体质"] = Dice.Of("3d6"),
            ["体型"] = Dice.Of("2d6+6"),
            ["敏捷"] = Dice.Of("3d6"),
            ["外貌"] = Dice.Of("3d6"),
            ["智力"] = Dice.Of("2d6+6"),
            ["意志"] = Dice.Of("3d6"),
            ["教育"] = Dice.Of("2d6+6"),
        };

        public static string DrawProperties(int size, int age) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < size; i++) {
                if (i > 0) {
                    sb.AppendLine().AppendLine("----------------");
                }
                sb.Append(DrawProperty(age));
            }
            return sb.ToString();
        }

        public static string DrawProperty(int age) {
            StringBuilder sb = new StringBuilder();
            Dictionary<string, int> result = new Dictionary<string, int>();
            Dictionary<string, int> addons = new Dictionary<string, int>();
            int rest = 0;
            // 基础值
            foreach (var e in properties) {
                result[e.Key] = e.Value.Roll() * 5;
            }
            // 年龄的影响
            if (age > 0) {
                if (age < 20) {
                    ConCheck(-5, "力量", "体型", addons);
                    addons["教育"] = -5;
                } else if (age < 40) {
                    AddEdu(1, result, addons);
                } else if (age < 50) {
                    AddEdu(2, result, addons);
                    ConCheck(-5, "力量", "体质", "敏捷", addons);
                    addons["外貌"] = -5;
                } else if (age < 60) {
                    AddEdu(3, result, addons);
                    ConCheck(-10, "力量", "体质", "敏捷", addons);
                    addons["外貌"] = -10;
                } else if (age < 70) {
                    AddEdu(4, result, addons);
                    ConCheck(-20, "力量", "体质", "敏捷", addons);
                    addons["外貌"] = -15;
                } else if (age < 80) {
                    AddEdu(4, result, addons);
                    ConCheck(-40, "力量", "体质", "敏捷", addons);
                    addons["外貌"] = -20;
                } else {
                    AddEdu(4, result, addons);
                    ConCheck(-80, "力量", "体质", "敏捷", addons);
                    addons["外貌"] = -25;
                }
            }
            // 统计
            foreach (var e in result) {
                sb.Append(e.Key).Append(':');
                int value = e.Value;
                if (addons.TryGetValue(e.Key, out int addon)) {
                    value += addon;
                    value = Math.Max(0, Math.Min(value, 99));
                    sb
                        .Append(value)
                        .Append('(')
                        .Append(e.Value).Append(addon >= 0 ? "+" + addon : Convert.ToString(addon))
                        .Append(')');
                } else {
                    sb.Append(e.Value);
                }
                sb.Append(' ');
                rest += value;
            }
            int luck = Dice.Roll("3d6") * 5;
            sb.Append("幸运:");
            if (age > 0 && age < 20) {
                int addLuck = Dice.Roll("3d6") * 5;
                sb.Append(Math.Max(luck, addLuck)).Append('(').Append(Math.Min(luck, addLuck)).Append(')');
            } else {
                sb.Append(luck);
            }
            sb.AppendLine();
            sb.Append($"带幸运：{rest + luck}，不带幸运：{rest}");
            return sb.ToString();
        }

        public static void AddEdu(Dictionary<string, int> result, Dictionary<string, int> addons) {
            if (!result.TryGetValue("教育", out int edu)) {
                edu = 0;
            }
            if (Dice.Roll("1d100") > edu) {
                addons["教育"] = Dice.Roll("1d10");
            }
        }

        public static void AddEdu(int times, Dictionary<string, int> result, Dictionary<string, int> addons) {
            if (!result.TryGetValue("教育", out int edu)) {
                edu = 0;
            }
            int prev = edu;
            for (int i = 0; i < times; i++) {
                if (Dice.Roll("1d100") > edu) {
                    int addon = Dice.Roll("1d10");
                    edu += addon;
                }
            }
            addons["教育"] = edu - prev;
        }

        private static int seed = (int)DateTime.Now.Ticks;
        public static void ConCheck(int total, string key1, string key2, Dictionary<string, int> addons) { 
            int val1 = (total / Math.Abs(total)) * new Random(seed++).Next(Math.Abs(total) + 1);
            int val2 = total - val1;
            addons[key1] = val1;
            addons[key2] = val2;
        }

        public static void ConCheck(int total, string key1, string key2, string key3, Dictionary<string, int> addons) {
            int sign = total / Math.Abs(total);
            int val1 = sign * new Random(seed++).Next(Math.Abs(total) + 1);
            int val2 = sign * new Random(seed++).Next(Math.Abs(total - val1) + 1);
            int val3 = total - val1 - val2;
            addons[key1] = val1;
            addons[key2] = val2;
            addons[key3] = val3;
        }
    }
}
