using Entities.Entities;
using GeoDatabase.ORM;
using GeoDatabase.ORM.Set.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Windows.Forms;
using WindowsFormsApp4.Initializers.DictionaryInitializers.ColumnsInitializers;

namespace WindowsFormsApp4.Initializers.DictionaryInitializers.Implementations
{
    internal class GasDictionartInitializer : IDictionaryinitialer<Gas>
    {
        public IEnumerable<object[]> CreateRows(IEnumerable<Gas> values)
        {
            var context = Program.ServiceProvider
                .GetRequiredService<GeoDbContext>();

            var set = context.Set<Gas>();
            foreach (var value in values)
            {
                yield return new object[] 
                { 
                    value.Id, value.Name, value.X, value.Y, set.Any(g => g.Id == value.Id)
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
