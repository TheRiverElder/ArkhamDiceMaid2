using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.RiverCommand.Parsing {
    public interface ICmdResult {

        /// <summary>
        /// 该结果所解析的输入命令字符串长度
        /// </summary>
        int MatchedLength { get; }

        /// <summary>
        /// 获取匹配的参数长度
        /// 注：仅包括当前层级的线性参数长度
        /// </summary>
        int ArgLength { get; }

        /// <summary>
        /// 判断这是个结果是一个错误信息还是一个打包好的分析结果
        /// </summary>
        bool IsError { get; }

        /// <summary>
        /// 如果是错误信息，则返回本身代表的错误信息
        /// </summary>
        string Error { get; }

        /// <summary>
        /// 如果是错误信息则返回自身以及错误链
        /// </summary>
        string[] Errors { get; }

        /// <summary>
        /// 如果是打包好的结果，则可以直接进行执行
        /// </summary>
        /// <returns></returns>
        object Execute();

    }
}
