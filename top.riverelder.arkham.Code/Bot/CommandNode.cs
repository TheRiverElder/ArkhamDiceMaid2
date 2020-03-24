using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Bot.ParamValidator;

namespace top.riverelder.arkham.Code.Bot {
    public class CommandNode {
        public Regex Reg { get; set; }
        public string ParamName { get; set; }
        public IParam Param { get; set; }
        public IList<CommandNode> Children { get; } = new List<CommandNode>();
        public Func<CmdEnv, bool> EnvValidator { get; set; }
        public ICmdExecutor Executor { get; set; }

        public CommandNode(string paramName, IParam param) {
            ParamName = paramName;
            Param = param;
        }

        public CommandNode() {
        }

        public bool Dispatch(string raw, CmdEnv env, Dictionary<string, object> args, out string reply) {
            string next = raw;
            string err = "参数匹配错误";
            Match m = Regex.Match(raw, @"^\s+");
            raw = m.Success ? raw.Substring(m.Value.Length) : raw;
            if ((EnvValidator == null || EnvValidator.Invoke(env)) && Param.Validate(raw, out object arg, out int length, out err)) {
                args[ParamName] = arg;
                next = raw.Substring(length);
            } else {
                reply = err;
                return false;
            }
            if (Children.Count == 0) {
                if (Executor != null) {
                    reply = Executor.Execute(args, new Dictionary<string, string>(), env);
                } else {
                    reply = "参数匹配成功，但是没有执行者";
                }
                return false;
            }
            foreach (CommandNode child in Children) {
                if (child.Dispatch(raw, env, new Dictionary<string, object>(args), out reply)) {
                    return true;
                }
            }
            reply = "参数匹配错误";
            return false;
        }

        public CommandNode Then(CommandNode node) {
            Children.Add(node);
            return this;
        }

        public CommandNode Executes(ICmdExecutor executor) {
            Executor = executor;
            return this;
        }
    }
}
