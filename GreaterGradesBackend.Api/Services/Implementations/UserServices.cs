using AutoMapper;
using GreaterGradesBackend.Api.Models;
using GreaterGradesBackend.Domain.Entities;
using GreaterGradesBackend.Domain.Interfaces;
using GreaterGradesBackend.Services.Interfaces;
using GreaterGradesBackend.Jwt;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace GreaterGradesBackend.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher; // For hashing passwords
        private readonly string _jwtSecret;
        private readonly int _jwtExpirationMinutes; // Expiration time for tokens

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IPasswordHasher<User> passwordHasher, JwtSettings jwtSettings)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _jwtSecret = jwtSettings.Secret;
            _jwtExpirationMinutes = jwtSettings.ExpirationMinutes;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Users.GetAllUsersAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetUserWithDetailsAsync(userId);
            if (user == null)
            {
                return null;
            }
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            // Check if the username already exists
            var existingUser = await _unitOfWork.Users.GetUserByUsernameAsync(createUserDto.Username);
            var existingInstitution = await _unitOfWork.Institutions.GetInstitutionWithDetailsAsync(createUserDto.InstitutionId);
            if (existingUser != null)
            {
                throw new Exception("Username already exists.");
            }
            if (existingInstitution == null)
            {
                throw new Exception("Institution does not exist");
            }

            // Create a new User entity
            var user = _mapper.Map<User>(createUserDto);
            user.PasswordHash = _passwordHasher.HashPassword(user, createUserDto.Password); // Hash the password

            await _unitOfWork.Users.AddAsync(user);

            await _unitOfWork.CompleteAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> GetUserFromUsernameAsync(string username)
        {
            var user = await _unitOfWork.Users.GetUserByUsernameAsync(username);
            if (user == null)
            {
                return null; // Authentication failed
            }
            return _mapper.Map<UserDto>(user);
        }

        public async Task<string> AuthenticateUserAsync(LoginDto loginDto)
        {
            var user = await _unitOfWork.Users.GetUserByUsernameAsync(loginDto.Username);
            if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password) == PasswordVerificationResult.Failed)
            {
                return null; // Authentication failed
            }

            // Generate JWT token
            return GenerateJwtToken(user);
        }

        public async Task<bool> UpdateUserAsync(int userId, UpdateUserDto updateUserDto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            _mapper.Map(updateUserDto, user); // Update user fields from DTO
            _unitOfWork.Users.Update(user);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            foreach (Grade grade in user.Grades)
            {
                _unitOfWork.Grades.Remove(grade);
            }

            _unitOfWork.Users.Remove(user);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            // Extracting and logging the role claim before creating the token
            var roleClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtExpirationMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
