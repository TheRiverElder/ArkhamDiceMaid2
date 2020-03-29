using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Model;

namespace top.riverelder.arkham.Code.Utils {

    public class CheckResult {
        private static Dictionary<int, string> typeStrings = new Dictionary<int, string> {
            [Unkonwn] = "未知",
            [GreatSuccess] = "大成功",
            [ExtremeSuccess] = "极难成功",
            [HardSuccess] = "困难成功",
            [NormalSuccess] = "成功",
            [Failure] = "失败",
            [GreatFailure] = "大失败",
        };

        /// <summary>
        /// 位置或出错
        /// </summary>
        public const int Unkonwn = 0;
        /// <summary>
        /// 大成功
        /// </summary>
        public const int GreatSuccess = 1;
        /// <summary>
        /// 极难成功
        /// </summary>
        public const int ExtremeSuccess = 2;
        /// <summary>
        /// 困难成功
        /// </summary>
        public const int HardSuccess = 3;
        /// <summary>
        /// 普通成功
        /// </summary>
        public const int NormalSuccess = 4;
        /// <summary>
        /// 失败
        /// </summary>
        public const int Failure = 5;
        /// <summary>
        /// 大失败
        /// </summary>
        public const int GreatFailure = 6;

        /// <summary>
        /// 获取结果类型所对应的字符串表达
        /// </summary>
        /// <returns>对应的字符串表达</returns>
        public string TypeString => typeStrings[type];

        /// <summary>
        /// 获取结果类型的精确字符串表达
        /// </summary>
        /// <returns>对应的字符串表达</returns>
        public string ActualTypeString {
            get {
                if (succeed) {
                    return typeStrings[type];
                }
                return type == GreatFailure ? typeStrings[GreatFailure] : typeStrings[Failure];
            }
        }


        public readonly string dice; //骰子表达式
        public readonly int value; //检定的普通数值，一直是普通难度
        public readonly int target; //检定的目标数值，根据难度而改变
        public readonly int result; //检定结果
        public readonly int type; //结果类型
        public readonly int level; //检定的难度等级
        public readonly bool succeed; //是否检定成功

        /// <summary>
        /// 判定是某种指定类型的成功
        /// </summary>
        /// <param name="successType"></param>
        /// <returns></returns>
        public bool Is(int successType) {
            switch (successType) {
                case ExtremeSuccess: return type >= GreatSuccess && type <= ExtremeSuccess;
                case HardSuccess: return type >= GreatSuccess && type <= HardSuccess;
                case NormalSuccess: return type >= GreatSuccess && type <= NormalSuccess;
                default: return false;
            }
        }

        public CheckResult(string dice, Value value, int result, int level) {
            this.dice = dice;
            this.value = value.Val;
            switch (level) {
                case NormalSuccess: target = value.Val; break;
                case HardSuccess: target = value.HardVal; break;
                case ExtremeSuccess: target = value.ExtremeVal; break;
                default: target = value.Val; break;
            }
            this.result = result;
            this.level = level;
            if (result <= Global.GreatSuccess) {
                type = GreatSuccess;
                succeed = true;
            } else if (result <= value.ExtremeVal) {
                type = ExtremeSuccess;
                succeed = level == NormalSuccess || level == HardSuccess || level == ExtremeSuccess;
            } else if (result <= value.HardVal) {
                type = HardSuccess;
                succeed = level == NormalSuccess || level == HardSuccess;
            } else if (result <= value.Val) {
                type = NormalSuccess;
                succeed = level == NormalSuccess;
            } else if (result <= Global.GreatFailure) {
                type = Failure;
                succeed = false;
            } else {
                type = GreatFailure;
                succeed = false;
            }
        }
    }
}
