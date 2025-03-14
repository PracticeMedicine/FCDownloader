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
using System.Xml.Linq;

namespace AridityTeam.Base
{
    public class ConVar : IConVar
    {
        public object? _value = null;
        public string? _name = null;
        public FCVAR? _flags = null;
        public string? _helpString = null;
        private bool _isRegistered = false;

        public ConVar(string? name, object? defaultValue)
        {
            Create(name, defaultValue, FCVAR.NONE, null);
        }

        public ConVar(string? name, object? defaultValue, FCVAR? flags)
        {
            Create(name, defaultValue, flags, null);
        }

        public ConVar(string? name, object? defaultValue, FCVAR? flags, string? helpString)
        {
            Create(name, defaultValue, flags, helpString);
        }

        ~ConVar()
        {
            if (_value != null)
            {
                _value = null;
            }
        }

        public FCVAR? GetFlags()
        {
            return _flags;
        }

        private void Create(string? name, object? defaultValue, FCVAR? flags = FCVAR.NONE, string? helpString = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name), "Name cannot be null or empty.");
            if (defaultValue == null) throw new ArgumentNullException(nameof(defaultValue), "Default value cannot be null.");

            _name = name;
            _value = defaultValue;
            _flags = flags;
            _helpString = helpString;

            CommandManager.Instance.RegisterConVar(this);
        }

        public bool IsRegistered
        {
            get => _isRegistered;
            set => _isRegistered = value;
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
            return long.TryParse(ToString(), out var result) ? result : 0; ;
        }

        public bool GetBool()
        {
            return GetInt() >= 1;
        }

        public string? GetString()
        {
            if (_value == null) return null;

            return _value.ToString();
        }
    }
}
