using Metrics;
using System.Collections.Generic;

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

        // lowest levenshtein -> best match
        // levenshtein comparable (small difference): sub-levenshtein (compare every word of string with every word of strings in set) 
        //                                               -> weighted sum, weights based on length of word

        public static double CompareSentences(string sentence1, string sentence2)
        {
            // default metric: levenshtein
            // extra: common letters
            // extra: common words
            // extra: comparable words in different position
            //      (total levenshtein)*weight + (words in order levenshtein)*weight + abs[(letters sen1) - (letters sen2)]*weight + ...
            // 1 -  ------------------------------------------------------------------------------------------------------------------------------
            //      (max total levenshtein + sum max levenshtein words in order + max diff letters + ...)*(max weight)


            // loop over words sentence 1:
            //  if word occurs in sentence 2 (levDist == 0): ...
            //  calculate levDist/common chars: if below threshold -> 'word occurs in sentence 2'

            int totalLevenshtein = LevenshteinDistance.Compute(sentence1, sentence2);
            IList<double> levenshteinPerWord = LevenshteinDistance.ComputePerWordInSentence(sentence1, sentence2);
            IDictionary<string, int> wordOccurrencesSentence1 = Words.CountOccurencesInSentence(sentence1);
            IDictionary<string, int> wordOccurrencesSentence2 = Words.CountOccurencesInSentence(sentence2);
            IDictionary<char, int> charOccurrencesSentence1 = Characters.CountOccurrencesInString(sentence1);
            IDictionary<char, int> charOccurrencesSentence2 = Characters.CountOccurrencesInString(sentence2);

            return 0;
        }

        public static IList<double> CompareToListOfSentences(string sentence1, IList<string> listOfSentences)
        {
            IList<double> results = new List<double>();
            foreach (string sentence in listOfSentences)
            {
                results.Add(CompareSentences(sentence1, sentence));
            }
            return results;
        }

        private static ISet<string> _charsToBeRemovedInPreprocessing = InitializeCharactersToBeRemoved();

        private static ISet<string> InitializeCharactersToBeRemoved()
        {
            return new HashSet<string>
            {
                "[", "]", "-", " ", "\n", "\r", "?", "!", "(", ")", "\"", "\'", ".", ";", ","
            };
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
