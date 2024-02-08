namespace Entities.Entities
{
    public class RoutePoint : EntityBase<int>
    {
        public double X { get; set; }

        public double Y { get; set; }

        public int RouteId { get; set; }
    }
}
