using Entities;
//using Interfaces.Database.Repositories;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DynamicForms.Forms
{
    internal partial class DictionaryForm : Form
    {
        public DictionaryForm()
        {
            InitializeComponent();
        }

        //public async Task Init<TEntity, TId>(IRepository<TEntity, TId> repository)
        //    where TEntity : DictionaryEntity<TId>
        //{
        //    var dataTable = new DataTable();
        //    foreach (var property in typeof(TEntity).GetProperties(BindingFlags.Instance | BindingFlags.Public))
        //    {
        //        dataTable.Columns.Add(property.Name, property.PropertyType);
        //    }

        //    foreach (var entity in await repository.GetAllAsync())
        //    {
        //        dataTable.Rows.Add(entity.AsArray());
        //    }
        //    dataGridView1.AutoResizeColumns();
        //}
    }
}
