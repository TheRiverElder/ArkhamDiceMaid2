using RiverCommand;
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
        public ParamParser<TEnv> Parser { get; } = null;
        public DictMapper<TEnv> Mapper { get; set; } = null;
        public bool Spread { get; set; } = false;

        protected readonly Dictionary<string, CommandNode<TEnv>> certainChildren = new Dictionary<string, CommandNode<TEnv>>();
        protected readonly List<CommandNode<TEnv>> children = new List<CommandNode<TEnv>>();

        public PreHandler<TEnv> Process { get; set; } = null;
        public CmdExecutor<TEnv> Executor { get; set; } = null;

        public CommandNode(string paramName, ParamParser<TEnv> parser) {
            ParamName = paramName;
            Parser = parser;
        }

        public CommandNode(ParamParser<TEnv> parser) {
            Parser = parser;
        }

        #region 解析相关

        /// <summary>
        /// 调度参数解析
        /// </summary>
        /// <param name="reader">原始字符流</param>
        /// <param name="env">环境</param>
        /// <param name="args">参数</param>
        /// <param name="res">解析出来的编译命令</param>
        /// <param name="err">错误</param>
        /// <returns>是否符合该节点</returns>
        public bool Dispatch(
            CmdDispatcher<TEnv> dispatcher,
            StringReader reader,
            TEnv env,
            Args args,
            int level,
            List<CompiledCommand<TEnv>> res
        ) {
            reader.Skip(Config.ListSeps);
            int start = reader.Cursor;
            // 匹配参数
            if (!MatchSelfListArg(dispatcher, reader, env, args, out string err)) {
                reader.Cursor = start;
                res.Add(MakeErr(err, level, reader.Cursor));
                return false;
            }

            // 判断映射参数部分开始
            if (ArgUtil.IsListArgEnd(reader)) {
                Args dict = new Args();
                // 若映射参数不为空，则开始解析映射参数
                if (Mapper != null && !Mapper.Parse(dispatcher, env, reader, dict, out err)) {
                    return false;
                } else if (ArgUtil.IsCommandEnd(reader)) {
                    if (Executor != null) {
                        res.Add(CompileWith(env, args, dict, level, reader.Cursor));
                        return true;
                    } else {
                        res.Add(MakeErr(
                            "该节点不应该是终结节点",
                            level,
                            reader.Cursor));
                        return false;
                    }
                } else {
                    res.Add(MakeErr(
                        "命令本应结束，却读取到未知字符：" +
                        reader.ReadToEndOrMaxOrEmpty(Config.MaxCut, Config.EmptyStrTip),
                        level,
                        reader.Cursor)
                    );
                    return false;
                }
            }

            // 开始匹配子节点
            return MatchChildren(dispatcher, reader, env, args, level + 1, res);
        }

        protected bool MatchSelfListArg(CmdDispatcher<TEnv> dispatcher, StringReader reader, TEnv env, Args args, out string err) {
            // 匹配参数
            object ori = null;
            if (Spread) { // 收集所有剩下的参数
                List<object> list = new List<object>();
                int s = reader.Cursor;
                while (reader.Skip(Config.ListSeps) && Parser.TryParse(dispatcher, env, reader, out ori)) {
                    list.Add(ori);
                    s = reader.Cursor;
                }
                reader.Cursor = s;
                if (list.Count == 0) {
                    err = "参数不匹配";
                    return false;
                }
                ori = list.ToArray();
            } else { // 仅检测一个参数
                if (!Parser.TryParse(dispatcher, env, reader, out ori)) {
                    err = "参数不匹配";
                    return false;
                }
            }

            // 预处理得到的参数，包括参数的可行检测，以及转换
            if (!ArgUtil.HandleArg(Process, env, args, ori, out object arg, out err)) {
                return false;
            }

            // 如果有参数名，则将该值赋予参数
            if (!string.IsNullOrEmpty(ParamName) && arg != null) {
                args[ParamName] = arg;
            }
            return true;
        }

        protected bool MatchChildren(CmdDispatcher<TEnv> dispatcher, StringReader reader, TEnv env, Args args, int childLevel, List<CompiledCommand<TEnv>> res) {
            if (certainChildren.Count == 0 && children.Count == 0 && !ArgUtil.IsListArgEnd(reader)) {
                res.Add(MakeErr("多余的参数：" + reader.ReadToEndOrMaxOrEmpty(Config.MaxCut, Config.EmptyStrTip), childLevel, reader.Cursor));
                return false;
            }
            
            // 开始匹配子节点
            int start = reader.Cursor;
            bool hasChildMatched = false;
            foreach (CommandNode<TEnv> child in GetRevelentNodes(reader)) {
                // 执行子节点的调度
                hasChildMatched |= child.Dispatch(dispatcher, reader, env, args.Derives(), childLevel, res);
                reader.Cursor = start;
            }
            if (!hasChildMatched) {
                res.Add(MakeErr(new StringBuilder()
                    .AppendLine("期待：" + string.Join("，", GetTips()))
                    .Append("得到：" + reader.ReadToEndOrMaxOrEmpty(Config.MaxCut, Config.EmptyStrTip))
                    .ToString(), childLevel, reader.Cursor));
            }
            return hasChildMatched;
        }

        protected CommandNode<TEnv>[] GetRevelentNodes(StringReader reader) {
            int start = reader.Cursor;
            string literal = reader.Read(ArgUtil.IsNameChar);
            reader.Cursor = start;
            if (!string.IsNullOrEmpty(literal) && certainChildren.TryGetValue(literal, out CommandNode<TEnv> node)) {
                return new CommandNode<TEnv>[] { node };
            } else {
                return children.ToArray();
            }
        }

        protected HashSet<CommandNode<TEnv>> GetAllChildren() {
            HashSet<CommandNode<TEnv>> ac = new HashSet<CommandNode<TEnv>>(certainChildren.Values);
            ac.UnionWith(children);
            return ac;
        }

        protected CompiledCommand<TEnv> CompileWith(TEnv env, Args args, Args dict, int len, int readerCursor) {
            return new CompiledCommand<TEnv>(readerCursor, len, Executor, env, args, dict);
        }

        protected CompiledCommand<TEnv> MakeErr(string err, int len, int readerCursor) {
            return new CompiledCommand<TEnv>(readerCursor, len, err);
        }

        #endregion

        #region 构造相关

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

        public CommandNode<TEnv> Executes(CmdReplyer<TEnv> replyer) {
            Executor = ArgUtil.Rep2Exe<TEnv>(replyer);
            return this;
        }

        public CommandNode<TEnv> Handles(PreHandler<TEnv> process) {
            Process = process;
            return this;
        }

        public CommandNode<TEnv> MapDict(DictMapper<TEnv> mapper) {
            Mapper = mapper;
            return this;
        }

        #endregion

        #region 提示相关

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

        #endregion
    }
}
