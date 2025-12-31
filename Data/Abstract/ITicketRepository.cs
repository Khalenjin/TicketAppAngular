using TicketApp.Entity;

namespace TicketApp.Data.Abstract
{
public interface ITicketRepository
{
    IQueryable<Ticket> Tickets { get; }
    Task CreateTicketAsync(Ticket ticket);
}
}
