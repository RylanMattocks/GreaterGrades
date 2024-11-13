using GreaterGradesBackend.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GreaterGradesBackend.Services.Interfaces
{
    public interface IGradeService
    {
        Task<IEnumerable<GradeDto>> GetAllGradesAsync();
        Task<GradeDto> GetGradeByIdAsync(int gradeId);
        //Task<GradeDto> CreateGradeAsync(CreateGradeDto createGradeDto);
        Task<bool> UpdateGradeAsync(int gradeId, UpdateGradeDto updateGradeDto);
        Task<bool> DeleteGradeAsync(int gradeId);
        Task<IEnumerable<GradeDto>> GetGradesByInstitutionIdAsync(int institutionId);
    }
}
