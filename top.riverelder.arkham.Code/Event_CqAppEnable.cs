
using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;
using System.IO;
using top.riverelder.arkham.Code;

using top.riverelder.arkham.Code.Commands;
using top.riverelder.arkham.Code.Utils;
using top.riverelder.arkham.Model.Code;

namespace top.riverelder.arkham.Code
{
    /// <summary>
	/// Type=1003 应用被启用, 事件实现
	/// </summary>
    public class Event_CqAppEnable : IAppEnable
    {
        public void AppEnable(object sender, CQAppEnableEventArgs e) {
            Global.Initialize(e.CQApi.AppDirectory);
            Chat.Api = e.CQApi;
            Chat.SelfId = e.CQApi.GetLoginQQId();
        }
    }
}
