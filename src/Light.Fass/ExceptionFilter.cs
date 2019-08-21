using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Light.Fass
{
    class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger logger;

        public ExceptionFilter(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger("Error");
        }

        public void OnException(ExceptionContext context)
        {
            Exception ex = context.Exception;
            if (ex is AggregateException) {
                ex = ex.InnerException;
            }
            var httpContext = context.HttpContext;
            var traceId = Guid.NewGuid().ToString("D");

            var logdetail = false;
            var code = 500;
            var message = "Unknow Error";
            if (ex is AuthException) {
                code = 401;
                message = ex.Message;
            }
            else if (ex is RequestException) {
                code = 400;
                message = ex.Message;
            }
            else if (ex is NotFoundException) {
                code = 404;
                message = ex.Message;
            }
            else if (ex is ParameterException) {
                code = 400;
                message = ex.Message;
            }
            else if (ex is BadHttpRequestException badHttpRequestException) {
                code = badHttpRequestException.StatusCode;
                message = ex.Message;
            }
            else {
                logdetail = true;
            }
            if (logdetail) {
                logger.LogError(ex, $"{traceId}|{httpContext.Request.Method}:{httpContext.Request.Path}{httpContext.Request.QueryString}");
            }
            else {
                logger.LogError($"{traceId}|{httpContext.Request.Method}:{httpContext.Request.Path}{httpContext.Request.QueryString}|{message}");
            }
            context.HttpContext.Response.StatusCode = code;
            context.Result = new JsonResult(new { traceId, message });
        }
    }
}