using Entities.Entities;
using MapWinGIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

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
        public bool IsValid { get; set; }           // чи прохідна точка
        public double Depth { get; set; }           // глибина в точці
    }


    internal class RouteBuilder
    {
        private readonly ShipParameters _shipParameters;
        private readonly Image _battimetry;
        private const double MinTurningAngle = 10.0; // мінімальний кут в градусах для визначення точки повороту
        private const int PointsPerTurn = 8; // кількість точок для згладжування повороту
        private const double AccelerationZone = 0.3;  // перші 30% шляху
        private const double DecelerationZone = 0.7;  // останні 30% шляху


        public RouteBuilder(ShipParameters shipParameters, Image battimetry)
        {
            _shipParameters = shipParameters;
            _battimetry = battimetry;
        }

        public List<RoutePoint> CalculateRoutePoints(Point startPoint, Point endPoint)
        {
            var result = new List<RoutePoint>()
            {
                new RoutePoint(startPoint), 
                new RoutePoint(endPoint), 
            };

            return result;

            // Розрахунок загальної відстані та напрямку
            var distance = calculateDistance(startPoint, endPoint);
            var heading = calculateHeading(startPoint, endPoint);

            // Розрахунок мінімального радіусу повороту
            double turnRadius = calculateTurnRadius();

            if (distance <= turnRadius * 2)
            {
                // Якщо відстань менша за діаметр повороту - створюємо дугу
                result.AddRange(calculateArcRoute(startPoint, endPoint, turnRadius));
            }
            else
            {
                // Створюємо маршрут з прямолінійних ділянок та поворотів
                result.AddRange(calculateStraightRoute(startPoint, endPoint, turnRadius));
            }

            return result;
        }

        private List<RoutePoint> calculateStraightRoute(Point startPoint, Point endPoint, double turnRadius)
        {
            var result = new List<RoutePoint>();

            // Визначаємо точки входу і виходу з поворотів
            var (entryPoint, exitPoint) = calculateTurnPoints(startPoint, endPoint, turnRadius);

            // Додаємо початкову пряму ділянку
            result.AddRange(calculateLinearSection(startPoint, entryPoint));

            // Додаємо поворот
            result.AddRange(calculateArcRoute(entryPoint, exitPoint, turnRadius));

            // Додаємо кінцеву пряму ділянку
            result.AddRange(calculateLinearSection(exitPoint, endPoint));

            return result;
        }


        private List<RoutePoint> calculateArcRoute(Point startPoint, Point endPoint, double radius)
        {
            var result = new List<RoutePoint>();

            // Знаходимо центр дуги
            var center = calculateArcCenter(startPoint, endPoint, radius);

            // Розраховуємо кути для початкової та кінцевої точок відносно центру
            double startAngle = calculateHeading(center, startPoint);
            double endAngle = calculateHeading(center, endPoint);

            // Визначаємо напрямок повороту
            if (Math.Abs(endAngle - startAngle) > Math.PI)
            {
                if (endAngle > startAngle)
                {
                    startAngle += 2 * Math.PI;
                }
                else
                {
                    endAngle += 2 * Math.PI;
                }
            }

            // Створюємо точки дуги
            for (int i = 0; i <= PointsPerTurn; i++)
            {
                double t = (double)i / PointsPerTurn;
                double angle = startAngle + (endAngle - startAngle) * t;

                var point = new Point();
                point.Set(
                    center.x + radius * Math.Cos(angle),
                    center.y + radius * Math.Sin(angle)
                );

                //// Розраховуємо швидкість на повороті
                //double speed = сalculateSpeedForTurn(radius, angle - startAngle);

                result.Add(createRoutePoint(point, 0d, angle + Math.PI / 2));
            }

            return result;
        }

        private Point calculateArcCenter(Point start, Point end, double radius)
        {
            // Знаходимо середню точку між початковою і кінцевою
            double midX = (start.x + end.x) / 2;
            double midY = (start.y + end.y) / 2;

            // Розраховуємо відстань між точками
            double distance = calculateDistance(start, end);

            // Знаходимо висоту трикутника для центру дуги
            double height = Math.Sqrt(radius * radius - Math.Pow(distance / 2, 2));

            // Розраховуємо напрямок перпендикуляру
            double angle = calculateHeading(start, end) + Math.PI / 2;

            var center = new Point();
            center.Set(
                midX + height * Math.Cos(angle),
                midY + height * Math.Sin(angle)
            );

            return center;
        }


        private RoutePoint createRoutePoint(Point point, double speed, double heading)
        {
            var depth = getDepthFromBathymetry(point);

            return new RoutePoint(point)
            {
                Speed = speed,
                Heading = heading,
                IsValid = validatePoint(point),
                Depth = depth
            };
        }

        private double calculateSpeedForPosition(double t, double totalDistance)
        {
            if (t < AccelerationZone)
            {
                // Зона прискорення
                return Math.Min(_shipParameters.MaxSpeed,
                              Math.Sqrt(2 * _shipParameters.Acceleration * t * totalDistance));
            }

            if (t > DecelerationZone)
            {
                // Зона гальмування
                double remainingT = 1 - t;
                return Math.Min(_shipParameters.MaxSpeed,
                              Math.Sqrt(2 * _shipParameters.Deceleration * remainingT * totalDistance));
            }

            // Зона постійної швидкості
            return _shipParameters.MaxSpeed;
        }

        private bool validatePoint(Point point)
        {
            // Перевірка чи точка знаходиться у воді
            var depthAtPoint = getDepthFromBathymetry(point);
            return depthAtPoint < 0;
        }

        private double getDepthFromBathymetry(Point point)
        {
            var band = _battimetry.ActiveBand == null
                ? _battimetry.Band[1]
                : _battimetry.ActiveBand;

            _battimetry.ProjectionToImage(point.x, point.y, out var column, out var row);

            return band.Value[column, row, out var battimetry] ? battimetry : 0; // Заглушка
        }

        /*
         Цей метод реалізує формулу гаверсинусів (Haversine formula)
         для обчислення відстані між двома точками на сфері
        */
        private double calculateDistance(Point point1, Point point2)
        {
            const double EarthRadius = 6371e3;
            var φ1 = point1.y * Math.PI / 180;
            var φ2 = point2.y * Math.PI / 180;
            var Δφ = (point2.y - point1.y) * Math.PI / 180;
            var Δλ = (point2.x - point1.x) * Math.PI / 180;

            var a = Math.Sin(Δφ / 2) * Math.Sin(Δφ / 2) +
                    Math.Cos(φ1) * Math.Cos(φ2) *
                    Math.Sin(Δλ / 2) * Math.Sin(Δλ / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadius * c;
        }

        //private List<RoutePoint> calculateTurnPoints(Point startPoint, Point turnPoint, Point endPoint)
        //{
        //    var turnPoints = new List<RoutePoint>();
        //    if (endPoint == null) return turnPoints;

        //    // Розрахунок радіусу повороту на основі швидкості та характеристик човна
        //    double turnRadius = calculateTurnRadius();

        //    // Знаходимо центр дуги повороту
        //    var center = calculateTurnCenter(startPoint, turnPoint, endPoint, turnRadius);

        //    // Розраховуємо точки дуги
        //    double startAngle = calculateHeading(center, startPoint);
        //    double endAngle = calculateHeading(center, endPoint);

        //    // Додаємо проміжні точки для плавного повороту
        //    for (int i = 0; i <= PointsPerTurn; i++)
        //    {
        //        double t = (double)i / PointsPerTurn;
        //        double currentAngle = startAngle + (endAngle - startAngle) * t;

        //        var pointX = center.x + turnRadius * Math.Cos(currentAngle);
        //        var pointY = center.y + turnRadius * Math.Sin(currentAngle);

        //        turnPoints.Add();
        //    }

        //    return turnPoints;
        //}

        private List<RoutePoint> calculateLinearSection(Point start, Point end)
        {
            var result = new List<RoutePoint>();

            double distance = calculateDistance(start, end);
            double heading = calculateHeading(start, end);

            // Визначаємо кількість проміжних точок
            int numberOfPoints = Math.Max(2, (int)(distance / (_shipParameters.Length * 2)));

            for (int i = 0; i <= numberOfPoints; i++)
            {
                double t = (double)i / numberOfPoints;

                var point = new Point();
                point.Set(
                    start.x + (end.x - start.x) * t,
                    start.y + (end.y - start.y) * t
                );

                double speed = calculateSpeedForPosition(t, distance);

                result.Add(createRoutePoint(point, speed, heading));
            }

            return result;
        }

        private (Point entryPoint, Point exitPoint) calculateTurnPoints(Point start, Point end, double radius)
        {
            double heading = calculateHeading(start, end);
            double distance = calculateDistance(start, end);

            // Розраховуємо точки входу і виходу з повороту
            var entryPoint = new Point();
            entryPoint.Set(
                start.x + radius * Math.Cos(heading),
                start.y + radius * Math.Sin(heading)
            );

            var exitPoint = new Point();
            exitPoint.Set(
                end.x - radius * Math.Cos(heading),
                end.y - radius * Math.Sin(heading)
            );

            return (entryPoint, exitPoint);
        }

        private double calculateTurnRadius()
        {
            // Мінімальний радіус повороту на основі швидкості та максимальної швидкості повороту
            return Math.Max(
                _shipParameters.Length * 2, // мінімальний радіус як подвійна довжина човна
                _shipParameters.MaxSpeed / _shipParameters.TurnRate // радіус на основі швидкості
            );
        }

        private Point calculateTurnCenter(Point start, Point turn, Point end, double radius)
        {
            // Розрахунок центру дуги повороту
            double bearing1 = calculateHeading(start, turn);
            double bearing2 = calculateHeading(turn, end);

            // Знаходимо точку на перпендикулярі до першого відрізка
            double centerX = turn.x + radius * Math.Cos(bearing1 + Math.PI / 2);
            double centerY = turn.y + radius * Math.Sin(bearing1 + Math.PI / 2);

            var point = new Point();
            point.Set(centerX, centerY);

            return point;
        }

        // Розрахунок напрямку руху в радіанах
        private double calculateHeading(Point from, Point to)
        {
            return Math.Atan2(to.y - from.y, to.x - from.x);
        }
    }
}
