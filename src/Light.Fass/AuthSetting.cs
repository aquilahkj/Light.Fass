using System;
using Microsoft.Extensions.Configuration;

namespace Light.Fass
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthSetting
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static AuthSetting LoadSettings(IConfiguration configuration)
        {
            var authSetting = new AuthSetting();
            var key = Environment.GetEnvironmentVariable("KEY");
            if (string.IsNullOrEmpty(key)) {
                key = configuration["Key"];
            }
            if (!string.IsNullOrEmpty(key)) {
                authSetting.Key = key;
            }
            else {
                throw new ConfigurationException("Key Variable does not exist");
            }

            try {
                var operatingValidTime = Environment.GetEnvironmentVariable("OPERATING_VALID_TIME");
                if (!string.IsNullOrEmpty(operatingValidTime)) {
                    authSetting.OperatingValidTime = int.Parse(operatingValidTime);
                }
                else {
                    authSetting.OperatingValidTime = configuration.GetValue<int>("OperatingValidTime");
                }

            }
            catch (Exception ex) {
                throw new ConfigurationException("OperatingValidTime Variable Error", ex);
            }

            return authSetting;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Key
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int OperatingValidTime
        {
            get;
            set;
        }
    }
}
