﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using top.riverelder.arkham.Code;
using top.riverelder.arkham.Model.Code;
using top.riverelder.arkham.UI.Model;

namespace top.riverelder.arkham.UI {

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {

        public static readonly Brush WarningBursh = new SolidColorBrush(Colors.Red);
        public static readonly Brush NoticeBursh = new SolidColorBrush(Colors.Green);
        public static readonly Brush MemberInfoBursh = new SolidColorBrush(Colors.Blue);
        public static readonly Brush NormalTextBursh = new SolidColorBrush(Colors.Black);
        public static readonly Brush CommandTextBursh = new SolidColorBrush(Colors.DarkCyan);
        public static readonly Brush ReplyTextBursh = new SolidColorBrush(Colors.DarkBlue);


        public MainWindow() {
            InitializeComponent();
            this.txtGroup.Text = Convert.ToString(Global.Groups.Keys.FirstOrDefault());
        }

        private Chat groupChat;

        public void AppendText(string text, int indent = 0) {
            AppendText(text, NormalTextBursh, indent);
        }

        public void AppendText(string text, Brush foreground, int indent = 0) {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
                pnlMessageList.Children.Add(new TextBlock {
                    Text = text,
                    Margin = new Thickness(indent, 0, 0, 0),
                    Foreground = foreground,
                });
                if (cbxAutoScroll.IsChecked.Value) {
                    lstMessage.ScrollToBottom();
                }
            }));
        }

        private WrapPanel panel = null;

        public void AppendSub(int indent = 0) {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
                if (panel != null) {
                    panel.Margin = new Thickness(indent, 0, 0, 0);
                    pnlMessageList.Children.Add(panel);
                    panel = null;
                    if (cbxAutoScroll.IsChecked.Value) {
                        lstMessage.ScrollToBottom();
                    }
                }
            }));
        }

        public void AppendSubImage(string link) {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
                if (panel == null) {
                    panel = new WrapPanel();
                }
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(link);
                bi.EndInit();
                panel.Children.Add(new Image() {
                    MaxHeight = 150,
                    Source = bi,
                });
            }));
        }

        public void Clear() {
            this.pnlMessageList.Children.Clear();
        }

        private void BtnGroup_Click(object sender, RoutedEventArgs e) {
            if (!long.TryParse(this.txtGroup.Text, out long groupId)) {
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
                txtGroupName.Text = $"{groupInfo.Name} ({groupInfo.CurrentMemberCount}人 - {groupId})";
                AppendText($"当前群：{groupInfo.Name} ({groupId})", NoticeBursh);
                foreach (var msg in chat.Messages) {
                    AddMessage(msg);
                }
                if (lstGroup.Visibility == Visibility.Visible) {
                    HideGroupSelections();
                }
            } else {
                AppendText("关联失败!", WarningBursh);
            }
        }

        private void AddMessage(Chat.Message msg) {
            AppendText($"{msg.Nick} ({msg.QQ}) {DateTime.FromBinary(msg.Time).ToShortTimeString()}", MemberInfoBursh);
            switch (msg.Type) {
                case Chat.Message.Normal: AppendText(msg.Text, NormalTextBursh, 20); break;
                case Chat.Message.Command: AppendText(msg.Text, CommandTextBursh, 20); break;
                case Chat.Message.Reply: AppendText(msg.Text, ReplyTextBursh, 20); break;
                case Chat.Message.Image: AppendSubImage(msg.Text); AppendSub(20) ; break;
            }
        }

        private void BtnSendMessage_Click(object sender, RoutedEventArgs e) {
            if (groupChat != null) {
                groupChat.SendMessage(iptMessage.Text);
                if (!cbxKeepText.IsChecked.Value) {
                    iptMessage.Clear();
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e) {
            if (groupChat != null) {
                groupChat.OnAddMessage -= this.AddMessage;
            }
        }

        private void ShowGroupSelections() {
            List<Group> data = new List<Group>();
            foreach (var g in Global.Groups) {
                data.Add(new Group(g.Key, Chat.Api.GetGroupInfo(g.Key).Name, g.Value ?? "<无团>"));
            }
            Binding binding = new Binding {
                Source = data
            };
            lstGroup.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            lstGroup.Visibility = Visibility.Visible;
        }

        private void HideGroupSelections() {
            lstGroup.Visibility = Visibility.Collapsed;
        }

        private void LstGroup_LostFocus(object sender, RoutedEventArgs e) {
            HideGroupSelections();
        }

        private void TxtGroup_GotFocus(object sender, RoutedEventArgs e) {
            ShowGroupSelections();
        }

        private void LstGroup_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            Group group = lstGroup.SelectedItem as Group;
            if (group is null) {
                return;
            }
            txtGroup.Text = Convert.ToString(group.Id);
            HideGroupSelections();
        }
    }
}
