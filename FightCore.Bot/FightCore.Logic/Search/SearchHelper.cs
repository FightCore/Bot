using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage;

namespace FightCore.Logic.Search
{
    public static class SearchHelper
    {
        public static string Normalize(string value)
        {
            var removedChars = new string[] {"-", "_", ".", "@", "`", "\"", " ", "(", ")", "="};

            value = removedChars.
                Aggregate(value, (current, removedChar) =>
                    current.Replace(removedChar, string.Empty));

            return value.ToLower().Trim();
        }
        public static string NormalizeKeepSpace(string value)
        {
            var removedChars = new string[] { "-", "_", ".", "@", "`", "\"", "(", ")", "=" };

            value = removedChars.
                Aggregate(value, (current, removedChar) =>
                    current.Replace(removedChar, string.Empty));

            return value.ToLower().Trim();
        }

        public static string FindMatch(ICollection<string> collection, string value)
        {
            string highestScoreEntry = null;
            var highestScore = 0.7;
            foreach (var entry in collection)
            {
                var score = JaroWinklerDistance.proximity(entry, value);
                if (score < highestScore)
                {
                    continue;
                }

                highestScore = score;
                highestScoreEntry = entry;
            }

            return highestScoreEntry;

            // Left unused for now, can be used as secondary check
            return (from item in collection
                where value.Length == item.Length
                let distance = value.ToCharArray()
                    .Zip(item.ToCharArray(), (c1, c2) => new {c1, c2})
                    .Count(m => m.c1 != m.c2)
                where distance != value.Length
                where distance < 2
                select item).FirstOrDefault();
        }
    }
}
