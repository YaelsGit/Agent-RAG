using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using static WebApi.DTOs.GiftDTO;
using static WebApi.DTOs.PurchaseDTO;

namespace WebApi.Interface
{
    public interface IGiftService
    {
        Task<Category> CreateCategory(CateroyFormDto CategoryForm);
        Task<GiftResponseDto> CreateGift(GiftFormDto GiftForm);
        Task<bool> DeleteGift(int Id);
        Task<GiftResponseDto?> UpdateGift(int Id, [FromBody] GiftResponseDto giftForm);
        Task<IEnumerable<GiftResponseDto?>> GetAllGift();
        Task<GiftResponseDto?> GetGiftByName(string name);
        Task<IEnumerable<GiftResponseDto?>> GetGiftByDonor(string firstName, string lastName);
        Task<IEnumerable<GiftResponseDto?>> GetGiftByNumPurchase(int num);
        Task<GiftPurchasesDto?> GetGiftPurchases(int giftId);
        Task<List<GiftPurchasesDto>> GetGiftsSortedByPrice();
        Task<List<GiftPurchasesDto>> GetGiftsSortedByMostPurchased();
        Task<GiftPurchasesWithUsersDto?> GetGiftPurchasesWithUsers(int giftId);
       Task<IEnumerable<GiftCategoryDto?>> SortedGiftByPriceOrCategory(string sorteBy);
        Task<IEnumerable<GiftWinnerDto>> GenerateWinnersReport();
        Task<decimal> GetTotalRevenue();
        Task<GiftWinnerDto?> DrawWinnerForGift(int giftId);
    }
}
