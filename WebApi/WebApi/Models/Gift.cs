namespace WebApi.Models
{
    public class Gift
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; }=string.Empty;
        public List<Purchase> Purchases { get; set; } = new List<Purchase>();
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int DonorId { get; set; }
        public Donor Donor { get; set; }
        public Decimal PriceCard { get; set; }
        public int Quantity { get; set; }
        public static bool IsRandom { get; set; } = false;


    }
}
