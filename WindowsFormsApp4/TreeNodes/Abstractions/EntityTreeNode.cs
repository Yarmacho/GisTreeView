using DynamicForms.Factories;
using Entities;
using Interfaces.Database.Abstractions;
using System.Threading.Tasks;
using Tools;

namespace WindowsFormsApp4.TreeNodes.Abstractions
{
    internal abstract class EntityTreeNode<TEntity> : MapTreeNodeBase<TEntity>
        where TEntity : EntityBase, new()
    {
        protected TEntity Entity;
        private readonly IRepositoriesProvider _repositoriesProvider;
        protected IWriteOnlyRepository<TEntity> GetRepository() => _repositoriesProvider.Get<IWriteOnlyRepository<TEntity>>();

        protected EntityTreeNode(TEntity entity, IRepositoriesProvider repositoriesProvider)
        {
            Entity = entity;
            _repositoriesProvider = repositoriesProvider;
        }

        public override async ValueTask Delete()
        {
            var repository = GetRepository();

            var deletedEntity = await repository.DeleteAsync(Entity);
            if (deletedEntity != null && await repository.SaveChanges())
            {
                Remove();
            }
        }

        public override async ValueTask Update()
        {
            var form = FormFactory.CreateForm(Entity, EditMode.Edit);
            if (form.Activate() == System.Windows.Forms.DialogResult.OK)
            {
                var newEntity = form.GetEntity<TEntity>();

                var repository = GetRepository();
                await repository.UpdateAsync(newEntity);

                if (await repository.SaveChanges())
                {
                    Entity = newEntity;
                    OnUpdate(newEntity);
                }
            }
        }

        public override string GetDescription()
        {
            return Entity.ToString();
        }
    }
}
