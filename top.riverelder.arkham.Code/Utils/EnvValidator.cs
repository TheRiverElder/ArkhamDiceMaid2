using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Bot;
using top.riverelder.arkham.Code.Model;

namespace top.riverelder.arkham.Code.Utils {
    public static class EnvValidator {

        public static bool ExistValue(CmdEnv env, string name, out Value value, out string err) {
            if (env.InvestigatorExist) {
                Investigator inv = env.Investigator;
                if (inv.Values.TryGet(name, out value)) {
                    err = null;
                    return true;
                }
                value = null;
                err = "未找到属性：" + name;
                return false;
            }
            value = null;
            err = "还未开团或还未车卡";
            return false;
        }

        public static bool ExistInv(CmdEnv env, out Investigator inv, out string err) {
            if (env.InvestigatorExist) {
                inv = env.Investigator;
                err = null;
                return false;
            }
            inv = null;
            err = "还未开团或还未车卡";
            return false;
        }

    }
}
