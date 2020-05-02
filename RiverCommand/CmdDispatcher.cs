using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using top.riverelder.RiverCommand.Utils;

namespace top.riverelder.RiverCommand {

    public class CmdDispatcher<TEnv> {

        private List<CommandNode<TEnv>> customActions = new List<CommandNode<TEnv>>();
        private Dictionary<string, string> aliases = new Dictionary<string, string>();
        private Dictionary<string, CommandNode<TEnv>> commands = new Dictionary<string, CommandNode<TEnv>>();

        public Dictionary<string, CommandNode<TEnv>> CommandMap => new Dictionary<string, CommandNode<TEnv>>(commands);


        public ICollection<CommandNode<TEnv>> Commands => commands.Values;

        public CommandNode<TEnv> this[string head] {
            get {
                if (commands.TryGetValue(head, out CommandNode<TEnv> cmd)) {
                    return cmd;
                }
                return null;
            }
        }

        public void Register(ICmdEntry<TEnv> e) {
            e.OnRegister(this);
        }

        public CommandNode<TEnv> Register(string head) {
            CommandNode<TEnv> n = PresetNodes.Literal<TEnv>(head);
            commands[head] = n;
            return n;
        }

        public void RegisterCustom(CommandNode<TEnv> node) {
            customActions.Add(node);
        }

        public void SetAlias(string alias, string replacement) {
            aliases[alias] = replacement;
        }

        public bool Dispatch(string raw, TEnv env, out string reply) {
            raw = raw.TrimStart();
            StringReader reader = new StringReader(raw);
            reader.SkipWhiteSpace();
            string alias = reader.ReadToWhiteSpace();
            if (aliases.TryGetValue(alias, out string act)) {
                reader = new StringReader(act + reader.ReadRest());
            }
            reader.Cursor = 0;
            string head = reader.ReadToWhiteSpace();
            reader.Cursor = 0;
            string err = null;
            try {
                if (commands.TryGetValue(head, out CommandNode<TEnv> node)) {
                    if (node.Dispatch(reader, env, new Args(), out string rp) == DispatchResult.MatchedAll) {
                        reply = rp;
                        return true;
                    } else {
                        err = rp;
                    }
                }
                foreach (var n in customActions) {
                    if (n.Dispatch(reader, env, new Args(), out reply) == DispatchResult.MatchedAll) {
                        return true;
                    }
                }
                reply = err;
                return false;
            } catch (Exception e) {
                reply = e.Message;
                return false;
            }
        }
    }
}
