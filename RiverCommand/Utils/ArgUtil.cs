using RiverCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.RiverCommand.ParamParsers;
using top.riverelder.RiverCommand.Parsing;

namespace top.riverelder.RiverCommand.Utils {
    public static class ArgUtil {
        
        public static HashSet<char> DictSeps = new HashSet<char>("\r\n,，;；");
        public static HashSet<char> Linkers = new HashSet<char>(":：=");
        

        public static bool HandleArg<TEnv>(PreHandler<TEnv> handler, TEnv env, Args args, object ori, out object arg, out string err) {
            if (handler != null) {
                try {
                    bool res = handler(env, args, ori, out arg, out err);
                    return res;
                } catch (Exception e) {
                    arg = null;
                    err = e.Message;
                    return false;
                }
            }
            arg = ori;
            err = null;
            return true;
        }

        public static bool IsListArgEnd(StringReader reader) {
            return IsCommandEnd(reader) || Config.DictSeps.Contains(reader.Peek());
        }

        public static bool IsCommandEnd(StringReader reader) {
            return !reader.Skip(Config.ListSeps) || 
                Config.CmdPrefix.Contains(reader.Peek()) || 
                Config.CloseParen.Contains(reader.Peek());
        }

        public static bool IsNameChar(char ch) {
            return ch == '_' || char.IsLetter(ch) || char.IsDigit(ch);
        }
        
        public static CmdExecutor<TEnv> Void2Null<TEnv>(VoidCmdExecutor<TEnv> vce) {
            return (TEnv env, Args args, Args dict) => {
                vce(env, args, dict);
                return null;
            };
        }
    }
}
