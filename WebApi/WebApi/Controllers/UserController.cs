using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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
            var purchase=await _userService.AddToBasket(Purchase);
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
            var Purchase = await _userService.TicketPurchase(purchase);
            if (Purchase == null)
            {
                return BadRequest();
            }
            return Ok(Purchase);
        }

    }
}
