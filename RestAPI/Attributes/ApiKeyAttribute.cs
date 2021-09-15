using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

namespace RestAPI.Attributes
{
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        { 
        var apiKey = context.HttpContext.Request.Headers["ApiKey"].SingleOrDefault();

             if (string.IsNullOrWhiteSpace(apiKey))
	          {
                  context.Result = new BadRequestObjectResult("ApiKey header is missing");

                   return;
	           }

            var usersRepository = context.HttpContext.RequestServices.GetService<IUsersRepository>();
            await next();
        }
    }
}
