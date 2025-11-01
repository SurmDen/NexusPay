using Identity.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Identity.Application.Services
{
    public class JwtTokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(string name, string email, Guid id)
        {
            List<Claim> claims = new List<Claim>()
            {
                 new Claim(ClaimTypes.Email, email),
                 new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                 new Claim(ClaimTypes.Name, name),
            };

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"] ?? throw new InvalidOperationException("security key was null")));

            var credentials = new SigningCredentials(secretKey, "HS256");

            var tokenOptions = new JwtSecurityToken
                (
                    issuer: _configuration["JWT:Issuer"],
                    audience: _configuration["JWT:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: credentials
                );

            string token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return token;
        }
    }
}
