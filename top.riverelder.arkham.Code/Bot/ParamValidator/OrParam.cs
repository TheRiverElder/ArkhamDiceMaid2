using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code.Bot.ParamValidator {
    class OrParam : IParam {

        private readonly string[] ids;

        public OrParam(params string[] ids) {
            this.ids = ids;
        }

        public bool Validate(string raw, out object result, out int length, out string err) {
            foreach (string id in ids) {
                if (raw.StartsWith(id)) {
                    result = id;
                    length = id.Length;
                    err = null;
                    return true;
                }
            }
            result = null;
            length = 0;
            err = "参数必须是以下之一：" + string.Join("|", ids);
            return false;
        }
    }
}
