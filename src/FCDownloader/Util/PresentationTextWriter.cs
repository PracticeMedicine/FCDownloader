/*
 * Copyright (c) 2025 The Aridity Team, all rights reserved.
 * **CONFIDENTIAL**
 * Unauthorized use or disclosure in any manner may result in disciplinary action up to and including termination of employment 
 * (in the case of employees), termination of an assignment or contract (in the case of contingent staff), and potential
 * civil and criminal liability.
 */
using System;
using System.IO;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace AridityTeam.BetaFortressSetup.Util
{
    [SupportedOSPlatform("windows")]
    internal class PresentationTextWriter : TextWriter
    {
        private Control textbox;
        private Window? window;

        public PresentationTextWriter(Control textbox)
        {
            this.textbox = textbox;
            this.window = null;
        }
        public PresentationTextWriter(Control textbox, Window? window = null)
        {
            this.textbox = textbox;
            this.window = window;
        }

        public override void Write(char value)
        {
            AppendTextToOutputText(this.textbox, value.ToString());
        }

        public override void Write(string? value)
        {
            AppendTextToOutputText(this.textbox, value);
        }

        private void AppendTextToOutputText(Control textControl, string? value)
        {
            if (textControl is TextBox textBox)
            {
                ThreadPool.QueueUserWorkItem(wait =>
                {
                    if (this.window != null)
                    {
                        this.window.Dispatcher.Invoke(() =>
                        {
                            textBox.AppendText(value);
                            textBox.ScrollToEnd();
                        });
                    }
                    else
                    {
                        textBox.AppendText(value);
                    }
                });
            }
            else if (textControl is RichTextBox richTextBox)
            {
                ThreadPool.QueueUserWorkItem(wait =>
                {
                    if (this.window != null)
                    {
                        this.window.Dispatcher.Invoke(() =>
                        {
                            // Append text
                            richTextBox.AppendText(value);
                            // Scroll to the end of the RichTextBox
                            richTextBox.ScrollToEnd();
                        });
                    }
                    else
                    {
                        // Append text
                        richTextBox.AppendText(value);
                        // Scroll to the end of the RichTextBox
                        richTextBox.ScrollToEnd();
                    }
                });
            }
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
}
