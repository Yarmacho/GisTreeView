using System.Collections.Generic;

namespace Entities
{
    public abstract class DictionaryEntity<TId> : EntityBase<TId>
    {
        public abstract IEnumerable<string> AsColumns();
        public abstract object[] AsDataRow();
    }
}
