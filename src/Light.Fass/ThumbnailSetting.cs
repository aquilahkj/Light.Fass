﻿using System;
using Microsoft.Extensions.Configuration;

namespace Light.Fass
{
    /// <summary>
    /// 
    /// </summary>
    public class ThumbnailSetting
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static ThumbnailSetting LoadSettings(IConfiguration configuration)
        {
            var thumbnailSetting = new ThumbnailSetting();
            var thumbnailDirectory = Environment.GetEnvironmentVariable("THUMBNAIL_DIRECTORY");
            if (thumbnailDirectory == null) {
                thumbnailDirectory = configuration["ThumbnailDirectory"];
            }
            if (thumbnailDirectory != null) {
                thumbnailSetting.ThumbnailDirectory = thumbnailDirectory;
            }

            var thumbnailExts = Environment.GetEnvironmentVariable("THUMBNAIL_EXTS");
            if (thumbnailExts == null) {
                thumbnailExts = configuration["ThumbnailExts"];
            }
            if (thumbnailExts != null) {
                thumbnailSetting.ThumbnailExts = thumbnailExts.Split(',', StringSplitOptions.RemoveEmptyEntries);
            }
            else {
                thumbnailSetting.ThumbnailExts = new string[0];
            }

            try {
                var thumbnailTypes = configuration.GetSection("ThumbnailTypes").Get<ThumbnailType[]>();
                thumbnailSetting.Types = thumbnailTypes ?? new ThumbnailType[0];
            }
            catch (Exception ex) {
                throw new ConfigurationException("ThumbnailTypes Variable Error", ex);
            }
            return thumbnailSetting;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ThumbnailDirectory
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string[] ThumbnailExts
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public ThumbnailType[] Types
        {
            get;
            set;
        }
    }
}
