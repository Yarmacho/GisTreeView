using MapWinGIS;
using System;
using System.Linq.Expressions;

namespace GeoDatabase.ORM.Mapper.Mappings.Builder
{
    public interface IMappingBuilder<T>
    {
        IMappingPropertyBuilder<T, TProperty> Property<TProperty>(Expression<Func<T, TProperty>> propertyExpression);
        IMappingBuilder<T> ToShapeFile(string path);
        IMappingBuilder<T> ToShapeFile(Shapefile shapefile);
    }
}
