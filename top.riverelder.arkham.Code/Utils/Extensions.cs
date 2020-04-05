using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Model;
using top.riverelder.RiverCommand;

namespace top.riverelder.arkham.Code.Utils {
    public static class Extensions {

        public static CommandNode<TEnv> Dice<TEnv>(string name) {
            return new CommandNode<TEnv>(name, new DiceParser());
        }

        public static CommandNode<TEnv> Value<TEnv>(string name) {
            return new CommandNode<TEnv>(name, new ValueParser());
        }




        public static PreProcess<DMEnv> ExistSce() {
            return (DMEnv env, Args args, object ori, out object arg, out string err) => {
                arg = null;
                if (!env.TryGetSce(out Scenario sce)) {
                    err = "还未开团";
                    return false;
                }
                err = null;
                return true;
            };
        }

        public static PreProcess<DMEnv> ExistSelfInv() {
            return (DMEnv env, Args args, object ori, out object arg, out string err) => {
                arg = null;
                if (!env.TryGetInv(out Scenario sce, out Investigator inv)) {
                    if (sce == null) {
                        err = "还未开团";
                        return false;
                    }
                    err = "你还未车卡";
                    return false;
                }
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
                if (!env.TryGetSce(out Scenario sce)) {
                    err = "还未开团";
                    return false;
                }
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
                if (!env.TryGetInv(out Scenario sce, out Investigator inv)) {
                    if (sce == null) {
                        err = "还未开团";
                        return false;
                    }
                    err = "你还未车卡";
                    return false;
                }
                string valueName = (string)ori;
                if (!inv.Values.TryWidelyGet(valueName, out Value value)) {
                    err = $"未找到{inv.Name}的{valueName}";
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
                if (!env.TryGetInv(out Scenario sce, out Investigator inv)) {
                    if (sce == null) {
                        err = "还未开团";
                        return false;
                    }
                    err = "你还未车卡";
                    return false;
                }
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
