using System;
using System.Collections.Generic;

namespace Assets.Code.Extensions
{
    public static class ListExtensions
    {
        private readonly static Random Rand = new Random();

        public static T GetRandomItem<T>(this List<T> input)
        {
            var index = (int) (Rand.NextDouble() * input.Count);
            return input[index > input.Count - 1 ? input.Count - 1 : index];
        }
    }
}
