using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public class ProfilDto
    {
        public int SceneId { get; set; }
        public double Depth { get; set; }
        public double Temperature { get; set; }
        public double SoundSpeed { get; set; }
        public double Salinity { get; set; }
        public double Absorbsion { get; set; }
    }
}
