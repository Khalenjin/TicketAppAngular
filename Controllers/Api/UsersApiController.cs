using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketApp.Data.Abstract;
using System.Security.Claims;

namespace TicketApp.Controllers.Api
{

[ApiController]
[Route("api/profile")]
public class ProfileApiController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ITicketPurchaseRepository _purchaseRepository;

    public ProfileApiController(IUserRepository userRepository, ITicketPurchaseRepository purchaseRepository)
    {
        _userRepository = userRepository;
        _purchaseRepository = purchaseRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        if (!User.Identity!.IsAuthenticated)
            return Unauthorized();

        int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var user = await _userRepository.Users.FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
            return NotFound();

        var purchases = await _purchaseRepository.TicketPurchases
            .Where(x => x.UserId == userId)
            .Include(x => x.Ticket)
            .ThenInclude(t => t.Hall)
            .Include(x => x.Seat)
            .ToListAsync();

        return Ok(new
        {
            id = user.Id,
            userName = user.UserName,
            email = user.Email,
            purchases = purchases.Select(p => new
            {
                playName = p.Ticket!.PlayName,
                hall = p.Ticket.Hall!.Name,
                date = p.Ticket.Date,
                seat = $"{p.Seat!.RowNumber}. sıra {p.Seat.SeatNumber}. koltuk"
            })
        });
    }
}
}