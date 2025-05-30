using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Authentication;

namespace BHHC.UW.PolicyCenter.API.V1.Extension
{
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
