using Native.Sdk.Cqp;
using Native.Sdk.Cqp.Enum;
using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;
using Native.Sdk.Cqp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using top.riverelder.arkham.Code.Exceptions;
using top.riverelder.arkham.Code.Model;
using top.riverelder.arkham.Model.Code;

namespace top.riverelder.arkham.Code {

    public class Event_GroupMessage : IGroupMessage {

        public void GroupMessage(object sender, CQGroupMessageEventArgs e) {
            Chat.Of(e.FromGroup.Id).AddMessage(new Chat.Message(
                e.FromGroup.GetGroupMemberInfo(e.FromQQ).Nick, 
                e.FromQQ.Id, 
                e.FromGroup.Id, 
                e.Message.Text, 
                DateTime.Now.ToBinary()
            ));
            string msg = e.Message.Text;
            if (msg.StartsWith(Global.Prefix)) {
                DMEnv env = new DMEnv(
                    e.FromQQ.Id,
                    e.FromGroup.Id,
                    e.FromGroup.GetGroupMemberInfo(e.FromQQ).MemberType == QQGroupMemberType.Manage
                    );
                if (Global.DoAt) {
                    env.Append(CQApi.CQCode_At(e.FromQQ));
                }
                if (Global.Reply(msg, env)) {
                    e.CQApi.SendGroupMessage(e.FromGroup, env.ToString().Trim());
                    e.CQLog.Info("ArkhamDiceMaid", "响应成功");
                } else {
                    e.CQLog.Info("ArkhamDiceMaid", "响应失败");
                }
            }
        }
    }
}
