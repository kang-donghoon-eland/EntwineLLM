using EntwineLlm.Clients;
using EntwineLlm.Clients.Interfacs;
using EntwineLlm.Commands.Interfaces;
using EntwineLlm.Enums;
using EntwineLlm.Models;

using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace EntwineLlm
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(RefactorSuggestionWindow))]
    [ProvideToolWindow(typeof(MarkdownViewerWindow))]
    [ProvideOptionPage(typeof(GeneralOptions), "EntwineLlm", "Configuration", 0, 0, true)]
    [ProvideOptionPage(typeof(ModelsOptions), "EntwineLlm", "Models", 0, 0, true)]
    internal sealed class EntwineLlmPackage : AsyncPackage
    {
        public const string PackageGuidString = "3c995b0e-1f37-4cef-9ac7-9771b3fb6162";

        public static AsyncPackage Instance { get; set; }
        public static ILlmClient LlmClient { get; set; }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            Instance = this;

            var generalOptions = this.GetDialogPage(typeof(GeneralOptions)) as GeneralOptions;
            if (generalOptions.ModelType == ClientAgentType.LmStudio)
            {
                LlmClient = new LmStudioClient(generalOptions);
            }
            else if (generalOptions.ModelType == ClientAgentType.Ollama)
            {
                LlmClient = new OllamaClient(generalOptions);
            }


            var commandsMenu = new CommandsMenu();
            await commandsMenu.InitializeAsync(this);

            var commands = new List<IBaseCommand>()
            {
                new RequestRefactorCommand(this),
                new GenerateTestsCommand(this),
                new DocumentCodeCommand(this),
                new CodeReviewCommand(this)
            };

            commandsMenu.AddCommands(commands);
        }
    }
}
