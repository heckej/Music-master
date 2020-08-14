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

        /// <summary>
        /// Calculates the difference
        /// </summary>
        /// <param name="dict1"></param>
        /// <param name="dict2"></param>
        /// <returns></returns>
        public static IDictionary<char, int> DifferenceOccurrences(IDictionary<char, int> dict1, IDictionary<char, int> dict2)
        {
            IDictionary<char, int> difference = new Dictionary<char, int>();
            foreach (var character in dict1.Keys)
            {
                if (dict2.ContainsKey(character))
                {
                    int diff = dict1[character] - dict2[character];
                    if (diff != 0)
                    {
                        if (diff < 0)
                            diff = -diff;
                        difference.Add(character, diff);
                    }
                }
                else
                    difference.Add(character, dict1[character]);
            }
            foreach (var character in dict2.Keys)
                if (!dict1.ContainsKey(character))
                    difference.Add(character, dict2[character]);
            return difference;
        }

    }
}
