using WebApi.Models;
using static WebApi.DTOs.PurchaseDTO;
using static WebApi.DTOs.UserDTO;

namespace WebApi.Interface
{
    public interface IUserService
    {
        Task<UserResponseDto> UserRegister(UserRegisterDto userRegister);
        Task<LoginResponseDto?> UserLogin(UserLoginedDto userLogin);
        Task<PurchaseBasketUserDto?> AddToBasket(PurchaseBasketDto Purchase, int userId);
        Task<List<PurchaseWithUserDto?>> TicketPurchase(PurchaseBasketDto purchase, int userId);
    }
}
