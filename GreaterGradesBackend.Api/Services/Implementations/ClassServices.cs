using AutoMapper;
using GreaterGradesBackend.Api.Models;
using GreaterGradesBackend.Domain.Entities;
using GreaterGradesBackend.Domain.Interfaces;
using GreaterGradesBackend.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using GreaterGradesBackend.Domain.Enums;

namespace GreaterGradesBackend.Services.Implementations
{
    public class ClassService : IClassService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ClassService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ClassDto>> GetAllClassesAsync()
        {
            var classes = await _unitOfWork.Classes.GetAllClassesWithDetailsAsync();
            return _mapper.Map<IEnumerable<ClassDto>>(classes);
        }

        public async Task<ClassDto> GetClassByIdAsync(int classId)
        {
            var classEntity = await _unitOfWork.Classes.GetClassWithDetailsAsync(classId);
            if (classEntity == null)
            {
                return null;
            }
            return _mapper.Map<ClassDto>(classEntity);
        }

        public async Task<ClassDto> CreateClassAsync(CreateClassDto createClassDto)
        {
            var classEntity = _mapper.Map<Class>(createClassDto);
            var existingInstitution = await _unitOfWork.Institutions.GetInstitutionWithDetailsAsync(createClassDto.InstitutionId);
            if (existingInstitution == null)
            {
                throw new Exception("Institution does not exist");
            }
            await _unitOfWork.Classes.AddAsync(classEntity);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<ClassDto>(classEntity);
        }

        public async Task<bool> UpdateClassAsync(int classId, UpdateClassDto updateClassDto)
        {
            var classEntity = await _unitOfWork.Classes.GetByIdAsync(classId);
            if (classEntity == null)
            {
                return false;
            }

            _mapper.Map(updateClassDto, classEntity);
            _unitOfWork.Classes.Update(classEntity);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> DeleteClassAsync(int classId)
        {
            var classEntity = await _unitOfWork.Classes.GetByIdAsync(classId);
            if (classEntity == null)
            {
                return false;
            }

            _unitOfWork.Classes.Remove(classEntity);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> AddStudentToClassAsync(int classId, int userId)
        {
            var classEntity = await _unitOfWork.Classes.GetClassWithDetailsAsync(classId);
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            
            if (classEntity == null || user == null)
            {
                return false;
            }

            if (!classEntity.Students.Contains(user))
            {
                classEntity.Students.Add(user);
                user.Classes.Add(classEntity);
                foreach (Assignment assignment in classEntity.Assignments)
                {
                    var grade = new Grade
                    {
                        User = user,
                        AssignmentId = assignment.AssignmentId,
                        Score = 0,
                        GradingStatus = GradeStatus.NotGraded
                    };
                    user.Grades.Add(grade);

                    // Add the new grade to the UnitOfWork
                    await _unitOfWork.Grades.AddAsync(grade);
                }
                await _unitOfWork.CompleteAsync();
            }

            return true;
        }


        public async Task<IEnumerable<ClassDto>> GetClassesByInstitutionIdAsync(int institutionId)
        {
            var classes = await _unitOfWork.Classes.GetClassesByInstitutionIdAsync(institutionId);
            return _mapper.Map<IEnumerable<ClassDto>>(classes);
        }

        public async Task<bool> AddTeacherToClassAsync(int classId, int userId)
        {
            var classEntity = await _unitOfWork.Classes.GetClassWithDetailsAsync(classId);
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            
            if (classEntity == null || user == null)
            {
                return false;
            }

            if (!classEntity.Teachers.Contains(user))
            {
                classEntity.Teachers.Add(user);
                user.TaughtClasses.Add(classEntity);
                await _unitOfWork.CompleteAsync();
            }

            return true;
        }

        public async Task<bool> RemoveTeacherFromClassAsync(int classId, int userId)
        {
            var classEntity = await _unitOfWork.Classes.GetClassWithDetailsAsync(classId);
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            
            if (classEntity == null || user == null)
            {
                return false;
            }

            if (classEntity.Teachers.Contains(user))
            {
                classEntity.Teachers.Remove(user);
                user.TaughtClasses.Remove(classEntity);
                await _unitOfWork.CompleteAsync();
            }

            return true;
        }

        public async Task<bool> RemoveStudentFromClassAsync(int classId, int userId)
        {
            var classEntity = await _unitOfWork.Classes.GetClassWithDetailsAsync(classId);
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (classEntity == null || user == null)
            {
                return false;
            }

            if (classEntity.Students.Contains(user))
            {
                classEntity.Students.Remove(user);
                user.Classes.Remove(classEntity);

                foreach (var assignment in classEntity.Assignments)
                {
                    var grade = await _unitOfWork.Grades.GetGradeByUserAndAssignmentAsync(userId, assignment.AssignmentId);
                    if (grade != null)
                    {
                        _unitOfWork.Grades.Remove(grade);
                        user.Grades.Remove(grade);
                    }
                }

                 await _unitOfWork.CompleteAsync();
            }

            return true;
        }

    }
}
