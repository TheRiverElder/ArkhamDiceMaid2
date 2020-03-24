using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code.Bot {
    public static class Env {
        public static Func<CmdEnv, bool> InvExist = env => env.InvestigatorExist;
        public static Func<CmdEnv, bool> SceExist = env => env.ScenarioExist;
    }
}
