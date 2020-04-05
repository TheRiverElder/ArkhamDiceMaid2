using System.Collections.Generic;

using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;

namespace top.riverelder.arkham.Code.Commands {


    class Command_Value : DiceCmdEntry {

        public string Usage => "数值 <属性名> <增加|设置|别名> <调整数值|新名字>";

        public override void OnRegister(CmdDispatcher<DMEnv> dispatcher) {
            dispatcher.Register("数值").Then(
                PresetNodes.String<DMEnv>("数值名").Then(
                    PresetNodes.Literal<DMEnv>("增加").Then(Extensions.Dice<DMEnv>("增量").Executes((env, args, dict) => IncVal(env.Inv, args.GetStr("数值名"), args.GetDice("增量"))))
                ).Then(
                    PresetNodes.Literal<DMEnv>("设置").Then(Extensions.Value<DMEnv>("新值").Executes((env, args, dict) => SetVal(env.Inv, args.GetStr("数值名"), args.Get<Value>("新值"))))
                ).Then(
                    PresetNodes.Literal<DMEnv>("别名").Then(PresetNodes.String<DMEnv>("新名").Executes((env, args, dict) => NewName(env.Inv, args.GetStr("数值名"), args.GetStr("新名"))))
                )
            ).Handles(Extensions.ExistSelfInv());
        }

        public static string IncVal(Investigator inv, string valueName, Dice increment) {
            if (!inv.Values.TryWidelyGet(valueName, out Value value)) {
                value = new Value(1);
                inv.Values.Put(valueName, value);
            }
            int prev = value.Val;
            int inc = increment.Roll();
            value.Add(inc);
            return $"{inv.Name}的{valueName}: {prev} + {inc} => {value.Val}";
        }

        public static string SetVal(Investigator inv, string valueName, Value newValue) {
            if (!inv.Values.TryWidelyGet(valueName, out Value value)) {
                value = new Value(1);
                inv.Values.Put(valueName, value);
            }
            string prev = value.ToString();
            if (newValue.Max > 0) {
                value.Max = newValue.Max;
            }
            value.Set(newValue.Val);
            return $"{inv.Name}的{valueName}: {prev} => {value.ToString()}";
        }

        public static string NewName(Investigator inv, string valueName, string newName) {
            if (!inv.Values.Names.Contains(valueName)) {
                return $"{inv.Name}没有本名为{valueName}的数值";
            }
            inv.Values.Set(valueName, newName);
            return $"{inv.Name}的{valueName}的新别名：{newName}";
        }
    }
}
