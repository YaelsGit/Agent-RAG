using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApi.Interface;
using WebApi.Service;
using WebApi.Validation;
using static WebApi.DTOs.UserDTO;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ITokenService tokenService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<UserResponseDto>> CreatRegister([FromBody] UserRegisterDto userRegister)
        {
            _logger.LogInformation("CreatRegister called for user {UserName}", userRegister.UserName);
            try
            {
                var userCreated = await _userService.UserRegister(userRegister);
                return userCreated;
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto data)
        {
            _logger.LogInformation("Login called for user {UserName}", data.Username);
            var loginDto = new UserLoginedDto
            {
                UserName = data.Username,
                Password = data.Password
            };
            var response = await _userService.UserLogin(loginDto);

            if (response == null)
            {
                return Unauthorized(new { message = "שם משתמש או סיסמה שגויים" });
            }
 
            return Ok(response);
        }

    }
}

