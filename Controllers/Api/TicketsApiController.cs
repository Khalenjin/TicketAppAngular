using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketApp.Data.Abstract;
using TicketApp.Data.Concrete.EfCore;

namespace TicketApp.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsApiController : ControllerBase
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly TicketContext _context;

        public TicketsApiController(ITicketRepository ticketRepository, TicketContext context)
        {
            _ticketRepository = ticketRepository;
            _context = context;
        }

        [HttpGet("/api/shows")]
        public IActionResult GetShows()
        {
            var shows = _ticketRepository.Tickets.Select(t => new
            {
                id = t.Id,
                title = t.PlayName,
                description = $"Salon: {t.Hall!.Name}",
                date = t.Date,
                imageUrl = "/images/default.jpg"
            }).ToList();

            return Ok(shows);
        }

        // 🔥 DÜZGÜN YERE ALINDI:
        [HttpGet("{ticketId}/seats")]
        public IActionResult GetSeats(int ticketId, [FromQuery] int count) {
            var ticket = _context.Tickets
                .Include(t => t.Hall)
                .ThenInclude(h => h.Seats)
                .FirstOrDefault(t => t.Id == ticketId);

            if (ticket == null)
                return NotFound();

            var response = new
            {
                ticketId = ticket.Id,
                personCount = count,
                seats = ticket.Hall.Seats
                    .OrderBy(s => s.RowNumber)
                    .ThenBy(s => s.SeatNumber)
                    .Select(s => new
                    {
                        id = s.Id,
                        rowNumber = s.RowNumber,
                        seatNumber = s.SeatNumber,
                        isReserved = s.IsReserved
                    }).ToList()
            };

            return Ok(response);
        }
    }
}
