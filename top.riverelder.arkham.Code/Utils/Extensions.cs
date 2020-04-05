using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Model;
using top.riverelder.RiverCommand;

namespace top.riverelder.arkham.Code.Utils {
    public static class Extensions {

        public static CommandNode<TEnv> Dice<TEnv>(string name) {
            return new CommandNode<TEnv>(name, new DiceParser());
        }


        public static int GetInt(this Args args, string name) {
            return args.Get<int>(name);
        }

        public static bool GetBool(this Args args, string name) {
            return args.Get<bool>(name);
        }

        public static string GetStr(this Args args, string name) {
            return args.Get<string>(name);
        }

        public static Dice GetDice(this Args args, string name) {
            return args.Get<Dice>(name);
        }

        public static Investigator GetInv(this Args args, string name) {
            return args.Get<Investigator>(name);
        }

        public static Value GetVal(this Args args, string name) {
            return args.Get<Value>(name);
        }

    }
}
