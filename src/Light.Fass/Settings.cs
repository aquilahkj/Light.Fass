using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

namespace Light.Fass
{
    public class Settings
    {
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
            settings.AuthSetting = AuthSetting.LoadSettings(configuration);
            settings.FileSetting = FileSetting.LoadSettings(configuration);
            settings.ThumbnailSetting = ThumbnailSetting.LoadSettings(configuration);
            settings.MimeSetting = MimeSetting.LoadSettings(configuration);
            settings.CacheSetting = CacheSetting.LoadSettings(configuration);
            return settings;
        }

        public bool UseLog
        {
            get;
            set;
        }

        public long? MaxUploadSize
        {
            get;
            set;
        }

        public AuthSetting AuthSetting
        {
            get;
            set;
        }

        public FileSetting FileSetting
        {
            get;
            set;
        }

        public ThumbnailSetting ThumbnailSetting
        {
            get;
            set;
        }

        public MimeSetting MimeSetting
        {
            get;
            set;
        }

        public CacheSetting CacheSetting
        {
            get;
            set;
        }
    }
}
