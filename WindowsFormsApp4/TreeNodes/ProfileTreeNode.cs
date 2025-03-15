using Entities;
using Entities.Entities;
using Interfaces.Database.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp4.Forms;
using WindowsFormsApp4.Initializers;
using WindowsFormsApp4.TreeNodes.Abstractions;

namespace WindowsFormsApp4.TreeNodes
{
    internal class ProfileTreeNode : MapTreeNodeBase, INodeWithMap
    {
        private readonly int _sceneId;
        private List<Profil> _profiles;

        public ProfileTreeNode(int sceneId, List<Profil> profiles)
        {
            _sceneId = sceneId;
            _profiles = profiles;
            Text = "Profiles";
        }

        Map INodeWithMap.Map => Map;

        public override string GetDescription()
        {
            if (_profiles == null)
            {
                return string.Empty;
            }

            var desc = new StringBuilder();
            foreach (var profile in _profiles)
            {
                desc.AppendLine($"Depth: {profile.Depth} | Temp: {profile.Temperature} | Salinity: {profile.Salinity} | Sound speed: {profile.SoundSpeed} | Absorbtion: {profile.Absorbsion}");
            }

            return desc.ToString();
        }

        protected override ContextMenu BuildContextMenu()
        {
            var contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add("Update", async (s, e) =>
            {
                var form = new ProfileFormV2(Map.Batimetry.OpenAsGrid(), _profiles);
                if (form.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                _profiles = form.Profiles;
                var repository = TreeView.ServiceProvider.GetRequiredService<IProfilesRepository>();
                foreach (var profil in _profiles)
                {
                    profil.SceneId = _sceneId;
                    await repository.AddAsync(profil);
                }

                await repository.SaveChanges();
            });

            return contextMenu;
        }

        protected override void ConfigureChildNodeEntity(object childEntity)
        {
        }
    }
}
