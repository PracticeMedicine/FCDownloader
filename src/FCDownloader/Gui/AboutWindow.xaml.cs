/*
 * Copyright (c) 2025 The Aridity Team, all rights reserved.
 * **CONFIDENTIAL**
 * Unauthorized use or disclosure in any manner may result in disciplinary action up to and including termination of employment 
 * (in the case of employees), termination of an assignment or contract (in the case of contingent staff), and potential
 * civil and criminal liability.
 */
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace AridityTeam.BetaFortressSetup.Gui
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            SetAppInfo();
        }

        private void SetAppInfo()
        {
            SetAppInfo(Assembly.GetEntryAssembly());
        }
        
        private void SetAppInfo(Assembly? assembly)
        {
            this.Title = $"About {assembly?
                .GetCustomAttribute<AssemblyProductAttribute>()?
                .Product}";
            this.AppName.Content = assembly?
                .GetCustomAttribute<AssemblyProductAttribute>()?
                .Product;
            this.AppVer.Content = $"Version {assembly?.GetName()?.Version?.ToString()}";
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ReactiveUI_Click(object sender, RoutedEventArgs e)
        {
            using (Process urlStarter = new Process())
            {
                urlStarter.StartInfo.FileName = "https://github.com/reactiveui/ReactiveUI.git";
                urlStarter.StartInfo.UseShellExecute = true;
                urlStarter.Start();
            }
        }

        private void ReactiveUILicense_Click(object sender, RoutedEventArgs e)
        {
            using (Process urlStarter = new Process())
            {
                urlStarter.StartInfo.FileName = "https://github.com/reactiveui/ReactiveUI/blob/main/LICENSE";
                urlStarter.StartInfo.UseShellExecute = true;
                urlStarter.Start();
            }
        }

        private void MSExtLogging_Click(object sender, RoutedEventArgs e)
        {
            using (Process urlStarter = new Process())
            {
                urlStarter.StartInfo.FileName = "https://github.com/dotnet/runtime/tree/main/src/libraries/Microsoft.Extensions.Logging";
                urlStarter.StartInfo.UseShellExecute = true;
                urlStarter.Start();
            }
        }

        private void MSExtLoggingLicense_Click(object sender, RoutedEventArgs e)
        {
            using (Process urlStarter = new Process())
            {
                urlStarter.StartInfo.FileName = "https://github.com/dotnet/runtime/blob/main/LICENSE.TXT";
                urlStarter.StartInfo.UseShellExecute = true;
                urlStarter.Start();
            }
        }

        private void Sentry_Click(object sender, RoutedEventArgs e)
        {
            using (Process urlStarter = new Process())
            {
                urlStarter.StartInfo.FileName = "https://github.com/getsentry/sentry-dotnet";
                urlStarter.StartInfo.UseShellExecute = true;
                urlStarter.Start();
            }
        }

        private void SentryLicense_Click(object sender, RoutedEventArgs e)
        {
            using (Process urlStarter = new Process())
            {
                urlStarter.StartInfo.FileName = "https://github.com/getsentry/sentry-dotnet/blob/main/LICENSE";
                urlStarter.StartInfo.UseShellExecute = true;
                urlStarter.Start();
            }
        }

        private void LibGit2Sharp_Click(object sender, RoutedEventArgs e)
        {
            using (Process urlStarter = new Process())
            {
                urlStarter.StartInfo.FileName = "https://github.com/libgit2/libgit2sharp";
                urlStarter.StartInfo.UseShellExecute = true;
                urlStarter.Start();
            }
        }

        private void LG2SLicense_Click(object sender, RoutedEventArgs e)
        {
            using (Process urlStarter = new Process())
            {
                urlStarter.StartInfo.FileName = "https://github.com/libgit2/libgit2sharp/blob/master/LICENSE.md";
                urlStarter.StartInfo.UseShellExecute = true;
                urlStarter.Start();
            }
        }
    }
}
