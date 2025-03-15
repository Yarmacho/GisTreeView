using Entities.Contracts;
using Entities.Entities;
using GeoDatabase.ORM;
using MapWinGIS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp4.Events;
using WindowsFormsApp4.Initializers;
using WindowsFormsApp4.Logic;

namespace WindowsFormsApp4.Forms
{
    public partial class ProfilesForm : Form
    {
        private Initializers.Map _map;
        private string _profilesPath;

        public ProfilesForm()
        {
            InitializeComponent(); 
            _profilesPath = Path.Combine(Program.Configuration.GetValue<string>("MapsPath"),
                Program.Configuration.GetValue<string>("ProfilesFolder"));

            _map = MapInitializer.Init(axMap1);
            _map.CursorMode = tkCursorMode.cmPan;
            listView1.CheckBoxes = false;
            listView1.MultiSelect = false;
            listView1.Click += (s, e) =>
            {
                if (listView1.FocusedItem is SceneListViewItem sceneListViewItem)
                {
                    sceneListViewItem.ZoomToShape();
                }
            };

            var battimetryGrid = _map.Batimetry.OpenAsGrid();

            var profilesCalculated = hasProfiles();
            calcProfiles.Enabled = !profilesCalculated;
            temperatureBtn.Enabled = profilesCalculated;
            salinityBtn.Enabled = profilesCalculated;
            noProfile.Checked = true;

            var maximum = Convert.ToInt32(battimetryGrid.Maximum);
            depthSlider.Maximum = maximum > 0 ? 0 : maximum;
            depthSlider.Minimum = Convert.ToInt32(battimetryGrid.Minimum);
            depthSlider.SmallChange = Math.Max(1, (depthSlider.Maximum - depthSlider.Minimum) / 100);
            depthSlider.LargeChange = Math.Max(1, (depthSlider.Maximum - depthSlider.Minimum) / 10);

            depthSlider.ValueChanged += (s, e) => { depthLabel.Text = $"Depth: {Math.Abs(depthSlider.Value)}"; };

            var dbContext = Program.ServiceProvider.GetRequiredService<GeoDbContext>();

            foreach (var scene in dbContext.Set<Scene>().ToList())
            {
                var shapeIndex = dbContext.ChangeTracker.GetShapeIndex(scene);
                listView1.Items.Add(new SceneListViewItem(_map, scene, shapeIndex));
            }

            var ts = new TSProfileBuilder(_map.Batimetry.OpenAsGrid(), _map.AxMap);

            CancellationTokenSource tokenSource = null;
            depthSlider.ValueChanged += async (s, e) =>
            {
                if (noProfile.Checked)
                {
                    return;
                }

                ProfileType? profileType = temperatureBtn.Checked
                    ? ProfileType.Temperature 
                    : salinityBtn.Checked
                        ? (ProfileType?)ProfileType.Salinity
                        : null;
                if (!profileType.HasValue)
                {
                    return;
                }

                if (tokenSource != null && !tokenSource.IsCancellationRequested)
                {
                    tokenSource.Cancel();
                }

                tokenSource = new CancellationTokenSource();

                ts.UpdateProfile(depthSlider.Value, profileType.Value, tokenSource.Token);
            };
        }

        private async void calcProfiles_Click(object sender, EventArgs e)
        {
            if (hasProfiles())
            {
                MessageBox.Show("Profiles are already calculated");
                calcProfiles.Enabled = false;
                return;
            }

            var profilesPath = Path.Combine(_profilesPath, "Map");
            if (!Directory.Exists(profilesPath))
            {
                Directory.CreateDirectory(profilesPath);
            }

            Program.ServiceProvider.GetService<IEventBus>()
                .Publish(new ProfilesRequested());
            //await Task.Run(() => calculateProfiles(_map.Batimetry.OpenAsGrid(), Path.Combine(_profilesPath, "Map")));
            calcProfiles.Enabled = false;
        }

        private bool hasProfiles()
        {
            var profilesPath = Path.Combine(_profilesPath, "Map");
            return Directory.Exists(profilesPath) && Directory.EnumerateFiles(profilesPath).Any();
        }

        private void calculateProfiles(Grid battimetry, string folder)
        {
            if (Directory.Exists(folder))
            {
                foreach (var file in Directory.EnumerateFiles(folder))
                {
                    File.Delete(file);
                }
            }
            else
            {
                Directory.CreateDirectory(folder);
            }

            var tsProcessor = new TsTableProcessor(battimetry, folder);
            tsProcessor.GenerateTsGrids();
        }
    }

    public class SceneListViewItem : ListViewItem
    {
        private readonly Initializers.Map _map;
        private readonly Scene _scene;
        private readonly int _shapeIndex;

        public SceneListViewItem(Initializers.Map map, Scene scene, int shapeIndex)
            : base(scene.Name ?? $"Scene {scene.Id}")
        {
            _map = map;
            _scene = scene;
            _shapeIndex = shapeIndex;
        }

        public void ZoomToShape()
        {
            _map.ZoomToShape<Scene>(_shapeIndex);
        }
    }
}
