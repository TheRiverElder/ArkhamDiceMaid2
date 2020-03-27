using System.Collections.Generic;
using top.riverelder.arkham.Code.Bot;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;

namespace top.riverelder.arkham.Code.Commands
{
    class Command_Value : ICommand
    {
        public string Name => "数值";

        public ArgumentValidater Validater { get; } = ArgumentValidater.Empty
                .SetListArgCount(3)
                .AddListArg(@"增加|设置|别名")
                .AddListArg(ArgumentValidater.Any)
                .AddListArg(ArgumentValidater.Any);

        public string Usage => "数值 <增加|设置|别名> <属性名> <调整数值|新名字>";

        public string Execute(string[] listArgs, IDictionary<string, string> dictArgs, string originalString, CmdEnv env)
        {
            if (!EnvValidator.ExistInv(env, out Investigator inv, out string err)) {
                return err;
            }

            string opt = listArgs[0];
            string name = listArgs[1];
            string arg3 = listArgs[2];

            if ("别名".Equals(opt)) {
                inv.Values.Set(name, arg3);
                return $"{name}的新别名：{arg3}";
            }

            if (!inv.Values.TryGet(name, out Value v)) {
                v = new Value(1);
                inv.Values.Put(name, v);
            }

            if (!Dice.TryParse(arg3, out Dice dice, out int length) || length == 0) {
                return $"数值必须是整数或骰子表达式：{arg3}";
            }
            int value = dice.Roll();

            int prev = v.Val;
            string addon = "";
            switch (opt) {
                case "增加": v.Add(value); addon = $" + {value}"; break;
                case "设置": v.Set(value); break;
            }
            SaveUtil.Save(env.Scenario);
            return $"{inv.Name}的{name}: {prev}{addon} => {v.Val}";
        }
    }
}
