using Entities;
using Interfaces.Database.Abstractions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Interfaces.Database.Repositories
{
    public interface IRepository { }

    public interface IRepository<TEntity, TId> : IWriteOnlyRepository<TEntity>
        where TEntity : EntityBase<TId>
    {
        Task<TEntity> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

        Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
