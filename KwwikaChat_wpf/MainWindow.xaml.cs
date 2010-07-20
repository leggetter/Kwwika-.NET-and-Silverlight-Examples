using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Diagnostics;

using Kwwika;
using Kwwika.Examples.Chat;

namespace KwwikaChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IChatView
    {
        private IChatController _controller;
        private ObservableCollection<ChatMessage> _chatMessages = new ObservableCollection<ChatMessage>();

        public MainWindow()
        {
            InitializeComponent();

            SetUpEventListeners();

            ChatMessagesDataGrid.ItemsSource = _chatMessages;

            IConnection connection = Service.Connect(ChatConfig.ApiKey, ChatConfig.Domain);

            _controller = new ChatController(this, connection, ChatConfig.ChatTopic, new Logger(DebugTextBox));
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
        }

        void ChatMessagePublishButton_Click(object sender, RoutedEventArgs e)
        {
            string text = ChatEntryTextBox.Text.Trim();
            if (text.Length > 1)
            {
                _controller.SendChatMessage(text, DateTime.UtcNow);
            }
        }

        #region IChatView Members

        public void MessageSent()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                ChatEntryTextBox.Text = "";
            }), null);
        }

        public void MessageSendingFailed(string reason)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                MessageBox.Show("Error: " + reason);
            }), null);
        }

        public void MessageReceived(ChatMessage message)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    _chatMessages.Add(message);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }), null);
        }

        public void FailedToEstablishChatChannel(string reason)
        {
            MessageBox.Show("Error: " + reason);
        }

        #endregion
    }
}
