using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace EntwineLlm.Extensions
{
    internal static class StringExtensions
    {
        public static string ReduceMultipleSpaces(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            return Regex.Replace(input, @"\s+", " ");
        }

        public static string EscapeJsonString(this string input)
        {
            var escapeSequences = new Dictionary<char, string>
            {
                {'\"', "\\\""},
                {'\\', "\\\\"},
                {'\b', "\\b"},
                {'\f', "\\f"},
                {'\n', "\\n"},
                {'\r', "\\r"},
                {'\t', "\\t"}
            };

            var stringBuilder = new StringBuilder();
            foreach (char c in input)
            {
                if (escapeSequences.TryGetValue(c, out var escape))
                {
                    stringBuilder.Append(escape);
                }
                else if (char.IsControl(c))
                {
                    stringBuilder.Append($"\\u{(int)c:X4}");
                }
                else
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString();
        }
    }
}
