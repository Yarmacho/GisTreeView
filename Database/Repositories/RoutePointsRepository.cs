using Database.Abstractions;
using Entities.Entities;
using Interfaces.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Database.Repositories
{
    internal class RoutePointsRepository : RepositoryBase<RoutePoint, int>, IRoutePointsRepository
    {
        private readonly AppDbContext _dbContext;

        public RoutePointsRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyCollection<RoutePoint>> GetAllAsync(int routeId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<RoutePoint>()
                .AsNoTracking()
                .Where(p => p.RouteId == routeId)
                .ToListAsync();
        }
    }
}
