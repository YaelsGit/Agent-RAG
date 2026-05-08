using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using WebApi.Data;

namespace WebApi.Validation
{
    public class RandomValidationAttribute : IAsyncActionFilter
    {
        private readonly WebApiContext _context;

        public RandomValidationAttribute(WebApiContext context)
        {
            _context = context;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            // בדיקה אם כבר בוצעה הגרלה
            var lotteryDone = await _context.Gifts.AnyAsync(g => g.IsRandom);

            if (lotteryDone)
            {
                context.Result = new BadRequestObjectResult(
                    "ההגרלה כבר בוצעה – לא ניתן לבצע פעולות נוספות במערכת."
                );
                return;
            }

            await next();
        }
    }
}