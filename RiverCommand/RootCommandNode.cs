using RiverCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.RiverCommand;
using top.riverelder.RiverCommand.ParamParsers;
using top.riverelder.RiverCommand.Parsing;
using top.riverelder.RiverCommand.Utils;

namespace top.riverelder.RiverCommand {
    public class RootCommandNode<TEnv> : CommandNode<TEnv> {
        public RootCommandNode(ParamParser<TEnv> parser) : base(null) {
        }

        public RootCommandNode() : base(null) {
        }

        public bool TryGetCommand(string head, out CommandNode<TEnv> cmd) {
            return certainChildren.TryGetValue(head, out cmd);
        }

        public ICollection<CommandNode<TEnv>> GetAllCommand() {
            return certainChildren.Values;
        }

        public new bool Dispatch(
            CmdDispatcher<TEnv> dispatcher,
            StringReader reader,
            TEnv env,
            Args args,
            int level,
            List<ICmdResult> res
        ) {
            // 直接匹配子节点
            return MatchChildren(dispatcher, reader, env, args, level + 1, res);
        }
    }
}
