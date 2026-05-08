using System.ComponentModel.DataAnnotations;
using WebApi.DTOs;
using WebApi.Interface;
using WebApi.Models;
using WebApi.Repository;
using static WebApi.DTOs.DonorDTO;
using static WebApi.DTOs.UserDTO;

namespace WebApi.Service
{
    public class DonorService : IDonorService
    {
        private readonly IDonorRepository donorRepository;
        private readonly ILogger<DonorService> _logger;
        public DonorService(IDonorRepository donorRepository, ILogger<DonorService> logger)
        {
            this.donorRepository = donorRepository;
            _logger = logger;
        }
        public async Task<DonorDto> CreatDonor(DonorFormDto donorForm)
        {
            _logger.LogInformation("Creating donor: {@DonorForm}", donorForm);
            var donor = new Donor
            {
                FirstName = donorForm.FirstName,
                LastName = donorForm.LastName,
                Email = donorForm.Email,
                Phone = donorForm.Phone
            };
            var createdUser = await donorRepository.CreateDonor(donor);
            _logger.LogInformation("Donor created with ID: {DonorId}", createdUser?.Id);
            return ResponseDto(createdUser);
        }
        private static DonorDto ResponseDto(Donor donor)
        {
            if (donor == null) throw new ArgumentNullException(nameof(donor));
            return new DonorDto
            {
                Id = donor.Id, 
                FirstName = donor.FirstName,
                LastName = donor.LastName
            };
        }
        public async Task<bool> DeleteDonor(int Id)
        {
            return await donorRepository.Delete(Id);
        }
        public async Task<DonorDto?> UpdateDonor(int Id, DonorFormDto donorForm)
        {
            var donor = await donorRepository.FindById(Id);
            if (donor == null)
                return null;
            if (donorForm.FirstName != "string")
                donor.FirstName = donorForm.FirstName;
            if (donorForm.LastName != "string")
                donor.LastName = donorForm.LastName;
            if (donorForm.Email != "user@exemple.com")
                donor.Email = donorForm.Email;
            if (donorForm.Phone != "string")
                donor.Phone = donorForm.Phone;
            var updateDonor = await donorRepository.Update(donor);
            return updateDonor != null ? ResponseDto(updateDonor) : null;
        }
        public async Task<IEnumerable<DonorDto?>> GetAllDonors()
        {
            _logger.LogInformation("Getting all donors");
            var donors = await donorRepository.GetAll();
            if (donors == null)
            {
                _logger.LogWarning("No donors found");
                return null;
            }
            return donors.Where(d => d != null).Select(d => ResponseDto(d!));
        }
        public async Task<IEnumerable<DonorDto?>> GetDonorByName(string firstName, string lastName)
        {
            _logger.LogInformation("Getting donor by name: {FirstName} {LastName}", firstName, lastName);
            var donors = await donorRepository.GetByName(firstName, lastName);
            if (donors == null)
            {
                _logger.LogWarning("No donors found for name: {FirstName} {LastName}", firstName, lastName);
                return null;
            }
            return donors.Where(d => d != null).Select(d => ResponseDto(d!));
        }
        public async Task<IEnumerable<DonorDto?>> GetDonorByEmail(string email)
        {
            _logger.LogInformation("Getting donor by email: {Email}", email);
            var donors = await donorRepository.GetByEmail(email);
            if (donors == null)
            {
                _logger.LogWarning("No donors found for email: {Email}", email);
                return null;
            }
            return donors.Where(d => d != null).Select(d => ResponseDto(d!));
        }
        public async Task<IEnumerable<DonorDto?>> GetDonorByGift(string gift)
        {
            _logger.LogInformation("Getting donor by gift: {Gift}", gift);
            var donors = await donorRepository.GetByGift(gift);
            if (donors == null)
            {
                _logger.LogWarning("No donors found for gift: {Gift}", gift);
                return null;
            }
            return donors.Where(d => d != null).Select(d => ResponseDto(d!));
        }

    }

}
