using Entities.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Interfaces.Database.Repositories
{
    public interface IProfilesRepository : IRepository<Profil, int>
    {
        Task<IReadOnlyCollection<Profil>> GetSceneProfiles(int sceneId, CancellationToken cancellationToken = default);
    }
}
