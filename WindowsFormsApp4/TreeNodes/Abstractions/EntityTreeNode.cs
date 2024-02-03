using Entities;
using Interfaces.Database.Abstractions;
using Interfaces.Database.Repositories;
using System.Threading.Tasks;

namespace WindowsFormsApp4.TreeNodes.Abstractions
{
    internal abstract class EntityTreeNode<TEntity, TId> : MapTreeNodeBase
        where TEntity : EntityBase<TId>
    {
        protected readonly TEntity Entity;
        private readonly IRepositoriesProvider _repositoriesProvider;

        protected EntityTreeNode(TEntity entity, IRepositoriesProvider repositoriesProvider)
        {
            Entity = entity;
            _repositoriesProvider = repositoriesProvider;
        }

        public override async ValueTask Delete()
        {
            var repository = _repositoriesProvider.Get<IRepository<TEntity, TId>>();

            var deletedEntity = await repository.DeleteAsync(Entity);
            if (deletedEntity != null && await repository.SaveChanges())
            {
                Remove();
            }
        }
    }
}
