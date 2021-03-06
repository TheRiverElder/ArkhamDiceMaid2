﻿using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;
using top.riverelder.RiverCommand.Parsing;
using static top.riverelder.RiverCommand.PresetNodes;

namespace top.riverelder.arkham.Code.Commands {


    class Command_Value : DiceCmdEntry {

        public string Usage => "数值 <属性名> <增加|设置|别名> <调整数值|新名字>";

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.Register("数值")
            .Then(
                Literal<DMEnv>("增加").Then(
                    String<DMEnv>("数值名").Then(
                        Extensions.Dice("增量")
                        .Executes((env, args, dict) => ChangeVal(env, env.Sce, env.Inv, args.GetStr("数值名"), args.GetDice("增量"), true))
                   )
                )
            ).Then(
                Literal<DMEnv>("减少").Then(
                    String<DMEnv>("数值名").Then(
                        Extensions.Dice("减量")
                        .Executes((env, args, dict) => ChangeVal(env, env.Sce, env.Inv, args.GetStr("数值名"), args.GetDice("减量"), false))
                    )
                )
            ).Then(
                Literal<DMEnv>("设置").Then(
                    String<DMEnv>("数值名").Then(
                        Int<DMEnv>("新值")
                        .Executes((env, args, dict) => SetVal(env, env.Sce, env.Inv, args.GetStr("数值名"), args.GetInt("新值"), -1))
                        .Then(
                            Int<DMEnv>("上限")
                            .Executes((env, args, dict) => SetVal(env, env.Sce, env.Inv, args.GetStr("数值名"), args.GetInt("新值"), args.GetInt("上限")))
                        )
                    )
                )
            ).Then(
                Literal<DMEnv>("删除").Then(
                    String<DMEnv>("数值名")
                    .Handles(Extensions.ExistSelfValue)
                    .Executes((env, args, dict) => RemoveVal(env, env.Sce, env.Inv, args.GetStr("数值名")))
                )
            ).Then(
                Literal<DMEnv>("上限").Then(
                    String<DMEnv>("数值名")
                    .Then(
                        Int<DMEnv>("上限值")
                        .Executes((env, args, dict) => SetMax(env, env.Sce, env.Inv, args.GetStr("数值名"), args.GetInt("上限值")))
                    )
                )
            ).Then(
                Literal<DMEnv>("别名").Then(
                    String<DMEnv>("数值名")
                    .Handles(Extensions.ExistSelfValue)
                    .Then(
                        String<DMEnv>("新名")
                        .Executes((env, args, dict) => NewName(env, env.Sce, env.Inv, args.GetStr("数值名"), args.GetStr("新名")))
                    )
                )
            ).Then(
                Literal<DMEnv>("st")
                .Executes((env, args, dict) => env.Next = "使用st子指令时，请保证除了“st”与剩下内容之间的空格外，再没有其它空白！")
                .Then(
                    Rest<DMEnv>("数值串")
                    .Executes((env, args, dict) => St(env, env.Inv, args.GetStr("数值串")))
                )
            ).Then(
                Literal<DMEnv>("补全")
                .Executes((env, args, dict) => CompleteWithDefaultValues(env, env.Sce, env.Inv))
            ).Then(
                Literal<DMEnv>("覆盖")
                .Executes((env, args, dict) => FillWithDefaultValues(env, env.Sce, env.Inv, false))
                .Then(
                    Literal<DMEnv>("强制")
                    .Executes((env, args, dict) => FillWithDefaultValues(env, env.Sce, env.Inv, true))
                )
            );

            dispatcher.SetAlias("设值", "数值 设置");
            dispatcher.SetAlias("增值", "数值 增加");
            dispatcher.SetAlias("减值", "数值 减少");
            dispatcher.SetAlias("回血", "数值 增加 体力");
            dispatcher.SetAlias("扣血", "数值 减少 体力");
            dispatcher.SetAlias("st", "数值 st");

            dispatcher.SetAlias("vl", "数值");
            dispatcher.SetAlias("sv", "数值 设置");
            dispatcher.SetAlias("ic", "数值 增加");
            dispatcher.SetAlias("dc", "数值 减少");
            dispatcher.SetAlias("ih", "数值 增加 体力");
            dispatcher.SetAlias("dh", "数值 减少 体力");
        }

        public static bool RemoveVal(DMEnv env, Scenario scenario, Investigator inv, string valueName) {
            if (inv.Values.Remove(valueName, out bool isAlias)) {
                SaveUtil.Save(scenario);
                env.Append($"移除了{inv.Name}的{(isAlias ? "别名" : "数值")}：{valueName}");
                return true;
            }
            env.Append($"未找到{inv.Name}的{valueName}");
            return false;
        }

        public static bool ChangeVal(DMEnv env, Scenario scenario, Investigator inv, string valueName, Dice increment, bool posotive) {
            int inc = increment.Roll() * (posotive ? 1 : -1);
            env.Append(inv.Change(valueName, inc));
            SaveUtil.Save(scenario);
            return true;
        }

        public static bool SetVal(DMEnv env, Scenario scenario, Investigator inv, string valueName, int val, int max) {
            if (!inv.Values.TryWidelyGet(valueName, out Value value)) {
                value = new Value(1);
                inv.Values.Put(valueName, value);
            }
            string prev = value.ToString();
            if (max > 0) {
                value.Max = max;
            }
            value.Set(val);
            env.Append($"{inv.Name}的{valueName}: {prev} => {value.ToString()}");
            SaveUtil.Save(scenario);
            return true;
        }

        public static bool SetMax(DMEnv env, Scenario scenario, Investigator inv, string valueName, int max) {
            if (!inv.Values.TryWidelyGet(valueName, out Value value)) {
                value = new Value(1);
                inv.Values.Put(valueName, value);
            }
            string prev = value.ToString();
            value.Max = max;
            env.Append($"{inv.Name}的{valueName}: {prev} => {value.ToString()}");
            SaveUtil.Save(scenario);
            return true;
        }

        public static bool NewName(DMEnv env, Scenario scenario, Investigator inv, string valueName, string newName) {
            if (inv.Values.SetAlias(newName, valueName)) {
                SaveUtil.Save(scenario);
                env.Append($"{inv.Name}的{valueName}的新别名：{newName}");
                return true;
            } else {
                env.Append($"{inv.Name}不存在原本名为{valueName}的数值");
                return false;
            }
        }

        public static void FillWithDefaultValues(DMEnv env, Scenario scenario, Investigator inv, bool force) {
            if (!force) {
                env.Append("覆盖操作有极大可能会对你已经设定的数值进行更改！若确定你在做什么，请在指令后面加上“强制”");
            }
            inv.Values.FillWith(Global.DefaultValues);
            SaveUtil.Save(scenario);
            env.Append($"{inv.Name}的数值与别名已被默认值覆盖！已有的数值可能被覆盖！");
        }

        public static void CompleteWithDefaultValues(DMEnv env, Scenario scenario, Investigator inv) {
            inv.Values.CompleteWith(Global.DefaultValues);
            SaveUtil.Save(scenario);
            env.Append($"{inv.Name}的数值与别名已被默认值补全！已有的数值未被修改！");
        }
        
        public static void St(DMEnv env, Investigator inv, string str) {
            ValueSet values = inv.Values;
            env.Append(inv.Name).Append("的数值：");
            Regex reg = new Regex(@"(\D+)(\d+)");
            Match m = reg.Match(str);
            int i = 0;
            while (m.Success) {
                int val = int.TryParse(m.Groups[2].Value, out int v) ? v : 1;
                string name = m.Groups[1].Value;
                if (values.TryWidelyGet(name, out Value value)) {
                    value.Set(val);
                } else {
                    value = new Value(val);
                    values.Put(name, value);
                }
                if (i++ % 3 == 0) {
                    env.Line();
                }
                env.Append(name).Append('：').Append(value).Append(' ');
                m = m.NextMatch();
            }
            env.Save();
        }
    }
}
