using System;
using System.Text.RegularExpressions;

namespace Light.Fass
{
    /// <summary>
    /// 
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryParseByteNumber(string content, out long value)
        {
            var regex = new Regex(@"^(?<num>\d+)(?<unit>[kmg]?)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            var match = regex.Match(content);
            if (match.Success) {
                var num = Convert.ToInt64(match.Groups["num"].Value);
                var unit = match.Groups["unit"].Value;
                switch (unit) {
                    case "K":
                    case "k":
                        value = num * 1024;
                        break;
                    case "M":
                    case "m":
                        value = num * 1024 * 1024;
                        break;
                    case "G":
                    case "g":
                        value = num * 1024 * 1024 *1024;
                        break;
                    default:
                        value = num;
                        break;
                }
                return true;
            }
            else {
                value = 0L;
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static long GetUtcTimestamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return (long)ts.TotalSeconds;//获取10位
        }
    }
}
