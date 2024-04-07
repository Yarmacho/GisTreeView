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
using GeoDatabase.ORM.Mapper;
using GeoDatabase.ORM;

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
        public async ValueTask AppendChild<TChildEntity, TChildNode>()
            where TChildNode : MapTreeNodeBase<TChildEntity>
            where TChildEntity : EntityBase, new()
        {
            var childNodeType = typeof(TChildNode);
            var childEntity = new TChildEntity();
            ConfigureChildNodeEntity(childEntity);
            IEntityForm form = null;


            var isShapeNode = isShapeNodeType(childNodeType);
            if (isShapeNode)
            {
                var config =
                    TreeView.ServiceProvider.GetRequiredService<IMappingConfigProvider>().GetConfig<TChildEntity>();
                form = FormFactory.CreateFormWithMap(childEntity, config.Shapefile,
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

            childEntity = form.GetEntity<TChildEntity>();
            ConfigureChildNodeEntity(childEntity);
            var repository = TreeView.RepositoriesProvider.Get<IWriteOnlyRepository<TChildEntity>>();
            childEntity = await repository.AddAsync(childEntity);
            await repository.SaveChanges();

            var shapeIndex = -1;
            if (form is IEntityFormWithMap formWithMap)
            {
                var shape = formWithMap.GetShape();
                if (shape == null || !shape.IsValid)
                {
                    MessageBox.Show("Shape invalid");
                    return;
                }

                var dbContext = TreeView.ServiceProvider.GetRequiredService<GeoDbContext>();
                var entry = dbContext.Set<TChildEntity>().Add(childEntity);
                entry.Shape = formWithMap.GetShape();

                dbContext.SaveChanges();


                shapeIndex = entry.ShapeIndex;
            }

            MapTreeNodeBase childNode = null;
            if (isShapeNode)
            {
                var layerHandle = TreeView.LayersInfo.GetLayerHandle(typeof(TChildEntity));
                var configsProvider = TreeView.ServiceProvider.GetRequiredService<IMappingConfigProvider>();
                Map.RemoveLayer(layerHandle);
                layerHandle = Map.AddLayer(configsProvider.GetConfig<TChildEntity>().Shapefile, true);
                TreeView.LayersInfo.SetLayerHandle(typeof(TChildEntity), layerHandle);

                childNode = (MapTreeNodeBase)Activator.CreateInstance(typeof(TChildNode),
                    new object[] { childEntity, shapeIndex, layerHandle });
            }
            else
            {
                childNode = (MapTreeNodeBase)Activator.CreateInstance(typeof(TChildNode),
                    new object[] { childEntity, TreeView.RepositoriesProvider });
            }

            childNode.SetMap(Map);
            Nodes.Add(childNode);
            Expand();

            if (childNode is RouteTreeNode routeTreeNode && childEntity is Route route)
            {
                routeTreeNode.SetRoute(route);
            }
        }

        private bool isShapeNodeType(Type type)
        {
            var nodeType = type;
            while (nodeType != null)
            {
                if (nodeType.IsGenericType && nodeType.GetGenericTypeDefinition() == typeof(ShapeTreeNode<>))
                {
                    return true;
                }

                nodeType = nodeType.BaseType;
            }

            return false;
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
