using EntwineLlm.Extensions;

namespace EntwineLlm.Helpers
{
    internal static class PromptHelper
    {
        public static string Generate(string model, string userCode)
        {
            var promptText = Properties.Resources.DefaultPrompt
                .EscapeJsonString()
                .ReduceMultipleSpaces();

            return Properties.Resources.LlmBaseRequest
                    .Replace("{PROMPT}", promptText)
                    .Replace("{MODEL}", model)
                    .Replace("{CODE}", userCode.EscapeJsonString());
        }
    }
}
