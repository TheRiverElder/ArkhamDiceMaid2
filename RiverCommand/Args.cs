using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.RiverCommand {
    public class Args {

        public Args Parent { get; private set; } = null;

        private readonly Dictionary<string, object> map = new Dictionary<string, object>();

        public Args(Args parent) {
            Parent = parent;
        }

        public Args() {
        }

        public void Put(string key, object value) {
            map[key] = value;
        }

        public bool TryGet<TValue>(string key, out TValue value) {
            if (map.TryGetValue(key, out object v)) {
                if (v is TValue) {
                    value = (TValue)v;
                    return true;
                }
            } else if (Parent != null) {
                return Parent.TryGet(key, out value);
            }
            value = default(TValue);
            return false;
        }

        public TValue Get<TValue>(string key) {
            if (map.TryGetValue(key, out object v)) {
                if (v is TValue) {
                    return (TValue)v;
                }
            } else if (Parent != null) {
                if (Parent.TryGet(key, out TValue value)) {
                    return value;
                }
            }
            return default(TValue);
        }

        public object this[string key] {
            get => TryGet(key, out object value) ? value : null;
            set => map[key] = value;
        }

        public Dictionary<string, object> Dict => new Dictionary<string, object>(map);

        public Args Derives() => new Args(this);
    }
}
