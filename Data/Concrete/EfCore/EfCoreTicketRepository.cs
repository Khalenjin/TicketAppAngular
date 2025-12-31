using TicketApp.Data;
using TicketApp.Data.Abstract;
using TicketApp.Entity;

namespace TicketApp.Data.Concrete.EfCore
{
public class EfCoreTicketRepository : ITicketRepository
{
    private readonly TicketContext _context;

    public EfCoreTicketRepository(TicketContext context)
    {
        _context = context;
    }

    public IQueryable<Ticket> Tickets => _context.Tickets;

    public async Task CreateTicketAsync(Ticket ticket)
    {
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
    }
}
}