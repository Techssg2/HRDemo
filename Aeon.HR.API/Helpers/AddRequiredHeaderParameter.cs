using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Description;

namespace Aeon.HR.API.Helpers
{
    public class AddRequiredHeaderParameter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.parameters == null)
                operation.parameters = new List<Parameter>();

            operation.parameters.Add(new Parameter
            {
                name = "secret",
                @in = "header",
                type = "string",
                description = "secret: <secret>",
                @default= "",
                required = true
            });
            operation.parameters.Add(new Parameter
            {
                name = "uxr",
                @in = "header",
                type = "string",
                description = "user login name",
                @default="<user name>",
                required = true
            });
        }
    }
}