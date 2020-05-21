using RiverCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using top.riverelder.RiverCommand.Utils;

namespace top.riverelder.RiverCommand {

    public class CmdDispatcher<TEnv> {
        
        private Dictionary<string, string> aliases = new Dictionary<string, string>();
        private readonly RootCommandNode<TEnv> Root = new RootCommandNode<TEnv>();


        public ICollection<CommandNode<TEnv>> Commands => Root.GetAllCommand();

        public CommandNode<TEnv> this[string head] {
            get {
                if (Root.TryGetCommand(head, out CommandNode<TEnv> cmd)) {
                    return cmd;
                }
                return null;
            }
        }

        public void Register(ICmdEntry<TEnv> e) {
            e.OnRegister(this);
        }

        /// <summary>
        /// 注册以字面量打头的指令
        /// </summary>
        /// <param name="head">指令头</param>
        /// <returns>注册的第一个节点</returns>
        public CommandNode<TEnv> Register(string head) {
            CommandNode<TEnv> n = PresetNodes.Literal<TEnv>(head);
            Root.AddChild(n);
            return n;
        }

        /// <summary>
        /// 加入可能以非字面量打头的指令
        /// </summary>
        /// <param name="node">指令节点</param>
        public void RegisterCustom(CommandNode<TEnv> node) {
            Root.AddChild(node);
        }
        
        /// <summary>
        /// 设置别名
        /// </summary>
        /// <param name="alias">别名</param>
        /// <param name="replacement">替换内容</param>
        public void SetAlias(string alias, string replacement) {
            aliases[alias] = replacement;
        }

        /// <summary>
        /// 调度命令，必须是完整命令，不能多，也不能少
        /// </summary>
        /// <param name="raw">原始字符串</param>
        /// <param name="env">环境</param>
        /// <param name="result">结果，可能是编译成功的指令或错误信息</param>
        /// <returns>是否调度成功</returns>
        public bool Dispatch(string raw, TEnv env, out CompiledCommand<TEnv> result) {
            raw = raw.TrimStart();
            StringReader reader = new StringReader(raw);
            if (!Dispatch(reader, env, out result)) {
                return false;
            } else if (reader.SkipWhiteSpace()) {
                result = new CompiledCommand<TEnv>(result.ReaderCursor, 0, "未能识别的部分：" + reader.ReadToEndOrMaxOrEmpty(Config.MaxCut, Config.EmptyStrTip));
                return false;
            }
            return true;
        }

        /// <summary>
        /// 调度命令，可以是命令的一部分
        /// </summary>
        /// <param name="stringReader">原始字符流</param>
        /// <param name="env">环境</param>
        /// <param name="result">调度结果</param>
        /// <returns>是否调度成功</returns>
        public bool Dispatch(StringReader stringReader, TEnv env, out CompiledCommand<TEnv> result) {
            StringReader reader = stringReader;
            // 跳过空白
            reader.Skip(Config.ListSeps);
            // 记录位置，应为之后可能会读取别名，也用于解析失败，恢复用
            int start = reader.Cursor;
            // 读取命令头，或者别名
            string alias = stringReader.Read(ArgUtil.IsNameChar);
            int offset = 0;
            // 查找替换项，并计算偏移
            if (!string.IsNullOrEmpty(alias) && aliases.TryGetValue(alias, out string replacement)) {
                reader = new StringReader(replacement + reader.ReadRest());
                offset = alias.Length - replacement.Length;
            } else {
                // 恢复原始字符流
                stringReader.Cursor = start;
                reader = new StringReader(reader.ReadRest());
            }
            // 恢复原始字符流
            stringReader.Cursor = start;
            List<CompiledCommand<TEnv>> res = new List<CompiledCommand<TEnv>>();
            // 开始调度
            bool ret = Root.Dispatch(this, reader, env, new Args(), 0, res);
            // 查找匹配成功的最长命令
            List<CompiledCommand<TEnv>> matched = new List<CompiledCommand<TEnv>>();
            List<CompiledCommand<TEnv>> errors = new List<CompiledCommand<TEnv>>();
            foreach (var cc in res) {
                (cc.IsErr ? errors : matched).Add(cc);
            }
            if (ret && matched.Count > 0) {
                result = GetLongestCmd(matched);
            } else if (!ret && errors.Count > 0) {
                // 如果没有最长成功命令，则返回最长错误
                result = GetLongestCmd(errors);
            } else {
                // 否则出错
                result = null;
                reader.Cursor = start;
                return false;
            }
            reader.Skip(result.ReaderCursor);
            // 此时reader已经不是原来的reader了，这样一来就要重新计算原始字符流的读取长度
            stringReader.Skip(reader.Cursor + offset);
            return ret;
        }

        /// <summary>
        /// 直接执行命令
        /// </summary>
        /// <param name="raw">原始字符串</param>
        /// <param name="env">环境</param>
        /// <param name="reply">回复</param>
        /// <returns>是否解析成功</returns>
        public bool Execute(string raw, TEnv env, out object ret, out string reply) {
            if (Dispatch(raw, env, out var ccmd)) {
                ret = ccmd.Execute(out reply);
                return true;
            }
            ret = null;
            reply = ccmd.ErrorStr;
            return false;
        }

        private CompiledCommand<TEnv> GetLongestCmd(List<CompiledCommand<TEnv>> list) {
            CompiledCommand<TEnv> result = list[0];
            foreach (var cc in list) {
                if (cc != result && cc.Length > result.Length) {
                    result = cc;
                }
            }
            return result;
        }
    }
}
