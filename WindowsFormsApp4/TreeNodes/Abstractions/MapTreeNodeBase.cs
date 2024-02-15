using AxMapWinGIS;
using DynamicForms.Factories;
using DynamicForms.Forms;
using Entities;
using Entities.Entities;
using Interfaces.Database.Abstractions;
using MapWinGIS;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp4.ShapeConverters;
using Tools;

namespace WindowsFormsApp4.TreeNodes.Abstractions
{
    internal abstract class MapTreeNodeBase : TreeNode, INodeWithDescription
    {
        public MapTreeNodeBase()
        {
            ContextMenu = BuildContextMenu();
        }

        protected AxMap Map;

        protected new MapTreeView TreeView => base.TreeView as MapTreeView;

        protected abstract void ConfigureChildNodeEntity(object childEntity);

        protected abstract ContextMenu BuildContextMenu();

        public void SetMap(AxMap map)
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
        public virtual async ValueTask AppendChild<TChildEntity, TChildNode>()
            where TChildNode : MapTreeNodeBase<TChildEntity>
            where TChildEntity : EntityBase, new()
        {
            var childNodeType = typeof(TChildNode);
            IEntityForm form = null;
            Shapefile shapeFile = null;
            int entityLayerHandle = -1;
            int shapeIndex = -1;

            var childEntity = new TChildEntity();
            ConfigureChildNodeEntity(childEntity);
            var isShapeNode = typeof(ShapeTreeNode<TChildEntity>).IsAssignableFrom(childNodeType);
            if (isShapeNode)
            {
                var entityType = typeof(TChildEntity);
                entityLayerHandle = entityType == typeof(Gas)
                    ? TreeView.LayersInfo.GasLayerHandle
                    : entityType == typeof(Scene)
                        ? TreeView.LayersInfo.SceneLayerHandle
                        : entityType == typeof(Ship)
                            ? TreeView.LayersInfo.ShipLayerHandle
                            : entityType == typeof(Route)
                                ? TreeView.LayersInfo.RoutesLayerHandle
                                : -1;
                if (entityLayerHandle == -1)
                {
                    return;
                }

                shapeFile = Map.get_Shapefile(entityLayerHandle);
                if (shapeFile == null)
                {
                    return;
                }

                form = FormFactory.CreateFormWithMap(childEntity, shapeFile,
                    EditMode.Add);
            }
            else
            {
                form = FormFactory.CreateForm(childEntity, EditMode.Add);
            }

            if (form.Activate() != DialogResult.OK)
            {
                return;
            }

            if (form is IEntityFormWithMap formWithMap)
            {
                var shape = formWithMap.GetShape();
                if (!shape.IsValid)
                {
                    return;
                }

                shapeFile.StartAppendMode();
                shapeIndex = shapeFile.EditAddShape(shape);
                shapeFile.StopAppendMode();
            }

            childEntity = form.GetEntity<TChildEntity>();
            var repository = TreeView.RepositoriesProvider.Get<IWriteOnlyRepository<TChildEntity>>();
            childEntity = await repository.AddAsync(childEntity);
            if (!await repository.SaveChanges())
            {
                return;
            }

            MapTreeNodeBase childNode = null;
            if (isShapeNode)
            {
                TreeView.ServiceProvider.GetRequiredService<IShapeEntityConverter<TChildEntity>>()
                    .WriteToShapeFile(shapeFile, shapeIndex, childEntity);

                childNode = (MapTreeNodeBase)Activator.CreateInstance(childNodeType,
                    new object[] { shapeFile, shapeIndex, entityLayerHandle });
            }
            else
            {
                childNode = (MapTreeNodeBase)Activator.CreateInstance(childNodeType,
                    new object[] { childEntity, TreeView.RepositoriesProvider });
            }

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
