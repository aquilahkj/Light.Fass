using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Light.Fass
{
    /// <summary>
    /// Mvc extensions.
    /// </summary>
    public static class ModelStateExtensions
    {
        /// <summary>
        /// Adds the invalid model state exception.
        /// </summary>
        /// <param name="options">Options.</param>
        public static void UseInvalidModelStateException(this ApiBehaviorOptions options)
        {
            options.InvalidModelStateResponseFactory += (ActionContext arg) => {
                var state = arg.ModelState;
                var ie = state as IEnumerable<KeyValuePair<string, ModelStateEntry>>;
                var sb = new StringBuilder();
                foreach (var item in ie) {
                    sb.Append($"{item.Key}:{item.Value.Errors[0].ErrorMessage};");

                }
                throw new ParameterException(sb.ToString());
            };
        }
    }
}
