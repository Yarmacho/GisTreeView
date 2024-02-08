using Database.Abstractions;
using Entities.Entities;
using Interfaces.Database.Repositories;

namespace Database.Repositories
{
    internal class RoutePointsRepository : RepositoryBase<RoutePoint, int>, IRoutePointsRepository
    {
        public RoutePointsRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
