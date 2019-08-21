using System;
using Microsoft.Extensions.Configuration;

namespace Light.Fass
{
    /// <summary>
    /// 
    /// </summary>
    public class CacheSetting
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static CacheSetting LoadSettings(IConfiguration configuration)
        {
            var cacheSetting = new CacheSetting();

            try {
                var uploadCacheTime = Environment.GetEnvironmentVariable("UPLOAD_CACHE_TIME");
                if (!string.IsNullOrEmpty(uploadCacheTime)) {
                    cacheSetting.UploadCacheTime = int.Parse(uploadCacheTime);
                }
                else {
                    cacheSetting.UploadCacheTime = configuration.GetValue<int>("UploadCacheTime");
                }
            }
            catch (Exception ex) {
                throw new ConfigurationException("UploadCacheTime Variable Error", ex);
            }

            try {
                var maxCacheCount = Environment.GetEnvironmentVariable("MAX_CACHE_COUNT");
                if (!string.IsNullOrEmpty(maxCacheCount)) {
                    cacheSetting.MaxCacheCount = int.Parse(maxCacheCount);
                }
                else {
                    cacheSetting.MaxCacheCount = configuration.GetValue<int>("MaxCacheCount");
                }

            }
            catch (Exception ex) {
                throw new ConfigurationException("MaxCacheCount Variable Error", ex);
            }

            try {
                var maxCacheTime = Environment.GetEnvironmentVariable("MAX_CACHE_TIME");
                if (!string.IsNullOrEmpty(maxCacheTime)) {
                    cacheSetting.MaxCacheTime = int.Parse(maxCacheTime);
                }
                else {
                    cacheSetting.MaxCacheTime = configuration.GetValue<int>("MaxCacheTime");
                }
            }
            catch (Exception ex) {
                throw new ConfigurationException("MaxCacheTime Variable Error", ex);
            }

            var maxCacheEntitySize = Environment.GetEnvironmentVariable("MAX_CACHE_ENTITY_SIZE");
            if (string.IsNullOrEmpty(maxCacheEntitySize)) {
                maxCacheEntitySize = configuration["MaxCacheEntitySize"];
            }
            if (!string.IsNullOrEmpty(maxCacheEntitySize)) {
                if (Utils.TryParseByteNumber(maxCacheEntitySize, out long value)) {
                    cacheSetting.MaxCacheEntitySize = value;
                }
                else {
                    throw new ConfigurationException("MaxCacheEntitySize Variable Error");
                }
            }

            return cacheSetting;
        }

        /// <summary>
        /// 
        /// </summary>
        public int UploadCacheTime
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int MaxCacheCount
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int MaxCacheTime
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public long MaxCacheEntitySize
        {
            get;
            set;
        }
    }
}