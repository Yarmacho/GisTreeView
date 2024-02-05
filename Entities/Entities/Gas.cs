using DynamicForms.Attributes;

namespace Entities.Entities
{
    public class Gas : EntityBase<int>
    {
        public string Name { get; set; }

        [IgnoreProperty(EditMode.Add)]
        public int ExperimentId { get; set; }
    }
}
