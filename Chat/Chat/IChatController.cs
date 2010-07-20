using System;

namespace Kwwika.Examples.Chat
{
    public interface IChatController
    {
        string Username { get; set; }
        
        void SendChatMessage(string text, DateTime sendTime);
    }
}
