namespace Entities.Entities
{
    public class RoutePoint : EntityBase<int>
    {
        public int RouteId { get; set; }

        public double X { get; set; }

        public double Y { get; set; }
    }
}
