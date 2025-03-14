using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Runtime.Versioning;

namespace AridityTeam.BetaFortressSetup.Util
{
    /// Implements AvalonEdit ICompletionData interface to provide the entries in the
    /// completion drop down.
    [SupportedOSPlatform("windows")] // holy shit the target os platform is already "windows" WHY TF DO I HAVE TO DO THIS?!?!?!
    public class IDECompletionData : ICompletionData
    {
        public IDECompletionData(string text)
        {
            this.Text = text;
            this.Description = null;
        }
        public IDECompletionData(string text, string desc)
        {
            this.Text = text;
            this.Description = desc;
        }

        public System.Windows.Media.ImageSource? Image
        {
            get { return null; }
        }

        public string Text { get; private set; }

        // Use this property if you want to show a fancy UIElement in the list.
        public object Content
        {
            get { return this.Text; }
        }

        public object? Description { get; private set; }

        public double Priority => 0.0;

        public void Complete(TextArea textArea, ISegment completionSegment,
            EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, this.Text);
        }
    }
}
