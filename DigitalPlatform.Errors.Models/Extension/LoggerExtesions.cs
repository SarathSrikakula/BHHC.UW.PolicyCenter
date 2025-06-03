using DigitalPlatform.Errors.Models.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalPlatform.Errors.Models.Extension
{
    public static class LoggerExtensions
    {
        /// <summary>
        /// Logs a custom error and returns a formatted error string.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="customError">The custom error object.</param>
        /// <param name="logLevel">The log level.</param>
        /// <param name="exception">The exception to log.</param>
        /// <param name="methodName">The method name where the error occurred.</param>
        /// <returns>A formatted error string for client response.</returns>
        public static string LogCustomError(this ILogger logger, CustomError customError, LogLevel logLevel, Exception exception, string methodName, object value)
        {
            var errorDescription = customError.IncludeErrorDescription
                ? $"{customError.ErrorCategory}: {customError.AppErrorCode} - {exception.Message}"
                : $"{customError.ErrorCategory}: {customError.AppErrorCode}";

            logger.Log(logLevel, exception, "Error in {MethodName}: {ErrorDescription}", methodName, errorDescription);

            return errorDescription;
        }
    }
}
