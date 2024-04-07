using AxMapWinGIS;
using DynamicForms.Factories;
using Entities;
using GeoDatabase.ORM;
using GeoDatabase.ORM.Set.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tools;

namespace WindowsFormsApp4.TreeNodes.Abstractions
{
    internal abstract class ShapeTreeNode<TEntity> : MapTreeNodeBase<TEntity>, INodeWithMap, IFocusable
        where TEntity : EntityBase<int>, new()
    {
        protected readonly TEntity Entity;
        protected readonly int ShapeIndex;
        protected readonly int LayerHandle;

        AxMap INodeWithMap.Map => Map;

        protected ShapeTreeNode(TEntity entity, int shapeIndex, int layerHandle)
        {
            Entity = entity;
            ShapeIndex = shapeIndex;
            LayerHandle = layerHandle;
        }

        public override ValueTask Delete()
        {
            if (Nodes.Count != 0)
            {
                MessageBox.Show("Node has child nodes!");
                return new ValueTask(Task.CompletedTask);
            }
            var dbContext = TreeView.ServiceProvider.GetRequiredService<GeoDbContext>();
            var set = dbContext.Set<TEntity>();
            var entity = set.FirstOrDefault(e => e.Id == Entity.Id);
            set.Delete(entity);

            dbContext.SaveChanges();
            Remove();

            return new ValueTask(Task.CompletedTask);
        }

        public override ValueTask Update()
        {
            var form = FormFactory.CreateForm(Entity, EditMode.Edit);
            if (form.Activate() != DialogResult.OK)
            {
                return new ValueTask();
            }

            var dbContext = TreeView.ServiceProvider.GetRequiredService<GeoDbContext>();
            var set = dbContext.Set<TEntity>();
            var entity = set.FirstOrDefault(e => e.Id == Entity.Id);
            set.Update(entity);

            dbContext.SaveChanges();

            OnUpdate(Entity);

            return new ValueTask();
        }


        public void Focus()
        {
            Map.ZoomToShape(LayerHandle, ShapeIndex);
        }

        public override string GetDescription()
        {
            return Entity?.ToString();
        }
    }
}
      