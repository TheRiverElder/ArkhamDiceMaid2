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
            if (Global.Sleep) {
                return;
            }
            Chat chat = Chat.Of(e.FromGroup.Id);
            string msg = e.Message.Text;
            if (msg.StartsWith(Global.Prefix)) {
                chat.AddMessage(e.Message.Text, e.FromQQ.Id, Chat.Message.Command);
                QQGroupMemberType memberType = e.FromGroup.GetGroupMemberInfo(e.FromQQ).MemberType;
                DMEnv env = new DMEnv(
                    e.FromQQ.Id,
                    e.FromGroup.Id,
                    memberType == QQGroupMemberType.Manage || memberType == QQGroupMemberType.Creator
                    );
                if (Global.DoAt) {
                    env.Append(CQApi.CQCode_At(e.FromQQ));
                }
                if (Global.Reply(msg, env)) {
                    string reply = env.ToString().Trim();
                    chat.SendMessage(reply, Chat.Message.Reply);
                    e.CQLog.Info("ArkhamDiceMaid", "响应成功");
                } else {
                    e.CQLog.Info("ArkhamDiceMaid", "响应失败");
                }
            } else {
                List<CQCode> codes = e.Message.CQCodes;
                
                if (codes?.Count > 0) {
                    long qq = e.FromQQ.Id;
                    foreach (CQCode code in codes) {
                        switch (code.Function) {
                            case CQFunction.Image: chat.AddMessage(e.CQApi.ReceiveImage(code), qq, Chat.Message.Image); break;
                        }
                    }
                } else {
                    chat.AddMessage(e.Message.Text, e.FromQQ.Id);
                }
            }

        }
    }
}
