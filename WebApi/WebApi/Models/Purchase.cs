namespace WebApi.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int GiftId { get; set; }
        public Gift Gift { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public static Decimal TotalSum { get; set; } = 0;
    }
}
