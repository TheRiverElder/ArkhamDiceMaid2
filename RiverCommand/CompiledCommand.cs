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

        public CmdExecutor<TEnv> Executor;
        public TEnv Env { get; set; }
        public Args Args { get; set; }
        public Args Dict { get; set; }

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
            return Executor(Env, Args, Dict, out reply);
        }

    }
}
