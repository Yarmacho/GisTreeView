using System.Threading.Tasks;

namespace Interfaces.Database.Abstractions
{
    public interface IDbManager
    {
        Task CreateAsync();

        Task DeleteAsync();

        Task ReCreateAsync();
    }
}
