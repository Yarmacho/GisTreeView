//using Entities.Entities;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using WindowsFormsApp4.TreeNodes;

//namespace WindowsFormsApp4.TreeBuilder.NodesBuilders
//{
//    internal class ProfilNodesBuilder : ShapeNodesBuilder<ProfilTreeNode, Profil>
//    {
//        private readonly IReadOnlyDictionary<int, ShipTreeNode> _shipNodes;

//        public ProfilNodesBuilder(IReadOnlyDictionary<int, ShipTreeNode> shipNodes)
//        {
//            _shipNodes = shipNodes;
//        }

//        public override async ValueTask<IEnumerable<ProfilTreeNode>> BuildNodes(BuildNodesParams buildNodesParams)
//        {
//            var nodes = new Dictionary<int, ProfilTreeNode>();

//            var shapefile = buildNodesParams.Map.get_Shapefile(buildNodesParams.ProfileLayerHandle);
//            for (int i = 0; i < shapefile.NumShapes; i++)
//            {
//                var id = GetProperty<int>(shapefile, i, "Id");
//                if (id == 0)
//                {
//                    continue;
//                }

//                var node = new ProfilTreeNode(shapefile, i, buildNodesParams.ProfileLayerHandle);
//                nodes[id] = node;

//                var shipId = GetProperty<int>(shapefile, i, "ShipId");
//                if (shipId != 0 && _shipNodes.TryGetValue(shipId, out var shipNode))
//                {
//                    shipNode.AddNode(node);
//                }
//            }

//            return nodes.Values;
//        }
//    }
//}
