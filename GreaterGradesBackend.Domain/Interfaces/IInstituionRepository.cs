using GreaterGradesBackend.Domain.Entities;
using System.Threading.Tasks;

namespace GreaterGradesBackend.Domain.Interfaces
{
    public interface IInstitutionRepository : IRepository<Institution>
    {
        Task<Institution> GetInstitutionWithDetailsAsync(int institutionId);
    }
}
