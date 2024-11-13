using System;
using System.Threading.Tasks;
using GreaterGradesBackend.Domain.Interfaces;
using GreaterGradesBackend.Infrastructure.Repositories;
using GreaterGradesBackend.Infrastructure;

namespace GreaterGradesBackend.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly GreaterGradesBackendDbContext _context;

        // Repositories
        //public IStudentRepository Students { get; }
        public IClassRepository Classes { get; }
        public IAssignmentRepository Assignments { get; }
        public IGradeRepository Grades { get; }
        public IUserRepository Users { get; }
        public IInstitutionRepository Institutions { get; }

        public UnitOfWork(GreaterGradesBackendDbContext context)
        {
            _context = context;

            // Initialize repositories
            //Students = new StudentRepository(_context);
            Classes = new ClassRepository(_context);
            Assignments = new AssignmentRepository(_context);
            Grades = new GradeRepository(_context);
            Users = new UserRepository(_context);
            Institutions = new InstitutionRepository(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        #region IDisposable Support
        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose the context
                    _context.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
