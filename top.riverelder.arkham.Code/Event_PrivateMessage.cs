using Native.Sdk.Cqp;
using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Model;

namespace top.riverelder.arkham.Code {
    public class Event_PrivateMessage : IPrivateMessage {


        public void PrivateMessage(object sender, CQPrivateMessageEventArgs e) {
            string msg = e.Message.Text;
            if (msg.StartsWith(Global.Prefix)) {
                DMEnv env = new DMEnv(
                    e.FromQQ.Id,
                    Global.Users.TryGetValue(e.FromQQ.Id, out long groupId) ? groupId : 0,
                    false
                );
                StringBuilder sb = new StringBuilder();
                if (Global.DoAt) {
                    sb.Append(CQApi.CQCode_At(e.FromQQ));
                }
                if (Global.Reply(msg, env, sb)) {
                    e.CQApi.SendPrivateMessage(e.FromQQ, sb.ToString().Trim());
                }
            }
        }
    }
}
