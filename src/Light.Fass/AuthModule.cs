using System;
using System.Security.Cryptography;
using System.Text;

namespace Light.Fass
{
    public class AuthModule
    {
        private readonly string key;

        private readonly int operatingValidTime;

        public AuthModule(AuthSetting authSetting)
        {
            this.key = authSetting.Key;
            if (authSetting.OperatingValidTime > 0) {
                this.operatingValidTime = authSetting.OperatingValidTime;
            }
            else {
                this.operatingValidTime = 60;
            }
        }

        public bool ValidDeleteToken(string user, string fileName, long timestamp, string token)
        {
            if (string.IsNullOrEmpty(token)) {
                return false;
            }
            if (string.IsNullOrEmpty(user)) {
                return false;
            }
            if (string.IsNullOrEmpty(fileName)) {
                return false;
            }
            var data = $"delete|{user}|{fileName}|{timestamp}|{key}";
            var result = Encrypt(data);
            if (result != token) {
                return false;
            }
            var current = GetUtcTimestamp();
            var diff = current - timestamp;
            return diff >= 0 && diff <= operatingValidTime;
        }

        public bool ValidUploadToken(string user, string fileName, int protect, long timestamp, string token)
        {
            if (string.IsNullOrEmpty(token)) {
                return false;
            }
            if (string.IsNullOrEmpty(user)) {
                return false;
            }
            var data = $"upload|{user}|{fileName}|{protect}|{timestamp}|{key}";
            var result = Encrypt(data);
            if (result != token) {
                return false;
            }
            var current = GetUtcTimestamp();
            var diff = current - timestamp;
            return diff >= 0 && diff <= operatingValidTime;
        }

        public bool ValidAccessToken(string user, string fileName, long expired, string token)
        {
            if (string.IsNullOrEmpty(token)) {
                return false;
            }
            if (string.IsNullOrEmpty(fileName)) {
                return false;
            }
            if (string.IsNullOrEmpty(user)) {
                return false;
            }
            var data = $"access|{user}|{fileName}|{expired}|{key}";
            var result = Encrypt(data);
            if (result != token) {
                return false;
            }
            var current = GetUtcTimestamp();
            return expired >= current;
        }

        /// <summary>
        /// Encrypts Md5.
        /// </summary>
        /// <returns>The password.</returns>
        /// <param name="content">Password.</param>
        private static string Encrypt(string content)
        {
            var sha256 = new SHA256Managed();
            var encryptedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(content));
            var sb = new StringBuilder();
            for (int i = 0; i < encryptedBytes.Length; i++) {
                sb.AppendFormat("{0:x2}", encryptedBytes[i]);
            }
            return sb.ToString();
        }


        private static long GetUtcTimestamp()
        {
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return (long)ts.TotalSeconds;//获取10位
        }
    }
}
