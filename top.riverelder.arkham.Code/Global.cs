using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Commands;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;

namespace top.riverelder.arkham.Code {
    public static class Global {
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
        /// 调试模式，打开的话，会返回每一条消息的结果
        /// </summary>
        public static bool Debug = true;
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
        public static CmdDispatcher<DMEnv> Dispatcher = new CmdDispatcher<DMEnv>();

        public static Scenario GetScenario(string name) {
            if (Scenarios.TryGetValue(name, out Scenario scenario)) {
                return scenario;
            }
            scenario = new Scenario(name);
            Scenarios[name] = scenario;
            return scenario;
        }

        public static void Initialize(string appDir) {
            AppDir = appDir;
            ConfFile = Path.Combine(appDir, RelConfFile);
            DataDir = Path.Combine(appDir, RelDataDir);

            SaveUtil.LoadGlobal();

            Dispatcher.Register(new Command_Check());
            Dispatcher.Register(new Command_Coc7());
            Dispatcher.Register(new Command_Control());
            Dispatcher.Register(new Command_Inv());
            Dispatcher.Register(new Command_Display());
            Dispatcher.Register(new Command_Fight());
            Dispatcher.Register(new Command_Heal());
            Dispatcher.Register(new Command_Help());
            //Dispatcher.Register(new Command_Horse());
            Dispatcher.Register(new Command_Item());
            Dispatcher.Register(new Command_Misc());
            Dispatcher.Register(new Command_Order());
            Dispatcher.Register(new Command_Global());
            Dispatcher.Register(new Command_Repeat());
            Dispatcher.Register(new Command_Roll());
            Dispatcher.Register(new Command_SanCheck());
            Dispatcher.Register(new Command_Scenario());
            //Dispatcher.Register(new Command_SetPrefix());
            Dispatcher.Register(new Command_Spell());
            Dispatcher.Register(new Command_Value());
        }
    }
}
