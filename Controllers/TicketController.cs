using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TicketApp.Data.Abstract;
using TicketApp.Entity;
using TicketApp.Models;
using TicketApp.Data.Concrete.EfCore;

namespace TicketApp.Controllers
{
    public class TicketController : Controller
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ITicketPurchaseRepository _purchaseRepository;
        private readonly TicketContext _context;

        public TicketController(
            ITicketRepository ticketRepository,
            ITicketPurchaseRepository purchaseRepository,
            TicketContext context)
        {
            _ticketRepository = ticketRepository;
            _purchaseRepository = purchaseRepository;
            _context = context;
        }

        public async Task<IActionResult> Details(int id)
        {
            var ticket = await _ticketRepository.Tickets.FirstOrDefaultAsync(x => x.Id == id);

            if (ticket == null)
                return NotFound();

            return View(ticket);
        }

        public IActionResult SelectPerson(int ticketId)
        {
            var model = new SelectPersonCountViewModel
            {
                TicketId = ticketId
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult SelectPerson(SelectPersonCountViewModel model)
        {
            return RedirectToAction("SelectSeat", new
            {
                ticketId = model.TicketId,
                count = model.PersonCount
            });
        }

    
        public IActionResult SelectSeat(int ticketId, int count)
            {
                var ticket = _context.Tickets
                    .Include(t => t.Hall)
                    .ThenInclude(h => h.Seats)
                    .FirstOrDefault(t => t.Id == ticketId);

                if (ticket == null)
                    return NotFound();

                var seats = ticket.Hall.Seats
                    .OrderBy(s => s.RowNumber)
                    .ThenBy(s => s.SeatNumber)
                    .ToList();

                var model = new SelectSeatViewModel
                {
                    TicketId = ticketId,
                    Seats = seats,
                    PersonCount = count 
                };

                return View(model);
            }



       [HttpPost]
        public IActionResult ConfirmSeats(SelectSeatViewModel model)
        {
            if (!string.IsNullOrEmpty(model.SelectedSeatIdsString))
            {
                model.SelectedSeatIds = model.SelectedSeatIdsString
                    .Split(',')
                    .Select(int.Parse)
                    .ToList();
            }

            model.Seats = _context.Seats
                .Where(s => model.SelectedSeatIds.Contains(s.Id))
                .ToList();

            if (model.SelectedSeatIds.Count != model.PersonCount)
            {
                ModelState.AddModelError("", $"Lütfen tam olarak {model.PersonCount} koltuk seçin.");

                model.Seats = _context.Seats
                    .Where(s => s.HallId == _context.Tickets.Find(model.TicketId)!.HallId)
                    .ToList();

                return View("SelectSeat", model);
            }

            var ticket = _context.Tickets.Find(model.TicketId);
            model.TotalPrice = ticket!.Price * model.SelectedSeatIds.Count;

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> BuySeats(SelectSeatViewModel model)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            foreach (var seatId in model.SelectedSeatIds)
            {
                var seat = await _context.Seats.FindAsync(seatId);
                if (seat == null || seat.IsReserved)
                    continue;

                seat.IsReserved = true;

                _context.TicketPurchases.Add(new TicketPurchase
                {
                    TicketId = model.TicketId,
                    SeatId = seatId,
                    UserId = userId,
                    PurchaseDate = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Profile", "Users");
        }
    }
}
