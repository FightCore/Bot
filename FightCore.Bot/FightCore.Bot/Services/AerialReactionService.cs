using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace FightCore.Bot.Services
{
    public class AerialReactionService
    {
        private readonly Dictionary<ulong, (string NormalizedCharacterName, string NormalizedMoveName)> _messagesWithAerials;

        private const double TimerInterval = 1000 * 60 * 2;

        public AerialReactionService()
        {
            _messagesWithAerials = new Dictionary<ulong, (string NormalizedCharacterName, string NormalizedMoveName)>();
        }

        private void DeleteAerialMessage(ulong failedId)
        {
            if (!_messagesWithAerials.ContainsKey(failedId))
            {
                return;
            }

            _messagesWithAerials.Remove(failedId);
        }

        public void AddAerialMessage(ulong failedId, string normalizedCharacterName, string normalizedMoveName)
        {
            if (_messagesWithAerials.ContainsKey(failedId))
            {
                return;
            }

            _messagesWithAerials.Add(failedId, (normalizedCharacterName, normalizedMoveName));

            // Start a timer for 2 minutes, after this time is over, remove the message with the failed id.
            var timer = new Timer(TimerInterval);
            timer.Elapsed += (sender, args) => DeleteAerialMessage(failedId);
            timer.Start();
        }

        public (string NormalizedCharacterName, string NormalizedMoveName) GetMoveForMessageId(ulong messageId)
        {
            if (!_messagesWithAerials.ContainsKey((messageId)))
            {
                return default;
            }

            return _messagesWithAerials[messageId];
        }

        public bool ContainsMessage(ulong messageId)
        {
            return _messagesWithAerials.ContainsKey(messageId);
        }
    }
}
