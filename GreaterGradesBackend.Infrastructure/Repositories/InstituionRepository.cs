using GreaterGradesBackend.Domain.Entities;
using GreaterGradesBackend.Domain.Interfaces;
using GreaterGradesBackend.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GreaterGradesBackend.Infrastructure.Repositories
{
    public class InstitutionRepository : Repository<Institution>, IInstitutionRepository
    {
        public InstitutionRepository(GreaterGradesBackendDbContext context) : base(context)
        {
        }

        public async Task<Institution> GetInstitutionWithDetailsAsync(int institutionId)
        {
            return await _context.Institutions
                .Include(i => i.Classes)
                .Include(i => i.Users)
                .FirstOrDefaultAsync(i => i.InstitutionId == institutionId);
        }
    }
}
