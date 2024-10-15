using System.Collections.Generic;
using System.Windows.Forms;

namespace WindowsFormsApp4.Initializers.DictionaryInitializers.ColumnsInitializers
{
    internal interface IDictionaryinitialer<T>
    {
        void ConfigureColumns(DataGridViewColumnCollection columns);

        IEnumerable<object[]> CreateRows(IEnumerable<T> values);
    }
}
