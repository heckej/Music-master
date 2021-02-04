using System.Collections.Generic;

namespace Metrics
{
    public static class Words
    {
        /// <summary>
        /// Counts the occurences of the words in a sentence.
        /// </summary>
        /// <param name="sentence">The sentence to be analysed.</param>
        /// <returns>The number of times each word of the given sentence occurs in the given sentence in lowercase.</returns>
        public static IDictionary<string, int> CountOccurencesInSentence(string sentence)
        {
            IDictionary<string, int> words = new Dictionary<string, int>();
            foreach (string word in sentence.Split(' '))
            {
                int countOccurrences = 0;
                words.TryGetValue(word.ToLower(), out countOccurrences);
                if (words.ContainsKey(word))
                    words[word] = countOccurrences + 1;
                else
                    words.Add(word.ToLower(), countOccurrences + 1);
            }
            return words;
        }

        public static IList<int> ComputeLengths(IList<string> words)
        {
            IList<int> lengths = new List<int>();
            foreach (string word in words)
                lengths.Add(word.Length);
            return lengths;
        }

        public static IList<double> ComputeNormalisedLengths(IList<string> words)
        {
            int totalLength = 0;
            var lengths = new List<double>();
            foreach (string word in words)
                totalLength += word.Length;
            foreach (string word in words)
                lengths.Add((double)word.Length / totalLength);
            return lengths;
        }

        /// <summary>
        /// Calculates the difference
        /// </summary>
        /// <param name="dict1"></param>
        /// <param name="dict2"></param>
        /// <returns></returns>
        public static IDictionary<string, int> DifferenceOccurrences(IDictionary<string, int> dict1, IDictionary<string, int> dict2)
        {
            var difference = new Dictionary<string, int>();
            foreach (var word in dict1.Keys)
            {
                if (dict2.ContainsKey(word))
                {
                    int diff = dict1[word] - dict2[word];
                    if (diff != 0)
                    {
                        if (diff < 0)
                            diff = -diff;
                        difference.Add(word, diff);
                    }
                }
                else
                    difference.Add(word, dict1[word]);
            }
            foreach (var character in dict2.Keys)
                if (!dict1.ContainsKey(character))
                    difference.Add(character, dict2[character]);
            return difference;
        }


    }
}
