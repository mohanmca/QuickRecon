using System.Collections.Generic;
using System.Linq;

namespace QuickRecon
{
    public static class StringHelper
    {
        public static IEnumerable<string> GetPrefixedItems(this IEnumerable<string> source, string prefix)
        {
            return source.Select(s => prefix + s);
        }

        public static IEnumerable<string> GetSuffixedItems(this IEnumerable<string> source, string suffix)
        {
            return source.Select(s => s + suffix);
        }
    }
}