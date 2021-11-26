using System;

namespace CollectPicture
{
    static class Extension
    {
        private static int lastIndex = -1;
        private static Random rnd = new Random();

        public static bool IsNullOrEmpty<T>(this T[] array)
        {
            return array == null || array.Length == 0;
        }

        public static T Choise<T>(this T[] array)
        {
            if (array.IsNullOrEmpty())
            {
                throw new ArgumentNullException("Array is NULL or Empty");
            }

            if (array.Length == 1)
            {
                return array[0];
            }

            int currentIndex = lastIndex;
            while (currentIndex == lastIndex)
            {
                currentIndex = rnd.Next(array.Length);
            }

            lastIndex = currentIndex;
            return array[currentIndex];
        }
    }
}