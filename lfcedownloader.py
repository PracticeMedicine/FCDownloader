"""
Master module. Runs basic checks and then offloads all
of the real work to functions defined in other files.
"""
import os
import traceback
import ctypes
from platform import system
from shutil import which
from subprocess import run
import sys
from sys import argv, exit, stdin
from rich import print
from gettext import gettext as _
import gettext
import gui
import downloads
import setup
import vars

# Disable QuickEdit so the process doesn't pause when clicked
if system() == 'Windows':
    kernel32 = ctypes.windll.kernel32
    kernel32.SetConsoleMode(kernel32.GetStdHandle(-10), (0x4|0x80|0x20|0x2|0x10|0x1|0x00|0x100))

def sanity_check():
    """
    This is mainly for Linux, because it's easy to launch it by double-clicking it, which would
    run it in the background and not show any output. PyInstaller has no way to force a terminal
    open for this on Linux. We could implement something similar to what we do to force using WT,
    but it's not a priority right now since Linux users can figure out how to use the terminal.
    """
    if not stdin or not stdin.isatty():
        print(_("Looks like we're running in the background. We don't want that, so we're exiting."))
        exit(1)

if sys.stdout.encoding == 'ascii':
    sys.stdout.reconfigure(encoding='utf-8')
if sys.stderr.encoding == 'ascii':
    sys.stderr.reconfigure(encoding='utf-8')

try:
    sanity_check()
    setup.setup_binaries()
    setup.setup_path(False)
    endpath = vars.INSTALL_PATH + '/tf_coop_lambda'
    # After this line, we have two possible paths: installing, or updating/repairing
    if os.path.exists(vars.INSTALL_PATH + '/tf_coop_lambda/gameinfo.txt'):
        if gui.message_yes_no(("It looks like the mod's already installed. Do you want to update it?")):
            downloads.pull(endpath)
            gui.message_end(_("All done!"), 0)
    else:
        downloads.clone(endpath)
        gui.message_end(_("All done!"), 0)
except Exception as ex:
    if ex is not SystemExit:
        traceback.print_exc()
        print(_("[italic magenta]----- Exception details above this line -----"))
        print(_("[bold red]:warning: The program has failed. Post a screenshot in #bug-reporting on the Discord or make a issue on our GitHub. :warning:[/bold red]"))
        print(_("[italic magenta]----- Application details under this line -----"))
        print(_("[bold red]BUILD NUMBER: 1150[/bold red]"))
        print(_("[bold red]VERSION: 1.1.0010[/bold red]"))
        print(_("[bold red]DATE: August 5 2023[/bold red]"))
        print(_("[bold red]CODENAME: lambdagon.lfcedownloader[/bold red]"))
        print(_("[bold red]FULL VERSION STRING: 1.1.0010.1150.lambdagon.lfcedownloader.05.08.2023[/bold red]"))
        if os.environ.get("WT_SESSION"):
            print(_("[bold]You are safe to close this window."))
        else:
            input(_("Press Enter to exit."))
        exit(1)

