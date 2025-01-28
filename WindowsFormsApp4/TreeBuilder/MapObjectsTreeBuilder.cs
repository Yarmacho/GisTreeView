using Interfaces.Database.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WindowsFormsApp4.TreeBuilder.NodesBuilders;
using WindowsFormsApp4.TreeBuilder.NodesBuilders.Abstractions;
using WindowsFormsApp4.TreeNodes;
using WindowsFormsApp4.TreeNodes.Abstractions;

namespace WindowsFormsApp4
{
    internal class MapObjectsTreeBuilder
    {
        public async ValueTask<IEnumerable<MapTreeNodeBase>> BuidNodes(BuildNodesParams buildNodesParams)
        {
            if (buildNodesParams.Map == null || buildNodesParams.GasLayerHandle == -1)
            {
                return new List<MapTreeNodeBase>();
            }

            var nodes = await getRootBuilder(buildNodesParams).BuildNodes(buildNodesParams);
            foreach (var node in nodes)
            {
                node.SetMap(buildNodesParams.Map);
            }

            return nodes;
        }

        private IMapTreeNodesBuilder getRootBuilder(BuildNodesParams buildNodesParams)
        {
            if (buildNodesParams.ShowExperiments)
            {
                return new ExperimentsNodesBuilder();
            }
            else if (buildNodesParams.SceneLayerHandle != -1)
            {
                return new SceneNodesBuider(new Dictionary<int, ExperimentTreeNode>());
            }
            else if (buildNodesParams.GasLayerHandle != -1)
            {
                return new GasNodesBuilder();
            }
            else if (buildNodesParams.ShipLayerHandle != -1)
            {
                return new ShipNodesBuilder(new Dictionary<int, SceneTreeNode>());
            }
            else if (buildNodesParams.RoutesLayerHandle != -1)
            {
                return new RouteNodesBuilder(new Dictionary<int, ShipTreeNode>());
            }
            //else if (buildNodesParams.ProfileLayerHandle != -1)
            //{
            //    return new ProfilNodesBuilder(new Dictionary<int, ShipTreeNode>());
            //}
            else
            {
                throw new ArgumentException(nameof(buildNodesParams));
            }
        }
    }
}
