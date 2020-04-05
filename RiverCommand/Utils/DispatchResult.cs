using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.RiverCommand.Utils {
    public enum DispatchResult {
        /// <summary>
        /// 仅仅匹配成功自身节点
        /// </summary>
        MatchedSelf,

        /// <summary>
        /// 匹配了自身以及所有后续节点
        /// </summary>
        MatchedAll,

        /// <summary>
        /// 就连自身节点都没匹配
        /// </summary>
        Unmatched,
    }
}
