using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Model;
using top.riverelder.RiverCommand;

namespace top.riverelder.arkham.Code.Utils {
    public static class DicePreprocesses {

        public static PreProcess<DMEnv> GetSelfValue() {
            return (DMEnv env, Args args, object ori, out object arg) => {
                Investigator inv = env.Inv;
                if (inv.Values.TryWidelyGet((string)ori, out Value v)) {
                    arg = v;
                    return true;
                } else {
                    arg = ori;
                    return false;
                }
            };
        }

    }
}
