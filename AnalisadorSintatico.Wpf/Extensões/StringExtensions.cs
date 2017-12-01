using System.Collections.Generic;
using System.Linq;

namespace System
{
    public static class StringExtensions
    {
        public static int ContainsAt(this string str, string value)
        {
            string simbolo = "";

            if (value.Length == 2)
            {
                for (int i = str.Length - 1; i >= 0; i--)
                {
                    if (simbolo.Length == 1)
                    {
                        simbolo = simbolo.Insert(0, str[i].ToString());
                    }
                    
                    if (simbolo.Length == 2 && String.Concat(str[i], str[i + 1]).Equals(value))
                    {
                        return i;
                    }

                    if (str[i].ToString().Equals("'"))
                    {
                        simbolo = "'";
                    }
                }
            }
            else
            {
                int i = str.Length - 1;

                while (i >= 0)
                {
                    if (str[i].ToString().Equals("'"))
                    {
                        i--;
                    }
                    else
                    {
                        if (str[i].ToString().Equals(value))
                        {
                            return i;
                        }
                    }

                    i--;
                }
            }

            return -1;
        }

        public static string StackToString<T>(Stack<T> stack, bool reversed = false)
        {
            string result = "";

            if (reversed)
            {
                foreach (T item in stack.Reverse())
                {
                    result += item + " ";
                }
            }
            else
            {
                foreach (T item in stack)
                {
                    result += item + " ";
                }
            }

            return result.TrimEnd();
        }
    }
}
