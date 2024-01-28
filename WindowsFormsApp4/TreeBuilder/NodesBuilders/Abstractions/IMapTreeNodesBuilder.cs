using System.Collections.Generic;
using WindowsFormsApp4.TreeNodes;

namespace WindowsFormsApp4.TreeBuilder.NodesBuilders.Abstractions
{
    interface IMapTreeNodesBuilder
    {
        IEnumerable<MapTreeNode> BuildNodes(BuildNodesParams buildNodesParams);
    }
}
