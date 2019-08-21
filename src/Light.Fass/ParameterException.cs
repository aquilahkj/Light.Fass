using System;
namespace Light.Fass
{
    /// <summary>
    /// Parameter exception.
    /// </summary>
    public class ParameterException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Light.Oaks.ParameterException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public ParameterException(string message) : base(message)
        {
        }

        ///// <summary>
        ///// Gets the errors.
        ///// </summary>
        ///// <value>The errors.</value>
        //public ErrorData[] Errors { get; }

        ///// <summary>
        ///// Initializes a new instance of the <see cref="T:Light.Oaks.ParameterException"/> class.
        ///// </summary>
        ///// <param name="message">Message.</param>
        ///// <param name="errors">Errors.</param>
        //public ParameterException(string message, ErrorData[] errors) : base(message)
        //{
        //    this.Errors = errors;
        //}
    }
}
