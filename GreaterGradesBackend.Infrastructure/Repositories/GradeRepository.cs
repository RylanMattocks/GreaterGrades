using GreaterGradesBackend.Domain.Entities;
using GreaterGradesBackend.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace GreaterGradesBackend.Infrastructure.Repositories
{
    public class GradeRepository : Repository<Grade>, IGradeRepository
    {
        public GradeRepository(GreaterGradesBackendDbContext context) : base(context)
        {
        }
        public async Task<Grade> GetGradeWithDetailsAsync(int gradeId)
        {
            return await _context.Grades
                .Include(g => g.User)      // Include related student
                .Include(g => g.Assignment)   // Include related assignment
                .FirstOrDefaultAsync(g => g.GradeId == gradeId);
        }
        public async Task<IEnumerable<Grade>> GetGradesByUserAsync(int userId)
        {
            return await _context.Grades
                .Where(g => g.UserId == userId)
                .ToListAsync();
        }
        public async Task<Grade> GetGradeByUserAndAssignmentAsync(int userId, int assignmentId)
        {
            return await _context.Grades
                .FirstOrDefaultAsync(g => g.UserId == userId && g.AssignmentId == assignmentId);
        }

        public async Task<IEnumerable<Grade>> GetGradesByInstitutionIdAsync(int institutionId)
        {
            return await _context.Grades
                .Include(g => g.Assignment)
                .ThenInclude(a => a.Class)
                .Where(g => g.Assignment.Class.InstitutionId == institutionId)
                .ToListAsync();
        }

    }
}
