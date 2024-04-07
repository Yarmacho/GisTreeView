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
using Entities;
using Expression = System.Linq.Expressions.Expression;
using Interfaces.Database.Repositories;
using WindowsFormsApp4.ShapeConverters;
using Microsoft.Extensions.DependencyInjection;
using GeoDatabase.ORM;
using System.Drawing;
using Point = MapWinGIS.Point;

namespace DynamicForms.Factories
{
    public static class FormFactory
    {
        public static IServiceProvider ServiceProvider { get; set; }

        public static IEntityFormWithMap CreateFormWithMap(object entity, Shapefile shapefile, EditMode editMode = EditMode.View)
        {
            var form = new EntityFormWithMap(entity) 
            {
                Text = getFormCaption(entity, editMode)
            };

            var entityIdProperty = entity.GetType().GetProperty("Id");
            var isDictionaryEntity = entityIdProperty != null && TypeTools.IsDerivedFrom(entity, typeof(DictionaryEntity<>)
                    .MakeGenericType(entityIdProperty.PropertyType));

            configureForm(form, entity, editMode);
            configureMap(form, shapefile);
            configureButtons(form, editMode);

            if (isDictionaryEntity)
            {
                foreach (var text in form.PropertiesTab.Controls.OfType<TextBox>())
                {
                    text.Enabled = false;
                }

                form.OnSelectFromDictionary += () =>
                {
                    var selectedRow = callDictionaryForm(entity, shapefile);
                    if (selectedRow == null)
                    {
                        return;
                    }

                    var point = new Point();
                    foreach (var property in selectedRow.GetType().GetProperties())
                    {
                        var control = form.PropertiesTab.Controls.OfType<TextBox>()
                            .FirstOrDefault(c => c.Name == property.Name);
                        if (control == null)
                        {
                            continue;
                        }

                        control.Text = property.GetValue(selectedRow)?.ToString();

                        if (property.Name.Equals("x", StringComparison.InvariantCultureIgnoreCase))
                        {
                            point.x = TypeTools.Convert<double>(property.GetValue(selectedRow));
                        }
                        else if (property.Name.Equals("y", StringComparison.InvariantCultureIgnoreCase))
                        {
                            point.y = TypeTools.Convert<double>(property.GetValue(selectedRow));
                        }
                    }

                    var shape = form.Shape ?? form.CreateShape();
                    form.InsertPoint(point);
                    form.Entity = selectedRow;
                };
            }
            else
            {
                form.TabControl.Location = new System.Drawing.Point(form.TabControl.Location.X, form.Map.Location.Y);

                form.Controls.RemoveByKey("customEntity");
                form.Controls.RemoveByKey("selectFromDict");
            }

            return form;
        }

        private static object callDictionaryForm(object entity, Shapefile shapefile)
        {
            var dictForm = new DictionaryForm();

            var entityType = entity.GetType();
            var idType = entity.GetType().GetProperty("Id").PropertyType;

            var serviceProviderGetMethod = typeof(IServiceProvider).GetMethod("GetService");
            var serviceProvider = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");
            var repositoryType = typeof(IRepository<,>).MakeGenericType(entityType, idType);
            var repository = Expression.Convert(Expression.Call(serviceProvider, serviceProviderGetMethod,
                Expression.Constant(repositoryType)), repositoryType);
            var converterType = typeof(IShapeEntityConverter<>).MakeGenericType(entityType);
            var converter = Expression.Convert(Expression.Call(serviceProvider, serviceProviderGetMethod,
                Expression.Constant(converterType)), converterType);
            var form = Expression.Constant(dictForm);
            var shfile = Expression.Convert(Expression.Constant(shapefile), typeof(Shapefile));

            var initMethod = typeof(DictionaryForm).GetMethod("Init").MakeGenericMethod(entityType, idType);

            var body = Expression.Call(form, initMethod, repository, shfile, converter);

            Expression.Lambda<Action<IServiceProvider>>(body, serviceProvider)
                .Compile()?.Invoke(ServiceProvider);

            if (dictForm.ShowDialog() != DialogResult.OK)
            {
                return null;
            }

            return dictForm.GetSelectedRecord();
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
                map.set_ShapeLayerFillTransparency(layersInfo.SceneLayerHandle, 0.2f);
                MapDesigner.ConnectShipsWithGases(map);

                var layerCheckBoxTop = 0;
                foreach (var layer in layersInfo)
                {
                    var checkBox = new CheckBox()
                    {
                        Text = layer.Key,
                        Checked = true
                    };
                    checkBox.Top = layerCheckBoxTop;
                    layerCheckBoxTop += 20;
                    checkBox.CheckedChanged += (s1, e1) =>
                    {
                        map.set_LayerVisible(layersInfo[checkBox.Text], checkBox.Checked);
                    };
                    form.LayersTab.Controls.Add(checkBox);
                }

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

                        form.ValidShape += (point, _) =>
                        {
                            batimetry.ProjectionToImage(point.x, point.y, out int column, out int row);
                            if (!band.Value[column, row, out var depth])
                            {
                                MessageBox.Show("Depth undefined");
                                return false;
                            }

                            if (depth >= 0)
                            {
                                MessageBox.Show("Depth must be neggative");
                                return false;
                            }

                            return true;
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
                        var context = ServiceProvider.GetRequiredService<GeoDbContext>();
                        var shipScene = context.Set<Scene>().FirstOrDefault(sc => sc.Id == ship.SceneId);
                        if (shipScene is null)
                        {

                            break;
                        }
                        map.ZoomToShape(layersInfo.SceneLayerHandle, context.ChangeTracker.GetShapeIndex(shipScene));

                        var shipGas = context.Set<Gas>().FirstOrDefault(g => g.Id == shipScene.GasId);
                        if (shipGas is null)
                        {
                            break;
                        }

                        form.ValidShape += (point, _) =>
                        {
                            if (shipScene == null)
                            {
                                MessageBox.Show("Scene not found");
                                return false;
                            }

                            var shape = new Shape();
                            if (!shape.Create(ShpfileType.SHP_POINT))
                            {
                                return false;
                            }
                            var pointIndex = 0;
                            shape.InsertPoint(point, ref pointIndex);

                            return shape.Intersects(shipScene.Shape);
                        };

                        var gasPoint = shipGas.Shape.Point[0];
                        int drawingLayerHandle = -1;
                        form.AfterShapeValid += (shape) =>
                        {
                            if (shape.numPoints == 1)
                            {
                                var point = shape.Point[0];
                                ship.X = point.x;
                                ship.Y = point.y;

                                if (drawingLayerHandle != -1)
                                {
                                    map.ClearDrawing(drawingLayerHandle);
                                }
                                drawingLayerHandle = map.NewDrawing(tkDrawReferenceList.dlSpatiallyReferencedList);
                                map.DrawLineEx(drawingLayerHandle, gasPoint.x, gasPoint.y, ship.X, ship.Y, 2, (uint)Color.Red.ToArgb());
                            }
                        };
                        break;
                    case Scene scene:
                        var sceneGas = ServiceProvider.GetRequiredService<GeoDbContext>()
                            .Set<Gas>().FirstOrDefault(g => g.Id == scene.GasId);
                        var origin = sceneGas.Shape.Point[0];

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
                            //if (coast != null && !coast.Intersects(shape))
                            //{
                            //    MessageBox.Show("Сцена побудована на материку. Побудуйте іншу сцену");
                            //    return false;
                            //}

                            return shape.numPoints != 4;
                        };

                        form.AfterShapeValid += (shape) =>
                        {
                            scene.Area = shape.Area;
                            scene.Angle = TypeTools.Convert<double>(form.Angle.Text);
                            scene.Side = TypeTools.Convert<double>(form.Length.Text);
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

                        map.ZoomToShape(layersInfo.SceneLayerHandle, sceneShapeId1);
                        form.ValidShape += (point, _) =>
                        {
                            var shape = new Shape();
                            if (!shape.Create(ShpfileType.SHP_POINT))
                            {
                                return false;
                            }
                            var pointIndex = 0;
                            shape.InsertPoint(point, ref pointIndex);

                            return shape.Intersects(sceneShape1);
                        };

                        form.AfterShapeValid += (shape) =>
                        {
                            route.Points.Add(new RoutePoint() 
                            {
                                RouteId = route.Id,
                                Id = route.Points.Count,
                                X = shape.Point[0].x,
                                Y = shape.Point[0].y
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
                .Where(c => c.Name != form.Length.Name && c.Name != form.Angle.Name)
                .Select(c => c.Left + c.Width)
                .DefaultIfEmpty(map.Location.X)
                .Max();

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

        private static void configureForm(Form form, object entity, EditMode editMode)
        {
            form.SuspendLayout();
            int usedHeight = 5;

            var formControls = form is EntityFormWithMap formWithMap ? formWithMap.PropertiesTab.Controls : form.Controls;

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
                formControls.Add(label);

                maxLabelWidth = Math.Max(maxLabelWidth, label.Width);

                TextBox textBox = new TextBox();
                textBox.Text = property.GetValue(entity)?.ToString();
                textBox.Top = usedHeight + 5;
                textBox.Name = property.Name;
                textBox.Width = 115;
                textBox.Enabled = (displayAttribute?.Enabled ?? true) && editMode != EditMode.View && editMode != EditMode.Delete;
                textBox.TextChanged += (s, e) =>
                {
                    property.SetValue(entity, TypeTools.Convert(textBox.Text, property.PropertyType));
                };
                controls.Add(textBox);

                formControls.Add(textBox);

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
