using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public interface IEntityWithCoordinates
    {
        double X { get; set; }

        double Y { get; set; }
    }
}
