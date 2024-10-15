using Entities;
using Interfaces.Database.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tools;
using WindowsFormsApp4.Forms.Abstractions;

namespace WindowsFormsApp4.TreeNodes.Abstractions
{
    internal abstract class EntityTreeNode<TEntity> : MapTreeNodeBase<TEntity>
        where TEntity : EntityBase, new()
    {
        protected TEntity Entity;
        protected IWriteOnlyRepository<TEntity> GetRepository() => TreeView.ServiceProvider
            .GetRequiredService<IWriteOnlyRepository<TEntity>>();

        protected EntityTreeNode(TEntity entity)
        {
            Entity = entity;
        }

        public override async ValueTask Delete()
        {
            if (Nodes.Count != 0)
            {
                MessageBox.Show("Node has child nodes!");
                return;
            }

            var repository = GetRepository();

            var deletedEntity = await repository.DeleteAsync(Entity);
            if (deletedEntity != null && await repository.SaveChanges())
            {
                Remove();
            }
        }

        public override async ValueTask Update()
        {
            var form = FormsSelector.Select(Entity, EditMode.Edit);
            if (form.ShowDialog() == DialogResult.OK)
            {
                var newEntity = form.Entity;

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
