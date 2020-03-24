using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code.Bot.ParamValidator {
    class LitParam : IParam {
        private string literial;

        public LitParam(string literial) {
            this.literial = literial;
        }

        public bool Validate(string raw, out object result, out int length, out string err) {
            if (raw.StartsWith(literial)) {
                result = literial;
                length = literial.Length;
                err = null;
                return true;
            }
            result = null;
            length = 0;
            err = "参数必须是：" + literial;
            return false;
        }
    }
}
