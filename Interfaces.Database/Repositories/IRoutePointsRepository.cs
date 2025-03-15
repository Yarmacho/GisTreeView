using Entities.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Interfaces.Database.Repositories
{
    public interface IRoutePointsRepository : IRepository<RoutePoint, int>
    {
        Task<IReadOnlyCollection<RoutePoint>> GetAllAsync(int routeId, CancellationToken cancellationToken = default);
    }
}
