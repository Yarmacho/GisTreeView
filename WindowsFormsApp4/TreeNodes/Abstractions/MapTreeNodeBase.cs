using AxMapWinGIS;
using DynamicForms.Factories;
using Entities;
using Entities.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp4.TreeNodes.Abstractions
{
    internal abstract class MapTreeNodeBase : TreeNode
    {
        public MapTreeNodeBase()
        {
            ContextMenu = BuildContextMenu();
        }

        protected AxMap Map;

        protected new MapTreeView TreeView => base.TreeView as MapTreeView;

        public abstract ValueTask Delete();
        public abstract ValueTask Update();
        public virtual async ValueTask AppendChild<TChildEntity, TChildNode>()
            where TChildNode : MapTreeNodeBase
            where TChildEntity : EntityBase, new()
        {
            var childNodeType = typeof(TChildNode);
            if (typeof(ShapeTreeNode).IsAssignableFrom(childNodeType))
            {
                var entityType = typeof(TChildEntity);
                var entityLayerHandle = entityType == typeof(Gas)
                    ? TreeView.LayersInfo.GasLayerHandle
                    : -1;
                if (entityLayerHandle == -1)
                {
                    return;
                }
                var shapeFile = Map.get_Shapefile(entityLayerHandle);

                var form = FormFactory.CreateFormWithMap<TChildEntity>(Path.GetDirectoryName(shapeFile.Filename),
                    shapeFile.ShapefileType, DynamicForms.Attributes.EditMode.Add);
                if (form.Activate() != DialogResult.OK)
                {
                    return;
                }

                var shape = form.GetShape();
                if (!shape.IsValid)
                {
                    return;
                }

                shapeFile.StartAppendMode();
                shapeFile.EditAddShape(shape);
                shapeFile.StopAppendMode();
            }
            else
            {

            }
        }

        protected virtual ContextMenu BuildContextMenu()
        {
            var items = new List<MenuItem>
            {
                new MenuItem("Delete", (s, e) => Delete()),
                new MenuItem("Update", (s, e) => Update())
            };

            return new ContextMenu(items.ToArray());
        }

        public void SetMap(AxMap map)
        {
            Map = map;
            foreach (var node in Nodes.OfType<ShapeTreeNode>())
            {
                node.SetMap(map);
            }
        }
    }
}
