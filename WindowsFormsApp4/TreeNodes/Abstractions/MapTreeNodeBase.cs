using AxMapWinGIS;
using Entities;
using Entities.Entities;
using Interfaces.Database.Abstractions;
using MapWinGIS;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp4.ShapeConverters;
using Tools;
using DynamicForms.Abstractions;
using WindowsFormsApp4.Forms.Abstractions;
using WindowsFormsApp4.Extensions;
using WindowsFormsApp4.Initializers;

namespace WindowsFormsApp4.TreeNodes.Abstractions
{
    internal abstract class MapTreeNodeBase : TreeNode, INodeWithDescription
    {
        public MapTreeNodeBase()
        {
            ContextMenu = BuildContextMenu();
        }

        protected Initializers.Map Map;

        protected new MapTreeView TreeView => base.TreeView as MapTreeView;

        protected abstract void ConfigureChildNodeEntity(object childEntity);

        protected abstract ContextMenu BuildContextMenu();

        public void SetMap(Initializers.Map map)
        {
            Map = map;
            foreach (var node in Nodes.OfType<INodeWithMap>())
            {
                node.SetMap(map);
            }
        }

        public abstract string GetDescription();
    }
    internal abstract class MapTreeNodeBase<TEntity> : MapTreeNodeBase
        where TEntity : EntityBase, new()
    {
        protected abstract void OnUpdate(TEntity entity);
        public abstract ValueTask Delete();
        public abstract ValueTask Update();

        public virtual async ValueTask<bool> AppendChild<TChildEntity, TChildNode>()
            where TChildNode : MapTreeNodeBase<TChildEntity>
            where TChildEntity : EntityBase, new()
        {
            var childNodeType = typeof(TChildNode);
            Shapefile shapeFile = null;
            int entityLayerHandle = -1;

            var childEntity = new TChildEntity();
            ConfigureChildNodeEntity(childEntity);

            var form = FormsSelector.Select(childEntity);
            if (form.ShowDialog() != DialogResult.OK)
            {
                return false;
            }

            childEntity = form.Entity;
            ConfigureChildNodeEntity(childEntity);
            var repository = TreeView.RepositoriesProvider.Get<IWriteOnlyRepository<TChildEntity>>();
            childEntity = await repository.AddAsync(childEntity);
            await repository.SaveChanges();

            form.CallOnFormClosedEvents();

            var childNode = (MapTreeNodeBase)Activator.CreateInstance(childNodeType,
                    new object[] { childEntity });

            childNode.SetMap(Map);
            Nodes.Add(childNode);
            Expand();

            if (childNode is RouteTreeNode routeTreeNode && childEntity is Route route)
            {
                routeTreeNode.SetRoute(route);
                if (route.Points.Count > 0)
                {
                    foreach (var point in route.Points)
                    {
                        childNode.Nodes.Add(new TreeNode($"{point.X} : {point.Y}"));
                    }
                    childNode.Expand();
                }
            }
            
            TreeView.Redraw();
            return true;
        }

        protected override ContextMenu BuildContextMenu()
        {
            var items = new List<MenuItem>
            {
                new MenuItem("Delete", (s, e) => Delete()),
                new MenuItem("Update", (s, e) => Update())
            };

            return new ContextMenu(items.ToArray());
        }
    }
}
