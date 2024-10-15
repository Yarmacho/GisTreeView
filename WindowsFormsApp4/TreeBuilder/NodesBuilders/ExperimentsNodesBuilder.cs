using Interfaces.Database.Abstractions;
using Interfaces.Database.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WindowsFormsApp4.TreeBuilder.NodesBuilders.Abstractions;
using WindowsFormsApp4.TreeNodes;
using WindowsFormsApp4.TreeNodes.Abstractions;

namespace WindowsFormsApp4.TreeBuilder.NodesBuilders
{
    internal class ExperimentsNodesBuilder : IMapTreeNodesBuilder
    {
        public async ValueTask<IEnumerable<MapTreeNodeBase>> BuildNodes(BuildNodesParams buildNodesParams)
        {
            var repositoriesProvider = buildNodesParams.ServiceProvider.GetRequiredService<IRepositoriesProvider>();
            var repository = repositoriesProvider.Get<IExperimentsRepository>();

            var nodes = new Dictionary<int, ExperimentTreeNode>();
            foreach (var experiment in await repository.GetByIdsAsync(buildNodesParams.ExperimentIds))
            {
                var node = new ExperimentTreeNode(experiment);

                nodes[experiment.Id] = node;
            }

            if (nodes.Count > 0 && buildNodesParams.GasLayerHandle != -1)
            {
                await new GasNodesBuilder(nodes).BuildNodes(buildNodesParams);
            }

            return nodes.Values;
        }
    }
}
