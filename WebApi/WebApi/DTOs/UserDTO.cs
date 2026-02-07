using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs
{
    public class UserDTO
    {
        public class UserRegisterDto
        {
            [Required,MaxLength(30)] 
            public string FirstName { get; set; }
            [Required, MaxLength(30)]
            public string LastName { get; set; }
            [Required, MaxLength(20)]
            public string UserName { get; set; }
            [Required,EmailAddress]
            public string Email { get; set; }
            [Required, MaxLength(10)]
            public string Password { get; set; }
            [Required, MaxLength(10),MinLength(9)]
            public string Phone { get; set; }
            public string City { get; set; } = string.Empty;
            public string Street { get; set; } = string.Empty;
            [Required]
            public int BuildingNumber { get; set; }

        }
        public class UserLoginedDto {

            [Required, MaxLength(20)]
            public string UserName { get; set; }
            [Required, MaxLength(10)]
            public string Password { get; set; }
        }
        public class UserDto {
         
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }
        public class UserResponseDto
        {
            public int Id { get; set; }
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
            public string City { get; set; }=string.Empty;
            public string Street { get; set; } = string.Empty;
            public int BuildingNumber { get; set; } 
        }
        public class LoginResponseDto
        {
            public string Token { get; set; } = string.Empty;
            public string TokenType { get; set; } = "Bearer";
            public int ExpiresIn { get; set; }
            public UserResponseDto User { get; set; } = null;
        }
        public class LoginRequestDto
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}
