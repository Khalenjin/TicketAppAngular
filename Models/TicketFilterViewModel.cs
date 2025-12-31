using TicketApp.Entity;

namespace TicketApp.Models
{
    public class TicketFilterViewModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public List<Ticket> Tickets { get; set; } = new();
    }
}
