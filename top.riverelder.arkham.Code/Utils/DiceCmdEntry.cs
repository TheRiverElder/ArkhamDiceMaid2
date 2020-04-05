using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Model;
using top.riverelder.RiverCommand;

namespace top.riverelder.arkham.Code.Utils {
    public abstract class DiceCmdEntry : ICmdEntry<DiceMaidEnv> {
        public static string KeySelfInv = "INV";

        public abstract void OnRegister(CmdDispatcher<DiceMaidEnv> dispatcher);
    }
}
