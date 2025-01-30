using Interfaces.Database.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp4.Initializers;
using WindowsFormsApp4.Properties;
using WindowsFormsApp4.TreeNodes.Abstractions;

namespace WindowsFormsApp4
{
    internal class MapTreeView : TreeView
    {
        private Button button1;

        public Map Map { get; internal set; }

        public void Redraw()
        {
            Map = MapInitializer.Init(Map.AxMap);
            foreach (var node in Nodes.OfType<MapTreeNodeBase>())
            {
                node.SetMap(Map);
            }
        }

        public MapTreeView()
        {
            ImageList = new ImageList();
            ImageList.Images.Add("scene", System.Drawing.Image.FromStream(new MemoryStream(Icons.area)));
            ImageList.Images.Add("gas", System.Drawing.Image.FromStream(new MemoryStream(Icons.sensor)));
            ImageList.Images.Add("ship", System.Drawing.Image.FromStream(new MemoryStream(Icons.ship)));
            ImageList.Images.Add("experiment", System.Drawing.Image.FromStream(new MemoryStream(Icons.experiment)));
            ImageList.Images.Add("route", System.Drawing.Image.FromStream(new MemoryStream(Icons.route)));
        }


        public IRepositoriesProvider RepositoriesProvider => ServiceProvider.GetRequiredService<IRepositoriesProvider>();

        public IServiceProvider ServiceProvider { get; internal set; }
    }
}
