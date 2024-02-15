using Entities.Entities;
using Interfaces.Database.Abstractions;
using Interfaces.Database.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp4.TreeNodes;

namespace WindowsFormsApp4.TreeBuilder.NodesBuilders
{
    internal class RouteNodesBuilder : ShapeNodesBuilder<RouteTreeNode, Route>
    {
        private readonly IReadOnlyDictionary<int, ShipTreeNode> _shipNodes;

        public RouteNodesBuilder(IReadOnlyDictionary<int, ShipTreeNode> shipNodes)
        {
            _shipNodes = shipNodes;
        }

        public override async ValueTask<IEnumerable<RouteTreeNode>> BuildNodes(BuildNodesParams buildNodesParams)
        {
            var repositoriesProvider = buildNodesParams.ServiceProvider.GetRequiredService<IRepositoriesProvider>();
            var repository = repositoriesProvider.Get<IRoutesRepository>();

            var routesDict = await repository.GetRoutesDictionary();

            var shapefile = buildNodesParams.Map.get_Shapefile(buildNodesParams.RoutesLayerHandle);
            var nodes = new Dictionary<int, RouteTreeNode>();
            for (var i = 0; i < shapefile.NumShapes; i++)
            {
                var id = GetProperty<int>(shapefile, i, "Id");
                if (!routesDict.TryGetValue(id, out var route))
                {
                    continue;
                }

                var node = new RouteTreeNode(shapefile, i, buildNodesParams.RoutesLayerHandle);
                node.SetRoute(route);

                nodes[route.Id] = node;

                if (_shipNodes.TryGetValue(route.ShipId, out var shipNode))
                {
                    shipNode.Nodes.Add(node);

                    foreach (var point in route.Points)
                    {
                        node.Nodes.Add(new TreeNode($"{point.X} : {point.Y}"));
                    }
                }
            }

            return nodes.Values;
        }
    }
}
