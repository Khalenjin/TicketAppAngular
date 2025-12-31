using TicketApp.Entity;

namespace TicketApp.Models
{
    public class UserProfileViewModel
    {
        public User User { get; set; } = null!;
        public List<Ticket> Tickets { get; set; } = new();
        public List<TicketPurchase> Purchases { get; set; } = new();

    }
}
