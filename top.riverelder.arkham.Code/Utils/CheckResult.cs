using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code.Utils
{

    public class CheckResult
    {
        /// <summary>
        /// 位置或出错
        /// </summary>
        public const int Unkonwn = 0;
        /// <summary>
        /// 大成功
        /// </summary>
        public const int GreatSuccess = 1;
        /// <summary>
        /// 成功
        /// </summary>
        public const int Success = 2;
        /// <summary>
        /// 失败
        /// </summary>
        public const int Failure = 3;
        /// <summary>
        /// 大失败
        /// </summary>
        public const int GreatFailure = 4;

        /// <summary>
        /// 获取结果类型所对应的字符串表达
        /// </summary>
        /// <param name="type">结果类型</param>
        /// <returns>对应的字符串表达</returns>
        public static string GetTypeString(int type)
        {
            switch (type)
            {
                case Unkonwn: return "未知";
                case GreatSuccess: return "大成功";
                case Success: return "成功";
                case Failure: return "失败";
                case GreatFailure: return "大失败";
                default: return "错误";
            }
        }


        public readonly string dice;
        public readonly int value;
        public readonly int result;
        public readonly int type;

        /// <summary>
        /// 是否算是成功，包括成功与大成功
        /// </summary>
        public bool Succeed { get { return type == GreatSuccess || type == Success; } }
        /// <summary>
        /// 是否算是失败，包括失败与大失败
        /// </summary>
        public bool Failed { get { return type == GreatFailure || type == Failure; } }

        public CheckResult(string dice, int value, int result, int type)
        {
            this.dice = dice;
            this.value = value;
            this.result = result;
            this.type = type;
        }
    }
}
