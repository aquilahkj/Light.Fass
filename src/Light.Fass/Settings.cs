using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

namespace Light.Fass
{
    /// <summary>
    /// 
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static Settings LoadSettings(IConfiguration configuration)
        {
            var settings = new Settings();
            var size = Environment.GetEnvironmentVariable("MAX_UPLOAD_SIZE");
            if (string.IsNullOrEmpty(size)) {
                size = configuration["MaxUploadSize"];
            }
            if (!string.IsNullOrEmpty(size)) {
                if (Utils.TryParseByteNumber(size, out long value)) {
                    settings.MaxUploadSize = value;
                }
                else {
                    throw new ConfigurationException("MaxUploadSize Variable Error");
                }
            }

            try {
                var useLog = Environment.GetEnvironmentVariable("USE_LOG");
                if (!string.IsNullOrEmpty(useLog)) {
                    settings.UseLog = bool.Parse(useLog);
                }
                else {
                    settings.UseLog = configuration.GetValue<bool>("UseLog");
                }
            }
            catch (Exception ex) {
                throw new ConfigurationException("UseLog Variable Error", ex);
            }

            try {
                var useSwagger = Environment.GetEnvironmentVariable("USE_SWAGGER");
                if (!string.IsNullOrEmpty(useSwagger)) {
                    settings.UseSwagger = bool.Parse(useSwagger);
                }
                else {
                    settings.UseSwagger = configuration.GetValue<bool>("UseSwagger");
                }
            }
            catch (Exception ex) {
                throw new ConfigurationException("UseSwagger Variable Error", ex);
            }
            settings.AuthSetting = AuthSetting.LoadSettings(configuration);
            settings.FileSetting = FileSetting.LoadSettings(configuration);
            settings.ThumbnailSetting = ThumbnailSetting.LoadSettings(configuration);
            settings.MimeSetting = MimeSetting.LoadSettings(configuration);
            settings.CacheSetting = CacheSetting.LoadSettings(configuration);
            return settings;
        }
        /// <summary>
        /// 
        /// </summary>
        public bool UseSwagger
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public bool UseLog
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public long? MaxUploadSize
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public AuthSetting AuthSetting
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public FileSetting FileSetting
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public ThumbnailSetting ThumbnailSetting
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public MimeSetting MimeSetting
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public CacheSetting CacheSetting
        {
            get;
            set;
        }
    }
}
