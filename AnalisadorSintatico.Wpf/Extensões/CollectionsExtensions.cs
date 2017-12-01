using System.Collections.Generic;

namespace System
{
    public static class ArrayExtensions
    {
        public static int GetIndexByValue<T>(this T[,] arr, T value, int dimension)
        {
            if (dimension == 0)
            {
                for (int i = 1; i < arr.GetLength(0); i++)
                {
                    if (arr[i, 0].Equals(value))
                        return i;
                }
            }
            else if (dimension == 1)
            {
                for (int i = 1; i < arr.GetLength(1); i++)
                {
                    if (arr[0, i].Equals(value))
                        return i;
                }
            }            

            return -1;
        }
        
        public static List<T1> TryGetRange<T1>(this List<T1> list, int index, int count)
        {
            if (index == count)
                return new List<T1>();

            return list.GetRange(index, count - 1);
        }
    }
}
