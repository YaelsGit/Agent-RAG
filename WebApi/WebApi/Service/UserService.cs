using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.DTOs;
using WebApi.Interface;
using WebApi.Models;
using static WebApi.DTOs.PurchaseDTO;
using static WebApi.DTOs.UserDTO;
using static WebApi.DTOs.WinnerDTO;

namespace WebApi.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IGiftRepository _giftRepository;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private readonly WebApiContext _db;

        public UserService(
            IUserRepository userRepository,
            IGiftRepository giftRepository,
            IConfiguration configuration,
            ITokenService tokenService,
            WebApiContext db)
        {
            _userRepository = userRepository;
            _giftRepository = giftRepository;
            _configuration = configuration;
            _tokenService = tokenService;
            _db = db;
        }

//Register
        public async Task<UserResponseDto> UserRegister(UserRegisterDto userRegister)
        {
            var findUser = await _userRepository.GetByUserNameAsync(userRegister.UserName);
            if (findUser != null)
                throw new ArgumentException($"UserName {userRegister.UserName} is already registered.");

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

            await _userRepository.CreateUser(user);
            await _userRepository.SaveChangesAsync();

            return ResponseDto(user);
        }

//Login
        public async Task<LoginResponseDto?> UserLogin(UserLoginedDto userLogin)
        {
            var user = await _userRepository.GetByUserNameAsync(userLogin.UserName);
            if (user == null)
                return null;

            var hashedPassword = HashPassword(userLogin.Password);
            if (user.Password != hashedPassword)
                return null;

            var token = _tokenService.GenerateToken(
                user.Id,
                user.UserName,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Role);

            var expiryMinutes = _configuration.GetValue<int>("JwtSettings:ExpiryMinutes", 60);

            return new LoginResponseDto
            {
                Token = token,
                TokenType = "Bearer",
                ExpiresIn = expiryMinutes * 60,
                User = ResponseDto(user)
            };
        }
        //basketActions
        public async Task<Purchase?> AddToBasket(PurchaseBasketDto dto, int userId)
        {

            // 1. שליפת המתנה מהמאגר כדי לבדוק את מצבה
            var gift = await _db.Gifts.FirstOrDefaultAsync(g => g.Id == dto.GiftId);

            if (gift == null)
                return null;

            // 2. בדיקה האם כבר יש זוכה למתנה זו
            // אם WinnerName אינו ריק, סימן שההגרלה כבר התקיימה
            if (!string.IsNullOrEmpty(gift.WinnerName))
            {
                throw new InvalidOperationException("לא ניתן להוסיף לסל: ההגרלה עבור מתנה זו כבר התקיימה.");
            }

            // 3. רק אם אין זוכה, נמשיך ליצירת הרכישה
            var purchase = new Purchase
            {
                UserId = userId,
                GiftId = dto.GiftId,
                Quantity = dto.Quentity,
                Date = DateTime.Now,
                basketStatus = BasketStatus.Draft,
                BusketId = dto.BusketId
            };

            _db.Purchases.Add(purchase);
            await _db.SaveChangesAsync();
            return purchase;
        }


        public async Task<List<PurchaseWithUserDto>> TicketPurchase(PurchaseBasketDto dto, int userId)
        {
            var gifts = await _db.Gifts.FindAsync(dto.GiftId);
            if (gifts != null && !string.IsNullOrEmpty(gifts.WinnerName))
            {
                throw new Exception("לא ניתן לרכוש כרטיסים למתנה זו, ההגרלה כבר התקיימה.");
            }
            var purchasesList = new List<PurchaseWithUserDto>();

            var existingDraft = await _db.Purchases
                .FirstOrDefaultAsync(p => p.UserId == userId && p.GiftId == dto.GiftId && p.basketStatus == BasketStatus.Draft);

            if (existingDraft == null)
            {
                throw new Exception("לא ניתן לבצע רכישה: המתנה לא נמצאת בסל שלך.");
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var gift = await _giftRepository.GetById(dto.GiftId);
            if (user == null || gift == null) return purchasesList;

            _db.Purchases.Remove(existingDraft);

            for (int i = 0; i < dto.Quentity; i++)
            {
                var purchase = new Purchase
                {
                    GiftId = dto.GiftId,
                    UserId = userId,
                    Date = DateTime.Now,
                    Quantity = 1,
                    basketStatus = BasketStatus.Confirmed,
                    BusketId = dto.BusketId

                };

                await _db.Purchases.AddAsync(purchase);

                purchasesList.Add(new PurchaseWithUserDto
                {
                    GiftId = gift.Id,
                    UserId = user.Id,
                    Date = purchase.Date,
                    FirstName = user.FirstName,
                    LastName = user.LastName
                });
            }

            await _db.SaveChangesAsync();
            return purchasesList;
        }

        public async Task<bool> ConfirmBasket(int userId)
        {
            var drafts = await _db.Purchases
                .Where(p => p.UserId == userId && p.basketStatus == BasketStatus.Draft)
                .ToListAsync();

            if (!drafts.Any()) return false;

            foreach (var draft in drafts)
            {
                for (int i = 0; i < draft.Quantity; i++)
                {
                    _db.Purchases.Add(new Purchase
                    {
                        GiftId = draft.GiftId,
                        UserId = userId,
                        Date = DateTime.Now,
                        Quantity = 1,
                        basketStatus = BasketStatus.Confirmed,
                        BusketId = draft.BusketId

                    });
                }
                _db.Purchases.Remove(draft);
            }

            await _db.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeletePurchase(int purchaseId, int userId)
        {
            var purchase = await _db.Purchases.FirstOrDefaultAsync(p => p.Id == purchaseId && p.UserId == userId);

            if (purchase == null) return false;

            if (purchase.basketStatus == BasketStatus.Confirmed)
                throw new Exception("לא ניתן למחוק מתנה שכבר נרכשה בפועל!");

            _db.Purchases.Remove(purchase);
            await _db.SaveChangesAsync();
            return true;
        }
        //checkit
        public async Task<List<GiftWithWinnerDto>> GetGiftsWithWinners()
        {
            return await _db.Gifts
                .Select(g => new GiftWithWinnerDto
                {
                    GiftName = g.Name,
                    WinnerName = g.WinnerName
                }).ToListAsync();
        }
        public async Task<bool> DeleteFromBasket(int purchaseId, int userId)
        {
            var purchase = await _db.Purchases
                .FirstOrDefaultAsync(p => p.Id == purchaseId && p.UserId == userId);

            if (purchase == null)
                return false;

            if (purchase.basketStatus == BasketStatus.Confirmed)
            {
                throw new InvalidOperationException("לא ניתן למחוק מתנה שכבר נרכשה בפועל.");
            }

            _db.Purchases.Remove(purchase);
            await _db.SaveChangesAsync();
            return true;
        }
//Actions
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
                BuildingNumber = user.BuildingNumber
            };
        }

        private static string HashPassword(string password)
        {
            return Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
}
