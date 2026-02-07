using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;
using WebApi.Interface;
using WebApi.Models;
using WebApi.Service;
using WebApi.Validation;
using static WebApi.DTOs.DonorDTO;
using static WebApi.DTOs.GiftDTO;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GiftController : ControllerBase
    {
        private readonly IGiftService _giftService;
        GiftController(IGiftService giftService)
        {
            _giftService = giftService;
        }
        [RandomValidation]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GiftCategoryDto?>>> GetAllGift()
        {
            var result= await _giftService.GetAllGift();
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [RandomValidation]
        [HttpGet]
        [Route("Name")]

        public async Task<ActionResult<GiftResponseDto>> GetGiftByName(string name)
        {
            var result = await _giftService.GetGiftByName(name);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [RandomValidation]
        [HttpGet]
        [Route("Donor")]

        public async Task<ActionResult<IEnumerable<GiftResponseDto>>> GetGiftByDonor(string firstName, string lastName) {

            var result =await _giftService.GetGiftByDonor(firstName, lastName);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [RandomValidation]
        [HttpGet]
        [Route("Count")]
        public async Task<ActionResult<IEnumerable<GiftResponseDto>>> GetGiftByNumPurchase(int num)
        {
            var result =await _giftService.GetGiftByNumPurchase(num);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [RandomValidation]
        [HttpPost]
        [Route("Category")]

        public async Task<ActionResult<Category>> CreateCategory([FromBody] CateroyFormDto CategoryForm)
        {
            try
            {
                var categoty = await _giftService.CreateCategory(CategoryForm);
                return categoty;
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { massage = ex.Message });
            }
        }
        [Authorize(Roles = "Admin")]
        [RandomValidation]
        [HttpPost]
        public async Task<ActionResult<GiftResponseDto>> Create([FromBody] GiftFormDto GiftForm)
        {
            try
            {
                var gift = await _giftService.CreateGift(GiftForm);
                return gift;
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { massage = ex.Message });
            }
        }
        [Authorize(Roles = "Admin")]
        [RandomValidation]
        [HttpPut]
        public async Task<ActionResult<GiftResponseDto>> Update(int Id, [FromBody] GiftResponseDto giftForm)
        {
            try
            {
                var gift = await _giftService.UpdateGift(Id, giftForm);
                if (gift == null)
                {
                    return NotFound(new { massage = $"Gift with Id{Id}not dound" });

                }
                return Ok(gift);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { massage = ex.Message });
            }
        }
        [Authorize(Roles = "Admin")]
        [RandomValidation]
        [HttpDelete]
        public async Task<ActionResult> Delete(int Id)
        {


            var result = await _giftService.DeleteGift(Id);
            if (result == false)
            {
                return NotFound(new { massage = $"Gift with Id{Id}not found" });
            }
            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [RandomValidation]
        [HttpGet]
        [Route("Purchase&Gift")]
        public async Task<IActionResult> GetGiftPurchases([FromBody] int giftId)
        {
            var result = await _giftService.GetGiftPurchases(giftId);

            if (result == null)
                return NotFound($"Gift with Id{giftId}not found");

            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [RandomValidation]
        [HttpGet]
        [Route(("purchases/sort-by-price"))]
        public async Task<ActionResult<List<GiftPurchasesDto>>> GetGiftsByPrice()
        {
            var result = await _giftService.GetGiftsSortedByPrice();
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("purchases/sort-by-most-purchased")]
        public async Task<ActionResult<List<GiftPurchasesDto>>> GetGiftsByMostPurchased()
        {
            var result = await _giftService.GetGiftsSortedByMostPurchased();
            return Ok(result);
        }
        [RandomValidation]
        [HttpGet("/purchases-with-users")]
        public async Task<ActionResult<GiftPurchasesWithUsersDto>> GetGiftPurchasesWithUsers([FromBody] int giftId)
        {
            var result = await _giftService.GetGiftPurchasesWithUsers(giftId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Random")]
        public async Task<ActionResult<IEnumerable<GiftWinnerDto>>> GiftRandom()
        {
            var result = await _giftService.GiftRandom();
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [RandomValidation]
        [HttpGet]
        [Route("TotalSum")]
        public async Task<ActionResult<TotalSumDto>> GetTotalSum()
        {
            var result = await _giftService.GetTotalSum();
            return Ok(result);
        }
        [RandomValidation]
        [HttpGet]
        [Route("GetBySorted")]
        public async Task<ActionResult<IEnumerable<GiftCategoryDto?>>> SortedGiftByPriceOrCategory([FromRoute]string sortedBy)
        {

            var result = await _giftService.SortedGiftByPriceOrCategory(sortedBy);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

    }

}