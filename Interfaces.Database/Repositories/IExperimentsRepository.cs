using Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Interfaces.Database.Repositories
{
    public interface IExperimentsRepository : IRepository<Experiment, int>
    {
        Task<IReadOnlyCollection<Experiment>> GetByIdsAsync(IEnumerable<int> ids,
            CancellationToken cancellationToken = default);
    }
}
