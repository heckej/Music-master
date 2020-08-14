using System;
using System.Collections.Generic;
using System.Text;

namespace Metrics
{
    public static class Characters
    {

        /// <summary>
        /// Counts the occurences of the characters in a string.
        /// </summary>
        /// <param name="str">The string to be analysed.</param>
        /// <returns>The number of times each character of the given string occurs in the given string in lowercase.</returns>
        public static IDictionary<char, int> CountOccurrencesInString(string str)
        {
            str = str.ToLower();
            IDictionary<char, int> chars = new Dictionary<char, int>();
            foreach (char chr in str)
            {
                int countOccurrences = 0;
                chars.TryGetValue(chr, out countOccurrences);
                if (chars.ContainsKey(chr))
                    chars[chr] = countOccurrences + 1;
                else
                    chars.Add(chr, countOccurrences + 1);
            }
            return chars;
        }


    }
}
