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
        public DonorService(IDonorRepository donorRepository)
        {
            this.donorRepository = donorRepository;
        }
        public async Task<DonorDto> CreatDonor(DonorFormDto donorForm)
        {
            var donor = new Donor
            {
                FirstName = donorForm.FirstName,
                LastName = donorForm.LastName,
                Email = donorForm.Email,
                Phone = donorForm.Phone
            };
            var createdUser = await donorRepository.CreateDonor(donor);
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
            if (donorForm.FirstName != "String")
                donor.FirstName = donorForm.FirstName;
            if (donorForm.LastName != "String")
                donor.LastName = donorForm.LastName;
            if (donorForm.Email != "user@exemple.com")
                donor.Email = donorForm.Email;
            if (donorForm.Phone != "String")
                donor.Phone = donorForm.Phone;
            var updateDonor = await donorRepository.Update(donor);
            return updateDonor != null ? ResponseDto(updateDonor) : null;
        }
        public async Task<IEnumerable<DonorDto?>> GetAllDonors()
        {
            var donors = await donorRepository.GetAll();
            if (donors == null)
            {
                return null;
            }
            return donors.Where(d => d != null).Select(d => ResponseDto(d!));
        }
        public async Task<IEnumerable<DonorDto?>> GetDonorByName(string firstName, string lastName)
        {
            var donors = await donorRepository.GetByName(firstName, lastName);
            if (donors == null)
            {
                return null;
            }
            return donors.Where(d => d != null).Select(d => ResponseDto(d!));
        }
        public async Task<IEnumerable<DonorDto?>> GetDonorByEmail(string email)
        {
            var donors = await donorRepository.GetByEmail(email);
            if (donors == null)
            {
                return null;
            }
            return donors.Where(d => d != null).Select(d => ResponseDto(d!));
        }
        public async Task<IEnumerable<DonorDto?>> GetDonorByGift(string gift)
        {
            var donors = await donorRepository.GetByGift(gift);
            if (donors == null)
            {
                return null;
            }
            return donors.Where(d => d != null).Select(d => ResponseDto(d!));
        }

    }

}
