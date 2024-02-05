using MapWinGIS;
using System.Threading.Tasks;

namespace WindowsFormsApp4.TreeNodes.Abstractions
{
    internal abstract class ShapeTreeNode : MapTreeNodeBase
    {
        protected readonly Shapefile Shapefile;
        protected readonly int ShapeIndex;
        protected readonly int LayerHandle;

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
            throw new System.NotImplementedException();
        }

        protected override ValueTask OnAppendingNode(object entity)
        {
            return new ValueTask();
        }

        protected override void ConfigureChildNodeEntity(object childEntity)
        {
        }

        public void Focus()
        {
            Map.ZoomToShape(LayerHandle, ShapeIndex);
        }
    }
}
      