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

namespace AridityTeam.Base
{
    public class ConCommand : IConCommand
    {
        private string? _name = null;
        private string? _helpString = null;
        private FCVAR? _flags = null;
        private ConCommandExecuteHandler? _executeAction = null;

        public ConCommand(string? name, ConCommandExecuteHandler? callback)
        {
            Create(name, callback, FCVAR.NONE, null);
        }

        public ConCommand(string? name, ConCommandExecuteHandler? callback, FCVAR? flags)
        {
            Create(name, callback, flags, null);
        }

        public ConCommand(string? name, ConCommandExecuteHandler? callback, FCVAR? flags, string? helpString)
        {
            Create(name, callback, flags, helpString);
        }

        private void Create(string? name, ConCommandExecuteHandler? callback, 
            FCVAR? flags, string? helpString)
        {
            _name = name;
            _executeAction = callback;
            _helpString = helpString;

            CommandManager.Instance.RegisterConCommand(this);
        }

        public FCVAR? GetFlags()
        {
            return _flags;
        }

        public string? GetHelpString() => _helpString;

        public void Execute(string? args)
        {
            Execute(this._executeAction, new ConCommandArgs()
            {
                AppliedArgs = args
            });
        }

        public void Execute(ConCommandExecuteHandler? callback, ConCommandArgs? args)
        {
            if (callback == null) throw new ArgumentNullException(nameof(callback), "Parameter is null!");
            if (args == null) throw new ArgumentNullException(nameof(args), "Parameter is null!");

            callback?.Invoke(args);
        }

        public string? GetName()
        {
            return _name;
        }

        public IConCommand GetConCommand()
        {
            return this;
        }
    }
}
