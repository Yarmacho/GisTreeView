using Database.Abstractions;
using Entities.Entities;
using Interfaces.Database.Repositories;

namespace Database.Repositories
{
    internal class ShipsRepository : RepositoryBase<Ship, int>, IShipsRepository
    {
        public ShipsRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
