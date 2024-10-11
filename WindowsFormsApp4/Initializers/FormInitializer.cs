using DynamicForms.Abstractions;
using MapWinGIS;
using System.IO;
using System;
using WindowsFormsApp4.Forms.Abstractions;

namespace WindowsFormsApp4.Initializers
{
    public static class FormInitializer
    {
        public static void TryAddDepthIndication<T>(this IEntityFormWithMapAndDepthLabel<T> form, Map map)
        {
            if (map.LayersInfo.BatimetryLayerHandle <= 0 || !map.SendMouseMove)
            {
                return;
            }

            var batimetry = map.Batimetry;
            if (batimetry is null)
            {
                return;
            }

            var activeBand = batimetry.ActiveBand;
            if (form.DepthLabel != null)
            {
                form.OnMouseMoveOnMap += (x, y) =>
                {
                    batimetry.ProjectionToImage(x, y, out int column, out int row);

                    var hasValue = activeBand.Value[column, row, out var depth];
                    form.DepthLabel.AutoSize = true;
                    form.DepthLabel.Text = hasValue ? $"Depth: {depth}" : string.Empty;
                };
            }
        }

        public static void ConfigureMouseDownEvent<T>(this IEntityFormWithMap<T> form)
        {
            form.Map.AxMap.SendMouseDown = true;
            form.Map.AxMap.MouseDownEvent += (s, e) =>
            {
                var projX = 0d;
                var projY = 0d;
                form.Map.PixelToProj(e.x, e.y, ref projX, ref projY);

                var point = new Point();
                point.x = projX;
                point.y = projY;

                form.CallMouseDownEvents(point);

                if (form.Map.CursorMode == tkCursorMode.cmAddShape)
                {
                    if (form.Shape == null)
                    {
                        createShape(form);
                    }

                    if (!form.CallValidateShapeEvents(point))
                    {
                        NotificationsManager.Popup("Invalid shape");
                    }
                    else
                    {
                        form.InsertPoint(point);

                        form.CallAfterValidShapeEvents();
                    }

                    form.Map.Redraw();
                }
            };
        }

        public static void ConfigureMouseMoveEvent<T>(this IEntityFormWithMap<T> form)
        {
            if (!form.Map.SendMouseMove)
            {
                return;
            }

            form.Map.AxMap.MouseMoveEvent += (s, e) =>
            {
                var projX = 0d;
                var projY = 0d;
                form.Map.PixelToProj(e.x, e.y, ref projX, ref projY);

                form.CallOnMouseMoveEvents(projX, projY);
            };
        }

        private static void createShape<T>(IEntityFormWithMap<T> form)
        {
            form.Shapefile.StartAppendMode();
            var shape = new Shape();
            shape.Create(form.Shapefile.ShapefileType);
            form.Shape = shape;

            var shapeIndex = 0;
            form.Shapefile.EditInsertShape(form.Shape, ref shapeIndex);
            form.Shapefile.StopAppendMode();
        }

        public static Shapefile CreateTempShapefile<T>(this IEntityFormWithMap<T> form, Shapefile originalShapefile)
        {
            var guid = Guid.NewGuid();
            var directory = Path.Combine(Path.GetDirectoryName(originalShapefile.Filename), "Temp");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var shapefile = originalShapefile.Clone();
            if (shapefile.CreateNew(Path.Combine(directory, $"{guid}.shp"), originalShapefile.ShapefileType))
            {
                var layer = form.Map.AxMap.AddLayer(shapefile, true);
                form.Map.AxMap.MoveLayerTop(layer);
            }

            return shapefile;
        }
    }
}
