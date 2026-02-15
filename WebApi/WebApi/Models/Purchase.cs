namespace WebApi.Models
{
    public enum BasketStatus
    {
        Draft = 0,
        Confirmed = 1
    }

    public class Purchase
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;

        public int GiftId { get; set; }
        public Gift? Gift { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public static decimal TotalSum { get; set; } = 0;

        public BasketStatus basketStatus { get; set; } = BasketStatus.Draft;
    }
}
