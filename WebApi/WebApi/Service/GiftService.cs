using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WebApi.DTOs;
using WebApi.Interface;
using WebApi.Models;
using WebApi.Repository;
using static WebApi.DTOs.DonorDTO;
using static WebApi.DTOs.GiftDTO;
using static WebApi.DTOs.PurchaseDTO;

namespace WebApi.Service
{
    public class GiftService : IGiftService
    {
        private readonly IGiftRepository _GiftRepository;
        private readonly ILogger<GiftService> _logger;

        public GiftService(IGiftRepository giftRepository, ILogger<GiftService> logger)
        {
            this._GiftRepository = giftRepository;
            _logger = logger;
        }

        public async Task<Category> CreateCategory(CateroyFormDto CategoryForm)
        {
            _logger.LogInformation("Creating category: {@CategoryForm}", CategoryForm);
            var Category = new Category
            {
                Name = CategoryForm.Name
            };
            var createdCategory = await _GiftRepository.CreateCategory(Category);
            return createdCategory;
        }

        public async Task<GiftResponseDto> CreateGift(GiftFormDto GiftForm)
        {
            var gift = new Gift
            {
                Name = GiftForm.Name,
                Description = GiftForm.Description,
                CategoryId = GiftForm.CategoryId,
                DonorId = GiftForm.DonorId,
                PriceCard = GiftForm.PriceCard,
                PictureId = GiftForm.PictureId
            };
            var createdGift = await _GiftRepository.CreateGift(gift);
            return ResponseDto(createdGift);
        }

        public async Task<bool> DeleteGift(int Id)
        {
            var gift = await _GiftRepository.FindById(Id);
            if (gift == null || (gift.Purchases != null && gift.Purchases.Any()))
            {
                return false;
            }
            return await _GiftRepository.Delete(Id);
        }

        public async Task<GiftResponseDto?> UpdateGift(int Id, GiftResponseDto giftForm)
        {
            var gift = await _GiftRepository.FindById(Id);
            if (gift == null) return null;

            if (giftForm.Name != "string") gift.Name = giftForm.Name;
            if (giftForm.Description != "string") gift.Description = giftForm.Description;
            if (giftForm.CategoryId != 0) gift.CategoryId = giftForm.CategoryId;
            if (giftForm.DonorId != 0) gift.DonorId = giftForm.DonorId;
            if (giftForm.PriceCard != 0) gift.PriceCard = giftForm.PriceCard;
            if (giftForm.PictureId != 0) gift.PictureId = giftForm.PictureId;

            var updateGift = await _GiftRepository.Update(gift);
            return updateGift != null ? ResponseDto(updateGift) : null;
        }

        private static GiftResponseDto ResponseDto(Gift gift)
        {
            return new GiftResponseDto
            {
                Id = gift.Id,
                Name = gift.Name,
                Description = gift.Description,
                CategoryId = gift.CategoryId,
                DonorId = gift.DonorId,
                PriceCard = gift.PriceCard,
                PictureId = gift.PictureId,
                WinnerName = gift.WinnerName // חשוב מאוד להעברה לאנגולר
            };
        }

        public async Task<IEnumerable<GiftResponseDto?>> GetAllGift()
        {
            var gifts = await _GiftRepository.GetAll();
            return gifts ?? Enumerable.Empty<GiftResponseDto?>();
        }

        public async Task<GiftResponseDto?> GetGiftByName(string name)
        {
            var gift = await _GiftRepository.GetByName(name);
            return gift != null ? ResponseDto(gift) : null;
        }

        public async Task<IEnumerable<GiftResponseDto?>> GetGiftByDonor(string firstName, string lastName)
        {
            var gifts = await _GiftRepository.GetByDonor(firstName, lastName);
            return gifts?.Select(g => g != null ? ResponseDto(g) : null) ?? Enumerable.Empty<GiftResponseDto?>();
        }

        public async Task<IEnumerable<GiftResponseDto?>> GetGiftByNumPurchase(int num)
        {
            var gifts = await _GiftRepository.GetByNumPurchase(num);
            return gifts?.Select(g => g != null ? ResponseDto(g) : null) ?? Enumerable.Empty<GiftResponseDto?>();
        }

        public async Task<GiftPurchasesDto?> GetGiftPurchases(int giftId)
        {
            var gift = await _GiftRepository.GetGiftWithPurchases(giftId);
            if (gift == null) return null;

            return new GiftPurchasesDto
            {
                GiftId = gift.Id,
                GiftName = gift.Name,
                PriceCard = gift.PriceCard,
                TotalPurchasedTickets = gift.Quantity,
                PictureId = gift.PictureId,
                Purchases = gift.Purchases.Select(p => new PurchaseDto
                {
                    Id = p.Id,
                    Date = p.Date,
                }).ToList()
            };
        }

        public async Task<List<GiftPurchasesDto>> GetGiftsSortedByPrice()
        {
            var gifts = await _GiftRepository.GetGiftsWithPurchases("price");
            return gifts.Select(g => new GiftPurchasesDto
            {
                GiftId = g.Id,
                GiftName = g.Name,
                PriceCard = g.PriceCard,
                Purchases = g.Purchases.Select(p => new PurchaseDto { Id = p.Id, Date = p.Date, GiftId = p.GiftId }).ToList()
            }).ToList();
        }

        public async Task<List<GiftPurchasesDto>> GetGiftsSortedByMostPurchased()
        {
            var gifts = await _GiftRepository.GetGiftsWithPurchases("mostPurchased");
            return gifts.Select(g => new GiftPurchasesDto
            {
                GiftId = g.Id,
                GiftName = g.Name,
                PriceCard = g.PriceCard,
                Purchases = g.Purchases.Select(p => new PurchaseDto { Id = p.Id, Date = p.Date, GiftId = p.GiftId }).ToList()
            }).ToList();
        }

        public async Task<GiftPurchasesWithUsersDto?> GetGiftPurchasesWithUsers(int giftId)
        {
            var gift = await _GiftRepository.GetGiftWithPurchasesIncludingUsers(giftId);
            if (gift == null) return null;

            return new GiftPurchasesWithUsersDto
            {
                GiftId = gift.Id,
                GiftName = gift.Name,
                PriceCard = gift.PriceCard,
                Purchases = gift.Purchases.Select(p => new PurchaseWithUserDto
                {
                    Id = p.Id,
                    FirstName = p.User?.FirstName ?? "Unknown",
                    LastName = p.User?.LastName ?? "User",
                }).ToList()
            };
        }

        public async Task<GiftWinnerDto?> DrawWinnerForGift(int giftId)
        {
            var gift = await _GiftRepository.GetGiftWithPurchasesIncludingUsers(giftId);
            if (gift == null) return null;

            var confirmedPurchases = gift.Purchases?
                .Where(p => p.basketStatus == BasketStatus.Confirmed)
                .ToList();

            if (confirmedPurchases == null || !confirmedPurchases.Any()) return null;

            var ticketPool = new List<User>();
            foreach (var purchase in confirmedPurchases)
            {
                for (int i = 0; i < purchase.Quantity; i++)
                {
                    if (purchase.User != null) ticketPool.Add(purchase.User);
                }
            }

            if (!ticketPool.Any()) return null;

            var random = new Random();
            var winner = ticketPool[random.Next(ticketPool.Count)];
            string fullName = $"{winner.FirstName} {winner.LastName}".Trim();

            // עדכון השם בתוך ה-Entity ושמירה ל-DB
            gift.WinnerName = fullName;
            await _GiftRepository.Update(gift);

            return new GiftWinnerDto
            {
                GiftId = gift.Id,
                GiftName = gift.Name,
                WinnerName = fullName,
            };
        }

        public async Task<IEnumerable<GiftWinnerDto>> GenerateWinnersReport()
        {
            var allGifts = await _GiftRepository.GetAll();
            var winnersReport = new List<GiftWinnerDto>();

            if (allGifts == null) return winnersReport;

            foreach (var giftDto in allGifts)
            {
                if (giftDto == null) continue;

                var winnerDto = await DrawWinnerForGift(giftDto.Id);
                winnersReport.Add(winnerDto ?? new GiftWinnerDto
                {
                    GiftId = giftDto.Id,
                    GiftName = giftDto.Name,
                    WinnerName = "אין רוכשים למתנה זו"
                });
            }
            return winnersReport;
        }

        public async Task<IEnumerable<GiftCategoryDto?>> SortedGiftByPriceOrCategory(string sorteBy)
        {
            var result = await _GiftRepository.SortedGiftByPriceOrCategory(sorteBy);
            return result?.Select(arr => new GiftCategoryDto()
            {
                Name = arr.Name,
                Description = arr.Description,
                CategoryName = arr.Category?.Name ?? "ללא קטגוריה",
                PriceCard = arr.PriceCard,
            }) ?? Enumerable.Empty<GiftCategoryDto?>();
        }

        public async Task<decimal> GetTotalRevenue()
        {
            var gifts = await _GiftRepository.GetGiftsWithPurchases("all");
            decimal totalRevenue = 0;

            foreach (var gift in gifts)
            {
                var confirmedTickets = gift.Purchases?
                    .Where(p => p.basketStatus == BasketStatus.Confirmed)
                    .Sum(p => p.Quantity) ?? 0;

                totalRevenue += (confirmedTickets * gift.PriceCard);
            }

            return totalRevenue;
        }
    }
}