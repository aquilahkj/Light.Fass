using System;
using Microsoft.Extensions.Configuration;

namespace Light.Fass
{
    public class AuthSetting
    {
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


        public string Key
        {
            get;
            set;
        }

        public int OperatingValidTime
        {
            get;
            set;
        }
    }
}
