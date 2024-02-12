using System;
using Tools;
using Tools.Attributes;

namespace Entities.Entities
{
    public class Scene : EntityBase<int>
    {
        public string Name { get; set; }

        [IgnoreProperty(EditMode.Add)]
        public int GasId { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {3}{1}Name: {0}{1}GasId: {2}{1}", Name, Environment.NewLine,
                GasId, Id);
        }
    }
}
