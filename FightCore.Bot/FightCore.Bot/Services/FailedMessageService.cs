using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Timers;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace FightCore.Bot.Services
{
    public class FailedMessageService
    {
        private readonly Dictionary<ulong, IUserMessage> _failedMessages;

        private const double TimerInterval = 1000 * 60 * 2;

        public FailedMessageService()
        {
            _failedMessages = new Dictionary<ulong, IUserMessage>();
        }

        private void DeleteFailedMessage(ulong failedId)
        {
            if (!_failedMessages.ContainsKey(failedId))
            {
                return;
            }

            _failedMessages.Remove(failedId);
        }

        public void AddFailedMessaged(ulong failedId, IUserMessage responseId)
        {
            if (_failedMessages.ContainsKey(failedId))
            {
                return;
            }

            _failedMessages.Add(failedId, responseId);

            // Start a timer for 2 minutes, after this time is over, remove the message with the failed id.
            var timer = new Timer(TimerInterval);
            timer.Elapsed += (sender, args) => DeleteFailedMessage(failedId);
            timer.Start();
        }

        public IUserMessage GetMessageIdForEditMessage(ulong messageId)
        {
            return _failedMessages.ContainsKey(messageId) ? _failedMessages[messageId] : null;
        }
    }
}
