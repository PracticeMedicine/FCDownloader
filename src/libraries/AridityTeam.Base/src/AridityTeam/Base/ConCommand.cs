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
using AridityTeam.Base.Util;

namespace AridityTeam.Base
{
    public class ConCommand : IConCommand
    {
        private string? _name;
        private string? _helpString;
        private readonly Fcvar? _flags = null;
        private ConCommandExecuteHandler? _executeAction;

        public ConCommand(string? name, ConCommandExecuteHandler? callback)
        {
            Create(name, callback, Fcvar.LogNone, null);
        }

        public ConCommand(string? name, ConCommandExecuteHandler? callback, Fcvar? flags)
        {
            Create(name, callback, flags, null);
        }

        public ConCommand(string? name, ConCommandExecuteHandler? callback, Fcvar? flags, string? helpString)
        {
            Create(name, callback, flags, helpString);
        }

        private void Create(string? name, ConCommandExecuteHandler? callback, 
            Fcvar? flags, string? helpString)
        {
            _name = name;
            _executeAction = callback;
            _helpString = helpString;

            CommandManager.Instance.RegisterConCommand(this);
        }

        public Fcvar? GetFlags()
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
            try
            {
                if (callback == null) throw new ArgumentNullException(nameof(callback), "Parameter is null!");
                if (args == null) throw new ArgumentNullException(nameof(args), "Parameter is null!");

                callback.Invoke(args);
            }
            catch (Exception e)
            {
                new Logger().Log(LogSeverity.LogError, $"The command \"{_name}\" threw an exception: \"{e.Message}\"");
            }
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
