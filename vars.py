"""
Tiny module that currently just establishes
the temp paths and some variables for other
modules to use.
"""
from platform import system
import sys
import tempfile

GIT_BINARY = None
INSTALL_PATH = None
TF2C_PATH = None

SCRIPT_MODE = len(sys.argv) > 1
