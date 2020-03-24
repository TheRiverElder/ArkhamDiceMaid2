using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code.Bot.ParamValidator
{
    public interface IParam
    {
        /// <summary>
        /// 对目标进行校验，并解析为自定义类型的结果，并返回错误
        /// </summary>
        /// <param name="raw">原始参数字符串</param>
        /// <param name="result">输出结果</param>
        /// <param name="err">输出错误</param>
        /// <returns>是否允许该参数</returns>
        bool Validate(string raw, out object result, out int length, out string err);
    }
}
