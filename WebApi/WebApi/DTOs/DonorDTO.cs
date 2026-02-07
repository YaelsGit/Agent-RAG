using System.ComponentModel.DataAnnotations;
using WebApi.Models;

namespace WebApi.DTOs
{
    public class DonorDTO
    {
        public class DonorFormDto
        {
            [Required]
            public int Id { get; set; }
            [Required, MaxLength(30)]
            public string FirstName { get; set; } = string.Empty;
            [Required, MaxLength(30)]
            public string LastName { get; set; } = string.Empty;
            [Required, EmailAddress]
            public string Email { get; set; } = string.Empty;
            [Required, MaxLength(10), MinLength(9)]
            public string Phone { get; set; } = string.Empty;
        }
        public class DonorDto
        {
            public int Id { get; set; }
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;

        }
        
    }
}