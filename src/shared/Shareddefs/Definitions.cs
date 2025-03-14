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
using AridityTeam.Shareddefs;
using System;
using System.IO;
using System.Runtime.Versioning;

namespace AridityTeam.BetaFortressSetup
{
    /// <summary>
    /// Contains most of the definitions to be edited to your preferences.
    /// This client installer still only supports Git for now.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class ClientDefs
    {
        // assembly prefrences
        public const string ProductTitle = "FortressInstaller";
        public const string ProductDesc = "An open-source Git powered installer utility.";
        public const string ProductCompany = "The Aridity Team";
        public const string ProductCopyright = $"Copyright (c) 2025 {ProductCompany}, all rights reserved.";
        public const string ProductVersion = $"0.0.131.0";

        // general preferences
        public static string GameName = "bf";
        public static string GamePrettyName = "Beta Fortress";
        public static string CopyrightText = $"Copyright (c) 2025 {ProductCompany}, all rights reserved."; // to be displayed in the about box

        // installation preferences
        public static string ModInstallationPath = Steam.GetSourceModsPath + "/" + GameName;

        public static string[] DirectoryNeeded =
        {
            Path.Combine(ModInstallationPath, "bin"),
            Path.Combine(ModInstallationPath, "resource"),
            Path.Combine(ModInstallationPath, "scripts"),
            Path.Combine(ModInstallationPath, "vpks"),
        };
        public static string[] FilesNeeded =
        {
            Path.Combine(ModInstallationPath, "bin/client.dll"),
            Path.Combine(ModInstallationPath, "bin/server.dll"),
            Path.Combine(ModInstallationPath, "bin/gameinfo.txt"),
        };

        // crashreporter prefrences
        public static bool EnableSentryCrashReporter = false;
        public static string SentryDSNLink = "";
        public static string SentryEnvironment =
#if DEBUG
            "development"
#else
            "production"
#endif // DEBUG
            ; // SentryEnvironment
        public static bool SentryEnableDebugLogging =
#if DEBUG
            true
#else
            false // its up to u to whether enable debug logging in Release.
#endif
            ; // SentryEnableDebugLogging

        // git preferences
        public static string RepoUri = "https://github.com/AridityTeam/bf";
        public static string RepoStableBranchName = "main";
        public static bool IsAGitRepository = true;

        // credential prefrences
        public static string GitAccUsername = "";
        public static string GitAccPassword = "";
        public static string GitAccToken = "";

        // database prefrences
        public static string InstalledDatabaseConfigPath = "";
    }
}