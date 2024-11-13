using GreaterGradesBackend.Domain.Entities;
using GreaterGradesBackend.Domain.Interfaces;
using GreaterGradesBackend.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GreaterGradesBackend.Infrastructure.Repositories
{
    public class AssignmentRepository : Repository<Assignment>, IAssignmentRepository
    {
        public AssignmentRepository(GreaterGradesBackendDbContext context) : base(context)
        {
        }

        public async Task<Assignment> GetAssignmentWithDetailsAsync(int assignmentId)
        {
            return await _context.Assignments
                .Include(a => a.Class)      // Include related class
                .Include(a => a.Grades)     // Include related grades
                .FirstOrDefaultAsync(a => a.AssignmentId == assignmentId);
        }
    }
}
