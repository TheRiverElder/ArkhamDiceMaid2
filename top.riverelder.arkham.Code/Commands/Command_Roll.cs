using System.Collections.Generic;
using System.Text;
using top.riverelder.arkham.Code.Bot;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;

namespace top.riverelder.arkham.Code.Commands
{
    public class Command_Roll : ICommand
    {
        public string Name => "投掷";

        public ArgumentValidater Validater { get; } = ArgumentValidater.Empty
                .SetListArgCount(1)
                .AddListArg(ArgumentValidater.Dice);

        public string Usage => "投掷 <骰子表达式>";

        public string Execute(string[] listArgs, IDictionary<string, string> dictArgs, string raw, CmdEnv env)
        {
            string diceExp = listArgs[0];

            return $"投掷结果：{diceExp} = {Dice.Roll(diceExp)}";
        }
    }
}
