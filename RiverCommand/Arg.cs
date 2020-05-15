using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiverCommand {
    public class Arg {

        public string Name;
        public object Value;

        public Arg(string name, object value) {
            Name = name;
            Value = value;
        }
    }
}
