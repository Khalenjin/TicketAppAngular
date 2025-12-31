using TicketApp.Entity;

namespace TicketApp.Data.Abstract
{
    public interface ITicketPurchaseRepository
    {
        IQueryable<TicketPurchase> TicketPurchases { get; }
        Task CreatePurchaseAsync(TicketPurchase purchase);
    }
}
