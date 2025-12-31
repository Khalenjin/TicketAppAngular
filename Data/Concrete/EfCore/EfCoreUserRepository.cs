using TicketApp.Data.Abstract;
using Microsoft.EntityFrameworkCore;
using TicketApp.Entity;

namespace TicketApp.Data.Concrete.EfCore
{
    public class EfCoreUserRepository : IUserRepository
    {
        private readonly TicketContext _context;

        public EfCoreUserRepository(TicketContext context)
        {
            _context = context;
        }

        public IQueryable<User> Users => _context.Users;

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task CreateUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
    }
}
