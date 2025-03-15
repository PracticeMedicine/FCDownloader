using System;
using System.Linq;

namespace AridityTeam.Base;

public class CommandLineUtil
{
    private string[]? _args { get; set; }
    public string[]? Args { get => _args; }

    public CommandLineUtil(string[]? args)
    {
        _args = args ?? throw new ArgumentNullException(nameof(args));
    }

    public bool FindParm(string? parm)
    {
        if (parm == null) throw new ArgumentNullException(nameof(parm));
        if (_args == null) throw new NullReferenceException("_args is null! Is CommandLineUtil constructed yet?");
        foreach (var arg in _args) {
            return arg.Equals(parm);
        }
        return false;
    }
}
