using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.RiverCommand;
using top.riverelder.RiverCommand.Parsing;

namespace top.riverelder.RiverCommand {
    public static class PreProcesses {

        /// <summary>
        /// 对线性参数进行映射
        /// </summary>
        /// <typeparam name="TEnv">环境</typeparam>
        /// <param name="map">映射表</param>
        /// <returns>映射过程</returns>
        public static PreHandler<TEnv> MapArg<TEnv>(IDictionary<object, object> map) {
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

        /// <summary>
        /// 将object[]转化为string[]，一般用于Rest参数的转化
        /// </summary>
        public static bool ConvertObjectArrayToStringArray<TEnv>(
            TEnv env, Args args, object ori, out object arg, out string err
        ) {
            arg = null;
            if (ori == null || !(ori is object[])) {
                err = "参数错误";
                return false;
            }
            object[] oa = (object[])ori;
            string[] sa = new string[oa.Length];
            for (int i = 0; i < oa.Length; i++) {
                sa[i] = Convert.ToString(oa[i]);
            }
            arg = sa;
            err = null;
            return true;
        }
    }
}
