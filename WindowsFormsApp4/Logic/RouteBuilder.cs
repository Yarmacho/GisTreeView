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
        private const double MinTurningAngle = 10.0; // мінімальний кут в градусах для визначення точки повороту
        private const int PointsPerTurn = 8; // кількість точок для згладжування повороту
        private const double AccelerationZone = 0.3;  // перші 30% шляху
        private const double DecelerationZone = 0.7;  // останні 30% шляху


        public RouteBuilder(ShipParameters shipParameters, Image battimetry,
            Initializers.Map map)
        {
            _shipParameters = shipParameters;
            _battimetry = battimetry;
            _map = map;
        }

        public List<RoutePoint> CalculateRouteBetweenPoints(Point startPoint, Point endPoint, double distanceBetweenPoints = 100d)
        {
            return CreateBaseRoute(startPoint, endPoint);
        }

        private List<RoutePoint> CreateBaseRoute(Point start, Point end)
        {
            var route = new List<RoutePoint>();

            // Розрахунок відстані між точками
            double distance = _map.AxMap.GeodesicDistance(start.x, start.y, end.x, end.y);

            // Визначаємо кількість проміжних точок (кожні 100 метрів)
            int numberOfPoints = Math.Max((int)(distance / 100), 10);

            for (int i = 0; i <= numberOfPoints; i++)
            {
                double t = i / (double)numberOfPoints;

                // Використовуємо сферичну інтерполяцію для більшої точності
                var point = InterpolateSpherical(start, end, t);
                route.Add(new RoutePoint(point));
            }

            return route;
        }

        private Point InterpolateSpherical(Point start, Point end, double t)
        {
            // Створюємо одну контрольну точку для квадратичної кривої Безьє
            var controlPoint = GenerateControlPoint(start, end, 0.5); // Контрольна точка на середині шляху

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

            return new Point { x = x, y = y };
        }

        private Point GenerateControlPoint(Point start, Point end, double factor)
        {
            // Розраховуємо відстань між точками
            double distance = _map.AxMap.GeodesicDistance(start.x, start.y, end.x, end.y);

            // Визначаємо зміщення для контрольної точки (адаптивне зміщення)
            double deviation = distance * Math.Sin(factor * Math.PI) * 0.2; // Синусоїдальне зміщення

            // Створюємо вектор напрямку
            double dx = end.x - start.x;
            double dy = end.y - start.y;

            // Нормалізуємо вектор
            double length = Math.Sqrt(dx * dx + dy * dy);
            double nx = -dy / length; // Перпендикулярний вектор
            double ny = dx / length;

            // Визначаємо напрямок відхилення на основі фактору та відстані
            double sign = Math.Cos(factor * 2 * Math.PI); // Плавна зміна напрямку

            // Обчислюємо проміжну позицію з урахуванням кривизни Землі
            double midX = start.x + dx * factor;
            double midY = start.y + dy * factor;

            // Додаємо відхилення для створення більш природної кривої
            return new Point
            {
                x = midX + nx * deviation * sign,
                y = midY + ny * deviation * sign
            };
        }
    }
}
