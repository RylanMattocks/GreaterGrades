using GreaterGradesBackend.Domain.Entities;
using System.Threading.Tasks;

namespace GreaterGradesBackend.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserWithDetailsAsync(int userId);
        Task<IEnumerable<User>> GetAllUsersAsync();
    }
}