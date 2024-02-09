using AxMapWinGIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp4.TreeNodes.Abstractions
{
    public interface INodeWithMap
    {
        AxMap Map { get; }

        void SetMap(AxMap map);
    }
}
