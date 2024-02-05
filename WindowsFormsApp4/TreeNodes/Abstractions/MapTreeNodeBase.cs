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
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp4.ShapeConverters;

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
            IEntityForm form = null;
            Shapefile shapeFile = null;
            int entityLayerHandle = -1;
            int shapeIndex = -1;

            var isShapeNode = typeof(ShapeTreeNode).IsAssignableFrom(childNodeType);
            if (isShapeNode)
            {
                var entityType = typeof(TChildEntity);
                entityLayerHandle = entityType == typeof(Gas)
                    ? TreeView.LayersInfo.GasLayerHandle
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

                form = FormFactory.CreateFormWithMap<TChildEntity>(shapeFile, Path.GetDirectoryName(shapeFile.Filename),
                    DynamicForms.Attributes.EditMode.Add);
            }
            else
            {
                form = FormFactory.CreateForm<TChildEntity>(DynamicForms.Attributes.EditMode.Add);
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

            var childEntity = form.GetEntity<TChildEntity>();
            ConfigureChildNodeEntity(childEntity);

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

            await childNode.OnAppendingNode(childEntity);
            childNode.SetMap(Map);
            Nodes.Add(childNode);
            Expand();
        }

        protected abstract void ConfigureChildNodeEntity(object childEntity);

        protected abstract ValueTask OnAppendingNode(object entity);

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
