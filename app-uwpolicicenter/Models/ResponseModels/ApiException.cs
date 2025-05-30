using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Authentication;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BHHC.UW.PolicyCenter.API.V1.Application.Models
{
    public class ApiException
    {
        [JsonProperty("isSystemException")]
        public bool IsSystemException { get; set; }

        [JsonProperty("errors")]
        public IList<Error>? Errors { get; set; }

        [JsonProperty("systemException")]
        public ProblemDetails? SystemException { get; set; }

        /// <summary>
        /// Create an empty ApiException.
        /// </summary>
        public ApiException() { }

        /// <summary>
        /// Create an ApiException with error list and/or system exception.
        /// </summary>
        public ApiException(IList<Error>? err = null, Exception? exception = null)
        {
            Errors = err;
            IsSystemException = exception != null;
            //SystemException = exception?.ToProblemDetails();
        }
    }
    public static class ExceptionExtensions
    {
        public static ProblemDetails ToProblemDetails(this Exception e)
        {
            return new ProblemDetails
            {
                Title = e.Message,
                Detail = $"Source: {e.Source}",
                Status = (int)GetErrorCode(e.InnerException ?? e)
            };
        }

        private static HttpStatusCode GetErrorCode(Exception e)
        {
            return e switch
            {
                ValidationException => HttpStatusCode.BadRequest,
                FormatException => HttpStatusCode.BadRequest,
                AuthenticationException => HttpStatusCode.Forbidden,
                NotImplementedException => HttpStatusCode.NotImplemented,
                _ => HttpStatusCode.InternalServerError
            };
        }
    }
}
