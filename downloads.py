from os import path
from subprocess import run
from platform import system
import vars

def clone(endpath):
    run([vars.GIT_BINARY, 'config', '--global', 'http.postBuffer', '524288000'])
    run([vars.GIT_BINARY, 'config', '--global', 'http.maxRequestBuffer', '524288000'])
    run([vars.GIT_BINARY, 'config', '--global', 'core.compression', '9'])
    run([vars.GIT_BINARY, 'config', '--system', 'credential.helper', 'manager-core'])
    run([vars.GIT_BINARY, 'clone', '--depth', '1', 'https://github.com/Lambdagon/fc', endpath], check=True)

def pull(endpath):
    run([vars.GIT_BINARY, 'config', '--global', 'http.postBuffer', '1048576000'])
    run([vars.GIT_BINARY, 'config', '--global', 'http.maxRequestBuffer', '1048576000'])
    run([vars.GIT_BINARY, 'config', '--global', 'core.compression', '9'])
    run([vars.GIT_BINARY, 'config', '--system', 'credential.helper', 'manager-core'])
    run([vars.GIT_BINARY, '-C', endpath, 'fetch', '--depth', '1'], check=True)
    run([vars.GIT_BINARY, '-C', endpath, 'reset', '--hard', 'origin/HEAD'], check=True)