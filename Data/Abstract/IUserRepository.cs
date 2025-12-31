using Microsoft.EntityFrameworkCore;
using TicketApp.Entity;

namespace TicketApp.Data.Abstract
{
    public interface IUserRepository
    {
        IQueryable<User> Users { get; }
        Task<User?> GetUserByEmailAsync(string email);
        Task CreateUser(User user);
    }
}
