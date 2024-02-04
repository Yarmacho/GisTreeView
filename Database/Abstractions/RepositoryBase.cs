using Entities;
using Interfaces.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Database.Abstractions
{
    internal abstract class RepositoryBase<TEntity, TId> : IRepository<TEntity, TId> 
        where TEntity : EntityBase<TId>
    {
        protected readonly DbSet<TEntity> DbSet;
        private readonly AppDbContext _dbContext;

        protected RepositoryBase(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            DbSet = dbContext.Set<TEntity>();
        }

        protected async Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default)
        {
            return await DbSet.AnyAsync(e => e.Id.Equals(id), cancellationToken);
        }

        protected async Task<bool> ExistsAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            return await DbSet.AnyAsync(e => e.Id.Equals(entity.Id), cancellationToken);
        }

        public async Task<TEntity> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
        {
            return await DbSet.SingleOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
        }

        public async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await DbSet.ToListAsync(cancellationToken);
        }

        public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (await ExistsAsync(entity, cancellationToken))
            {
                return null;
            }

            var entry = await DbSet.AddAsync(entity, cancellationToken);
            return entry?.Entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (!await ExistsAsync(entity, cancellationToken))
            {
                return null;
            }

            var entry = DbSet.Update(entity);
            return entry?.Entity;
        }
        
        public async Task<TEntity> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (!await ExistsAsync(entity, cancellationToken))
            {
                return null;
            }

            var entry = DbSet.Remove(entity);
            return entry?.Entity;
        }

        public async Task<bool> SaveChanges(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}