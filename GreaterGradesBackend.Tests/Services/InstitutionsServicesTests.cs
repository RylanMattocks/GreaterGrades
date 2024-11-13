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

namespace GreaterGradesBackend.Tests
{
    public class InstitutionServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly InstitutionService _institutionService;

        public InstitutionServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _institutionService = new InstitutionService(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetInstitutionByIdAsync_ExistingId_ReturnsMappedInstitution()
        {
            // Arrange
            var institution = new Institution { InstitutionId = 1, Name = "Test Institution" };
            _mockUnitOfWork.Setup(u => u.Institutions.GetInstitutionWithDetailsAsync(1)).ReturnsAsync(institution);
            _mockMapper.Setup(m => m.Map<InstitutionDto>(institution)).Returns(new InstitutionDto { InstitutionId = 1, Name = "Test Institution" });

            // Act
            var result = await _institutionService.GetInstitutionByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.InstitutionId);
        }

        [Fact]
        public async Task GetInstitutionByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Institutions.GetInstitutionWithDetailsAsync(It.IsAny<int>())).ReturnsAsync((Institution)null);

            // Act
            var result = await _institutionService.GetInstitutionByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllInstitutionsAsync_ReturnsMappedInstitutions()
        {
            // Arrange
            var institutions = new List<Institution> { new Institution { InstitutionId = 1, Name = "Test Institution" } };
            _mockUnitOfWork.Setup(u => u.Institutions.GetAllAsync()).ReturnsAsync(institutions);
            _mockMapper.Setup(m => m.Map<IEnumerable<InstitutionDto>>(institutions))
                       .Returns(new List<InstitutionDto> { new InstitutionDto { InstitutionId = 1, Name = "Test Institution" } });

            // Act
            var result = await _institutionService.GetAllInstitutionsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(1, result.First().InstitutionId);
        }

        [Fact]
        public async Task CreateInstitutionAsync_ValidInstitution_ReturnsCreatedInstitution()
        {
            // Arrange
            var createInstitutionDto = new CreateInstitutionDto { Name = "New Institution" };
            var institution = new Institution { InstitutionId = 1, Name = "New Institution" };

            _mockMapper.Setup(m => m.Map<Institution>(createInstitutionDto)).Returns(institution);
            _mockUnitOfWork.Setup(u => u.Institutions.AddAsync(institution)).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).Returns(Task.FromResult(1));  // Modified to return Task<int>
            _mockMapper.Setup(m => m.Map<InstitutionDto>(institution)).Returns(new InstitutionDto { InstitutionId = 1, Name = "New Institution" });

            // Act
            var result = await _institutionService.CreateInstitutionAsync(createInstitutionDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.InstitutionId);
            Assert.Equal("New Institution", result.Name);
        }

        [Fact]
        public async Task UpdateInstitutionAsync_ExistingInstitution_UpdatesInstitution()
        {
            // Arrange
            var updateInstitutionDto = new UpdateInstitutionDto { Name = "Updated Institution" };
            var institution = new Institution { InstitutionId = 1, Name = "Old Institution" };

            _mockUnitOfWork.Setup(u => u.Institutions.GetByIdAsync(1)).ReturnsAsync(institution);
            _mockMapper.Setup(m => m.Map(updateInstitutionDto, institution));
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).Returns(Task.FromResult(1));

            // Act
            var result = await _institutionService.UpdateInstitutionAsync(1, updateInstitutionDto);

            // Assert
            Assert.True(result);
            _mockUnitOfWork.Verify(u => u.Institutions.Update(institution), Times.Once);
        }

        [Fact]
        public async Task UpdateInstitutionAsync_NonExistingInstitution_ReturnsFalse()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Institutions.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Institution)null);

            // Act
            var result = await _institutionService.UpdateInstitutionAsync(999, new UpdateInstitutionDto());

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteInstitutionAsync_ExistingInstitution_DeletesInstitution()
        {
            // Arrange
            var institution = new Institution { InstitutionId = 1, Name = "Test Institution" };
            _mockUnitOfWork.Setup(u => u.Institutions.GetByIdAsync(1)).ReturnsAsync(institution);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).Returns(Task.FromResult(1));

            // Act
            var result = await _institutionService.DeleteInstitutionAsync(1);

            // Assert
            Assert.True(result);
            _mockUnitOfWork.Verify(u => u.Institutions.Remove(institution), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteInstitutionAsync_NonExistingInstitution_ReturnsFalse()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Institutions.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Institution)null);

            // Act
            var result = await _institutionService.DeleteInstitutionAsync(999);

            // Assert
            Assert.False(result);
        }
    }
}
