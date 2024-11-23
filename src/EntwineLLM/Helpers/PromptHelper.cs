using System.Collections.Generic;
using System.Text;

namespace EntwineLlm.Helpers
{
    internal static class PromptHelper
    {
        public static string Generate(string model, string userCode)
        {
            const string prompt = @"
{
  ""model"": ""{MODEL}"",
  ""stream"": false,
  ""messages"": [
    {
      ""role"": ""assistant"",
      ""content"": ""{PROMPT}""
    },
    {
      ""role"": ""user"",
      ""content"": ""[CODE]: {CODE}""
    }
  ]
}";

            var promptText = Properties.Resources.DefaultPrompt
                .Replace("\n", "\\n")
                .Replace("\r", "\\r");

            return prompt
                    .Replace("{PROMPT}", promptText)
                    .Replace("{MODEL}", model)
                    .Replace("{CODE}", EscapeJsonString(userCode));
        }

        private static string EscapeJsonString(string input)
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
