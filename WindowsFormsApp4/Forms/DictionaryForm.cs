using Entities;
using Interfaces.Database.Repositories;
using MapWinGIS;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tools;
using WindowsFormsApp4.ShapeConverters;

namespace DynamicForms.Forms
{
    internal partial class DictionaryForm : Form
    {
        private object _selectedRecord;
        private Dictionary<int, object> _definedRows = new Dictionary<int, object>();

        public DictionaryForm()
        {
            InitializeComponent();
            AcceptButton = submit;
            submit.DialogResult = DialogResult.OK;
        }

        public async Task Init<TEntity, TId>(IRepository<TEntity, TId> repository, Shapefile shapefile,
            IShapeEntityConverter<TEntity> converter)
            where TEntity : DictionaryEntity<TId>, new()
        {
            var shapes = getShapes<TEntity, TId>(shapefile, converter);

            foreach (var column in new TEntity().AsColumns())
            {
                dataGridView1.Columns.Add(column, column);
            }
            dataGridView1.Columns.Add("IsOnMap", "On map");

            var counter = 0;
            foreach (var entity in await repository.GetAllAsync())
            {
                var row = entity.AsDataRow();
                Array.Resize(ref row, row.Length + 1);
                row[row.Length - 1] = shapes.ContainsKey(entity.Id);

                _definedRows.Add(counter++, entity);
                dataGridView1.Rows.Add(row);
            }

            dataGridView1.AutoResizeColumns();
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToOrderColumns = false;
            dataGridView1.MultiSelect = false;

            dataGridView1.CellClick += (s, e) =>
            {
                _selectedRecord = _definedRows.TryGetValue(e.RowIndex, out var row) && row is TEntity entity
                    ? shapes.TryGetValue(entity.Id, out var _)
                        ? null
                        : row
                    : null;
            };
        }

        private Dictionary<TId, TEntity> getShapes<TEntity, TId>(Shapefile shapefile,
            IShapeEntityConverter<TEntity> converter)
            where TEntity : DictionaryEntity<TId>, new()
        {
            var result = new Dictionary<TId, TEntity>();
            for (var i = 0; i < shapefile.NumShapes; i++)
            {
                var entity = converter.FromShapeFile(shapefile, i);
                result[entity.Id] = entity;
            }

            return result;
        }

        public T GetSelectedRecord<T>()
        {
            return TypeTools.Convert<T>(_selectedRecord);
        }

        public object GetSelectedRecord()
        {
            return _selectedRecord;
        }
    }
}
