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

    public class RoutePoint
    {
        public RoutePoint(Point point)
        {
            X = point.x;
            Y = point.y;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Speed { get; set; }           // швидкість в даній точці (м/с)
        public double Heading { get; set; }         // напрямок руху в радіанах
        public double Depth { get; set; }           // глибина в точці
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
            var result = new List<RoutePoint>();

            // Відстань між точками в метрах
            double distance = _map.AxMap.GeodesicDistance(startPoint.x, startPoint.y,
                endPoint.x, endPoint.y);

            // Визначаємо кількість проміжних точок
            // Припустимо, що хочемо мати точку кожні 100 метрів
            int numberOfPoints = (int)(distance / 100) + 1;

            for (int i = 0; i <= numberOfPoints; i++)
            {
                // Коефіцієнт інтерполяції від 0 до 1
                double t = i / (double)numberOfPoints;

                // Лінійна інтерполяція між початковою і кінцевою точкою в проекції карти
                var point = new Point() 
                {
                    x = startPoint.x + (endPoint.x - startPoint.x) * t,
                        y = startPoint.y + (endPoint.y - startPoint.y) * t
                };

                result.Add(new RoutePoint(point));
            }

            return result;
        }
    }
}
