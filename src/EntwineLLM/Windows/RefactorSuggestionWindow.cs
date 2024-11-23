using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;

namespace EntwineLlm
{
    [Guid("22dcf2f8-c8c1-4cf4-b1aa-cde7897a63a8")]
    public class RefactorSuggestionWindow : ToolWindowPane
    {
        public RefactorSuggestionWindow() : base(null)
        {
            this.Caption = "Code suggestion";
            this.Content = new RefactorSuggestionWindowControl();
        }
    }
}
