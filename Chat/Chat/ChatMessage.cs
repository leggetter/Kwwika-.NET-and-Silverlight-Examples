using System;
using System.Net;

namespace Kwwika.Examples.Chat
{
    public class ChatMessage
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public DateTime Sent { get; set; }

        public ChatMessage(string name, string message, DateTime sent)
        {
            Name = name;
            Message = message;
            Sent = sent;
        }
    }
}
