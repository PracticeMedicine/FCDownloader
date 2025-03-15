using System;
using Microsoft.Win32;

namespace FCDownloader;

public class ModManager : IDisposable
{
    private static ModManager? instance;
    public static ModManager Instance {
        get { return instance ?? (instance = new ModManager());} 
    }

    public void Dispose() {
        
    }
}