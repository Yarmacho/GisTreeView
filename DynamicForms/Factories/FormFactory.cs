using AxMapWinGIS;
using Tools.Attributes;
using DynamicForms.Forms;
using MapWinGIS;
using Entities.Entities;
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
        public static IEntityFormWithMap CreateFormWithMap(object entity, Shapefile shapefile, EditMode editMode = EditMode.View)
        {
            var form = new EntityFormWithMap(entity) 
            {
                Text = getFormCaption(entity, editMode)
            };
            configureForm(form, entity, editMode);
            addMap(form, shapefile);
            configureButtons(form, editMode);

            return form;
        }

        public static IEntityFormWithMap CreateFormWithMap<T>(Shapefile shapefile, EditMode editMode = EditMode.View)
            where T : new()
        {
            return CreateFormWithMap(new T(), shapefile, editMode);
        }

        private static void addMap(EntityFormWithMap form, Shapefile shapefile)
        {
            var map = new AxMap();

            map.BeginInit();
            form.SuspendLayout();

            map.Size = new System.Drawing.Size(600, 400);
            map.Enabled = true;
            map.Name = "map";
            map.Location = new System.Drawing.Point(form.Width - 10, 30);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(form.GetType());
            map.OcxState = ((AxHost.State)(resources.GetObject("axMap1.OcxState")));

            var panBtn = new Button();
            panBtn.Text = "Pan";
            panBtn.Name = "panBtn";
            panBtn.Location = new System.Drawing.Point(form.Width - 10, 5);
            panBtn.Size = new System.Drawing.Size(80, 25);
            panBtn.Click += (s, e) => map.CursorMode = tkCursorMode.cmPan;

            var addShapeBtn = new Button();
            addShapeBtn.Text = "Add shape";
            addShapeBtn.Name = "addShape";
            addShapeBtn.Location = new System.Drawing.Point(form.Width + 80, 5);
            addShapeBtn.Size = new System.Drawing.Size(80, 25);
            addShapeBtn.Click += (s, e) => map.CursorMode = tkCursorMode.cmAddShape;

            var zoomInBtn = new Button();
            zoomInBtn.Text = "Zoom in";
            zoomInBtn.Name = "ZoomIn";
            zoomInBtn.Location = new System.Drawing.Point(form.Width + 170, 5);
            zoomInBtn.Size = new System.Drawing.Size(80, 25);
            zoomInBtn.Click += (s, e) => map.CursorMode = tkCursorMode.cmZoomIn;

            var zoomOutBtn = new Button();
            zoomOutBtn.Text = "Zoom out";
            zoomOutBtn.Name = "ZoomOut";
            zoomOutBtn.Location = new System.Drawing.Point(form.Width + 260, 5);
            zoomOutBtn.Size = new System.Drawing.Size(80, 25);
            zoomOutBtn.Click += (s, e) => map.CursorMode = tkCursorMode.cmZoomOut;

            form.Controls.Add(addShapeBtn);
            form.Controls.Add(panBtn);
            form.Controls.Add(zoomInBtn);
            form.Controls.Add(zoomOutBtn);

            var directory = Path.Combine("mapPath", "Temp");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var tempFileName = Path.Combine(directory, $"{Guid.NewGuid()}.shp");
            var shapeFileClone = shapefile.Clone();
            MapInitResult layersInfo = null;
            form.Load += (s, e) =>
            {
                map.SendMouseDown = !(form.GetEntity<object>() is Scene);
                map.CursorMode = MapWinGIS.tkCursorMode.cmPan;
                layersInfo = MapInitializer.Init(Path.GetDirectoryName(shapefile.Filename), map);
                if (shapeFileClone.CreateNew(tempFileName, shapefile.ShapefileType))
                {
                    var layer = map.AddLayer(shapeFileClone, true);
                    map.MoveLayerTop(layer);
                }

                var scene = form.GetEntity<Scene>();
                if (scene != null)
                {
                    var angle = 45d;
                    var sideLength = 30000d;
                    var gasShapefile = map.get_Shapefile(layersInfo.GasLayerHandle);
                    object result = null;
                    string error = null;
                    if (!gasShapefile.Table.Query($"[Id] = {scene.GasId}", ref result, ref error))
                    {
                        return;
                    }
                    var gasShapeId = (result as int[] ?? Array.Empty<int>()).DefaultIfEmpty(-1).First();
                    var gasShape = gasShapefile.Shape[gasShapeId];
                    var origin = gasShape.Point[0];

                    var label = new System.Windows.Forms.Label();
                    label.Text = "Angle";
                    label.Top = 435;
                    label.Left = map.Left;
                    label.AutoSize = true;
                    form.Controls.Add(label);

                    var angleTextBox = new MaskedTextBox();
                    angleTextBox.Mask = "###.##";
                    angleTextBox.Top = 450;
                    angleTextBox.Text = angle.ToString();
                    angleTextBox.Left = map.Left;
                    angleTextBox.Width = 115;
                    angleTextBox.TextChanged += (s1, e1) =>
                    {
                        angle = TypeTools.Convert<double>(angleTextBox.Text);
                        buildScene(shapeFileClone, ref form.Shape, origin, angle, sideLength);
                        map.Redraw();
                    };
                    form.Controls.Add(angleTextBox);

                    label = new System.Windows.Forms.Label();
                    label.Text = "Length";
                    label.Top = 435;
                    label.Left = map.Left + 125;
                    label.AutoSize = true;
                    form.Controls.Add(label);

                    var lenghtTextBox = new MaskedTextBox();
                    lenghtTextBox.Mask = "#######.##";
                    lenghtTextBox.Top = 450;
                    lenghtTextBox.Text = sideLength.ToString();
                    lenghtTextBox.Left = map.Left + 125;
                    lenghtTextBox.Width = 115;
                    lenghtTextBox.TextChanged += (s1, e1) =>
                    {
                        sideLength = TypeTools.Convert<double>(lenghtTextBox.Text);
                        buildScene(shapeFileClone, ref form.Shape, origin, angle, sideLength);
                        map.Redraw();
                    };
                    form.Controls.Add(lenghtTextBox);

                    form.Height += 30;

                    buildScene(shapeFileClone, ref form.Shape, origin, angle, sideLength);
                    map.Redraw();
                }
            };

            form.FormClosed += (s, e) =>
            {
                map.Dispose();
                foreach (var file in Directory.GetFiles(directory, $"{Path.GetFileNameWithoutExtension(tempFileName)}.*"))
                {
                    File.Delete(file);
                }
            };

            var ship = form.GetEntity<Ship>();
            map.MouseDownEvent += (s, e) =>
            {
                if (map.CursorMode == tkCursorMode.cmAddShape)
                {
                    var projX = 0d;
                    var projY = 0d;
                    map.PixelToProj(e.x, e.y, ref projX, ref projY);

                    if (form.Shape == null)
                    {
                        shapeFileClone.StartAppendMode();
                        var shape = new Shape();
                        shape.Create(shapeFileClone.ShapefileType);
                        form.Shape = shape;

                        var shapeIndex = 0;
                        shapeFileClone.EditInsertShape(form.Shape, ref shapeIndex);
                        shapeFileClone.StopAppendMode();
                    }

                    var point = new Point();
                    point.x = projX;
                    point.y = projY;

                    var isPoint = shapeFileClone.ShapefileType == ShpfileType.SHP_POINT || shapeFileClone.ShapefileType == ShpfileType.SHP_POINTZ;
                    if (isPoint && form.Shape.numPoints == 1)
                    {
                        form.Shape.Point[0] = point;
                    }
                    else
                    {
                        var pointIndex = 0;
                        form.Shape.InsertPoint(point, ref pointIndex);
                    }

                    if (ship != null)
                    {
                        object result = null;
                        string error = null;
                        var sceneShapeFile = map.get_Shapefile(layersInfo.SceneLayerHandle);
                        if (!sceneShapeFile.Table.Query($"[SceneId] = {ship.SceneId}", ref result, ref error))
                        {
                            MessageBox.Show("Scene not found");
                            return;
                        }
                        var sceneShapeId = (result as int[] ?? Array.Empty<int>()).DefaultIfEmpty(-1).First();
                        var sceneShape = sceneShapeFile.Shape[sceneShapeId];

                        if (!form.Shape.Intersects(sceneShape))
                        {
                            form.Shape.Clear();
                        }
                    }

                    map.Redraw();
                }
            };

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

        private static void buildScene(Shapefile shapefile, ref Shape shape, Point point, double angle, double sideLength)
        {
            if (shapefile.NumShapes != 0)
            {
                shapefile.StartEditingShapes();
                shapefile.EditClear();
                shapefile.StopEditingShapes();
            }

            if (angle == 0 || sideLength == 0)
            {
                return;
            }

            shapefile.StartAppendMode();
            shapefile.StartEditingShapes();
            shape = new Shape();
            shape.Create(shapefile.ShapefileType);

            var pointA = new Point();
            pointA.x = point.x + sideLength;
            pointA.y = point.y + sideLength;

            var pointB = new Point();
            pointB.x = point.x - sideLength;
            pointB.y = point.y + sideLength;

            var pointC = new Point();
            pointC.x = point.x - sideLength;
            pointC.y = point.y - sideLength;
            
            var pointD = new Point();
            pointD.x = point.x + sideLength;
            pointD.y = point.y - sideLength;

            var pointIndex = -1;
            shape.InsertPoint(pointA, ref pointIndex);
            shape.InsertPoint(pointB, ref pointIndex);
            shape.InsertPoint(pointC, ref pointIndex);
            shape.InsertPoint(pointD, ref pointIndex);

            shape.Rotate(point.x, point.y, angle);

            var shapeIndex = 0;
            shapefile.EditInsertShape(shape, ref shapeIndex);
            shapefile.StopEditingShapes();
            shapefile.StopAppendMode();
        }

        public static IEntityForm CreateForm(object entity, EditMode editMode = EditMode.View)
        {
            var form = new EntityForm(entity) 
            {
                Text = getFormCaption(entity, editMode)
            };
            configureForm(form, entity, editMode);
            configureButtons(form, editMode);

            return form;
        }

        public static IEntityForm CreateForm<T>(EditMode editMode = EditMode.View)
            where T : new()
        {
            return CreateForm(new T(), editMode);
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
                textBox.Enabled = (displayAttribute?.Enabled ?? true) && editMode != EditMode.View && editMode != EditMode.Delete;
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

        private static string getFormCaption(object entity, EditMode editMode)
        {
            return $"{entity.GetType().Name}. {editMode}";
        }
    }
}
