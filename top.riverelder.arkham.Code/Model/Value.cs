using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Utils;

namespace top.riverelder.arkham.Code.Model {
    /// <summary>
    /// 封装一个值
    /// </summary>
    public class Value {
        private static Regex reg = new Regex(@"(\d+)(/(\d+))?");
        public static Value Of(string name, string raw) {
            Match match = reg.Match(raw);
            if (!match.Success) {
                return new Value(name, 0, -1);
            }
            int val = int.Parse(match.Groups[1].Value);
            int max = match.Groups[3].Success ? int.Parse(match.Groups[3].Value) : -1;
            return new Value(name, val, max);
        }


        /// <summary>
        /// 数值的名字
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 普通数值
        /// </summary>
        public int Val = 50;

        /// <summary>
        /// 最大值，默认为-1，即没有上限
        /// </summary>
        public int Max = -1;

        /// <summary>
        /// 困难值，为普通值的1/2
        /// </summary>
        [JsonIgnore]
        public int HardVal => Val / 2;

        /// <summary>
        /// 极难值，为普通值的1/5
        /// </summary>
        [JsonIgnore]
        public int ExtremeVal => Val / 5;

        #region 构造器
        public Value(string name, int val, int max) {
            Name = name;
            Val = val;
            Max = max;
        }

        public Value(string name, int val) {
            Name = name;
            Val = val;
        }

        public Value() {
            Name = "未命名";
        }
        #endregion

        #region 数值变化
        /// <summary>
        /// 设置新值，这个设置受到根据其最大值的限制，除非Max为负数
        /// </summary>
        /// <param name="newVal">新的值</param>
        /// <returns>改变后的数值</returns>
        public int Set(int newVal) => Val = Max < 0 ? Math.Max(0, newVal) : Math.Min(Math.Max(0, newVal), Max);

        /// <summary>
        /// 增加数值，受到根据其最大值的限制，除非Max为负数
        /// </summary>
        /// <param name="delta">改变量，可以为负数</param>
        /// <returns>改变后的数值</returns>
        public int Add(int delta) => Set(Val + delta);

        /// <summary>
        /// 减少数值，受到根据其最大值的限制，除非Max为负数
        /// </summary>
        /// <param name="delta">改变量，可以为负数</param>
        /// <returns>改变后的数值</returns>
        public Value Sub(int delta) {
            Set(Val - delta);
            return this;
        }
        #endregion

        public override string ToString() => Max < 0 ? $"{Val}" : $"{Val}/{Max}";

        public Value Copy() => new Value(Name, Val, Max);


        #region 检定
        public static readonly string DefaultDice = "1d100";

        /// <summary>
        /// 以默认的骰子与默认的普通难度进行检定
        /// </summary>
        /// <returns>检定结果</returns>
        public CheckResult Check() {
            return Check(DefaultDice, CheckResult.NormalSuccess);
        }

        /// <summary>
        /// 以默认的骰子进行检定
        /// </summary>
        /// <param name="level">难度等级</param>
        /// <returns>检定结果</returns>
        public CheckResult Check(int level) {
            return Check(DefaultDice, level);
        }

        /// <summary>
        /// 使用指定的骰子进行检定
        /// </summary>
        /// <param name="diceExp">指定的骰子表达式</param>
        /// <param name="level">难度等级</param>
        /// <returns>检定结果</returns>
        public CheckResult Check(string diceExp, int level) {
            int result = Dice.Roll(diceExp);
            return new CheckResult(diceExp, this, result, level);
        }
        #endregion
    }
}
