using AxMapWinGIS;
using MapWinGIS;
using System;
using System.Windows.Forms;
using WindowsFormsApp4.Extensions;

namespace WindowsFormsApp4.Initializers
{
    public class MapSynchronizer
    {
        private readonly AxMap _sourceMap;
        private readonly AxMap _targetMap;
        private double _lastSourceAspectRatio;
        private double _lastTargetAspectRatio;

        public event Action<Point> OnMouseMove;

        public MapSynchronizer(AxMap sourceMap, AxMap targetMap)
        {
            _sourceMap = sourceMap ?? throw new ArgumentNullException(nameof(sourceMap));
            _targetMap = targetMap ?? throw new ArgumentNullException(nameof(targetMap));

            UpdateAspectRatios();
            AttachEventHandlers();
        }

        private void UpdateAspectRatios()
        {
            _lastSourceAspectRatio = (double)_sourceMap.Width / _sourceMap.Height;
            _lastTargetAspectRatio = (double)_targetMap.Width / _targetMap.Height;
        }

        private void AttachEventHandlers()
        {
            _sourceMap.ExtentsChanged += OnExtendsChanged;
            _sourceMap.MouseMoveEvent += (s, e) =>
            {
                if (OnMouseMove == null)
                {
                    return;
                }

                var projX = 0d;
                var projY = 0d;
                _sourceMap.PixelToProj(e.x, e.y, ref projX, ref projY);

                var point = new Point() { x = projX, y = projY };
                OnMouseMove.CallAllSubsribers(point);
            };
        }

        private void OnExtendsChanged(object sender, EventArgs e)
        {
            SynchronizeMaps();
        }

        private void SynchronizeMaps()
        {
            var sourceExtents = _sourceMap.GeographicExtents;
            if (sourceExtents == null) { return; }

            // Розраховуємо центр вихідної карти
            double centerX = (sourceExtents.xMin + sourceExtents.xMax) / 2;
            double centerY = (sourceExtents.yMin + sourceExtents.yMax) / 2;

            // Розраховуємо ширину та висоту вихідної карти
            double sourceWidth = sourceExtents.xMax - sourceExtents.xMin;
            double sourceHeight = sourceExtents.yMax - sourceExtents.yMin;

            // Коригуємо розміри відповідно до співвідношення сторін цільової карти
            double targetWidth, targetHeight;
            if (_lastTargetAspectRatio > _lastSourceAspectRatio)
            {
                // Цільова карта ширша - підлаштовуємо ширину
                targetWidth = sourceHeight * _lastTargetAspectRatio;
                targetHeight = sourceHeight;
            }
            else
            {
                // Цільова карта вища - підлаштовуємо висоту
                targetWidth = sourceWidth;
                targetHeight = sourceWidth / _lastTargetAspectRatio;
            }


            var xMin = centerX - targetWidth / 2;
            var xMax = centerX + targetWidth / 2;
            var yMin = centerY - targetHeight / 2;
            var yMax = centerY + targetHeight / 2;

            // Встановлюємо нові екстенти для цільової карти
            var targetExtents = new Extents();
            targetExtents.SetBounds(xMin, yMin, sourceExtents.zMin, xMax, yMax, sourceExtents.zMax);

            _targetMap.SetGeographicExtents(targetExtents);
            _targetMap.ZoomPercent = _sourceMap.ZoomPercent;
        }

        public void Detach()
        {
            _sourceMap.ExtentsChanged -= OnExtendsChanged;
        }
    }

}
