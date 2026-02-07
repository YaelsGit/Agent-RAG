namespace WebApi.DTOs
{
    public class PurchaseDTO
    {
        public class PurchaseDto
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public int GiftId { get; set; }

        }
        public class PurchaseWithUserDto
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public int GiftId { get; set; }
            public int UserId { get; set; }
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
        }
        public class PurchaseBasketDto
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public int GiftId { get; set; }
            public int UserId { get; set; }
            public int Quentity { get; set; } = 1;

        }
    }
}
