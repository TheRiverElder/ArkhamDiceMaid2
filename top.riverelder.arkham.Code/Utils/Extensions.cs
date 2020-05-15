using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Model;
using top.riverelder.RiverCommand;

namespace top.riverelder.arkham.Code.Utils {
    public static class Extensions {

        public static CommandNode<DMEnv> Dice(string name) {
            return new CommandNode<DMEnv>(name, new DiceParser());
        }

        public static CommandNode<TEnv> Value<TEnv>(string name) {
            return new CommandNode<TEnv>(name, new ValueParser());
        }




        public static PreProcess<DMEnv> ExistSce() {
            return (DMEnv env, Args args, object ori, out object arg, out string err) => {
                arg = env.Sce;
                arg = null;
                err = null;
                return true;
            };
        }

        public static PreProcess<DMEnv> ExistSelfInv() {
            return (DMEnv env, Args args, object ori, out object arg, out string err) => {
                arg = env.Inv;
                err = null;
                return true;
            };
        }

        public static PreProcess<DMEnv> ExistInv() {
            return (DMEnv env, Args args, object ori, out object arg, out string err) => {
                arg = null;
                if (ori == null || !(ori is string)) {
                    err = "参数错误";
                    return false;
                }
                Scenario sce = env.Sce;
                string invName = (string)ori;
                if (!sce.TryGetInvestigator(invName, out Investigator inv)) {
                    err = "未找到调查员：" + invName;
                    return false;
                }
                err = null;
                arg = inv;
                return true;
            };
        }

        public static PreProcess<DMEnv> GetSelfValue() {
            return (DMEnv env, Args args, object ori, out object arg, out string err) => {
                arg = null;
                if (ori == null || !(ori is string)) {
                    err = "参数错误";
                    return false;
                }
                Investigator inv = env.Inv;
                string valueName = (string)ori;
                if (!inv.Values.TryWidelyGet(valueName, out Value value)) {
                    err = $"未找到{inv.Name}的{valueName}";
                    arg = null;
                    return false;
                }
                err = null;
                arg = value;
                return true;
            };
        }

        public static PreProcess<DMEnv> ExistSelfValue() {
            return (DMEnv env, Args args, object ori, out object arg, out string err) => {
                arg = null;
                if (ori == null || !(ori is string)) {
                    err = "参数错误";
                    return false;
                }
                Investigator inv = env.Inv;
                string valueName = (string)ori;
                if (!inv.Values.TryWidelyGet(valueName, out Value value)) {
                    err = $"未找到{inv.Name}的{valueName}";
                    arg = ori;
                    return false;
                }
                err = null;
                arg = ori;
                return true;
            };
        }

        public static PreProcess<DMEnv> ConvertObjectArrayToStringArray() {
            return (DMEnv env, Args args, object ori, out object arg, out string err) => {
                arg = null;
                if (ori == null || !(ori is object[])) {
                    err = "参数错误";
                    return false;
                }
                object[] oa = (object[])ori;
                string[] sa = new string[oa.Length];
                for (int i = 0; i < oa.Length; i++) {
                    sa[i] = Convert.ToString(oa[i]);
                }
                arg = sa;
                err = null;
                return true;
            };
        }








        public static int GetInt(this Args args, string name) {
            return args.Get<int>(name);
        }

        public static bool GetBool(this Args args, string name) {
            return args.Get<bool>(name);
        }

        public static string GetStr(this Args args, string name) {
            return args.Get<string>(name);
        }

        public static Dice GetDice(this Args args, string name) {
            return args.Get<Dice>(name);
        }

        public static Investigator GetInv(this Args args, string name) {
            return args.Get<Investigator>(name);
        }

        public static Value GetVal(this Args args, string name) {
            return args.Get<Value>(name);
        }

    }
}
