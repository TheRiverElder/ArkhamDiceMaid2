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
using top.riverelder.arkham.Code.Bot;

namespace top.riverelder.arkham.Code
{
    public class Event_GroupMessage : IGroupMessage
    {
        public void GroupMessage(object sender, CQGroupMessageEventArgs e)
        {
            string msg = e.Message.Text;
            if (msg.StartsWith(Global.Prefix))
            {
                string raw = msg.Substring(Global.Prefix.Length);
                string[] cmds = Regex.Split(raw, "([\n;；])" + Global.Prefix);
                StringBuilder sb = new StringBuilder().Append(CQApi.CQCode_At(e.FromQQ));
                foreach (string c in cmds) {
                    if (Global.Dispatcher.Compile(c, out CompiledCommand cc, out string err)) {
                        CmdEnv env = new CmdEnv(e.FromQQ.Id, e.FromGroup.Id, Global.Groups.TryGetValue(e.FromGroup.Id, out string name) ? name : string.Empty);
                        sb.AppendLine().Append(cc.Execute(env));
                    }
                    e.CQLog.InfoReceive("Command", c);
                }
                e.CQApi.SendGroupMessage(e.FromGroup, sb.ToString());

            }
        }
    }
}
