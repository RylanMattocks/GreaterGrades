using GreaterGradesBackend.Api.Models;
using GreaterGradesBackend.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GreaterGradesBackend.Services.Interfaces
{
    public interface IInstitutionService
    {
        Task<InstitutionDto> GetInstitutionByIdAsync(int institutionId);
        Task<IEnumerable<InstitutionDto>> GetAllInstitutionsAsync();
        Task<InstitutionDto> CreateInstitutionAsync(CreateInstitutionDto createInstitutionDto);
        Task<bool> UpdateInstitutionAsync(int institutionId, UpdateInstitutionDto updateInstitutionDto);
        Task<bool> DeleteInstitutionAsync(int institutionId);
    }
}
