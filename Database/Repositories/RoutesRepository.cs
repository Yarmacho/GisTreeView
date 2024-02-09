using Database.Abstractions;
using Entities.Entities;
using Interfaces.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Database.Repositories
{
    internal class RoutesRepository : RepositoryBase<Route, int>, IRoutesRepository
    {
        public RoutesRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<List<Route>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Include(r => r.Points)
                .ToListAsync(cancellationToken);
        }

        public override async Task<Route> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Include(r => r.Points)
                .SingleOrDefaultAsync(e => e.Id.Equals(id), cancellationToken);
        }

        public async Task<Dictionary<int, Route>> GetRoutesDictionary(CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Include(r => r.Points)
                .ToDictionaryAsync(k => k.Id, cancellationToken);
        }
    }
}
