using Xunit;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GreaterGradesBackend.Services.Implementations;
using GreaterGradesBackend.Domain.Interfaces;
using GreaterGradesBackend.Api.Models;
using GreaterGradesBackend.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using GreaterGradesBackend.Jwt;
using System;

namespace GreaterGradesBackend.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IPasswordHasher<User>> _mockPasswordHasher;
        private readonly UserService _userService;
        private readonly JwtSettings _jwtSettings;

        public UserServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockPasswordHasher = new Mock<IPasswordHasher<User>>();
            _jwtSettings = new JwtSettings { Secret = "testsecret", ExpirationMinutes = 30 };
            _userService = new UserService(_mockUnitOfWork.Object, _mockMapper.Object, _mockPasswordHasher.Object, _jwtSettings);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsMappedUsers()
        {
            // Arrange
            var users = new List<User> { new User { UserId = 1, Username = "user1" } };
            _mockUnitOfWork.Setup(u => u.Users.GetAllUsersAsync()).ReturnsAsync(users);
            _mockMapper.Setup(m => m.Map<IEnumerable<UserDto>>(users))
                       .Returns(new List<UserDto> { new UserDto { UserId = 1, Username = "user1" } });

            // Act
            var result = await _userService.GetAllUsersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(1, result.First().UserId);
        }

        [Fact]
        public async Task GetUserByIdAsync_ExistingId_ReturnsMappedUser()
        {
            // Arrange
            var user = new User { UserId = 1, Username = "user1" };
            _mockUnitOfWork.Setup(u => u.Users.GetUserWithDetailsAsync(1)).ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(new UserDto { UserId = 1, Username = "user1" });

            // Act
            var result = await _userService.GetUserByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.UserId);
        }

        [Fact]
        public async Task GetUserByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Users.GetUserWithDetailsAsync(It.IsAny<int>())).ReturnsAsync((User)null);

            // Act
            var result = await _userService.GetUserByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        /*[Fact]
        public async Task CreateUserAsync_UsernameExists_ThrowsException()
        {
            // Arrange
            var createUserDto = new CreateUserDto { Username = "existingUser", InstitutionId = 1 };
            _mockUnitOfWork.Setup(u => u.Users.GetUserByUsernameAsync("existingUser")).ReturnsAsync(new User());

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _userService.CreateUserAsync(createUserDto));
        }*/

        [Fact]
        public async Task CreateUserAsync_InstitutionDoesNotExist_ThrowsException()
        {
            // Arrange
            var createUserDto = new CreateUserDto { Username = "newUser", InstitutionId = 999 };
            _mockUnitOfWork.Setup(u => u.Users.GetUserByUsernameAsync("newUser")).ReturnsAsync((User)null);
            _mockUnitOfWork.Setup(u => u.Institutions.GetInstitutionWithDetailsAsync(999)).ReturnsAsync((Institution)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _userService.CreateUserAsync(createUserDto));
        }

        [Fact]
        public async Task CreateUserAsync_ValidUser_CreatesAndReturnsUser()
        {
            // Arrange
            var createUserDto = new CreateUserDto { Username = "newUser", Password = "password123", InstitutionId = 1 };
            var user = new User { UserId = 1, Username = "newUser" };

            _mockUnitOfWork.Setup(u => u.Users.GetUserByUsernameAsync("newUser")).ReturnsAsync((User)null);
            _mockUnitOfWork.Setup(u => u.Institutions.GetInstitutionWithDetailsAsync(1)).ReturnsAsync(new Institution());
            _mockMapper.Setup(m => m.Map<User>(createUserDto)).Returns(user);
            _mockPasswordHasher.Setup(p => p.HashPassword(user, createUserDto.Password)).Returns("hashedPassword");
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).Returns(Task.FromResult(1));  // Modified to return Task<int>
            _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(new UserDto { UserId = 1, Username = "newUser" });

            // Act
            var result = await _userService.CreateUserAsync(createUserDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.UserId);
        }


        /*[Fact]
        public async Task AuthenticateUserAsync_ValidCredentials_ReturnsJwtToken()
        {
            // Arrange
            var loginDto = new LoginDto { Username = "user1", Password = "password123" };
            var user = new User { UserId = 1, Username = "user1", PasswordHash = "hashedPassword" };

            _mockUnitOfWork.Setup(u => u.Users.GetUserByUsernameAsync("user1")).ReturnsAsync(user);
            _mockPasswordHasher.Setup(p => p.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password))
                               .Returns(PasswordVerificationResult.Success);

            // Act
            var result = await _userService.AuthenticateUserAsync(loginDto);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }*/

        [Fact]
        public async Task AuthenticateUserAsync_InvalidCredentials_ReturnsNull()
        {
            // Arrange
            var loginDto = new LoginDto { Username = "user1", Password = "wrongPassword" };
            var user = new User { UserId = 1, Username = "user1", PasswordHash = "hashedPassword" };

            _mockUnitOfWork.Setup(u => u.Users.GetUserByUsernameAsync("user1")).ReturnsAsync(user);
            _mockPasswordHasher.Setup(p => p.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password))
                               .Returns(PasswordVerificationResult.Failed);

            // Act
            var result = await _userService.AuthenticateUserAsync(loginDto);

            // Assert
            Assert.Null(result);
        }

        /*[Fact]
        public async Task UpdateUserAsync_ExistingUser_UpdatesUser()
        {
            // Arrange
            var updateUserDto = new UpdateUserDto { Username = "updatedUser" };
            var user = new User { UserId = 1, Username = "user1" };

            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(1)).ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map(updateUserDto, user));
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _userService.UpdateUserAsync(1, updateUserDto);

            // Assert
            Assert.True(result);
        }*/

        [Fact]
        public async Task UpdateUserAsync_NonExistingUser_ReturnsFalse()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((User)null);

            // Act
            var result = await _userService.UpdateUserAsync(999, new UpdateUserDto());

            // Assert
            Assert.False(result);
        }

        /*[Fact]
        public async Task DeleteUserAsync_ExistingUser_DeletesUserAndGrades()
        {
            // Arrange
            var user = new User { UserId = 1, Grades = new List<Grade> { new Grade { GradeId = 1 } } };

            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(1)).ReturnsAsync(user);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _userService.DeleteUserAsync(1);

            // Assert
            Assert.True(result);
            _mockUnitOfWork.Verify(u => u.Grades.Remove(It.IsAny<Grade>()), Times.Once);
            _mockUnitOfWork.Verify(u => u.Users.Remove(user), Times.Once);
        }*/

        [Fact]
        public async Task DeleteUserAsync_NonExistingUser_ReturnsFalse()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((User)null);

            // Act
            var result = await _userService.DeleteUserAsync(999);

            // Assert
            Assert.False(result);
        }
    }
}
