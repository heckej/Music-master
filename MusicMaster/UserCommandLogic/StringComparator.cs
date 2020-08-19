using Metrics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace UserCommandLogic
{
    public static class StringComparator
    {
        public static IDictionary<string, int> WordDistanceToWordsInSentence(string word, string sentence)
        {
            IDictionary<string, int> wordDistanceToWordsInSentence = new Dictionary<string, int>();
            foreach (string wordInSentence in sentence.Split(' '))
                wordDistanceToWordsInSentence.Add(wordInSentence, LevenshteinDistance.Compute(word, wordInSentence));
            return wordDistanceToWordsInSentence;
        }

        public static (string bestMatch, int weight) GetBestMatch(string value, ISet<string> setToMatch)
        {
            if (value == null)
                return (null, 0);
            value = value.ToLower();
            string bestMatch = null;
            int bestLevDist = int.MaxValue; // Initialise to 'infinity'
            int currentLevDist;



            foreach (string setValue in setToMatch)
            {
                if (value == setValue.ToLower())
                    return (setValue, 0);
                currentLevDist = LevenshteinDistance.Compute(value, setValue.ToLower());
                if (setValue.ToLower().Contains(value))
                    currentLevDist -= value.Length;
                else if (value.Contains(setValue.ToLower()))
                    currentLevDist -= setValue.Length;
                if (currentLevDist < bestLevDist)
                {
                    bestLevDist = currentLevDist;
                    bestMatch = setValue;
                }
            }
            return (bestMatch, bestLevDist);
        }

        public static double CompareSentences(string sentence1, string stentence2)
        {
            // default metric: levenshtein
            // extra: common letters
            // extra: common words
            // extra: comparable words in different position

            // loop over words sentence 1:
            //  if word occurs in sentence 2 (levDist == 0): ...
            //  calculate levDist/common chars: if below threshold -> 'word occurs in sentence 2'

            return 0;
        }
        public static string PreprocessString(string s)
        {
            if (s is null)
                return null;
            foreach (var c in _charsToBeRemovedInPreprocessing)
                s = s.Replace(c, "");
            return s.ToLower().Replace("&", "and");
        }

        public static IList<string> PreprocessCollection(IList<string> list)
        {
            var results = new List<string>();
            foreach (var s in list)
            {
                var str = PreprocessString(s);
                results.Add(str);
            }
            return results;
        }

        public static HashSet<string> PreprocessCollection(HashSet<string> set)
        {
            var results = new HashSet<string>();
            foreach (var s in set)
            {
                var str = PreprocessString(s);
                results.Add(str);
            }
            return results;
        }

        public static SortedSet<string> PreprocessCollection(SortedSet<string> set)
        {
            var results = new SortedSet<string>();
            foreach (var s in set)
            {
                var str = PreprocessString(s);
                results.Add(str);
            }
            return results;
        }
    }
}
