using RiverCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Exceptions;
using top.riverelder.arkham.Code.Model;
using top.riverelder.RiverCommand;

namespace top.riverelder.arkham.Code.Utils {
    public static class Extensions {

        #region 拓展CommandNode的工厂

        public static CommandNode<DMEnv> Dice(string name) {
            return new CommandNode<DMEnv>(name, new DiceParser());
        }

        public static CommandNode<DMEnv> Value(string name) {
            return new CommandNode<DMEnv>(name, new ValueParser());
        }

        #endregion

        /// <summary>
        /// 参数断言是否为字符串
        /// </summary>
        /// <param name="ori">原始参数</param>
        /// <param name="str">转化为的字符串</param>
        private static void AssertIsString(object ori, out string str) {
            if (ori == null || !(ori is string)) {
                throw new DiceException($"参数错误：{ori}不是字符串");
            }
            str = (string)ori;
        }

        #region 拓展PreHandler

        public static bool ExistSce(DMEnv env, Args args, object ori, out object arg, out string err) {
            arg = env.Sce;
            arg = null;
            err = null;
            return true;
        }

        public static bool ExistSelfInv(DMEnv env, Args args, object ori, out object arg, out string err) {
            arg = env.Inv;
            err = null;
            return true;
        }

        public static bool ExistInv(DMEnv env, Args args, object ori, out object arg, out string err) {
            arg = null;
            AssertIsString(ori, out string invName);
            Scenario sce = env.Sce;
            if (!sce.TryGetInvestigator(invName, out Investigator inv)) {
                err = "未找到调查员：" + invName;
                return false;
            }
            err = null;
            arg = inv;
            return true;
        }

        public static bool GetSelfValue(DMEnv env, Args args, object ori, out object arg, out string err) {
            arg = null;
            AssertIsString(ori, out string valueName);
            Investigator inv = env.Inv;
            if (!inv.Values.TryWidelyGet(valueName, out Value value)) {
                err = $"未找到{inv.Name}的{valueName}";
                arg = null;
                return false;
            }
            err = null;
            arg = value;
            return true;
        }

        public static bool ExistSelfValue(DMEnv env, Args args, object ori, out object arg, out string err) {
            arg = null;
            AssertIsString(ori, out string valueName);
            Investigator inv = env.Inv;
            if (!inv.Values.TryWidelyGet(valueName, out Value value)) {
                err = $"未找到{inv.Name}的{valueName}";
                arg = ori;
                return false;
            }
            err = null;
            arg = ori;
            return true;
        }

        #endregion




        #region 拓展Args的返回类型

        public static int GetInt(this Args args, string name) {
            return args.Get<int>(name);
        }

        public static bool GetBool(this Args args, string name) {
            return args.Get<bool>(name);
        }

        public static string GetStr(this Args args, string name) {
            return args.Get<string>(name);
        }

        public static CompiledCommand<DMEnv> GetCmd(this Args args, string name) {
            return args.Get<CompiledCommand<DMEnv>>(name);
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
        #endregion

    }
}
