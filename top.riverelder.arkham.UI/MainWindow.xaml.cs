using Native.Sdk.Cqp.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using top.riverelder.arkham.Code;
using top.riverelder.arkham.Model.Code;

namespace top.riverelder.arkham.UI {

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {

        public static readonly Brush WarningBursh = new SolidColorBrush(Colors.Red);
        public static readonly Brush NoticeBursh = new SolidColorBrush(Colors.Green);
        public static readonly Brush MemberInfoBursh = new SolidColorBrush(Colors.Blue);
        public static readonly Brush MessageTextBursh = new SolidColorBrush(Colors.Blue);


        public MainWindow() {
            InitializeComponent();
            this.iptGroup.Text = Convert.ToString(Global.Groups.Keys.FirstOrDefault());
        }

        private Chat groupChat;

        public void AppendText(string text, int indent = 0) {
            AppendText(text, MessageTextBursh, indent);
        }

        public void AppendText(string text, Brush foreground, int indent = 0) {
            try {
                Dispatcher.BeginInvoke(
                    DispatcherPriority.Normal,
                    new Action(() => {
                        pnlMessageList.Children.Add(new TextBlock {
                            Text = text,
                            Margin = new Thickness(indent, 0, 0, 0),
                            Foreground = foreground,
                        });
                        if (cbxAutoScroll.IsChecked.Value) {
                            lstMessage.ScrollToBottom();
                        }
                    }));
            } catch (Exception e) {
                MessageBox.Show(e.StackTrace);
            }
        }

        public void Clear() {
            this.pnlMessageList.Children.Clear();
        }

        private void BtnGroup_Click(object sender, RoutedEventArgs e) {
            if (!long.TryParse(this.iptGroup.Text, out long groupId)) {
                return;
            }
            if (groupChat != null) {
                groupChat.OnAddMessage -= this.AddMessage;
            }
            Chat chat = Chat.Of(groupId);
            var groupInfo = Chat.Api.GetGroupInfo(groupId);
            if (groupInfo != null) {
                groupChat = chat;
                groupChat.OnAddMessage += this.AddMessage;
                Clear();
                txtGroupName.Text = $"{groupInfo.Name} ({groupInfo.CurrentMemberCount}人)";
                AppendText($"当前群：{groupInfo.Name} ({groupId})", NoticeBursh);
                foreach (var msg in chat.Messages) {
                    AddMessage(msg);
                }
            } else {
                AppendText("关联失败!", WarningBursh);
            }
        }

        private void AddMessage(Chat.Message msg) {
            AppendText($"{msg.Nick} ({msg.QQ}) {DateTime.FromBinary(msg.Time).ToShortTimeString()}", MemberInfoBursh);
            AppendText(msg.Text, MessageTextBursh, 20);
        }

        private void BtnSendMessage_Click(object sender, RoutedEventArgs e) {
            if (groupChat != null) {
                groupChat.SendMessage(iptMessage.Text);
                if (!cbxKeepText.IsChecked.Value) {
                    iptMessage.Clear();
                }
            }
        }
    }
}
