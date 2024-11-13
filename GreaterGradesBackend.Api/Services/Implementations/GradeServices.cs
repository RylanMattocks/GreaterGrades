using AutoMapper;
using GreaterGradesBackend.Api.Controllers;
using GreaterGradesBackend.Api.Models;
using GreaterGradesBackend.Domain.Entities;
using GreaterGradesBackend.Domain.Interfaces;
using GreaterGradesBackend.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GreaterGradesBackend.Services.Implementations
{
    public class GradeService : IGradeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GradeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GradeDto>> GetAllGradesAsync()
        {
            var grades = await _unitOfWork.Grades.GetAllAsync();
            return _mapper.Map<IEnumerable<GradeDto>>(grades);
        }

        public async Task<IEnumerable<GradeDto>> GetAllGradesByUserId(int userId)
        {
            var grades = _unitOfWork.Grades.GetGradesByUserAsync(userId);
            return _mapper.Map<IEnumerable<GradeDto>>(grades);
        }

        public async Task<IEnumerable<GradeDto>> GetGradesByInstitutionIdAsync(int institutionId)
        {
            var grades = _unitOfWork.Grades.GetGradesByInstitutionIdAsync(institutionId);
            return _mapper.Map<IEnumerable<GradeDto>>(grades);
        }

        public async Task<GradeDto> GetGradeByIdAsync(int gradeId)
        {
            var grade = await _unitOfWork.Grades.GetByIdAsync(gradeId);
            if (grade == null)
            {
                return null;
            }
            return _mapper.Map<GradeDto>(grade);
        }

        /*public async Task<GradeDto> CreateGradeAsync(CreateGradeDto createGradeDto)
        {
            var grade = _mapper.Map<Grade>(createGradeDto);
            await _unitOfWork.Grades.AddAsync(grade);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<GradeDto>(grade);
        }*/

        public async Task<bool> UpdateGradeAsync(int gradeId, UpdateGradeDto updateGradeDto)
        {
            var grade = await _unitOfWork.Grades.GetByIdAsync(gradeId);
            if (grade == null)
            {
                return false;
            }

            _mapper.Map(updateGradeDto, grade);
            _unitOfWork.Grades.Update(grade);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> DeleteGradeAsync(int gradeId)
        {
            var grade = await _unitOfWork.Grades.GetByIdAsync(gradeId);
            if (grade == null)
            {
                return false;
            }

            _unitOfWork.Grades.Remove(grade);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
