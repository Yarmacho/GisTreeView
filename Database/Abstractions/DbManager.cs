using Interfaces.Database.Abstractions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading.Tasks;

namespace Database.Abstractions
{
    internal class DbManager : IDbManager
    {
        private readonly DatabaseFacade _databaseFacade;

        public DbManager(AppDbContext context)
        {
            _databaseFacade = context.Database;
        }

        public async Task CreateAsync()
        {
            await _databaseFacade.EnsureCreatedAsync();
        }

        public async Task DeleteAsync()
        {
            await _databaseFacade.EnsureDeletedAsync();
        }

        public async Task ReCreateAsync()
        {
            await DeleteAsync();
            await CreateAsync();
        }
    }
}
