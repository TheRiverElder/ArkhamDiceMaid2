using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.RiverCommand.Utils;

namespace top.riverelder.RiverCommand.ParamParsers {
    public abstract class ParamParser {

        /// <summary>
        /// 如果出现该字符串则能确定调用此参数的节点
        /// </summary>
        public virtual string[] Certain { get; } = null;

        /// <summary>
        /// 该参数的类型提示信息
        /// </summary>
        public virtual string Tip { get; } = null;

        /// <summary>
        /// 解析自身节点的参数
        /// </summary>
        /// <param name="reader">字符流</param>
        /// <param name="result">解析结果</param>
        /// <returns>是否解析成功</returns>
        protected abstract bool Parse(StringReader reader, out object result);

        /// <summary>
        /// 尝试解析自身节点参数
        /// </summary>
        /// <param name="reader">字符流</param>
        /// <param name="result">解析结果</param>
        /// <returns>是否解析成功</returns>
        public bool TryParse(StringReader reader, out object result) {
            int start = reader.Cursor;
            if (Parse(reader, out result)) {
                return true;
            } else {
                reader.Cursor = start;
                return false;
            }
        }
    }
}
