using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.RiverCommand;

namespace top.riverelder.RiverCommand {
    public static class PreProcesses {

        public static PreProcess<TEnv> Mapper<TEnv>(IDictionary<object, object> map) {
            return (TEnv env, Args args, object ori, out object arg) => map.TryGetValue(ori, out arg);
        }

    }
}
