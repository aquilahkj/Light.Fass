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
}
