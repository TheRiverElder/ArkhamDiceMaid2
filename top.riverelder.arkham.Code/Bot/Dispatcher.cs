using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code.Bot {

    public class Dispatcher {

        private Dictionary<string, string> aliases = new Dictionary<string, string>();
        private Dictionary<string, CommandNode> commands = new Dictionary<string, CommandNode>();

        public Dispatcher Register(Command cmd) {
            foreach (var p in cmd.Aliases) {
                aliases[p.Key] = p.Value;
            }
            commands[cmd.Name] = cmd.Root;
            return this;
        }

        public bool Dispatch(string raw, CmdEnv env, out string reply) {
            Match m = Regex.Match(raw, @"\s+");
            int index = m.Success ? m.Index : raw.Length;
            string alias = raw.Substring(0, index);
            if (aliases.TryGetValue(alias, out string act)) {
                raw = act + raw.Substring(index);
            }
            m = Regex.Match(raw, @"\s+");
            index = m.Success ? m.Index : raw.Length;
            string head = raw.Substring(0, index);
            string body = raw.Substring(index + (m.Success ? m.Value.Length : 0));
            if (commands.TryGetValue(head, out CommandNode node)) {
                return node.Dispatch(body, env, new Dictionary<string, object>(), out reply);
            } else {
                reply = "未找到指令";
                return false;
            }
        }
    }
}
