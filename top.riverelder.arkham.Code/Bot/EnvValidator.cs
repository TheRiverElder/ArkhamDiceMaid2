using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Model;

namespace top.riverelder.arkham.Code.Bot {
    public static class EnvValidator {
        public static bool ExistValue(CmdEnv env, string name, out Value value, out string err) {
            if (!env.ScenarioExist) {
                err = "该羣还未开团";
                value = null;
                return false;
            }
            if (!env.InvestigatorExist) {
                err = "你还未车卡";
                value = null;
                return false;
            }
            Investigator inv = env.Investigator;
            if (!inv.Values.TryWidelyGet(name, out value)) {
                err = $"未找到{inv.Name}的{name}";
                value = null;
                return false;
            }
            err = null;
            return true;
        }

        public static bool ExistInv(CmdEnv env, out Investigator inv, out string err) {
            if (!env.ScenarioExist) {
                err = "该羣还未开团";
                inv = null;
                return false;
            }
            if (!env.InvestigatorExist) {
                err = "你还未车卡";
                inv = null;
                return false;
            }
            err = null;
            inv = env.Investigator;
            return true;
        }
    }
}
