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
using System.Linq;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;

using AridityTeam.Base;
using AridityTeam.BetaFortressSetup.Util;
using AridityTeam.BetaFortressSetup.Startup;

namespace AridityTeam.BetaFortressSetup.Gui
{
    /// <summary>
    /// Interaction logic for ConsoleWindow.xaml
    /// </summary>
    [SupportedOSPlatform("windows")]
    public partial class ConsoleWindow : Window
    {
        private AridLogger? _logger;
        public TextBox? ConsoleOutputBox = null;
        ConVar bfclient_loglevel = new ConVar("bfclient_loglevel", "1", FCVAR.DEVELOPMENT_ONLY, 
            "dummy command.");
        ConCommand get_convar_val = new ConCommand("get_convar_val", (args) =>
        {
            string? input = args?.AppliedArgs?.Trim();
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("get_convar_val: you must enter an ConVar name!!!"); 
                return;
            }

            string[] parts = input.Split(new char[] { ' ', '=', ':' }, 2);
            if (parts.Length == 0) return;

            string? name = parts[0].Trim();

            IConVar? _var = CommandManager.Instance.GetConVarByName(name);

            if (_var != null)
            {
                Console.WriteLine("get_convar_val: value for \"{0}\" is \"{1}\".", _var.GetName(), _var.GetString());
            }
            else
            {
                Console.WriteLine("get_convar_val: could not find \"{0}\"!!!", name);
            }
        }, FCVAR.NONE, "Gets the \"ConVar\"'s value.");
        ConCommand help = new ConCommand("help", args =>
        {
            string? input = args?.AppliedArgs?.Trim();
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine("help: you must enter an ConVar name!!!");
                return;
            }

            string[] parts = input.Split(new char[] { ' ', '=', ':' }, 2);
            if (parts.Length == 0) return;

            string? name = parts[0].Trim();

            IConCommand? _cmd = CommandManager.Instance.GetConCommandByName(name);
            IConVar? _var = CommandManager.Instance.GetConVarByName(name);

            if (_cmd != null)
            {
                Console.WriteLine("{0}:\n" +
                    "   {1}", _cmd.GetName(), _cmd.GetHelpString());
            }
            else if (_var != null)
            {
                Console.WriteLine("{0}:\n" +
                    "   {1}", _var.GetName(), _var.GetHelpString());
            }
            else
            {
                Console.WriteLine("help: error getting command: Could not find \"{0}\".", name);
            }
        }, FCVAR.NONE, "Gets the \"ConVar\"'s or \"ConCommand\"'s help string.");
        ConCommand quit = new ConCommand("quit", args =>
        {
            Application.Current.Shutdown(0);
        }, FCVAR.NONE, "Quits the application.");

        public ConsoleWindow()
        {
            _logger = new AridLogger(typeof(ConsoleWindow));
            InitializeComponent();
            ConsoleOutputBox = OutputText;
        }

        private void RunCommand()
        {
            if (this.CommandInput.Text.Contains(';'))
            {
                Console.WriteLine("command separator isn't supported on \"aridsh\"");
                return;
            }

            string input = this.CommandInput.Text.Trim();
            if (string.IsNullOrEmpty(input)) return;

            string[] parts = input.Split(new char[] { ' ', '=', ':' }, 2); // Split into at most 2 parts
            if (parts.Length == 0) return;

            string name = parts[0].Trim();
            IConCommand? _cmd = CommandManager.Instance.GetConCommandByName(name);
            IConVar? _var = CommandManager.Instance.GetConVarByName(name);
            object? _val = parts.Length > 1 ? parts[1].Trim() : null; // Get the value if it exists

            _logger?.Info("Value: {0}", _val);

            Console.WriteLine("] {0}", this.CommandInput.Text);
            this.CommandInput.Text = string.Empty;

            if (_cmd != null)
            {
                if (!SetupInstallerMain.DevMode && _cmd.GetFlags() == FCVAR.DEVELOPMENT_ONLY)
                    throw new InvalidOperationException("Cannot run this command unless developer mode is on.");

                _logger?.Info("Executing ConCommand (\"{0}\")...", _cmd.GetName());
                _cmd.Execute(_val?.ToString());
            }
            else if (_var != null)
            {
                if (!SetupInstallerMain.DevMode && _var.GetFlags() == FCVAR.DEVELOPMENT_ONLY)
                    throw new InvalidOperationException("Cannot run this command unless developer mode is on.");

                if (string.IsNullOrEmpty(_val?.ToString())) throw new ArgumentNullException(nameof(_val), "Expecting an argument!");

                _logger?.Info("Setting a value for \"{0}\" to \"{1}\"...", _var.GetName(), _val.ToString());
                _var.SetValue(_val);
            }
            else
            {
                _logger?.Err("Could not find \"{0}\".", name);
                Console.Error.WriteLine("Could not find \"{0}\".", name);
            }
        }

        private void Window_Loaded(object sender, EventArgs e)
        {
            Console.WriteLine(
                "welcome to aridsh\n" +
                "a quake/half-life like dev console\n" +
                "set/create commands using ConVar or ConCommand.\n" +
                "if ur not a dev then why are u here?" +
                "\n");
        }

        private void OutputText_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void CommandInEnterBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RunCommand();
            }
            catch (Exception ex)
            {
                _logger?.Err(ex.Message);
                Console.Error.WriteLine(ex.Message);
            }
        }

        private void CommandInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                try
                {
                    RunCommand();
                }
                catch (Exception ex)
                {
                    _logger?.Err(ex.Message);
                    Console.Error.WriteLine(ex.Message);
                }
            }
        }
    }
}
