using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.RiverCommand.ParamParsers;
using top.riverelder.RiverCommand.Utils;

namespace RiverCommand.Utils {
    public class ArgReader {

        public static HashSet<char> CmdEnd = new HashSet<char>("/");
        public static HashSet<char> ArgSep = new HashSet<char>(" \t\n,，;；");

        private StringReader Reader;

        public ArgReader(StringReader reader) {
            Reader = reader;
        }

        //public bool TryRead(ParamParser parser) {

        //}
    }
}
