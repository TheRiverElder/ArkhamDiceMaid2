using Native.Sdk.Cqp.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using top.riverelder.arkham.Model.Code;

namespace top.riverelder.arkham.UI {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private Chat groupChat;

        public void AppendText(string text) {
            TextBlock txt = new TextBlock() {
                Text = text,
            };
            this.pnlMessageList.Children.Add(txt);
        }

        private void BtnGroup_Click(object sender, RoutedEventArgs e) {
            if (!long.TryParse(this.iptGroup.Text, out long groupId)) {
                return;
            }
            if (groupChat != null) {
                groupChat.OnAddMessage -= this.AddMessage;
            }
            Label lblNotice = new Label();
            if (Chat.TryGetChat(groupId, out Chat chat)) {
                groupChat = chat;
                groupChat.OnAddMessage += this.AddMessage;
                lblNotice.Content = "关联成功！";
            } else {
                lblNotice.Content = "关联失败!";
            }
            AppendText(lblNotice.Content.ToString());
        }

        private void AddMessage(Chat.Message msg) {
            Label lblInfo = new Label() {
                Content = $"{msg.Nick} {msg.QQ} {DateTime.FromBinary(msg.Time).ToShortTimeString()}",
            };
            Label lblContent = new Label() {
                Content = $"{msg.Text}",
                Margin = new Thickness(50, 0, 0, 0),
            };
            AppendText(lblInfo.Content.ToString());
            AppendText(lblContent.Content.ToString());
        }

        private void BtnSendMessage_Click(object sender, RoutedEventArgs e) {
            if (groupChat != null) {
                groupChat.SendMessage(iptMessage.Text);
            }
        }

        private void LstMessageList_SelectionChanged(object sender, SelectionChangedEventArgs e) {

        }
    }
}
