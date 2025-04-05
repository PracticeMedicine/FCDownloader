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
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using System;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Runtime.Versioning;
using Microsoft.Win32;

namespace AridityTeam.Base.Util
{
    /// <summary>
    /// Beta Fortress Team's own implementation of the Steam class like from TF2CLauncher
    /// DO NOT CHANGE UNLESS YOU KNOW WHAT YOUR DOING
    /// </summary>
    public static class Steam
    {
        /// <summary>
        /// Find path to sourcemod folder.
        /// </summary>
        [SupportedOSPlatform("linux")]
        public static string? GetSourceModsPathLinux()
        {
            try
            {
                string? path = null;
                // Expand the user home directory for the file path.
                var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    ".steam", "registry.vdf");
                using var file = new StreamReader(filePath, Encoding.UTF8);
                // Read each line in the file.
                while (file.ReadLine() is { } line)
                {
                    if (!line.Contains("SourceModInstallPath")) continue;
                    var startIndex = line.IndexOf("/home", StringComparison.OrdinalIgnoreCase);
                    if (startIndex >= 0 && startIndex < line.Length)
                    {
                        // Extract substring starting at "/home" up to the last character.
                        // In the original Python code, [line.index('/home'):-1] is used.
                        // Since StreamReader.ReadLine() removes newline characters, we take the substring from startIndex to the end.
                        path = line[startIndex..].Replace(@"\\", "/");
                    }

                    break;
                }

                return path;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Returns a value where the Steam client was installed
        /// </summary>
        [SupportedOSPlatform("windows")]
        private static string? GetSteamPath
        {
            get
            {
                using var key = RegistryUtil.OpenSubKey(Registry.CurrentUser, @"SOFTWARE\Valve\Steam");
                if (key == null) return null;
                // make sure the path actually exists before returning the value
                return Directory.Exists(RegistryUtil.GetString(key, "SteamPath")) ? RegistryUtil.GetString(key, "SteamPath") : null;
            }
        }

        /// <summary>
        /// Returns a value where the "sourcemods" directory is
        /// </summary>
        [SupportedOSPlatform("windows")]
        public static string? GetSourceModsPath
        {
            get
            {
                using var key = RegistryUtil.OpenSubKey(Registry.CurrentUser, @"SOFTWARE\Valve\Steam");
                if (key != null)
                {
                    // make sure the path actually exists before returning the value
                    if (Directory.Exists(RegistryUtil.GetString(key, "SourceModInstallPath")))
                    {
                        return RegistryUtil.GetString(key, "SourceModInstallPath");
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Returns the "Steam/steamapps/common" directory if it exists
        /// </summary>
        [SupportedOSPlatform("windows")]
        public static string? GetSteamAppsPath
        {
            get
            {
                if (GetSteamPath == null)
                    return null;

                var path = Path.Combine(GetSteamPath, "steamapps", "common");
                return Directory.Exists(path) ? path : null;
            }
        }

        /// <summary>
        /// Checks if Steam is installed by checking if the registry keys for Steam exists or checking if the Steam installation directory exists
        /// </summary>
        [SupportedOSPlatform("windows")]
        public static bool IsSteamInstalled => Directory.Exists(GetSteamPath);

        /// <summary>
        /// Checks if the specific app ID is installed
        /// (ex.: 220 is HL2, 240 is CS:S, 440 is TF2)
        /// </summary>
        /// <param name="appId">Steam application to check</param>
        /// <returns>Returns true if it is installed.</returns>
        [SupportedOSPlatform("windows")]
        public static bool IsAppInstalled(int appId)
        {
            using var key = RegistryUtil.OpenSubKey(Registry.CurrentUser, @"SOFTWARE\Valve\Steam\Apps\" + appId);
            return key != null &&
                   // make sure the path actually exists before returning the value
                   RegistryUtil.GetBool(key, "Installed");
        }

        /// <summary>
        /// **WARNING!!!**
        /// not implemented in linux
        /// Checks if the specific app ID is updating
        /// Only useful in some cases
        /// </summary>
        /// <param name="appId">Steam application ID to check</param>
        /// <returns>Returns true if it is updating.</returns>
        [SupportedOSPlatform("windows")]
        public static bool IsAppUpdating(int appId)
        {
            using var key = RegistryUtil.OpenSubKey(Registry.CurrentUser, StringUtil.CombineString(@"SOFTWARE\Valve\Steam\Apps\", appId));
            return key != null &&
                   // make sure the path actually exists before returning the value
                   RegistryUtil.GetBool(key, "Updating");
        }

        // note -- for source engine games: use a mutex instead but this is another workaround
        /// <summary>
        /// Checks if the specific app ID is running
        /// NOTE: If the game/software has a mutex, you can use the Mutex class as a better alternative.
        /// </summary>
        /// <param name="appId">Steam application ID to check</param>
        /// <returns>Returns true if it is running.</returns>
        [SupportedOSPlatform("windows")]
        public static bool IsAppRunning(int appId)
        {
            using var key = RegistryUtil.OpenSubKey(Registry.CurrentUser, StringUtil.CombineString(@"SOFTWARE\Valve\Steam", appId));
            if (key != null)
            {
                return RegistryUtil.GetString(key, "RunningAppID") == appId.ToString();
            }

            return false;
        }

        /// <summary>
        /// Runs a specific app ID
        /// </summary>
        /// <param name="appId">Steam Application (ID) to launch</param>
        [SupportedOSPlatform("windows")]
        public static void RunApp(int appId)
        {
            using var p = new Process();
            p.StartInfo.FileName = GetSteamPath + "/steam.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.Arguments = "-applaunch " + appId;
            p.Start();
        }

        /// <summary>
        /// Runs a specific app ID with extra launch options
        /// </summary>
        /// <param name="appId">Steam Application (ID) to launch</param>
        /// <param name="args">Extra arguments to launch with</param>
        [SupportedOSPlatform("windows")]
        public static void RunApp(int appId, string args)
        {
            using var p = new Process();
            p.StartInfo.FileName = GetSteamPath + "/steam.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.Arguments = "-applaunch " + appId + " " + args;
            p.Start();
        }
    }
}
