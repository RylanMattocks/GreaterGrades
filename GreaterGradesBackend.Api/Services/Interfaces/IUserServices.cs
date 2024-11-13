using GreaterGradesBackend.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GreaterGradesBackend.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(int userId);
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<string> AuthenticateUserAsync(LoginDto loginDto);
        Task<bool> UpdateUserAsync(int userId, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(int userId);
        Task<UserDto> GetUserFromUsernameAsync(string username);
    }
}
