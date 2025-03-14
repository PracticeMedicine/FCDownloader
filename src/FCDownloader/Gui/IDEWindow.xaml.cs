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
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;

// avalonedit usings
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using AridityTeam.BetaFortressSetup.Util;
using System.Collections.Concurrent;

namespace AridityTeam.BetaFortressSetup.Gui
{
    internal class SourceEngineCommandInfo
    {
        public required string Command { get; set; }
        public required string Description { get; set; }
    }

    /// <summary>
    /// Interaction logic for IDEWindow.xaml
    /// Usually used for editing "config.cfg".
    /// </summary>
    [SupportedOSPlatform("windows")] // every single fucking time i have to do this.
    public partial class IDEWindow : Window
    {
        private string? _filePath = null;
        private CompletionWindow? completionWindow = null;

        private RemovableConcurrentBag<SourceEngineCommandInfo> _sourceEngCmds = 
            new RemovableConcurrentBag<SourceEngineCommandInfo>();
        private RemovableConcurrentBag<SourceEngineCommandInfo> _gameCmds = 
            new RemovableConcurrentBag<SourceEngineCommandInfo>();

        private RemovableConcurrentBag<SourceEngineCommandInfo> _allCmds;

        public IDEWindow(string? filePath)
        {
            _filePath = filePath;
            InitializeComponent();

            AddGameCommands();

            _allCmds = new RemovableConcurrentBag<SourceEngineCommandInfo>(
                _gameCmds.Union(_sourceEngCmds));
        }

        private void AddEngineCommands()
        {
            this._sourceEngCmds.Add(new SourceEngineCommandInfo()
            {
                Command = "exec",
                Description = "Loads and executes an another .cfg file."
            });
            this._sourceEngCmds.Add(new SourceEngineCommandInfo()
            {
                Command = "give",
                Description = "Gives an weapon."
            });
            this._sourceEngCmds.Add(new SourceEngineCommandInfo()
            {
                Command = "givecurrentammo",
                Description = "Gives the ammo that the weapon is using."
            });
            this._sourceEngCmds.Add(new SourceEngineCommandInfo()
            {
                Command = "sv_cheats",
                Description = "If its set to 1, then server cheats are enabled. (obviously)"
            });
        }

        private void AddGameCommands()
        {
            AddEngineCommands();

            this._gameCmds.Add(new SourceEngineCommandInfo()
            {
                Command = "tf_bot_add",
                Description = "Adds/creates a TFBot."
            });

            this._gameCmds.Add(new SourceEngineCommandInfo()
            {
                Command = "tf_bot_kick",
                Description = "Kicks all TFBots."
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.EditorView.TextArea.TextEntering += EditorView_TextArea_TextEntering;
            this.EditorView.TextArea.TextEntered += EditorView_TextArea_TextEntered;
        }

        private void EditorView_TextArea_TextEntered(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            switch(e.Text)
            {
                case ".":
                    // Open code completion after the user has pressed dot:
                    completionWindow = new CompletionWindow(EditorView.TextArea);
                    IList<ICompletionData> data0 = completionWindow.CompletionList.CompletionData;

                    foreach (var cmd in _allCmds)
                    {
                        if (cmd == null) return;

                        data0.Add(new IDECompletionData(cmd.Command, cmd.Description));
                    }

                    completionWindow.Show();
                    completionWindow.Closed += delegate {
                        completionWindow = null;
                    };
                    break;
                case "give":
                    // Open code completion after the user has pressed dot:
                    completionWindow = new CompletionWindow(EditorView.TextArea);
                    IList<ICompletionData> data1 = completionWindow.CompletionList.CompletionData;

                    RemovableConcurrentBag<SourceEngineCommandInfo> _items = new RemovableConcurrentBag<SourceEngineCommandInfo>();
                    _items.Add(new SourceEngineCommandInfo() 
                    { 
                        Command = "weapon_ar2",
                        Description = "HL2: AR2"
                    });
                    _items.Add(new SourceEngineCommandInfo() 
                    { 
                        Command = "weapon_pistol",
                        Description = "HL2: 9mm Pistol"
                    });
                    _items.Add(new SourceEngineCommandInfo() 
                    { 
                        Command = "weapon_smg1",
                        Description = "HL2: Sub-machine gun"
                    });
                    _items.Add(new SourceEngineCommandInfo() 
                    { 
                        Command = "weapon_crossbow",
                        Description = "HL2: Standard crossbow"
                    });
                    _items.Add(new SourceEngineCommandInfo() 
                    { 
                        Command = "weapon_gravitygun",
                        Description = "HL2: Zero point energy manipulator"
                    });

                    foreach (var wpn in _items)
                    {
                        if (wpn == null) return;

                        data1.Add(new IDECompletionData(wpn.Command, wpn.Description));
                    }

                    completionWindow.Show();
                    completionWindow.Closed += delegate {
                        completionWindow = null;
                    };
                    break;
            }

        }

        private void EditorView_TextArea_TextEntering(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
        }
    }
}
