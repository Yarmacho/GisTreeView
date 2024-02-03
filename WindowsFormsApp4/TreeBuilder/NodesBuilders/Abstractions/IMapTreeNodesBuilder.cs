using System.Collections.Generic;
using System.Threading.Tasks;
using WindowsFormsApp4.TreeNodes.Abstractions;

namespace WindowsFormsApp4.TreeBuilder.NodesBuilders.Abstractions
{
    interface IMapTreeNodesBuilder
    {
        ValueTask<IEnumerable<MapTreeNodeBase>> BuildNodes(BuildNodesParams buildNodesParams);
    }
}
