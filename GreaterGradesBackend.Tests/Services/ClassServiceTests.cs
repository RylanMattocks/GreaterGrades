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
using GreaterGradesBackend.Domain.Enums;
using System;

namespace GreaterGradesBackend.Tests
{
    public class ClassServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ClassService _classService;

        public ClassServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _classService = new ClassService(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllClassesAsync_ReturnsMappedClasses()
        {
            // Arrange
            var classes = new List<Class> { new Class { ClassId = 1, Subject = "Test Class" } };
            _mockUnitOfWork.Setup(u => u.Classes.GetAllClassesWithDetailsAsync()).ReturnsAsync(classes);
            _mockMapper.Setup(m => m.Map<IEnumerable<ClassDto>>(classes))
                       .Returns(new List<ClassDto> { new ClassDto { ClassId = 1, Subject = "Test Class" } });

            // Act
            var result = await _classService.GetAllClassesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(1, result.First().ClassId);
        }

        [Fact]
        public async Task GetClassByIdAsync_ExistingId_ReturnsMappedClass()
        {
            // Arrange
            var classEntity = new Class { ClassId = 1, Subject = "Test Class" };
            _mockUnitOfWork.Setup(u => u.Classes.GetClassWithDetailsAsync(1)).ReturnsAsync(classEntity);
            _mockMapper.Setup(m => m.Map<ClassDto>(classEntity)).Returns(new ClassDto { ClassId = 1, Subject = "Test Class" });

            // Act
            var result = await _classService.GetClassByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ClassId);
        }

        [Fact]
        public async Task GetClassByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Classes.GetClassWithDetailsAsync(It.IsAny<int>())).ReturnsAsync((Class)null);

            // Act
            var result = await _classService.GetClassByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        /*[Fact]
        public async Task CreateClassAsync_ValidClass_ReturnsCreatedClass()
        {
            // Arrange
            var createClassDto = new CreateClassDto { Subject = "New Class", InstitutionId = 1 };
            var classEntity = new Class { ClassId = 1, Subject = "New Class" };

            _mockUnitOfWork.Setup(u => u.Institutions.GetInstitutionWithDetailsAsync(1)).ReturnsAsync(new Institution());
            _mockMapper.Setup(m => m.Map<Class>(createClassDto)).Returns(classEntity);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).Returns(Task.FromResult(1));
            _mockMapper.Setup(m => m.Map<ClassDto>(classEntity)).Returns(new ClassDto { ClassId = 1, Subject = "New Class" });

            // Act
            var result = await _classService.CreateClassAsync(createClassDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ClassId);
        }*/

        [Fact]
        public async Task CreateClassAsync_InstitutionDoesNotExist_ThrowsException()
        {
            // Arrange
            var createClassDto = new CreateClassDto { Subject = "New Class", InstitutionId = 999 };
            _mockUnitOfWork.Setup(u => u.Institutions.GetInstitutionWithDetailsAsync(999)).ReturnsAsync((Institution)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _classService.CreateClassAsync(createClassDto));
        }

        [Fact]
        public async Task UpdateClassAsync_ExistingClass_UpdatesClass()
        {
            // Arrange
            var updateClassDto = new UpdateClassDto { Subject = "Updated Class" };
            var classEntity = new Class { ClassId = 1, Subject = "Old Class" };

            _mockUnitOfWork.Setup(u => u.Classes.GetByIdAsync(1)).ReturnsAsync(classEntity);
            _mockMapper.Setup(m => m.Map(updateClassDto, classEntity));
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).Returns(Task.FromResult(1));

            // Act
            var result = await _classService.UpdateClassAsync(1, updateClassDto);

            // Assert
            Assert.True(result);
            _mockUnitOfWork.Verify(u => u.Classes.Update(classEntity), Times.Once);
        }

        [Fact]
        public async Task UpdateClassAsync_NonExistingClass_ReturnsFalse()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Classes.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Class)null);

            // Act
            var result = await _classService.UpdateClassAsync(999, new UpdateClassDto());

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteClassAsync_ExistingClass_DeletesClass()
        {
            // Arrange
            var classEntity = new Class { ClassId = 1, Subject = "Test Class" };
            _mockUnitOfWork.Setup(u => u.Classes.GetByIdAsync(1)).ReturnsAsync(classEntity);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).Returns(Task.FromResult(1));

            // Act
            var result = await _classService.DeleteClassAsync(1);

            // Assert
            Assert.True(result);
            _mockUnitOfWork.Verify(u => u.Classes.Remove(classEntity), Times.Once);
        }

        [Fact]
        public async Task DeleteClassAsync_NonExistingClass_ReturnsFalse()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Classes.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Class)null);

            // Act
            var result = await _classService.DeleteClassAsync(999);

            // Assert
            Assert.False(result);
        }

        /*[Fact]
        public async Task AddStudentToClassAsync_ExistingClassAndUser_AddsStudent()
        {
            // Arrange
            var classEntity = new Class { ClassId = 1, Students = new List<User>(), Assignments = new List<Assignment> { new Assignment { AssignmentId = 1 } } };
            var user = new User { UserId = 1, Grades = new List<Grade>(), Classes = new List<Class>() };

            _mockUnitOfWork.Setup(u => u.Classes.GetClassWithDetailsAsync(1)).ReturnsAsync(classEntity);
            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(1)).ReturnsAsync(user);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).Returns(Task.FromResult(1));

            // Act
            var result = await _classService.AddStudentToClassAsync(1, 1);

            // Assert
            Assert.True(result);
            Assert.Contains(user, classEntity.Students);
            _mockUnitOfWork.Verify(u => u.Grades.AddAsync(It.IsAny<Grade>()), Times.Once);
        }*/

        /*[Fact]
        public async Task AddStudentToClassAsync_NonExistingClassOrUser_ReturnsFalse()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Classes.GetClassWithDetailsAsync(It.IsAny<int>())).ReturnsAsync((Class)null);

            // Act
            var result = await _classService.AddStudentToClassAsync(1, 1);

            // Assert
            Assert.False(result);
        }*/

        /*[Fact]
        public async Task RemoveStudentFromClassAsync_ExistingClassAndUser_RemovesStudent()
        {
            // Arrange
            var classEntity = new Class { ClassId = 1, Students = new List<User> { new User { UserId = 1 } }, Assignments = new List<Assignment> { new Assignment { AssignmentId = 1 } } };
            var user = new User { UserId = 1, Classes = new List<Class> { classEntity }, Grades = new List<Grade> { new Grade { AssignmentId = 1, UserId = 1 } } };

            _mockUnitOfWork.Setup(u => u.Classes.GetClassWithDetailsAsync(1)).ReturnsAsync(classEntity);
            _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(1)).ReturnsAsync(user);
            _mockUnitOfWork.Setup(u => u.CompleteAsync()).Returns(Task.FromResult(1));

            // Act
            var result = await _classService.RemoveStudentFromClassAsync(1, 1);

            // Assert
            Assert.True(result);
            Assert.DoesNotContain(user, classEntity.Students);
            _mockUnitOfWork.Verify(u => u.Grades.Remove(It.IsAny<Grade>()), Times.Once);
        }*/

        /*[Fact]
        public async Task RemoveStudentFromClassAsync_NonExistingClassOrUser_ReturnsFalse()
        {
            // Arrange
            _mockUnitOfWork.Setup(u => u.Classes.GetClassWithDetailsAsync(It.IsAny<int>())).ReturnsAsync((Class)null);

            // Act
            var result = await _classService.RemoveStudentFromClassAsync(1, 1);

            // Assert
            Assert.False(result);
        }*/


    [Fact]
public async Task AddTeacherToClassAsync_ExistingClassAndUser_AddsTeacher()
{
    // Arrange
    var classEntity = new Class { ClassId = 1, Teachers = new List<User>() };
    var user = new User { UserId = 1, TaughtClasses = new List<Class>() };

    _mockUnitOfWork.Setup(u => u.Classes.GetClassWithDetailsAsync(1)).ReturnsAsync(classEntity);
    _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(1)).ReturnsAsync(user);
    _mockUnitOfWork.Setup(u => u.CompleteAsync()).Returns(Task.FromResult(1));

    // Act
    var result = await _classService.AddTeacherToClassAsync(1, 1);

    // Assert
    Assert.True(result);
    Assert.Contains(user, classEntity.Teachers);
    _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
}

/*[Fact]
public async Task AddTeacherToClassAsync_NonExistingClassOrUser_ReturnsFalse()
{
    // Arrange
    _mockUnitOfWork.Setup(u => u.Classes.GetClassWithDetailsAsync(It.IsAny<int>())).ReturnsAsync((Class)null);

    // Act
    var result = await _classService.AddTeacherToClassAsync(1, 1);

    // Assert
    Assert.False(result);
}*/

/*[Fact]
public async Task RemoveTeacherFromClassAsync_ExistingClassAndUser_RemovesTeacher()
{
    // Arrange
    var classEntity = new Class { ClassId = 1, Teachers = new List<User> { new User { UserId = 1 } } };
    var user = new User { UserId = 1, TaughtClasses = new List<Class> { classEntity } };

    _mockUnitOfWork.Setup(u => u.Classes.GetClassWithDetailsAsync(1)).ReturnsAsync(classEntity);
    _mockUnitOfWork.Setup(u => u.Users.GetByIdAsync(1)).ReturnsAsync(user);
    _mockUnitOfWork.Setup(u => u.CompleteAsync()).Returns(Task.FromResult(1));

    // Act
    var result = await _classService.RemoveTeacherFromClassAsync(1, 1);

    // Assert
    Assert.True(result);
    Assert.DoesNotContain(user, classEntity.Teachers);
    _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
}*/

/*[Fact]
public async Task RemoveTeacherFromClassAsync_NonExistingClassOrUser_ReturnsFalse()
{
    // Arrange
    _mockUnitOfWork.Setup(u => u.Classes.GetClassWithDetailsAsync(It.IsAny<int>())).ReturnsAsync((Class)null);

    // Act
    var result = await _classService.RemoveTeacherFromClassAsync(1, 1);

    // Assert
    Assert.False(result);
}*/

[Fact]
public async Task GetClassesByInstitutionIdAsync_ValidInstitutionId_ReturnsMappedClasses()
{
    // Arrange
    var classes = new List<Class> { new Class { ClassId = 1, InstitutionId = 1, Subject = "Test Class" } };
    _mockUnitOfWork.Setup(u => u.Classes.GetClassesByInstitutionIdAsync(1)).ReturnsAsync(classes);
    _mockMapper.Setup(m => m.Map<IEnumerable<ClassDto>>(classes))
               .Returns(new List<ClassDto> { new ClassDto { ClassId = 1, Subject = "Test Class" } });

    // Act
    var result = await _classService.GetClassesByInstitutionIdAsync(1);

    // Assert
    Assert.NotNull(result);
    Assert.Single(result);
    Assert.Equal(1, result.First().ClassId);
}

    }
}
