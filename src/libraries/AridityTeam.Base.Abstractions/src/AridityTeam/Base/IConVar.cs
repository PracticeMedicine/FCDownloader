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

namespace AridityTeam.Base
{
    /// <summary>
    /// Implements the <see cref="ConVar"/> class. <para/>
    /// Do not inherit this class to make a <see cref="ConVar"/>. Instead just use the actual
    /// <see cref="ConVar"/> class as the base of your ConVar.
    /// </summary>
    public interface IConVar : IConCommandBase
    {
        /// <summary>
        /// Sets the <see cref="ConVar"/>'s value.
        /// </summary>
        /// <param name="value">New value of the <see cref="ConVar"/></param>
        public void SetValue(object? value);

        /// <summary>
        /// Gets the help string.
        /// </summary>
        /// <returns></returns>
        public string? GetHelpString();

        /// <summary>
        /// Return the name of the <see cref="ConVar"/>.
        /// </summary>
        /// <returns>Returns the name of the <see cref="ConVar"/>.</returns>
        public string? GetName();

        /// <summary>
        /// Return the <see cref="ConVar"/> value as a integral.
        /// </summary>
        /// <returns>Returns the integral value.</returns>
        public int GetInt();

        /// <summary>
        /// Return the <see cref="ConVar"/> value as a float.
        /// </summary>
        /// <returns>Returns the float value.</returns>
        public float GetFloat();

        /// <summary>
        /// Return the <see cref="ConVar"/> value as a boolean.
        /// </summary>
        /// <returns>Returns true if the <see cref="ConVar"/> is set to or higher than 1.</returns>
        public bool GetBool();

        /// <summary>
        /// Returns the <see cref="ConVar"/> value as a string.
        /// </summary>
        /// <returns>Returns the actual <see cref="ConVar"/> value.</returns>
        public string? GetString();
    }
}
