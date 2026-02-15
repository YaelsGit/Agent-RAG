using System.ComponentModel.DataAnnotations;
using WebApi.Models;
using static WebApi.DTOs.PurchaseDTO;

namespace WebApi.DTOs
{
    public class GiftDTO
    {
        public class GiftFormDto
        {
  
            [Required, MaxLength(30)]
            public string Name { get; set; } = string.Empty;
            [Required]
            public string Description { get; set; } = string.Empty;
            public int CategoryId { get; set; }
            public int DonorId { get; set; }
            [Required]
            [Range(0, 99.99)]
            public Decimal PriceCard { get; set; }
        }
        public class CateroyFormDto
        {
            [Required, MaxLength(30)]
            public string Name { get; set; } = string.Empty;
        }
        public class GiftResponseDto
        {
            [Required]
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public int CategoryId { get; set; }
            public int DonorId { get; set; }
            [Range(0, 99.99)]

            public Decimal PriceCard { get; set; }
        }
        public class GiftCategoryDto
        {
            [Required]
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string CategoryName { get; set; }=string.Empty;
            public Decimal PriceCard { get; set; }
            public int PictureId { get; set; }
        }
        public class GiftDonorDto
        {
            [Required]
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string DonorName { get; set; }= string.Empty;
             public Decimal PriceCard { get; set; }

        }
        public class GiftPurchasesDto
        {
            public int GiftId { get; set; }
            public string GiftName { get; set; } = string.Empty;
            public Decimal PriceCard { get; set; }
            public int TotalPurchasedTickets { get; set; }

            public List<PurchaseDto> Purchases { get; set; } = new();

        }
        public class GiftPurchasesWithUsersDto
        {
            public int GiftId { get; set; }
            public string GiftName { get; set; } = string.Empty;
            [Range(0, 99.99)]
            public Decimal PriceCard { get; set; }
            public int Quantity { get; set; }

            public List<PurchaseWithUserDto> Purchases { get; set; } = new();
        }
        public class GiftWinnerDto
        {
            public int GiftId { get; set; }
            public string GiftName { get; set; } = string.Empty;
            public int WinnerId { get; set; }
            public String WinnerName { get; set; } = string.Empty;
        }
        public class TotalSumDto
        {
            public Decimal TotalSum { get; set; }
        }
    }
}
