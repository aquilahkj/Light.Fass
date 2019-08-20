using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Light.Fass
{
    public class CacheModule
    {
        readonly MemoryCache uploadCache;

        readonly MemoryCache fileCache;

        readonly TimeSpan uploadTimespan;

        readonly TimeSpan maxCacheTimespan;

        readonly int maxCacheCount;

        readonly long maxCacheEntitySize;

        public CacheModule(CacheSetting cacheSetting)
        {
            uploadCache = new MemoryCache(new MemoryCacheOptions());
            fileCache = new MemoryCache(new MemoryCacheOptions());
            if (cacheSetting.UploadCacheTime > 0) {
                uploadTimespan = new TimeSpan(0, 0, cacheSetting.UploadCacheTime);
            }
            else {
                uploadTimespan = new TimeSpan(0, 0, 60);
            }
            if (cacheSetting.MaxCacheTime > 0) {
                maxCacheTimespan = new TimeSpan(0, cacheSetting.MaxCacheTime, 0);
            }
            else {
                maxCacheTimespan = new TimeSpan(0, 30, 0);
            }
            if (cacheSetting.MaxCacheCount > 0) {
                maxCacheCount = cacheSetting.MaxCacheCount;
            }
            else {
                maxCacheCount = 10000;
            }
            if (cacheSetting.MaxCacheCount > 0) {
                maxCacheEntitySize = cacheSetting.MaxCacheEntitySize;
            }
            else {
                maxCacheEntitySize = 5 * 1024 * 1024;
            }
        }

        public void SetUploadName(string token, string name)
        {
            uploadCache.Set(token, name, uploadTimespan);
        }

        public bool TryGetUploadName(string token, out string value)
        {
            return uploadCache.TryGetValue(token, out value);
        }

        public async Task<byte[]> GetAndSetFileCache(FileInfo fileInfo)
        {
            byte[] buffer;
            if (fileCache.Count > maxCacheCount || fileInfo.Length > maxCacheEntitySize) {
                buffer = null;
            }
            else {
                var key = fileInfo.Name;
                if (!fileCache.TryGetValue(key, out buffer)) {
                    buffer = new byte[fileInfo.Length];
                    using (var stream = fileInfo.OpenRead()) {
                        await stream.ReadAsync(buffer, 0, buffer.Length);
                    }
                    fileCache.Set(key, buffer, new MemoryCacheEntryOptions() {
                        SlidingExpiration = maxCacheTimespan
                    });
                }
            }
            return buffer;
        }

        public void RemoveFileCache(string fileName)
        {
            fileCache.Remove(fileName);
        }
    }
}
