using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Metrics
{

    /// <summary>
    /// A class that contains methods based on the Levensthein distance to compare strings.
    /// </summary>
    public static class LevenshteinDistance
    {
        /// <summary>
        /// Computes the Levensthein distance between two strings.
        /// </summary>
        /// <see>Code copied from Dot Net Perls: https://www.dotnetperls.com/levenshtein </see>
        public static int Compute(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }

        /// <summary>
        /// Computes the Levenshtein distance between the respective words of two given sentences.
        /// </summary>
        /// <param name="sentence1">The first sentence to be compared.</param>
        /// <param name="sentence2">The second sentence to be compared.</param>
        /// <param name="ratio">Calculate as similarity ratios</param>
        /// <returns>A list of the Levenshtein distances between the respective words. 
        /// The list contains as much elements as there are words in the sentence with the most words.
        /// If one sentence is longer than the other, the words that don't have a respective word to 
        /// be compared to are compared to the empty string.</returns>
        public static IList<double> ComputePerWordInSentence(string sentence1, string sentence2, bool ratio=false)
        {
            IList<double> results = new List<double>();
            IList<string> wordsLongestSentence;
            IList<string> wordsShortestSentence;

            if (sentence1.Split(' ').Length > sentence2.Length)
            {
                wordsLongestSentence = sentence1.Split(' ');
                wordsShortestSentence = sentence2.Split(' ');
            }            
            else
            {
                wordsLongestSentence = sentence2.Split(' ');
                wordsShortestSentence = sentence1.Split(' ');
            }
            for (int i = 0; i < wordsLongestSentence.Count; i++)
            {
                if (i < wordsShortestSentence.Count)
                    if (!ratio)
                        results.Add(Compute(wordsLongestSentence[i], wordsShortestSentence[i]));
                    else
                    {
                        results.Add(ComputeSimilarityRatio(wordsLongestSentence[i], wordsShortestSentence[i]));
                    }
                else
                {
                    if (!ratio)
                        results.Add(wordsLongestSentence[i].Length);
                    else
                        results.Add(0);
                }
            }

            return results; 
        }

        public static IList<double> ComputeBestPerWordInFirstSentence(string sentence1, string sentence2, bool ratio=false)
        {
            IList<double> results = new List<double>();
            var wordsSentence1 = sentence1.Split(' ');
            var wordsSentence2 = sentence2.Split(' ');
            foreach (var word1 in wordsSentence1)
            {
                double bestLev = word1.Length;
                double currentLev = bestLev;
                if (ratio)
                    bestLev = 0;
                foreach (var word2 in wordsSentence2)
                {
                    if (ratio)
                    {
                        currentLev = ComputeSimilarityRatio(word1, word2);
                        if (currentLev > bestLev)
                            bestLev = currentLev;
                    }
                    else
                    {
                        currentLev = Compute(word1, word2);
                        if (currentLev < bestLev)
                            bestLev = currentLev;
                    }
                }
                results.Add(bestLev);
            }
            return results;
        }

        /// <summary>
        /// Computes the Levenshtein similarity ratio of two strings.
        /// </summary>
        /// <param name="s">The first string to be compared.</param>
        /// <param name="t">The second string to be compared.</param>
        /// <returns>(s.Length + t.Length - Compute(s,t))/(s.Length + t.Length)</returns>
        public static double ComputeSimilarityRatio(string s, string t)
        {
            return (double) (s.Length + t.Length - Compute(s,t)) / (s.Length + t.Length);
        }
        public static (string bestMatch, double levenshteinDistance) GetBestMatch(string value, ISet<string> setToMatch, bool ratio=false)
        {
            if (value == null)
                return (null, 0);
            value = value.ToLower();
            string bestMatch = null;
            double bestLevDist = int.MaxValue;
            if (ratio)
                bestLevDist = 0;
            double currentLevDist;

            foreach (var item in setToMatch)
            {
                if (ratio)
                {
                    currentLevDist = Compute(item, value);
                    if (currentLevDist > bestLevDist)
                        bestLevDist = currentLevDist;
                }
                else
                {
                    currentLevDist = ComputeSimilarityRatio(item, value);
                    if (currentLevDist < bestLevDist)
                        bestLevDist = currentLevDist;
                }
            }
            return (bestMatch, bestLevDist);
        }

        public static ISet<(string, double)> GetAllBestMatches(string value, ISet<string> setToMatch, double threshold, bool ratio=false)
        {
            var results = new HashSet<(string, double)>();
            if (value == null)
                return null;
            value = value.ToLower();
            double currentScore;
            foreach (var item in setToMatch)
            {
                if (ratio)
                {
                    currentScore = ComputeSimilarityRatio(item, value);
                    if (currentScore >= threshold)
                        results.Add((item, currentScore));
                }
                else
                {
                    currentScore = Compute(item, value);
                    if (currentScore <= threshold)
                        results.Add((item, currentScore));
                }
            }
            return results;
        }
    }
}
