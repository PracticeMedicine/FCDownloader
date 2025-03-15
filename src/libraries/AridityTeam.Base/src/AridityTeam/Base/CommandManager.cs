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
using System.Collections.Concurrent;
using System.Linq;

namespace AridityTeam.Base
{
    /// <summary>
    /// Manages <see cref="ConVar"/>s and <see cref="ConCommand"/>s.
    /// </summary>
    public class CommandManager
    {
        private readonly ObservableConcurrentBag<IConCommand> _commands;
        private readonly ObservableConcurrentBag<IConVar> _conVars;

        private static CommandManager? _instance;
        private static readonly object _threadLocker = new object();

        /// <summary>
        /// Gets the existing instance of <see cref="CommandManager"/>.
        /// </summary>
        public static CommandManager Instance
        {
            get
            {
                lock (_threadLocker)
                {
                    return _instance ??= new CommandManager();
                }
            }
        }

        /// <summary>
        /// Constructs a new <see cref="CommandManager"/>. <para/>
        /// (use <see cref="CommandManager.Instance"/> instead.)
        /// </summary>
        private CommandManager()
        {
            _commands = new ObservableConcurrentBag<IConCommand>();
            _conVars = new ObservableConcurrentBag<IConVar>();

            _commands.ItemAdded += cmd => { /* Handle command added */ };
            _conVars.ItemAdded += var =>
            {
                if (var is ConVar conVar) conVar.IsRegistered = true;
            };
        }

        /// <summary>
        /// Gets the <see cref="ConVar"/> by looking for the exact name.
        /// </summary>
        /// <param name="name">Name to look for.</param>
        /// <returns>Returns null if the <see cref="ConVar"/> couldn't be found.</returns>
        public IConVar? GetConVarByName(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            return _conVars.FirstOrDefault(conVar =>
                conVar?.GetName()?.Equals(name, StringComparison.OrdinalIgnoreCase) == true);
        }

        /// <summary>
        /// Gets the <see cref="ConCommand"/> by looking for the exact name.
        /// </summary>
        /// <param name="name">Name to look for.</param>
        /// <returns>Returns null if the <see cref="ConCommand"/> couldn't be found.</returns>
        public IConCommand? GetConCommandByName(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            return _commands.FirstOrDefault(cmd =>
                cmd?.GetName()?.Equals(name, StringComparison.OrdinalIgnoreCase) == true);
        }

        /// <summary>
        /// Executes a <see cref="ConCommand"/> only when the exact name specified is found.
        /// </summary>
        /// <param name="name">Name to look for.</param>
        /// <param name="args">Arguments to pass to the command.</param>
        public void ExecuteConCommandByName(string name, string? args)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            var cmd = GetConCommandByName(name);
            cmd?.Execute(args);
        }

        /// <summary>
        /// Sets the <see cref="ConVar"/>'s value.
        /// </summary>
        /// <param name="name">Name to look for.</param>
        /// <param name="value">New value to set.</param>
        public void SetConVarByName(string name, object? value)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (value == null) throw new ArgumentNullException(nameof(value), "Expected a value!");

            var conVar = GetConVarByName(name);
            conVar?.SetValue(value);
        }

        /// <summary>
        /// Registers the <see cref="ConVar"/> to the manager.
        /// </summary>
        /// <param name="newConVar">The <see cref="ConVar"/> to register.</param>
        /// <returns>The registered <see cref="ConVar"/>.</returns>
        public IConVar RegisterConVar(IConVar? newConVar)
        {
            if (newConVar == null) throw new ArgumentNullException(nameof(newConVar), "Parameter is null!");

            _conVars.Add(newConVar);
            return newConVar;
        }

        /// <summary>
        /// Registers the <see cref="ConCommand"/> to the manager.
        /// </summary>
        /// <param name="newCmd">The <see cref="ConCommand"/> to register.</param>
        /// <returns>The registered <see cref="ConCommand"/>.</returns>
        public IConCommand RegisterConCommand(IConCommand? newCmd)
        {
            if (newCmd == null) throw new ArgumentNullException(nameof(newCmd), "Parameter is null!");

            _commands.Add(newCmd);
            return newCmd;
        }
    }
}
