using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace top.riverelder.RiverCommand.Utils {
    public class NumberType {


        public static readonly NumberType NonPositiveInt = new NumberType(@"^(0|-\d+)", "非正整数");
        public static readonly NumberType NegativeInt = new NumberType(@"^-0*[1-9]\d*", "负整数");
        public static readonly NumberType Int = new NumberType(@"^[+-]?\d+", "整数");
        public static readonly NumberType PositiveInt = new NumberType(@"^\+?0*[1-9]\d*", "正整数");
        public static readonly NumberType NonNegativeInt = new NumberType(@"^\+?\d+", "非负整数");




        public readonly Regex Reg;
        public readonly string Str;

        public NumberType(string reg, string str) {
            Reg = new Regex(reg);
            Str = str;
        }

        public NumberType(Regex reg, string str) {
            Reg = reg;
            Str = str;
        }
    }
}
