using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApi.Models;

namespace WebApi.Models
{
    public class Basket
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public BasketStatus Status { get; set; } = BasketStatus.Draft;
        public ICollection<Purchase>? Purchases { get; set; }
    }
}
