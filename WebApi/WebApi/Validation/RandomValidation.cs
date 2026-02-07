using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using WebApi.Data;
using WebApi.Models;


namespace WebApi.Validation
{
    public class RandomValidationAttribute : Attribute,IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {

            if (Gift.IsRandom == true)
            {
                context.Result = new BadRequestObjectResult(
                    "The lottery has already been held – tickets cannot be purchased."
                );
                return;
            }
            await next();
        }
    }
}
