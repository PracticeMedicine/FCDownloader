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
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Interop;

using AridityTeam.BetaFortressSetup.Startup;
using AridityTeam.BetaFortressSetup.Gui;
using AridityTeam.BetaFortressSetup.Util;

namespace AridityTeam.BetaFortressSetup.Installer
{
    [SupportedOSPlatform("windows")]
    public class InstallerStartup
    {
        App? app;

        AridLogger? logger;

        public void InitializeInstallerApp()
        {
            app = new App();

            System.Windows.Forms.Integration.WindowsFormsHost.EnableWindowsFormsInterop();
            ComponentDispatcher.ThreadIdle -= ComponentDispatcher_ThreadIdle; // ensure we don't register twice
            ComponentDispatcher.ThreadIdle += ComponentDispatcher_ThreadIdle;
            InitializeInstallerApp(new MainWindow());
        }

        private void InitializeInstallerApp(Window window)
        {
            // initialize the logger if we havent yet
            if (logger == null)
            {
                logger = new AridLogger(typeof(InstallerStartup));
            }

            // initialize the app constructor if we havent yet
            if (app == null) app = new App();

            DatabaseManager.DatabaseManager.Instance.ExceptionHandler = (ex) =>
            {
                SetupInstallerMain.HandleExeception(ex, false);
            };

            DatabaseManager.DatabaseManager.Instance.Init();

            try
            {
                app.Exit += App_Exit;
                logger.Info("Starting application user interface...");
                app.Run(window);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format("An error occured while performing an operation in the user interface:\n{0}", ex.Message),
                    "Application Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
        }

        static void ComponentDispatcher_ThreadIdle(object? sender, EventArgs e)
        {
            System.Windows.Forms.Application.RaiseIdle(e);
        }
    }
}
