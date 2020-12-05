using System.Collections.Generic;
using System.Linq;

namespace FightCore.Logic.Search
{
    public static class SearchHelper
    {
        public static string Normalize(string value)
        {
            var removedChars = new string[] {"-", "_", ".", "@", "`", "\"", " ", "(", ")"};

            value = removedChars.
                Aggregate(value, (current, removedChar) =>
                    current.Replace(removedChar, string.Empty));

            return value.ToLower().Trim();
        }
        public static string NormalizeKeepSpace(string value)
        {
            var removedChars = new string[] { "-", "_", ".", "@", "`", "\"", "(", ")" };

            value = removedChars.
                Aggregate(value, (current, removedChar) =>
                    current.Replace(removedChar, string.Empty));

            return value.ToLower().Trim();
        }

        public static string FindMatch(ICollection<string> collection, string value)
        {
            foreach (var item in collection)
            {
                if (value.Length != item.Length)
                {
                    continue;
                }

                var distance =
                    value.ToCharArray()
                        .Zip(item.ToCharArray(), (c1, c2) => new { c1, c2 })
                        .Count(m => m.c1 != m.c2);
                if (distance == value.Length)
                {
                    continue;
                }

                if (distance < 2)
                {
                    return item;
                }
            }

            return null;
        }
    }
}
