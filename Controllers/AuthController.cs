using Microsoft.AspNetCore.Mvc;
using Orcamento.Services;
using Orcamento.Dtos;

namespace Orcamento.Controllers
{
     [ApiController]
     [Route("auth")]
     public class AuthController : ControllerBase
     {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var usuario = await _authService.RegistrarUsuario(dto);

            if (usuario == null)
            {
                return BadRequest("Usuário já existe.");
            }

            return Ok("Usuário criado com sucesso.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var resultado = await _authService.LoginUsuario(dto);

            if (resultado == null)
            {
                return Unauthorized("Email ou senha inválidos!");
            }

            return Ok(new
            {
                accessToken = resultado.Value.AccessToken,
                refreshToken = resultado.Value.RefreshToken
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshDto dto)
        {
            var resultado = await _authService.RefreshToken(dto.RefreshToken);

            if (resultado == null)
            {
                return Unauthorized("Refresh token inválido ou expirado.");
            }

            return Ok(new
            {
                accessToken = resultado.Value.AccessToken,
                refreshToken = resultado.Value.RefreshToken
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshDto dto)
        {
            var sucesso = await _authService.Logout(dto.RefreshToken);

            if (!sucesso)
            {
                return BadRequest("Token inválido.");
            }

            return Ok("Logout realizado com sucesso.");
        }
    }
}
