using Entities.Entities;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Tools;
using WindowsFormsApp4.Initializers.DictionaryInitializers;

namespace DynamicForms.Forms
{
    internal partial class DictionaryForm<T> : Form
        where T : IDictionaryEntity
    {
        private T _selectedRecord;
        private Dictionary<int, T> _definedRows = new Dictionary<int, T>();

        public DictionaryForm(IEnumerable<T> entities)
        {
            InitializeComponent();
            AcceptButton = submit;
            submit.DialogResult = DialogResult.OK;

            dataGridView1.Columns.ConfigureColumns<T>();
            dataGridView1.Rows.AddRows(entities);

            var counter = 0;
            _definedRows = entities.ToDictionary(_ => counter++);
            
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.Blue;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;
            dataGridView1.CellClick += (s, e) =>
            { 
                _selectedRecord = _definedRows.TryGetValue(e.RowIndex, out var row)
                    ? row
                    : default;
            };
        }

        public T GetSelectedRecord()
        {
            return TypeTools.Convert<T>(_selectedRecord);
        }
    }
}
