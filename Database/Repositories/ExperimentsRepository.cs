using Database.Abstractions;
using Entities;
using Interfaces.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Database.Repositories
{
    internal class ExperimentsRepository : RepositoryBase<Experiment, int>, IExperimentsRepository
    {
        public ExperimentsRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<IReadOnlyCollection<Experiment>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            var query = DbSet.AsNoTracking();
            if (ids != null && ids.Any())
            {
                query = query.Where(e => ids.Contains(e.Id));
            }
            return await query.ToListAsync(cancellationToken);
        }
    }
}
