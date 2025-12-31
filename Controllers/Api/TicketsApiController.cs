using Microsoft.AspNetCore.Mvc;
using TicketApp.Data.Abstract;

namespace TicketApp.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsApiController : ControllerBase
    {
        private readonly ITicketRepository _ticketRepository;

        public TicketsApiController(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        [HttpGet("/api/shows")]
public IActionResult GetShows()
{
    var shows = _ticketRepository.Tickets.Select(t => new
    {
        id = t.Id,
        title = t.PlayName,             // ✅ PlayName
        description = $"Salon: {t.Hall!.Name}", // (varsa Hall.Name göster)
        date = t.Date,
        imageUrl = "/images/default.jpg"       // veya sabit bir resim kullan
    }).ToList();

    return Ok(shows);
}

    }
}
