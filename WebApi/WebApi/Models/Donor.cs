namespace WebApi.Models
{
    public class Donor
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public List<Gift> Gifts { get; set; }=new List<Gift>();
        public string Email { get; set; }=string.Empty;
        public string Phone { get; set; }= string.Empty;
    }
}
