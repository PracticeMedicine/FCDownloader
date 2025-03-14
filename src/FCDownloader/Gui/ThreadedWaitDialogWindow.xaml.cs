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
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace AridityTeam.BetaFortressSetup.Gui
{
    /// <summary>
    /// Interaction logic for ThreadedWaitDialogWindow.xaml
    /// </summary>
    public partial class ThreadedWaitDialogWindow : Window
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private static RemovableConcurrentBag<ThreadedWaitDialogWindow> _dialogWindows = new RemovableConcurrentBag<ThreadedWaitDialogWindow>();
        private static object _lock = new object();

        private string? _winName = null;
        private bool _initialized = false;
        private bool _canClose = true;
        private bool _removeFromListOnClose = false;

        public ThreadedWaitDialogWindow()
        {
            ThreadPool.QueueUserWorkItem(ok =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    InitializeComponent();
                    Initialized += (iSender, iArgs) => { _initialized = true; };
                });
            });
        }

        public void ChangeTitle(string newTitle)
        {
            ThreadPool.QueueUserWorkItem(ok =>
            {
                this.Dispatcher.BeginInvoke(() =>
                this.Title = newTitle);
            });
        }

        public void ChangeText(string newText)
        {
            ThreadPool.QueueUserWorkItem(ok =>
            {
                this.Dispatcher.BeginInvoke(() =>
                this.LblWaitMsg.Content = newText);
            });
        }

        public void ChangeProgressBar(int percentage, bool isIndeterminate = false, int minimum = 0, int maximum = 100)
        {
            ThreadPool.QueueUserWorkItem(ok =>
            {
                this.Dispatcher.BeginInvoke(() =>
                {
                    this.ProgressBar.Minimum = minimum;
                    this.ProgressBar.Maximum = maximum;
                    this.ProgressBar.Value = percentage;
                    this.ProgressBar.IsIndeterminate = isIndeterminate;

                    if (isIndeterminate == true)
                    {
                        this.CircularIndProgressBar.Visibility = Visibility.Visible;
                        this.ProgressBar.Width = 505;
                    }
                    else
                    {
                        this.CircularIndProgressBar.Visibility = Visibility.Collapsed;
                        this.ProgressBar.Width = 530;
                    }
                });
            });
        }

        /// <summary>
        /// Gets all wait dialog windows.
        /// </summary>
        /// <returns>self-explanatory</returns>
        private static ThreadedWaitDialogWindow?[] GetAllDialogWindows()
        {
            return _dialogWindows.ToArray();
        }

        /// <summary>
        /// Gets the wait dialog window by name.
        /// </summary>
        /// <param name="nameToFind">Specified name to find</param>
        /// <returns>Returns the found window; returns null if the window could not be found.</returns>
        public static ThreadedWaitDialogWindow? GetDialogWindowByTitle(string nameToFind)
        {
            foreach(var window in _dialogWindows)
            {
                if (window?._winName == nameToFind) return window;
            }
            return null;
        }

        /// <summary>
        /// Gets the current wait dialog's progress bar.
        /// </summary>
        /// <returns>Returns the current progress bar control.</returns>
        public ProgressBar GetProgressBar()
        {
            return this.ProgressBar;
        }

        public void ShowWaitDialog()
        {
            var _window = this;

            ThreadPool.QueueUserWorkItem(ok =>
            {
                _window.Dispatcher.Invoke(() =>
                {
                    _window.Show();
                });
            });
        }

        /// <summary>
        /// Creates and shows a new wait dialog window
        /// </summary>
        /// <param name="winName">Window name to be accessed with GetDialogWindowByTitle</param>
        /// <param name="title">Window title</param>
        /// <param name="text">Wait dialog text contents</param>
        /// <param name="noBorder">Whether to enable window borders</param>
        /// <param name="showCloseBtn">Whether to enable the close button</param>
        /// <param name="removeOnClose">Whether to remove this window from the list on close</param>
        /// <param name="parent">Window parent</param>
        /// <returns></returns>
        public static ThreadedWaitDialogWindow ShowWaitDialog(
            string winName, string title, string text, bool noBorder, bool showCloseBtn = true, bool removeOnClose = false,
            Window? parent = null)
        {
            lock (_lock)
            {
                ThreadedWaitDialogWindow _window = new ThreadedWaitDialogWindow(); 

                _window._winName = winName;
                _window._removeFromListOnClose = removeOnClose;
                _dialogWindows.Add(_window);

                ThreadPool.QueueUserWorkItem(ok =>
                {
                    _window.Dispatcher.Invoke(() =>
                    {
                        if (_window._initialized != true) _window.InitializeComponent();
                        _window.Title = title;
                        _window.LblWaitMsg.Content = text;
                        _window.WindowStyle = noBorder ? WindowStyle.None : WindowStyle.SingleBorderWindow;
                        _window.ProgressBar.IsIndeterminate = true;
                        _window._canClose = showCloseBtn;

                        if (parent != null)
                        {
                            _window.Owner = parent;

                            _window.ShowDialog();
                        }
                        else
                        {
                            _window.Show();
                        }

                        if (showCloseBtn != true)
                        {
                            // Get the window handle
                            var hwnd = new WindowInteropHelper(_window).Handle;

                            // Get the current window style
                            var style = GetWindowLong(hwnd, GWL_STYLE);

                            // Remove the close button (WS_SYSMENU)
                            SetWindowLong(hwnd, GWL_STYLE, style & ~WS_SYSMENU);
                        }
                    });
                });

                return _window;
            }
        }

        /// <summary>
        /// Closes the wait dialog.
        /// USE THIS INSTEAD OF Window.Close()
        /// </summary>
        public void CloseWaitDialog()
        {
            ThreadPool.QueueUserWorkItem(ok =>
            {
                this.Dispatcher.Invoke(() => { this._canClose = true; this.Close(); });
            });
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(ok =>
            {
                if (_removeFromListOnClose == true)
                {
                    _dialogWindows.Remove(this);
                }
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !_canClose;
        }
    }
}
