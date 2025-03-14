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
using System.Reflection;
using System.Runtime.Versioning;
using System.Windows;
using ReactiveUI;
using Microsoft.Toolkit.Uwp.Notifications;

using AridityTeam.Base.Util;
using AridityTeam.BetaFortressSetup.Gui.ViewModels;
using AridityTeam.BetaFortressSetup.Util;

#pragma warning disable 8602

namespace AridityTeam.BetaFortressSetup.Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [SupportedOSPlatform("windows")]
    public partial class MainWindow : ReactiveWindow<AppViewModel>
    {
        private AridLogger? _logger = null;
        private ModManager? _modmgr = null;
        private bool _alreadyShownUpdateError = false;
        private bool _alreadyShownNewUpdateMsg = false;
        private string? _assemblyProductName = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyProductAttribute>().Product;

        public MainWindow()
        {
            var loadingWinDlg = ThreadedWaitDialogWindow.ShowWaitDialog("loading-mainwin-wait-dlg", "Loading the main window...",
                "Loading main window...", true, true, true);

            if (_logger == null)
            {
                loadingWinDlg.ChangeText("Creating logger...");
                _logger = new AridLogger(typeof(MainWindow));
            }

            loadingWinDlg.ChangeText("Subscribing to Toast notification events...");
            ToastNotificationManagerCompat.OnActivated += ToastNotification_OnActivated;

            loadingWinDlg.ChangeText("Creating window...");
            InitializeComponent();
            this.Title = _assemblyProductName;
            this.QuitItem.Header = $"Quit {_assemblyProductName}";
            this.AboutItem.Header = $"About {_assemblyProductName}";
            this.ViewModel = new AppViewModel(this);

            _modmgr = this.ViewModel.ModManager;

            loadingWinDlg.ChangeText("Activating");

            this.WhenActivated(d =>
            {
                _logger.Info("Activated.");

                loadingWinDlg.ChangeText("Fetching installation status...");
                _modmgr.CheckForUpdates();

                loadingWinDlg.ChangeText("Binding commands...");

                // file commanmds
                d(this.BindCommand(this.ViewModel, vm => vm.QuitCommand, win => win.QuitItem));

                // tools commands
                d(this.BindCommand(this.ViewModel, vm => vm.EditConfigCommand, win => win.EditBFConfigItem));

                // help commands
                d(this.BindCommand(this.ViewModel, vm => vm.ViewHelpCommand, win => win.ViewHelpItem));
                d(this.BindCommand(this.ViewModel, vm => vm.WhatsNewCommand, win => win.WhatsNewItem));
                d(this.BindCommand(this.ViewModel, vm => vm.AboutCommand, win => win.AboutItem));

                d(this.BindCommand(this.ViewModel, vm => vm.InstallCommand, win => win.BtnInstall));
                d(this.BindCommand(this.ViewModel, vm => vm.UpdateCommand, win => win.BtnUpdate));
                d(this.BindCommand(this.ViewModel, vm => vm.UninstallCommand, win => win.BtnUninstall));

                d(this.Bind(this.ViewModel,
                    vm => vm.IsInstallEnabled,
                    v => v.BtnInstall.IsEnabled));
                d(this.Bind(this.ViewModel,
                    vm => vm.IsUpdateEnabled,
                    v => v.BtnUpdate.IsEnabled));
                d(this.Bind(this.ViewModel,
                    vm => vm.IsUninstallEnabled,
                    v => v.BtnUninstall.IsEnabled));

                loadingWinDlg.ChangeText("Setting element configurations...");
                this.WelcomeLabel.Content = $"Welcome to {Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyProductAttribute>().Product}!";

                this.ViewModel.IsInstallEnabled = false;
                this.ViewModel.IsUpdateEnabled = false;
                this.ViewModel.IsUninstallEnabled = false;

                UpdateInstallationStatus();

                HeartbeatInstance checkForUpdatesInstance = new HeartbeatInstance()
                {
                    InstanceName = "Installation Status Updater",
                    HeartbeatTime = 5250,
                    ActionToRun = () =>
                    {
                        this.Dispatcher.Invoke(() => UpdateInstallationStatus());
                    },
                };

                HeartbeatManager.Instance.AddInstance(checkForUpdatesInstance);

                loadingWinDlg.CloseWaitDialog();
            });
        }

        private void ToastNotification_OnActivated(ToastNotificationActivatedEventArgsCompat e)
        {
            ToastArguments? args = ToastArguments.Parse(e.Argument);
            _logger.Info("Toast arguments: {0}", e.Argument);

            if (e.Argument.Contains("pls-update-idot"))
            {
                _logger.Info("updating...");
                this.ViewModel.UpdateMod();
            }

            if (!this.IsVisible)
            {
                this.Show();
            }

            if (this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
            }

            this.Activate();
            this.Topmost = true;  // important
            this.Topmost = false; // important
            this.Focus();         // important
        }

        private void ShowNewUpdateNotification()
        {
            if (this.IsFocused != true)
            {
                new ToastContentBuilder()
                .AddText($"New update for {ClientDefs.GamePrettyName} available!")
                .AddText("A new update has been pushed to the remote!")
                .AddText($"Commit Message: {_modmgr.GetNextUpdateMsg()}")
                .AddButton(new ToastButton("Update", "pls-update-idot"))
                .AddButton(new ToastButton("Dismiss", "pls-no-update-idot"))
                .Show();
            }
            else
            {
                MessageBox.Show("A new update has been pushed to the remote!\n" +
                    $"Commit Message: {_modmgr.GetNextUpdateMsg()}",
                    Assembly.GetEntryAssembly()?
                            .GetCustomAttribute<AssemblyProductAttribute>()?
                            .Product, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public async void UpdateInstallationStatus()
        {
            _logger.Debug("Updating installation information...");

            _modmgr.CheckForUpdates();

            if (_modmgr.GetInstallationStatus() != EModInstallationStatus.CurrentlyInstalled)
            {
                if (_modmgr.GetInstallationStatus() == EModInstallationStatus.OutOfDate)
                {
                    if (_alreadyShownNewUpdateMsg != true)
                    {
                        _alreadyShownNewUpdateMsg = true;

                        ShowNewUpdateNotification();
                    }

                    this.ViewModel.IsInstallEnabled = false;
                    this.ViewModel.IsUpdateEnabled = true;
                    this.ViewModel.IsUninstallEnabled = true;
                }
                else if (_modmgr.GetInstallationStatus() == EModInstallationStatus.MissingFiles)
                {
                    await this.Dispatcher.InvokeAsync(() =>
                        MessageBox.Show("Game files are missing, please reinstall the game!",
                        Assembly.GetEntryAssembly()?
                                                .GetCustomAttribute<AssemblyProductAttribute>()?
                                                .Product,
                        MessageBoxButton.OK, MessageBoxImage.Hand)
                    );

                    this.ViewModel.IsInstallEnabled = false;
                    this.ViewModel.IsUpdateEnabled = false;
                    this.ViewModel.IsUninstallEnabled = true;
                }
                else if (_modmgr.GetInstallationStatus() == EModInstallationStatus.Errored)
                {
                    if (_alreadyShownUpdateError != true)
                    {
                        _alreadyShownUpdateError = true;
                        MessageBox.Show(
                            string.Format("An error occured while updating installation status:\n{0}\n\n" +
                            "Repository to fetch: {1}", 
                            _modmgr.GetLastException().Message, ClientDefs.RepoUri),
                            Assembly.GetEntryAssembly()?
                                                .GetCustomAttribute<AssemblyProductAttribute>()?
                                                .Product,
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        Application.Current.Shutdown(_modmgr.GetLastException().HResult);
                    }

                    this.ViewModel.IsInstallEnabled = false;
                    this.ViewModel.IsUpdateEnabled = false;
                    this.ViewModel.IsUninstallEnabled = true;
                }
                else
                {
                    // considered uninstalled
                    this.ViewModel.IsInstallEnabled = true;
                    this.ViewModel.IsUpdateEnabled = false;
                    this.ViewModel.IsUninstallEnabled = false;
                }
            }
            else
            {
                this.ViewModel.IsInstallEnabled = false;
                this.ViewModel.IsUpdateEnabled = false;
                this.ViewModel.IsUninstallEnabled = true;
            }
        }

        private void ReactiveWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            HeartbeatManager.Instance.Dispose();
            _modmgr.Dispose();
            _logger = null;
        }
    }
}
