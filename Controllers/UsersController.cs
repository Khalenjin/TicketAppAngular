using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using TicketApp.Data.Abstract;
using TicketApp.Models;
using Microsoft.EntityFrameworkCore;
using TicketApp.Entity;

namespace TicketApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly ITicketPurchaseRepository _purchaseRepository;

        public UsersController(IUserRepository userRepository, ITicketRepository ticketRepository, ITicketPurchaseRepository purchaseRepository)
        {
            _userRepository = userRepository;
            _ticketRepository = ticketRepository;
            _purchaseRepository = purchaseRepository;
        }

        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
                return RedirectToAction("Profile");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userRepository.Users
            .FirstOrDefaultAsync(x =>
            x.Email == model.Email || x.UserName == model.Email);

            if (user == null || user.Password != model.Password)
            {
                ModelState.AddModelError("", "Girdiğiniz bilgiler yanlış.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return RedirectToAction("Profile");
        }


        public IActionResult Register()
        {
            if (User.Identity!.IsAuthenticated)
                return RedirectToAction("Profile");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Email daha önce kullanılmış mı?
            var exist = await _userRepository.GetUserByEmailAsync(model.Email!);

            if (exist != null)
            {
                ModelState.AddModelError("", "Bu e-posta kullanılmaktadır.");
                return View(model);
            }

            var user = new User
            {
                UserName = model.UserName!,
                Email = model.Email!,
                Password = model.Password!
            };

            await _userRepository.CreateUser(user);

            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Profile()
            {
                if (!User.Identity!.IsAuthenticated)
                    return RedirectToAction("Login");

                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var user = await _userRepository.Users
                    .FirstOrDefaultAsync(x => x.Id == userId);

                if (user == null)
                    return NotFound();

                var purchases = await _purchaseRepository.TicketPurchases
                    .Where(x => x.UserId == userId)
                    .Include(x => x.Ticket)
                    .Include(x => x.Seat) 
                    .ToListAsync();

                var model = new UserProfileViewModel
                {
                    User = user,
                    Purchases = purchases
                };

                return View(model);
            }


    }
}
