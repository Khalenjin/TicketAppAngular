using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using TicketApp.Data.Abstract;
using TicketApp.Entity;

namespace TicketApp.Controllers.Api
{
    [ApiController]
    [Route("api/auth")]
    public class AuthApiController : ControllerBase
    {
        private readonly IUserRepository _users;

        public AuthApiController(IUserRepository users)
        {
            _users = users;
        }

        public record LoginRequest(string EmailOrUserName, string Password);
        public record RegisterRequest(string UserName, string Email, string Password);

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.EmailOrUserName) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest(new { message = "Email/username ve şifre zorunlu." });

            var user = await _users.Users.FirstOrDefaultAsync(x =>
                x.Email == req.EmailOrUserName || x.UserName == req.EmailOrUserName);

            if (user == null || user.Password != req.Password)
                return Unauthorized(new { message = "Girdiğiniz bilgiler yanlış." });

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(ClaimTypes.Role, user.Role ?? "User") // ✅ EKLENDİ (ROLE CLAIM)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok(new
            {
                id = user.Id,
                userName = user.UserName,
                email = user.Email,
                role = user.Role // ✅ EKLENDİ (frontend admin kontrol için)
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.UserName) ||
                string.IsNullOrWhiteSpace(req.Email) ||
                string.IsNullOrWhiteSpace(req.Password))
                return BadRequest(new { message = "Tüm alanlar zorunlu." });

            var exist = await _users.GetUserByEmailAsync(req.Email);
            if (exist != null)
                return Conflict(new { message = "Bu e-posta kullanılmaktadır." });

            var user = new User
            {
                UserName = req.UserName,
                Email = req.Email,
                Password = req.Password,
                Role = "User" // ✅ EKLENDİ: default role
            };

            await _users.CreateUser(user);

            return Ok(new { message = "Kayıt başarılı." });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { message = "Çıkış yapıldı." });
        }

        [HttpGet("me")]
        public IActionResult Me()
        {
            if (!(User?.Identity?.IsAuthenticated ?? false))
                return Unauthorized();

            return Ok(new
            {
                id = User.FindFirstValue(ClaimTypes.NameIdentifier), // ✅ SENİN ARADIĞIN KODUN YERİ BURASI
                userName = User.Identity?.Name,
                role = User.FindFirstValue(ClaimTypes.Role) ?? "User" // ✅ EKLENDİ
            });
        }
    }
}
