using Entities;
using MapWinGIS;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WindowsFormsApp4.TreeBuilder.NodesBuilders.Abstractions;
using WindowsFormsApp4.TreeNodes.Abstractions;

namespace WindowsFormsApp4.TreeBuilder.NodesBuilders
{
    internal abstract class ShapeNodesBuilder<TNode, TEntity> : IMapTreeNodesBuilder
        where TNode : ShapeTreeNode<TEntity>
        where TEntity : EntityBase<int>, new()
    {
        public abstract ValueTask<IEnumerable<TNode>> BuildNodes(BuildNodesParams buildNodesParams);

        protected T GetProperty<T>(Shapefile shapefile, int shape, string property)
        {
            var idFieldIndex = shapefile.FieldIndexByName[property];
            if (idFieldIndex != -1)
            {
                return (T)Convert.ChangeType(shapefile.CellValue[idFieldIndex, shape], typeof(T));
            }

            return default;
        }

        async ValueTask<IEnumerable<MapTreeNodeBase>> IMapTreeNodesBuilder.BuildNodes(BuildNodesParams buildNodesParams)
        {
            return await BuildNodes(buildNodesParams);
        }
    }
}
