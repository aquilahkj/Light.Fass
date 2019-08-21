using System;
namespace Light.Fass
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public AuthException(string message) : base(message)
        {
        }
    }
}
