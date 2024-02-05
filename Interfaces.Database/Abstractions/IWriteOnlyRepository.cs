using Entities;
using Interfaces.Database.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Interfaces.Database.Abstractions
{
    public interface IWriteOnlyRepository<TEntity> : IRepository
        where TEntity : EntityBase
    {
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task<TEntity> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task<bool> SaveChanges(CancellationToken cancellationToken = default);
    }
}
