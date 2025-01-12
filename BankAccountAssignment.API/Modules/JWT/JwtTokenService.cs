using BankAccountAssignment.API.Modules.Data;
using BankAccountAssignment.API.Modules.UserModules.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;

namespace BankAccountAssignment.API.Modules.JWT
{
    public class JwtTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public JwtTokenService(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        public string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:DurationInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public void BlacklistToken(string token, int userID)
        {
            using(var scope = _serviceProvider.CreateScope()) {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Add(new ExpiredUserToken {
                    Token = token,
                    UserId = userID
                });
                context.SaveChangesAsync();
            }
            return;
        }

        public bool IsTokenBlacklisted(string token)
        {
            using (var scope = _serviceProvider.CreateScope()) {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var isExists = context.Set<ExpiredUserToken>().Any(x => x.Token == token);
                return isExists;
            }
        }
    }
}
