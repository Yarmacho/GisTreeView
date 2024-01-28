using AxMapWinGIS;
using MapWinGIS;
using System;
using System.Collections.Generic;
using WindowsFormsApp4.TreeBuilder.NodesBuilders.Abstractions;
using WindowsFormsApp4.TreeNodes;

namespace WindowsFormsApp4.TreeBuilder.NodesBuilders
{
    internal abstract class MapTreeNodesBuilder<TNode> : IMapTreeNodesBuilder
        where TNode : MapTreeNode
    {
        public abstract IEnumerable<TNode> BuildNodes(BuildNodesParams buildNodesParams);

        protected T GetProperty<T>(Shapefile shapefile, int shape, string property)
        {
            var idFieldIndex = shapefile.FieldIndexByName[property];
            if (idFieldIndex != -1)
            {
                return (T)Convert.ChangeType(shapefile.CellValue[idFieldIndex, shape], typeof(T));
            }

            return default;
        }

        IEnumerable<MapTreeNode> IMapTreeNodesBuilder.BuildNodes(BuildNodesParams buildNodesParams)
        {
            return BuildNodes(buildNodesParams);
        }
    }
}
