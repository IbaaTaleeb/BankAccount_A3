using System;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using BankAccountAssignment.API.Modules.UserModules.Model;
using BankAccountAssignment.API.Modules.Data;
using System.Collections.Generic;
using BankAccountAssignment.API.Modules.JWT;

namespace BankAccountAssignment.API.Modules.UserModules.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtTokenService _jwtTokenService;
        private readonly HttpContext? _httpContext;

        public UserService(ApplicationDbContext context, JwtTokenService jwtTokenService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _jwtTokenService = jwtTokenService;
            _httpContext = httpContextAccessor.HttpContext;

        }

        public async Task<bool> RegisterUserAsync(UserRegistration registration)
        {
            if (await _context.Users.AnyAsync(u => u.Email == registration.Email))
            {
                return false; // Email already exists
            }

            var user = new User
            {
                Name = registration.Name,
                Email = registration.Email,
                MobileNumber = registration.MobileNumber,
                DateOfBirth = registration.DateOfBirth,
                CreationDate = DateTime.Now,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registration.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<string> LoginUserAsync(UserLogin login)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == login.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
            {
                return null; // Invalid credentials
            }

            return _jwtTokenService.GenerateJwtToken(user);
        }

        public async Task<bool> LogoutUserAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false; // Invalid token
            }

            var currentUserIDClaim = Convert.ToInt32(_httpContext?.User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value);
            _jwtTokenService.BlacklistToken(token, currentUserIDClaim);
            return await Task.FromResult(true);
        }
    }
}
