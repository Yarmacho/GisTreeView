using Entities.Entities;
using GeoDatabase.ORM;
using GeoDatabase.ORM.Set.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WindowsFormsApp4.Initializers.DictionaryInitializers.ColumnsInitializers
{
    internal class ShipDictioanaryInitializer : IDictionaryinitialer<Ship>
    {
        public IEnumerable<object[]> CreateRows(IEnumerable<Ship> values)
        {
            var context = Program.ServiceProvider
                .GetRequiredService<GeoDbContext>();
            
            var shipSet = context.Set<Ship>();
            foreach (var value in values)
            {
                yield return new object[] 
                {
                    value.Id, value.Name, value.X, value.Y, shipSet.Any(s => s.Id == value.Id)
                };
            }
        }

        public void ConfigureColumns(DataGridViewColumnCollection columns)
        {
            columns.Add("Id", "Id");
            columns.Add("Name", "Name");
            columns.Add("X", "X");
            columns.Add("Y", "Y");
            columns.Add("OnMap", "Is on map");
        }
    }
}
