using System;
namespace Light.Fass
{
    /// <summary>
    /// 
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
