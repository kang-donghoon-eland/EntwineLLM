﻿using EntwineLlm.Clients;
using EntwineLlm.Enums;
using EntwineLlm.Helpers;
using EntwineLlm.Models;
using Microsoft.VisualStudio.Shell;
using System;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace EntwineLlm
{
    internal class RefactoringHelper
    {
        private readonly GeneralOptions _generalOptions;
        private readonly ModelsOptions _modelsOptions;
        private readonly AsyncPackage _package;

        public RefactoringHelper(AsyncPackage package)
        {
            _package = package;
            _generalOptions = package.GetDialogPage(typeof(GeneralOptions)) as GeneralOptions;
            _modelsOptions = package.GetDialogPage(typeof(ModelsOptions)) as ModelsOptions;
        }

        public async Task RequestCodeSuggestionsAsync(
            string methodCode,
            string activeDocumentPath,
            CodeType codeType,
            string manualPrompt = "")
        {
            var suggestion = await GetCodeSuggestionsAsync(methodCode, codeType, manualPrompt);

            if (suggestion.Type == CodeType.Undefined)
            {
                WindowHelper.ErrorBox("There was an error during LLM query. Please check if LLM APIs are reachable");
                return;
            }

            switch (suggestion.Type)
            {
                case CodeType.Documentation:
                case CodeType.Review:
                    await ShowMarkdownWindowAsync(suggestion.Code);
                    break;

                default:
                    await ShowSuggestionWindowAsync(suggestion.Code, activeDocumentPath);
                    break;
            }
        }

        private async Task<CodeSuggestionResponse> GetCodeSuggestionsAsync(string methodCode, CodeType codeType, string manualPrompt)
        {
            var promptHelper = new PromptHelper(_generalOptions.Language);

            var prompt = codeType switch
            {
                CodeType.Manual => promptHelper.CreateForManualRequest(_modelsOptions.LlmFollowUp, methodCode, manualPrompt),
                CodeType.Refactor => promptHelper.CreateForRefactor(_modelsOptions.LlmRefactor, methodCode),
                CodeType.Test => promptHelper.CreateForTests(_modelsOptions.LlmUnitTests, methodCode),
                CodeType.Documentation => promptHelper.CreateForDocumentation(_modelsOptions.LlmDocumentation, methodCode),
                CodeType.Review => promptHelper.CreateForReview(_modelsOptions.LlmReview, methodCode),
                _ => throw new ArgumentException("Invalid requested code type"),
            };

            using var llmClient = new LlmClient(_generalOptions.LlmServer);
            return await llmClient.GetCodeSuggestionsAsync(codeType, prompt);
        }

        private async Task ShowSuggestionWindowAsync(string suggestion, string activeDocumentPath)
        {
            ToolWindowPane window = await WindowHelper.ShowToolWindowAsync<RefactorSuggestionWindow>(_package);
            var control = (RefactorSuggestionWindowControl)window.Content;
            control.DisplaySuggestion(suggestion, activeDocumentPath);
        }

        private async Task ShowMarkdownWindowAsync(string suggestion)
        {
            ToolWindowPane window = await WindowHelper.ShowToolWindowAsync<MarkdownViewerWindow>(_package);
            var control = (MarkdownViewer)window.Content;
            control.DisplaySuggestion(suggestion);
        }
    }
}
