using GreaterGradesBackend.Domain.Entities;

namespace GreaterGradesBackend.Domain.Interfaces
{
    public interface IGradeRepository : IRepository<Grade>
    {
        Task<Grade> GetGradeWithDetailsAsync(int GradeId);
        Task<IEnumerable<Grade>> GetGradesByUserAsync(int userId);
        Task<Grade> GetGradeByUserAndAssignmentAsync(int userId, int assignmentId);

        Task<IEnumerable<Grade>> GetGradesByInstitutionIdAsync(int institutionId);

    }
}
