using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code.Model
{
    /// <summary>
    /// 武器属性
    /// </summary>
    public class WeaponInfo
    {

        /// <summary>
        /// 对应技能名字
        /// </summary>
        public string SkillName { get; set; } = "肉搏";

        /// <summary>
        /// 对应技能数值
        /// </summary>
        public int SkillValue { get; set; } = 25;

        /// <summary>
        /// 造成伤害的骰子表达式
        /// </summary>
        public string Damage { get; set; } = "1d1";

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
        /// 故障值
        /// </summary>
        public int Mulfunction { get; set; } = 100;

        /// <summary>
        /// 当前装弹数
        /// </summary>
        public int CurrentLoad { get; set; } = 1;

        public WeaponInfo(string skillName, int skillValue, string damage, bool impale, int maxCount, int capacity, int mulfunction, int currentLoad) {
            SkillName = skillName;
            SkillValue = skillValue;
            Damage = damage;
            Impale = impale;
            MaxCount = maxCount;
            Capacity = capacity;
            Mulfunction = mulfunction;
            CurrentLoad = currentLoad;
        }

        public WeaponInfo()
        {
        }
    }
}
