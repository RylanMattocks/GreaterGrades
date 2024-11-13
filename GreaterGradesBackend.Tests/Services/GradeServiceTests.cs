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
    public class GradeServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GradeService _gradeService;

        public GradeServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _gradeService = new GradeService(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllGradesAsync_ReturnsMappedGrades()
        {
            // Arrange
            var grades = new List<Grade> { new Grade { GradeId = 1, Score = 95 } };
            _mockUnitOfWork.Setup(u => u.Grades.GetAllAsync()).ReturnsAsync(grades);
            _mockMapper.Setup(m => m.Map<IEnumerable<GradeDto>>(grades))
                       .Returns(new List<GradeDto> { new GradeDto { GradeId = 1, Score = 95 } });

            // Act
            var result = await _gradeService.GetAllGradesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(1, result.First().GradeId);
        }

        /*[Fact]
        public async Task GetAllGradesByUserId_ReturnsMappedGradesForUser()
        {
            // Arrange
            var grades = new List<Grade> { new Grade { GradeId = 1, Score = 90, UserId = 1 } };
            _mockUnitOfWork.Setup(u => u.Grades.GetGradesByUserAsync(1)).ReturnsAsync(grades);
            _mockMapper.Setup(m => m.Map<IEnumerable<GradeDto>>(grades))
                       .Returns(new List<GradeDto> { new GradeDto { GradeId = 1, Score = 90 } });

            // Act
            var result = await _gradeService.GetAllGradesByUserId(1);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(1, result.First().GradeId);
        }*/

        /*[Fact]
        public async Task GetGradesByInstitutionIdAsync_ReturnsMappedGradesForInstitution()
        {
            // Arrange
            var grades = new List<Grade> { new Grade { GradeId = 1, Score = 85, InstitutionId = 1 } };
            _mockUnitOfWork.Setup(u => u.Grades.GetGradesByInstitutionIdAsync(1)).ReturnsAsync(grades);
            _mockMapper.Setup(m => m.Map<IEnumerable<GradeDto>>(grades))
                       .Returns(new List<GradeDto> { new GradeDto { GradeId = 1, Score = 85 } });

            // Act
            var result = await _gradeService.GetGradesByInstitutionIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(1, result.First().GradeId);
        }*/

        [Fact]
        public async Task GetGradeByIdAsync_ExistingId_ReturnsMappedGrade()
        {
            // Arrange
            var grade = new Grade { GradeId = 1, Score = 95 };
            _mockUnitOfWork.Setup(u => u.Grades.GetByIdAsync(1)).ReturnsAsync(grade);
            _mockMapper.Setup(m => m.Map<GradeDto>(grade)).Returns(new GradeDto { GradeId = 1, Score = 95 });

            // Act
            var result = await _gradeService.GetGradeByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.GradeId);
        }

        [Fact]
        public async Task GetGradeByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Grades.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Grade)null);

            // Act
            var result = await _gradeService.GetGradeByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        /*[Fact]
        public async Task CreateGradeAsync_ReturnsCreatedGrade()
        {
            // Arrange
            var createGradeDto = new CreateGradeDto { Score = 100, UserId = 1};
            var grade = new Grade { GradeId = 1, Score = 100, UserId = 1, AssignmentId = 1 };

            _mockMapper.Setup(m => m.Map<Grade>(createGradeDto)).Returns(grade);
            _mockUnitOfWork.Setup(u => u.Grades.AddAsync(grade)).Returns(Task.CompletedTask);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).Returns(Task.CompletedTask);
            _mockMapper.Setup(m => m.Map<GradeDto>(grade)).Returns(new GradeDto { GradeId = 1, Score = 100 });

            var result = 1;

            Assert.NotNull(result);
            Assert.Equal(1, result);
        }*/

        [Fact]
        public async Task UpdateGradeAsync_ExistingId_UpdatesGrade()
        {
            // Arrange
            var updateGradeDto = new UpdateGradeDto { Score = 85 };
            var grade = new Grade { GradeId = 1, Score = 80 };

            _mockUnitOfWork.Setup(u => u.Grades.GetByIdAsync(1)).ReturnsAsync(grade);
            _mockMapper.Setup(m => m.Map(updateGradeDto, grade));

            // Act
            var result = await _gradeService.UpdateGradeAsync(1, updateGradeDto);

            // Assert
            Assert.True(result);
            _mockUnitOfWork.Verify(u => u.Grades.Update(grade), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateGradeAsync_NonExistingId_ReturnsFalse()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Grades.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Grade)null);

            // Act
            var result = await _gradeService.UpdateGradeAsync(999, new UpdateGradeDto());

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteGradeAsync_ExistingId_DeletesGrade()
        {
            // Arrange
            var grade = new Grade { GradeId = 1, Score = 90 };
            _mockUnitOfWork.Setup(u => u.Grades.GetByIdAsync(1)).ReturnsAsync(grade);

            // Act
            var result = await _gradeService.DeleteGradeAsync(1);

            // Assert
            Assert.True(result);
            _mockUnitOfWork.Verify(u => u.Grades.Remove(grade), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteGradeAsync_NonExistingId_ReturnsFalse()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Grades.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Grade)null);

            // Act
            var result = await _gradeService.DeleteGradeAsync(999);

            // Assert
            Assert.False(result);
        }
    }
}
