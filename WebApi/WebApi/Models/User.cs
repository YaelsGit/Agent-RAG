using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WebApi.DTOs;
using static WebApi.DTOs.PurchaseDTO;


namespace WebApi.Models
{
   public enum Status
    {
        User=0,
        Admin=1
    }
    [Index(nameof(UserName),IsUnique = true)]

    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }= string.Empty;
        public string LastName { get; set; } = string.Empty;
       
        public string UserName { get; set; }=string.Empty;
       
        public string Password { get; set; }= string.Empty;
        public Status Role { get; set; }= Status.User;
        [EmailAddress]
        public string Email { get; set; }
     
        public string Phone { get; set; }
        public string City { get; set; }=String.Empty;
        public string Street { get; set; } = String.Empty;
        public int BuildingNumber { get; set; }
        public List<Purchase> Purchases { get; set; }= new List<Purchase>();
        public List<PurchaseBasketUserDto> PurchaseDto { get; set; } = new List<PurchaseBasketUserDto>();


    }
}
