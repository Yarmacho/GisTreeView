using Interfaces.Database.Repositories;

namespace Interfaces.Database.Abstractions
{
    public interface IRepositoriesProvider
    {
        T Get<T>() where T : IRepository;
    }
}
