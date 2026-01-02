using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketApp.Data.Abstract;
using TicketApp.Data.Concrete.EfCore;
using Microsoft.AspNetCore.Authorization;
using TicketApp.Entity;

namespace TicketApp.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsApiController : ControllerBase
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly TicketContext _context;

        public TicketsApiController(
            ITicketRepository ticketRepository,
            TicketContext context)
        {
            _ticketRepository = ticketRepository;
            _context = context;
        }

        // =========================
        // ✅ GÖSTERİ OLUŞTUR (ADMIN)
        // =========================
        [HttpPost("shows")]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateShow([FromBody] CreateShowDto dto)
        {
            if (dto == null)
                return BadRequest();

            // 1️⃣ Salon oluştur (gösteriye özel)
            var hall = new Hall
            {
                Name = $"Salon - {dto.Title}",
                RowCount = 10,
                SeatsPerRow = 12
            };
            _context.Halls.Add(hall);
            _context.SaveChanges();

            // 2️⃣ Koltukları üret
            var seats = new List<Seat>();
            for (int row = 1; row <= hall.RowCount; row++)
            {
                for (int seat = 1; seat <= hall.SeatsPerRow; seat++)
                {
                    seats.Add(new Seat
                    {
                        HallId = hall.Id,
                        RowNumber = row,
                        SeatNumber = seat,
                        IsReserved = false
                    });
                }
            }
            _context.Seats.AddRange(seats);
            _context.SaveChanges();

            // 3️⃣ Gösteri oluştur
            var ticket = new Ticket
            {
                PlayName = dto.Title,
                Date = dto.Date,
                Price = dto.Price,
                HallId = hall.Id
            };
            _context.Tickets.Add(ticket);
            _context.SaveChanges();

            return Ok(new { message = "Gösteri başarıyla oluşturuldu." });
        }

        // =========================
        // ✅ GÖSTERİLERİ GETİR
        // =========================
        [HttpGet("/api/shows")]
        public IActionResult GetShows(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var query = _ticketRepository.Tickets
                .Include(t => t.Hall)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(t => t.Date.Date >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(t => t.Date.Date <= endDate.Value.Date);

            var shows = query.Select(t => new
            {
                id = t.Id,
                title = t.PlayName,
                description = $"Salon: {t.Hall!.Name}",
                date = t.Date,
                imageUrl = "/images/default.jpg"
            }).ToList();

            return Ok(shows);
        }

        // =========================
        // ✅ KOLTUKLAR
        // =========================
        [HttpGet("{ticketId}/seats")]
        public IActionResult GetSeats(int ticketId, [FromQuery] int count)
        {
            var ticket = _context.Tickets
                .Include(t => t.Hall)
                .ThenInclude(h => h.Seats)
                .FirstOrDefault(t => t.Id == ticketId);

            if (ticket == null || ticket.Hall == null)
                return NotFound();

            return Ok(new
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
                    })
            });
        }

        // =========================
        // 🗑️ GÖSTERİ SİL (ADMIN)
        // =========================
        [HttpDelete("shows/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteShow(int id)
        {
            var ticket = _context.Tickets.FirstOrDefault(t => t.Id == id);
            if (ticket == null)
                return NotFound();

            var hall = _context.Halls.FirstOrDefault(h => h.Id == ticket.HallId);

            if (hall != null)
            {
                var seats = _context.Seats
                    .Where(s => s.HallId == hall.Id)
                    .ToList();

                _context.Seats.RemoveRange(seats);
                _context.Halls.Remove(hall);
            }

            _context.Tickets.Remove(ticket);
            _context.SaveChanges();

            return Ok(new { message = "Gösteri silindi." });
        }
    }

    // =========================
    // DTO
    // =========================
    public class CreateShowDto
    {
        public string Title { get; set; } = "";
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
    }
}
