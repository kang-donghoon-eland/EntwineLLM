using EntwineLlm.Helpers;
using EntwineLlm.Models;
using Microsoft.VisualStudio.Shell;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace EntwineLlm
{
    public class RefactoringHelper
    {
        private readonly EntwineLlmOptions _options;

        public RefactoringHelper(AsyncPackage package)
        {
            _options = package.GetDialogPage(typeof(EntwineLlmOptions)) as EntwineLlmOptions;
        }

        public async Task RequestAndShowRefactoringSuggestionAsync(string methodCode, string activeDocumentPath)
        {
            var suggestion = await GetOllamaRefactoringSuggestionAsync(methodCode);
            await ShowRefactoringSuggestionAsync(suggestion, activeDocumentPath);
        }

        private async Task<string> GetOllamaRefactoringSuggestionAsync(string methodCode)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = _options.LlmRequestTimeOut;

                var prompt = PromptHelper.Generate(_options.LlmModel, methodCode);
                var content = new StringContent(prompt, Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync($"{_options.LlmUrl}/api/chat", content);
                    response.EnsureSuccessStatusCode();

                    var responseContent = await response.Content.ReadAsStringAsync();
                    var code = JObject.Parse(responseContent)["message"]["content"].ToString();

                    return code
                        .Replace("```csharp", "")
                        .Replace("```", "");
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        private static async Task ShowRefactoringSuggestionAsync(string suggestion, string activeDocumentPath)
        {
            ToolWindowPane window = await RequestRefactorCommand.Instance.ShowToolWindowAsync();
            var control = (RefactorSuggestionWindowControl)window.Content;
            control.DisplaySuggestion(suggestion, activeDocumentPath);
        }
    }
}
