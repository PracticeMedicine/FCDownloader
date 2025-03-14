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
using Microsoft.Win32;
using System.IO;
using System.Security.Principal;

namespace AridityTeam.ElevatorProcess
{
    static class Program
    {
        private static AridLogger? _logger;
        private static string[]? _commandLineArgs;
        private static int _numOfArgsApplied = 0;

        [System.Runtime.InteropServices.DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool GetTokenInformation(
        IntPtr tokenHandle,
        int tokenInformationClass,
        out uint tokenInformation,
        int tokenInformationLength,
        out int returnLength);

        private const int TokenElevation = 20; // TokenElevation is the information class for elevation

        static bool IsTokenElevated(IntPtr tokenHandle)
        {
            uint tokenElevation;
            int returnLength;

            // Get the elevation information from the token
            if (GetTokenInformation(tokenHandle, TokenElevation, out tokenElevation, sizeof(uint), out returnLength))
                return tokenElevation != 0; // Non-zero means the token is elevated

            return false; // Failed to get token information
        }

        public static int Main(string[]? args)
        {
            _logger = new AridLogger(typeof(Program));
            _commandLineArgs = args;

            try
            {
                if (_commandLineArgs != null)
                {
                    foreach (string? arg in _commandLineArgs)
                    {
                        _numOfArgsApplied++;
                        _logger.Info("Arg #{0}: {1}", _numOfArgsApplied, arg);
                    }
                }

                static bool IsAdministrator()
                {
                    WindowsIdentity identity = WindowsIdentity.GetCurrent();
                    WindowsPrincipal principal = new WindowsPrincipal(identity);

                    // Check if the user is in the Administrators group
                    if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
                        return false; // Not an administrator

                    // Check if the process is elevated
                    IntPtr tokenHandle = identity.Token;
                    if (tokenHandle == IntPtr.Zero)
                        return false; // No token available

                    // Use the Windows API to check if the token is elevated
                    return IsTokenElevated(tokenHandle);
                }

                static bool FindParm(string? parm)
                {
                    if (_commandLineArgs != null)
                    {
                        foreach (string? arg in _commandLineArgs)
                        {
                            if (arg.Equals(parm)) return true;
                        }
                    }

                    return false;
                }

                if (!IsAdministrator())
                    throw new MethodAccessException("Tried to run elevation process as user!");

                if (FindParm("/modManager"))
                {
                    _logger.Info("In mod manager mode...");

                    RegistryKey? reg = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\AridityTeam\BFClient");

                    if (reg == null)
                        throw new OperationCanceledException("Operation canceled, because of an registry key missing.");

                    string? installDir = reg.GetValue("ModInstallationDir")?.ToString();

                    if (installDir == null)
                        throw new NullReferenceException("Registry key is set as null.");

                    if (!Directory.Exists(installDir))
                        throw new DirectoryNotFoundException("Could not find the installation directory.");

                    if (FindParm("/uninstall"))
                    {
                        _logger.Info("Uninstalling Beta Fortress...");
                        Directory.Delete(installDir, true);
                    }
                }
                else
                {
                    throw new ApplicationException("Invalid arguments applied!");
                }
            }
            catch (Exception ex)
            {
                _logger.Err("An error occured: {0}", ex.Message);
                return 1;
            }

            return 0;
        }
    }
}
