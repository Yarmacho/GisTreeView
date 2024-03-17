using GeoDatabase.ORM.Mapper;
using GeoDatabase.ORM.Set;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GeoDatabase.ORM.QueryBuilder
{
    internal class ShapesQueryProvider : IShapesQueryProvider
    {
        private readonly MappingConfigs _configs;
        private readonly IShapesMapper _shapesMapper;

        public ShapesQueryProvider(MappingConfigs configs, IShapesMapper shapesMapper)
        {
            _configs = configs;
            _shapesMapper = shapesMapper;
        }

        public IShapesQueryable<TElement> CreateQuery<TElement>(Expression<Func<TElement, bool>> expression) where TElement : new()
        {
            return new ShapesQueryable<TElement>(expression, this);
        }

        public IEnumerable<TResult> Execute<TResult>(Expression<Func<TResult, bool>> expression) where TResult : new()
        {
            var config = _configs.GetConfig<TResult>();

            var builder = new QueryBuilder(config);
            var query = builder.Compile(expression);
            if (string.IsNullOrEmpty(query))
            {
                query = "1=1";
            }

            object result = new object();
            string error = string.Empty;
            if (!config.Shapefile.Table.Query(query, ref result, ref error))
            {
                if (error != "Selection is empty")
                {
                    throw new System.Exception(error);
                }
                result = new List<int>();
            }

            var ids = result as IEnumerable<int>;
            if (ids == null)
            {
                yield break;
            }

            foreach (var id in ids)
            {
                yield return _shapesMapper.Map<TResult>(id);
            }
        }
    }
}
