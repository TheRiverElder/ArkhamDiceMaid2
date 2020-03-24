using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code.Model
{
    /// <summary>
    /// 值集合，用于管理值
    /// </summary>
    public class ValueSet
    {
        public Dictionary<string, Value> values = new Dictionary<string, Value>();
        public Dictionary<string, string> aliases = new Dictionary<string, string>();

        [JsonIgnore]
        public ICollection<string> Names => values.Keys;

        /// <summary>
        /// 判断改名字是否有对应的值
        /// </summary>
        /// <param name="name">名字</param>
        /// <returns>判断结果</returns>
        public bool Has(string name)
        {
            if (values.ContainsKey(name))
            {
                return true;
            }
            if (!aliases.TryGetValue(name, out string alias))
            {
                return false;
            }
            return values.ContainsKey(alias);
        }

        /// <summary>
        /// 尝试获取该名字对应的值
        /// </summary>
        /// <param name="name">名字</param>
        /// <param name="val">结果</param>
        /// <returns>是否成功</returns>
        public bool TryGet(string name, out Value val)
        {
            if(values.TryGetValue(name, out val))
            {
                return true;
            }
            if (aliases.TryGetValue(name, out string alias))
            {
                return values.TryGetValue(alias, out val);
            }
            return false;
        }

        /// <summary>
        /// 尝试获取该名字对应的值，如果该调查员不存在这个属性，则在全局默认属性中查找并复制到该成员
        /// </summary>
        /// <param name="name">名字</param>
        /// <param name="val">结果</param>
        /// <returns>是否成功</returns>
        public bool TryWidelyGet(string name, out Value val) {
            if (values.TryGetValue(name, out val)) {
                return true;
            }
            if (aliases.TryGetValue(name, out string alias)) {
                return values.TryGetValue(alias, out val);
            }
            if (Global.DefaultValues.TryGet(name, out Value v)) {
                val = v.Copy();
                Put(val);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Value this[string name] {
            get => values.ContainsKey(name) ? values[name] : values[aliases[name]];
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="val">值</param>
        public void Put(Value val, params string[] als)
        {
            values[val.Name] = val;
            foreach (string alias in als)
            {
                aliases[alias] = val.Name;
            }
        }

        /// <summary>
        /// 设置别名
        /// </summary>
        /// <param name="name">主名</param>
        /// <param name="als">别名</param>
        public void Set(string name, params string[] als)
        {
            foreach (string alias in als)
            {
                aliases[alias] = name;
            }
        }

        public ValueSet Copy()
        {
            ValueSet set = new ValueSet();
            foreach (KeyValuePair<string, Value> pair in values.AsEnumerable())
            {
                set.values[pair.Key] = pair.Value.Copy();
            }
            foreach (KeyValuePair<string, string> pair in aliases.AsEnumerable())
            {
                set.aliases[pair.Key] = pair.Value;
            }
            return set;
        }

        public void Clear()
        {
            values.Clear();
            aliases.Clear();
        }

        public void Fill(ValueSet tmp)
        {
            foreach (KeyValuePair<string, Value> pair in tmp.values.AsEnumerable())
            {
                values[pair.Key] = pair.Value.Copy();
            }
            foreach (KeyValuePair<string, string> pair in tmp.aliases.AsEnumerable())
            {
                aliases[pair.Key] = pair.Value;
            }
        }
    }
}
