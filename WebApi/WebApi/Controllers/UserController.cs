using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApi.Interface;
using WebApi.Models;
using WebApi.Service;
using WebApi.Validation;
using static WebApi.DTOs.PurchaseDTO;
using static WebApi.DTOs.UserDTO;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [RandomValidation]

    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost]
        public async Task<ActionResult<PurchaseBasketDto?>> AddToBasket(PurchaseBasketDto Purchase)
        {
            var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value
           );
            var purchase=await _userService.AddToBasket(Purchase,userId);
            if (purchase == null)
            {
                return BadRequest();
            }
            return Ok(purchase);
        }
        [HttpPost]
        [Route("TicketPurchase")]
        public async Task<ActionResult<PurchaseWithUserDto?>>TicketPurchase([FromBody] PurchaseBasketDto purchase)
        {
            var userId = int.Parse(
             User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );
            var Purchase = await _userService.TicketPurchase(purchase,userId);
            if (Purchase == null)
            {
                return BadRequest();
            }
            return Ok(Purchase);
        }
        [HttpPost("ConfirmBasket")]
        public async Task<IActionResult> ConfirmBasket()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _userService.ConfirmBasket(userId);
            if (!result)
                return BadRequest("לא ניתן לאשר סל: ייתכן שכבר אושר או שההגרלה הסתיימה.");
            return Ok();
        }

        [HttpGet("Winners")]
        [AllowAnonymous]
        public async Task<IActionResult> GetWinners()
        {
            var winners = await _userService.GetWinners();
            return Ok(winners);
        }
    }
}
