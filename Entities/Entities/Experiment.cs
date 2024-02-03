using DynamicForms.Attributes;

namespace Entities
{
    public class Experiment : EntityBase<int>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        [IgnoreProperty(EditMode.Add)]
        public int GasId { get; set; }
    }
}
