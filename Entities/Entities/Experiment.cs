using System;
using Tools;
using Tools.Attributes;

namespace Entities
{
    public class Experiment : EntityBase<int>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        [IgnoreProperty(EditMode.Add)]
        public int GasId { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {4}{1}Name: {0}{1}Description: {2}{1}GasId: {3}", Name, Environment.NewLine,
                Description, GasId, Id);
        }
    }
}
