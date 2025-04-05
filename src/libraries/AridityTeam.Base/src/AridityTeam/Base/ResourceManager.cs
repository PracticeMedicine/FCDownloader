using System;
using System.Collections.Concurrent;
using AridityTeam.Base.ProcessUtil;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace AridityTeam.Base
{
    public class ResourceManager
    {
        private readonly ConcurrentDictionary<string, IntPtr> _cache = new();
        private readonly Memory _memory = new();
        private readonly int _maxCacheSize = 100 * 1024 * 1024; // 100 MB
        private int _currentCacheSize;

        public async Task<IntPtr> LoadResourceAsync(string filePath, bool precache = false)
        {
            if (_cache.TryGetValue(filePath, out var cachedPtr))
            {
                return cachedPtr; // Return cached data if available
            }

            if (precache)
            {
                // Precache the resource
                return await LoadAndCacheResourceAsync(filePath);
            }

            // Lazy load the resource
            return await LoadResourceFromDiskAsync(filePath);
        }

        private async Task<IntPtr> LoadAndCacheResourceAsync(string filePath)
        {
            var dataPtr = await LoadResourceFromDiskAsync(filePath);
            if (dataPtr == IntPtr.Zero) return dataPtr;
            var dataSize = GetResourceSize(filePath);
            if (_currentCacheSize + dataSize > _maxCacheSize)
            {
                EvictOldResources(); // Free up space in the cache
            }

            _cache[filePath] = dataPtr;
            _currentCacheSize += dataSize;
            return dataPtr;
        }

        private async Task<IntPtr> LoadResourceFromDiskAsync(string filePath)
        {
            try
            {
                var data = await File.ReadAllBytesAsync(filePath);
                if (_memory.UncheckedMalloc(data.Length, out var dataPtr))
                {
                    // Copy the data to the allocated memory
                    for (var i = 0; i < data.Length; i++)
                    {
                        Marshal.WriteByte(dataPtr, i, data[i]);
                    }
                    return dataPtr;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load resource: {ex.Message}");
            }
            return IntPtr.Zero;
        }

        private int GetResourceSize(string filePath)
        {
            try
            {
                return (int)new FileInfo(filePath).Length;
            }
            catch
            {
                return 0;
            }
        }

        private void EvictOldResources()
        {
            // Implement an eviction policy (e.g., LRU)
            // For simplicity, this example removes the first item in the cache
            foreach (var key in _cache.Keys)
            {
                if (_cache.TryRemove(key, out var dataPtr))
                {
                    var dataSize = GetResourceSize(key);
                    _currentCacheSize -= dataSize;
                    _memory.Free(dataPtr); // Free the memory
                    break;
                }
            }
        }

        public void FreeResource(string filePath)
        {
            if (_cache.TryRemove(filePath, out var dataPtr))
            {
                var dataSize = GetResourceSize(filePath);
                _currentCacheSize -= dataSize;
                _memory.Free(dataPtr); // Free the memory
            }
        }

        public void FreeAllResources()
        {
            foreach (var key in _cache.Keys)
            {
                if (_cache.TryRemove(key, out var dataPtr))
                {
                    _memory.Free(dataPtr); // Free the memory
                }
            }
            _cache.Clear();
            _currentCacheSize = 0;
        }
    }
}
