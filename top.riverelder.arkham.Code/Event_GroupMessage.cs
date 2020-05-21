﻿using Native.Sdk.Cqp;
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

namespace top.riverelder.arkham.Code {

    public class Event_GroupMessage : IGroupMessage {

        public void GroupMessage(object sender, CQGroupMessageEventArgs e) {
            string msg = e.Message.Text;
            if (msg.StartsWith(Global.Prefix)) {
                DMEnv env = new DMEnv(
                    e.FromQQ.Id,
                    e.FromGroup.Id,
                    e.FromGroup.GetGroupMemberInfo(e.FromQQ).MemberType == QQGroupMemberType.Manage
                    );
                StringBuilder sb = new StringBuilder();
                if (Global.DoAt) {
                    sb.Append(CQApi.CQCode_At(e.FromQQ));
                }
                if (Global.Reply(msg, env, sb)) {
                    e.CQApi.SendGroupMessage(e.FromGroup, sb.ToString().Trim());
                }
            }
        }
    }
}
