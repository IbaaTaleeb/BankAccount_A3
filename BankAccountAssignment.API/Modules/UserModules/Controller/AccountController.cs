using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BankAccountAssignment.API.Modules.UserModules.Services;

namespace BankAccountAssignment.API.Modules.UserModules.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        // Register Endpoint
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistration registrationDto)
        {
            var result = await _userService.RegisterUserAsync(registrationDto);

            if (!result)
            {
                return BadRequest("User registration failed. Email might already exist.");
            }

            return Ok(new { Message = "User registered successfully." });
        }

        // Login Endpoint
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin loginDto)
        {
            var token = await _userService.LoginUserAsync(loginDto);

            if (token == null)
            {
                return Unauthorized(new { Message = "Invalid credentials." });
            }

            return Ok(new { Message = "Login successful.", Token = token });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Extract the token from the Authorization header
            var token = HttpContext.Request.Headers["Authorization"].ToString().Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { Message = "Token is required." });
            }

            var result = await _userService.LogoutUserAsync(token);

            if (!result)
            {
                return BadRequest(new { Message = "Logout failed." });
            }

            return Ok(new { Message = "Logout successful." });
        }
    }
}
