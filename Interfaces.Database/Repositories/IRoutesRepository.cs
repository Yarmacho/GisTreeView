using Entities.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Interfaces.Database.Repositories
{
    public interface IRoutesRepository : IRepository<Route, int>
    {
        Task<Dictionary<int, Route>> GetRoutesDictionary(CancellationToken cancellationToken = default);
    }
}
