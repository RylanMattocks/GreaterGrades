using System;
using System.Threading.Tasks;

namespace GreaterGradesBackend.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Repositories
        IClassRepository Classes { get; }
        IAssignmentRepository Assignments { get; }
        IGradeRepository Grades { get; }
        IUserRepository Users { get; }
        IInstitutionRepository Institutions { get; }

        // Commit changes
        Task<int> CompleteAsync();
    }

}
