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
        public Inventory Inventory { get; } = new Inventory();

        /// <summary>
        /// 角色的战斗事件
        /// </summary>
        public Queue<FightEvent> Fights = new Queue<FightEvent>();

        public Investigator(string name, string desc) {
            Name = name;
            Desc = desc;
        }

        public Investigator() {
        }

        public int Build { get; private set; } = 0;
        public string DamageBonus { get; private set; } = "0";

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

        public bool Attack(Investigator target, string weaponName, out string err) {
            if (!Inventory.TryGet(weaponName, out Item item) || !item.IsWeapon) {
                err = $"未找到武器：{weaponName}";
                return false;
            }
            WeaponInfo wi = item.Weapon;
            if (!Values.TryGet(wi.Skill.Name, out Value skill)) {
                skill = wi.Skill;
            }
            CheckResult result = skill.Check();
            err = null;
            return true;
        }
    }
}
