using Entities.Entities;
using System;
using System.Collections.Generic;
using WindowsFormsApp4.Initializers;

namespace WindowsFormsApp4.Logic
{
    public class TimeOffsetCalculator
    {
        private readonly ShipParameters _shipParams;
        private readonly Map _map;

        public TimeOffsetCalculator(ShipParameters shipParameters, Map map)
        {
            _shipParams = shipParameters;
            _map = map;
        }

        /// <summary>
        /// Розрахувати швидкість та часове зміщення для кожної точки маршруту
        /// </summary>
        /// <param name="routePoints">Список точок маршруту</param>
        /// <param name="initialSpeed">Початкова швидкість судна (м/с)</param>
        /// <returns>Список точок маршруту з розрахованими значеннями</returns>
        public List<RoutePoint> CalculateRouteParameters(List<RoutePoint> routePoints, double initialSpeed = 0)
        {
            if (routePoints == null || routePoints.Count <= 1)
            {
                return routePoints ?? new List<RoutePoint>();
            }

            // Перевіряємо, що початкова швидкість не перевищує максимальну
            initialSpeed = Math.Min(initialSpeed, _shipParams.MaxSpeed);

            // Ініціалізуємо першу точку
            routePoints[0].Speed = initialSpeed;
            routePoints[0].TimeOffset = TimeSpan.Zero;
            initEnvProperties(routePoints[0]);

            // Остання точка завжди має швидкість 0 (зупинка)
            routePoints[routePoints.Count - 1].Speed = 0;

            // ЕТАП 1: Прямий прохід - встановлення максимально можливих швидкостей
            // з урахуванням обмежень на поворотах
            CalculateForwardSpeedLimits(routePoints);

            // ЕТАП 2: Зворотний прохід - встановлення обмежень швидкості для гарантованої зупинки
            // в кінцевій точці та перед різкими поворотами
            CalculateBackwardSpeedLimits(routePoints);

            // ЕТАП 3: Додатковий прямий прохід для перевірки обмежень прискорення
            // (додаткова перевірка для уникнення нереалістичного прискорення)
            for (int i = 1; i < routePoints.Count; i++)
            {
                RoutePoint prevPoint = routePoints[i - 1];
                RoutePoint currentPoint = routePoints[i];

                double distance = _map.AxMap.GeodesicDistance(prevPoint.X, prevPoint.Y, currentPoint.X, currentPoint.Y);

                // Максимальна швидкість, яку можна досягти з урахуванням прискорення
                double maxAchievableSpeed = Math.Sqrt(Math.Pow(prevPoint.Speed, 2) + 2 * _shipParams.Acceleration * distance);

                // Якщо поточна швидкість більша за досяжну, зменшуємо її
                if (currentPoint.Speed > maxAchievableSpeed)
                {
                    currentPoint.Speed = maxAchievableSpeed;
                }

                initEnvProperties(currentPoint);
            }

            // ЕТАП 4: Фінальний зворотний прохід для гарантованої зупинки
            // (додаткова перевірка з підвищеним запасом безпеки)
            for (int i = routePoints.Count - 2; i >= 0; i--)
            {
                RoutePoint currentPoint = routePoints[i];
                RoutePoint nextPoint = routePoints[i + 1];

                double distance = _map.AxMap.GeodesicDistance(currentPoint.X, currentPoint.Y, nextPoint.X, nextPoint.Y);

                // Використовуємо більш консервативне значення уповільнення для останнього сегмента
                double safetyFactor = (i == routePoints.Count - 2) ? 0.7 : 0.9;
                double safeDeceleration = _shipParams.Deceleration * safetyFactor;

                // Максимальна швидкість для безпечного гальмування
                double maxSafeSpeed = Math.Sqrt(Math.Pow(nextPoint.Speed, 2) + 2 * safeDeceleration * distance);

                // Якщо поточна швидкість більша за безпечну, зменшуємо її
                if (currentPoint.Speed > maxSafeSpeed)
                {
                    currentPoint.Speed = maxSafeSpeed;
                }
            }

            // ЕТАП 5: Розрахунок часових зміщень
            CalculateTimeOffsets(routePoints);

            return routePoints;
        }

        private void initEnvProperties(RoutePoint point)
        {
            if (_map.TryGetDepth(point.X, point.Y, out var depth))
            {
                point.Depth = depth;
            }

            if (_map.TryGetTemperature(point.X, point.Y, out var temperature))
            {
                point.Temperature = temperature;
            }

            if (_map.TryGetSalinity(point.X, point.Y, out var salinity))
            {
                point.Salinity = salinity;
            }
        }

        /// <summary>
        /// Прямий прохід для встановлення максимальних швидкостей з урахуванням прискорення
        /// та обмежень на поворотах
        /// </summary>
        private void CalculateForwardSpeedLimits(List<RoutePoint> routePoints)
        {
            double currentSpeed = routePoints[0].Speed;

            for (int i = 1; i < routePoints.Count - 1; i++) // Не включаємо останню точку, яка має швидкість 0
            {
                RoutePoint prevPoint = routePoints[i - 1];
                RoutePoint currentPoint = routePoints[i];

                // Розраховуємо відстань до попередньої точки
                double distance = _map.AxMap.GeodesicDistance(prevPoint.X, prevPoint.Y, currentPoint.X, currentPoint.Y);

                // Розраховуємо кут повороту
                double turnAngle = 0;
                if (i > 1)
                {
                    turnAngle = CalculateTurnAngle(routePoints[i - 2], prevPoint, currentPoint);
                }

                // Визначаємо обмеження швидкості через кут повороту
                double speedReductionFactor = CalculateSpeedReductionForTurn(turnAngle);
                double maxSpeedAtPoint = _shipParams.MaxSpeed * speedReductionFactor;

                // Також перевіряємо наступний поворот, щоб почати зниження швидкості завчасно
                if (i < routePoints.Count - 2)
                {
                    // Кут повороту між поточною, наступною і наступною за наступною точками
                    double nextTurnAngle = CalculateTurnAngle(prevPoint, currentPoint, routePoints[i + 1]);

                    // Відстань до наступної точки
                    double distanceToNextPoint = _map.AxMap.GeodesicDistance(
                        currentPoint.X, currentPoint.Y,
                        routePoints[i + 1].X, routePoints[i + 1].Y);

                    // Зниження швидкості для наступного повороту
                    double nextSpeedReduction = CalculateSpeedReductionForTurn(nextTurnAngle);
                    double maxSpeedForNextTurn = _shipParams.MaxSpeed * nextSpeedReduction;

                    // Якщо наступний поворот різкий, заздалегідь знижуємо швидкість
                    // тим сильніше, чим більше кут і чим менша відстань
                    if (nextTurnAngle > 0.3) // приблизно 17 градусів
                    {
                        // Коефіцієнт зниження залежно від відстані до повороту
                        // Чим менша відстань, тим сильніше зниження
                        double distanceFactor = Math.Min(1.0, distanceToNextPoint / (5 * _shipParams.Length));

                        // Поступове зниження швидкості при наближенні до повороту
                        double anticipatedMaxSpeed = maxSpeedForNextTurn +
                                                    (maxSpeedAtPoint - maxSpeedForNextTurn) * distanceFactor;

                        // Застосовуємо обмеження
                        maxSpeedAtPoint = Math.Min(maxSpeedAtPoint, anticipatedMaxSpeed);
                    }
                }

                // Розраховуємо досяжну швидкість з урахуванням прискорення
                // v^2 = v0^2 + 2*a*s (формула рівноприскореного руху)
                double achievableSpeed = Math.Sqrt(Math.Pow(currentSpeed, 2) + 2 * _shipParams.Acceleration * distance);

                // Обмежуємо досяжну швидкість максимально допустимою в цій точці
                achievableSpeed = Math.Min(achievableSpeed, maxSpeedAtPoint);

                // Оновлюємо швидкість у точці
                currentPoint.Speed = achievableSpeed;
                currentSpeed = achievableSpeed;
            }
        }

        /// <summary>
        /// Зворотний прохід для встановлення обмежень швидкості, 
        /// щоб гарантувати можливість зупинки та уповільнення перед поворотами
        /// </summary>
        private void CalculateBackwardSpeedLimits(List<RoutePoint> routePoints)
        {
            // Починаємо з передостанньої точки і рухаємося до початку
            for (int i = routePoints.Count - 2; i >= 0; i--)
            {
                RoutePoint currentPoint = routePoints[i];
                RoutePoint nextPoint = routePoints[i + 1];

                // Розраховуємо відстань до наступної точки
                double distance = _map.AxMap.GeodesicDistance(currentPoint.X, currentPoint.Y, nextPoint.X, nextPoint.Y);

                // Розраховуємо максимально допустиму швидкість у поточній точці,
                // щоб мати можливість уповільнитися до швидкості в наступній точці
                // Використовуємо формулу v^2 = v_next^2 + 2*a*s, де:
                // v - швидкість у поточній точці
                // v_next - швидкість у наступній точці
                // a - уповільнення
                // s - відстань

                // Для безпеки множимо уповільнення на коефіцієнт 0.9, щоб гарантовано зупинитися
                double safeDeceleration = _shipParams.Deceleration * 0.9;

                // Якщо наступна точка має швидкість 0, додаємо невеликий запас відстані для гарантованої зупинки
                if (nextPoint.Speed < 0.01)
                {
                    // Зменшуємо ефективну відстань на 10% для гарантії зупинки
                    distance = Math.Max(0, distance * 0.9);
                }

                // Розрахунок максимальної швидкості для безпечного уповільнення
                double maxSpeedForDeceleration = Math.Sqrt(Math.Pow(nextPoint.Speed, 2) + 2 * safeDeceleration * distance);

                // Обмежуємо швидкість у поточній точці
                currentPoint.Speed = Math.Min(currentPoint.Speed, maxSpeedForDeceleration);
            }
        }

        /// <summary>
        /// Розрахунок часових зміщень на основі встановлених швидкостей
        /// </summary>
        private void CalculateTimeOffsets(List<RoutePoint> routePoints)
        {
            for (int i = 1; i < routePoints.Count; i++)
            {
                RoutePoint prevPoint = routePoints[i - 1];
                RoutePoint currentPoint = routePoints[i];

                // Відстань між точками
                double distance = _map.AxMap.GeodesicDistance(prevPoint.X, prevPoint.Y, currentPoint.X, currentPoint.Y);

                // Для останньої точки особливо ретельно перевіряємо можливість зупинки
                if (i == routePoints.Count - 1)
                {
                    // Розраховуємо мінімальну відстань, потрібну для зупинки з поточної швидкості
                    double minStopDistance = Math.Pow(prevPoint.Speed, 2) / (2 * _shipParams.Deceleration);

                    // Якщо відстань недостатня для повної зупинки, коригуємо швидкість попередньої точки
                    if (minStopDistance > distance)
                    {
                        // Розраховуємо максимальну швидкість, з якої можна зупинитися на цій відстані
                        prevPoint.Speed = Math.Sqrt(2 * _shipParams.Deceleration * distance * 0.9);
                        Console.WriteLine($"WARNING: Adjusted pre-final speed from {prevPoint.Speed} to {Math.Sqrt(2 * _shipParams.Deceleration * distance * 0.9)} m/s to ensure stop");
                    }
                }

                // Розрахунок часу сегменту з урахуванням прискорення/уповільнення
                TimeSpan segmentTime = CalculateSegmentTime(prevPoint.Speed, currentPoint.Speed, distance);

                // Додаємо час до загального зміщення
                currentPoint.TimeOffset = prevPoint.TimeOffset.Add(segmentTime);
            }
        }

        /// <summary>
        /// Розрахувати кут повороту між трьома послідовними точками
        /// </summary>
        private double CalculateTurnAngle(RoutePoint point1, RoutePoint point2, RoutePoint point3)
        {
            // Розрахунок векторів
            double vector1X = point2.X - point1.X;
            double vector1Y = point2.Y - point1.Y;
            double vector2X = point3.X - point2.X;
            double vector2Y = point3.Y - point2.Y;

            // Нормалізація векторів
            double magnitude1 = Math.Sqrt(vector1X * vector1X + vector1Y * vector1Y);
            double magnitude2 = Math.Sqrt(vector2X * vector2X + vector2Y * vector2Y);

            // Уникаємо ділення на нуль
            if (magnitude1 < 0.000001 || magnitude2 < 0.000001)
            {
                return 0;
            }

            // Нормалізовані вектори
            double norm1X = vector1X / magnitude1;
            double norm1Y = vector1Y / magnitude1;
            double norm2X = vector2X / magnitude2;
            double norm2Y = vector2Y / magnitude2;

            // Скалярний добуток нормалізованих векторів
            double dotProduct = norm1X * norm2X + norm1Y * norm2Y;

            // Векторний добуток для визначення знаку повороту
            double crossProduct = norm1X * norm2Y - norm1Y * norm2X;

            // Обмежуємо значення dotProduct до діапазону [-1, 1]
            dotProduct = Math.Max(-1, Math.Min(1, dotProduct));

            // Розрахунок кута між векторами
            double angle = Math.Acos(dotProduct);

            // Знак кута (додатний - поворот ліворуч, від'ємний - праворуч)
            // Але для спрощення повертаємо абсолютне значення, оскільки 
            // для зниження швидкості важлива тільки величина кута
            return Math.Abs(angle);
        }

        /// <summary>
        /// Розрахувати коефіцієнт зниження швидкості для кута повороту
        /// </summary>
        private double CalculateSpeedReductionForTurn(double turnAngle)
        {
            if (turnAngle <= 0.01)
            {
                return 1.0; // Немає зниження для прямого руху
            }

            // Враховуємо обмеження маневреності судна (_shipParams.TurnRate)
            // Розраховуємо максимальну швидкість, на якій човен може зробити поворот з даним кутом

            // Чим більший човен, тим більше зниження швидкості на поворотах
            double sizeFactor = Math.Max(0.3, Math.Min(1.0, 10.0 / _shipParams.Length));

            // Базове зниження швидкості на основі кута
            // Використовуємо квадратичну залежність для більш різкого зниження при великих кутах
            double baseReduction = 1.0 - Math.Pow(turnAngle / Math.PI, 1.5);

            // Врахування здатності човна до повороту (коефіцієнт TurnRate)
            // Більший TurnRate - краща маневреність - менше зниження швидкості
            double turnRateFactor = Math.Min(2.0, Math.Max(0.2, _shipParams.TurnRate * 20));

            // Комбінуємо всі фактори
            // 1. Базове зниження на основі кута
            // 2. Коригування на основі маневреності
            // 3. Коригування на основі розміру човна
            double finalReduction = baseReduction * turnRateFactor * sizeFactor;

            // Обмежуємо коефіцієнт зверху і знизу
            // Мінімальне зниження 10% навіть для невеликих поворотів
            // Максимальне зниження до 10% від максимальної швидкості для дуже різких поворотів
            return Math.Max(0.1, Math.Min(0.9, finalReduction));
        }

        /// <summary>
        /// Розрахувати досяжну швидкість з урахуванням обмежень прискорення/уповільнення
        /// </summary>
        private double CalculateAchievableSpeed(double currentSpeed, double targetSpeed, double distance)
        {
            // Визначаємо, чи потрібно прискорюватись чи уповільнюватись
            bool needAcceleration = targetSpeed > currentSpeed;

            if (needAcceleration)
            {
                // Розраховуємо час і відстань, необхідні для прискорення
                double timeToAccelerate = (targetSpeed - currentSpeed) / _shipParams.Acceleration;
                double accelerationDistance = currentSpeed * timeToAccelerate +
                                             0.5 * _shipParams.Acceleration * timeToAccelerate * timeToAccelerate;

                // Якщо відстань недостатня для досягнення цільової швидкості
                if (accelerationDistance > distance * 0.7) // Залишаємо 30% відстані на рух з постійною швидкістю
                {
                    // Розраховуємо максимально досяжну швидкість на даній відстанці
                    // Використовуємо рівняння руху: v^2 = v0^2 + 2*a*s
                    double achievableSpeed = Math.Sqrt(Math.Pow(currentSpeed, 2) +
                                                      2 * _shipParams.Acceleration * distance * 0.7);
                    return Math.Min(targetSpeed, achievableSpeed);
                }
            }
            else // Потрібно уповільнитись
            {
                // Розраховуємо час і відстань для уповільнення
                double timeToDecelerate = (currentSpeed - targetSpeed) / _shipParams.Deceleration;
                double decelerationDistance = targetSpeed * timeToDecelerate +
                                             0.5 * _shipParams.Deceleration * timeToDecelerate * timeToDecelerate;

                // Якщо відстань недостатня для досягнення цільової швидкості
                if (decelerationDistance > distance)
                {
                    // Визначаємо мінімальну швидкість, до якої можемо уповільнитись
                    // v^2 = v0^2 - 2*a*s
                    double minAchievableSpeed = Math.Sqrt(Math.Max(0, Math.Pow(currentSpeed, 2) -
                                                                    2 * _shipParams.Deceleration * distance));
                    return Math.Max(targetSpeed, minAchievableSpeed);
                }
            }

            return targetSpeed; // Повертаємо цільову швидкість, якщо вона досяжна
        }

        /// <summary>
        /// Розрахувати час проходження сегменту з урахуванням прискорення і уповільнення
        /// </summary>
        private TimeSpan CalculateSegmentTime(double startSpeed, double endSpeed, double distance)
        {
            // Особливий випадок - обидві швидкості близькі до нуля
            if (startSpeed < 0.01 && endSpeed < 0.01)
            {
                return TimeSpan.FromSeconds(0);
            }

            // Якщо кінцева швидкість нульова (зупинка), використовуємо спеціальну формулу
            if (endSpeed < 0.01)
            {
                // Час, необхідний для повної зупинки з початкової швидкості
                double timeToStop = startSpeed / _shipParams.Deceleration;

                // Відстань, потрібна для зупинки
                double distanceToStop = (startSpeed * startSpeed) / (2 * _shipParams.Deceleration);

                // Перевіряємо, чи достатня відстань для повної зупинки
                if (distanceToStop > distance)
                {
                    // Якщо недостатня, розраховуємо час руху по формулі квадратного рівняння
                    // s = v0*t - 0.5*a*t^2, розв'язуємо відносно t
                    double a = 0.5 * _shipParams.Deceleration;
                    double b = -startSpeed;
                    double c = distance;

                    double discriminant = b * b - 4 * a * c;

                    // Якщо дискримінант від'ємний (що не повинно бути), використовуємо спрощену формулу
                    if (discriminant < 0)
                    {
                        return TimeSpan.FromSeconds(2 * distance / startSpeed);
                    }

                    // Беремо додатній корінь
                    double t = (-b - Math.Sqrt(discriminant)) / (2 * a);
                    return TimeSpan.FromSeconds(Math.Max(0, t));
                }

                return TimeSpan.FromSeconds(timeToStop);
            }

            // Якщо швидкість не змінюється суттєво, просто ділимо відстань на середню швидкість
            if (Math.Abs(startSpeed - endSpeed) < 0.1)
            {
                // Уникаємо ділення на нуль
                double avgSpeed = Math.Max(0.1, (startSpeed + endSpeed) / 2);
                return TimeSpan.FromSeconds(distance / avgSpeed);
            }

            // Визначаємо, чи потрібно прискорюватись чи уповільнюватись
            bool isAccelerating = endSpeed > startSpeed;

            // Отримуємо відповідне прискорення/уповільнення
            double acceleration = isAccelerating ? _shipParams.Acceleration : -_shipParams.Deceleration;

            // Час для зміни швидкості від startSpeed до endSpeed з даним прискоренням
            double timeToChangeSpeed = Math.Abs(endSpeed - startSpeed) / Math.Abs(acceleration);

            // Відстань, пройдена за цей час
            // s = v0*t + 0.5*a*t^2
            double distanceForSpeedChange = startSpeed * timeToChangeSpeed +
                                           0.5 * acceleration * timeToChangeSpeed * timeToChangeSpeed;

            // Перевіряємо, чи подолали ми всю відстань за час зміни швидкості
            if (Math.Abs(distanceForSpeedChange) >= distance)
            {
                // Якщо ми не можемо досягти кінцевої швидкості на даній відстані,
                // розраховуємо час руху, розв'язуючи квадратне рівняння
                // s = v0*t + 0.5*a*t^2
                double a = 0.5 * acceleration;
                double b = startSpeed;
                double c = -distance;

                double discriminant = b * b - 4 * a * c;

                // Якщо дискримінант від'ємний (що не повинно бути), використовуємо спрощену формулу
                if (discriminant < 0)
                {
                    return TimeSpan.FromSeconds(2 * distance / (startSpeed + endSpeed));
                }

                // Вибираємо правильний корінь (той, що дає додатній час)
                double t1 = (-b + Math.Sqrt(discriminant)) / (2 * a);
                double t2 = (-b - Math.Sqrt(discriminant)) / (2 * a);

                double t = (t1 > 0) ? t1 : t2;
                return TimeSpan.FromSeconds(Math.Max(0, t));
            }

            // Якщо після зміни швидкості залишається відстань, додаємо час руху з постійною швидкістю
            double remainingDistance = distance - Math.Abs(distanceForSpeedChange);
            double timeWithConstantSpeed = remainingDistance / Math.Abs(endSpeed);

            return TimeSpan.FromSeconds(timeToChangeSpeed + timeWithConstantSpeed);
        }
    }
}
