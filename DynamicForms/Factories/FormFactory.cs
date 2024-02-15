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
            configureMap(form, shapefile);
            configureButtons(form, editMode);

            return form;
        }

        public static IEntityFormWithMap CreateFormWithMap<T>(Shapefile shapefile, EditMode editMode = EditMode.View)
            where T : new()
        {
            return CreateFormWithMap(new T(), shapefile, editMode);
        }

        private static void configureMap(EntityFormWithMap form, Shapefile shapefile)
        {
            MapInitResult layersInfo = null;
            var map = form.Map;
            form.Load += (s, e) =>
            {
                layersInfo = MapInitializer.Init(Path.GetDirectoryName(shapefile.Filename), map);
                map.set_ShapeLayerFillTransparency(layersInfo.SceneLayerHandle, 0.3f);

                if (layersInfo.BatimetryLayerHandle != -1)
                {
                    var depthLabel = form.Controls.OfType<System.Windows.Forms.Label>()
                        .FirstOrDefault(l => l?.Name == "depth");
                    var batimetry = map.get_Image(layersInfo.BatimetryLayerHandle);
                    if (batimetry != null && depthLabel != null)
                    {
                        var band = batimetry.Band[1];

                        form.OnMouseMoveOnMap += (x, y) =>
                        {
                            var longtitude = 0d;
                            var latitude = 0d;

                            map.PixelToProj(x, y, ref longtitude, ref latitude);
                            batimetry.ProjectionToImage(longtitude, latitude, out int column, out int row);

                            var hasValue = band.Value[column, row, out var depth];
                            depthLabel.AutoSize = true;
                            depthLabel.Text = hasValue ? $"Depth: {depth}" : string.Empty;
                        };
                    }
                }

                form.CreateNewShapefile(shapefile);
                form.SendMouseDownEvent = !(form.Entity is Scene);

                object result = null;
                string error = null;
                switch (form.Entity)
                {
                    case Gas gas:
                        form.AfterShapeValid += (shape) =>
                        {
                            if (shape.numPoints == 1)
                            {
                                var point = shape.Point[0];
                                gas.X = point.x;
                                gas.Y = point.y;
                            }
                        };
                        break;
                    case Ship ship:
                        form.ValidShape += (point, shape) =>
                        {
                            var sceneShapeFile = map.get_Shapefile(layersInfo.SceneLayerHandle);
                            if (!sceneShapeFile.Table.Query($"[SceneId] = {ship.SceneId}", ref result, ref error))
                            {
                                MessageBox.Show("Scene not found");
                                return false;
                            }

                            var sceneShapeId = (result as int[] ?? Array.Empty<int>()).DefaultIfEmpty(-1).First();
                            var sceneShape = sceneShapeFile.Shape[sceneShapeId];

                            return form.Shape.Intersects(sceneShape);
                        };

                        form.AfterShapeValid += (shape) =>
                        {
                            if (shape.numPoints == 1)
                            {
                                var point = shape.Point[0];
                                ship.X = point.x;
                                ship.Y = point.y;
                            }
                        };
                        break;
                    case Scene scene:
                        var gasShapefile = map.get_Shapefile(layersInfo.GasLayerHandle);
                        if (!gasShapefile.Table.Query($"[Id] = {scene.GasId}", ref result, ref error))
                        {
                            return;
                        }
                        var gasShapeId = (result as int[] ?? Array.Empty<int>()).DefaultIfEmpty(-1).First();
                        var gasShape = gasShapefile.Shape[gasShapeId];
                        var origin = gasShape.Point[0];

                        form.OnChangeParameters += (shapefileCloned, angle, length) =>
                        {
                            buildScene(shapefileCloned, ref form.Shape, origin, angle, length);
                            map.Redraw();
                        };

                        var coastShapefile = map.get_Shapefile(layersInfo.CoastLayerHandle);
                        string coastQuery = "[OBJECTID]=1";
                        Shape coast = null;
                        if (coastShapefile.Table.Query(coastQuery, ref result, ref error))
                        {
                            int coastShapeId = (result as int[] ?? Array.Empty<int>()).DefaultIfEmpty(-1).First();
                            coast = coastShapefile.Shape[coastShapeId];
                        }

                        form.ValidShape += (point, shape) =>
                        {
                            if (coast != null && !coast.Intersects(shape))
                            {
                                MessageBox.Show("Сцена побудована на материку. Побудуйте іншу сцену");
                                return false;
                            }

                            return shape.numPoints != 4;
                        };

                        var angleTextBox = form.Controls.OfType<TextBox>()
                            .FirstOrDefault(c => c.Name == "angle");
                        var lengthTextBox = form.Controls.OfType<TextBox>()
                            .FirstOrDefault(c => c.Name == "length");
                        form.AfterShapeValid += (shape) =>
                        {
                            scene.Area = shape.Area;
                            scene.Angle = TypeTools.Convert<double>(angleTextBox.Text);
                            scene.Side = TypeTools.Convert<double>(lengthTextBox.Text);
                        };
                        break;
                    case Route route:
                        var shipShapefile = map.get_Shapefile(layersInfo.ShipLayerHandle);
                        if (!shipShapefile.Table.Query($"[ShipId] = {route.ShipId}", ref result, ref error))
                        {
                            return;
                        }

                        int shipShapeId = (result as int[] ?? Array.Empty<int>()).DefaultIfEmpty(-1).First();
                        if (shipShapeId == -1)
                        {
                            return;
                        }

                        var sceneIdFieldIndex = shipShapefile.FieldIndexByName["SceneId"];
                        var sceneId = TypeTools.Convert<int>(shipShapefile.CellValue[sceneIdFieldIndex, shipShapeId]);

                        var sceneShapefile = map.get_Shapefile(layersInfo.SceneLayerHandle);
                        if (!sceneShapefile.Table.Query($"[SceneId] = {sceneId}", result, error))
                        {
                            return;
                        }

                        var sceneShapeId1 = (result as int[] ?? Array.Empty<int>()).DefaultIfEmpty(-1).First();
                        var sceneShape1 = sceneShapefile.Shape[sceneShapeId1];
                        if (sceneShape1 == null)
                        {
                            return;
                        }

                        form.ValidShape += (point, _) =>
                        {
                            var shape = new Shape();
                            if (!shape.Create(ShpfileType.SHP_POINT))
                            {
                                return false;
                            }
                            var pointIndex = 0;
                            shape.InsertPoint(point, ref pointIndex);

                            return sceneShape1.Intersects(shape);
                        };

                        form.OnMapMouseDown += (point) =>
                        {
                            route.Points.Add(new RoutePoint() 
                            {
                                RouteId = route.Id,
                                Id = route.Points.Count,
                                X = point.x,
                                Y = point.y
                            });
                        };
                        break;
                }

                if (!(form.Entity is Scene))
                {
                    form.HideAngleAndLength();
                }
            };


            if (form.Height < 500)
            {
                form.Height = 500;
            }
            else
            {
                map.Height = form.Height - 30;
            }

            var maxControlWidth = form.Controls.OfType<TextBox>()
                .Where(c => c.Name != "angle" && c.Name != "length").Max(c => c.Left + c.Width);

            if (maxControlWidth > map.Location.X)
            {
                form.MoveMapControls(maxControlWidth - map.Location.X + 10);
            }

            form.Width = map.Location.X + map.Width + 20;

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

            if (angle == 0 && sideLength == 0)
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

        private static void configureForm(Form form, object entity, EditMode editMode = EditMode.View)
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
