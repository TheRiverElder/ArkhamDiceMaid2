using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code.Model {
    /// <summary>
    /// 物品
    /// </summary>
    public class Item {

        /// <summary>
        /// 物品名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 对应技能名字
        /// </summary>
        public string SkillName { get; set; } = "斗殴";

        /// <summary>
        /// 对应技能名字
        /// </summary>
        public string Type { get; set; } = "肉搏";

        /// <summary>
        /// 造成伤害的骰子表达式
        /// </summary>
        public string Damage { get; set; } = "1d3+db";

        /// <summary>
        /// 是否能造成穿刺
        /// </summary>
        public bool Impale { get; set; } = false;

        /// <summary>
        /// 在一轮战斗中的最大使用次数
        /// </summary>
        public int MaxCount { get; set; } = 1;

        /// <summary>
        /// 弹匣容量
        /// </summary>
        public int Capacity { get; set; } = 1;

        /// <summary>
        /// 故障值值
        /// </summary>
        public int Mulfunction { get; set; } = 100;

        /// <summary>
        /// 当前装弹数
        /// </summary>
        public int CurrentLoad { get; set; } = 1;

        /// <summary>
        /// 弹药每轮消耗
        /// </summary>
        public int Cost { get; set; } = 1;

        public Item(string name) {
            Name = name;
        }
    }
}
