using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Experiment : EntityBase<int>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int GasId { get; set; }
    }
}
