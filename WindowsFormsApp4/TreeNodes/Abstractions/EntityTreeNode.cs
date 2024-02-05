using DynamicForms.Factories;
using Entities;
using Interfaces.Database.Abstractions;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace WindowsFormsApp4.TreeNodes.Abstractions
{
    internal abstract class EntityTreeNode<TEntity> : MapTreeNodeBase
        where TEntity : EntityBase
    {
        protected readonly TEntity Entity;
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
            var form = FormFactory.CreateForm(Entity, DynamicForms.Attributes.EditMode.Edit);
            if (form.Activate() == System.Windows.Forms.DialogResult.OK)
            {
                var newEntity = form.GetEntity<TEntity>();

                var repository = GetRepository();
                await repository.UpdateAsync(newEntity);

                if (await repository.SaveChanges())
                {
                    OnUpdate(newEntity);
                }
            }
        }

        protected abstract void OnUpdate(TEntity entity);
    }
}
