using TicketApp.Data.Abstract;
using TicketApp.Entity;
using TicketApp.Data;
using Microsoft.EntityFrameworkCore;

namespace TicketApp.Data.Concrete.EfCore
{
    public class EfCoreTicketPurchaseRepository : ITicketPurchaseRepository
    {
        private readonly TicketContext _context;

        public EfCoreTicketPurchaseRepository(TicketContext context)
        {
            _context = context;
        }

        public IQueryable<TicketPurchase> TicketPurchases => _context.TicketPurchases;

        public async Task CreatePurchaseAsync(TicketPurchase purchase)
        {
            _context.TicketPurchases.Add(purchase);
            await _context.SaveChangesAsync();
        }
    }
}
