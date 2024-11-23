using EntwineLlm.Helpers;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace EntwineLlm
{
    internal sealed class RequestRefactorCommand
    {
        public const int CommandId = 256;

        public static readonly Guid CommandSet = new Guid("714b6862-aad7-434e-8415-dd928555ba0e");

        private readonly AsyncPackage package;

        private static string _activeDocumentPath;

        private RequestRefactorCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        public static RequestRefactorCommand Instance { get; private set; }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new RequestRefactorCommand(package, commandService);
        }

        private void Execute(object sender, EventArgs e)
        {
            _ = PerformRefactoringSuggestionAsync();
        }

        private async Task PerformRefactoringSuggestionAsync()
        {
            var progressBarHelper = new ProgressBarHelper(ServiceProvider.GlobalProvider);
            progressBarHelper.StartIndeterminateDialog();

            var methodCode = GetCurrentMethodCode();
            var refactoringHelper = new RefactoringHelper(package);
            await refactoringHelper.RequestAndShowRefactoringSuggestionAsync(methodCode, _activeDocumentPath);

            progressBarHelper.StopDialog();
        }

        private static string GetCurrentMethodCode()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (!(Package.GetGlobalService(typeof(EnvDTE.DTE)) is EnvDTE.DTE dte))
            {
                return string.Empty;
            }

            var activeDocument = dte.ActiveDocument;
            if (activeDocument == null)
            {
                return string.Empty;
            }

            if (!(activeDocument.Selection is EnvDTE.TextSelection textSelection))
            {
                return string.Empty;
            }

            _activeDocumentPath = dte.ActiveDocument.FullName;
            return textSelection.Text;
        }

        public async Task<ToolWindowPane> ShowToolWindowAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            var window = await package.FindToolWindowAsync(typeof(RefactorSuggestionWindow), 0, true, package.DisposalToken);
            if (window == null || window.Frame == null)
            {
                throw new NotSupportedException("Cannot create window");
            }

            var windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
            return window;
        }
    }
}
