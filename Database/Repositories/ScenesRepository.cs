using Database.Abstractions;
using Entities.Entities;
using Interfaces.Database.Repositories;

namespace Database.Repositories
{
    internal class ScenesRepository : RepositoryBase<Scene, int>, ISceneRepository
    {
        public ScenesRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
