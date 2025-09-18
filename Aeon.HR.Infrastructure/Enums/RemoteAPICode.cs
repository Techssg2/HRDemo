using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Infrastructure.Enums
{
    public enum RemoteAPICode
    {
        [Description("Success")]
        OK = 200,
        [Description("Success of a resource creation when using the POST method")]
        Created = 201,
        [Description("The request parameters are incomplete or missing")]
        BadRequest = 400,
        [Description("The action or the request URI is not allowed by the system")]
        Forbidden = 403,
        [Description("The resource referenced by the URI was not found")]
        NotFound = 404,
        [Description("One of the requested action has generated an error")]
        UnprocessableEntity = 422,
        [Description("Your application is making too many requests and is being rate limited")]
        TooManyRequests = 429,
        [Description("Used in case of time out or when the request, otherwise correct, was not able to complete.")]
        InternalServerError = 500
    }
}
