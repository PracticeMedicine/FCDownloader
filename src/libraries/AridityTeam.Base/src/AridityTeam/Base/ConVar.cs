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
    public abstract class ConVar : IConVar
    {
        private object? _value;
        private string? _name;
        private Fcvar? _flags;
        private string? _helpString;

        protected ConVar(string? name, object? defaultValue)
        {
            Create(name, defaultValue, Fcvar.LogNone, null);
        }

        protected ConVar(string? name, object? defaultValue, Fcvar? flags)
        {
            Create(name, defaultValue, flags, null);
        }

        protected ConVar(string? name, object? defaultValue, Fcvar? flags, string? helpString)
        {
            Create(name, defaultValue, flags, helpString);
        }

        ~ConVar()
        {
            if (_value is not null)
            {
                _value = null;
            }
        }

        public Fcvar? GetFlags()
        {
            return _flags;
        }

        private void Create(string? name, object? defaultValue, Fcvar? flags, string? helpString)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name), "Name cannot be null or empty.");

            _name = name;
            _value = defaultValue ?? throw new ArgumentNullException(nameof(defaultValue), "Default value cannot be null.");
            _flags = flags;
            _helpString = helpString;

            CommandManager.Instance.RegisterConVar(this);
        }

        public string? GetName() => _name;
        public string? GetHelpString() => _helpString;

        public void SetValue(object? value)
        {
            _value = value;
        }

        public int GetInt()
        {
            return int.TryParse(ToString(), out var result) ? result : 0;
        }

        public float GetFloat()
        {
            return float.TryParse(ToString(), out var result) ? result : 0.0f;
        }

        public double GetDouble()
        {
            return double.TryParse(ToString(), out var result) ? result : 0.0;
        }

        public long GetLong()
        {
            return long.TryParse(ToString(), out var result) ? result : 0;
        }

        public bool GetBool()
        {
            return GetInt() >= 1;
        }

        public string? GetString()
        {
            return _value?.ToString();
        }
    }
}
