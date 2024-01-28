using MapWinGIS;
using System;
using System.Collections.Generic;
using WindowsFormsApp4.TreeBuilder.NodesBuilders;
using WindowsFormsApp4.TreeBuilder.NodesBuilders.Abstractions;
using WindowsFormsApp4.TreeNodes;

namespace WindowsFormsApp4
{
    internal class MapObjectsTreeBuilder
    {
        public IEnumerable<MapTreeNode> BuidNodes(BuildNodesParams buildNodesParams)
        {
            if (buildNodesParams.Map == null || buildNodesParams.GasLayerHandle == -1)
            {
                return new List<MapTreeNode>();
            }

            var nodes = getRootBuilder(buildNodesParams).BuildNodes(buildNodesParams);
            foreach (var node in nodes)
            {
                node.SetMap(buildNodesParams.Map);
            }

            return nodes;
        }

        private static IMapTreeNodesBuilder getRootBuilder(BuildNodesParams buildNodesParams)
        {
            if (buildNodesParams.GasLayerHandle != -1)
            {
                return new GasNodesBuilder();
            }
            else if (buildNodesParams.SceneLayerHandle != -1)
            {
                return new SceneNodesBuider(new Dictionary<int, GasTreeNode>());
            }
            else if (buildNodesParams.ShipLayerHandle != -1)
            {
                return new ShipNodesBuilder(new Dictionary<int, SceneTreeNode>());
            }
            else if (buildNodesParams.ProfileLayerHandle != -1)
            {
                return new ProfilNodesBuilder(new Dictionary<int, ShipTreeNode>());
            }
            else
            {
                throw new ArgumentException(nameof(buildNodesParams));
            }
        }
    }
}
