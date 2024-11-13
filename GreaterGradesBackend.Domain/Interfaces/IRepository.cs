using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GreaterGradesBackend.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        // Get all entities
        Task<IEnumerable<T>> GetAllAsync();

        // Get entity by ID
        Task<T> GetByIdAsync(int id);

        // Find entities based on a predicate
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        // Add a new entity
        Task AddAsync(T entity);

        // Remove an entity
        void Remove(T entity);

        // Update an entity
        void Update(T entity);
    }
}
