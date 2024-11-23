using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using TextRange = System.Windows.Documents.TextRange;
using TextSelection = EnvDTE.TextSelection;

namespace EntwineLlm
{
    public partial class RefactorSuggestionWindowControl : UserControl
    {
        private string _activeDocumentPath;

        public RefactorSuggestionWindowControl()
        {
            InitializeComponent();
        }

        public void DisplaySuggestion(string suggestion, string activeDocumentPath)
        {
            SuggestionBox.Text = suggestion;
            _activeDocumentPath = activeDocumentPath;
        }

        private void ReplaceSelectedTextInIDE(string newText)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (!(Package.GetGlobalService(typeof(DTE)) is DTE dte))
            {
                return;
            }

            if (!string.IsNullOrEmpty(_activeDocumentPath))
            {
                var document = dte.Documents?.Item(_activeDocumentPath);
                document?.Activate();

                var textSelection = document?.Selection as TextSelection;
                textSelection?.Insert(newText, (int)vsInsertFlags.vsInsertFlagsContainNewText);
            }
        }

        private void btnApply_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string suggestionText = SuggestionBox.Text;
            ReplaceSelectedTextInIDE(suggestionText);

            btnClose_Click(sender, e);
        }

        private void btnClose_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var parentWindow = System.Windows.Window.GetWindow(this);
            parentWindow?.Close();
        }

        private void btnAbout_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow();
            var window = new System.Windows.Window
            {
                Title = "About EntwineLLM",
                Background = Brushes.Black,
                Content = aboutWindow,
                Width = 450,
                Height = 250,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Application.Current.MainWindow
            };

            window.ShowDialog();
        }
    }
}