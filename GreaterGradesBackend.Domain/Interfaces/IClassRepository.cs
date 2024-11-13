using GreaterGradesBackend.Domain.Entities;
using System.Threading.Tasks;

namespace GreaterGradesBackend.Domain.Interfaces
{
    public interface IClassRepository : IRepository<Class>
    {
        Task<Class> GetClassWithDetailsAsync(int classId);
        Task<List<Class>> GetAllClassesWithDetailsAsync();
        Task<IEnumerable<Class>> GetClassesByInstitutionIdAsync(int institutionId);
    }
}
