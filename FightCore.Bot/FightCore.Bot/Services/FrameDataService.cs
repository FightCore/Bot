using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FightCore.Bot.Helpers;
using FightCore.Bot.Models.FrameData;
using FightCore.MeleeFrameData;
using Newtonsoft.Json;

namespace FightCore.Bot.Services
{
    public class FrameDataService
    {
        private readonly List<WrapperCharacter> _characters;
        private readonly List<Misc> _miscs;
        private readonly List<NormalizedEntity> _entities;

        public FrameDataService()
        {
            List<Dodge> dodges;
            List<Attack> attacks;
            List<Grab> grabs;
            List<Throw> throws;
            _characters = JsonConvert.DeserializeObject<List<WrapperCharacter>>(File.ReadAllText("Data/Names.json"));

            foreach (var wrapperCharacter in _characters)
            {
                wrapperCharacter.NormalizedName = SearchHelper.Normalize(wrapperCharacter.Name);
            }

            using (var frameDataContext = new FrameDataContext())
            {
                attacks = frameDataContext.Attacks.ToList();
                dodges = frameDataContext.Dodges.ToList();
                grabs = frameDataContext.Grabs.ToList();
                throws = frameDataContext.Throws.ToList();
                _miscs = frameDataContext.Misc.ToList();
            }

            foreach (var attack in attacks)
            {
                attack.NormalizedCharacter = SearchHelper.Normalize(attack.Character);
                attack.NormalizedType = SearchHelper.Normalize(attack.Move);
                attack.NormalizedName = SearchHelper.Normalize(attack.Name);
            }

            foreach (var dodge in dodges)
            {
                dodge.NormalizedCharacter = SearchHelper.Normalize(dodge.Character);
                dodge.NormalizedType = SearchHelper.Normalize(dodge.Type);
                dodge.NormalizedName = SearchHelper.Normalize(dodge.Name);
            }

            foreach (var grab in grabs)
            {
                grab.NormalizedCharacter = SearchHelper.Normalize(grab.Character);
                grab.NormalizedType = SearchHelper.Normalize(grab.Type);
                grab.NormalizedName = SearchHelper.Normalize(grab.Name);
            }

            foreach (var @throw in throws)
            {
                @throw.NormalizedCharacter = SearchHelper.Normalize(@throw.Character);
                @throw.NormalizedType = DataHacks.FixThrowType(@throw.Type);
                @throw.NormalizedName = SearchHelper.Normalize(@throw.Name);
            }

            foreach (var misc in _miscs)
            {
                misc.NormalizedCharacter = SearchHelper.Normalize(misc.Character);
                misc.NormalizedType = SearchHelper.Normalize(misc.Character);
                misc.NormalizedName = SearchHelper.Normalize(misc.Character);
            }

            _entities = new List<NormalizedEntity>();
            _entities.AddRange(attacks);
            _entities.AddRange(dodges);
            _entities.AddRange(grabs);
            _entities.AddRange(throws);
        }

        public WrapperCharacter GetCharacter(string name)
        {
            var normalizedName = SearchHelper.Normalize(name);
            var character = _characters.FirstOrDefault(wrapperCharacter =>
                wrapperCharacter.Name.Equals(normalizedName, StringComparison.InvariantCultureIgnoreCase));

            return character ?? _characters.FirstOrDefault(wrapperCharacter => wrapperCharacter.Names.Contains(normalizedName));
        }

        public Misc GetMiscForCharacter(string name)
        {
            return _miscs.FirstOrDefault(misc => misc.NormalizedCharacter == SearchHelper.Normalize(name));
        }

        public List<NormalizedEntity> GetMoves(string characterName)
        {
            var character = GetCharacter(characterName);
            if (character == null)
            {
                return null;
            }

            return _entities.Where(entity => entity.NormalizedCharacter == character.NormalizedName).ToList();
        }

        public NormalizedEntity GetMove(string character, string move)
        {
            //==================================
            // Step 1: Normalize and prepare entities for search.
            //==================================
            var normalizedMove = SearchHelper.Normalize(move);
            var characterEntity = GetCharacter(character);

            //==================================
            // Step 2: Check if the move is a special name like Shine or Knee.
            //==================================
            if (characterEntity.Moves?.Any() == true)
            {
                var specialMoveName = characterEntity.Moves.FirstOrDefault(storedMove =>
                    storedMove.Key == normalizedMove);

                // keyValuePairs can not be null, check if its the default.
                if (!specialMoveName.Equals(default(KeyValuePair<string, string>)))
                {
                    normalizedMove = specialMoveName.Value;
                }
            }

            //==================================
            // Step 3: Look for the move/attack directly using no search optimizations.
            //==================================
            var moveEntity = _entities.FirstOrDefault(attack =>
                attack.NormalizedCharacter == characterEntity.NormalizedName
                && attack.NormalizedType == normalizedMove);

            // If found, just return it.
            if (moveEntity != null)
            {
                // Move was found directly, return it and false.
                return moveEntity;
            }

            //==================================
            // Step 4: Look for the move/attack in the fancy move name.
            //==================================
            moveEntity = _entities.FirstOrDefault(attack =>
                attack.NormalizedCharacter == characterEntity.NormalizedName
                && attack.NormalizedName.Contains(normalizedMove));

            if (moveEntity != null)
            {
                return moveEntity;
            }

            //==================================
            // Step 5: Get all attacks for a character and search for a move using algorithms.
            // Most notably levenshtein distance filtering out the small spelling errors like "faii" instead of "fair".
            //==================================
            var moves = _entities.Where(attack =>
                attack.NormalizedCharacter == characterEntity.NormalizedName).ToList();

            // Get the name of the move using search algorithms
            var moveName = SearchHelper.FindMatch(moves.Select(move => move.NormalizedType).ToList(),
                normalizedMove);

            // If this is still not found, just return null.
            if (string.IsNullOrWhiteSpace(moveName))
            {
                return null;
            }

            // If it is found, return the move and mention that its found indirectly.
            return moves.FirstOrDefault(storedMove =>
                storedMove.NormalizedType.Equals(moveName, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
