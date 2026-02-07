using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;
using WebApi.Interface;
using WebApi.Models;
using WebApi.Service;
using WebApi.Validation;
using static WebApi.DTOs.DonorDTO;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [RandomValidation]
    [Authorize(Roles = "Admin")]

    public class DonorController : ControllerBase
    {
        private readonly IDonorService _donorService;
        public DonorController(IDonorService donorService)
        {
            _donorService = donorService;
        }
        [HttpPost]
        public async Task<ActionResult<DonorDto>> Create([FromBody] DonorFormDto donorForm)
        {
            try
            {
                var donor = await _donorService.CreatDonor(donorForm);
                return donor;
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { massage = ex.Message });
            }
        }
        [HttpPut]
        public async Task<ActionResult<DonorFormDto>> Update(int id, [FromBody] DonorFormDto donorForm)
        {
            try
            {
                var donor = await _donorService.UpdateDonor(id, donorForm);
                if (donor == null)
                {
                    return NotFound(new { massage = $"Donor with Id{id}not dound" });

                }
                return Ok(donor);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { massage = ex.Message });
            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {


            var result = await _donorService.DeleteDonor(id);
            if (result == false)
            {
                return NotFound(new { massage = $"Donor with Id{id}not dound" });
            }
            return NoContent();
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DonorDto?>>> GetAllDonor()
        {
            return Ok(await _donorService.GetAllDonors());
        }
        [HttpGet]
        [Route("Name")]
        public async Task<ActionResult<IEnumerable<DonorDto>>>GetDonorByName(string firstName, string lastName)
        {
            var result = await _donorService.GetDonorByName(firstName, lastName);
            return Ok(result);
        }
        [HttpGet]
        [Route("Email")]

        public async Task<ActionResult<IEnumerable<DonorDto>>>GetDonorByEmail(string email)
        {
            var result = await _donorService.GetDonorByEmail(email);
            return Ok(result);
        }
        [HttpGet]
        [Route("Gift")]

        public async Task<ActionResult<IEnumerable<DonorDto>>> GetDonorByGift(string gift)
        {
            var result = await _donorService.GetDonorByGift(gift);
            return Ok(result);
        }

    }
}