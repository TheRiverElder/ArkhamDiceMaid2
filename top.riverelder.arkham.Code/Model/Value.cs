using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code.Model
{
    /// <summary>
    /// 封装一个值
    /// </summary>
    public class Value
    {
        private static Regex reg = new Regex(@"(\d+)(/(\d+))?");
        public static Value Of(string name, string raw)
        {
            Match match = reg.Match(raw);
            if (!match.Success)
            {
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

        public Value(string name, int val, int max)
        {
            Name = name;
            Val = val;
            Max = max;
        }

        public Value(string name, int val)
        {
            Name = name;
            Val = val;
        }

        public Value()
        {
            Name = "未命名";
        }

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

        public override string ToString() => Max < 0 ? $"{Val}" : $"{Val}/{Max}";

        public Value Copy() => new Value(Name, Val, Max);
    }
}
