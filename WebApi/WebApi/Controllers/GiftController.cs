using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
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
        private readonly ILogger<GiftController> _logger;

        public GiftController(IGiftService giftService, ILogger<GiftController> logger)
        {
            _giftService = giftService;
            _logger = logger;
        }

        //[ServiceFilter(typeof(RandomValidationAttribute))]

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GiftCategoryDto?>>> GetAllGift()
        {
            _logger.LogInformation("GetAllGift called");
            var result = await _giftService.GetAllGift();
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        //[ServiceFilter(typeof(RandomValidationAttribute))]
        [HttpGet]
        [Route("Name")]
        public async Task<ActionResult<GiftResponseDto>> GetGiftByName(string name)
        {
            _logger.LogInformation("GetGiftByName called with name: {name}", name);
            var result = await _giftService.GetGiftByName(name);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        //[ServiceFilter(typeof(RandomValidationAttribute))]
        [HttpGet]
        [Route("Donor")]
        public async Task<ActionResult<IEnumerable<GiftResponseDto>>> GetGiftByDonor(string firstName, string lastName)
        {
            _logger.LogInformation("GetGiftByDonor called with firstName: {firstName}, lastName: {lastName}", firstName, lastName);
            var result = await _giftService.GetGiftByDonor(firstName, lastName);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(RandomValidationAttribute))]
        [HttpGet]
        [Route("Count")]
        public async Task<ActionResult<IEnumerable<GiftResponseDto>>> GetGiftByNumPurchase(int num)
        {
            _logger.LogInformation("GetGiftByNumPurchase called with num: {num}", num);
            var result = await _giftService.GetGiftByNumPurchase(num);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        //[ServiceFilter(typeof(RandomValidationAttribute))]
        [HttpPost]
        [Route("Category")]
        public async Task<ActionResult<Category>> CreateCategory([FromBody] CateroyFormDto CategoryForm)
        {
            _logger.LogInformation("CreateCategory called");
            try
            {
                var category = await _giftService.CreateCategory(CategoryForm);
                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating category");
                return StatusCode(500, new { message = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [Authorize(Roles = "Admin")]
        //[ServiceFilter(typeof(RandomValidationAttribute))]
        [HttpPost]
        public async Task<ActionResult<GiftResponseDto>> Create([FromBody] GiftFormDto GiftForm)
        {
            _logger.LogInformation("Create called");
            try
            {
                var gift = await _giftService.CreateGift(GiftForm);
                return gift;
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while creating gift");
                return BadRequest(new { massage = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        //[ServiceFilter(typeof(RandomValidationAttribute))]
        [HttpPut("{id}")]
        public async Task<ActionResult<GiftResponseDto>> Update(int Id, [FromBody] GiftResponseDto giftForm)
        {
            _logger.LogInformation("Update called with Id: {Id}", Id);
            try
            {
                var gift = await _giftService.UpdateGift(Id, giftForm);
                if (gift == null)
                {
                    _logger.LogWarning("Gift with Id {Id} not found", Id);
                    return NotFound(new { massage = $"Gift with Id{Id}not dound" });
                }
                return Ok(gift);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error occurred while updating gift");
                return BadRequest(new { massage = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        //[ServiceFilter(typeof(RandomValidationAttribute))]
        [HttpDelete("{Id:int}")]
        public async Task<ActionResult> Delete(int Id)
        {
            _logger.LogInformation("Delete called with Id: {Id}", Id);
            var result = await _giftService.DeleteGift(Id);
            if (result == false)
            {
                _logger.LogWarning("Gift with Id {Id} not found", Id);
                return NotFound(new { massage = $"Gift with Id{Id}not found" });
            }
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        //[ServiceFilter(typeof(RandomValidationAttribute))]
        [HttpGet]
        [Route("Purchase&Gift")]
        public async Task<IActionResult> GetGiftPurchases(int giftId)
        {
            _logger.LogInformation("GetGiftPurchases called with giftId: {giftId}", giftId);
            var result = await _giftService.GetGiftPurchases(giftId);

            if (result == null)
            {
                _logger.LogWarning("Gift with Id {giftId} not found", giftId);
                return NotFound($"Gift with Id{giftId}not found");
            }

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        //[ServiceFilter(typeof(RandomValidationAttribute))]
        [HttpGet]
        [Route(("sort-by-price"))]
        public async Task<ActionResult<List<GiftPurchasesDto>>> GetGiftsByPrice()
        {
            _logger.LogInformation("GetGiftsByPrice called");
            var result = await _giftService.GetGiftsSortedByPrice();
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("sort-by-most-purchased")]
        public async Task<ActionResult<List<GiftPurchasesDto>>> GetGiftsByMostPurchased()
        {
            _logger.LogInformation("GetGiftsByMostPurchased called");
            var result = await _giftService.GetGiftsSortedByMostPurchased();
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(RandomValidationAttribute))]
        [HttpGet("purchases-with-users")]
        public async Task<ActionResult<GiftPurchasesWithUsersDto>> GetGiftPurchasesWithUsers(int giftId)
        {
            _logger.LogInformation("GetGiftPurchasesWithUsers called with giftId: {giftId}", giftId);
            var result = await _giftService.GetGiftPurchasesWithUsers(giftId);

            if (result == null)
            {
                _logger.LogWarning("Gift with Id {giftId} not found", giftId);
                return NotFound();
            }

            return Ok(result);
        }

        [ServiceFilter(typeof(RandomValidationAttribute))]
        [HttpGet]
        [Route("GetBySorted")]
        public async Task<ActionResult<IEnumerable<GiftCategoryDto?>>> SortedGiftByPriceOrCategory(string sortedBy)
        {
            _logger.LogInformation("SortedGiftByPriceOrCategory called with sortedBy: {sortedBy}", sortedBy);
            var result = await _giftService.SortedGiftByPriceOrCategory(sortedBy);
            if (result == null)
            {
                _logger.LogWarning("No gifts found for sortedBy: {sortedBy}", sortedBy);
                return NotFound();
            }
            return Ok(result);
        }

//Draw
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(RandomValidationAttribute))]
        [HttpPost("drawAllWinners")]
        public async Task<IActionResult> DrawAllWinners()
        {
            var allWinners = await _giftService.GenerateWinnersReport();

            if (allWinners == null || !allWinners.Any())
                return NotFound("לא נמצאו מתנות או רוכשים במערכת");

            return Ok(allWinners); 
        }
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(RandomValidationAttribute))]
        [HttpGet("generateWinnersFile")]
        public async Task<IActionResult> GenerateWinnersFile()
        {
            var winners = await _giftService.GenerateWinnersReport();
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            var jsonString = JsonSerializer.Serialize(winners, jsonOptions);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "WinnersReport.json");
            await System.IO.File.WriteAllTextAsync(filePath, jsonString, Encoding.UTF8);
            var fileBytes = Encoding.UTF8.GetBytes(jsonString);
            return File(fileBytes, "application/json", "WinnersReport.json");
        }
        [Authorize(Roles = "Admin")]
        [ServiceFilter(typeof(RandomValidationAttribute))]
        [HttpGet("generateTotalSumFile")]
        public async Task<IActionResult> GenerateRevenueFile()
        {
            var totalRevenue = await _giftService.GetTotalRevenue();

            var reportData = new
            {
                ReportDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm"),
                TotalRevenue = totalRevenue,
                Currency = "ILS"
            };

            var jsonString = JsonSerializer.Serialize(reportData, new JsonSerializerOptions { WriteIndented = true });
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "RevenueReport.json");

            await System.IO.File.WriteAllTextAsync(filePath, jsonString, Encoding.UTF8);

            return File(Encoding.UTF8.GetBytes(jsonString), "application/json", "RevenueReport.json");
        }
    }
}