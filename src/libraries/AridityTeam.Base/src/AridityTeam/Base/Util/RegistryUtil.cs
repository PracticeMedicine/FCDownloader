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
using System.Runtime.Versioning;
using Microsoft.Win32;

namespace AridityTeam.Base.Util
{
    /// <summary>
    /// Wrapper for Registry functions.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static class RegistryUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="subKey"></param>
        /// <returns></returns>
        public static RegistryKey? CreateSubKey(RegistryKey? key, string? subKey)
        {
            if (key == null)
                return null;

            return subKey == null ? null : key.CreateSubKey(subKey, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="subKey"></param>
        /// <returns></returns>
        public static RegistryKey? OpenSubKey(RegistryKey? key, string? subKey)
        {
            if (key == null)
                return null;

            return subKey == null ? null : key.OpenSubKey(subKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="regKey"></param>
        /// <param name="valueName"></param>
        /// <returns></returns>
        public static int? GetInt(RegistryKey? regKey, string? valueName)
        {
            var key = regKey;

            var keyValue = key?.GetValue(valueName);

            return (int?)keyValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="regKey"></param>
        /// <param name="valueName"></param>
        /// <returns></returns>
        public static string? GetString(RegistryKey? regKey, string? valueName)
        {
            var key = regKey;

            var keyValue = key?.GetValue(valueName);

            return keyValue?.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="regKey"></param>
        /// <param name="valueName"></param>
        /// <returns></returns>
        public static bool GetBool(RegistryKey? regKey, string? valueName)
        {
            var key = regKey;
            return key?.GetValue(valueName) != null && key.GetValue(valueName)?.Equals(1) == true;
        }
    }
}
