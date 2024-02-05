using Entities.Entities;
using MapWinGIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WindowsFormsApp4.TreeNodes.Abstractions;

namespace WindowsFormsApp4.TreeNodes
{
    internal class GasTreeNode : ShapeTreeNode
    {
        public GasTreeNode(Shapefile shapefile, int shapeIndex, int layerHandle) : base(shapefile, shapeIndex, layerHandle)
        {
            var nameFieldIndex = Shapefile.FieldIndexByName["Ent_num"];
            if (nameFieldIndex == -1)
            {
                throw new ArgumentException("Incorrent shapefile provided!");
            }
            Name = Shapefile.CellValue[nameFieldIndex, shapeIndex]?.ToString();
            Text = Shapefile.CellValue[nameFieldIndex, shapeIndex]?.ToString();
        }

        public IReadOnlyList<SceneTreeNode> SceneNodes => Nodes.OfType<SceneTreeNode>().ToList();

        public void AddNode(SceneTreeNode node)
        {
            Nodes.Add(node);
        }

        protected override async ValueTask OnAppendingNode(object entity)
        {
            if (!(entity is Gas gas))
            {
                return;
            }

            var nameFieldIndex = Shapefile.FieldIndexByName["Ent_num"];
            var experimentIdFieldIndex = Shapefile.FieldIndexByName["ExperimentId"];

            Shapefile.StartEditingShapes();

            Shapefile.EditCellValue(nameFieldIndex, ShapeIndex, gas.Name);
            Shapefile.EditCellValue(experimentIdFieldIndex, ShapeIndex, gas.ExperimentId);

            Shapefile.StopEditingShapes();
        }
    }
}
