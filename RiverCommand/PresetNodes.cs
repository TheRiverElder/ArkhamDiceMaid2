using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.RiverCommand.ParamParsers;
using top.riverelder.RiverCommand.Utils;

namespace top.riverelder.RiverCommand {
    public static class PresetNodes {

        public static CommandNode<TEnv> Literal<TEnv>(string word) {
            return new CommandNode<TEnv>(new LiteralParser<TEnv>(word));
        }

        public static CommandNode<TEnv> Int<TEnv>(string name) {
            return new CommandNode<TEnv>(name, new IntParser<TEnv>());
        }
        
        public static CommandNode<TEnv> String<TEnv>(string name) {
            return new CommandNode<TEnv>(name, new StringParser<TEnv>());
        }

        public static CommandNode<TEnv> Or<TEnv>(string name, params string[] words) {
            return new CommandNode<TEnv>(name, new OrParser<TEnv>(words));
        }

        public static CommandNode<TEnv> Bool<TEnv>(string name, string trueValue, string falseValue) {
            return new CommandNode<TEnv>(name, new BoolParser<TEnv>(trueValue, falseValue));
        }

        public static CommandNode<TEnv> Rest<TEnv>(string name) {
            return new CommandNode<TEnv>(name, new RestParser<TEnv>());
        }
    }
}
