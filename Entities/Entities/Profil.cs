using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Entities
{
    public class Profil : EntityBase<int>
    {
        public int ExperimentId { get; set; }
        public double Depth { get; set; }
        public double Temperature { get; set; }
        public double SoundSpeed { get; set; }
        public double Salinity { get; set; }
        public double Absorbsion { get; set; }
    }
}
