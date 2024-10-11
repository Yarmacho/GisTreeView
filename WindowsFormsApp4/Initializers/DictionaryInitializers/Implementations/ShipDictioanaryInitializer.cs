using Entities.Entities;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WindowsFormsApp4.Initializers.DictionaryInitializers.ColumnsInitializers
{
    internal class ShipDictioanaryInitializer : IDictionaryinitialer<Ship>
    {
        public IEnumerable<object[]> CreateRows(IEnumerable<Ship> values)
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
            columns.Add("OnMap", "Is on map");
        }
    }
}
