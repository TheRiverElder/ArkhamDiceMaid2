using Native.Tool.IniConfig.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Utils;

namespace top.riverelder.arkham.Code.Model {
    /// <summary>
    /// 调查员
    /// </summary>
    public class Investigator {
        /// <summary>
        /// 调查员的姓名没请务必取地简单明了，不带难打的或不可见的字符
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述，包括年龄、居住地、背景等
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 数值，包括属性、技能、生命等
        /// </summary>
        public ValueSet Values { get; } = new ValueSet();

        /// <summary>
        /// 物品栏
        /// </summary>
        public Dictionary<string, Item> Inventory { get; } = new Dictionary<string, Item>();

        /// <summary>
        /// 角色的战斗事件
        /// </summary>
        public Queue<FightEvent> Fights = new Queue<FightEvent>();

        /// <summary>
        /// 角色学会的法术
        /// </summary>
        public HashSet<string> Spells = new HashSet<string>();

        /// <summary>
        /// 该人物的标签
        /// </summary>
        public HashSet<string> Tags = new HashSet<string>();

        public Investigator(string name, string desc) {
            Name = name;
            Desc = desc;
        }

        public Investigator() {
        }

        public int Build { get; private set; } = 0;
        public string DamageBonus { get; set; } = "0";

        /// <summary>
        /// 自动计算体格、伤害加值
        /// </summary>
        public bool Calc(out string err) {
            if (!Values.TryGet("力量", out Value str) || !Values.TryGet("体型", out Value siz)) {
                err = "未找到力量或体型";
                return false;
            }
            int n = str.Val + siz.Val;
            if (n <= 64) { Build = -2; DamageBonus = "-2"; } else if (n <= 84) { Build = -1; DamageBonus = "-1"; } else if (n <= 124) { Build = 0; DamageBonus = "0"; } else if (n <= 164) { Build = 1; DamageBonus = "1d4"; } else if (n <= 204) { Build = 2; DamageBonus = "1d6"; } else {
                int b = (int)Math.Ceiling((n - 204) / 80.0);
                Build = 2 + b;
                DamageBonus = b + "d6";
            }
            err = null;
            return true;
        }

        /// <summary>
        /// 判断该调查员是否带了某标签
        /// </summary>
        /// <param name="tag">标签</param>
        /// <returns>检查结果</returns>
        public bool Is(string tag) {
            return Tags.Contains(tag.ToUpper());
        }

        /// <summary>
        /// 检定数值，最主要的功能是方便隐藏HIDE_VALUE数值
        /// </summary>
        /// <param name="valueName">数值名</param>
        /// <param name="hardness">难度</param>
        /// <param name="result">检定结果</param>
        /// <param name="str">字符串化</param>
        /// <returns>是否存在该数值</returns>
        public bool Check(string valueName, int hardness, out CheckResult result, out string str) {
            if (!Values.TryWidelyGet(valueName, out Value value)) {
                result = null;
                str = $"未找到{Name}的【{valueName}】";
                return false;
            }
            result = value.Check(hardness);
            string valueStr = Is("HIDE_VALUE") ? "???" : $"{result.target}/{result.value}";
            str = $"{Name}的{result.level}{valueName}：({valueStr}) => {result.points}，{result.ActualTypeString}";
            return true;
        }

        public bool Check(string valueName, out CheckResult result, out string str) {
            return Check(valueName, CheckResult.NormalSuccess, out result, out str);
        }

        public string Change(string valueName, int delta) {
            if (!Values.TryWidelyGet(valueName, out Value value)) {
                Values.Put(valueName, value = new Value(0));
            }
            string deltaStr = delta < 0 ? Convert.ToString(delta) : "+" + delta;
            return $"{Name}的{valueName}：" + (
                Is("HIDE_VALUE") ?
                $"??? {deltaStr} => ???" :
                $"{value.Val} {deltaStr} => {value.Add(delta)}");
        }
    }
}
