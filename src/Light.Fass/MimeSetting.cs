using System;
using Microsoft.Extensions.Configuration;

namespace Light.Fass
{
    /// <summary>
    /// 
    /// </summary>
    public class MimeSetting
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static MimeSetting LoadSettings(IConfiguration configuration)
        {
            var mimeSetting = new MimeSetting();
            var defaultMime = Environment.GetEnvironmentVariable("DEFAULT_MIME");
            if (string.IsNullOrEmpty(defaultMime)) {
                defaultMime = configuration["DefaultMime"];
            }
            if (!string.IsNullOrEmpty(defaultMime)) {
                mimeSetting.DefaultMime = defaultMime;
            }


            try {
                var mimeInfos = configuration.GetSection("MimeInfos").Get<MimeInfo[]>();
                mimeSetting.MimeInfos = mimeInfos ?? new MimeInfo[0];
            }
            catch (Exception ex) {
                throw new ConfigurationException("MimeInfos Variable Error", ex);
            }

            return mimeSetting;
        }

        /// <summary>
        /// 
        /// </summary>
        public MimeInfo[] MimeInfos
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string DefaultMime
        {
            get;
            set;
        }
    }
}
