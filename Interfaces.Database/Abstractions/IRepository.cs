using Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Interfaces.Database.Repositories
{
    public interface IRepository { }

    public interface IRepository<TEntity, TId> : IRepository
        where TEntity : EntityBase<TId>
    {
        Task<TEntity> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

        Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task<TEntity> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task<bool> SaveChanges(CancellationToken cancellationToken = default);
    }
}
