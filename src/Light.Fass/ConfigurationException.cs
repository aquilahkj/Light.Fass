using System;
namespace Light.Fass
{
    /// <summary>
    /// 
    /// </summary>
    public class ConfigurationException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public ConfigurationException(string message) : base(message)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
