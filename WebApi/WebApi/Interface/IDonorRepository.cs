using WebApi.Models;

namespace WebApi.Interface
{
    public interface IDonorRepository
    {
        Task<Donor> CreateDonor(Donor donor);
        Task<bool> Delete(int Id);
        Task<Donor?> FindById(int Id);
        Task<Donor?> Update(Donor donor);
        Task<ICollection<Donor?>> GetAll();
        Task<IEnumerable<Donor?>> GetByName(string firstName, string lastName);
        Task<IEnumerable<Donor?>> GetByEmail(string email);
        Task<IEnumerable<Donor?>> GetByGift(string gift);








    }
}
