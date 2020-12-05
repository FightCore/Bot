using System;
using System.Collections.Generic;
using System.Text;
using FightCore.Logic.Aliasses.FrameData;
using FightCore.Logic.Services;

namespace FightCore.Logic.Search
{
    public interface ICharacterSearcher
    {
        (WrapperCharacter Character, string Remainder) Get(string[] query);
    }

    public class CharacterSearcher : ICharacterSearcher
    {
        private readonly FrameDataService _frameDataService;

        public CharacterSearcher(FrameDataService frameDataService)
        {
            _frameDataService = frameDataService;
        }

        public (WrapperCharacter Character, string Remainder) Get(string[] query)
        {
            WrapperCharacter characterEntity = null;
            string character = null;
            string tempCharacter = null;
            string remainder = null;
            var iterator = 1;
            foreach (var section in query)
            {
                if (tempCharacter == null)
                {
                    character = string.Join(' ', query);
                    tempCharacter = section;
                }
                else
                {
                    tempCharacter += " " + section;
                }

                characterEntity = _frameDataService.GetCharacter(tempCharacter);
                if (characterEntity == null)
                {
                    iterator++;
                    continue;
                }

                var split = iterator++;
                character = tempCharacter;
                remainder = string.Join(' ', query[split..]);
            }

            if (characterEntity == null && !string.IsNullOrWhiteSpace(character))
            {
                characterEntity = _frameDataService.GetCharacter(character);
            }

            if (characterEntity != null)
            {
                return (characterEntity, remainder);
            }

            // Reset all the temp variables.
            tempCharacter = null;
            character = null;
            remainder = null;
            iterator = 1;

            Array.Reverse(query);
            foreach (var section in query)
            {
                if (tempCharacter == null)
                {
                    character = string.Join(' ', query);
                    tempCharacter = section;
                }
                else
                {
                    tempCharacter = section + " " + tempCharacter;
                }

                characterEntity = _frameDataService.GetCharacter(tempCharacter);
                if (characterEntity == null)
                {
                    iterator++;
                    continue;
                }

                var split = iterator++;
                character = tempCharacter;
                remainder = string.Join(' ', query[split..]);
            }

            if (characterEntity == null && !string.IsNullOrWhiteSpace(character))
            {
                characterEntity = _frameDataService.GetCharacter(character);
            }

            return (characterEntity, remainder);
        }
    }
}
