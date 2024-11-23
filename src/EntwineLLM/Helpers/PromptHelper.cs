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
            var stringBuilder = new StringBuilder();

            foreach (char c in input)
            {
                if (c == '"')
                {
                    stringBuilder.Append("\\\"");
                }
                else if (c == '\\')
                {
                    stringBuilder.Append("\\\\");
                }
                else if (c == '\b')
                {
                    stringBuilder.Append("\\b");
                }
                else if (c == '\f')
                {
                    stringBuilder.Append("\\f");
                }
                else if (c == '\n')
                {
                    stringBuilder.Append("\\n");
                }
                else if (c == '\r')
                {
                    stringBuilder.Append("\\r");
                }
                else if (c == '\t')
                {
                    stringBuilder.Append("\\t");
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
