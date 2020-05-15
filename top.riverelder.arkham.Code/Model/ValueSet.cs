using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code.Model {
    /// <summary>
    /// 值集合，用于管理值
    /// </summary>
    public class ValueSet {
        public Dictionary<string, Value> values = new Dictionary<string, Value>();
        public Dictionary<string, string> aliases = new Dictionary<string, string>();

        [JsonIgnore]
        public ICollection<string> Names => values.Keys;

        /// <summary>
        /// 判断改名字是否有对应的值
        /// </summary>
        /// <param name="name">名字</param>
        /// <returns>判断结果</returns>
        public bool Has(string name) {
            if (values.ContainsKey(name)) {
                return true;
            }
            if (!aliases.TryGetValue(name, out string alias)) {
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
        public bool TryGet(string name, out Value val) {
            if (values.TryGetValue(name, out val)) {
                return true;
            }
            if (aliases.TryGetValue(name, out string alias)) {
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
                Put(name, val);
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// 判断一个名字是否是别名，它首先不能是真名，其次必须是别名
        /// </summary>
        /// <param name="alias">名字</param>
        /// <param name="name">真名</param>
        /// <returns>是否是别名</returns>
        public bool IsAlias(string alias, out string name) {
            if (values.ContainsKey(alias)) {
                name = alias;
                return false;
            }
            return aliases.TryGetValue(alias, out name);
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Value this[string name] => TryWidelyGet(name, out Value value) ? value : null;

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="val">值</param>
        public void Put(string name, Value val) {
            values[name] = val;
        }

        /// <summary>
        /// 设置别名，若原名不存在，则设置失败
        /// </summary>
        /// <param name="alias">别名</param>
        /// <param name="name">原名</param>
        /// <returns>是否设置成功</returns>
        public bool SetAlias(string alias, string name) {
            if (!values.ContainsKey(name)) {
                return false;
            }
            aliases[alias] = name;
            return true;
        }

        /// <summary>
        /// 移除一个数值，如果是别名，则只移除别名
        /// </summary>
        /// <param name="name">数值名</param>
        public bool Remove(string name, out bool isAlias) {
            if (aliases.Remove(name)) {
                isAlias = true;
                return true;
            } else if (values.Remove(name)) {
                isAlias = false;
                return true;
            } else {
                isAlias = false;
                return false;
            }
        }

        public ValueSet Copy() {
            ValueSet set = new ValueSet();
            foreach (KeyValuePair<string, Value> pair in values.AsEnumerable()) {
                set.values[pair.Key] = pair.Value.Copy();
            }
            foreach (KeyValuePair<string, string> pair in aliases.AsEnumerable()) {
                set.aliases[pair.Key] = pair.Value;
            }
            return set;
        }

        public void Clear() {
            values.Clear();
            aliases.Clear();
        }

        /// <summary>
        /// 填充模板中的所有属性与别名，会覆盖已有属性
        /// </summary>
        /// <param name="tmp">用来填充的模板</param>
        public void FillWith(ValueSet tmp) {
            foreach (KeyValuePair<string, Value> pair in tmp.values.AsEnumerable()) {
                values[pair.Key] = pair.Value.Copy();
            }
            foreach (KeyValuePair<string, string> pair in tmp.aliases.AsEnumerable()) {
                aliases[pair.Key] = pair.Value;
            }
        }

        /// <summary>
        /// 补充没有的属性与别名，不会覆盖已有属性
        /// </summary>
        /// <param name="tmp">用来补充的模板</param>
        public void CompleteWith(ValueSet tmp) {
            foreach (var pair in tmp.values) {
                if (!values.ContainsKey(pair.Key)) {
                    values[pair.Key] = pair.Value.Copy();
                }
            }
            foreach (var pair in tmp.aliases) {
                if (!aliases.ContainsKey(pair.Key)) {
                    aliases[pair.Key] = pair.Value;
                }
            }
        }
    }
}