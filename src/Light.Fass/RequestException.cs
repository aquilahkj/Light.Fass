using System;
namespace Light.Fass
{
    /// <summary>
    /// 
    /// </summary>
    public class RequestException : Exception
    {
        public RequestException(string message) : base(message)
        {
        }
    }
}
