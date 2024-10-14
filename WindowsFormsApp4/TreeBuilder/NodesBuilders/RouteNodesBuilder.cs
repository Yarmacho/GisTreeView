using Entities.Entities;
using GeoDatabase.ORM;
using Interfaces.Database.Abstractions;
using Interfaces.Database.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
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

            var dbContext = buildNodesParams.ServiceProvider.GetRequiredService<GeoDbContext>();
            var nodes = new Dictionary<int, RouteTreeNode>();
            foreach (var routeShape in dbContext.Set<Route>().ToList())
            {
                if (!routesDict.TryGetValue(routeShape.Id, out var route))
                {
                    continue;
                }

                var node = new RouteTreeNode(route);
                node.SetRoute(route);

                nodes[route.Id] = node;

                if (_shipNodes.TryGetValue(route.ShipId, out var shipNode))
                {
                    shipNode.Nodes.Add(node);
                }
            }

            return nodes.Values;
        }
    }
}
