using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Infrastructure
{
    public static class StringHelpers
    {
       

        public static string CapitalizeWords(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (value.Length == 0)
                return value;

            StringBuilder result = new StringBuilder(value);
            result[0] = char.ToUpper(result[0]);
            for (int i = 1; i < result.Length; ++i)
            {
                if (char.IsWhiteSpace(result[i - 1]))
                    result[i] = char.ToUpper(result[i]);
                else
                    result[i] = char.ToLower(result[i]);
            }
            return result.ToString();
        }


        public static string ReplaceAllNonAlphaNumericChars(this string inString)
        {
            return ReplaceAllNonAlphaNumericChars(inString, string.Empty);
        }

        public static string ReplaceAllNonAlphaNumericChars(this string inString, string replaceBy)
        {
            if (string.IsNullOrEmpty(inString))
                return string.Empty;

            return Regex.Replace(inString.Trim(), @"[^A-Za-z0-9]+", replaceBy);
        }
        public static string ReplaceAllNonNumericChars(this string inString)
        {
            return ReplaceAllNonNumericChars(inString, string.Empty);
        }

        public static string ReplaceAllNonNumericChars(this string inString, string replaceBy)
        {
            if (string.IsNullOrEmpty(inString))
                return string.Empty;

            return Regex.Replace(inString.Trim(), @"[^0-9]+", replaceBy);
        }



        /// <summary>
        /// Compute the distance between two strings.
        /// </summary>
        public static int ComputeLevenshteinDistance(string s, string t)
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

        public static string GetRandomString()
        {
            string path = System.IO.Path.GetRandomFileName();
            path = path.Replace(".", ""); // Remove period.
            return path;
        }

        public static string GetRandomString(int length, bool lowerCase)
        {
            string charsList = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";
            char[] chars = charsList.ToCharArray();

            var cryptoGen = new System.Security.Cryptography.RNGCryptoServiceProvider();
            byte[] bytData = new byte[length];
            cryptoGen.GetBytes(bytData);

            StringBuilder result = new StringBuilder();
            foreach (byte b in bytData)
            {
                result.Append(chars[b % chars.Length]);
            }

            return lowerCase ? result.ToString().ToLower() : result.ToString();
        }

        public static int WordCount(this String str)
        {
            return str.Split(new char[] { ' ', '.', '?' },
                             StringSplitOptions.RemoveEmptyEntries).Length;
        }

        public static string[] StringToArray(string stringValue)
        {
            return stringValue.Split(new Char[] { ' ', ',', '.', ':', ';', '\t' });
        }

        public static string[] StringToArray(string stringValue, string delimiter)
        {
            return stringValue.Split(delimiter.ToCharArray());
        }

        public static string EncodeToBase64String(this string input)
        {
            byte[] bytes = System.Text.UTF8Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytes);
        }
        public static string DecodeFromBase64String(this string input)
        {
            byte[] bytes = Convert.FromBase64String(input);
            return System.Text.UTF8Encoding.UTF8.GetString(bytes);
        }
    }
}
