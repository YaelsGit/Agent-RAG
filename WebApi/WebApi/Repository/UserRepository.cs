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
        public async Task<User?> GetByUserId(int Id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == Id);
        }
        public async Task<Purchase?> TicketPurchase(Purchase purchase)
        {
            if(purchase == null) return null;
            var gift= await _context.Gifts.FirstOrDefaultAsync(u => u.Id==purchase.GiftId);
            if(gift == null) return null;
            gift.Quantity += 1;
            await _context.SaveChangesAsync();
            purchase.User.Purchases.Add(purchase);
            await _context.SaveChangesAsync();
            return purchase; 
        }

    }
}