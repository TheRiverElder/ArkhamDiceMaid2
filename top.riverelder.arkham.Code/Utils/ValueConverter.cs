using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Model;

namespace top.riverelder.arkham.Code.Utils {

    class ValueConverter : JsonConverter<Value> {

        public override Value ReadJson(JsonReader reader, Type objectType, Value existingValue, bool hasExistingValue, JsonSerializer serializer) {
            string s = reader.Value.ToString();
            Value val = Value.Of(s);
            //Util.Log($"读取 {s} 解析得到 {val.ToString()}");
            return val;
        }

        public override void WriteJson(JsonWriter writer, Value value, JsonSerializer serializer) {
            if (value == null) {
                writer.WriteNull();
            } else {
                //Util.Log($"写入 {value.ToString()}");
                writer.WriteValue(value.ToString());
            }
        }
    }
}
