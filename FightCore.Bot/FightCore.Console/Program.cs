using System;
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

            System.Console.WriteLine("FightCore Console app v1.");
            while (true)
            {
                System.Console.WriteLine("Input Query");
                var query = System.Console.ReadLine();
                if (string.IsNullOrWhiteSpace(query))
                {
                    continue;
                }
                var (character, remainder) = characterSearcher.Get(query.Split(' '));
                if (string.IsNullOrWhiteSpace(remainder))
                {
                    System.Console.WriteLine(character == null ? "Not found" : character.Name);

                    continue;
                }

                System.Console.WriteLine("Move.");
            }
        }
    }
}
