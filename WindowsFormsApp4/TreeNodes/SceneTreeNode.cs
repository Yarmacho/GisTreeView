using Entities.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tools;
using WindowsFormsApp4.TreeNodes.Abstractions;

namespace WindowsFormsApp4.TreeNodes
{
    internal class SceneTreeNode : ShapeTreeNode<Scene, int>
    {
        public SceneTreeNode(Scene scene) : base(scene)
        {
            Name = scene.Name;
            Text = scene.Name;
        }

        public IReadOnlyList<ShipTreeNode> ShipNodes => Nodes.OfType<ShipTreeNode>().ToList();

        public void AddNode(ShipTreeNode node)
        {
            Nodes.Add(node);
        }

        protected override ContextMenu BuildContextMenu()
        {
            var menu = base.BuildContextMenu();
            menu.MenuItems.Add(0, new MenuItem("Add sea object", async (s, e) => await AppendChild<Ship, ShipTreeNode>()));

            return menu;
        }

        protected override void ConfigureChildNodeEntity(object childEntity)
        {
            if (childEntity is Ship ship)
            {
                ship.SceneId = Entity.Id;
            }
        }

        protected override void OnUpdate(Scene entity)
        {
            Name = entity.Name;
            Text = entity.Name;
        }

        public override async ValueTask<bool> Delete()
        {
            if (!await base.Delete())
            {
                return false;
            }

            if (Map.SceneBattimetries.TryGetValue(Entity.Id, out var layerHandle))
            {
                var image = Map.AxMap.get_Image(layerHandle);
                if (File.Exists(image.SourceFilename))
                {
                    Map.AxMap.RemoveLayer(layerHandle);
                    File.Delete(image.SourceFilename);
                }
            }

            return true;
        }
    }
}
