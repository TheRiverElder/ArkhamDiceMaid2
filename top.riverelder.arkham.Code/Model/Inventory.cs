using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code.Model
{
    /// <summary>
    /// 物品栏
    /// </summary>
    public class Inventory
    {
        public IDictionary<string, Item> items = new Dictionary<string, Item>();

        /// <summary>
        /// 判断改名字是否有对应的物品
        /// </summary>
        /// <param name="name">名字</param>
        /// <returns>判断结果</returns>
        public bool Has(string name) => items.ContainsKey(name);

        /// <summary>
        /// 尝试获取该名字对应的物品
        /// </summary>
        /// <param name="name">名字</param>
        /// <param name="val">结果</param>
        /// <returns>是否成功</returns>
        public bool TryGet(string name, out Item item) => items.TryGetValue(name, out item);

        /// <summary>
        /// 获取物品
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Item this[string name] => items[name];

        /// <summary>
        /// 增加物品
        /// </summary>
        /// <param name="vals">一系列物品</param>
        public void Put(params Item[] its)
        {
            foreach (Item item in its)
            {
                items[item.Name] = item;
            }
        }

        /// <summary>
        /// 失去物品
        /// </summary>
        /// <param name="vals">物品名</param>
        public Item Remove(string name)
        {
            if (items.TryGetValue(name, out Item item))
            {
                items.Remove(name);
                return item;
            }
            return null;
        }

        /// <summary>
        /// 获取所有的物品
        /// </summary>
        [JsonIgnore]
        public ICollection<Item> Items => items.Values;

        public int Count => items.Count;
    }
}
