using System.Threading;
using System.Threading.Tasks;

namespace Interfaces.Database
{
    public interface IUnitOfWork
    {
        Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
