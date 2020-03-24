
using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;
using System.IO;
using top.riverelder.arkham.Code;
using top.riverelder.arkham.Code.Bot;
using top.riverelder.arkham.Code.Bot.ParamValidator;
using top.riverelder.arkham.Code.Commands;
using top.riverelder.arkham.Code.Utils;

namespace top.riverelder.arkham.Code
{
    /// <summary>
	/// Type=1003 应用被启用, 事件实现
	/// </summary>
    public class Event_CqAppEnable : IAppEnable
    {
        public void AppEnable(object sender, CQAppEnableEventArgs e) {
            Global.AppDir = e.CQApi.AppDirectory;
            Global.ConfFile = Path.Combine(e.CQApi.AppDirectory, Global.RelConfFile);
            Global.DataDir = Path.Combine(e.CQApi.AppDirectory, Global.RelDataDir);

            SaveUtil.LoadGlobal();

            Global.Dispatcher.Register(new Command_Check());
            Global.Dispatcher.Register(new Command_Control());
            Global.Dispatcher.Register(new Command_CreateInv());
            Global.Dispatcher.Register(new Command_CreateScenario());
            Global.Dispatcher.Register(new Command_Display());
            Global.Dispatcher.Register(new Command_Fight());
            Global.Dispatcher.Register(new Command_Help());
            Global.Dispatcher.Register(new Command_Horse());
            Global.Dispatcher.Register(new Command_Item());
            Global.Dispatcher.Register(new Command_Global());
            Global.Dispatcher.Register(new Command_ReloadScenario());
            Global.Dispatcher.Register(new Command_Roll());
            Global.Dispatcher.Register(new Command_SanCheck());
            Global.Dispatcher.Register(new Command_SaveScenario());
            Global.Dispatcher.Register(new Command_SetPrefix());
            Global.Dispatcher.Register(new Command_Value());
            
        }
    }
}
