using Microsoft.EntityFrameworkCore;
using System.Globalization;
using WebApi.Data;
using WebApi.Interface;
using WebApi.Models;
using static WebApi.DTOs.GiftDTO;

namespace WebApi.Repository
{
    public class GiftRepository:IGiftRepository
    {
        private readonly WebApiContext _context;
        public GiftRepository(WebApiContext context)
        {
            _context = context;
        }
        public async Task<Category> CreateCategory(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }
        public async Task<Gift> CreateGift(Gift gift)
        {
            _context.Gifts.Add(gift);
            await _context.SaveChangesAsync();
            return gift;
        }
        public async Task<bool> Delete(int Id)
        {
            var gift = await _context.Gifts.FindAsync(Id);
            if (gift == null)
            {
                return false;
            }
            _context.Gifts.Remove(gift);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<Gift?> FindById(int Id)
        {
            return await _context.Gifts.FindAsync(Id);

        }
        public async Task<Gift?> Update(Gift gift)
        {
            var giftObj = await FindById(gift.Id);
            if (giftObj == null)
                return null;
            _context.Entry(giftObj).CurrentValues.SetValues(gift);
            await _context.SaveChangesAsync();
            return giftObj;
        }
        public async Task<ICollection<GiftCategoryDto?>> GetAll()
        {
            return await _context.Gifts.Select(g => new GiftCategoryDto
            {
                Name = g.Name,
                Description = g.Description,
                CategoryName = g.Category!.Name,
                PriceCard= g.PriceCard

            }).ToListAsync();
        }
        public async Task<Gift?> GetByName(string name)
        {
            return await _context.Gifts
                .FirstOrDefaultAsync(d => d.Name == name);
        }
        public async Task<IEnumerable<Gift?>>GetByDonor(string firstName,string lastName)
        {
            return await _context.Gifts
                .Where(d => d.Donor.FirstName == firstName && d.Donor.LastName == lastName).ToListAsync() ;
        }
        public async Task<IEnumerable<Gift?>> GetByNumPurchase(int num)
        {
            return await _context.Gifts
                .Where(g=>g.Purchases.Count()==num).ToListAsync();
        }
        public async Task<Gift?> GetGiftWithPurchases(int giftId)
        {
            return await _context.Gifts
                .Include(g => g.Purchases)
                .FirstOrDefaultAsync(g => g.Id == giftId);
        }
        public async Task<List<Gift>> GetGiftsWithPurchases(string sortBy)
        {
            var query = _context.Gifts
                .Include(g => g.Purchases)
                .AsQueryable();

            if (sortBy == "price")
            {
                query = query.OrderByDescending(g => g.PriceCard);
            }
            else if (sortBy == "mostPurchased")
            {
                query = query.OrderByDescending(g => g.Quantity);
            }

            return await query.ToListAsync();
        }
        public async Task<Gift?> GetGiftWithPurchasesIncludingUsers(int giftId)
        {
            return await _context.Gifts
                .Include(g => g.Purchases)
                    .ThenInclude(p => p.User) 
                .FirstOrDefaultAsync(g => g.Id == giftId);
        }
        public async Task<IEnumerable<GiftWinnerDto?>>GiftRandom()
        {
            var random = new Random();
            List<GiftWinnerDto>Winners=new List<GiftWinnerDto>();
            var gifts =await _context.Gifts.ToListAsync();
            for (int i = 0; i < gifts.Count(); i++)
            {
                if (gifts[i].Purchases.Count() == 0)
                {
                    var win = new GiftWinnerDto
                    {
                        GiftId = gifts[i].Id,
                        GiftName = gifts[i].Name,
                        WinnerId = 0,
                        WinnerName = "",
                    };
                    Winners.Add(win);
                }
                else {
                    int index = random.Next(gifts[i].Purchases.Count());
                    var win = new GiftWinnerDto
                    {
                        GiftId = gifts[i].Id,
                        GiftName = gifts[i].Name,
                        WinnerId = gifts[i].Purchases[index].UserId,
                        WinnerName = gifts[i].Purchases[index].User.FirstName+" "+ gifts[i].Purchases[index].User.LastName,
                    };
                    Winners.Add(win);

                }

            }
            return Winners;

        }
        public async Task<IEnumerable<Gift?>> SortedGiftByPriceOrCategory(string sorteBy)
        {
            var query = _context.Gifts
                            .AsQueryable();
            if (query == null)
            {
                return null;
            }
            if (sorteBy == "price")
                query = query.OrderByDescending(q => q.PriceCard);
            else if(sorteBy =="category")
                query = query.OrderByDescending(q => q.Category.Name);
         return query;
        }
    }
}
