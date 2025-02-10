using Entities.Dtos;
using Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp4.Logic
{
    public class TurnOptimizer
    {
        private readonly ShipParameters _shipParameters;
        private readonly double _minTurnRadius;

        public TurnOptimizer(ShipParameters shipParameters)
        {
            _shipParameters = shipParameters;
            // Розрахунок мінімального радіусу повороту на основі параметрів судна
            // R = V / (ω * √(1 - (V²/g*R)))
            // де V - швидкість, ω - кутова швидкість, g - прискорення вільного падіння
            _minTurnRadius = CalculateMinTurnRadius();
        }

        private double CalculateMinTurnRadius()
        {
            const double g = 9.81; // прискорення вільного падіння
            double v = _shipParameters.MaxSpeed;
            double omega = _shipParameters.TurnRate;

            // Ітеративний метод для розв'язання рівняння
            double r = v / omega; // початкове наближення
            for (int i = 0; i < 10; i++)
            {
                r = v / (omega * Math.Sqrt(1 - (v * v) / (g * r)));
            }

            // Додаємо 20% запасу для безпеки
            return r * 1.2;
        }

        public List<RoutePoint> OptimizeTurn(RoutePoint start, RoutePoint middle, RoutePoint end)
        {
            var optimizedPoints = new List<RoutePoint>();

            // Розрахунок векторів для вхідного та вихідного напрямків
            var inVector = new Vector(middle.X - start.X, middle.Y - start.Y);
            var outVector = new Vector(end.X - middle.X, end.Y - middle.Y);

            // Розрахунок кута повороту
            double turnAngle = Vector.AngleBetween(inVector, outVector);

            // Якщо кут повороту менший за мінімально можливий, повертаємо оригінальні точки
            if (Math.Abs(turnAngle) <= _shipParameters.TurnRate)
            {
                optimizedPoints.Add(start);
                optimizedPoints.Add(middle);
                optimizedPoints.Add(end);
                return optimizedPoints;
            }

            // Розрахунок точок входу і виходу з повороту
            var (entryPoint, exitPoint) = CalculateTurnPoints(start, middle, end, turnAngle);

            // Генерація точок дуги повороту
            var arcPoints = GenerateArcPoints(entryPoint, middle, exitPoint, turnAngle);

            // Формування фінального маршруту
            optimizedPoints.Add(start);
            optimizedPoints.Add(entryPoint);
            optimizedPoints.AddRange(arcPoints);
            optimizedPoints.Add(exitPoint);
            optimizedPoints.Add(end);

            return optimizedPoints;
        }

        private (RoutePoint entry, RoutePoint exit) CalculateTurnPoints(
            RoutePoint start,
            RoutePoint middle,
            RoutePoint end,
            double turnAngle)
        {
            // Розрахунок відстані від точки повороту до точок входу/виходу
            double turnDistance = _minTurnRadius * Math.Tan(turnAngle / 4);

            // Нормалізовані вектори напрямку
            var inDirection = Vector.Normalize(new Vector(middle.X - start.X, middle.Y - start.Y));
            var outDirection = Vector.Normalize(new Vector(end.X - middle.X, end.Y - middle.Y));

            // Розрахунок точок входу і виходу
            var entryPoint = new RoutePoint(
                middle.X - inDirection.X * turnDistance,
                middle.Y - inDirection.Y * turnDistance
            );

            var exitPoint = new RoutePoint(
                middle.X + outDirection.X * turnDistance,
                middle.Y + outDirection.Y * turnDistance
            );

            return (entryPoint, exitPoint);
        }

        private List<RoutePoint> GenerateArcPoints(
            RoutePoint entry,
            RoutePoint center,
            RoutePoint exit,
            double totalAngle)
        {
            var arcPoints = new List<RoutePoint>();

            // Розрахунок кількості точок на основі довжини дуги
            double arcLength = _minTurnRadius * Math.Abs(totalAngle);
            int numPoints = Math.Max(10, (int)(arcLength / 10)); // точка кожні 10 метрів

            // Генерація точок дуги
            for (int i = 1; i < numPoints - 1; i++)
            {
                double t = i / (double)(numPoints - 1);
                double currentAngle = totalAngle * t;

                // Матриця повороту
                double cos = Math.Cos(currentAngle);
                double sin = Math.Sin(currentAngle);

                // Розрахунок позиції точки
                var radius = new Vector(entry.X - center.X, entry.Y - center.Y);
                var rotated = new Vector(
                    radius.X * cos - radius.Y * sin,
                    radius.X * sin + radius.Y * cos
                );

                arcPoints.Add(new RoutePoint(
                    center.X + rotated.X,
                    center.Y + rotated.Y
                ));
            }

            return arcPoints;
        }
    }
}
