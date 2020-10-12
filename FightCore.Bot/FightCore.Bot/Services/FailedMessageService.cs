using System.Collections.Generic;
using System.Net.NetworkInformation;
using Discord;
using Discord.WebSocket;

namespace FightCore.Bot.Services
{
    public class FailedMessageService
    {
        private readonly Dictionary<ulong, IUserMessage> _failedMessages;

        public FailedMessageService()
        {
            _failedMessages = new Dictionary<ulong, IUserMessage>();
        }

        public void AddFailedMessaged(ulong failedId, IUserMessage responseId)
        {
            if (_failedMessages.ContainsKey(failedId))
            {
                return;
            }

            _failedMessages.Add(failedId, responseId);
        }

        public IUserMessage GetMessageIdForEditMessage(ulong messageId)
        {
            return _failedMessages.ContainsKey(messageId) ? _failedMessages[messageId] : null;
        }
    }
}
