using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }


        [RandomValidation]
        [HttpPost("AddToBasket")]
        public async Task<ActionResult<Purchase?>> AddToBasket([FromBody] PurchaseBasketDto purchase)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                var result = await _userService.AddToBasket(purchase, userId);

                if (result == null) return BadRequest("User or Gift not found.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [RandomValidation]
        [HttpPost("TicketPurchase")]
        public async Task<ActionResult<List<PurchaseWithUserDto>>> TicketPurchase([FromBody] PurchaseBasketDto purchase)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _userService.TicketPurchase(purchase, userId);

            if (!result.Any())
                return BadRequest("Purchase failed.");

            return Ok(result);
        }
        [RandomValidation]
        [HttpPost("ConfirmBasket")]
        public async Task<IActionResult> ConfirmBasket()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _userService.ConfirmBasket(userId);

            if (!result)
                return BadRequest("Basket cannot be confirmed or is empty.");

            return Ok("Basket confirmed successfully.");
        }


        [HttpDelete("RemoveFromBasket/{purchaseId}")]
        public async Task<IActionResult> RemoveFromBasket(int purchaseId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            try
            {
                var result = await _userService.DeleteFromBasket(purchaseId, userId);
                if (!result)
                    return NotFound("Purchase not found or access denied.");

                return Ok("Item removed from basket.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GiftsWithWinners")]
        [AllowAnonymous]
        public async Task<IActionResult> GetGiftsWithWinners()
        {
            _logger.LogInformation("Fetching gifts with their winners");
            var results = await _userService.GetGiftsWithWinners();
            return Ok(results);
        }
    }
}
