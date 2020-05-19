using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.RiverCommand;

namespace RiverCommand {
    public class CompiledCommand<TEnv> {

        public int Length { get; }
        public string Error { get; }

        private CmdExecutor<TEnv> Executor;
        private TEnv Env;
        private Args Args;
        private Args Dict;

        public bool IsErr => !string.IsNullOrEmpty(Error);

        public string ErrorStr => $"层级{Length}，" + Error;

        public CompiledCommand(int length, CmdExecutor<TEnv> executor, TEnv env, Args args, Args dict) {
            Length = length;
            Executor = executor;
            Env = env;
            Args = args;
            Dict = dict;
        }

        public CompiledCommand(int length, string error) {
            Length = length;
            Error = error;
        }

        public object Execute(out string reply) {
            try {
                reply = (string)Executor(Env, Args, Dict);
            } catch (Exception e) {
                reply = e.Message;
            }
            return reply;
        }

    }
}
