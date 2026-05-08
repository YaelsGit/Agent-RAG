using WebApi.DTOs;
using WebApi.Models;
using static WebApi.DTOs.PurchaseDTO;

using static WebApi.DTOs.UserDTO;
using static WebApi.DTOs.WinnerDTO;

namespace WebApi.Interface
{
    public interface IUserService
    {
        Task<UserResponseDto> UserRegister(UserRegisterDto userRegister);
        Task<LoginResponseDto?> UserLogin(UserLoginedDto userLogin);
        Task<Purchase?> AddToBasket(PurchaseBasketDto dto, int userId);
        Task<List<PurchaseWithUserDto>> TicketPurchase(PurchaseBasketDto dto, int userId);
        Task<bool> ConfirmBasket(int userId);
        Task<bool> DeleteFromBasket(int purchaseId, int userId);
        Task<List<GiftWithWinnerDto>> GetGiftsWithWinners();
        Task<List<object>> GetUserBasket(int userId); // הפונקציה החדשה
    }
}
