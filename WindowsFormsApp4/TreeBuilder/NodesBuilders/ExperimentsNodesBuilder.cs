using Interfaces.Database.Abstractions;
using Interfaces.Database.Repositories;
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
        private readonly IExperimentsRepository _repository;
        private readonly IRepositoriesProvider _repositoriesProvider;

        public ExperimentsNodesBuilder(IRepositoriesProvider repositoriesProvider)
        {
            _repository = repositoriesProvider.Get<IExperimentsRepository>();
            _repositoriesProvider = repositoriesProvider;
        }

        public async ValueTask<IEnumerable<MapTreeNodeBase>> BuildNodes(BuildNodesParams buildNodesParams)
        {
            var nodes = new Dictionary<int, ExperimentTreeNode>();
            foreach (var experiment in await _repository.GetByIdsAsync(buildNodesParams.ExperimentIds))
            {
                var node = new ExperimentTreeNode(experiment, _repositoriesProvider);

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
