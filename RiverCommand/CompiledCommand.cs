using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.RiverCommand;

namespace RiverCommand {
    public class CompiledCommand<TEnv> {

        public int Length { get; }

        private CmdExecutor<TEnv> Executor;
        private TEnv Env;
        private Args Args;
        private Args Dict;

        public CompiledCommand(int length, CmdExecutor<TEnv> executor, TEnv env, Args args, Args dict) {
            Length = length;
            Executor = executor;
            Env = env;
            Args = args;
            Dict = dict;
        }

        public object Execute() {
            return Executor(Env, Args, Dict);
        }

    }
}
