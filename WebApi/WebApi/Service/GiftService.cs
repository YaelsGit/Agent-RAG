using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public GiftService(IGiftRepository giftRepository)
        {
            this._GiftRepository = giftRepository;
        }
        public async Task<Category> CreateCategory(CateroyFormDto CategoryForm)
        {
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
                PriceCard = GiftForm.PriceCard

            };
            var createdGift = await _GiftRepository.CreateGift(gift);
            return ResponseDto(createdGift);
        }
        public async Task<bool> DeleteGift(int Id)
        {
            var gift = await _GiftRepository.FindById(Id);
            if (gift == null)
            {
                Console.WriteLine("Gift not found in repository");
                return false;
            }

            Console.WriteLine($"Gift Purchases Count: {gift.Purchases.Count()}");

            if (gift.Purchases.Count() > 0)
            {
                Console.WriteLine("Cannot delete gift with purchases");
                return false;
            }

            return await _GiftRepository.Delete(Id);
        }

        public async Task<GiftResponseDto?> UpdateGift(int Id, [FromBody] GiftResponseDto giftForm)
        {
            var gift = await _GiftRepository.FindById(Id);
            if (gift == null)
                return null;
            if (giftForm.Name != "String")
                gift.Name = giftForm.Name;
            if (giftForm.Description != "String")
                gift.Description = giftForm.Description;
            if (giftForm.CategoryId != 0)
                gift.CategoryId = giftForm.CategoryId;
            if (giftForm.DonorId != 0)
                gift.DonorId = giftForm.DonorId;
            if (giftForm.PriceCard != 0)
                gift.PriceCard = giftForm.PriceCard;
            var updateGift = await _GiftRepository.Update(gift);
            return updateGift != null ? ResponseDto(updateGift) : null;
        }
        private static GiftResponseDto ResponseDto(Gift gift)
        {
            return new GiftResponseDto
            {
                Name = gift.Name,
                Description = gift.Description,
                CategoryId = gift.CategoryId,
                DonorId = gift.DonorId,
                PriceCard = gift.PriceCard
            };
        }
        public async Task<IEnumerable<GiftCategoryDto?>> GetAllGift()
        {
            var gifts = await _GiftRepository.GetAll();
            if (gifts == null)
            {
                return null;
            }
            return gifts;
        }
        public async Task<GiftResponseDto?> GetGiftByName(string name)
        {
            var gift = await _GiftRepository.GetByName(name);
            if (gift == null)
            {
                return null;
            }
            return ResponseDto(gift);
        }
        public async Task<IEnumerable<GiftResponseDto?>> GetGiftByDonor(string firstName, string lastName)
        {
            var gifts = await _GiftRepository.GetByDonor(firstName, lastName);
            if (gifts == null)
            {
                return null;
            }
            return gifts.Select(ResponseDto);
        }
        public async Task<IEnumerable<GiftResponseDto?>> GetGiftByNumPurchase(int num)
        {
            var gifts = await _GiftRepository.GetByNumPurchase(num);
            if (gifts == null)
            {
                return null;
            }
            return gifts.Select(ResponseDto);
        }
        public async Task<GiftPurchasesDto?> GetGiftPurchases(int giftId)
        {
            var gift = await _GiftRepository.GetGiftWithPurchases(giftId);

            if (gift == null)
                return null;

            return new GiftPurchasesDto
            {
                GiftId = gift.Id,
                GiftName = gift.Name,
                PriceCard = gift.PriceCard,
                TotalPurchasedTickets = gift.Quantity,

                Purchases = gift.Purchases.Select(p => new PurchaseDto
                {
                    Id = p.Id,
                    Date = p.Date,
                }

                ).ToList()
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
                TotalPurchasedTickets = g.Quantity,

                Purchases = g.Purchases.Select(p => new PurchaseDto
                {
                    Id = p.Id,
                    Date = p.Date,
                    GiftId = p.GiftId
                }).ToList()
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
                TotalPurchasedTickets = g.Quantity,

                Purchases = g.Purchases.Select(p => new PurchaseDto
                {
                    Id = p.Id,
                    Date = p.Date,
                    GiftId = p.GiftId
                }).ToList()
            }).ToList();
        }
        public async Task<GiftPurchasesWithUsersDto?> GetGiftPurchasesWithUsers(int giftId)
        {
            var gift = await _GiftRepository.GetGiftWithPurchasesIncludingUsers(giftId);

            if (gift == null)
                return null;

            return new GiftPurchasesWithUsersDto
            {
                GiftId = gift.Id,
                GiftName = gift.Name,
                PriceCard = gift.PriceCard,
                Quantity = gift.Quantity,

                Purchases = gift.Purchases.Select(p => new PurchaseWithUserDto
                {
                    Id = p.Id,
                    Date = p.Date,
                    GiftId = p.GiftId,
                    UserId = p.UserId,
                    FirstName = p.User.FirstName,
                    LastName = p.User.LastName,
                }).ToList()
            };
        }
        public async Task<GiftWinnerDto?> DrawWinnerForGift(int giftId)
        {
            var gift = await _GiftRepository.GetGiftWithPurchasesIncludingUsers(giftId);

            if (gift == null || !gift.Purchases.Any()) return null;

            var ticketPool = new List<User>();
            foreach (var purchase in gift.Purchases.Where(p => p.basketStatus == BasketStatus.Confirmed))
            {
                for (int i = 0; i < purchase.Quantity; i++)
                {
                    ticketPool.Add(purchase.User);
                }
            }

            if (!ticketPool.Any()) return null;

            var random = new Random();
            var winner = ticketPool[random.Next(ticketPool.Count)];

            return new GiftWinnerDto
            {
                GiftName = gift.Name,
                WinnerName = $"{winner.FirstName} {winner.LastName}",
            };
        }


        public async Task<IEnumerable<GiftCategoryDto?>> SortedGiftByPriceOrCategory(string sorteBy)
        {
            var result = await _GiftRepository.SortedGiftByPriceOrCategory(sorteBy);
            if (result == null)
                return null;
            return result.Select(arr => new GiftCategoryDto()
            {
                Name = arr.Name,
                Description = arr.Description,
                CategoryName = arr.Category != null ? arr.Category.Name : "",
                PriceCard = arr.PriceCard,
            });


        }
        public async Task<IEnumerable<GiftWinnerDto>> GenerateWinnersReport()
        {
            var allGifts = await _GiftRepository.GetAll();
            var winnersReport = new List<GiftWinnerDto>();

            foreach (var gift in allGifts)
            {
                var winner = await DrawWinnerForGift(gift.Id);

                if (winner != null)
                {
                    winnersReport.Add(winner);
                }
                else
                {
                    winnersReport.Add(new GiftWinnerDto
                    {
                        GiftName = gift.Name,
                        WinnerName = "אין רוכשים למתנה זו",
                    });
                }
            }

            return winnersReport;
        }
        public async Task<decimal> GetTotalRevenue()
        {
            var gifts = await _GiftRepository.GetGiftsWithPurchases("all");

            decimal totalRevenue = 0;

            foreach (var gift in gifts)
            {
                var confirmedTickets = gift.Purchases
                    .Where(p => p.basketStatus == BasketStatus.Confirmed)
                    .Sum(p => p.Quantity);

                totalRevenue += (confirmedTickets * gift.PriceCard);
            }

            return totalRevenue;
        }
    }
}