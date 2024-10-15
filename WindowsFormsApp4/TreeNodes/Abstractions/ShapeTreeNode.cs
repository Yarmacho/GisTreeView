using AxMapWinGIS;
using Entities;
using GeoDatabase.ORM;
using GeoDatabase.ORM.Set.Extensions;
using MapWinGIS;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp4.Forms.Abstractions;

namespace WindowsFormsApp4.TreeNodes.Abstractions
{
    internal abstract class ShapeTreeNode<TEntity, TId> : MapTreeNodeBase<TEntity>, INodeWithMap, IFocusable
        where TEntity : EntityBase<TId>, new()
    {
        Initializers.Map INodeWithMap.Map => Map;

        protected TEntity Entity { get; }

        protected ShapeTreeNode(TEntity entity)
        {
            Entity = entity;
        }

        public override ValueTask Delete()
        {
            if (Nodes.Count != 0)
            {
                MessageBox.Show("Node has child nodes!");
                return new ValueTask(Task.CompletedTask);
            }

            var context = TreeView.ServiceProvider.GetRequiredService<GeoDbContext>();
            context.Set<TEntity>().Delete(Entity);
            context.SaveChanges();

            return new ValueTask(Task.CompletedTask);
        }

        public override ValueTask Update()
        {
            var form = FormsSelector.Select(Entity, Tools.EditMode.Edit);
            if (form.ShowDialog() != DialogResult.OK)
            {
                return new ValueTask();
            }

            var context = TreeView.ServiceProvider.GetRequiredService<GeoDbContext>();
            context.Set<TEntity>().Update(Entity);
            OnUpdate(Entity);
            context.SaveChanges();

            return new ValueTask();
        }


        public void Focus()
        {
            var context = TreeView.ServiceProvider.GetRequiredService<GeoDbContext>();
            var entity = context.Set<TEntity>().FirstOrDefault(e => e.Id.Equals(Entity.Id));

            var shapeIndex = context.ChangeTracker.GetShapeIndex(entity);
            if (shapeIndex != -1)
            {
                Map.ZoomToShape<TEntity>(shapeIndex);
            }
        }

        public override string GetDescription()
        {
            return Entity?.ToString();
        }
    }
}
      