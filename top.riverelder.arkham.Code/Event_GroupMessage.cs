using Native.Sdk.Cqp;
using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;
using Native.Sdk.Cqp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Model;

namespace top.riverelder.arkham.Code {

    public class Event_GroupMessage : IGroupMessage {

        public void GroupMessage(object sender, CQGroupMessageEventArgs e) {
            string msg = e.Message.Text;
            if (msg.StartsWith(Global.Prefix)) {
                string raw = msg.Substring(Global.Prefix.Length);
                string[] cmds = Regex.Split(raw, @"[ \t\n\r；;]+" + Global.Prefix);
                StringBuilder sb = new StringBuilder().Append(CQApi.CQCode_At(e.FromQQ));
                bool flag = false;
                foreach (string c in cmds) {
                    e.CQLog.InfoReceive("DiceCommand", c);
                    if (Global.Dispatcher.Dispatch(c, new DMEnv(), out string reply) || Global.Debug) {
                        //CmdEnv env = new CmdEnv(e.FromQQ.Id, e.FromGroup.Id, Global.Groups.TryGetValue(e.FromGroup.Id, out string name) ? name : string.Empty);
                        sb.AppendLine().Append(reply);
                        flag = true;
                    }
                }
                if (flag) {
                    e.CQApi.SendGroupMessage(e.FromGroup, sb.ToString());
                }
            }
        }
    }
}
