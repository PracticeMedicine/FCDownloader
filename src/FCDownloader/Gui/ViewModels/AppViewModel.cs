/*
 * Copyright (c) 2025 The Aridity Team
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 */
using System;
using System.IO;
using System.Reflection;
using ReactiveUI;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Runtime.Versioning;
using System.Windows.Input;

using AridityTeam.BetaFortressSetup.Util;

namespace AridityTeam.BetaFortressSetup.Gui.ViewModels
{
    /// <summary>
    /// View model for MainWindow
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class AppViewModel : ReactiveObject, IScreen
    {
        public RoutingState Router { get; }
        private Window _currentWindow { get; }
        private static readonly Regex _progressRegex =
            new Regex(@"(\d+)% \((\d+)/(\d+)\)", RegexOptions.Compiled);

        // file items
        public ICommand QuitCommand { get; }

        // tools items
        public ICommand EditConfigCommand { get; }

        // help items
        public ICommand ViewHelpCommand { get; }
        public ICommand WhatsNewCommand { get; }
        public ICommand AboutCommand { get; }

        public ICommand InstallCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand UninstallCommand { get; }

        private ModManager _modmgr;
        public ModManager ModManager
        {
            get => _modmgr;
        }

        private bool _isInstallEnabled;
        public bool IsInstallEnabled
        {
            get => _isInstallEnabled;
            set => this.RaiseAndSetIfChanged(ref _isInstallEnabled, value);
        }

        private bool _isUpdateEnabled;
        public bool IsUpdateEnabled
        {
            get => _isUpdateEnabled;
            set => this.RaiseAndSetIfChanged(ref _isUpdateEnabled, value);
        }

        private bool _isUninstallEnabled;
        public bool IsUninstallEnabled
        {
            get => _isUninstallEnabled;
            set => this.RaiseAndSetIfChanged(ref _isUninstallEnabled, value);
        }

        public AppViewModel(Window win)
        {
            if (_modmgr == null)
            {
                _modmgr = new ModManager();
            }

            _currentWindow = win;

            Router = new RoutingState();

            // file items
            QuitCommand = ReactiveCommand.Create(QuitCommand_Execute);

            // tools items
            EditConfigCommand = ReactiveCommand.Create(EditConfigCommand_Execute);

            // help items
            ViewHelpCommand = ReactiveCommand.Create(ViewHelpCommand_Execute);
            WhatsNewCommand = ReactiveCommand.Create(WhatsNewCommand_Execute);
            AboutCommand = ReactiveCommand.Create(AboutCommand_Execute);

            InstallCommand = ReactiveCommand.Create(InstallCommand_Execute);
            UpdateCommand = ReactiveCommand.Create(UpdateCommand_Execute);
            UninstallCommand = ReactiveCommand.Create(UninstallCommand_Execute);
        }

        public void InstallMod()
        {
            var win = ThreadedWaitDialogWindow.ShowWaitDialog("install-progress-dlg", $"Installing {ClientDefs.GamePrettyName}",
            "Setting up the installation process...", true, false, true, _currentWindow);

            _modmgr.CheckoutProgressHandler += (path, completed, total) =>
            {
                double percentage = (double)completed / total * 100;
                int truncatedPercentage = (int)percentage; // Truncate the decimal part
                string msg = $"Updating files: {truncatedPercentage}% ({completed}/{total})";
                win.ChangeText(msg);
                win.ChangeProgressBar(truncatedPercentage, false, 0, 100);
            };

            _modmgr.TransferProgressHandler += (progress) =>
            {
                double percentage = (double)progress.ReceivedObjects / progress.TotalObjects * 100;
                int truncatedPercentage = (int)percentage; // Truncate the decimal part
                string msg = $"Received objects: {truncatedPercentage}% ({progress.ReceivedObjects}/{progress.TotalObjects})";
                win.ChangeText(msg);
                win.ChangeProgressBar(truncatedPercentage, false, 0, 100);

                return true;
            };

            _modmgr.ProgressHandler += (msg) =>
            {
                int percentage = 0;

                var match = _progressRegex.Match(msg);
                if (match.Success)
                {
                    percentage = int.Parse(match.Groups[1].Value);
                    int current = int.Parse(match.Groups[2].Value);
                    int total = int.Parse(match.Groups[3].Value);
                }

                win.ChangeText(msg);
                win.ChangeProgressBar(percentage, false, 0, 100);

                return true;
            };

            _modmgr.InstanceFinishedHandler += async (sender, e) =>
            {
                await _currentWindow.Dispatcher.InvokeAsync(() =>
                    MessageBox.Show(_currentWindow, "Installation completed!\n" +
                    $"Restart Steam to play {ClientDefs.GamePrettyName}.", Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyProductAttribute>()?.Product,
                    MessageBoxButton.OK, MessageBoxImage.Information)
                );

                win.CloseWaitDialog();

                _modmgr.ProgressHandler = null;
                _modmgr.TransferProgressHandler = null;
                _modmgr.InstanceFinishedHandler = null;
                _modmgr.InstanceErroredHandler = null;
            };

            _modmgr.InstanceErroredHandler += async (e) =>
            {
                await _currentWindow.Dispatcher.InvokeAsync(() =>
                    MessageBox.Show(_currentWindow, $"An error occured while trying to install {ClientDefs.GamePrettyName}:\n" + e.Message,
                    Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyProductAttribute>()?.Product, MessageBoxButton.OK, MessageBoxImage.Error)
                );

                _modmgr.InstanceErroredHandler = null;
            };

            _modmgr.Install();

            if (_currentWindow is MainWindow window)
            {
                window.UpdateInstallationStatus();
            }
        }

        public void UpdateMod()
        {
            var win = ThreadedWaitDialogWindow.ShowWaitDialog("update-progress-dlg", $"Updating {ClientDefs.GamePrettyName}",
            "Setting up for the update process...", true, false, true, _currentWindow);

            _modmgr.CheckoutProgressHandler += (path, completed, total) =>
            {
                double percentage = (double)completed / total * 100;
                int truncatedPercentage = (int)percentage; // Truncate the decimal part
                string msg = $"Updating files: {completed}/{total}";
                win.ChangeText(msg);
                win.ChangeProgressBar(truncatedPercentage, false, 0, 100);
            };

            _modmgr.TransferProgressHandler += (progress) =>
            {
                double percentage = (double)progress.ReceivedObjects / progress.TotalObjects * 100;
                int truncatedPercentage = (int)percentage; // Truncate the decimal part
                string msg = $"Received objects: {truncatedPercentage}% ({progress.ReceivedObjects}/{progress.TotalObjects})";
                win.ChangeText(msg);
                win.ChangeProgressBar(truncatedPercentage, false, 0, 100);

                return true;
            };

            _modmgr.ProgressHandler += (msg) =>
            {
                int percentage = 0;

                var match = _progressRegex.Match(msg);
                if (match.Success)
                {
                    percentage = int.Parse(match.Groups[1].Value);
                    int current = int.Parse(match.Groups[2].Value);
                    int total = int.Parse(match.Groups[3].Value);
                }

                win.ChangeText(msg);
                win.ChangeProgressBar(percentage, false, 0, 100);

                return true;
            };

            _modmgr.InstanceFinishedHandler += async (sender, e) =>
            {
                await _currentWindow.Dispatcher.InvokeAsync(() =>
                    MessageBox.Show(_currentWindow, "Update has been completed!", Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyProductAttribute>()?.Product,
                    MessageBoxButton.OK, MessageBoxImage.Information)
                );

                win.CloseWaitDialog();

                _modmgr.ProgressHandler = null;
                _modmgr.TransferProgressHandler = null;
                _modmgr.InstanceFinishedHandler = null;
                _modmgr.InstanceErroredHandler = null;
            };

            _modmgr.InstanceErroredHandler += async (e) =>
            {
                await _currentWindow.Dispatcher.InvokeAsync(() =>
                    MessageBox.Show(_currentWindow, $"An error occured while trying to update {ClientDefs.GamePrettyName}:\n" + e.Message,
                    Assembly.GetEntryAssembly()?
                            .GetCustomAttribute<AssemblyProductAttribute>()?
                            .Product,
                    MessageBoxButton.OK, MessageBoxImage.Error)
                );

                _modmgr.InstanceErroredHandler = null;
            };

            _modmgr.Update();

            if (_currentWindow is MainWindow window)
            {
                window.UpdateInstallationStatus();
            }
        }

        // command execution funcs

        /// <summary>
        /// Purpose: quits the app, thats it.
        /// </summary>
        private void QuitCommand_Execute()
        {
            _currentWindow.Close();
        }

        /// <summary>
        /// Purpose: quits the app, thats it.
        /// </summary>
        private void EditConfigCommand_Execute()
        {
            try
            {
                IDEWindow ide = new IDEWindow(Path.Combine(ClientDefs.ModInstallationPath, "cfg", "config.cfg"));
                ide.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error loading AridIDE", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Purpose: shows the about box, thats it.
        /// </summary>
        private void AboutCommand_Execute()
        {
            AboutWindow box = new AboutWindow();
            box.ShowDialog();
        }

        /// <summary>
        /// Purpose: goes to https://aridity.pages.dev/bf/bfclient-help, thats it.
        /// </summary>
        private void ViewHelpCommand_Execute()
        {
            using (Process urlStarter = new Process())
            {
                urlStarter.StartInfo.FileName = "https://aridity.pages.dev/bf/client-help";
                urlStarter.StartInfo.UseShellExecute = true;
                urlStarter.Start();
            }
        }

        /// <summary>
        /// Purpose: shows the whats new dialog, thats it.
        /// </summary>
        private void WhatsNewCommand_Execute()
        {
            WhatsNewWindow whatsNewWindow = new WhatsNewWindow();
            whatsNewWindow.ShowDialog();
        }

        /// <summary>
        /// Purpose: shows the whats new dialog, thats it.
        /// </summary>
        private void InstallCommand_Execute()
        {
            InstallMod();
        }

        /// <summary>
        /// Purpose: shows the whats new dialog, thats it.
        /// </summary>
        private void UpdateCommand_Execute()
        {
            UpdateMod();
        }

        /// <summary>
        /// Purpose: shows the whats new dialog, thats it.
        /// </summary>
        private async void UninstallCommand_Execute()
        {
            var msgbox = await _currentWindow.Dispatcher.InvokeAsync(() =>
                MessageBox.Show(_currentWindow,
                    "This may not work as expected because of an Git-related file is \"being used\" by nothing.\n" +
                    $"You must delete the \"{ClientDefs.ModInstallationPath}\" folder by yourself.\n" +
                    "\n" +
                    $"This process requires administrator permissions to uninstall {ClientDefs.GamePrettyName}.\n" +
                    "Do you want to continue?", Assembly.GetEntryAssembly()?
                                                .GetCustomAttribute<AssemblyProductAttribute>()?
                                                .Product, 
                    MessageBoxButton.YesNo, MessageBoxImage.Question)
            );
            if (msgbox != MessageBoxResult.Yes) return;

            _modmgr.Uninstall();
        }
    }
}
