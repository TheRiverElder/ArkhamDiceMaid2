using top.riverelder.arkham.Code;

namespace top.riverelder.arkham.Code.Utils
{
    public class CheckUtil
    {

        public static readonly string DefaultDice = "1d100";

        /// <summary>
        /// 以默认的骰子进行检定
        /// </summary>
        /// <param name="value">检定参数</param>
        /// <returns>检定结果</returns>
        public static CheckResult Check(int value)
        {
            return Check(DefaultDice, value);
        }

        /// <summary>
        /// 使用指定的骰子进行检定
        /// </summary>
        /// <param name="diceExp">指定的骰子表达式</param>
        /// <param name="value">检定参数</param>
        /// <returns>检定结果</returns>
        public static CheckResult Check(string diceExp, int value)
        {
            int result = Dice.Roll(diceExp);
            int type = CheckResult.Unkonwn;
            if (result <= Global.GreatSuccess)
            {
                type = CheckResult.GreatSuccess;
            }
            else if (result <= value)
            {
                type = CheckResult.Success;
            }
            else if (result <= Global.GreatFailure)
            {
                type = CheckResult.Failure;
            }
            else
            {
                type = CheckResult.GreatFailure;
            }

            return new CheckResult(diceExp, value, result, type);
        }

    }
}
