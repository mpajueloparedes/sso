using System.Linq.Expressions;

namespace MaproSSO.Domain.Common
{
    //public interface IRepository<T> where T : BaseEntity, IAggregateRoot
    //{
    //    Task<T> GetByIdAsync(Guid id);
    //    Task<IEnumerable<T>> GetAllAsync();
    //    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    //    Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
    //    Task AddAsync(T entity);
    //    void Update(T entity);
    //    void Remove(T entity);
    //    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    //    Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);
    //}
    public interface IRepository<T> where T : class, IAggregateRoot
    {
        Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    }

}