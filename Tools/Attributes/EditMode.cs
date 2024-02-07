using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    [Flags]
    public enum EditMode
    {
        View = 1,
        Add = 2,
        Edit = 4,
        Delete = 8
    }
}
