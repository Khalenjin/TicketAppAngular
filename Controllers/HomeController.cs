using Microsoft.AspNetCore.Mvc;
using TicketApp.Data.Abstract;
using Microsoft.EntityFrameworkCore;
using TicketApp.Models;

namespace TicketApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITicketRepository _ticketRepository;

        public HomeController(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            var query = _ticketRepository.Tickets.AsQueryable();

                if (startDate.HasValue)
                    query = query.Where(t => t.Date >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(t => t.Date <= endDate.Value);

                var model = new TicketFilterViewModel
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Tickets = await query.OrderBy(t => t.Date).ToListAsync()
                };

                return View(model);
        }

    }
}
