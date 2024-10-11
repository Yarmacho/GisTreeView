using Entities.Entities;
using System.Collections.Generic;
using System.Windows.Forms;
using WindowsFormsApp4.Initializers.DictionaryInitializers.ColumnsInitializers;

namespace WindowsFormsApp4.Initializers.DictionaryInitializers.Implementations
{
    internal class GasDictionartInitializer : IDictionaryinitialer<Gas>
    {
        public IEnumerable<object[]> CreateRows(IEnumerable<Gas> values)
        {
            foreach (var value in values)
            {
                yield return new object[] 
                { 
                    value.Id, value.Name, value.X, value.Y
                };
            }
        }

        public void ConfigureColumns(DataGridViewColumnCollection columns)
        {
            columns.Add("Id", "Id");
            columns.Add("Name", "Name");
            columns.Add("X", "X");
            columns.Add("Y", "Y");
            columns.Add("IsOnMap", "Is on map");
        }
    }
}
