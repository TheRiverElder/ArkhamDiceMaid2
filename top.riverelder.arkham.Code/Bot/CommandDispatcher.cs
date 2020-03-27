using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code.Bot {
    public class CommandDispatcher {
        // 空行
        static private readonly Regex EmptyLine = new Regex(@"^\s*$");
        // 分割行
        static private readonly Regex Divider = new Regex("[;；\n]+");
        // 分割命令名称与线性参数
        static private readonly Regex Splitter = new Regex(@"\s+");

        private readonly IDictionary<string, ICommand> commands = new Dictionary<string, ICommand>();
        private readonly IDictionary<string, string> aliases = new Dictionary<string, string>();

        /// <summary>
        /// 所有注册的命令
        /// </summary>
        public ICollection<ICommand> Commands => commands.Values;

        /// <summary>
        /// 所有注册的命令头
        /// </summary>
        public ICollection<string> Heads => commands.Keys;

        public ICommand this[string head] {
            get => commands[head];
        }

        /// <summary>
        /// 注册命令
        /// </summary>
        /// <param name="command">要注册的命令</param>
        public void Register(ICommand command) {
            commands[command.Name] = command;
        }

        /// <summary>
        /// 设置别名
        /// </summary>
        /// <param name="alias">别名</param>
        /// <param name="leadingCommand">将别名替换为的字符串</param>
        public void AddAlias(string alias, string leadingCommand) {
            aliases[alias] = leadingCommand;
        }

        /// <summary>
        /// 解析命令行
        /// </summary>
        /// <param name="originalString">原始字符串</param>
        /// <param name="senderToken">发送者token</param>
        /// <param name="result">解析的结果</param>
        /// <returns>解析是否成功</returns>
        public bool Compile(string originalString, out CompiledCommand result, out string err) {
            string[] lines = Divider.Split(originalString.Trim());
            string[] headLine = Splitter.Split(lines[0].Trim());
            string commandName = headLine[0];
            if (!commands.TryGetValue(commandName, out ICommand command)) {
                if (!aliases.TryGetValue(commandName, out string leadingCommand)) {
                    result = null;
                    err = "不存在命令：" + commandName;
                    return false;
                }
                headLine = Splitter.Split(leadingCommand + lines[0].Substring(headLine[0].Length));
                commandName = headLine[0];
                if (!commands.TryGetValue(commandName, out command)) {
                    result = null;
                    err = "不存在别名对应的命令：" + commandName;
                    return false;
                }
            }

            // 解析线性参数
            string[] listArgs = new string[headLine.Length - 1];
            Array.Copy(headLine, 1, listArgs, 0, headLine.Length - 1);

            // 解析映射参数
            Dictionary<string, string> dictArgs = new Dictionary<string, string>();
            for (int i = 1; i < lines.Length; i++) {
                string line = lines[i];
                if (EmptyLine.IsMatch(line)) {
                    continue;
                }
                int index = line.IndexOf(':');
                if (index < 0) {
                    index = line.IndexOf('：');
                }
                if (index < 0) {
                    result = null;
                    err = "不合规范的映射表达式：" + line;
                    return false;
                }
                string key = line.Substring(0, index).Trim();
                string value = line.Substring(index + 1).Trim();
                dictArgs.Add(key, value);
            }

            if (!command.Validater.Match(listArgs, dictArgs, out err)) {
                result = null;
                return false;
            }

            CompiledCommand compiledCommand = new CompiledCommand(command, listArgs, dictArgs, originalString);

            result = compiledCommand;
            err = null;
            return true;
        }
    }
}
