using GreaterGradesBackend.Domain.Entities;
using GreaterGradesBackend.Domain.Interfaces;
using GreaterGradesBackend.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GreaterGradesBackend.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(GreaterGradesBackendDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Classes)
                .Include(u => u.TaughtClasses)
                .Include(u => u.Grades)
                .ToListAsync();
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.Classes)
                .Include(u => u.Grades)
                .Include(u => u.TaughtClasses)
                .FirstOrDefaultAsync(u => u.Username == username);
        }
        public async Task<User> GetUserWithDetailsAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Classes)
                .Include(u => u.Grades)
                .Include(u => u.TaughtClasses)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

    }
}