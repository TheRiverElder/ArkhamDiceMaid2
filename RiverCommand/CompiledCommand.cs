using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.RiverCommand;

namespace RiverCommand {
    public class CompiledCommand<TEnv> {

        public int ReaderCursor { get; }
        public int Length { get; }
        public string Error { get; }

        private CmdExecutor<TEnv> Executor;
        private TEnv Env;
        private Args Args;
        private Args Dict;

        public bool IsErr => !string.IsNullOrEmpty(Error);

        public string ErrorStr => $"解析错误：第{Length}个参数\n" + Error;

        public CompiledCommand(int readerCursor, int length, CmdExecutor<TEnv> executor, TEnv env, Args args, Args dict) {
            ReaderCursor = readerCursor;
            Length = length;
            Executor = executor;
            Env = env;
            Args = args;
            Dict = dict;
        }

        public CompiledCommand(int readerCursor, int length, string error) {
            ReaderCursor = readerCursor;
            Length = length;
            Error = error;
        }

        public object Execute(out string reply) {
            try {
                return Executor(Env, Args, Dict, out reply);
            } catch (Exception e) {
                reply = e.Message;
                return false;
            }
        }

    }
}
