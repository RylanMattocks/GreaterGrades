using Xunit;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using GreaterGradesBackend.Services.Implementations;
using GreaterGradesBackend.Domain.Interfaces;
using GreaterGradesBackend.Api.Models;
using GreaterGradesBackend.Domain.Entities;
using GreaterGradesBackend.Domain.Enums;

namespace GreaterGradesBackend.Tests
{
    public class AssignmentServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly AssignmentService _assignmentService;

        public AssignmentServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _assignmentService = new AssignmentService(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllAssignmentsAsync_ReturnsMappedAssignments()
        {
            // Arrange
            var assignments = new List<Assignment> { new Assignment { AssignmentId = 1, Name = "Assignment 1" } };
            _mockUnitOfWork.Setup(u => u.Assignments.GetAllAsync()).ReturnsAsync(assignments);
            _mockMapper.Setup(m => m.Map<IEnumerable<AssignmentDto>>(assignments))
                       .Returns(new List<AssignmentDto> { new AssignmentDto { AssignmentId = 1, Name = "Assignment 1" } });

            // Act
            var result = await _assignmentService.GetAllAssignmentsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(1, result.First().AssignmentId);
        }

        [Fact]
        public async Task GetAssignmentByIdAsync_ExistingId_ReturnsMappedAssignment()
        {
            // Arrange
            var assignment = new Assignment { AssignmentId = 1, Name = "Assignment 1" };
            _mockUnitOfWork.Setup(u => u.Assignments.GetAssignmentWithDetailsAsync(1)).ReturnsAsync(assignment);
            _mockMapper.Setup(m => m.Map<AssignmentDto>(assignment)).Returns(new AssignmentDto { AssignmentId = 1, Name = "Assignment 1" });

            // Act
            var result = await _assignmentService.GetAssignmentByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.AssignmentId);
        }

        [Fact]
        public async Task GetAssignmentByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Assignments.GetAssignmentWithDetailsAsync(It.IsAny<int>())).ReturnsAsync((Assignment)null);

            // Act
            var result = await _assignmentService.GetAssignmentByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAssignmentAsync_ExistingId_UpdatesAssignment()
        {
            // Arrange
            var updateAssignmentDto = new UpdateAssignmentDto { Name = "Updated Assignment" };
            var assignment = new Assignment { AssignmentId = 1, Name = "Old Assignment" };

            _mockUnitOfWork.Setup(u => u.Assignments.GetByIdAsync(1)).ReturnsAsync(assignment);
            _mockMapper.Setup(m => m.Map(updateAssignmentDto, assignment));

            // Act
            var result = await _assignmentService.UpdateAssignmentAsync(1, updateAssignmentDto);

            // Assert
            Assert.True(result);
            _mockUnitOfWork.Verify(u => u.Assignments.Update(assignment), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAssignmentAsync_NonExistingId_ReturnsFalse()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Assignments.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Assignment)null);

            // Act
            var result = await _assignmentService.UpdateAssignmentAsync(999, new UpdateAssignmentDto());

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAssignmentAsync_ExistingId_DeletesAssignmentAndGrades()
        {
            // Arrange
            var assignment = new Assignment { AssignmentId = 1, ClassId = 1, Grades = new List<Grade>() };
            var classEntity = new Class { ClassId = 1, Students = new List<User> { new User { UserId = 1 }, new User { UserId = 2 } } };

            _mockUnitOfWork.Setup(u => u.Assignments.GetByIdAsync(1)).ReturnsAsync(assignment);
            _mockUnitOfWork.Setup(u => u.Classes.GetClassWithDetailsAsync(1)).ReturnsAsync(classEntity);
            _mockUnitOfWork.Setup(u => u.Grades.GetGradeByUserAndAssignmentAsync(It.IsAny<int>(), 1)).ReturnsAsync(new Grade());

            // Act
            var result = await _assignmentService.DeleteAssignmentAsync(1);

            // Assert
            Assert.True(result);
            _mockUnitOfWork.Verify(u => u.Grades.Remove(It.IsAny<Grade>()), Times.Exactly(classEntity.Students.Count));
            _mockUnitOfWork.Verify(u => u.Assignments.Remove(assignment), Times.Once);
            _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAssignmentAsync_NonExistingId_ReturnsFalse()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Assignments.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Assignment)null);

            // Act
            var result = await _assignmentService.DeleteAssignmentAsync(999);

            // Assert
            Assert.False(result);
        }
    }
}
