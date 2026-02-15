using WebApi.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using WebApi.Data;
using WebApi.DTOs;
using WebApi.Interface;
using WebApi.Models;
using WebApi.Repository;
using static WebApi.DTOs.PurchaseDTO;
using static WebApi.DTOs.UserDTO;

namespace WebApi.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private readonly WebApiContext _db;

        public UserService(IUserRepository userRepository, IConfiguration configuration, ITokenService tokenService, WebApiContext db )
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _tokenService = tokenService;
            _db = db;

        }
        public async Task<UserResponseDto> UserRegister(UserRegisterDto userRegister)
        {
            var findUser = await _userRepository.GetByUserNameAsync(userRegister.UserName);
            if (findUser != null)
            {
                throw new ArgumentException($"UserName {userRegister.UserName} is already registered.");
            }
            var user = new User
            {
                FirstName = userRegister.FirstName,
                LastName = userRegister.LastName,
                UserName = userRegister.UserName,
                Email = userRegister.Email,
                Password = HashPassword(userRegister.Password),
                Phone = userRegister.Phone,
                City = userRegister.City,
                Street = userRegister.Street,
                BuildingNumber = userRegister.BuildingNumber,
            };


            var createdUser = await _userRepository.CreateUser(user);
            return ResponseDto(createdUser);

        }
        public async Task<LoginResponseDto?> UserLogin(UserLoginedDto userLogin)
        {
            var user = await _userRepository.GetByUserNameAsync(userLogin.UserName);

            if (user == null)
            {
                return null;
            }

            var hashedPassword = HashPassword(userLogin.Password);
            if (user.Password != hashedPassword)
            {
                return null;
            }
            var token = _tokenService.GenerateToken(user.Id, user.UserName, user.FirstName, user.LastName, user.Email, user.Role);
            var expiryMinutes = _configuration.GetValue<int>("JwtSettings:ExpiryMinutes", 60);


            return new LoginResponseDto
            {
                Token = token,
                TokenType = "Bearer",
                ExpiresIn = expiryMinutes * 60,
                User = ResponseDto(user)
            };
        }
        private static UserResponseDto ResponseDto(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                City = user.City,
                Street = user.Street,
                BuildingNumber = user.BuildingNumber,

            };
        }
        private static string HashPassword(string password)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }
        public async Task<PurchaseBasketUserDto?> AddToBasket(PurchaseBasketDto purchase, int userId)
        {

            var purchases = new PurchaseBasketUserDto
            {
                UserId = userId,
                GiftId = purchase.GiftId,
                Date = purchase.Date,
                Quentity = purchase.Quentity,
            };
            var user = await _userRepository.GetByUserId(purchases.UserId);

            if (user == null)
            {
                return null;
            }
            for (var i = 0; user.PurchaseDto.Count() > 0; i++)
            {
                if (user.PurchaseDto[i].GiftId == purchases.GiftId)
                {
                    user.PurchaseDto[i].Quentity++;
                    return purchases;
                }
            }
            user.PurchaseDto.Add(purchases);
            return purchases;
        }
        public async Task<List<PurchaseWithUserDto?>> TicketPurchase(PurchaseBasketDto purchase, int userId)

        {
            List<PurchaseWithUserDto> lp = new List<PurchaseWithUserDto>();
            if (purchase == null)
                return null;
            for (int i = 0; i < purchase.Quentity; i++)
            {
                var purch = new Purchase()
                {
                    Date = DateTime.Now,
                    GiftId = purchase.GiftId,
                    UserId = userId,
                };
                Purchase.TotalSum += purch.Gift.PriceCard;
                var newPurchase = await _userRepository.TicketPurchase(purch);
                if (newPurchase != null)
                    lp.Add(new PurchaseWithUserDto()
                    {
                        Date = newPurchase.Date,
                        UserId = newPurchase.UserId,
                        GiftId = newPurchase.GiftId,
                        FirstName = newPurchase.User.FirstName,
                        LastName = newPurchase.User.LastName,
                    });
            }
            return lp;


        }


        public async Task<bool> ConfirmBasket(int userId)
        {
            // בדיקה ישירה מה־DB
            var raffleDone = await _db.Baskets.AnyAsync(b => b.Status == BasketStatus.Confirmed);
            if (raffleDone)
                return false;

            var basket = await _db.Baskets
                .FirstOrDefaultAsync(b => b.UserId == userId && b.Status == BasketStatus.Draft);

            if (basket == null)
                return false;

            basket.Status = BasketStatus.Confirmed;
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<List<UserResponseDto>> GetWinners()
        {
            var users = await _db.Purchases
                .Include(p => p.User)
                .GroupBy(p => p.UserId)
                .Select(g => g.First().User)
                .ToListAsync();

            return users.Select(u => new UserResponseDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Phone = u.Phone,
                City = u.City,
                Street = u.Street,
                BuildingNumber = u.BuildingNumber
            }).ToList();
        }

    }
}

