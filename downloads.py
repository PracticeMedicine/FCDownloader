from os import path
from subprocess import run
from platform import system
import vars

def clone(endpath):
    run([vars.GIT_BINARY, 'config', '--global', 'http.postBuffer', vars.GitPostBuffer])
    run([vars.GIT_BINARY, 'config', '--global', 'http.maxRequestBuffer', vars.GitPostBuffer])
    run([vars.GIT_BINARY, 'config', '--global', 'core.compression', '9'])
    run([vars.GIT_BINARY, 'config', '--system', 'credential.helper', 'manager-core'])
    run([vars.GIT_BINARY, 'clone', '--depth', '1', vars.GitRepo, endpath], check=True)

def pull(endpath):
    run([vars.GIT_BINARY, 'config', '--global', 'http.postBuffer', vars.GitPostBuffer])
    run([vars.GIT_BINARY, 'config', '--global', 'http.maxRequestBuffer', vars.GitPostBuffer])
    run([vars.GIT_BINARY, 'config', '--global', 'core.compression', '9'])
    run([vars.GIT_BINARY, 'config', '--system', 'credential.helper', 'manager-core'])
    run([vars.GIT_BINARY, '-C', endpath, 'fetch', '--depth', '1'], check=True)
    run([vars.GIT_BINARY, '-C', endpath, 'reset', '--hard', 'origin/HEAD'], check=True)