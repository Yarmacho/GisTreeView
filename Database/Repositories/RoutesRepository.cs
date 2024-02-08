using Database.Abstractions;
using Entities.Entities;
using Interfaces.Database.Repositories;

namespace Database.Repositories
{
    internal class RoutesRepository : RepositoryBase<Route, int>, IRoutesRepository
    {
        public RoutesRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
