using System.Collections.Generic;

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
                        Extensions.Value<DMEnv>("新值")
                        .Executes((env, args, dict) => SetVal(env.Sce, env.Inv, args.GetStr("数值名"), args.Get<Value>("新值")))
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
            );

            dispatcher.SetAlias("设值", "数值 设值");
            dispatcher.SetAlias("增值", "数值 增加");
            dispatcher.SetAlias("减值", "数值 减少");
            dispatcher.SetAlias("回血", "数值 增加 体力");
            dispatcher.SetAlias("扣血", "数值 减少 体力");
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

        public static string SetVal(Scenario scenario, Investigator inv, string valueName, Value newValue) {
            if (!inv.Values.TryWidelyGet(valueName, out Value value)) {
                value = new Value(1);
                inv.Values.Put(valueName, value);
            }
            string prev = value.ToString();
            if (newValue.Max > 0) {
                value.Max = newValue.Max;
            }
            value.Set(newValue.Val);
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
    }
}
