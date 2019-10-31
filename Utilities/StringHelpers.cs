using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    public static class StringHelpers
    {
        public static string LimitStringToCount(string text, int limit = 2000)
        {
            while (text.Length > limit)
            {
                text.Remove(0);
            }
            return text;
        }

        public static List<string> SplitStringToMultiple(string text, int limit = 2000)
        {
            var result = new List<string>();
            while (text.Length > 0)
            {
                if (text.Length > limit)
                {
                    result.Add(text.Substring(0, 2000));
                    text.Remove(0, 2000);
                }
                else
                {
                    result.Add(text.Substring(0, text.Length));
                    text.Remove(0, text.Length);
                }
            }
            return result;
        }
    }
}
