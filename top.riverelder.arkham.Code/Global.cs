﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Commands;
using top.riverelder.arkham.Code.Exceptions;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.RiverCommand;
using top.riverelder.RiverCommand.Parsing;

namespace top.riverelder.arkham.Code {
    public static class Global {
        public static string RelDataDir = "data";
        public static string RelConfFile = "game.ini";
        public static string RelLogFile = "log.txt";

        public static string AppDir = "/";
        public static string DataDir = AppDir + RelDataDir;
        public static string ConfFile = AppDir + RelConfFile;
        public static string LogFile = AppDir + RelLogFile;

        public static void SetAppDir(string appDir) {
            AppDir = appDir;
            ConfFile = Path.Combine(appDir, RelConfFile);
            DataDir = Path.Combine(appDir, RelDataDir);
            LogFile = Path.Combine(appDir, RelLogFile);
        }

        public static void Log(params string[] content) {
            try {
                File.WriteAllLines(LogFile, content);
            } catch { }
        }




        /// <summary>
        /// 睡眠，相当于在群里关闭骰娘，但是小窗消息还是会有响应
        /// </summary>
        public static bool Sleep = false;

        /// <summary>
        /// 翻译腔，这并不会被保存
        /// </summary>
        public static bool TranslatorTone = false;

        /// <summary>
        /// 召唤骰娘的前缀
        /// </summary>
        public static string Prefix = "/";


        /// <summary>
        /// 是否允许灌铅
        /// </summary>
        public static bool AllowedLead = false;

        /// <summary>
        /// 骰子灌铅，50为正常值，最小为1，最大为100
        /// </summary>
        public static int Lead = 50;
        public static int SetLead(int lead) => Lead = AllowedLead ? Math.Max(1, Math.Min(lead, 100)) : 50;

        /// <summary>
        /// 自动读团
        /// </summary>
        public static bool AutoLoad = true;
        /// <summary>
        /// 每个消息都会At发信人
        /// </summary>
        public static bool DoAt = true;
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
        /// 用户ID到群ID的映射
        /// </summary>
        public static IDictionary<long, long> Users = new Dictionary<long, long>();
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
            SetAppDir(appDir);

            SaveUtil.LoadGlobal();

            Dispatcher.Register(new Command_Check());
            Dispatcher.Register(new Command_Coc7());
            Dispatcher.Register(new Command_Control());
            Dispatcher.Register(new Command_If());
            Dispatcher.Register(new Command_Inv());
            Dispatcher.Register(new Command_Display());
            Dispatcher.Register(new Command_Fight());
            Dispatcher.Register(new Command_Heal());
            Dispatcher.Register(new Command_Help());
            Dispatcher.Register(new Command_Horse());
            Dispatcher.Register(new Command_Item());
            Dispatcher.Register(new Command_Misc());
            Dispatcher.Register(new Command_Order());
            Dispatcher.Register(new Command_Grow());
            Dispatcher.Register(new Command_Global());
            Dispatcher.Register(new Command_Repeat());
            Dispatcher.Register(new Command_Roll());
            Dispatcher.Register(new Command_SanCheck());
            Dispatcher.Register(new Command_Scenario());
            //Dispatcher.Register(new Command_SetPrefix());
            Dispatcher.Register(new Command_Spell());
            Dispatcher.Register(new Command_Status());
            Dispatcher.Register(new Command_Value());

            Dispatcher.Register(new Command_Custom());
        }

        /// <summary>
        /// 回复内容
        /// </summary>
        /// <param name="msg">接受的信息</param>
        /// <param name="env">环境</param>
        /// <returns>是否有信息输出</returns>
        public static bool Reply(string msg, DMEnv env) {
            string raw = msg.Substring(Prefix.Length);
            string[] cmds = Regex.Split(raw, @"[ \t\n\r；;]+" + Prefix);
            bool flag = false;
            foreach (string c in cmds) {
                try {
                    if (Sleep && !Regex.IsMatch(c, @"^全局\s+")) {
                        break;
                    }
                    Dispatcher.Dispatch(c, env, out ICmdResult result);
                    if (!result.IsError) {
                        env.Line();
                        result.Execute();
                        flag = true;
                    } else if (Debug) {
                        env.LineAppend(result.ToString());
                        flag = true;
                    }
                } catch (DiceException ex) {
                    env.LineAppend(ex.Message);
                    flag = true;
                }
            }
            return flag;
        }
    }
}