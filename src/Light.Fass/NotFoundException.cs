using System;
namespace Light.Fass
{
    /// <summary>
    /// 
    /// </summary>
    public class NotFoundException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
