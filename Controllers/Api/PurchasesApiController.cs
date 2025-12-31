using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TicketApp.Data.Concrete.EfCore;
using TicketApp.Entity;

namespace TicketApp.Controllers.Api
{
    public record PurchaseRequest(int TicketId, List<int> SeatIds);

    [ApiController]
    [Route("api/purchases")]
    public class PurchasesApiController : ControllerBase
    {
        private readonly TicketContext _context;

        public PurchasesApiController(TicketContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PurchaseRequest req)
        {
            if (!User.Identity!.IsAuthenticated)
                return Unauthorized();

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            using var tx = await _context.Database.BeginTransactionAsync();

            var seats = await _context.Seats
                .Where(s => req.SeatIds.Contains(s.Id))
                .ToListAsync();

            if (seats.Count != req.SeatIds.Count)
                return BadRequest("Bazı koltuklar bulunamadı.");

            if (seats.Any(s => s.IsReserved))
                return Conflict("Seçtiğiniz koltuklardan bazıları az önce rezerve edildi.");

            foreach (var seat in seats)
            {
                seat.IsReserved = true;

                _context.TicketPurchases.Add(new TicketPurchase
                {
                    TicketId = req.TicketId,
                    SeatId = seat.Id,
                    UserId = userId,
                    PurchaseDate = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return Ok(new { message = "Satın alma başarılı" });
        }
    }
}
