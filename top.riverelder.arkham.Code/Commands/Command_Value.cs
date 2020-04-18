using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;
using static top.riverelder.RiverCommand.PresetNodes;

namespace top.riverelder.arkham.Code.Commands {


    class Command_Value : DiceCmdEntry {

        public string Usage => "数值 <属性名> <增加|设置|别名> <调整数值|新名字>";

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.Register("数值")
            .Handles(Extensions.ExistSelfInv())
            .Then(
                Literal<DMEnv>("增加").Then(
                    String<DMEnv>("数值名").Then(
                        Extensions.Dice("增量")
                        .Executes((env, args, dict) => ChangeVal(env.Sce, env.Inv, args.GetStr("数值名"), args.GetDice("增量"), true))
                   )
                )
            ).Then(
                Literal<DMEnv>("减少").Then(
                    String<DMEnv>("数值名").Then(
                        Extensions.Dice("减量")
                        .Executes((env, args, dict) => ChangeVal(env.Sce, env.Inv, args.GetStr("数值名"), args.GetDice("减量"), false))
                    )
                )
            ).Then(
                Literal<DMEnv>("设置").Then(
                    String<DMEnv>("数值名").Then(
                        Int<DMEnv>("新值")
                        .Executes((env, args, dict) => SetVal(env.Sce, env.Inv, args.GetStr("数值名"), args.GetInt("新值"), -1))
                        .Then(
                            Int<DMEnv>("上限")
                            .Executes((env, args, dict) => SetVal(env.Sce, env.Inv, args.GetStr("数值名"), args.GetInt("新值"), args.GetInt("上限")))
                        )
                    )
                )
            ).Then(
                Literal<DMEnv>("上限").Then(
                    String<DMEnv>("数值名").Then(
                        Int<DMEnv>("上限值")
                        .Executes((env, args, dict) => SetMax(env.Sce, env.Inv, args.GetStr("数值名"), args.GetInt("上限值")))
                    )
                )
            ).Then(
                Literal<DMEnv>("别名").Then(
                    String<DMEnv>("数值名")
                    .Handles(Extensions.ExistSelfValue())
                    .Then(
                        String<DMEnv>("新名")
                        .Executes((env, args, dict) => NewName(env.Sce, env.Inv, args.GetStr("数值名"), args.GetStr("新名")))
                    )
                )
            ).Then(
                Literal<DMEnv>("st")
                .Executes((env, args, dict) => StHelp())
                .Then(
                    Rest<DMEnv>("数值串")
                    .Executes((env, args, dict) => St(env, env.Inv, args.GetStr("数值串")))
                )
            ).Then(
                Literal<DMEnv>("补全")
                .Executes((env, args, dict) => CompleteWithDefaultValues(env.Sce, env.Inv))
            ).Then(
                Literal<DMEnv>("覆盖")
                .Executes((env, args, dict) => FillWithDefaultValues(env.Sce, env.Inv, false))
                .Then(
                    Literal<DMEnv>("强制")
                    .Executes((env, args, dict) => FillWithDefaultValues(env.Sce, env.Inv, true))
                )
            );

            dispatcher.SetAlias("设值", "数值 设置");
            dispatcher.SetAlias("增值", "数值 增加");
            dispatcher.SetAlias("减值", "数值 减少");
            dispatcher.SetAlias("回血", "数值 增加 体力");
            dispatcher.SetAlias("扣血", "数值 减少 体力");
            dispatcher.SetAlias("st", "数值 st");
        }

        public static string ChangeVal(Scenario scenario, Investigator inv, string valueName, Dice increment, bool posotive) {
            if (!inv.Values.TryWidelyGet(valueName, out Value value)) {
                value = new Value(1);
                inv.Values.Put(valueName, value);
            }
            int prev = value.Val;
            int inc = increment.Roll() * (posotive ? 1 : -1);
            value.Add(inc);
            SaveUtil.Save(scenario);
            return $"{inv.Name}的{valueName}: {prev} + {inc} => {value.Val}";
        }

        public static string SetVal(Scenario scenario, Investigator inv, string valueName, int val, int max) {
            if (!inv.Values.TryWidelyGet(valueName, out Value value)) {
                value = new Value(1);
                inv.Values.Put(valueName, value);
            }
            string prev = value.ToString();
            if (max > 0) {
                value.Max = max;
            }
            value.Set(val);
            SaveUtil.Save(scenario);
            return $"{inv.Name}的{valueName}: {prev} => {value.ToString()}";
        }

        public static string SetMax(Scenario scenario, Investigator inv, string valueName, int max) {
            if (!inv.Values.TryWidelyGet(valueName, out Value value)) {
                value = new Value(1);
                inv.Values.Put(valueName, value);
            }
            string prev = value.ToString();
            value.Max = max;
            SaveUtil.Save(scenario);
            return $"{inv.Name}的{valueName}: {prev} => {value.ToString()}";
        }

        public static string NewName(Scenario scenario, Investigator inv, string valueName, string newName) {
            inv.Values.Set(valueName, newName);
            SaveUtil.Save(scenario);
            return $"{inv.Name}的{valueName}的新别名：{newName}";
        }

        public static string FillWithDefaultValues(Scenario scenario, Investigator inv, bool force) {
            if (!force) {
                return "覆盖操作有极大可能会对你已经设定的数值进行更改！若确定你在做什么，请在指令后面加上“强制”";
            }
            inv.Values.FillWith(Global.DefaultValues);
            SaveUtil.Save(scenario);
            return $"{inv.Name}的数值与别名已被默认值覆盖！已有的数值可能被覆盖！";
        }

        public static string CompleteWithDefaultValues(Scenario scenario, Investigator inv) {
            inv.Values.CompleteWith(Global.DefaultValues);
            SaveUtil.Save(scenario);
            return $"{inv.Name}的数值与别名已被默认值补全！已有的数值未被修改！";
        }

        public static string StHelp() {
            return $"使用st子指令时，请保证除了“st”与剩下内容之间的空格外，再没有其它空白了！";
        }

        public static string St(DMEnv env, Investigator inv, string str) {
            ValueSet values = inv.Values;
            StringBuilder sb = new StringBuilder().Append(inv.Name).Append("的数值：");
            Regex reg = new Regex(@"(\D+)(\d+)");
            Match m = reg.Match(str);
            int i = 0;
            while (m.Success) {
                int val = int.TryParse(m.Groups[2].Value, out int v) ? v : 1;
                string name = m.Groups[1].Value;
                Value value = new Value(val);
                values.Put(name, value);
                if (i++ % 3 == 0) {
                    sb.AppendLine();
                }
                sb.Append(name).Append('：').Append(value.ToString()).Append(' ');
                m = m.NextMatch();
            }
            env.Save();
            return sb.ToString();
        }
    }
}
