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
using System.Diagnostics;
using System.IO;
using System.Runtime.Versioning;
using Sentry;
using Microsoft.Win32;
using System.Windows.Forms;

using AridityTeam.BetaFortressSetup.Installer;
using AridityTeam.BetaFortressSetup.Util;
using AridityTeam.BetaFortressSetup.Gui;

namespace AridityTeam.BetaFortressSetup.Startup
{
    [SupportedOSPlatform("windows")]
    internal class SetupInstallerMain
    {
        private static ConsoleWindow? _consoleWin = null;
        private static bool _devMode = false;
        static AridLogger? logger = null;

        public static bool DevMode => _devMode;

        public static void HandleExeception(Exception ex, bool showMsgBox = true)
        {
#if false
            if(ex.GetType() == typeof(FileNotFoundException))
            {
                MessageBox.Show(
                    "Could not load library client. Try restarting. If that doesn't work, go to the AridInstaller setup and click on Repair.", 
                    "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
#endif
            {
                if (showMsgBox == true)
                {
                    MessageBox.Show($"An error occured:\n" +
                    $"{ex.Message}", "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            SentrySdk.CaptureException(ex);
        }

        [STAThread]
        public static void Main(string[] args)
        {
            _consoleWin = new ConsoleWindow();
            logger = new AridLogger(typeof(SetupInstallerMain));
            logger.Info("Starting up...");

            //MemoryUtil.CheckForMemoryLeaks(150);
            PresentationTextWriter _conWriter = new PresentationTextWriter(_consoleWin.OutputText, _consoleWin);
            Console.SetOut(new MultiTextWriter(_conWriter, Console.Out));
            Console.SetError(new MultiTextWriter(_conWriter, Console.Error));

            try
            {
                foreach(string parm in args)
                {
                    if (parm.Contains("/dev", StringComparison.OrdinalIgnoreCase))
                        _devMode = true;    
                    if (parm.Contains("/console", StringComparison.OrdinalIgnoreCase))
                        _consoleWin.Show();
                }

                Run();
            }
            catch (Exception ex)
            {
                HandleExeception(ex);
            }
        }

        private static void Run()
        {
            try
            {
                RegistryKey? reg = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\AridityTeam\BFClient", true);
                if (reg == null) reg = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\AridityTeam\BFClient", true);

                if (reg.GetValue("InstallerInstallationDir") == null)
                    reg.SetValue("InstallerInstallationDir", AppDomain.CurrentDomain.BaseDirectory);

                if (reg.GetValue("ModInstallationDir") == null)
                    if (Directory.Exists(ClientDefs.ModInstallationPath))
                        reg.SetValue("ModInstallationDir", ClientDefs.ModInstallationPath);
                
                RunApplication();
            }
            finally
            {
                if (_consoleWin != null)
                    _consoleWin.Close();

                logger?.Debug("Leaving Run()...");
            }
        }

        private static void RunApplication()
        {
            try
            {
                InstallerStartup startup = new InstallerStartup();
                startup.InitializeInstallerApp();
            }
            finally
            {
                logger?.Debug("Leaving RunApplication()...");
            }
        }
    }
}
