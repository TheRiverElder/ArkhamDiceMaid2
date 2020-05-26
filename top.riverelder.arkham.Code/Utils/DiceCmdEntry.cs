using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Model;
using top.riverelder.RiverCommand;
using top.riverelder.RiverCommand.Parsing;

namespace top.riverelder.arkham.Code.Utils {
    public abstract class DiceCmdEntry : ICmdEntry<DMEnv> {

        public abstract void OnRegister(CmdDispatcher<DMEnv> dispatcher);
    }
}
