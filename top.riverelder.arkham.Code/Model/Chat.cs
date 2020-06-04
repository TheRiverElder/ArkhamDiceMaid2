using Native.Sdk.Cqp;
using Native.Sdk.Cqp.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace top.riverelder.arkham.Model.Code {

    public delegate void OnAddMessageHandler(Chat.Message msg);

    public class Chat {

        public class Message {
            public string Nick { get; }
            public long QQ { get; }
            public long GroupId { get; }
            public string Text { get; }
            public long Time { get; }

            public Message(string nick, long qQ, long groupId, string text, long time) {
                Nick = nick;
                QQ = qQ;
                GroupId = groupId;
                Text = text;
                Time = time;
            }
        }

        public static readonly int MaxChatCacheCount = 100;

        private static Dictionary<long, Chat> chats = new Dictionary<long, Chat>();

        public static Chat Of(long groupId, CQApi api) {
            if (chats.TryGetValue(groupId, out Chat chat)) {
                return chat;
            }
            chat = new Chat(groupId, api);
            chats[groupId] = chat;
            return chat;
        }

        public static bool TryGetChat(long groupId, out Chat chat) {
            return chats.TryGetValue(groupId, out chat);
        }


        public long GroupId { get; }
        public CQApi Api { get; }
        public List<Message> Messages = new List<Message>();
        public event OnAddMessageHandler OnAddMessage;

        public Chat(long groupId, CQApi api) {
            GroupId = groupId;
            Api = api;
        }

        public void AddMessage(Message msg) {
            if (Messages.Count > 0 && Messages.Count >= MaxChatCacheCount) {
                Messages.RemoveAt(0);
            }
            Messages.Add(msg);
            OnAddMessage?.Invoke(msg);
        }

        public void SendMessage(string text) {
            Api.SendGroupMessage(GroupId, text);
            AddMessage(new Message(
                Api.GetGroupMemberInfo(GroupId, Api.GetLoginQQId()).Nick,
                Api.GetLoginQQId(),
                GroupId,
                text,
                DateTime.Now.ToBinary()
            ));
        }
    }
}
