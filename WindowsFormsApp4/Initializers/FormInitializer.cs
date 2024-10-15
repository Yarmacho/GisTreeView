using DynamicForms.Abstractions;
using MapWinGIS;
using System.IO;
using System;
using WindowsFormsApp4.Forms.Abstractions;
using Entities;
using Microsoft.Extensions.DependencyInjection;
using GeoDatabase.ORM;
using GeoDatabase.ORM.Set.Extensions;

namespace WindowsFormsApp4.Initializers
{
    public static class FormInitializer
    {
        public static void TryAddDepthIndication(this IEntityFormWithMapAndDepthLabel form)
        {
            if (form.Map.LayersInfo.BatimetryLayerHandle <= 0 || !form.Map.SendMouseMove)
            {
                return;
            }

            var batimetry = form.Map.Batimetry;
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
                        form.СreateShape();
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

        public static void ConfigureSaveShapeOnFormClosed<T, TId>(this IEntityFormWithMap<T> form)
            where T : EntityBase<TId>, new()
        {
            form.OnEntityFormClosed += () =>
            {
                if (form.Entity is null) 
                {
                    throw new Exception("Entity not initialized");
                }

                if (form.Shape is null || !form.Shape.IsValid)
                {
                    throw new Exception("Shape is not valid");
                }

                var context = Program.ServiceProvider
                    .GetRequiredService<GeoDbContext>();

                var set = context.Set<T>();
                if (set.Any(e => e.Id.Equals(form.Entity.Id)))
                {
                    set.Update(form.Entity, form.Shape);
                }
                else
                {
                    set.Add(form.Entity, form.Shape);
                }

                context.SaveChanges();
            };
        }

        public static void СreateShape<T>(this IEntityFormWithMap<T> form)
        {
            form.Shapefile.StartAppendMode();
            var shape = new Shape();
            shape.Create(form.Shapefile.ShapefileType);

            form.Shape = shape;

            var shapeIndex = 0;
            form.Shapefile.EditInsertShape(form.Shape, ref shapeIndex);
            form.Shapefile.StopAppendMode();
        }

        public static Shapefile CreateTempShapefile<T>(this IEntityFormWithMap<T> form, Shapefile originalShapefile, float transparency = 1f)
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
                form.Map.AxMap.set_ShapeLayerFillTransparency(layer, transparency);
                form.Map.AxMap.MoveLayerTop(layer);
            }

            return shapefile;
        }
    }
}
