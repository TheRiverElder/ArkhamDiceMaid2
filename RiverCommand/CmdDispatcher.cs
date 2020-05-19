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

        public bool Dispatch(string raw, TEnv env, out CommandNode<TEnv> ccmd) {
            raw = raw.TrimStart();
            StringReader reader = new StringReader(raw);
            reader.SkipWhiteSpace();
            string alias = reader.Read(ArgUtil.IsNameChar);
            if (aliases.TryGetValue(alias, out string act)) {
                reader = new StringReader(act + " " + reader.ReadRest());
            }
            reader.Cursor = 0;
            List<CompiledCommand<TEnv>> res = new List<CompiledCommand<TEnv>>();
            if (commands.TryGetValue(head, out CommandNode<TEnv> node)) {
                if (node.Dispatch(reader, env, new Args(), 1, res)) {
                    reply = rp;
                    return true;
                } else {
                    err = rp;
                }
            }
            foreach (var n in customActions) {
                if (n.Dispatch(reader, env, new Args(), out reply)) {
                    return true;
                }
            }
            reply = err;
            return false;

        }
    }
}
