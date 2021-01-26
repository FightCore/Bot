using System;
using FightCore.Logic.Formatting;
using FightCore.Logic.Search;
using FightCore.Logic.Services;

namespace FightCore.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var frameDataService = new FrameDataService();
            var characterSearcher = new CharacterSearcher(frameDataService);
            var characterFormatting = new CharacterFormatting("{0}:");

            System.Console.WriteLine("FightCore Console app v1.");
            while (true)
            {
                WriteLine("==============================================");
                WriteLine("Input Query");
                var query = System.Console.ReadLine();
                WriteLine("==============================================");
                if (string.IsNullOrWhiteSpace(query))
                {
                    continue;
                }
                var (character, remainder) = characterSearcher.Get(query.Split(' '));
                if (character == null)
                {
                    System.Console.WriteLine("Not found");
                    continue;
                }
                if (string.IsNullOrWhiteSpace(remainder))
                {
                    var statistics = frameDataService.GetStatsForCharacter(character.NormalizedName);
                    var info = frameDataService.GetInfoForCharacter(character.NormalizedName);
                    WriteLine(character.Name);
                    WriteLine(characterFormatting.CreateMovementData(statistics).ToString());
                    WriteLine(characterFormatting.CreateCharacterFrameData(statistics).ToString());
                    WriteLine(characterFormatting.CreateCharacterMiscData(info, character).ToString());
                    continue;
                }

                var move = frameDataService.GetMove(character.NormalizedName, remainder, character);
                if (move == null)
                {
                    WriteLine("Not found");
                    continue;
                }

                WriteLine(move.Name);
                WriteLine(characterFormatting.CreateMoveData(move).ToString());
                WriteLine(characterFormatting.CreateHitboxData(move).ToString());
            }
        }

        private static void WriteLine(string value)
        {
            System.Console.WriteLine(value);
        }
    }
}
