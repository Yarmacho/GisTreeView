using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Database.Abstractions
{
    public interface IShapesManager
    {
        void DeleteAllShapes(string shapesPath);
    }
}
