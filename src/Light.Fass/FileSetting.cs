using System;
using Microsoft.Extensions.Configuration;

namespace Light.Fass
{
    /// <summary>
    /// 
    /// </summary>
    public class FileSetting
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static FileSetting LoadSettings(IConfiguration configuration)
        {
            var fileSetting = new FileSetting();
            var fileDirectory = Environment.GetEnvironmentVariable("FILE_DIRECTORY");
            if (string.IsNullOrEmpty(fileDirectory)) {
                fileDirectory = configuration["FileDirectory"];
            }
            if (!string.IsNullOrEmpty(fileDirectory)) {
                fileSetting.FileDirectory = fileDirectory;
            }

            var allowExts = Environment.GetEnvironmentVariable("ALLOW_EXTS");
            if (string.IsNullOrEmpty(allowExts)) {
                allowExts = configuration["AllowExts"];
            }
            if (!string.IsNullOrEmpty(allowExts)) {
                fileSetting.AllowExts = allowExts.Split(',', StringSplitOptions.RemoveEmptyEntries);
            }
            else {
                fileSetting.AllowExts = new string[0];
            }
            return fileSetting;
        }
        /// <summary>
        /// 
        /// </summary>
        public string FileDirectory
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public string[] AllowExts
        {
            get;
            set;
        }
    }
}
