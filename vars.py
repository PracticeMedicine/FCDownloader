"""
Tiny module that currently just establishes
the temp paths and some variables for other
modules to use.
"""
from platform import system
import sys
import tempfile

# Assembly info (im literally a .net dev :sob:)
Name = "FCDownloader" # name of the util
Company = "The FC Team" # company/publisher
Codename = "lambdagon.fcdownloaderutil" # codename
# version info
VersionMajor = "1"
VersionMinor = "0"
VersionBuild = "1165"
VersionRevision = "0"

FullVersionStr = str.format("{major}.{minor}.{build}.{revision}", 
                         major=VersionMajor,
                         minor=VersionMinor,
                         build=VersionBuild,
                         revision=VersionRevision)

# eh
GameName = 'fc'
GamePrettyName = 'Fortress Connected'

# git stuff
GitPostBuffer = '524288000'
GitRepo = 'https://github.com/Lambdagon/fc.git'

GIT_BINARY = None
INSTALL_PATH = None
TF2C_PATH = None

# idk
FullGameInstallPath = None

SCRIPT_MODE = len(sys.argv) > 1

