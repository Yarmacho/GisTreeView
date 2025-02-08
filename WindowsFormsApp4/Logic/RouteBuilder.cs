using Entities.Entities;
using MapWinGIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using WindowsFormsApp4.Initializers;

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
        }

        public double Length { get; set; }          // довжина човна в метрах
        public double MaxSpeed { get; set; }        // максимальна швидкість в м/с
        public double TurnRate { get; set; }        // максимальна швидкість повороту в радіанах/с
        public double Acceleration { get; set; }    // прискорення в м/с²
        public double Deceleration { get; set; }    // уповільнення в м/с²
        public double Width { get; set; }       // Ширина човна в метрах
        public double Weight { get; set; }      // Вага човна в кг
    }

    internal class RouteBuilder
    {
        private readonly ShipParameters _shipParameters;
        private readonly Image _battimetry;
        private readonly Initializers.Map _map;

        private List<RouteSegment> _segments = new List<RouteSegment>();

        public RouteBuilder(ShipParameters shipParameters, Image battimetry,
            Initializers.Map map)
        {
            _shipParameters = shipParameters;
            _battimetry = battimetry;
            _map = map;
        }


        public List<RoutePoint> CalculateRouteBetweenPoints(Point startPoint, Point endPoint, double distanceBetweenPoints = 100d)
        {
            var baseRoute = CreateBaseRoute(startPoint, endPoint);

            var optimizedRoute = OptimizeRoute(baseRoute);

            return optimizedRoute;
        }

        private List<RoutePoint> CreateBaseRoute(Point start, Point end)
        {
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
                route.Add(new RoutePoint(point));
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
            var optimized = new List<RoutePoint>();

            // Згладжування траєкторії за допомогою ковзного середнього
            int windowSize = 5;
            for (int i = 0; i < route.Count; i++)
            {
                var window = GetWindowPoints(route, i, windowSize);
                var smoothed = new RoutePoint(new Point
                {
                    x = window.Average(p => p.X),
                    y = window.Average(p => p.Y)
                });
                optimized.Add(smoothed);
            }

            return optimized;
        }

        private List<RoutePoint> GetWindowPoints(List<RoutePoint> route, int center, int size)
        {
            var window = new List<RoutePoint>();
            int radius = size / 2;

            for (int i = -radius; i <= radius; i++)
            {
                int index = center + i;
                if (index >= 0 && index < route.Count)
                {
                    window.Add(route[index]);
                }
            }

            return window;
        }

        private class RouteSegment
        {
            public Point Start { get; set; }
            public Point End { get; set; }
            public Vector Direction { get; set; }  // Напрямок в кінці сегмента
        }

        private class Vector
        {
            public double X { get; set; }
            public double Y { get; set; }

            public Vector Normalize()
            {
                double length = Math.Sqrt(X * X + Y * Y);
                return new Vector { X = X / length, Y = Y / length };
            }
        }
    }
}
