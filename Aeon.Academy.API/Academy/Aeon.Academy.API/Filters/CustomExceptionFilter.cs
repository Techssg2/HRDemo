using Aeon.Academy.Services;
using Aeon.HR.Infrastructure.Utilities;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace Aeon.Academy.API.Filters
{
    public class CustomExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            HttpResponseMessage response = null;
            var exception = actionExecutedContext.Exception;
            if(exception is UnauthorizedAccessException)
            {
                response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
            else
            {
                response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An unhandled exception was thrown by service." + actionExecutedContext.Exception.Message),
                    ReasonPhrase = "Internal Server Error.Please Contact your Administrator."
                };

                var logger = ServiceLocator.Resolve<ILogger>();
                logger.LogError(exception);
            } 
            actionExecutedContext.Response = response;
        }
    }
}