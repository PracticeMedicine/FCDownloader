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
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//using System.Net;
//using System.Reflection;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows;

using AridityTeam.Base;
using AridityTeam.BetaFortressSetup.Gui;

namespace AridityTeam.BetaFortressSetup.Util
{
    /// <summary>
    /// Main mod manager.
    /// Manages things like installing the mod nor updating the mod.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class ModManager : IDisposable
    {
        // all private values :3
        private bool _isAnotherInstanceRunning = false;
        private EModInstallationStatus? _installStatus = EModInstallationStatus.None;

        private AridLogger? _logger = new AridLogger(typeof(ModManager));
        //private string PrivateUrlRootDir = ClientDefs.RepoUri + "/raw/refs/heads/main/";
        private Exception? _lastException = null;
        private string? _currentInstallDir = string.Empty;
        private string? _newUpdateMsg = string.Empty;
        private readonly object _lock = new object();
        private static SHA256 _sha256 = SHA256.Create();

        // all public values :3
        public EventHandler? InstanceFinishedHandler { get; set; }
        public ExceptionHandler? InstanceErroredHandler { get; set; }
        public LibGit2Sharp.Handlers.CheckoutProgressHandler? CheckoutProgressHandler { get; set; }
        public LibGit2Sharp.Handlers.TransferProgressHandler? TransferProgressHandler { get; set; }
        public LibGit2Sharp.Handlers.ProgressHandler? ProgressHandler { get; set; }

        /// <summary>
        /// Gets the last exception
        /// </summary>
        /// <returns></returns>
        public Exception? GetLastException()
        {
            return _lastException;
        }

        /// <summary>
        /// Main mod manager.
        /// Manages things like installing the mod nor updating the mod.
        /// </summary>
        public ModManager()
        {
            _installStatus = EModInstallationStatus.None;
            _lastException = null;
            InstanceFinishedHandler = null;
            InstanceErroredHandler = null;
            CheckoutProgressHandler = null;
            TransferProgressHandler = null;
            ProgressHandler = null;
            _isAnotherInstanceRunning = false;
        }
        
        /// <summary>
        /// 
        /// </summary>
        private void ClearVariables(bool disposing = false)
        {
            _logger?.Info("Clearing the unnecessary variables out...");
            _lastException = null;

            if (disposing)
            {
                _logger = null;
                _installStatus = null;
                _currentInstallDir = null;

                InstanceFinishedHandler = null;
                InstanceErroredHandler = null;
                CheckoutProgressHandler = null;
                TransferProgressHandler = null;
                ProgressHandler = null;
            }
        }

        /// <summary>
        /// Gets the next commit's message.
        /// </summary>
        /// <returns>Returns you already know.</returns>
        public string? GetNextUpdateMsg()
        {
            return _newUpdateMsg;
        }

        /// <summary>
        /// Updates the installation status.
        /// </summary>
        /// <exception cref="OperationCanceledException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        private void UpdateInstallationStatus()
        {
            ClearVariables();

            lock (_lock)
            {
                string? monitorSlug = "installation-status-update";
                Sentry.SentryId? checkInId = _logger?.Sentry_CaptureCheckIn(monitorSlug, Sentry.CheckInStatus.InProgress);
                _logger?.Info("Updating installation status...");

                if (_isAnotherInstanceRunning == true)
                {
                    _logger?.Info("Another installation instance is running! Aborting to avoid inteference...");
                    _logger?.Sentry_CaptureCheckIn(monitorSlug, Sentry.CheckInStatus.Ok, checkInId);
                    return;
                }

                try
                {
                    bool _actuallyOutOfDate = false;

                    RegistryKey? reg = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\AridityTeam\BFClient");

                    if (reg == null)
                    {
                        _installStatus = EModInstallationStatus.NotInstalled;
                        return;
                    }

                    if (reg?.GetValue("ModInstallationDir") == null)
                    {
                        _installStatus = EModInstallationStatus.NotInstalled;
                        return;
                    }

                    var installDir = reg?.GetValue("ModInstallationDir")?.ToString();
                    if (string.IsNullOrEmpty(installDir))
                    {
                        _installStatus = EModInstallationStatus.NotInstalled;
                        return;
                    }

                    if (!Directory.Exists(installDir))
                    {
                        _installStatus = EModInstallationStatus.NotInstalled;
                        return;
                    }

                    var installProgressDlg = ThreadedWaitDialogWindow.GetDialogWindowByTitle("install-progress-dlg");
                    if (installProgressDlg != null) installProgressDlg.CloseWaitDialog();

                    var updateProgressDlg = ThreadedWaitDialogWindow.GetDialogWindowByTitle("update-progress-dlg");
                    if (updateProgressDlg != null) updateProgressDlg.CloseWaitDialog();

                    /*var cdllRemote = (HttpWebRequest)HttpWebRequest.Create(PrivateUrlRootDir + "/bin/client.dll");
                    cdllRemote.UserAgent = string.Format("{0}/{1}", 
                        Assembly.GetEntryAssembly().GetName().Name, 
                        Assembly.GetEntryAssembly().GetName().Version);

                    var gamedllRemote = (HttpWebRequest)HttpWebRequest.Create(PrivateUrlRootDir + "/bin/server.dll");
                    gamedllRemote.UserAgent = string.Format("{0}/{1}", 
                        Assembly.GetEntryAssembly().GetName().Name, 
                        Assembly.GetEntryAssembly().GetName().Version);

                    var response1 = cdllRemote.GetResponse();
                    var content1 = response1.GetResponseStream();
                    string clientDllFile = "";
                    string serverDllFile = "";

                    using (var reader = new StreamReader(content1))
                    {
                        clientDllFile = reader.ReadToEnd();
                    }*/

                    if (!Directory.Exists(installDir))
                    {
                        _installStatus = EModInstallationStatus.NotInstalled;
                    }
                    else
                    {
                        List<string?> _missingDirs = new List<string?>();
                        foreach (var dir in ClientDefs.DirectoryNeeded)
                        {
                            if (!Directory.Exists(dir)) _missingDirs.Add(dir);
                        }

                        List<string?> _missingFiles = new List<string?>();
                        foreach(var file in ClientDefs.FilesNeeded)
                        {
                            if (!File.Exists(file)) _missingFiles.Add(file);
                        }

                        _currentInstallDir = installDir;

                        if (_missingDirs.Any() || _missingFiles.Any())
                        {
                            _installStatus = EModInstallationStatus.MissingFiles; // missing files, wont continue.
                            throw new FileNotFoundException("Missing files detected!");
                        }

                        /*
                        // client.dll
                        string cdllRemoteHash = string.Empty;
                        string cdllLocalHash = string.Empty;

                        // server.dll
                        string gamedllRemoteHash = string.Empty;
                        string gamedllLocalHash = string.Empty;

                        string cdllLocalFile = string.Empty;
                        string gamedllLocalFile = string.Empty;

                        using (StreamReader reader1 = new StreamReader(installDir + "/bin/client.dll"))
                        using (StreamReader reader2 = new StreamReader(installDir + "/bin/server.dll"))
                        {
                            cdllLocalFile = reader1.ReadToEnd();
                            gamedllLocalFile = reader2.ReadToEnd();
                        }

                        // verify the hash of both client and server dlls
                        cdllRemoteHash = GetHash(clientDllFile);
                        gamedllRemoteHash = GetHash(serverDllFile);

                        cdllLocalHash = GetHash(cdllLocalFile);
                        gamedllLocalHash = GetHash(gamedllLocalFile);

                        _actuallyOutOfDate = !VerifyHash(cdllLocalFile, cdllRemoteHash) ||
                            !VerifyHash(gamedllLocalFile, gamedllRemoteHash);
                        */

                        string logMessage = "";

                        // lastly verify the current local commit
                        using (var repo = new LibGit2Sharp.Repository(installDir))
                        {
                            var fo = new LibGit2Sharp.FetchOptions()
                            {
                                Depth = 1
                            };

                            var remote = repo.Network.Remotes["origin"];
                            var refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
                            LibGit2Sharp.Commands.Fetch(repo, remote.Name, refSpecs, fo, logMessage);

                            // Get local and remote commits
                            var localCommit = repo.Head.Tip;
                            var remoteCommit = repo.Branches["origin/main"].Tip;  // Adjust branch name as needed

                            // Check if local is behind remote
                            _actuallyOutOfDate = localCommit.Id != remoteCommit.Id;

                            if (_actuallyOutOfDate == true)
                                _logger?.Info("New commit on the remote: {0}", remoteCommit.Id);

                            _newUpdateMsg = remoteCommit.Message;

                            remote = null;
                            refSpecs = null;
                            localCommit = null;
                            remoteCommit = null;
                        }
                        _logger?.Info(logMessage);

                        _installStatus = _actuallyOutOfDate ?
                            EModInstallationStatus.OutOfDate :
                            EModInstallationStatus.CurrentlyInstalled;

                        _logger?.Sentry_CaptureCheckIn(monitorSlug, Sentry.CheckInStatus.Ok, checkInId);

                        installDir = null;
                        reg = null;
                    }
                }
                catch (Exception ex)
                {
                    _logger?.Crit("An error occurred while checking the current installation status: {0}", ex.Message);
                    _lastException = ex;
                    _installStatus = EModInstallationStatus.Errored;
                    _logger?.Sentry_CaptureCheckIn(monitorSlug, Sentry.CheckInStatus.Error, checkInId);
                    InstanceErroredHandler?.Invoke(ex);
                }

                _logger?.Info("Current installation status: {0}", _installStatus);
                monitorSlug = null;
                checkInId = null;
            }
        }

        private static string GetHash(string input)
        {
            byte[] data = _sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sBuilder = new StringBuilder(data.Length * 2);
            foreach (byte b in data)
            {
                sBuilder.Append(b.ToString("x2"));
            }
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        private static bool VerifyHash(string input, string hash)
        {
            // Hash the input.
            var hashOfInput = GetHash(input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            return comparer.Compare(hashOfInput, hash) == 0;
        }

        /// <summary>
        /// Checks for updates.
        /// </summary>
        public void CheckForUpdates()
        {
            UpdateInstallationStatus();
        }

        /// <summary>
        /// Gets the current installation status;
        /// </summary>
        /// <returns></returns>
        public EModInstallationStatus? GetInstallationStatus()
        {
            return _installStatus;
        }
        
        /// <summary>
        /// Installs the mod to the default installation folder/directory.
        /// </summary>
        public void Install()
        {
            Install(ClientDefs.ModInstallationPath, ClientDefs.RepoStableBranchName);
        }

        /// <summary>
        /// Installs the mod to the specified installation folder/directory.
        /// </summary>
        /// <param name="directory">Output directory</param>
        /// <exception cref="OperationCanceledException">Throws when the mod was already been installed or an another installation instance is already running.</exception>
        public void Install(string directory, string branchName)
        {
            lock (_lock)
            {
                var monitorSlug = "install-process";
                var checkInId = _logger?.Sentry_CaptureCheckIn(monitorSlug, Sentry.CheckInStatus.InProgress);

                try
                {
                    UpdateInstallationStatus();

                    if (_installStatus == EModInstallationStatus.CurrentlyInstalled)
                        throw new OperationCanceledException("Beta Fortress has already been installed!");

                    if (_isAnotherInstanceRunning == true)
                        throw new OperationCanceledException("Another installation instance is already running, please wait before running another instance!!!");

                    if (Directory.Exists(directory) == true)
                    {
                        Uninstall();
                    }

                    string? installDir = string.Empty;

                    using (RegistryKey? reg = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\AridityTeam\BFClient", true))
                    {
                        if (reg?.GetValue("ModInstallationDir") == null)
                        {
                            reg?.SetValue("ModInstallationDir", directory);
                        }

                        installDir = reg?.GetValue("ModInstallationDir")?.ToString();
                    }

                    Task.Run(() =>
                    {
                        _isAnotherInstanceRunning = true;

                        var co = new LibGit2Sharp.CloneOptions(new LibGit2Sharp.FetchOptions()
                        {
                            Depth = 1,
                            OnTransferProgress = (LibGit2Sharp.TransferProgress progress) =>
                            {
                                if (TransferProgressHandler == null) return true;

                                return TransferProgressHandler.Invoke(progress);
                            },
                            OnProgress = (msg) =>
                            {
                                if (ProgressHandler == null) return true;

                                return ProgressHandler.Invoke(msg);
                            },
                            RepositoryOperationCompleted = (context) =>
                            {
                                if (context.RepositoryPath != ClientDefs.ModInstallationPath &&
                                context.RemoteUrl != ClientDefs.RepoUri)
                                    throw new InvalidDataException("Post-installation error: Incorrect data provided.");

                                InstanceFinishedHandler?.Invoke(this, new EventArgs());
                            }
                        })
                        {
                            BranchName = branchName,
                            RecurseSubmodules = true, // if theres any one.
                            OnCheckoutProgress = (path, completed, total) =>
                            {
                                if (CheckoutProgressHandler == null) return;
                                CheckoutProgressHandler.Invoke(path, completed, total);
                            },
                            Checkout = true,
                        };

                        LibGit2Sharp.Repository.Clone(ClientDefs.RepoUri, directory, co);

                        //InstanceFinishedHandler?.Invoke(this, new EventArgs());
                        _isAnotherInstanceRunning = false;

                        UpdateInstallationStatus();
                    });

                    _logger?.Sentry_CaptureCheckIn(monitorSlug, Sentry.CheckInStatus.Ok, checkInId);
                }
                catch (Exception ex)
                {
                    _lastException = ex;
                    _logger?.Sentry_CaptureCheckIn(monitorSlug, Sentry.CheckInStatus.Error, checkInId);
                    InstanceErroredHandler?.Invoke(ex);
                }
            }
        }

        /// <summary>
        /// Updates the mod.
        /// </summary>
        /// <exception cref="OperationCanceledException"></exception>
        public void Update()
        {
            lock (_lock)
            {
                var monitorSlug = "update-process";
                var checkInId = _logger?.Sentry_CaptureCheckIn(monitorSlug, Sentry.CheckInStatus.InProgress);

                try
                {
                    UpdateInstallationStatus();

                    if (_installStatus == EModInstallationStatus.NotInstalled)
                        throw new OperationCanceledException("Beta Fortress isn't installed yet!");

                    if (_installStatus == EModInstallationStatus.OutOfDate)
                    {
                        _isAnotherInstanceRunning = true;

                        using (var repo = new LibGit2Sharp.Repository(_currentInstallDir))
                        {
                            var po = new LibGit2Sharp.PullOptions()
                            {
                                MergeOptions = new LibGit2Sharp.MergeOptions()
                                {
                                    OnCheckoutProgress = CheckoutProgressHandler,
                                },
                                FetchOptions = new LibGit2Sharp.FetchOptions()
                                {
                                    OnTransferProgress = (LibGit2Sharp.TransferProgress progress) =>
                                    {
                                        if (TransferProgressHandler == null) return true;

                                        return TransferProgressHandler.Invoke(progress);
                                    },
                                    OnProgress = (msg) =>
                                    {
                                        if (ProgressHandler == null) return true;

                                        return ProgressHandler.Invoke(msg);
                                    },
                                    Depth = 1,
                                    RepositoryOperationCompleted = (context) =>
                                    {
                                        if (context.RepositoryPath != ClientDefs.ModInstallationPath &&
                                        context.RemoteUrl != ClientDefs.RepoUri)
                                            throw new InvalidDataException("Post-installation error: Incorrect data provided.");

                                        InstanceFinishedHandler?.Invoke(this, new EventArgs());
                                    }
                                }
                            };

                            // User information to create a merge commit
                            var signature = new LibGit2Sharp.Signature(
                                new LibGit2Sharp.Identity(
                                    "Beta Fortress Client Git Manager", "aridityteam@gmail.com"), DateTimeOffset.Now);

                            LibGit2Sharp.Commands.Pull(repo, signature, po);

                            //InstanceFinishedHandler?.Invoke(this, new EventArgs());
                            _isAnotherInstanceRunning = false;

                            UpdateInstallationStatus();
                        }

                        _logger?.Sentry_CaptureCheckIn(monitorSlug, Sentry.CheckInStatus.Ok, checkInId);
                    }
                    else
                    {
                        MessageBox.Show("Beta Fortress is currently up-to-date!", "Beta Fortress Client", MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    _lastException = ex;
                    _logger?.Sentry_CaptureCheckIn(monitorSlug, Sentry.CheckInStatus.Error, checkInId);
                    InstanceErroredHandler?.Invoke(ex);
                }
            }
        }

        /// <summary>
        /// Uninstalls the mod
        /// </summary>
        /// <exception cref="OperationCanceledException">Throws when the mod isn't installed or there's another installation instance already running.</exception>
        /// <exception cref="InvalidOperationException">Throws when there's an error with the elevator process</exception>
        public void Uninstall()
        {
            lock (_lock)
            {
                if (_installStatus != EModInstallationStatus.CurrentlyInstalled)
                    throw new OperationCanceledException("Beta Fortress isn't been installed!");

                if (_isAnotherInstanceRunning == true)
                    throw new OperationCanceledException("Another installation instance is already running, please wait before running another instance!!!");

                Task.Run(() =>
                {
                    _isAnotherInstanceRunning = true;
                    using (Process p = new Process())
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo()
                        {
                            FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AridityTeam.ElevatorProcess.exe"),
                            Arguments = "/modManager /uninstall",
                            UseShellExecute = true,
                            CreateNoWindow = true,
                            Verb = "RunAs"
                        };

                        p.StartInfo = startInfo;

                        p.Start();
                        p.ErrorDataReceived += (sender, e) =>
                        {
                            throw new InvalidOperationException("An error occurred on the application: " + e.Data);
                        };

                        p.WaitForExit();
                    }

                    InstanceFinishedHandler?.Invoke(this, new EventArgs());
                    _isAnotherInstanceRunning = false;
                });
            }
        }

        /// <summary>
        /// Disposes shit.
        /// </summary>
        public void Dispose()
        {
            ClearVariables(true);
            _sha256.Dispose();
        }
    }
}
