using WebApi.Models;
using static WebApi.DTOs.UserDTO;

namespace WebApi.Interface
{
    public interface IUserRepository
    {
        Task<User>CreateUser(User user);
        Task<User?>GetByUserNameAsync(string userName);
        Task<User?> GetByUserId(int userId);
        Task AddPurchase(int userId, Purchase purchase);
        Task SaveChangesAsync();

    }
}
