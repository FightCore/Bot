using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FightCore.Bot.Helpers;
using FightCore.Bot.Models.FrameData;
using FightCore.Bot.Models.Helpers;
using FightCore.FrameData;
using FightCore.FrameData.Models;
using FightCore.MeleeFrameData;
using Newtonsoft.Json;
using Move = FightCore.FrameData.Models.Move;

namespace FightCore.Bot.Services
{
    public class FrameDataService
    {
        private readonly List<WrapperCharacter> _characters;
        private readonly List<Misc> _miscs;
        private readonly List<MoveAlias> _moveAliases;
        private readonly List<Character> _frameDataCharacters;

        public FrameDataService()
        {
            _characters = JsonConvert.DeserializeObject<List<WrapperCharacter>>(File.ReadAllText("Data/Names.json"));
            _moveAliases = JsonConvert.DeserializeObject<List<MoveAlias>>(File.ReadAllText("Data/MoveAlias.json"));

            var moveService = new MoveService();
            _frameDataCharacters = moveService.GetCharacters().GetAwaiter().GetResult();

            foreach (var frameDataCharacter in _frameDataCharacters)
            {
                foreach (var move in frameDataCharacter.Moves)
                {
                    move.NormalizedMoveName = SearchHelper.Normalize(move.Name);
                }
            }

            foreach (var wrapperCharacter in _characters)
            {
                wrapperCharacter.NormalizedName = SearchHelper.Normalize(wrapperCharacter.Name);
            }

            using (var frameDataContext = new MeleeFrameDataContext())
            {
                _miscs = frameDataContext.Misc.ToList();
            }

            foreach (var misc in _miscs)
            {
                misc.NormalizedCharacter = SearchHelper.Normalize(misc.Character);
                misc.NormalizedType = SearchHelper.Normalize(misc.Character);
                misc.NormalizedName = SearchHelper.Normalize(misc.Character);
            }
        }

        public WrapperCharacter GetCharacter(string name)
        {
            var normalizedName = SearchHelper.Normalize(name);
            var character = _characters.FirstOrDefault(wrapperCharacter =>
                wrapperCharacter.NormalizedName.Equals(normalizedName, StringComparison.InvariantCultureIgnoreCase));

            return character ?? _characters.FirstOrDefault(wrapperCharacter =>
                wrapperCharacter.Names.Any(alias => alias == normalizedName)
                || wrapperCharacter.NormalizedName.Contains(normalizedName));
        }

        public Misc GetMiscForCharacter(string name)
        {
            return _miscs.FirstOrDefault(misc => misc.NormalizedCharacter == SearchHelper.Normalize(name));
        }

        public List<Move> GetMoves(string characterName)
        {
            var character = GetCharacter(characterName);
            if (character == null)
            {
                return null;
            }

            return _frameDataCharacters.FirstOrDefault(entity => entity.NormalizedName == character.NormalizedName)?.Moves.ToList();
        }

        public Move GetMove(string character, string move, WrapperCharacter wrapperCharacter)
        {
            //==================================
            // Step 1: Normalize and prepare entities for search.
            //==================================
            var normalizedMove = SearchHelper.Normalize(move);
            var normalizedCharacter = SearchHelper.Normalize(character);
            var characterEntity = _frameDataCharacters.FirstOrDefault(storedCharacter =>
                storedCharacter.NormalizedName == normalizedCharacter);

            if (characterEntity == null)
            {
                return null;
            }

            //==================================
            // Step 2: Get move aliases for specific things like "b" meaning "neutralb".
            //==================================
            var alias = _moveAliases.FirstOrDefault(moveAlias =>
                moveAlias.Alias.Any(storedAlias => storedAlias == normalizedMove));

            if (alias != null)
            {
                normalizedMove = alias.Move;
                move = alias.Move;
            }

            //==================================
            // Step 3: Check if the move is a special name like Shine or Knee.
            //==================================
            if (wrapperCharacter?.Moves?.Any() == true)
            {
                var specialMoveName = wrapperCharacter.Moves.FirstOrDefault(storedMove =>
                    storedMove.Key == normalizedMove);

                // keyValuePairs can not be null, check if its the default.
                if (!specialMoveName.Equals(default(KeyValuePair<string, string>)))
                {
                    normalizedMove = specialMoveName.Value;
                    move = specialMoveName.Value;
                }
            }

            //==================================
            // Step 3: Check if the move contains something like "air"
            //==================================
            var airAliases = new[] { "(air)", "aerial" };
            foreach (var airAlias in airAliases)
            {
                // Check the standard non-normalized as the ( and ) would get filtered out by the normalization
                // Note that we cant search for "air" because bair, fair, dair and uair would be messed up.
                if (!move.Contains(airAlias))
                {
                    continue;
                }

                move = move.Replace(airAlias, "");
                // Add "a" as a prefix because thats how the aerial move names work.
                move = "a" + move;

                // Normalize it after all to make sure the rest works.
                normalizedMove = SearchHelper.Normalize(move);
                break;
            }


            //==================================
            // Step 4: Look for the move/attack directly using no search optimizations.
            //==================================
            var moveEntity = characterEntity.Moves.FirstOrDefault(attack => 
                             attack.NormalizedName == normalizedMove);

            // If found, just return it.
            if (moveEntity != null)
            {
                // Move was found directly, return it and false.
                return moveEntity;
            }

            //==================================
            // Step 5: Look for the move/attack in the fancy move name.
            //==================================
            moveEntity = characterEntity.Moves.FirstOrDefault(attack => 
                attack.Name.Equals(normalizedMove, StringComparison.InvariantCultureIgnoreCase)
                || attack.Name.Contains(normalizedMove, StringComparison.InvariantCultureIgnoreCase)
                || attack.Name.Contains(move, StringComparison.InvariantCultureIgnoreCase)
                || attack.NormalizedMoveName.Contains(move, StringComparison.InvariantCultureIgnoreCase));

            if (moveEntity != null)
            {
                return moveEntity;
            }

            //==================================
            // Step 6: Get all attacks for a character and search for a move using algorithms.
            // Most notably levenshtein distance filtering out the small spelling errors like "faii" instead of "fair".
            //==================================
            // Get the name of the move using search algorithms
            var moveName = SearchHelper.FindMatch(
                characterEntity.Moves.Select(storedMove => storedMove.NormalizedName).ToList(),
                normalizedMove);

            // If this is still not found, just return null.
            if (string.IsNullOrWhiteSpace(moveName))
            {
                return null;
            }

            // If it is found, return the move and mention that its found indirectly.
            return characterEntity.Moves.FirstOrDefault(storedMove =>
                storedMove.NormalizedName.Equals(moveName, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
