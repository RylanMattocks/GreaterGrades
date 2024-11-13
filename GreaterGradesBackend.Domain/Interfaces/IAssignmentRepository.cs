using GreaterGradesBackend.Domain.Entities;
using System.Threading.Tasks;

namespace GreaterGradesBackend.Domain.Interfaces
{
    public interface IAssignmentRepository : IRepository<Assignment>
    {
        Task<Assignment> GetAssignmentWithDetailsAsync(int assignmentId);
    }
}
