
using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;
using top.riverelder.arkham.Code;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Code.Utils;

namespace top.riverelder.arkham.Code
{
    /// <summary>
	/// Type=1002 酷Q退出, 事件实现
	/// </summary>
    public class Event_CqExit : ICQExit
    {

        public void CQExit(object sender, CQExitEventArgs e)
        {
            SaveUtil.SaveGlobal();
            foreach (Scenario s in Global.Scenarios.Values)
            {
                SaveUtil.Save(s);
            }
        }
    }
}
