using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.RiverCommand;

namespace top.riverelder.RiverCommand.Parsing {

    public class CommandResult<TEnv> : ICmdResult {

        public int MatchedLength { get; }

        public int ArgLength { get; }

        public bool IsError => false;

        public string Error => throw new NotSupportedException("命令调度的匹配结果不是错误信息");

        public string[] Errors => throw new NotSupportedException("命令调度的匹配结果不是错误信息");

        public CmdExecutor<TEnv> Executor;
        public TEnv Env { get; set; }
        public Args Args { get; set; }
        public Args Dict { get; set; }

        public CommandResult(int matchedLength, int argLength, CmdExecutor<TEnv> executor, TEnv env, Args args, Args dict) {
            MatchedLength = matchedLength;
            ArgLength = argLength;
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
