using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orcamento.Dtos;
using Orcamento.Dtos.Data;
using Orcamento.Models;
using System.Reflection;

namespace Orcamento.Services
{
    public class AuthService
    {
        private readonly TokenService _tokenService;

        private readonly AppDbContext _context;

        public AuthService(TokenService tokenService, AppDbContext context)
        {
            _tokenService = tokenService;
            _context = context;
        }

        public async Task<User?> RegistrarUsuario(RegisterDto dto)
        {
            var usuarioExiste = await _context.Users
                .FirstOrDefaultAsync(t => t.Email == dto.Email);

            if (usuarioExiste != null)
            {
                return null;
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Email = dto.Email.Trim().ToLower(),
                PasswordHash = passwordHash
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return user;
        }
        public async Task<(string AccessToken, string RefreshToken)?> LoginUsuario(LoginDto dto)
        {
            var usuarioExiste = await _context.Users.FirstOrDefaultAsync(t => t.Email == dto.Email);

            if(usuarioExiste == null)
            {
                return null;
            }

            var validarPassword = BCrypt.Net.BCrypt.Verify(dto.Password, usuarioExiste.PasswordHash);

            if (!validarPassword)
            {
                return null;
            }

            var accessToken = _tokenService.GenerateAccessToken(usuarioExiste);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync(usuarioExiste);

            return (accessToken, refreshToken);
        }
        public async Task<(string AccessToken, string RefreshToken)?> RefreshToken(string refreshToken)
        {
            var resultado = await _tokenService.RefreshAsync(refreshToken);

            if (resultado == null)
            {
                return null;
            }

            return resultado;
        }
        public async Task<bool> Logout(string refreshToken)
        {
            var stored = await _context.RefreshTokens
                .FirstOrDefaultAsync(r => r.Token == refreshToken && r.IsActive);

            if (stored == null)
            {
                return false;
            }
                
            stored.IsActive = false;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
