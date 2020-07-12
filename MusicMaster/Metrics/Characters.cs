using System;
using System.Collections.Generic;
using System.Text;

namespace Metrics
{
    public static class Characters
    {
        public static IDictionary<char, int> Count(string str)
        {
            IDictionary<char, int> chars = new Dictionary<char, int>();
            foreach (char chr in str)
            {
                int oldVal = 0;
                chars.TryGetValue(chr, out oldVal);
                chars.Add(chr, oldVal + 1);
            }
            return chars;
        }


    }
}
