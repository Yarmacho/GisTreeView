using AxMapWinGIS;
using Entities;
using MapWinGIS;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp4.Forms.Abstractions;
using WindowsFormsApp4.ShapeConverters;

namespace WindowsFormsApp4.TreeNodes.Abstractions
{
    internal abstract class ShapeTreeNode<TEntity> : MapTreeNodeBase<TEntity>, INodeWithMap, IFocusable
        where TEntity : EntityBase, new()
    {
        protected readonly Shapefile Shapefile;
        protected readonly int ShapeIndex;
        protected readonly int LayerHandle;

        AxMap INodeWithMap.Map => Map;

        protected ShapeTreeNode(Shapefile shapefile, int shapeIndex, int layerHandle)
        {
            Shapefile = shapefile;
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

            Shapefile.StartEditingShapes();
            if (Shapefile.EditDeleteShape(ShapeIndex) && Shapefile.Save())
            {
                Remove();
            }
            Shapefile.StopEditingShapes();

            return new ValueTask(Task.CompletedTask);
        }

        public override ValueTask Update()
        {
            var converter = TreeView.ServiceProvider.GetRequiredService<IShapeEntityConverter<TEntity>>();

            var entity = converter.FromShapeFile(Shapefile, ShapeIndex);

            var form = FormsSelector.Select(entity, Tools.EditMode.Edit);
            if (form.ShowDialog() != DialogResult.OK)
            {
                return new ValueTask();
            }

            entity = form.Entity;
            converter.WriteToShapeFile(Shapefile, ShapeIndex, entity);
            OnUpdate(entity);

            return new ValueTask();
        }


        public void Focus()
        {
            Map.ZoomToShape(LayerHandle, ShapeIndex);
        }

        public override string GetDescription()
        {
            var converter = TreeView.ServiceProvider.GetRequiredService<IShapeEntityConverter<TEntity>>();

            var entity = converter.FromShapeFile(Shapefile, ShapeIndex);

            return entity.ToString();
        }
    }
}
      