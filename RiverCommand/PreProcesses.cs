using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.RiverCommand;

namespace top.riverelder.RiverCommand {
    public static class PreProcesses {

        public static PreProcess<TEnv> Mapper<TEnv>(IDictionary<object, object> map) {
            return (TEnv env, Args args, object ori, out object arg, out string err) => {
                if (map.TryGetValue(ori, out arg)) {
                    err = null;
                    return true;
                } else {
                    err = "不合法的输入：" + ori;
                    return false;
                }
            };
        }

    }
}
