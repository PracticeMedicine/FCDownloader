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
using System.IO;
using System.Collections.Generic;
using System.Text.Json;

using AridityTeam.Base;
using AridityTeam.BetaFortressSetup.DatabaseManager.Infos;
using System.Net.Http;
using System.Threading.Tasks;

namespace AridityTeam.BetaFortressSetup.DatabaseManager
{
    /// <summary>
    /// Manages the JSON database for Beta Fortress Client
    /// </summary>
    public class DatabaseManager
    {
        private ObservableList<BFClientDatabase> _installedDbs;
        private AridLogger _logger;
        private string _installedDbConfigPath = string.Empty;
        private static DatabaseManager? _instance = null;

        public ExceptionHandler? ExceptionHandler { get; set; }

        /// <summary>
        /// Gets the existing instance of DatabaseManager.
        /// If there is no instance of the class, it will create a new one.
        /// </summary>
        public static DatabaseManager Instance
        {
            get
            {
                object _lock = new object();
                lock(_lock)
                {
                    if (_instance == null) _instance = new DatabaseManager();
                    return _instance;
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public DatabaseManager()
        {
            if (_logger == null)
            {
                _logger = new AridLogger(typeof(DatabaseManager));
            }

            _installedDbs = new ObservableList<BFClientDatabase>();
            _installedDbs.ItemAdded += InstalledDbList_OnItemAdded;
        }

        /// <summary>
        /// An event.
        /// </summary>
        /// <param name="db"></param>
        private void InstalledDbList_OnItemAdded(BFClientDatabase? db)
        {
        }

        /// <summary>
        /// Initializes the manager.
        /// </summary>
        public void Init()
        {
            Init(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "/Config/InstalledDatabases"));
        }

        /// <summary>
        /// idk
        /// </summary>
        /// <returns></returns>
        public string GetInstalledDatabaseConfigPath()
        {
            return _installedDbConfigPath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="installedDbPath"></param>
        public void Init(string installedDbPath)
        {
            try
            {
                _installedDbConfigPath = installedDbPath;
                string[] _installedDbFiles = Directory.GetFiles(installedDbPath, "*.json", SearchOption.AllDirectories);

                foreach (string dbFilePath in _installedDbFiles)
                {
                    _logger.Info("Found {0}!", dbFilePath);

                    string fileContent = File.ReadAllText(dbFilePath);

                    InstalledDatabasesInfo? installedDbConfig = JsonSerializer.Deserialize<InstalledDatabasesInfo>(fileContent);

                    string dbContent = string.Empty;

                    using (HttpClient client = new HttpClient())
                    {
                        Task.Run(async () =>
                        {
                            // Send a GET request to the specified URL
                            HttpResponseMessage response = await client.GetAsync(installedDbConfig?.DatabaseUri + "/index.json");
                            response.EnsureSuccessStatusCode(); // Throw if not a success code.

                            // Read the content as a string
                            dbContent = await response.Content.ReadAsStringAsync();
                        });
                    }

                    BFClientDatabase? db = JsonSerializer.Deserialize<BFClientDatabase>(dbContent);

                    _installedDbs.Add(db);
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                if (ExceptionHandler != null) ExceptionHandler.Invoke(ex);
            }
            catch (Exception ex)
            {
                if (ExceptionHandler != null) ExceptionHandler.Invoke(ex);
            }
        }
    }
}
