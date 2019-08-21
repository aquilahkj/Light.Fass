using System;
namespace Light.Fass
{
    /// <summary>
    /// 
    /// </summary>
    public class RequestException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public RequestException(string message) : base(message)
        {
        }
    }
}
