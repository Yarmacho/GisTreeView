namespace Entities.Entities
{
    public class ExperimentEnvironment : EntityBase<int>
    {
        public int ExperimentId { get; set; }
        public double ReflectionCoef { get; set; }
    }
}
