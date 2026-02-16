using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Interface;
using WebApi.Models;
using static WebApi.DTOs.PurchaseDTO;
using static WebApi.DTOs.UserDTO;

namespace WebApi.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly WebApiContext _context;

        public UserRepository(WebApiContext context)
        {
            _context = context;
        }

        public async Task<User> CreateUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

      
        public async Task<User?>GetByUserNameAsync(string userName)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userName);
        }
        public async Task<User?> GetByUserId(int userId)
        {
            return await _context.Users
                .Include(u => u.Purchases)
                .Include(u => u.PurchaseDto)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task AddPurchase(int userId, Purchase purchase)
        {
            var user = await GetByUserId(userId);
            if (user != null)
            {
                user.Purchases.Add(purchase);
                await _context.SaveChangesAsync();
            }
 
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}