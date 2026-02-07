using WebApi.Models;
using WebApi.Repository;
using static WebApi.DTOs.DonorDTO;

namespace WebApi.Interface
{
    public interface IDonorService
    {
        Task<DonorDto> CreatDonor(DonorFormDto donorForm);
        Task<bool> DeleteDonor(int Id);
        Task<DonorDto?> UpdateDonor(int Id, DonorFormDto donorForm);
        Task<IEnumerable<DonorDto?>> GetAllDonors();
        Task<IEnumerable<DonorDto?>> GetDonorByName(string firstName, string lastName);
        Task<IEnumerable<DonorDto?>> GetDonorByEmail(string email);
        Task<IEnumerable<DonorDto?>>GetDonorByGift(string gift);




    }
}
