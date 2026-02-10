using WebApi.Models;
using static WebApi.DTOs.GiftDTO;

namespace WebApi.Interface
{
    public interface IGiftRepository
    {
        Task<Category> CreateCategory(Category category);
        Task<Gift> CreateGift(Gift gift);
        Task<bool> Delete(int Id);
        Task<Gift?> FindById(int Id);
        Task<Gift?> Update(Gift gift);
        Task<ICollection<GiftCategoryDto?>> GetAll();
        Task<Gift?> GetByName(string name);
        Task<IEnumerable<Gift?>> GetByDonor(string firstName, string lastName);
        Task<IEnumerable<Gift?>> GetByNumPurchase(int num);
        Task<Gift?> GetGiftWithPurchases(int giftId);
        Task<List<Gift>> GetGiftsWithPurchases(string sortBy);
        Task<Gift?> GetGiftWithPurchasesIncludingUsers(int giftId);
        Task<List<GiftWinnerDto?>> GiftRandom();
        Task<IEnumerable<Gift?>> SortedGiftByPriceOrCategory(string sorteBy);

    }
}
