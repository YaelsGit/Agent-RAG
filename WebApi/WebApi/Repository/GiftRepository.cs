using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json;
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
            Console.WriteLine($"Trying to delete Gift Id: {Id}");

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
            Console.WriteLine(">>> GetAll() Repository CALLED");
            var count = await _context.Gifts.CountAsync();
            Console.WriteLine($"GIFTS COUNT = {count}");
            var gifts = await _context.Gifts.Include(g => g.Category).Include(g => g.Donor).ToListAsync();
            foreach (var gift in gifts)
            {
                Console.WriteLine($"Gift: Id={gift.Id}, Name={gift.Name}, CategoryId={gift.CategoryId}, DonorId={gift.DonorId}, Category={(gift.Category != null ? gift.Category.Name : "null")}, Donor={(gift.Donor != null ? gift.Donor.FirstName + " " + gift.Donor.LastName : "null")}");
            }
            return gifts.Select(g => new GiftCategoryDto
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                CategoryName = g.Category != null ? g.Category.Name : string.Empty,
                PriceCard = g.PriceCard,
                PictureId = g.Id // Use correct property if available
            }).ToList();
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
        public async Task<List<GiftWinnerDto>> GiftRandom()
        {
            var random = new Random();
            List<GiftWinnerDto> winners = new();

            var gifts = await _context.Gifts
                .Include(g => g.Purchases)
                .ThenInclude(p => p.User)
                .ToListAsync();

            foreach (var gift in gifts)
            {
                if (!gift.Purchases.Any())
                {
                    // אם אין רכישות, מחזיר זוכה "דיפולטי"
                    winners.Add(new GiftWinnerDto
                    {
                        GiftId = gift.Id,
                        GiftName = gift.Name,
                        WinnerId = -1, // מזהה דיפולטי
                        WinnerName = "אין זוכה"
                    });
                }
                else
                {
                    int index = random.Next(gift.Purchases.Count);
                    var purchase = gift.Purchases[index];

                    winners.Add(new GiftWinnerDto
                    {
                        GiftId = gift.Id,
                        GiftName = gift.Name,
                        WinnerId = purchase.UserId,
                        WinnerName = $"{purchase.User.FirstName} {purchase.User.LastName}"
                    });
                }
            }

            // שמירה לקובץ JSON
            var json = JsonSerializer.Serialize(
                winners,
                new JsonSerializerOptions { WriteIndented = true }
            );

            var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "giftWinners.json"
            );

            await File.WriteAllTextAsync(path, json);

            return winners;
        }

        public async Task<IEnumerable<Gift?>> SortedGiftByPriceOrCategory(string sorteBy)
        {
            var query = _context.Gifts.Include(g => g.Category).AsQueryable();

            if (sorteBy == "price")
                query = query.OrderByDescending(g => g.PriceCard);
            else if (sorteBy == "category")
                query = query.OrderByDescending(g => g.Category != null ? g.Category.Name : "");

            return await query.ToListAsync();
        }

    }
}
