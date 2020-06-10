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

        public static CQApi Api { get; set; }
        public static long SelfId { get; set; }
        

        public class Message {

            public const int
                Notice = 1,
                Warning = 2,
                Normal = 3,
                Command = 4,
                Reply = 5,
                Image = 6,
                Record = 7;

            public string Nick { get; }
            public long QQ { get; }
            public string Text { get; }
            public long Time { get; }
            public int Type { get; }

            public Message(string nick, long qQ, string text, long time, int type) {
                Nick = nick;
                QQ = qQ;
                Text = text;
                Time = time;
                Type = type;
            }
        }

        public static readonly int MaxChatCacheCount = 100;

        private static Dictionary<long, Chat> chats = new Dictionary<long, Chat>();

        public static Chat Of(long groupId) {
            if (chats.TryGetValue(groupId, out Chat chat)) {
                return chat;
            }
            chat = new Chat(groupId);
            chats[groupId] = chat;
            return chat;
        }

        public static bool TryGetChat(long groupId, out Chat chat) {
            return chats.TryGetValue(groupId, out chat);
        }


        public long GroupId { get; }
        public List<Message> Messages = new List<Message>();
        public event OnAddMessageHandler OnAddMessage;

        public Chat(long groupId) {
            GroupId = groupId;
        }

        public void AddMessage(string text, long qq, int type = Message.Normal) {
            AddMessage(new Message(Api.GetGroupMemberInfo(GroupId, qq).Nick, qq, text,DateTime.Now.ToBinary(), type));
        }

        public void AddMessage(Message msg) {
            if (Messages.Count > 0 && Messages.Count >= MaxChatCacheCount) {
                Messages.RemoveAt(0);
            }
            Messages.Add(msg);
            OnAddMessage?.Invoke(msg);
        }

        public void SendMessage(string text, int type = Message.Normal) {
            Api.SendGroupMessage(GroupId, text);
            AddMessage(new Message(
                Api.GetGroupMemberInfo(GroupId, Api.GetLoginQQId()).Nick,
                Api.GetLoginQQId(),
                text,
                DateTime.Now.ToBinary(),
                type
            ));
        }
    }
}
