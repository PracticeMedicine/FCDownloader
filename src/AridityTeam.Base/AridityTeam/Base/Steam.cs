﻿/*
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
using System.IO;
using System.Diagnostics;
using System.Runtime.Versioning;
using Microsoft.Win32;

namespace AridityTeam.Base
{
    /// <summary>
    /// Beta Fortress Team's own implementation of the Steam class like from TF2CLauncher
    /// DO NOT CHANGE UNLESS YOU KNOW WHAT YOUR DOING
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static class Steam
    {
        /// <summary>
        /// Returns a value where the Steam client was installed
        /// </summary>
        public static string? GetSteamPath
        {
            get
            {
                using (RegistryKey? key = RegistryUtil.OpenSubKey(Registry.CurrentUser, @"SOFTWARE\Valve\Steam"))
                {
                    if (key != null)
                    {
                        // make sure the path actually exists before returning the value
                        if (Directory.Exists(RegistryUtil.GetString(key, "SteamPath")))
                        {
                            return RegistryUtil.GetString(key, "SteamPath");
                        }
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Returns a value where the "sourcemods" directory is
        /// </summary>
        public static string? GetSourceModsPath
        {
            get
            {
                using (RegistryKey? key = RegistryUtil.OpenSubKey(Registry.CurrentUser, @"SOFTWARE\Valve\Steam"))
                {
                    if (key != null)
                    {
                        // make sure the path actually exists before returning the value
                        if (Directory.Exists(RegistryUtil.GetString(key, "SourceModInstallPath")))
                        {
                            return RegistryUtil.GetString(key, "SourceModInstallPath");
                        }
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Returns the "Steam/steamapps/common" directory if it exists
        /// </summary>
        public static string? GetSteamAppsPath
        {
            get
            {
                if (GetSteamPath == null)
                    return null;

                string? _path = Path.Combine(GetSteamPath, "steamapps", "common");
                if (Directory.Exists(_path))
                {
                    return _path;
                }
                return null;
            }
        }

        /// <summary>
        /// Checks if Steam is installed by checking if the registry keys for Steam exists or checking if the Steam installation directory exists
        /// </summary>
        public static bool IsSteamInstalled
        {
            get
            {
                return Directory.Exists(GetSteamPath);
            }
        }

        /// <summary>
        /// Checks if the specific app ID is installed
        /// (ex.: 220 is HL2, 240 is CS:S, 440 is TF2)
        /// </summary>
        /// <param name="appId">Steam application to check</param>
        /// <returns>Returns true if it is installed.</returns>
        public static bool IsAppInstalled(int appId)
        {
            using (RegistryKey? key = RegistryUtil.OpenSubKey(Registry.CurrentUser, @"SOFTWARE\Valve\Steam\Apps\" + appId))
            {
                if (key != null)
                {
                    // make sure the path actually exists before returning the value
                    return RegistryUtil.GetBool(key, "Installed");
                }
            }
            return false;
        }

        /// <summary>
        /// **WARNING!!!**
        /// not implemented in linux
        /// Checks if the specific app ID is updating
        /// Only useful in some cases
        /// </summary>
        /// <param name="appId">Steam application ID to check</param>
        /// <returns>Returns true if it is updating.</returns>
        public static bool IsAppUpdating(int appId)
        {
            using (RegistryKey? key = RegistryUtil.OpenSubKey(Registry.CurrentUser, StringUtil.CombineString(@"SOFTWARE\Valve\Steam\Apps\", appId)))
            {
                if (key != null)
                {
                    // make sure the path actually exists before returning the value
                    return RegistryUtil.GetBool(key, "Updating");
                }
            }
            return false;
        }

        // note -- for source engine games: use a mutex instead but this is another workaround
        /// <summary>
        /// Checks if the specific app ID is running
        /// NOTE: If the game/software has a mutex, you can use the Mutex class as an better alternative.
        /// </summary>
        /// <param name="appId">Steam application ID to check</param>
        /// <returns>Returns true if it is running.</returns>
        public static bool IsAppRunning(int appId)
        {
            using (RegistryKey? key = RegistryUtil.OpenSubKey(Registry.CurrentUser, StringUtil.CombineString(@"SOFTWARE\Valve\Steam", appId)))
            {
                if (key != null)
                {
                    return RegistryUtil.GetString(key, "RunningAppID") == appId.ToString();
                }
            }
            return false;
        }

        /// <summary>
        /// Runs a specific app ID
        /// </summary>
        /// <param name="appId">Steam Application (ID) to launch</param>
        public static void RunApp(int appId)
        {
            using (Process p = new Process())
            {
                p.StartInfo.FileName = GetSteamPath + "/steam.exe";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.Arguments = "-applaunch " + appId;
                p.Start();
            }
        }

        /// <summary>
        /// Runs a specific app ID with extra launch options
        /// </summary>
        /// <param name="appId">Steam Application (ID) to launch</param>
        /// <param name="args">Extra arguments to launch with</param>
        public static void RunApp(int appId, string args)
        {
            using (Process p = new Process())
            {
                p.StartInfo.FileName = GetSteamPath + "/steam.exe";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.Arguments = "-applaunch " + appId + " " + args;
                p.Start();
            }
        }
    }
}
