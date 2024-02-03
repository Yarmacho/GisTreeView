using MapWinGIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp4.ShapeFactories
{
    public interface IShapesFactory
    {
        void BeginCraete();

        bool AddPoint(double x, double y, double z = default);

        Shape EndCreate();
    }
}
