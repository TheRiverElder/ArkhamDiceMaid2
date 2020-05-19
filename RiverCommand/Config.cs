using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiverCommand {
    public static class Config {

        public static string Head = "/";
        public static HashSet<char> ListSeps = new HashSet<char>(" \t");
        public static HashSet<char> DictSeps = new HashSet<char>("\r\n;,；，");
        public static HashSet<char> Linkers = new HashSet<char>(":：=");

        public static int MaxCut = 5;
        public static string EmptyStrTip = "<空>";

    }
}
