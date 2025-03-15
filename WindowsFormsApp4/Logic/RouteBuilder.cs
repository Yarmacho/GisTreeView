using Entities.Entities;
using MapWinGIS;
using System;
using System.Collections.Generic;
using System.Linq;
using Vector = Entities.Dtos.Vector;

namespace WindowsFormsApp4.Logic
{
    public class ShipParameters
    {
        public ShipParameters(Ship ship)
        {
            Length = ship.Lenght;
            TurnRate = ship.TurnRate;
            MaxSpeed = ship.MaxSpeed;
            Acceleration = ship.Acceleration;
            Deceleration = ship.Deceleration;
            MinDepth = 10;
        }

        public double Length { get; set; }          // довжина човна в метрах
        public double MaxSpeed { get; set; }        // максимальна швидкість в м/с
        public double TurnRate { get; set; }        // максимальна швидкість повороту в радіанах/с
        public double Acceleration { get; set; }    // прискорення в м/с²
        public double Deceleration { get; set; }    // уповільнення в м/с²
        public double Width { get; set; }       // Ширина човна в метрах
        public double Weight { get; set; }      // Вага човна в кг
        public double MinDepth { get; set; }
    }

    internal class RouteBuilder
    {
        private readonly ShipParameters _shipParameters;
        private readonly Image _battimetry;
        private readonly Initializers.Map _map;
        private readonly Shape _scene;
        private List<RouteSegment> _segments = new List<RouteSegment>();

        public RouteBuilder(ShipParameters shipParameters, Image battimetry,
            Initializers.Map map, Shape scene)
        {
            _shipParameters = shipParameters;
            _battimetry = battimetry;
            _map = map;
            _scene = scene;
        }


        public List<RoutePoint> CalculateRouteBetweenPoints(Point startPoint, Point endPoint)
        {
            var baseRoute = CreateBaseRoute(startPoint, endPoint);

            var optimizedRoute = OptimizeRoute(baseRoute);

            return optimizedRoute;
        }

        private List<RoutePoint> CreateBaseRoute(Point start, Point end)
        {
            if (!ValidatePoint(start))
            {
                NotificationsManager.Popup("Початкова точка маршруту не відповідає вимогам безпеки");
                return new List<RoutePoint>();
            }

            if (!ValidatePoint(end))
            {
                NotificationsManager.Popup("Кінцева точка маршруту не відповідає вимогам безпеки");
                return new List<RoutePoint>();
            }

            var route = new List<RoutePoint>();

            // Розрахунок відстані між точками
            double distance = _map.AxMap.GeodesicDistance(start.x, start.y, end.x, end.y);

            // Визначаємо кількість проміжних точок (кожні 100 метрів)
            int numberOfPoints = Math.Max((int)(distance / 10), 10);

            var lastSegment = _segments.LastOrDefault();

            var lastDirection = lastSegment?.Direction;
            for (int i = 0; i <= numberOfPoints; i++)
            {
                double t = i / (double)numberOfPoints;

                // Використовуємо сферичну інтерполяцію для більшої точності
                var point = InterpolateSpherical(start, end, t, lastDirection);
                var routePoint = new RoutePoint(point);

                route.Add(routePoint);
            }

            return route;
        }

        private Point InterpolateSpherical(Point start, Point end, double t, Vector previousDirection = null)
        {
            // Розраховуємо напрямок поточного сегмента
            var currentDirection = new Vector
            {
                X = end.x - start.x,
                Y = end.y - start.y
            };

            // Якщо є попередній напрямок, використовуємо його для створення плавного переходу
            var controlPoint = GenerateControlPoint(start, end, previousDirection ?? currentDirection);

            // Застосовуємо формулу квадратичної кривої Безьє
            double oneMinusT = 1 - t;
            double oneMinusTSquared = oneMinusT * oneMinusT;
            double tSquared = t * t;

            // Обчислюємо координати точки на кривій
            double x = oneMinusTSquared * start.x +
                       2 * oneMinusT * t * controlPoint.x +
                       tSquared * end.x;

            double y = oneMinusTSquared * start.y +
                       2 * oneMinusT * t * controlPoint.y +
                       tSquared * end.y;

            // Зберігаємо інформацію про сегмент для наступних обчислень
            if (t == 1.0)
            {
                var endDirection = new Vector
                {
                    X = end.x - controlPoint.x,
                    Y = end.y - controlPoint.y
                };

                _segments.Add(new RouteSegment
                {
                    Start = start,
                    End = end,
                    Direction = endDirection.Normalize()
                });
            }

            return new Point { x = x, y = y };
        }

        private Point GenerateControlPoint(Point start, Point end, Vector direction)
        {
            // Розраховуємо відстань між точками
            double distance = _map.AxMap.GeodesicDistance(start.x, start.y, end.x, end.y);

            // Нормалізуємо напрямок
            var normalizedDirection = direction.Normalize();

            // Визначаємо позицію контрольної точки вздовж напрямку
            double controlDistance = distance * 0.5; // Половина відстані

            return new Point
            {
                x = start.x + normalizedDirection.X * controlDistance,
                y = start.y + normalizedDirection.Y * controlDistance
            };
        }
        private List<RoutePoint> OptimizeRoute(List<RoutePoint> route)
        {
            var optimizer = new TurnOptimizer(_shipParameters);
            var optimized = new List<RoutePoint>();

            // Якщо маршрут занадто короткий для оптимізації
            if (route.Count < 3)
            {
                return route;
            }

            // Додаємо першу точку
            optimized.Add(route[0]);

            // Оптимізуємо кожен поворот
            for (int i = 1; i < route.Count - 1; i++)
            {
                var turnPoints = optimizer.OptimizeTurn(
                    route[i - 1],
                    route[i],
                    route[i + 1]
                );

                // Додаємо всі точки крім першої та останньої,
                // щоб уникнути дублювання
                for (int j = 1; j < turnPoints.Count - 1; j++)
                {
                    optimized.Add(turnPoints[j]);
                }
            }

            // Додаємо останню точку
            optimized.Add(route[route.Count - 1]);

            return optimized;
        }

        private bool ValidatePoint(Point point)
        {
            if (!IsPointInScene(point))
            {
                return false;
            }

            if (!IsDepthSufficient(point))
            {
                return false;
            }

            return true;
        }

        private bool IsPointInScene(Point point)
        {
            if (_scene == null)
            {
                return true;
            }

            var pointShape = new Shape();
            pointShape.Create(ShpfileType.SHP_POINT);
            var pointIndex = pointShape.numPoints;
            pointShape.InsertPoint(point, pointIndex);

            return _scene.Intersects(pointShape);
        }

        private double calculateAngleBetweenVectors(Vector v1, Vector v2)
        {
            // Нормалізація векторів
            var v1Normalized = v1.Normalize();
            var v2Normalized = v2.Normalize();

            // Розрахунок кута через скалярний добуток
            var dotProduct = v1Normalized.X * v2Normalized.X + v1Normalized.Y * v2Normalized.Y;

            // Обмеження значення для уникнення помилок через неточності обчислень
            dotProduct = Math.Max(-1.0, Math.Min(1.0, dotProduct));

            return Math.Acos(dotProduct);
        }

        private bool IsDepthSufficient(Point point)
        {
            if (_battimetry == null)
            {
                return true;
            }

            try
            {
                // Перетворюємо географічні координати в координати растру
                _battimetry.ProjectionToImage(point.x, point.y, out var column, out var row);

                var band = _battimetry.ActiveBand ?? _battimetry.Band[1];

                var depth = 0d;
                var hasValue = band != null && band.Value[column, row, out depth];

                // Перевіряємо чи глибина достатня
                // Враховуємо, що значення глибини може бути від'ємним
                return !hasValue || (depth < 0 && Math.Abs(depth) >= _shipParameters.MinDepth);
            }
            catch (Exception)
            {
                // Якщо виникла помилка при отриманні значення глибини,
                // вважаємо точку небезпечною
                return false;
            }
        }

        private Point FindSafePoint(Point unsafePoint)
        {
            try
            {
                if (_scene == null)
                {
                    return null;
                }

                var extents = _scene.Extents;

                // Початковий радіус пошуку - 1% від розміру сцени
                double sceneSize = Math.Max(extents.xMax - extents.xMin, extents.yMax - extents.yMin);
                double initialRadius = sceneSize * 0.01;
                double maxRadius = sceneSize * 0.1; // Максимальний радіус - 10% від розміру сцени
                double currentRadius = initialRadius;

                const int maxAttempts = 16; // Кількість напрямків пошуку

                while (currentRadius <= maxRadius)
                {
                    for (int i = 0; i < maxAttempts; i++)
                    {
                        double angle = (2 * Math.PI * i) / maxAttempts;
                        double dx = currentRadius * Math.Cos(angle);
                        double dy = currentRadius * Math.Sin(angle);

                        // Перевіряємо точку в поточному напрямку
                        var testPoint = new Point
                        {
                            x = unsafePoint.x + dx,
                            y = unsafePoint.y + dy
                        };

                        if (ValidatePoint(testPoint))
                        {
                            return testPoint;
                        }

                        // Перевіряємо точку в протилежному напрямку
                        testPoint = new Point
                        {
                            x = unsafePoint.x - dx,
                            y = unsafePoint.y - dy
                        };

                        if (ValidatePoint(testPoint))
                        {
                            return testPoint;
                        }
                    }

                    currentRadius *= 1.5;
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Помилка при пошуку безпечної точки: {ex.Message}");
                return null;
            }
        }

        private class RouteSegment
        {
            public Point Start { get; set; }
            public Point End { get; set; }
            public Vector Direction { get; set; }  // Напрямок в кінці сегмента
        }
    }
}
