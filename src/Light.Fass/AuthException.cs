﻿using System;
namespace Light.Fass
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthException : Exception
    {
        public AuthException(string message) : base(message)
        {
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class RequestException : Exception
    {
        public RequestException(string message) : base(message)
        {
        }
    }
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
