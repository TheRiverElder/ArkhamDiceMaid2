using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Code.Exceptions {
    public class DiceException : Exception {
        public DiceException(string message) : base(message) {
        }
    }
}
