using Database.Abstractions;
using Entities.Entities;
using Interfaces.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Database.Repositories
{
    internal class ProfilesRepository : RepositoryBase<Profil, int>, IProfilesRepository
    {
        private readonly AppDbContext _dbContext;

        public ProfilesRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyCollection<Profil>> GetSceneProfiles(int sceneId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<Profil>()
                .AsNoTracking()
                .Where(p => p.SceneId == sceneId)
                .OrderBy(p => p.Depth)
                .ToListAsync();
        }
    }
}
