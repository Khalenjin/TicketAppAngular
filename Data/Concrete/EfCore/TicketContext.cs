using Microsoft.EntityFrameworkCore;
using TicketApp.Entity;

namespace TicketApp.Data.Concrete.EfCore
{
    public class TicketContext : DbContext
    {
        public TicketContext(DbContextOptions<TicketContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users => Set<User>();
        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<TicketPurchase> TicketPurchases => Set<TicketPurchase>();
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Seat> Seats { get; set; }
    }
}
