using RiverCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.RiverCommand.ParamParsers;

namespace top.riverelder.RiverCommand.Utils {
    public static class ArgUtil {
        
        public static HashSet<char> DictSeps = new HashSet<char>("\r\n,，;；");
        public static HashSet<char> Linkers = new HashSet<char>(":：=");
        public static StringParser DefaultParser = new StringParser();

        public static bool TryParseDictArg(
            StringReader reader,
            ParamParser parser,
            out string name, 
            out object arg
        ) {
            // 设值默认返回结果
            name = null;
            arg = null;
            // 跳过空白与分隔符
            reader.Read(DictSeps);
            if (!reader.HasNext) {
                return false;
            }
            // 解析名字
            name = reader.Read(IsNameChar);
            reader.SkipWhiteSpace();
            if (!reader.HasNext || !Linkers.Contains(reader.Peek()) || string.IsNullOrEmpty(name)) {
                name = null;
                return false;
            }
            reader.Read(Linkers);
            if (!reader.HasNext) {
                name = null;
                return false;
            }
            // 正式分析参数值
            return (parser ?? DefaultParser).TryParse(reader, out arg);
        }

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
            return !reader.Skip(Config.ListSeps) || Config.CmdPrefix.Contains(reader.Peek());
        }

        public static bool IsNameChar(char ch) {
            return ch == '_' || char.IsLetter(ch) || char.IsDigit(ch);
        }
        
    }
}
