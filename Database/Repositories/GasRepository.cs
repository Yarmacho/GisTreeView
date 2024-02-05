using Database.Abstractions;
using Entities.Entities;
using Interfaces.Database.Repositories;

namespace Database.Repositories
{
    internal class GasRepository : RepositoryBase<Gas, int>, IGasRepository
    {
        public GasRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
