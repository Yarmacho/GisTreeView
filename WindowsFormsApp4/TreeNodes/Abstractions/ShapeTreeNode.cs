using MapWinGIS;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp4.ShapeFactories;

namespace WindowsFormsApp4.TreeNodes.Abstractions
{
    internal abstract class ShapeTreeNode : MapTreeNodeBase
    {
        protected readonly Shapefile Shapefile;
        protected readonly int ShapeIndex;
        protected readonly int LayerHandle;
        protected virtual IShapesFactory ShapesFactory { get; }

        public override bool AppendMode
        {
            get => Shapefile.AppendMode;
            set
            {
                if (value)
                {
                    Shapefile.StartAppendMode();     
                }
                else
                {
                    Shapefile.StopAppendMode();
                }
            }
        }

        protected virtual bool NeedStopEditingContextMenuItem => false;

        public ShapeTreeNode(Shapefile shapefile, int shapeIndex, int layerHandle)
        {
            Shapefile = shapefile;
            ShapeIndex = shapeIndex;
            LayerHandle = layerHandle;

            ContextMenu = BuildContextMenu();
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

        public void Focus()
        {
            Map.ZoomToShape(LayerHandle, ShapeIndex);
        }

        protected override ContextMenu BuildContextMenu()
        {
            var menu = base.BuildContextMenu();

            if (NeedStopEditingContextMenuItem)
            {
                menu.MenuItems.Add(new MenuItem("Stop editing", (s, e) => 
                {
                    var shape = ShapesFactory.EndCreate();  
                }));
            }

            return menu;
        }

        public virtual void AppendChild(double x, double y)
        {
            try
            {

            }
            finally
            {
                AppendMode = false;
            }
        }
    }
}
      