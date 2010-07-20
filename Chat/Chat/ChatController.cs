using System;
using System.Net;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

using Kwwika;
using Kwwika.Logging;

namespace Kwwika.Examples.Chat
{
    public class ChatController : IChatController, ICommandListener, ISubscriptionListener
    {        
        private readonly IConnection _connection;
        private readonly string _chatTopicName;        
        private readonly ILogger _logger;
        private readonly IChatView _view;

        private string _username;

        public ChatController(IChatView view, IConnection connection, string chatTopicName, ILogger logger)
        {
            _view = view;
            _connection = connection;
            _chatTopicName = chatTopicName;
            _logger = logger;
            
            _connection.Logger = _logger;
            _connection.Subscribe(chatTopicName, this);
        }

        #region IChatController Members

        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
            }
        }
        
        public void SendChatMessage(string text, DateTime sendTime)
        {
            TimeSpan t = (sendTime - new DateTime(1970, 1, 1));
            int millisSinceEpoch = ((int)t.TotalSeconds) * 1000;
            var values = new Dictionary<string, string>() { { "name", _username }, { "text", text }, { "datetime", millisSinceEpoch.ToString() } };
            _connection.Publish(_chatTopicName, values, this);
        }

        #endregion

        #region ICommandListener Members

        public void CommandError(string topic, CommandErrorType code)
        {
            _view.MessageSendingFailed(code.ToString());
        }

        public void CommandSuccess(string topic)
        {
            _view.MessageSent();
        }

        #endregion

        #region ISubscriptionListener Members

        public void TopicError(ISubscription sub, CommandErrorType error)
        {
            _view.FailedToEstablishChatChannel(error.ToString());
        }

        public void TopicUpdated(ISubscription sub, Dictionary<string, string> values, bool isImage)
        {
            Debug.WriteLine("Topic Updated: " + sub.TopicName);

            if (values.ContainsKey("text"))
            {
                var message = new ChatMessage(values["name"], values["text"], JavaScriptDateToDateTime(values["datetime"]));
                _view.MessageReceived(message);
            }
        }

        #endregion

        private static DateTime JavaScriptDateToDateTime(string date)
        {
            long msSinceEpoch = Int64.Parse(date);
            return new DateTime(1970, 1, 1) + new TimeSpan(msSinceEpoch * 10000);
        }
    }
}
