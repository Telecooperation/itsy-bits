using System;
using System.Collections.Generic;
using System.Linq;

namespace ItsyBitsOptitrackCapture
{
    public static class Extensions
    {
        public static bool Range(this double x, double a, double b)
        {
            return (x >= a && x <= b);
        }

        public static int ToAwesomeSeed(this String seedString)
        {
            return Array.ConvertAll(seedString.ToCharArray(), c => (int)c).Sum();
        }

        public static void Shuffle<T>(this IList<T> list, int seed)
        {
            var rng = new Random(seed);
            int n = list.Count;

            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

    }
}
