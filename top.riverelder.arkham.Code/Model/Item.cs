using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code.Model
{
    /// <summary>
    /// 物品
    /// </summary>
    public class Item
    {
        /// <summary>
        /// 物品名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 物品的武器属性，可为null，若为null则该物品不是武器
        /// </summary>
        public WeaponInfo Weapon { get; set; }

        /// <summary>
        /// 判断该物品是否为武器
        /// </summary>
        [JsonIgnore]
        public bool IsWeapon => Weapon != null;

        [JsonConstructor]
        public Item(string name, WeaponInfo weapon)
        {
            Name = name;
            Weapon = weapon;
        }

        public Item()
        {
        }

        public Item(string name) : this(name, null) { }

    }
}
