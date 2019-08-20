using System;
namespace Light.Fass
{
    public class AuthException : Exception
    {
        public AuthException(string message) : base(message)
        {
        }
    }

    public class RequestException : Exception
    {
        public RequestException(string message) : base(message)
        {
        }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
