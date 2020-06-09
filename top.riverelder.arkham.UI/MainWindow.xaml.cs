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
        public MainWindow() {
            InitializeComponent();
            this.iptGroup.Text = Convert.ToString(Global.Groups.Keys.FirstOrDefault());
        }

        private Chat groupChat;

        public void AppendText(string text, int indent = 0) {
            AppendText(new TextBlock() {
                Text = text,
                Margin = new Thickness(indent, 0, 0, 0),
            });
        }

        public void AppendText(TextBlock txt) {
            try {
                Dispatcher.Invoke(new Action(() => pnlMessageList.Children.Add(txt)));
            } catch (Exception e) {
                MessageBox.Show(e.Message);
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
                AppendText(new TextBlock {
                    Text = $"当前群：{groupInfo.Name} ({groupId})",
                    TextAlignment = TextAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.Green),
                });
                foreach (var msg in chat.Messages) {
                    AddMessage(msg);
                }
            } else {
                AppendText(new TextBlock {
                    Text = "关联失败!",
                    TextAlignment = TextAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.Red),
                });
            }
        }

        private void AddMessage(Chat.Message msg) {
            AppendText($"{msg.Nick} ({msg.QQ}) {msg.Time}");
            AppendText(msg.Text, 20);
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
