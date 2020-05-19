using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using top.riverelder.RiverCommand.ParamParsers;
using top.riverelder.RiverCommand.Utils;

namespace top.riverelder.RiverCommand {
    public class CommandNode<TEnv> {
        

        public string ParamName { get; set; } = null;
        public ParamParser Parser { get; } = null;
        public DictMapper Mapper { get; set; } = null;
        public bool Spread { get; set; } = false;

        private readonly IDictionary<string, CommandNode<TEnv>> certainChildren = new Dictionary<string, CommandNode<TEnv>>();
        private readonly IList<CommandNode<TEnv>> children = new List<CommandNode<TEnv>>();

        public PreHandler<TEnv> Process { get; set; } = null;
        public CmdExecutor<TEnv> Executor { get; set; } = null;

        public CommandNode(string paramName, ParamParser parser) {
            ParamName = paramName;
            Parser = parser;
        }

        public CommandNode() {
        }

        public CommandNode(ParamParser parser) {
            Parser = parser;
        }

        /// <summary>
        /// 调度参数解析
        /// </summary>
        /// <param name="reader">原始字符流</param>
        /// <param name="env">环境</param>
        /// <param name="args">参数</param>
        /// <param name="reply">回复消息</param>
        /// <returns>是否符合该节点</returns>
        public DispatchResult Dispatch(StringReader reader, TEnv env, Args args, out string reply) {
            string err = "参数匹配错误";
            reader.SkipWhiteSpace();
            int start = reader.Cursor;
            // 匹配参数
            object ori = null;
            if (Spread) { // 收集所有剩下的参数
                List<object> list = new List<object>();
                int s = start;
                while (Parser.TryParse(reader, out ori)) {
                    list.Add(ori);
                    s = reader.Cursor;
                    reader.SkipWhiteSpace();
                }
                reader.Cursor = s;
                if (list.Count == 0) {
                    reply = err;
                    reader.Cursor = start;
                    return DispatchResult.Unmatched;
                }
                ori = list.ToArray();
            } else { // 仅检测一个参数
                if (!Parser.TryParse(reader, out ori)) {
                    reader.Cursor = start;
                    reply = err;
                    return DispatchResult.Unmatched;
                }
            }

            // 预处理得到的参数，包括参数的可行检测，以及转换
            if (!ArgUtil.HandleArg(Process, env, args, ori, out object arg, out err)) {
                reader.Cursor = start;
                reply = err;
                return DispatchResult.Unmatched;
            }

            // 如果有参数名，则将该值赋予参数
            if (!string.IsNullOrEmpty(ParamName) && arg != null) {
                args[ParamName] = arg;
            }

            DispatchResult childResult = DispatchResult.Unmatched;
            // 判断映射参数部分是否还未开始
            if (reader.HasNext) {
                start = reader.Cursor;
                childResult = DispatchResult.MatchedAll;
                // 优先匹配子节点，执行子节点的逻辑
                foreach (CommandNode<TEnv> child in GetRevelentNodes(reader)) {
                    // 执行子节点的调度
                    childResult = child.Dispatch(reader, env, args.Derives(), out reply);
                    if (childResult == DispatchResult.MatchedAll) {
                        // 如果某个子节点全部匹配则完美收官，返回结果
                        return childResult;
                    } else if (childResult == DispatchResult.MatchedSelf) {
                        // 如果某个子节点只匹配了自身，则结束遍历，但是暂不返回结果
                        err = reply;
                        reader.Cursor = start;
                        break;
                    }
                    // 如果某个子节点也根本不匹配，则继续遍历下一个节点
                    reader.Cursor = start;
                }
            }
            // 执行到这里，说明子节点没有一个是完美匹配的
            // 如果自身节点本来有执行逻辑的话，则忽略后续参数，执行自身节点的执行逻辑，并判定为参数匹配成功
            if (Executor != null) {
                // 解析映射参数
                Args dict = new Args();
                if (Mapper != null) {
                    reader.SkipTo(ArgSep);
                    Mapper.Parse(reader, dict, out string dictErr);
                }
                reply = Executor(env, args, dict);
                return DispatchResult.MatchedAll;
            } else if (childResult == DispatchResult.MatchedSelf) {
                reply = err;
                return childResult;
            }
            // 到这里说明自己没有执行逻辑，而子节点也一个都没有匹配成功，则视为只匹配了自身
            reader.SkipWhiteSpace();
            reply = new StringBuilder()
                .AppendLine("期待：" + string.Join("，", GetTips()))
                .Append("得到：" + (reader.HasNext ? reader.PeekRest() : "<空>"))
                .ToString();
            return DispatchResult.MatchedSelf;
        }


        private CommandNode<TEnv>[] GetRevelentNodes(StringReader reader) {
            int start = reader.Cursor;
            reader.SkipWhiteSpace();
            string literal = reader.HasNext ? reader.ReadToWhiteSpaceOr(ArgSep) : null;
            reader.Cursor = start;
            if (!string.IsNullOrEmpty(literal) && certainChildren.TryGetValue(literal, out CommandNode<TEnv> node)) {
                return new CommandNode<TEnv>[] { node };
            } else {
                return children.ToArray();
            }
        }

        public void AddChild(CommandNode<TEnv> node) {
            string[] certain = node.Parser.Certain;
            if (certain != null && certain.Length > 0) {
                foreach (string c in certain) {
                    certainChildren[c] = node;
                }
            } else {
                children.Add(node);
            }
        }

        private HashSet<CommandNode<TEnv>> GetAllChildren() {
            HashSet<CommandNode<TEnv>> ac = new HashSet<CommandNode<TEnv>>(certainChildren.Values);
            ac.UnionWith(children);
            return ac;
        }

        public CommandNode<TEnv> Then(CommandNode<TEnv> node) {
            AddChild(node);
            return this;
        }

        public CommandNode<TEnv> Next(CommandNode<TEnv> node) {
            AddChild(node);
            return node;
        }

        public CommandNode<TEnv> Rest(CommandNode<TEnv> node) {
            node.Spread = true;
            AddChild(node);
            return this;
        }

        public CommandNode<TEnv> Executes(CmdExecutor<TEnv> executor) {
            Executor = executor;
            return this;
        }

        public CommandNode<TEnv> Handles(PreHandler<TEnv> process) {
            Process = process;
            return this;
        }

        public CommandNode<TEnv> MapDict(DictMapper mapper) {
            Mapper = mapper;
            return this;
        }

        public string[] GetTips() {
            HashSet<CommandNode<TEnv>> allChildren = GetAllChildren();
            HashSet<string> tips = new HashSet<string>();
            foreach (CommandNode<TEnv> child in allChildren) {
                tips.Add(string.IsNullOrEmpty(child.ParamName) ? child.Parser.Tip : child.ParamName);
            }
            return tips.ToArray();
        }

        /// <summary>
        /// 获取帮助字符串
        /// </summary>
        /// <returns></returns>
        public List<string> GetHelp() {
            List<string> ret = new List<string>();
            string self = Help;
            if (Executor != null) {
                ret.Add(self);
            }
            foreach (CommandNode<TEnv> child in GetAllChildren()) {
                List<string> childHelp = child.GetHelp();
                foreach (string s in childHelp) {
                    ret.Add(self + " " + s);
                }
            }
            return ret;
        }

        /// <summary>
        /// 获取帮助信息
        /// </summary>
        public virtual string Help => 
            string.IsNullOrEmpty(ParamName) ? 
            (Parser.IsLiteral ? Parser.Tip : $"<{Parser.Tip}>") :
            $"<{ParamName}:{Parser.Tip}>";
    }
}
