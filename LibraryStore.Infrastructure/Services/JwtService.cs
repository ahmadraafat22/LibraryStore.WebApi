using LibraryStore.Domain.Entities;
using LibraryStore.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibraryStore.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private IConfiguration _config;
        public JwtService(UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _config = config;
            _userManager = userManager;
        }
        public async Task<string> GenerateToken(ApplicationUser user)
        {
            // to generate token we need 1- claims , 2-signIn Creds , 3-Configuration settings 

            // define claims ls 
            List<Claim> tokenClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            };
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                tokenClaims.Add(new Claim(ClaimTypes.Role, role));
            }
            if (string.IsNullOrEmpty(_config["JWT:Key"]))
            {
                throw new Exception("there is no token Key !");
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));

            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            // generating token 
            var token = new JwtSecurityToken(
                issuer: _config["JWT:Iss"],
                audience: _config["JWT:Aud"],
                signingCredentials: creds,
                claims: tokenClaims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["JWT:ExpiryMinutes"]))
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
