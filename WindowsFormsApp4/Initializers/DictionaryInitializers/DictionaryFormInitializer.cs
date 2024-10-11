using Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp4.Initializers.DictionaryInitializers.ColumnsInitializers;
using WindowsFormsApp4.Initializers.DictionaryInitializers.Implementations;

namespace WindowsFormsApp4.Initializers.DictionaryInitializers
{
    internal static class DictionaryFormInitializer
    {
        public static void ConfigureColumns<T>(this DataGridViewColumnCollection columns)
            where T : IDictionaryEntity
        {
            var entityType = typeof(T);
            if (entityType == typeof(Ship))
            {
                new ShipDictioanaryInitializer().ConfigureColumns(columns);
            }
            else if (entityType == typeof(Gas))
            {
                new GasDictionartInitializer().ConfigureColumns(columns);
            }
            else
            {
                throw new NotImplementedException($"Dictionary configurator for type {entityType} is not implemented");
            }
        }

        public static void AddRows<T>(this DataGridViewRowCollection rows, IEnumerable<T> values)
            where T : IDictionaryEntity
        {
            IEnumerable<object[]> entityRows;

            var entityType = typeof(T);
            if (entityType == typeof(Ship))
            {
                entityRows = new ShipDictioanaryInitializer().CreateRows(values.OfType<Ship>());
            }
            else if (entityType == typeof(Gas))
            {
                entityRows = new GasDictionartInitializer().CreateRows(values.OfType<Gas>());
            }
            else
            {
                throw new NotImplementedException($"Dictionary configurator for type {entityType} is not implemented");
            }

            foreach (var row in entityRows)
            {
                rows.Add(row);
            }
        }
    }
}
