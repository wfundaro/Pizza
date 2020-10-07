using System;
using System.Collections.Generic;
using System.Text;

namespace PizzaApp.Helpers
{
    public static class StringUtils
    {
        public static string Capitalize(this string str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return str;
            }
            return str.Substring(0, 1).ToUpper() + str.Substring(1, str.Length - 1).ToLower();
        }
    }
}
