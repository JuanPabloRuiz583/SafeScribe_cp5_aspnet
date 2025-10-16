using cp5.DTOS;
using cp5.models;
using cp5.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;

namespace cp5.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private static List<User> _users = new(); // Simula o banco
        private readonly ITokenService _tokenService;
        private readonly ITokenBlacklistService _blacklistService;

        public AuthController(ITokenService tokenService, ITokenBlacklistService blacklistService)
        {
            _tokenService = tokenService;
            _blacklistService = blacklistService;
        }

        [HttpPost("registrar")]
        public IActionResult Registrar(UserRegisterDto dto)
        {
            if (_users.Any(u => u.Username == dto.Username))
                return BadRequest("Usuário já existe.");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Id = _users.Count + 1,
                Username = dto.Username,
                PasswordHash = hashedPassword,
                Role = dto.Role
            };

            _users.Add(user);
            return Ok("Usuário registrado com sucesso!");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequestDto dto)
        {
            var user = _users.FirstOrDefault(u => u.Username == dto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Credenciais inválidas.");

            var token = _tokenService.GenerateToken(user);
            return Ok(new { Token = token });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var jti = User.Claims.FirstOrDefault(c => c.Type == "jti")?.Value;
            if (string.IsNullOrEmpty(jti))
                return BadRequest("Token inválido.");

            await _blacklistService.AddToBlacklistAsync(jti);
            return Ok("Logout realizado com sucesso!");
        }
    }
}
