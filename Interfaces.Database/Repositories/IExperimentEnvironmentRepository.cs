using Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Database.Repositories
{
    public interface IExperimentEnvironmentRepository : IRepository<ExperimentEnvironment, int>
    {
        Task<ExperimentEnvironment> GetAsync(int experimentId);
    }
}
