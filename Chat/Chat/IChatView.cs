using System;
using System.Net;

namespace Kwwika.Examples.Chat
{
    public interface IChatView
    {
        void MessageSent();

        void MessageSendingFailed(string reason);

        void MessageReceived(ChatMessage message);

        void FailedToEstablishChatChannel(string p);
    }
}
