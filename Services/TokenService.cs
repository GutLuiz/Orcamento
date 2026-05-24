using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Orcamento.Data;
using Orcamento.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Orcamento.Services
{
    public class TokenService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public TokenService(AppDbContext context,IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public string GenerateAccessToken(User user)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Key"]!);
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                }),
                Expires = DateTime.UtcNow.AddMinutes(60),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<string> GenerateRefreshTokenAsync(User user)
        {
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            // desativa tokens antigos
            var tokensAntigos = _context.RefreshTokens
                .Where(r => r.UserId == user.Id && r.IsActive);

            foreach (var t in tokensAntigos)
            {
                t.IsActive = false;
            }
                

            _context.RefreshTokens.Add(new RefreshToken
            {
                Token = token,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            });

            await _context.SaveChangesAsync();
            return token;
        }

        public async Task<(string AccessToken, string RefreshToken)?> RefreshAsync(string refreshToken)
        {
            var stored = await _context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == refreshToken && r.IsActive);

            if (stored == null)
                return null;

            if (stored.ExpiresAt < DateTime.UtcNow)
                return null;

            // desativa o token usado
            stored.IsActive = false;
            await _context.SaveChangesAsync();

            // gera novo par
            var novoAccess = GenerateAccessToken(stored.User);
            var novoRefresh = await GenerateRefreshTokenAsync(stored.User);

            return (novoAccess, novoRefresh);
        }
    }
}