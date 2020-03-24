using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Bot;
using top.riverelder.arkham.Code.Model;

namespace top.riverelder.arkham.Code
{
    public static class Global
    {
        public static string RelDataDir = "data";
        public static string RelConfFile = "game.ini";

        public static string AppDir = "/";
        public static string DataDir = AppDir + RelDataDir;
        public static string ConfFile = AppDir + RelConfFile;

        /// <summary>
        /// 召唤骰娘的前缀
        /// </summary>
        public static string Prefix = "/";
        /// <summary>
        /// 默认的人物属性
        /// </summary>
        static public ValueSet DefaultValues = new ValueSet();
        /// <summary>
        /// 大成功值，若小等于则判定为大成功
        /// </summary>
        static public int GreatSuccess { get; set; } = 5;
        /// <summary>
        /// 大失败值，若大等于则判定为大失败
        /// </summary>
        static public int GreatFailure { get; set; } = 95;
        /// <summary>
        /// 群ID到模组名的映射
        /// </summary>
        public static IDictionary<long, string> Groups = new Dictionary<long, string>();
        /// <summary>
        /// 模组
        /// </summary>
        public static IDictionary<string, Scenario> Scenarios = new Dictionary<string, Scenario>();
        /// <summary>
        /// 命令调度器
        /// </summary>
        public static CommandDispatcher Dispatcher = new CommandDispatcher();

        public static Scenario GetScenario(string name)
        {
            if (Scenarios.TryGetValue(name, out Scenario scenario))
            {
                return scenario;
            }
            scenario = new Scenario(name);
            Scenarios[name] = scenario;
            return scenario;
        }
    }
}
