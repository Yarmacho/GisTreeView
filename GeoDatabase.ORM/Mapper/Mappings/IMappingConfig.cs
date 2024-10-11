﻿using MapWinGIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoDatabase.ORM.Mapper.Mappings
{
    public interface IMappingConfig<T>
    {
        Shapefile Shapefile { get; }
    }
}
