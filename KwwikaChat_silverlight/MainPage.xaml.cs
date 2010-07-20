using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Collections.ObjectModel;

using Kwwika;
using Kwwika.Logging;
using Kwwika.Examples.Chat;

namespace KwwikaChat
{
    public partial class MainPage : UserControl, IChatView
    {
        IChatController _controller;
        private ObservableCollection<ChatMessage> _chatMessages = new ObservableCollection<ChatMessage>();

        public MainPage()
        {
            InitializeComponent();

            SetUpEventListeners();

            ChatMessagesDataGrid.ItemsSource = _chatMessages;            

            Service.ConnectionCreated += new ConnectionCreatedEventHandler(kwwika_ConnectionCreated);
            Service.Connect(ChatConfig.ApiKey, ChatConfig.Domain);
        }

        void kwwika_ConnectionCreated(ConnectionCreatedEventArgs e)
        {
            _controller = new ChatController(this, e.Connection, ChatConfig.ChatTopic, new Logger(DebugTextBox));
        }

        private void SetUpEventListeners()
        {
            ChatMessagePublishButton.Click += new RoutedEventHandler(ChatMessagePublishButton_Click);
            NameMessageSelectButton.Click += new RoutedEventHandler(NameMessageSelectButton_Click);
        }

        void NameMessageSelectButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text.Trim();
            if (name.Length > 2)
            {
                _controller.Username = name;

                UserNameLabel.Content = name;

                ChatLayout.Visibility = System.Windows.Visibility.Visible;
                NameEntryPanel.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("Please provide a username with of at least 3 characters");
            }
        }

        void ChatMessagePublishButton_Click(object sender, RoutedEventArgs e)
        {
            string text = ChatEntryTextBox.Text.Trim();
            if(text.Length > 1)
            {
                _controller.SendChatMessage(text, DateTime.UtcNow);                
            }
        }

        #region IChatView Members

        public void MessageSent()
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                ChatEntryTextBox.Text = "";
            });
        }

        public void MessageSendingFailed(string reason)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                MessageBox.Show("Error: " + reason);
            });
        }

        public void MessageReceived(ChatMessage message)
        {
            this.Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        _chatMessages.Add(message);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                });
        }

        public void FailedToEstablishChatChannel(string reason)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                MessageBox.Show("Error: " + reason);
            }), null);
        }

        #endregion
    }
}
