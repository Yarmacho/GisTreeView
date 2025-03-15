using Database.Abstractions;
using Entities.Entities;
using Interfaces.Database.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace Database.Repositories
{
    internal class ExperimentEnvironmentRepository : RepositoryBase<ExperimentEnvironment, int>, IExperimentEnvironmentRepository
    {
        private readonly AppDbContext dbContext;

        public ExperimentEnvironmentRepository(AppDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<ExperimentEnvironment> GetAsync(int experimentId)
        {
            return dbContext.Set<ExperimentEnvironment>()
                .Where(e => e.ExperimentId == experimentId)
                .FirstOrDefault();
        }
    }
}
