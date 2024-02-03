using AxMapWinGIS;
using DynamicForms.Attributes;
using DynamicForms.Forms;
using MapWinGIS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Tools;

namespace DynamicForms.Factories
{
    public static class FormFactory
    {
        public static IEntityForm CreateFormWithMap(object entity, string mapsPath = null, ShpfileType shapeType = ShpfileType.SHP_POINT, EditMode editMode = EditMode.View)
        {
            var form = new EntityFormWithMap(entity);
            configureForm(form, entity, editMode);
            addMap(form, mapsPath, shapeType);
            configureButtons(form, editMode);

            return form;
        }

        private static void addMap(EntityFormWithMap form, string mapPath, ShpfileType shapeType = ShpfileType.SHP_POINT)
        {
            var map = new AxMap();

            map.BeginInit();
            form.SuspendLayout();

            map.Size = new System.Drawing.Size(600, 400);
            map.Enabled = true;
            map.Name = "map";
            map.Location = new System.Drawing.Point(form.Width - 10, 5);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(form.GetType());
            map.OcxState = ((AxHost.State)(resources.GetObject("axMap1.OcxState")));

            var tempFileName = Path.Combine(mapPath, $"{Guid.NewGuid()}.shp");
            var shapeFile = new Shapefile();
            shapeFile.DefaultDrawingOptions.SetDefaultPointSymbol(tkDefaultPointSymbol.dpsStar);
            shapeFile.DefaultDrawingOptions.FillColor = new Utils().ColorByName(tkMapColor.Blue);
            form.Load += (s, e) =>
            {
                map.SendMouseDown = true;
                map.CursorMode = MapWinGIS.tkCursorMode.cmPan;
                MapInitializer.Init(mapPath, map);
                if (shapeFile.CreateNew(tempFileName, shapeType))
                {
                    map.AddLayer(shapeFile, true);
                }
            };

            map.MouseDownEvent += (s, e) =>
            {
                if (map.CursorMode == tkCursorMode.cmAddShape)
                {
                    var projX = 0d;
                    var projY = 0d;
                    map.PixelToProj(e.x, e.y, ref projX, ref projY);

                    if (form.Shape == null)
                    {
                        shapeFile.StartAppendMode();
                        var shape = new Shape();
                        shape.Create(shapeType);
                        form.Shape = shape;

                        var shapeIndex = 0;
                        shapeFile.EditInsertShape(form.Shape, ref shapeIndex);
                        shapeFile.StopAppendMode();
                    }

                    var point = new Point();
                    point.x = projX;
                    point.y = projY;

                    shapeFile.StartEditingShapes();
                    if (shapeType == ShpfileType.SHP_POINT && form.Shape.numPoints == 1)
                    {
                        form.Shape.Point[0] = point;
                    }
                    else
                    {
                        var pointIndex = 0;
                        form.Shape.InsertPoint(point, ref pointIndex);
                    }
                    shapeFile.StopEditingShapes();
                }
            };

            var menuItems = new MenuItem[]
            {
                new MenuItem("Pan", (s, e) => map.CursorMode = MapWinGIS.tkCursorMode.cmPan),
                new MenuItem("Add shape", (s, e) => map.CursorMode = MapWinGIS.tkCursorMode.cmAddShape),
                new MenuItem("Zoom in", (s, e) => map.CursorMode = MapWinGIS.tkCursorMode.cmZoomIn),
                new MenuItem("Zoom out", (s, e) => map.CursorMode = MapWinGIS.tkCursorMode.cmZoomOut),
            };

            form.ContextMenu = new ContextMenu(menuItems);
            form.Controls.Add(map);

            if (form.Height < 500)
            {
                form.Height = 500;
            }
            else
            {
                map.Height = form.Height - 30;
            }
            form.Width += 650;

            map.EndInit();
            form.ResumeLayout();
        }

        public static IEntityForm CreateForm(object entity, EditMode editMode = EditMode.View)
        {
            var form = new EntityForm(entity);
            configureForm(form, entity, editMode);
            configureButtons(form, editMode);

            return form;
        }

        private static void configureForm(EntityForm form, object entity, EditMode editMode = EditMode.View)
        {
            form.SuspendLayout();
            int usedHeight = 0;

            var controls = new List<Control>();
            var maxLabelWidth = 0;
            foreach (var property in entity.GetType().GetProperties())
            {
                var propertyAttributes = property.GetCustomAttributes(true);
                var ignoreAttribute = propertyAttributes.OfType<IgnorePropertyAttribute>().FirstOrDefault();
                if (ignoreAttribute != null && ignoreAttribute.EditMode.HasFlag(editMode))
                {
                    continue;
                }

                var displayAttribute = propertyAttributes.OfType<DisplayAttribute>().FirstOrDefault();

                System.Windows.Forms.Label label = new System.Windows.Forms.Label();
                label.Text = displayAttribute?.Label ?? property.Name;
                label.Top = usedHeight + 7;
                label.Left = 5;
                label.AutoSize = true;
                form.Controls.Add(label);

                maxLabelWidth = Math.Max(maxLabelWidth, label.Width);

                TextBox textBox = new TextBox();
                textBox.Text = property.GetValue(entity)?.ToString();
                textBox.Top = usedHeight + 5;
                textBox.Width = 115;
                textBox.Enabled = editMode != EditMode.View && editMode != EditMode.Delete;
                textBox.TextChanged += (s, e) =>
                {
                    property.SetValue(entity, TypeTools.Convert(textBox.Text, property.PropertyType));
                };
                controls.Add(textBox);

                form.Controls.Add(textBox);

                usedHeight += textBox.Height + 10;
            }

            var maxControlWidth = 0;
            foreach (var control in controls)
            {
                control.Left = maxLabelWidth + 10;
                maxControlWidth = Math.Max(maxControlWidth, control.Width);
            }

            form.ResumeLayout();
            form.Width = maxLabelWidth + maxControlWidth + 90;
            form.Height = usedHeight + 65;
        }

        private static void configureButtons(Form form, EditMode editMode)
        {
            if (editMode != EditMode.View)
            {
                var btn = new Button()
                {
                    DialogResult = DialogResult.OK,
                    Text = "Ok",
                };
                btn.Top = form.Height - btn.Height - 40;
                btn.Left = form.Width - btn.Width - 20;

                form.AcceptButton = btn;
                form.Controls.Add(btn);
            }
        }
    }
}
