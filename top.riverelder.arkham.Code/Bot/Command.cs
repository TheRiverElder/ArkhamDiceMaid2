using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code.Bot {
    public class Command {

        public string Name { get; }

        public CommandNode Root { get; }

        public Dictionary<string, string> Aliases { get; }
    }
}
